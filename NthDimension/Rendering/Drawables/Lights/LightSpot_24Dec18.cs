/* LICENSE
 * Copyright (C) 2008 - 2018 SYSCON Technologies, Hellas - All Rights Reserved
 * Written by Stylianos N. Polychroniadis (info@polytronic.gr) http://www.polytronic.gr
 * 
 * This file is part of nthDimension Platform
 * 
 * WARNING! Commercial Software, All Use Must Be Licensed
 * This software is protected by Hellenic Copyright Law and International Treaties. 
 * Unauthorized use, duplication, reverse engineering, any form of redistribution, or 
 * use in part or in whole other than by prior, express, printed and signed license 
 * for use is subject to civil and criminal prosecution. 
*/

using NthDimension.Rendering.Configuration;
using NthDimension.Rendering.Drawables.Models;
using NthDimension.Rendering.GameViews;
using NthDimension.Rendering.Shaders;
using NthDimension.Rendering.Utilities;

namespace NthDimension.Rendering.Drawables.Lights
{
    using System;
    using System.Text;
    using System.Xml;
    using Rendering.Configuration;
    using Rendering.Drawables.Models;
    using Rendering.Shaders;
    using NthDimension.Rasterizer;

    //using OpenGL.GLSL.API3x;

    using NthDimension.Algebra;

#if _WINDOWS_
    //using OpenGL.Windows.GLSL.API3x;
#endif

    public class LightSpot : Light
    {
        public int          ProjectionTexture = 0;
        private float       nextFarUpdate;
        private string      texturename;
        private bool        useProjectionTexture = false;  // was false

        public int          lightId;

        new public static string nodename = "lamp";

        public LightSpot(ApplicationObject parent)
        {
            Parent = (Drawable)parent;

            

            Name = Scene.GetUniqueName();

            IgnoreCulling = true;

            setupShadow();

            Parent.forceUpdate();

            createRenderObject();

            shadowQuality = Settings.Instance.video.shadowQuality;

            //this.Texture = @"temp\fishtexture.png";

            Scene.Spotlights.Add(this);

            wasUpdated = true;
        }

       

        public void setupShadow()
        {
            viewInfo                = new ViewInfo(this);
            viewInfo.zNear          = 0.7f;  // was 0.7

            float zfar = (ApplicationBase.Instance.Scene.EyePos - this.Position).LengthFast;
            if (zfar < 0) zfar *= -1f;
            if (zfar < 1) zfar = 400f;
            zfar = 100;
            viewInfo.zFar           = zfar; //400f;   // was 10
            viewInfo.fovy           = /*ApplicationBase.Instance.Width / ApplicationBase.Instance.Height;//*/(float)Math.PI / 2f;
            //viewInfo.aspect         = 16 / 9f;
            viewInfo.UpdateProjectionMatrix();
        }

        #region load/save

        public override void save(ref StringBuilder sb, int level)
        {
            // converting object information to strings
            string myposition = GenericMethods.StringFromVector3(Position);
            string direction = GenericMethods.StringFromVector3(PointingDirection);
            string stringColor = GenericMethods.StringFromVector3(colorRgb);
            string mparent = Parent.Name;


            string tab = GenericMethods.tabify(level - 1);
            string tab2 = GenericMethods.tabify(level);

            sb.AppendLine(tab + "<lamp name='" + Name + "'>");
            sb.AppendLine(tab2 + "<position>" + myposition + "</position>");
            sb.AppendLine(tab2 + "<direction>" + direction + "</direction>");
            sb.AppendLine(tab2 + "<color>" + stringColor + "</color>");
            //sb.AppendLine(tab2 + "<parent>" + mparent + "'/>");

            if (Texture != null)
                sb.AppendLine(tab2 + "<texture>" + Texture + "</texture>");

            // output saving message
            Utilities.ConsoleUtil.log(string.Format("@ Saving Light: '{0}'", Name));

            sb.AppendLine(tab + "</lamp>");

            // save childs
            saveChilds(ref sb, level);
        }

        protected override void specialLoad(ref System.Xml.XmlTextReader reader, string type)
        {
            if (reader.Name == "texture" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                Texture = reader.Value;
            }
        }

        #endregion load/save

        Matrix4 lightMvp = Matrix4.Identity;
        public override void Update()
        {
            if (Position != Parent.Position)
            {
                Position = Parent.Position;
                wasUpdated = true;
            }

            if (Orientation != Parent.Orientation)
            {
                Orientation = Parent.Orientation;
                wasUpdated = true;
            }

            wasUpdated = true;

            if (wasUpdated ||
                ApplicationBase.Instance.VAR_FrameTime > nextFarUpdate)
            {

                Matrix4 lightMvp = Matrix4.Identity;

                lightMvp = viewInfo.modelviewProjectionMatrix;
                shadowMatrix = Matrix4.Mult(lightMvp, bias);
                setupShadow();

                //if (ApplicationBase.Instance.Player.PlayerViewMode == ApplicationUser.enuPlayerViewMode.FirstPerson ||
                //    ApplicationBase.Instance.Player.PlayerViewMode == ApplicationUser.enuPlayerViewMode.ThirdPersonVR)
                //{
                //    lightMvp = viewInfo.modelviewProjectionMatrix;
                //    shadowMatrix = Matrix4.Mult(lightMvp, bias);
                //    setupShadow();
                //}

                //if (ApplicationBase.Instance.Player.PlayerViewMode == ApplicationUser.enuPlayerViewMode.ThirdPerson)                // MAJOR update Dec-09-18 restores SPOTLIGTHS!!!
                //{
                //    viewInfo.Position                       = Position;
                //    viewInfo.Orientation                    = Orientation;


                //    Vector3 target                          = Position;
                //    Orientation.TransformVector(ref target);

                //    Matrix4 lightView                       = Matrix4.LookAt(Position,
                //                                                             //position + PointingDirection.Normalized(),
                //                                                             target,
                //                                                             new Vector3(0, 1, 0));

                //    viewInfo.modelviewMatrix                = ApplicationBase.Instance.Player.ViewInfo.modelviewMatrix * lightView;
                //    viewInfo.invModelviewMatrix             = lightView.Inverted();


                //    Matrix4 lightProj                       = ApplicationBase.Instance.Player.ViewInfo.projectionMatrix;

                //    viewInfo.UpdateProjectionMatrix();

                //    viewInfo.projectionMatrix = Matrix4.Mult(viewInfo.projectionMatrix, lightProj.Inverted());


                //    lightMvp = Matrix4.Mult(viewInfo.projectionMatrix,
                //                            viewInfo.modelviewMatrix);

                //    viewInfo.modelviewProjectionMatrix      = lightMvp;
                //    viewInfo.invModelviewProjectionMatrix   = lightMvp.Inverted();
                //    viewInfo.calculateVectors();

                //    shadowMatrix = Matrix4.Mult(lightMvp, bias);
                //}

                updateChilds();
                nextFarUpdate = ApplicationBase.Instance.VAR_FrameTime + 1000f;
            }

            
        }

        public override void kill()
        {
            Scene.Spotlights.Remove(this);

            Parent.removeChild(this);

            killChilds();
        }

        public override void activate(Shaders.Shader shader, Drawable drawble)
        {
            int active = 1;
            ApplicationBase.Instance.Renderer.Uniform3(shader.LightLocationsLocation[lightId], ref position);
            ApplicationBase.Instance.Renderer.Uniform3(shader.LightDirectionsLocation[lightId], ref pointingDirection);
            ApplicationBase.Instance.Renderer.Uniform3(shader.LightColorsLocation[lightId], ref colorRgb);

            ApplicationBase.Instance.Renderer.UniformMatrix4(shader.LightViewMatrixLocation[lightId], false, ref shadowMatrix);

            ApplicationBase.Instance.Renderer.Uniform1(shader.LightActiveLocation[lightId], 1, ref active);
        }

        public override void activateDeffered(Shaders.Shader shader)
        {
            shader.InsertUniform(Uniform.defPosition,   ref position);
            shader.InsertUniform(Uniform.defDirection,  ref pointingDirection);
            shader.InsertUniform(Uniform.defColor,      ref colorRgb);
            shader.InsertUniform(Uniform.in_no_lights,  ref Scene.LightCount);
            shader.InsertUniform(Uniform.curLight,      ref lightId);

            shader.InsertUniform(Uniform.defMatrix,     ref shadowMatrix);
            shader.InsertUniform(Uniform.defInvPMatrix, ref viewInfo.invModelviewProjectionMatrix);

            shader.InsertUniform(Uniform.shadowQuality, ref shadowQuality);

            if(ProjectionTexture > 0)
                ApplicationBase.Instance.Renderer.Uniform1(shader.LightTextureLocation[lightId], 1, ref ProjectionTexture);
        }

        public override Vector4 Color
        {
            get
            {
                return base.Color;
            }
            set
            {
                base.Color = value;

                Drawable tmpPar = (Drawable)Parent;
                tmpPar.EmissionColor = colorRgb;
            }
        }

        public string Texture
        {
            get { return texturename; }
            set
            {
                texturename = value;
                ProjectionTexture = ApplicationBase.Instance.TextureLoader.getTextureId(value);
                useProjectionTexture = true;
            }
        }

        private void createRenderObject()
        {
            drawable = new LightVolume(this);
            drawable.setMaterial("defShading\\lightSpot.xmf");
            //drawable.setMaterial("defShading\\volSpot.xmf");
            drawable.setMesh("base\\spotlight_cone.obj");
            drawable.IgnoreCulling = true;
            drawable.Color = new Vector4(0.6f, 0.7f, 1.0f, 1);
            drawable.IsVisible = true;

            //this.Texture = "base\\uvmap.png";     // MAR-19-18 DEBUG (tring to display Light Volume)
        }
    }
}
