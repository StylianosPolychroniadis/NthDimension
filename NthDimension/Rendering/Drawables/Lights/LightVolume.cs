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


using NthDimension.Rendering.Drawables.Lights;
using NthDimension.Rendering.GameViews;
using NthDimension.Rendering.Geometry;
using NthDimension.Rendering.Shaders;
using NthDimension.Rendering.Utilities;

namespace NthDimension.Rendering.Drawables.Models
{
    using System;

    using NthDimension.Algebra;
    using NthDimension.Rasterizer;
    using Rendering.Drawables.Lights;
    using Rendering.Geometry;
    using Rendering.Shaders;

#if _WINDOWS_
    //using NthDimension.OpenGL.Windows.GLSL.API3x;
#endif

    public sealed class LightVolume : Model
    {
        Light light;

        public LightVolume(ApplicationObject parent)
        {
            Parent = parent;
            light = (Light)parent;
            this.IgnoreCulling = true;
        }

        //public override void Update()
        //{
        //    base.Update();

        //    updateModelMatrix();
        //}

        internal void draw(int[] textures, ref ViewInfo curView)
        {
            Shaders.Shader shader = materials[0].shader;
            MeshVbo curMesh = meshes[0];

            drawEffect(curMesh, shader, textures, ref curView);
            
        }

        private void drawEffect(MeshVbo curMesh, Shader shader, int[] textures, ref ViewInfo curView)
        {
            ApplicationBase.Instance.CheckGlError(string.Format("(!) OpenGL Error: Draw LightVolume {0}:{1} Shade: {2}", this.GetType(), Name, shader.Name));

            Factories.ShaderLoader.UpdateShaderDebug(shader);

            ApplicationBase.Instance.Renderer.UseProgram(shader.Handle);

            #region Log Validate Status
#if !OPTIMIZED
            int validateStatus = -1;
            ApplicationBase.Instance.Renderer.ValidateProgram(shader.Handle);
            ApplicationBase.Instance.Renderer.GetProgram(shader.Handle, GetProgramParameterName.ValidateStatus, out validateStatus);
            if (validateStatus != 1)
                ConsoleUtil.log(string.Format("\tValidate Status: {0}:{1}", shader.Name, validateStatus));
#endif
            #endregion Check Validate Status

            shader.InsertUniform(Uniform.in_screensize, ref ApplicationBase.Instance.VAR_ScreenSize_Virtual);     // Uniform location reported -1
            shader.InsertUniform(Uniform.in_rendersize, ref ApplicationBase.Instance.VAR_ScreenSize_Current);     // should be (128, 128);
            shader.InsertUniform(Uniform.in_time, ref ApplicationBase.Instance.VAR_FrameTime);
            shader.InsertUniform(Uniform.invMVPMatrix, ref curView.invModelviewProjectionMatrix);

            shader.InsertUniform(Uniform.viewUp, ref curView.pointingDirectionUp);
            shader.InsertUniform(Uniform.viewRight, ref curView.pointingDirectionRight);

            Vector3 curViewPointingDirection = curView.PointingDirection * curView.zFar;
            shader.InsertUniform(Uniform.viewDirection, ref curViewPointingDirection);

            Vector3 curViewPosition = curView.Position;
            shader.InsertUniform(Uniform.viewPosition, ref curViewPosition);

            if (Scene != null)
            {
                shader.InsertUniform(Uniform.in_no_lights, ref Scene.LightCount);
                shader.InsertUniform(Uniform.curLight, ref Scene.CurrentLight);
                shader.InsertUniform(Uniform.in_eyepos, ref Scene.EyePos);          // Uniform location reported -1         
            }

            SetupMatrices(ref curView, ref shader, ref curMesh);

            light.activateDeferred(shader);
            initTextures(textures, shader.Handle, "Texture");

            ApplicationBase.Instance.Renderer.BindVertexArray(vaoHandle[0]);
            ApplicationBase.Instance.Renderer.DrawElements(PrimitiveType, meshes[0].MeshData.Indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);

            ApplicationBase.Instance.CheckGlError(string.Format("(!) OpenGL Error: Draw LightVolume {0}:{1} Shade: {2}", this.GetType(), Name, shader.Name));
        }

    }
}
