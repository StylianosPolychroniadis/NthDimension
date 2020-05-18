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

namespace NthDimension.Rendering.Drawables.Lights
{
    using System.Text;
    using System.Xml;

    using NthDimension.Algebra;
    using NthDimension.Rendering.GameViews;
    using NthDimension.Rendering.Drawables.Models;
    using NthDimension.Rendering.Shaders;

#if _WINDOWS_
    //using OpenGL.Windows.GLSL.API3x;
#endif

    public sealed class LightSpot : Light
    {
        new public static string    nodename                = "lamp";
        public int                  lightId;
        public int                  ProjectionTexture       = 0;

        private float               nextFarUpdate;
        private string              texturename;
        private bool                useProjectionTexture    = false;  // was false

        public LightSpot(ApplicationObject parent)
        {
            Parent = (Drawable)parent;

            Scene.Spotlights.Add(this);

            Name = Scene.GetUniqueName();

            IgnoreCulling = true;

            setupShadow();

            Parent.forceUpdate();

            createRenderObject();

            shadowQuality = Settings.Instance.video.shadowQuality;

            //this.Texture = @"temp\fishtexture.png";
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


        private void setupShadow()
        {
            viewInfo                = new ViewInfo(this);
            viewInfo.zNear          = 0.7f;  // was 0.7
            viewInfo.zFar           = 1500f;
            viewInfo.UpdateProjectionMatrix();

            // NOTE FOVY was set to PI/2 in Original
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
            sb.AppendLine(tab2 + "<near>" + viewInfo.zNear + "</near>");
            sb.AppendLine(tab2 + "<far>" + viewInfo.zNear + "</far>");

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

            // TODO:: zNear & zFar
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

            if (wasUpdated //) 
                || ApplicationBase.Instance.VAR_FrameTime > nextFarUpdate)
            {
                updateChilds();

                viewInfo.zNear                          = 0.7f;
                viewInfo.zFar = 100f;// 10f;
                viewInfo.Position                       = position;
                viewInfo.pointingDirection              = PointingDirection;
                viewInfo.wasUpdated = true;
                
                //// ------------------------------------------------------------------------------------------------------------------------
                //// Works but modifying the light translation/rotation kills the light
                //Matrix4 lookAt                          = Matrix4.LookAt(position, position + PointingDirection, new Vector3(0, 1f, 0)); ;
                //viewInfo.modelviewMatrix                = Matrix4.Mult(lookAt, ApplicationBase.Instance.Player.ViewInfo.modelviewMatrix);
                //viewInfo.invModelviewMatrix             = Matrix4.Mult(lookAt, ApplicationBase.Instance.Player.ViewInfo.invModelviewMatrix);

                //viewInfo.projectionMatrix               = Matrix4.CreatePerspectiveFieldOfView(viewInfo.fovy, ApplicationBase.Instance.Width/ApplicationBase.Instance.Height, viewInfo.zNear, viewInfo.zFar);

                //viewInfo.modelviewProjectionMatrix      = Matrix4.Mult(viewInfo.modelviewMatrix, viewInfo.projectionMatrix);                
                //viewInfo.invModelviewProjectionMatrix   = Matrix4.Invert(viewInfo.modelviewProjectionMatrix);
                //// ------------------------------------------------------------------------------------------------------------------------

                Matrix4 lightMvp                        = viewInfo.modelviewProjectionMatrix;
             
                shadowMatrix                            = Matrix4.Mult(lightMvp, shadowBias);
             
                nextFarUpdate                           = ApplicationBase.Instance.VAR_FrameTime + Settings.Instance.video.effectsUpdateInterval;

                //ConsoleUtil.log(string.Format("Spotlight updated {0}", DateTime.Now.ToString("hh:mm:ss.fff")));
                //wasUpdated = false;
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
            int active = 0;
            if (viewInfo.frustrumCheck(drawble))
            {
                ApplicationBase.Instance.Renderer.Uniform3(shader.LightLocationsLocation[lightId], ref position);
                ApplicationBase.Instance.Renderer.Uniform3(shader.LightDirectionsLocation[lightId], ref pointingDirection);
                ApplicationBase.Instance.Renderer.Uniform3(shader.LightColorsLocation[lightId], ref colorRgb);

                ApplicationBase.Instance.Renderer.UniformMatrix4(shader.LightViewMatrixLocation[lightId], false, ref shadowMatrix);
                active = 1;
            }

            ApplicationBase.Instance.Renderer.Uniform1(shader.LightActiveLocation[lightId], 1,                  ref active);
        }

        public override void activateDeferred(Shaders.Shader shader)
        {
            shader.InsertUniform(Uniform.defPosition,   ref position);              // ORIGINAL
            shader.InsertUniform(Uniform.defDirection,  ref pointingDirection);     // ORIGINAL
            shader.InsertUniform(Uniform.defColor,      ref colorRgb);
            shader.InsertUniform(Uniform.defMatrix,     ref shadowMatrix);
            shader.InsertUniform(Uniform.defInvPMatrix, ref viewInfo.invModelviewProjectionMatrix);
            
            shader.InsertUniform(Uniform.in_no_lights,  ref Scene.LightCount);
            shader.InsertUniform(Uniform.curLight,      ref lightId);

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

        
    }
}
