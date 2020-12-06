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

using Shader = NthDimension.Rendering.Shaders.Shader;
using System;
using System.Text;

using NthDimension.Algebra;
using NthDimension.Rendering.GameViews;
using NthDimension.Rendering.Geometry;
using NthDimension.Rendering.Scenegraph;
using NthDimension.Rendering.Shaders;
using System.Xml;

//using NthDimension.OpenGL.GLSL.API3x;

#if _WINDOWS_
//using NthDimension.OpenGL.Windows.GLSL.API3x;
#endif

namespace NthDimension.Rendering.Drawables.Models
{
    public class Skybox : Model
    {
        //1 - FRONT
        //2 - LEFT
        //3 - BACK
        //4 - RIGHT
        //5 - UP
        //6 - DOWN

        new public static string    nodename = "skybox";
        private readonly string[]   _defaultSkybox = new string[6]
        {
            @"sky1.xmf",
            @"sky2.xmf",
            @"sky3.xmf",
            @"sky4.xmf",
            @"sky5.xmf",
            @"sky6.xmf"
        };

        string[] skymats = new string[6];

        private bool m_assetsInitialized = false;

        public Skybox(ApplicationObject obj):this(obj as SceneGame)
        {
            // Ugly hack
        }

        public Skybox(SceneGame mScene) : base(mScene)
        {
            //if (skymats.Length != 6)
            //    throw new ArgumentException("Skybox materials, wrong number of materials <> 6");

            
#if _DEVUI_
            return;
#endif

            Parent = mScene;
            this.Scene = mScene;
            mScene.AddDrawable(this);

            float skyScale = NthDimension.Settings.Instance.view.CullZFar / (float)Math.Sqrt(3);

            Size = new Vector3(skyScale, skyScale, skyScale);
        }

        public override void save(ref System.Text.StringBuilder sb, int level)
        {
            // reading Object Atrributes and Converting them to Strings
            string front    = Materials[0];
            string back     = Materials[2];
            string left     = Materials[1];
            string right    = Materials[3];
            string up       = Materials[4];
            string down     = Materials[5];

            string tab = GenericMethods.tabify(level - 1);
            string tab2 = GenericMethods.tabify(level);

            sb.AppendLine(tab + "<" + nodename + " name='" + Name + "'>");
            sb.AppendLine(tab2 + "<front>" + front + "</front>");
            sb.AppendLine(tab2 + "<back>" + back + "</back>");
            sb.AppendLine(tab2 + "<left>" + left + "</left>");
            sb.AppendLine(tab2 + "<right>" + right + "</right>");
            sb.AppendLine(tab2 + "<up>" + up + "</up>");
            sb.AppendLine(tab2 + "<down>" + down + "</down>");

            if (IgnoreCulling)
                sb.AppendLine(tab2 + "<cullignore/>");

            /*
            // Creating Sql Command
            sb.Append("INSERT INTO WorldObjects (id, name, position, rotation , material, meshes, pboxes, static )" +
                " VALUES(NULL, '" + name + "', '" + position + "', '" + rotation + "' , '" + stringMaterial + "' , '"
                + meshes + "' , '" + pboxes + "' , " + isstatic + ");");

             */

            Utilities.ConsoleUtil.log(string.Format("@ Saving Skybox: '{0}'", Name));

            saveChilds(ref sb, level);

            sb.AppendLine(tab + "</" + nodename + ">");
        }

        public override void Update()
        {
#if _DEVUI_
            return;
#endif
            if (!m_assetsInitialized)
                this.createRenderObject();

            for (int m = 0; m < this.meshes.Length; m++)
                if(this.meshes[m].CurrentLod != MeshVbo.MeshLod.Level0)
                    this.meshes[m].CurrentLod = MeshVbo.MeshLod.Level0;

            Position = Scene.EyePos;

            updateChilds();
        }

        protected override void setSpecialUniforms(ref Shaders.Shader shader, ref MeshVbo curMesh)
        {
#if _DEVUI_
            return;
#endif
            Vector3 sunColor = Vector3.Zero;

            if (Scene.DirectionalLights.Count <= 0)
                sunColor = Scene.Color.Xyz;
            else
                sunColor = Scene.DirectionalLights[0].Color.Xyz; ;

            shader.InsertUniform(Uniform.in_lightsun, ref sunColor);
        }

        public override void drawNormal(ViewInfo curView)
        {
        }

        public override void drawSelection(ViewInfo curView)
        {
        }

        protected override void specialLoad(ref XmlTextReader reader, string type)
        {
            if (reader.Name.ToLower() == "front" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                skymats[0] = reader.Value;
            }

            if (reader.Name.ToLower() == "left" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                skymats[1] = reader.Value;
            }

            if (reader.Name.ToLower() == "back" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                skymats[2] = reader.Value;
            }
            if (reader.Name.ToLower() == "right" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                skymats[3] = reader.Value;
            }

            if (reader.Name.ToLower() == "up" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                skymats[4] = reader.Value;
            }

            if (reader.Name.ToLower() == "down" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                skymats[5] = reader.Value;
            }
        }

        private void createRenderObject()
        {
            m_assetsInitialized = true;

            if (null == skymats)
                skymats = _defaultSkybox;

            for (int i = 0; i < 6; i++)
            {
                try
                {
                    addMaterial(String.IsNullOrEmpty(skymats[i]) ? _defaultSkybox[i]
                                                                 : skymats[i]);
                    addMesh("skybox\\sky_" + (i + 1) + ".obj");
                }
                catch
                {
                    continue;
                }
            }

            
        }
    }
}
