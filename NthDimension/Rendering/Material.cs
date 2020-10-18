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

#define _WINDOWS_

using Shader = NthDimension.Rendering.Shaders.Shader;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using ProtoBuf;
using NthDimension.Algebra;
using NthDimension.Rendering.Configuration;
using NthDimension.Rendering.Drawables.Models;
using NthDimension.Rendering.Loaders;
using NthDimension.Rendering.Utilities;
using NthDimension.Rasterizer;
using NthDimension.Utilities;

namespace NthDimension.Rendering
{
    [Serializable, ProtoContract]
    public struct Material
    {
        //generic stuff
        [ProtoMember(10)]
        public string                   pointer;
        [ProtoMember(180)]
        public int                      identifier;
        //[ProtoMember(190)]
        public bool                     loaded;
        [ProtoMember(430)]
        public Type                     type;
        //stuff to be saved
        [ProtoMember(440)]
        public string                   name;

#if _WINDOWS_
        [ProtoMember(450)]
        public Shaders.Shader           shader;
        [ProtoMember(460)]
        public Shaders.Shader           ssnshader;      // Screen-space Normals
        [ProtoMember(480)]
        public Shaders.Shader           shadowshader;   // Includes spotlights
        [ProtoMember(490)]
        public Shaders.Shader           definfoshader;  // Deferred sun-light shader
        // TODO (PBR)
        // public Shaders.Shader        aoshader;       // TODO:: Convert ambient occlusion to read from texture instead of current dynamic noise (PBR)

        [ProtoMember(470)]
        public Shaders.Shader           selectionshader;

#endif

        [ProtoMember(500)]
        public Material.Propertys       propertys;
        public Texture[]                Textures
        {
            get { return textures; }
        }

        [ProtoMember(510)]
        private Texture[]               textures;

        #region Type enumerator
        [ProtoContract]
        public enum Type
        {
            [ProtoMember(20)]
            fromXml,
            [ProtoMember(30)]
            fromCache,
            [ProtoIgnore()]
            dynamic
        };
        #endregion

        #region TexType Enumerator
        [ProtoContract]
        public enum TexType
        {
            [ProtoMember(40)]
            baseTexture,
            [ProtoMember(50)]
            baseTextureTwo,
            [ProtoMember(60)]
            baseTextureThree,
            [ProtoMember(65)]
            baseTextureFour,
            [ProtoMember(70)]
            normalTexture,
            [ProtoMember(80)]
            emitTexture,
            [ProtoMember(90)]
            reflectionTexture,
            [ProtoMember(100)]
            emitMapTexture,
            [ProtoMember(110)]
            specMapTexture,
            [ProtoMember(120)]
            envMapTexture,
            [ProtoMember(130)]
            envTexture,
            [ProtoMember(140)]
            definfoTexture,
            [ProtoMember(150)]
            videoTexture
        }
        #endregion

        #region WorldTexture enumerator
        [ProtoContract]
        public enum WorldTexture
        {
            [ProtoMember(150)]
            lightMap,
            [ProtoMember(160)]
            reflectionMap,
            [ProtoMember(170)]
            noise
        }
        #endregion

        #region Propertys struct
        [Serializable, ProtoContract]
        public struct Propertys
        {
            [ProtoMember(200)]
            public bool envMapAlphaBaseTexture;
            [ProtoMember(210)]
            public bool useEnv;
            [ProtoMember(220)]
            public bool envMapAlphaNormalTexture;
            [ProtoMember(230)]
            public Vector3 envMapTint;
            [ProtoMember(240)]
            public bool emitMapAlphaNormalTexture;
            [ProtoMember(250)]
            public bool emitMapAlphaBaseTexture;
            [ProtoMember(260)]
            public Vector3 emitMapTint;
            [ProtoMember(270)]
            public bool useEmit;

            //   #region Added 2019 - Parallax Mapping - killed cache

            [ProtoMember(460)]
            public bool heightmapBaseTexture;
            public Vector3 heightmapHeight;

            //   #endregion

            [ProtoMember(280)]
            public bool useAlpha;
            [ProtoMember(290)]
            public float refStrength;
            [ProtoMember(300)]
            public float blurStrength;
            [ProtoMember(310)]
            public float fresnelStrength;

            [ProtoMember(320)]
            public bool useLight;
            [ProtoMember(330)]
            public bool useSpec;
            [ProtoMember(340)]
            public bool specMapAlphaNormalTexture;
            [ProtoMember(350)]
            public bool specMapAlphaBaseTexture;
            [ProtoMember(360)]
            public Vector3 specMapTint;
            [ProtoMember(370)]
            public float specExp;
            [ProtoMember(380)]
            public bool noCull;

            [ProtoMember(390)]
            public bool noDepthMask;
            [ProtoMember(400)]
            public bool additive;

            [ProtoMember(410)]
            public float fresnelExp;
            [ProtoMember(420)]
            public float fresnelStr;
            [ProtoMember(430)]
            public bool useTile;
            [ProtoMember(440)]
            public int tileU;
            [ProtoMember(450)]
            public int tileV;
        }
        #endregion


        public override string ToString()
        {
            return name;
         }

        internal void cacheMaterial(ref List<Material> mList)
        {
            
            Material tmpMat = new Material();

            tmpMat.name = name;

            //propertys
            tmpMat.propertys = propertys;

            //shaders
            tmpMat.shader = shader.NameOnly();
            tmpMat.ssnshader = ssnshader.NameOnly();
            tmpMat.selectionshader = selectionshader.NameOnly();
            tmpMat.shadowshader = shadowshader.NameOnly();
            tmpMat.definfoshader = definfoshader.NameOnly();

            //textures
            int texCount = textures.Length;
            tmpMat.textures = new Texture[texCount];
            for (int i = 0; i < texCount; i++)
            {
                tmpMat.textures[i] = textures[i].nameOnly();
            }

            mList.Add(tmpMat);
        }

        public void setTexture(TexType type, Texture texture)
        {
            
            textures[(int)type] = texture;
        }

        public int getTextureId(TexType type)
        {
            if (null == textures) // ADDED FOR QUICK DEBUG -> REMOVE -> STORE ONLY TEXTURE NAMES IN PROTOBUF
                return 0;

            return textures[(int)type].texture;
        }

        public string getTextureName(TexType type)
        {
            return textures[(int)type].name;
        }

        public void setArys()
        {
            int texCount = Enum.GetValues(typeof(TexType)).Length;
            if (textures == null)
                textures = new Texture[texCount];
        }

        private int wrapS;
        private int wrapT;
        public void setTextureWrap(int wrapS, int wrapT)        // TODO:: Function NEVER WORKED!!!! Investigate in Drawable.activateWorldTexture
        {
            this.wrapS = wrapS;
            this.wrapT = wrapT;
        }

        /*
public Texture nameOnly()
{
    Texture tmpTex = new Texture();

    tmpTex.texture = texture;
    tmpTex.name = name;

    return tmpTex;
}
 */

        internal void resolveTextures(TextureLoader textureLoader)
        {
            if (null == textures)   // ADDED FOR QUICK DEBUG -> REMOVE -> STORE ONLY TEXTURE NAMES IN PROTOBUF
                return;

            int textureCount = textures.Length;
            for (int i = 0; i < textureCount; i++)
            {
                try
                {
                    string texname = textures[i].name;
                    if (texname != null)
                        textures[i] = textureLoader.getTexture(texname);
                }
                catch { }
            }
        }

        internal void resolveShaders(ShaderLoader shaderLoader)
        {
            if (shader.Name != null)
                shader = shaderLoader.GetShaderByName(shader.Name);

            if (shadowshader.Name != null)
                shadowshader = shaderLoader.GetShaderByName(shadowshader.Name);

            if (ssnshader.Name != null)
                ssnshader = shaderLoader.GetShaderByName(ssnshader.Name);

            if (selectionshader.Name != null)
                selectionshader = shaderLoader.GetShaderByName(selectionshader.Name);

            if (definfoshader.Name != null)
                definfoshader = shaderLoader.GetShaderByName(definfoshader.Name);
        }

        internal void activateTexture(TexType type, ref int texunit, ref int handle)
        {
            string  name     = type.ToString();
            int     texid    = getTextureId(type);

            // TODO:: Maintain the uniform inside the shader program
            // https://stackoverflow.com/questions/39786138/opengl-pass-texture-to-program-once-or-at-every-rendering

            /*
                You should bind the texture before every draw. You only need to set the location once. 
                You can also do layout(binding = 1) in your shader code for that. The location uniform 
                stays with the program. The texture binding is a global GL state. Also be careful about 
                ActiveTexture: it is a global GL state.

                Good practice would be:

                On program creation, once, set texture location (uniform)
                On draw: SetActive(i), Bind(i), Draw, SetActive(i) Bind(0), SetActive(0)
             */

            if (texid != 0)
            {
                ApplicationBase.Instance.Renderer.ActiveTexture(TextureUnit.Texture0 + texunit);
                ApplicationBase.Instance.Renderer.BindTexture(TextureTarget.Texture2D, texid);

                ApplicationBase.Instance.Renderer.Uniform1(ApplicationBase.Instance.Renderer.GetUniformLocation(handle, name), texunit);
                texunit++;
            }
        }

        public static string CreateDynamic(string texture, string meshFile, out string faceTexture)
        {
            faceTexture = string.Empty;
            string matName = meshFile.ToLower().Contains("generic")
                ? @"characters\defaultGenericFace.xmf"
                : @"characters\defaultRoundFace.xmf";

            // 0. The material definition
            //string matDef = "<material shader=\"defInfoFace.xsp\" ssnshader=\"ssnormals.xsp\" selection=\"selection.xsp\" shadow=\"shadow.xsp\" definfo=\"defInfoFace.xsp\" > <textures base=\"{0}\" normal=\"{1}\" definfo=\"{0}\"/> <emit  tint=\"0|0|0\"/> <fresnel exp=\"0\" strength=\"0\" />  <lighted /> </material> ";
            string matDef = "<material shader=\"defaultlight.xsp\" ssnshader=\"ssnormals.xsp\" selection=\"selection.xsp\" shadow=\"shadow.xsp\" definfo=\"defInfo.xsp\" > <textures base=\"{0}\" normal=\"{1}\" definfo=\"{0}\"/> <emit  tint=\"0|0|0\"/> <fresnel exp=\"0\" strength=\"0\" />  <lighted /> </material> ";

            // TODO:: Modify material definition as such a) pass the skinColor, vertex position values to 'shader'. 
            // TODO                                      b) Switch defInfo shader to correct defInfo.xsp 

            //1.Download Texture
            string[] strUrl = texture.Split('@');

            try
            {

                System.Net.WebClient client = new System.Net.WebClient();
                System.IO.Stream stream = client.OpenRead(strUrl[1]);
                System.Drawing.Image bitmapColored = System.Drawing.Image.FromStream(stream);

                stream.Flush();
                stream.Close();
                client.Dispose();

                string texturesPath = Path.Combine(DirectoryUtil.Documents,
                                                     NthDimension.Rendering.Configuration.GameSettings.TextureTempFolder);

                string materialsPath = Path.Combine(DirectoryUtil.Documents,
                                                      NthDimension.Rendering.Configuration.GameSettings.MaterialTempFolder);

                if (!Directory.Exists(texturesPath))
                    Directory.CreateDirectory(texturesPath);

                if (!Directory.Exists(materialsPath))
                    Directory.CreateDirectory(materialsPath);

                #region Base Texture (_d.png)
                string[] picFile    = strUrl[1].Split('/');
                string userId       = System.IO.Path.GetFileNameWithoutExtension(picFile[picFile.Length - 1]);
                string baseTexture  = string.Format("{0}{1}_d.png",              // NO \\ after {0}     //string baseTexture = string.Format("{0}{1}.png", // MODIFIED MARCH-13-18 for GrayScale texture
                                     texturesPath,
                                     userId);

                if (bitmapColored != null)
                {
                    #region Original
                    // Original (before apply body texture color)
                    //bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    //bitmap.Save(baseTexture, ImageFormat.Png);
                    #endregion

                    bitmapColored.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    bitmapColored.Save(baseTexture, ImageFormat.Png);  //bitmapColored.Save(baseTexture.Replace("_d.png", ".png"), ImageFormat.Png); // for grayscale

                    #region GrayScale (disabled)
                    //Bitmap bitmapGrayscale = BitmapUtils.MakeGrayscale3((Bitmap)bitmapColored);
                    //bitmapGrayscale.Save(baseTexture, ImageFormat.Png);
                    #endregion

                }
                faceTexture = baseTexture;
                #endregion

                #region Normal Texture (_n.png)
                NormalGenerator normalGen = new NormalGenerator();
                normalGen.Load(baseTexture, 1);

                Image normal = normalGen.CreateNormalMap(1f);

                string normalTexture = string.Format("{0}{1}_n.png",            // NO \\ after {0}
                    texturesPath,
                    userId);

                normal.Save(normalTexture);
                #endregion

                // TODO:: Create Bump Texture
                string specularTexture = string.Format("{0}{1}_s.png",          // NO \\ after {0}
                    texturesPath,
                    userId);

                //Imaging.CavityGenerator cavity = new Imaging.CavityGenerator()

                ApplicationBase.Instance.TextureLoader.fromPng(baseTexture, true);
                ApplicationBase.Instance.TextureLoader.fromPng(normalTexture, true);

                ApplicationBase.Instance.TextureLoader.LoadTextures(null, true);

                string dynMaterialFile = string.Format("{1}.xmf", 
                                                        materialsPath, 
                                                        userId);

                string fileContent = string.Format(matDef,
                                                   Path.GetFileName(baseTexture),                   // baseTexture.Replace(GameSettings.TextureFolder, ""),
                                                   Path.GetFileName(normalTexture)                  // normalTexture.Replace(GameSettings.TextureFolder, "")//,
                                                   //baseTexture.Replace(GameSettings.TextureFolder, "").Replace("_d.png", ".png")
                                                   );

                using (StreamWriter sw = new StreamWriter(Path.Combine(materialsPath, dynMaterialFile)))
                    sw.Write(fileContent);

                ApplicationBase.Instance.MaterialLoader.FromXmlFile(Path.Combine(materialsPath, dynMaterialFile));

                matName = dynMaterialFile;

            }
            catch (Exception we)
            {
                ConsoleUtil.errorlog("Dynamic Texture: ",
                    string.Format("{0}{1}{2}", strUrl[1], Environment.NewLine, we.Message));
            }


            return matName;
        }

    }
}
