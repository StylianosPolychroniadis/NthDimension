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

using System;

using NthDimension.Algebra;
using NthDimension.Rendering.Geometry;
using NthDimension.Rendering.Scenegraph;
using NthDimension.Rendering.Shaders;
using NthDimension.Rendering.Utilities;
using NthDimension.Rasterizer;
//using NthDimension.OpenGL.GLSL.API3x;
//using NthDimension.OpenGL.Windows.GLSL.API3x;
using Shader = NthDimension.Rendering.Shaders.Shader;

namespace NthDimension.Rendering.Drawables
{
    public class Quad2d : Drawable
    {
        public Quad2d(string material = "composite.xmf", string meshLod0 = "base\\sprite_plane.obj")
        {
            setMaterial(material);
            setMesh(meshLod0, MeshVbo.MeshLod.Level0);
        }

        public Quad2d(ApplicationObject parent):this()
        {
            Parent = parent;
        }

        public void draw(Shaders.Shader curShader, int[] curtexture)
        {
            draw(curShader, curtexture, Uniform.in_vector, Vector2.Zero);
        }
        /*
        public void draw(Shader curShader, int[] curtexture, Shader.Uniform uniform, Vector2 vector2)
        {
            draw(curShader, curtexture,new UniformPairList( Uniform.in_vector, Vector2.Zero));
        }
        */
        public void draw(Shaders.Shader shader, int[] curtexture, Uniform uniform, object obj)
        {
            ApplicationBase.Instance.CheckGlError(string.Format("(!) OpenGL Error: Draw Model {0}:{1} Shade: {2}", this.GetType(), Name, shader.Name));

            Factories.ShaderLoader.UpdateShaderDebug(shader);

            ApplicationBase.Instance.Renderer.UseProgram(shader.Handle, true);

            #region Log Validate Status
            int validateStatus = -1;
            ApplicationBase.Instance.Renderer.ValidateProgram(shader.Handle);
            ApplicationBase.Instance.Renderer.GetProgram(shader.Handle, GetProgramParameterName.ValidateStatus, out validateStatus);
            if (validateStatus != 1)
                ConsoleUtil.log(string.Format("\tValidate Status: {0}:{1}", shader.Name, validateStatus));
            #endregion Check Validate Status

            shader.InsertGenUniform(uniform, obj);

            shader.InsertUniform(Uniform.in_screensize, ref ApplicationBase.Instance.VAR_ScreenSize_Virtual);
            shader.InsertUniform(Uniform.in_rendersize, ref ApplicationBase.Instance.VAR_ScreenSize_Current);
            shader.InsertUniform(Uniform.in_time, ref ApplicationBase.Instance.VAR_FrameTime);

            if (Scene != null)
            {
                shader.InsertUniform(Uniform.in_no_lights, ref Scene.LightCount);
                shader.InsertUniform(Uniform.curLight, ref Scene.CurrentLight);
                shader.InsertUniform(Uniform.in_eyepos, ref Scene.EyePos);
                
            }

            initTextures(curtexture, shader.Handle, "Texture");
            ApplicationBase.Instance.Renderer.BindVertexArray(vaoHandle[0]);
            ApplicationBase.Instance.Renderer.DrawElements(PrimitiveType, meshes[0].MeshData.Indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);
            //Game.Instance.Renderer.BindVertexArray(0);
            ApplicationBase.Instance.CheckGlError(string.Format("(!) OpenGL Error: Draw Model {0}:{1} Shade: {2}", this.GetType(), Name, shader.Name));
        }

        internal void draw(enuShaderFilters shaderTypes, int[] p, Uniform uniform, Vector2 vector2)
        {
            Shaders.Shader shader = Scene.GetShader(shaderTypes);
            draw(shader, p, uniform, vector2);
        }

        internal void draw(enuShaderFilters shaderTypes, int[] p)
        {
            Shaders.Shader shader = Scene.GetShader(shaderTypes);
            draw(shader, p);
        }

        internal void draw(enuShaderFilters shaderTypes, int[] p, Uniform uniform, Matrix4 matrix4)
        {
            Shaders.Shader shader = Scene.GetShader(shaderTypes);
            draw(shader, p, uniform, matrix4);
        }
    }
}
