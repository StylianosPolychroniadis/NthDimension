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

using System.Linq;
using NthDimension.Rendering.Culling;
using NthDimension.Rendering.GameViews;
using NthDimension.Rendering.Geometry;
using NthDimension.Rendering.Scenegraph;
using NthDimension.Rendering.Shaders;
using NthDimension.Rendering.Utilities;

namespace NthDimension.Rendering.Drawables
{
    using System;
    using System.Collections.Generic;

    using NthDimension.Algebra;
    using Rendering.Geometry;
    using Rendering.Scenegraph;
    using Rendering.Shaders;
    //using NthDimension.OpenGL.GLSL.API3x;
    using NthDimension.Rasterizer;

#if _WINDOWS_
    //using NthDimension.OpenGL.Windows;
    //using NthDimension.OpenGL.Windows.GLSL.API3x;
#endif


    public class Drawable : ApplicationObject
    {
        public enum RenderLayer
        {
            Solid,
            Both,
            Transparent
        }
        //protected WeakReference                 _renderer;

        protected int[]                         vaoHandle               = new int[0];

        public MeshVbo[]                           meshes                  = new MeshVbo[0];

        protected Material[]                    materials               = new Material[0];

        protected Matrix4                       modelMatrix             = Matrix4.Identity;
        protected Matrix4                       orientation             = Matrix4.Identity;

        private List<string>                    materialNames            = new List<string>();
        private List<string>                    meshNames                = new List<string>();
        private List<string[]>                  textureNames             = new List<string[]>();

        private float                           biggestMeshSphere;
        private const float                     biggestMeshSphereMulitplier = 1f;//1.2f;

        #region Properties
        /// <summary>
        /// Primitive rasterizer type, Triangles by default
        /// </summary>
        public PrimitiveType                    PrimitiveType           = PrimitiveType.Triangles;
        public bool                             IsVisible               = true;
        public bool                             IgnoreCulling           = false;
        public bool                             IgnoreLod               = false;                // Added May-05-18 
        public bool                             CastShadows             = true;
        public bool                             ReceiveShadows          = true;
        public float                            BoundingSphere          = 1f;
        public float                            SelectedSmooth;
        public float                            Selected;
        public float                            DistToCamera;
        public RenderLayer                      Renderlayer;
        public Vector2                          ShaderVector;

        public Matrix4                          ModelMatrix
        {
            get { return modelMatrix; }
            set
            {
                if (modelMatrix != value)
                {
                    modelMatrix = value;
                    position = MathHelper.Mult(new Vector4(0, 0, 0, 1), value).Xyz;     // Un-commented Apr-12-18
                    //position = modelMatrix.ExtractTranslation();                      // Commented Apr-12-18 
                    //wasUpdated = true;
                }
            }
        }

        private object                          _lockScene              = new object();
        public override SceneGame                   Scene
        {
            get
            { return base.Scene; }
            set
            {
                if (base.Scene != null)
                {
                    lock (_lockScene)
                    {


                        base.Scene.RemoveDrawable(this);
                        base.Scene = value;
                        base.Scene.AddDrawable(this);
                    }

                }
            }
        }
        public override Matrix4                 Orientation
        {
            get { return orientation; }
            set
            {
                if (orientation != value)
                {
                    orientation = value;
                    Vector4 dir = new Vector4(0, 1, 0, 0);

                    pointingDirection = MathHelper.Mult(dir, value).Xyz;

                    //wasUpdated = true;    // Commented Apr-12-18 (also commented in vanilla)
                }
            }
        }
        public override Vector3                 Position
        {
            get { return position; }
            set
            {
                if (position != value)
                {
                    position = value;
                    updateModelMatrix();

                    //wasUpdated = true;    // Commented Apr-12-18 (also commented in vanilla)
                }
            }
        }
        public override Vector3                 PointingDirection
        {
            get { return pointingDirection; }
            set
            {
                if (pointingDirection != value)
                {
                    pointingDirection = value;
                    orientation = GenericMethods.MatrixFromVector(value);

                    //wasUpdated = true;    // Commented Apr-12-18 (also commented in vanilla)
                }
            }
        }
        public override Vector3                 Size
        {
            get { return size; }
            set
            {
                if (size != value)
                {
                    size = value;
                    updateModelMatrix();
                    updateBoundingSphere();

                    //wasUpdated = true;    // Commented Apr-12-18 (also commented in vanilla)
                }
            }
        }

        /// <summary>
        /// Creates the VAOs for the Mesh[]
        /// </summary>
        public List<string>                     Meshes
        {
            get
            {
                return meshNames;
            }
            set
            {
                meshNames = value;
                int meshAmnt = value.Count;

                meshes = new MeshVbo[meshAmnt];
                


                for (int i = 0; i < meshAmnt; i++)
                {
                    MeshVbo newMesh = (MeshVbo)ApplicationBase.Instance.MeshLoader.GetMeshByName(meshNames[i]);
                    newMesh.CurrentLod = MeshVbo.MeshLod.Level0;

                    if (newMesh.BoundingSphere > biggestMeshSphere)
                        biggestMeshSphere = newMesh.BoundingSphere * biggestMeshSphereMulitplier;

                    meshes[i] = newMesh;
                }

                updateBoundingSphere();

                CreateVAO();
            }
        }
        public List<string>                     Materials
        {
            get
            {
                return materialNames;
            }
            set
            {
                materialNames = value;
                int materialAmnt = value.Count;
                materials = new Material[materialAmnt];
                for (int i = 0; i < materialAmnt; i++)
                {
                    try
                    {
                        materials[i] = ApplicationBase.Instance.MaterialLoader.GetMaterialByName(materialNames[i]);
                    }
                    catch(Exception mE)
                    {
                        ConsoleUtil.errorlog("Drawable.Materials.get() ", string.Format("{0}\n{1}", mE.Message, mE.StackTrace));
                    }
                }
            }
        }
        public Vector3                          EmissionColor
        {
            get { return materials[0].propertys.emitMapTint; }
            set
            {
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i].propertys.emitMapTint = value;
                }
            }
        }

        public DrawablePerformanceMetric        Performance = new DrawablePerformanceMetric();

        #endregion

        #region Ctor
        public Drawable(/*RendererBase renderer*/)
        {
            //this._renderer = new WeakReference(renderer);
        }
        #endregion

        #region matices/position
        protected virtual void SetupMatrices(ref ViewInfo curView, ref Shaders.Shader shader, ref MeshVbo curMesh)
        {
            shader.InsertUniform(Uniform.projection_matrix,     ref curView.projectionMatrix);
            shader.InsertUniform(Uniform.modelview_matrix,      ref curView.modelviewMatrix);
            shader.InsertUniform(Uniform.rotation_matrix,       ref orientation);
            shader.InsertUniform(Uniform.model_matrix,          ref modelMatrix);
        }

        public void UpdateBounds()
        {
            updateBoundingSphere();
            updateBoundingBox();
        }
        public virtual void updateBoundingSphere()
        {
            #region Bounding Sphere
            if (size.X >= size.Y && size.X >= size.Z)
            {
                BoundingSphere = biggestMeshSphere * size.X;
                return;
            }

            if (size.Y >= size.X && size.Y >= size.Z)
            {
                BoundingSphere = biggestMeshSphere * size.Y;
                return;
            }
            if (size.Z >= size.Y && size.Z >= size.X)
            {
                BoundingSphere = biggestMeshSphere * size.Z;
                return;
            }

            BoundingSphere = biggestMeshSphere * size.X;
            #endregion

           
        }
        private void updateBoundingBox()
        {
            //#region Bounding Box

            ////List<Vector3> vecs = meshes.Select(pos => pos.positionVboData.ToArray()).ToArray();
            //Vector3[] vecs = meshes.SelectMany(pos => pos.positionVboData).Distinct().ToArray();
            //if(vecs.Count() > 0)
            //    OctreeBounds = BoundingAABB.CreateFromPoints(vecs);
            //#endregion

            for (int m = 0; m < meshes.Length; m++)
            //meshes[m].OctreeBounds = BoundingAABB.CreateFromPoints(meshes[m].positionVboData);
            {
                if (!meshes[m].HasCentroid)
                {
                    meshes[m].CalculateCentroid(this.Position);
                    //meshes[m].CalculateCentroid();

                    if (meshes[m].MeshData.Positions.Length > 0)
                    {
                        meshes[m].OctreeBounds = BoundingAABB.CreateFromPoints(meshes[m].MeshData.Positions);
                        //meshes[m].OctreeBounds.Center += this.Position;
                    }
                }
            }
        }

        internal void updateModelMatrix()
        {
            Matrix4 scaleMatrix = Matrix4.CreateScale(Size);
            Matrix4 translationMatrix = Matrix4.CreateTranslation(position);
            Matrix4.Mult(ref scaleMatrix, ref translationMatrix, out modelMatrix);
        }

        #endregion matices/position

        #region material
        public void setMaterial(string name)
        {
            try
            {                
                materials = new Material[] {ApplicationBase.Instance.MaterialLoader.GetMaterialByName(name)};
                materialNames.Add(name);
            }
            catch
            {
                ConsoleUtil.errorlog("Drawable Error: ", string.Format("Failed to assign material '{0}' to drawable '{1}'", name, this.Name));
            }
        }

        internal void addMaterial(string name)
        {
           

            Material[] tmpMaterials = new Material[materials.Length + 1];
            for (int i = 0; i < materials.Length; i++)
            {
                tmpMaterials[i] = materials[i];
            }

            Material mat = ApplicationBase.Instance.MaterialLoader.GetMaterialByName(name);


            tmpMaterials[materials.Length] = mat;

            materials = tmpMaterials;
            materialNames.Add(name);
        }

      
        public Shaders.Shader activateMaterial(ref Material curMat)
        {
            int                 texunit     = 0;
            Shaders.Shader      shader      = curMat.shader;
            int                 handle      = shader.Handle;
            Material.Propertys  propertys   = curMat.propertys;


            if (!shader.Loaded)
                return shader;

            //ConsoleUtil.log(string.Format("Activating Material {0} : {1} ", curMat.name, debugModelName));

#if _SHADERDEBUG_
            Factories.ShaderLoader.UsingShader(shader);
#endif

            ApplicationBase.Instance.Renderer.UseProgram(handle);

            #region Log Validate Status
#if !OPTIMIZED
            int validateStatus = -1;
            ApplicationBase.Instance.Renderer.ValidateProgram(handle);
            ApplicationBase.Instance.Renderer.GetProgram(handle, GetProgramParameterName.ValidateStatus, out validateStatus);
            if (validateStatus != 1)
                ConsoleUtil.log(string.Format("\tValidate Status: {0}:{1}", shader.Name, validateStatus));
#endif
            #endregion Check Validate Status

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

            int emit = propertys.useEmit ? 1 : 0;

            curMat.activateTexture(Material.TexType.baseTexture,        ref texunit, ref handle);
            curMat.activateTexture(Material.TexType.base2Texture,       ref texunit, ref handle);
            curMat.activateTexture(Material.TexType.base3Texture,       ref texunit, ref handle);
            curMat.activateTexture(Material.TexType.definfoTexture,     ref texunit, ref handle);
            curMat.activateTexture(Material.TexType.reflectionTexture,  ref texunit, ref handle);
            curMat.activateTexture(Material.TexType.normalTexture,      ref texunit, ref handle);

            if (propertys.useEmit)
            {
                curMat.activateTexture(Material.TexType.emitTexture,    ref texunit, ref handle);
                shader.InsertUniform(Uniform.in_emitcolor, ref propertys.emitMapTint);
            }
            shader.InsertUniform(Uniform.use_emit, ref emit);

            shader.InsertUniform(Uniform.fresnelExp,                    ref propertys.fresnelExp);
            shader.InsertUniform(Uniform.fresnelStr,                    ref propertys.fresnelStr);

            activateWorldTexture(Material.WorldTexture.reflectionMap,   ref texunit, handle);
            shader.InsertUniform(Uniform.in_eyepos,                     ref Scene.EyePos);
         
            activateWorldTexture(Material.WorldTexture.lightMap,        ref texunit, handle);

       
//            if (propertys.useEmit)
//            {
//                emit = 1;   // TODO:: Get from material def

//                //throw new NotImplementedException();

//                shader.InsertUniform(Uniform.in_emitcolor, ref propertys.emitMapTint);
                

//#region Commented out section
                 
//                //int emitBasealpha = 0;
//                //if (propertys.emitMapAlphaBaseTexture)
//                //    emitBasealpha = 1;

//                //int emitNormalalpha = 0;
//                //if (propertys.emitMapAlphaNormalTexture)
//                //    emitNormalalpha = 1;

//                //shader.InsertUniform(Uniform.emit_a_normal, ref emitNormalalpha);
//                //shader.InsertUniform(Uniform.emit_a_base, ref emitBasealpha);
//                //shader.InsertUniform(Uniform.in_emitcolor, ref propertys.emitMapTint);
                 
//                //if (curMat.envMapTexture != 0)
//                //{
//                //    Game.Instance.Renderer.ActiveTexture(TextureUnit.Texture0 + texunit);
//                //    Game.Instance.Renderer.BindTexture(TextureTarget.Texture2D, curMat.envMapTexture);
//                //    Game.Instance.Renderer.Uniform1(Game.Instance.Renderer.GetUniformLocation(handle, "envMapTexture"), texunit);
//                //    texunit++;
//                //}
                
//#endregion
//            }
//            shader.InsertUniform(Uniform.use_emit, ref emit);

#region Transparency   (WARNING::: Transparency is ALWAYS 1 - MODIFY!!!!!)
            int transparency = 0;
            if (propertys.useAlpha)
            {
                transparency = 1;      // TODO:: Get from material def

                shader.InsertUniform(Uniform.ref_size, ref propertys.refStrength);
                shader.InsertUniform(Uniform.blur_size, ref propertys.blurStrength);
                shader.InsertUniform(Uniform.fresnel_str, ref propertys.fresnelStrength);

                ApplicationBase.Instance.Renderer.ActiveTexture(TextureUnit.Texture0 + texunit);
                ApplicationBase.Instance.Renderer.BindTexture(TextureTarget.Texture2D, this.Scene.BackdropTextures[0]);
                ApplicationBase.Instance.Renderer.Uniform1(ApplicationBase.Instance.Renderer.GetUniformLocation(handle, "backColorTexture"), texunit);
                texunit++;


                ApplicationBase.Instance.Renderer.ActiveTexture(TextureUnit.Texture0 + texunit);
                ApplicationBase.Instance.Renderer.BindTexture(TextureTarget.Texture2D, this.Scene.BackdropTextures[1]);
                ApplicationBase.Instance.Renderer.Uniform1(ApplicationBase.Instance.Renderer.GetUniformLocation(handle, "backDepthTexture"), texunit);
                texunit++;

            }
            shader.InsertUniform(Uniform.use_alpha, ref transparency);
#endregion

            if (propertys.noCull)
            {
                //GameBase.Instance.Renderer.Disable(EnableCap.CullFace);
                ApplicationBase.Instance.Renderer.CullFaceEnabled = false;
            }
            else
            {
                //GameBase.Instance.Renderer.Enable(EnableCap.CullFace);
                ApplicationBase.Instance.Renderer.CullFaceEnabled = true;
            }

            if (propertys.noDepthMask)
            {
                ApplicationBase.Instance.Renderer.DepthMask(false);
            }
            else
            {
                ApplicationBase.Instance.Renderer.DepthMask(true);
            }

            if (propertys.additive)
            {
#if OPENTK3
                Game.Instance.Renderer.BlendFunc(BlendingFactor.One, BlendingFactor.One);
#else
                ApplicationBase.Instance.Renderer.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
#endif
            }
            else
            {
#if OPENTK3
                Game.Instance.Renderer.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
#else
                ApplicationBase.Instance.Renderer.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
#endif
            }

#if _SHADERDEBUG_
            Factories.ShaderLoader.UsingShader(shader);
#endif


            return shader;
        }

        private void activateWorldTexture(Material.WorldTexture type, ref int texunit, int handle)
        {
            string name = type.ToString();
            int texid = Scene.GetTextureId(type);

            if (type == Material.WorldTexture.lightMap && texid == 0)
                ConsoleUtil.errorlog("lightMap ID ", " ZERO");

            if (texid != 0)
            {
                ApplicationBase.Instance.Renderer.ActiveTexture(TextureUnit.Texture0 + texunit);
                ApplicationBase.Instance.Renderer.BindTexture(TextureTarget.Texture2D, texid);
                ApplicationBase.Instance.Renderer.Uniform1(ApplicationBase.Instance.Renderer.GetUniformLocation(handle, name), texunit);
                texunit++;
            }
        }

        public Shaders.Shader activateMaterialNormal(Material curMat/*, string debugModelName*/) // string debugModelName used ONLY for debug REMOVE REMOVE REMOVE
        {
            int texunit = 0;
            Shaders.Shader shader = curMat.ssnshader;
            int handle = shader.Handle;

            if (!shader.Loaded)
                return shader;

            //ConsoleUtil.log(string.Format("Activating Material Normal {0} : {1} ", curMat.name, debugModelName));

#if _SHADERDEBUG_
            Factories.ShaderLoader.UsingShader(shader);
#endif

            ApplicationBase.Instance.Renderer.UseProgram(handle);

            #region Log Validate Status
#if !OPTIMIZED
            int validateStatus = -1;
            ApplicationBase.Instance.Renderer.ValidateProgram(shader.Handle);
            ApplicationBase.Instance.Renderer.GetProgram(shader.Handle, GetProgramParameterName.ValidateStatus, out validateStatus);
            if (validateStatus != 1)
                ConsoleUtil.log(string.Format("\tValidate Status: {0}:{1}", shader.Name, validateStatus));
#endif
#endregion Check Validate Status

            shader.InsertUniform(Uniform.in_specexp, ref curMat.propertys.specExp);

            int normalTexture = curMat.getTextureId(Material.TexType.normalTexture);
            if (normalTexture != 0)
            {
                ApplicationBase.Instance.Renderer.ActiveTexture(TextureUnit.Texture0 + texunit);
                ApplicationBase.Instance.Renderer.BindTexture(TextureTarget.Texture2D, normalTexture);
                ApplicationBase.Instance.Renderer.Uniform1(ApplicationBase.Instance.Renderer.GetUniformLocation(handle, "normalTexture"), texunit);
                texunit++;
            }

#if _SHADERDEBUG_
            Factories.ShaderLoader.UsingShader(shader);
#endif

            return shader;
        }

        public Shaders.Shader activateMaterialSelection(Material curMat)
        {
            int texunit = 0;
            int handle = curMat.selectionshader.Handle;

            if (!curMat.selectionshader.Loaded)
                return curMat.selectionshader;

#if _SHADERDEBUG_
            Factories.ShaderLoader.UsingShader(curMat.selectionshader);
#endif

            ApplicationBase.Instance.Renderer.UseProgram(handle);

#region Log Validate Status
            int validateStatus = -1;
            ApplicationBase.Instance.Renderer.ValidateProgram(handle);
            ApplicationBase.Instance.Renderer.GetProgram(handle, GetProgramParameterName.ValidateStatus, out validateStatus);
            if(validateStatus != 1)
                ConsoleUtil.log(string.Format("\tValidate Status: {0}:{1}", curMat.selectionshader.Name, validateStatus));
#endregion Check Validate Status

            curMat.activateTexture(Material.TexType.normalTexture, ref texunit, ref handle);

#if _SHADERDEBUG_
            Factories.ShaderLoader.UsingShader(curMat.selectionshader);
#endif

            return curMat.selectionshader;
        }

        public Shaders.Shader activateMaterialShadow(Material curMat/*, string debugModelName*/) // string debugModelName used ONLY for debug -> REMOVE REMOVE REMOVE
        {
            Shaders.Shader shader = curMat.shadowshader;
            int handle = curMat.shadowshader.Handle;

            if (!shader.Loaded)
                return shader;

            //ConsoleUtil.log(string.Format("Activating Material Shadow {0} : {1} ", curMat.name, debugModelName));

#if _SHADERDEBUG_
            Factories.ShaderLoader.UsingShader(shader);
#endif

            ApplicationBase.Instance.Renderer.UseProgram(handle);

            #region Log Validate Status
#if !OPTIMIZED
#if !OPTIMIZED
            int validateStatus = -1;
            ApplicationBase.Instance.Renderer.ValidateProgram(handle);
            ApplicationBase.Instance.Renderer.GetProgram(handle, GetProgramParameterName.ValidateStatus, out validateStatus);
            if (validateStatus != 1)
                ConsoleUtil.log(string.Format("\tValidate Status: {0}:{1}", curMat.selectionshader.Name, validateStatus));
#endif
#endif
#endregion Check Validate Status

            shader.InsertUniform(Uniform.in_no_lights,  ref Scene.LightCount);
            shader.InsertUniform(Uniform.curLight,      ref Scene.CurrentLight);
            //Vector3 eye = Scene.EyePos;                                               // Commented Apr-12-18
            //shader.InsertUniform(Uniform.in_eyepos,     ref eye);                     // Commented Apr-12-18

#if _SHADERDEBUG_
            Factories.ShaderLoader.UsingShader(shader);
#endif

            return curMat.shadowshader;
        }

#endregion material

#region textures

        protected int initEnvTextures(int texunit, int handle)
        {
            for (int i = 0; i < 6; i++)
            {
                ApplicationBase.Instance.Renderer.ActiveTexture(TextureUnit.Texture0 + texunit);
                ApplicationBase.Instance.Renderer.BindTexture(TextureTarget.Texture2D, Scene.EnvTextures[i]);
                ApplicationBase.Instance.Renderer.Uniform1(ApplicationBase.Instance.Renderer.GetUniformLocation(handle, "EnvTexture" + (i + 1)), texunit);
                texunit++;
            }
            return texunit;
        }

        protected void initTextures(int[] texures, int handle, string target)
        {
            for (int i = 0; i < texures.Length; i++)
            {
                ApplicationBase.Instance.Renderer.ActiveTexture(TextureUnit.Texture0 + i);
                ApplicationBase.Instance.Renderer.BindTexture(TextureTarget.Texture2D, texures[i]);
                int uniform = ApplicationBase.Instance.Renderer.GetUniformLocation(handle, target + (i + 1));

                // MARCH-15-18 WEIRD BUG in composite.xsp -> Texture6 does not exist?!?!?!?!
                //if (uniform == -1)
                //    ConsoleUtil.errorlog(string.Format("\tUniform Sampler2d {0}", target+(i+1)), "does not exist");

                ApplicationBase.Instance.Renderer.Uniform1(uniform, i);
            }
        }

#endregion textures

#region draw (virtual)
        public virtual void draw()
        {
        }

        public virtual void draw(ViewInfo curView)
        {
        }

        public virtual void draw(ViewInfo curView, bool transparencyLayer)
        {
        }

        public virtual void draw(ViewInfo curView, Shaders.Shader curShader)
        {
        }

        public virtual void drawSelection(ViewInfo curView)
        {
        }

        public virtual void drawNormal(ViewInfo curView)
        {
        }

        public virtual void drawDefInfo(ViewInfo curView)
        {
        }

        public virtual void drawShadow(ViewInfo curView)
        {
        }

#endregion draw (virtual)

#region mesh management

        public void setMesh(string name)
        {
            try
            {
                setMesh(ApplicationBase.Instance.MeshLoader.GetMeshByName(name));
            }
            catch
            {
                ConsoleUtil.errorlog("Drawable Error: ", string.Format("Failed to assign mesh '{0}' to drawable '{1}'", name, this.Name));
            }
        }
        public void setMesh(string name, MeshVbo.MeshLod lodLevel)
        {
            MeshVbo mesh = ApplicationBase.Instance.MeshLoader.GetMeshByName(name);
            mesh.CurrentLod = lodLevel;
            setMesh(mesh);
        }

        public void setMesh(MeshVbo newMesh)
        {
            meshNames = new List<string> { newMesh.Name };

            meshes = new MeshVbo[1];
            meshes[0] = newMesh;
            meshes[0].CurrentLod = MeshVbo.MeshLod.Level0;

            biggestMeshSphere = newMesh.BoundingSphere * biggestMeshSphereMulitplier;

            updateBoundingSphere();

            CreateVAO();
        }

        public void addMesh(string name)
        {
            addMesh(ApplicationBase.Instance.MeshLoader.GetMeshByName(name));
        }

        public virtual void addMesh(MeshVbo newMesh)
        {
            meshNames.Add(newMesh.Name);

            MeshVbo[] tmpMesh = new MeshVbo[meshes.Length + 1];
            for (int i = 0; i < meshes.Length; i++)
            {
                tmpMesh[i] = meshes[i];
                tmpMesh[i].CurrentLod = MeshVbo.MeshLod.Level0;
            }
            tmpMesh[meshes.Length] = newMesh;

            meshes = tmpMesh;

            if (newMesh.BoundingSphere > biggestMeshSphere)
                biggestMeshSphere = newMesh.BoundingSphere * biggestMeshSphereMulitplier;

            updateBoundingSphere();
            updateBoundingBox();

            

            CreateVAO();
        }
#endregion mesh management

#region VAO
        public void CreateVAO()
        {


            ApplicationBase.Instance.CheckGlError(string.Format("(!) OpenGL Error: Create VAOs() {0}:{1}", this.GetType(), Name));

            int[] newHandle = new int[meshes.Length];

            for (int i = 0; i < newHandle.Length && i < vaoHandle.Length; i++)
            {
                newHandle[i] = vaoHandle[i];
            }
            vaoHandle = newHandle;

            for (int i = 0; i < meshes.Length; i++)
            {
                int matIdx = 0;

                if (meshes.Length == materials.Length)
                    matIdx = i;

                Shaders.Shader curShader = materials[matIdx].shader;

                MeshVbo curMesh = meshes[i];
                int shaderHandle = curShader.Handle;

                int shaderAttribIndex_normal    = ApplicationBase.Instance.Renderer.GetAttribLocation(   shaderHandle, "in_normal");
                int shaderAttribIndex_position  = ApplicationBase.Instance.Renderer.GetAttribLocation(   shaderHandle, "in_position");
                int shaderAttribIndex_tangent   = ApplicationBase.Instance.Renderer.GetAttribLocation(   shaderHandle, "in_tangent");
                int shaderAttribIndex_texture   = ApplicationBase.Instance.Renderer.GetAttribLocation(   shaderHandle, "in_texture");

                // GL3 allows us to store the vertex layout in a "vertex array object" (VAO).
                // This means we do not have to re-issue VertexAttribPointer calls
                // every time we try to use a different vertex layout - these calls are
                // stored in the VAO so we simply need to bind the correct VAO.

               

                if (vaoHandle[i] == 0)
                    ApplicationBase.Instance.Renderer.GenVertexArrays(1, out vaoHandle[i]);
meshes[i].MeshData.vaoHandle = vaoHandle;
                ApplicationBase.Instance.Renderer.BindVertexArray(vaoHandle[i]);

#region Bones

                if (null != curMesh.MeshData.MeshBones)
                {
                    int affectingBonesCount = curMesh.MeshData.MeshBones.AffectingBonesCount;
                    for (int j = 0; j < affectingBonesCount; j++)
                    {
                        int boneIdIndex = ApplicationBase.Instance.Renderer.GetAttribLocation(shaderHandle, "in_joint_" + j);
                        int boneWeightIndex = ApplicationBase.Instance.Renderer.GetAttribLocation(shaderHandle, "in_weight_" + j);

                        if (boneIdIndex != -1)
                        {
                            ApplicationBase.Instance.Renderer.EnableVertexAttribArray(boneIdIndex);
                            ApplicationBase.Instance.Renderer.BindBuffer(BufferTarget.ArrayBuffer,
                                curMesh.MeshData.MeshBones.BoneIdHandles[j]);
                            ApplicationBase.Instance.Renderer.VertexAttribPointer(boneIdIndex, 1, VertexAttribPointerType.Int,
                                false, /*sizeof(int)*/ 0, (IntPtr)0);
                        }

                        if (boneWeightIndex != -1)
                        {
                            ApplicationBase.Instance.Renderer.EnableVertexAttribArray(boneWeightIndex);
                            ApplicationBase.Instance.Renderer.BindBuffer(BufferTarget.ArrayBuffer,
                                curMesh.MeshData.MeshBones.BoneWeigthHandles[j]);
                            ApplicationBase.Instance.Renderer.VertexAttribPointer(boneWeightIndex, 1, VertexAttribPointerType.Float,
                                true, /*sizeof(float)*/ 0, (IntPtr)0);
                        }
                    }
                }

#endregion

#region Normals
                if (shaderAttribIndex_normal != -1)
                {
                    ApplicationBase.Instance.Renderer.EnableVertexAttribArray(shaderAttribIndex_normal);
                    ApplicationBase.Instance.Renderer.BindBuffer(BufferTarget.ArrayBuffer, curMesh.MeshData.NormalHandle);
                    ApplicationBase.Instance.Renderer.VertexAttribPointer(shaderAttribIndex_normal, 3, VertexAttribPointerType.Float, true, /*Vector3.SizeInBytes*/ 0, (IntPtr)0);
                    //Game.Instance.Renderer.BindAttribLocation(shaderHandle, 0, "in_normal");
                }
#endregion

#region Positions
                if (shaderAttribIndex_position != -1)
                {
                    ApplicationBase.Instance.Renderer.EnableVertexAttribArray(shaderAttribIndex_position);
                    ApplicationBase.Instance.Renderer.BindBuffer(BufferTarget.ArrayBuffer, curMesh.MeshData.PositionHandle);
                    ApplicationBase.Instance.Renderer.VertexAttribPointer(shaderAttribIndex_position, 3, VertexAttribPointerType.Float, true, /*Vector3.SizeInBytes*/ 0, (IntPtr)0);
                    //Game.Instance.Renderer.BindAttribLocation(shaderHandle, 1, "in_position");
                }
#endregion

#region Tangents
                if (shaderAttribIndex_tangent != -1)
                {
                    ApplicationBase.Instance.Renderer.EnableVertexAttribArray(shaderAttribIndex_tangent);
                    ApplicationBase.Instance.Renderer.BindBuffer(BufferTarget.ArrayBuffer, curMesh.MeshData.TangentHandle);
                    ApplicationBase.Instance.Renderer.VertexAttribPointer(shaderAttribIndex_tangent, 3, VertexAttribPointerType.Float, true, /*Vector3.SizeInBytes*/ 0, (IntPtr)0);
                    //Game.Instance.Renderer.BindAttribLocation(shaderHandle, 2, "in_tangent");
                }
#endregion

#region Textures UV
                if (shaderAttribIndex_texture != -1)
                {
                    ApplicationBase.Instance.Renderer.EnableVertexAttribArray(shaderAttribIndex_texture);
                    ApplicationBase.Instance.Renderer.BindBuffer(BufferTarget.ArrayBuffer, curMesh.MeshData.TextureHandle);
                    ApplicationBase.Instance.Renderer.VertexAttribPointer(shaderAttribIndex_texture, 2, VertexAttribPointerType.Float, true, /*Vector2.SizeInBytes*/ 0, (IntPtr)0);
                    //Game.Instance.Renderer.BindAttribLocation(shaderHandle, 3, "in_texture");

                }
#endregion

                ApplicationBase.Instance.Renderer.BindBuffer(BufferTarget.ElementArrayBuffer, curMesh.MeshData.EboHandle);
                //GameBase.Instance.Renderer.BindVertexArray(0);
            }

            ApplicationBase.Instance.CheckGlError(string.Format("(!) OpenGL Error: Create VAOs() {0}:{1}", this.GetType(), Name));
        }
#endregion

#region FrustumCheck
        //public virtual bool FrustrumCheck(ViewInfo info)
        //{
        //    // using Mesh.IsVisible for Model (PhysModel)

        //    Vector4 vSpacePos = GenericMethods.Mult(new Vector4(this.Position, 1), info.modelviewProjectionMatrix);


        //    float range = this.BoundingSphere;

        //    float distToDrawAble = (position - info.Position).LengthFast;

        //    if (distToDrawAble < range * 1.5f)
        //        return true;

        //    if (distToDrawAble - range > info.zFar)
        //        return false;

        //    if (vSpacePos.W <= 0)
        //        return false;

        //    range /= vSpacePos.W * 0.6f;

        //    if (float.IsNaN(range) || float.IsInfinity(range))
        //        return false;

        //    vSpacePos /= vSpacePos.W;


        //    return (
        //        vSpacePos.X < (1f + range) && vSpacePos.X > -(1f + range) &&
        //        vSpacePos.Y < (1f + range * info.aspect) && vSpacePos.Y > -(1f + range * info.aspect)
        //        );
        //}

        
#endregion


    }
}
