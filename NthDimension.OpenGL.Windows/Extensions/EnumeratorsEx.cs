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

namespace NthDimension.Rasterizer.Windows
{
    using System;

    public static class ConversionsEx
    {
        private static string serr = "Cast failed: ";

        #region Buffer Extensions
        public static OpenTK.Graphics.OpenGL.BufferParameterName ToOpenTK(this NthDimension.Rasterizer.BufferParameterName target)
        {
            OpenTK.Graphics.OpenGL.BufferParameterName ret = OpenTK.Graphics.OpenGL.BufferParameterName.BufferSize;

            if (!OpenTK.Graphics.OpenGL.BufferParameterName.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }
        public static OpenTK.Graphics.OpenGL.BufferTarget ToOpenTK(this NthDimension.Rasterizer.BufferTarget target)
        {
            OpenTK.Graphics.OpenGL.BufferTarget ret = OpenTK.Graphics.OpenGL.BufferTarget.ArrayBuffer;

            if(!OpenTK.Graphics.OpenGL.BufferTarget.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr+target.ToString());

            return ret;
        }
        public static OpenTK.Graphics.OpenGL.BufferUsageHint ToOpenTK(this NthDimension.Rasterizer.BufferUsageHint target)
        {
            OpenTK.Graphics.OpenGL.BufferUsageHint ret = OpenTK.Graphics.OpenGL.BufferUsageHint.StaticDraw;

            if(!OpenTK.Graphics.OpenGL.BufferUsageHint.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr+target.ToString());

            return ret;
        }
        public static OpenTK.Graphics.OpenGL.VertexAttribPointerType ToOpenTK(this NthDimension.Rasterizer.VertexAttribPointerType target)
        {
            OpenTK.Graphics.OpenGL.VertexAttribPointerType ret = OpenTK.Graphics.OpenGL.VertexAttribPointerType.Int;

            if (!OpenTK.Graphics.OpenGL.VertexAttribPointerType.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }
        #endregion

        #region Element Extensions

        public static OpenTK.Graphics.OpenGL.BeginMode ToOpenTK(this NthDimension.Rasterizer.BeginMode target)
        {
            OpenTK.Graphics.OpenGL.BeginMode ret = OpenTK.Graphics.OpenGL.BeginMode.Polygon;

            if(!OpenTK.Graphics.OpenGL.BeginMode.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr+target.ToString());

            return ret;
        }

        public static OpenTK.Graphics.OpenGL.DrawElementsType ToOpenTK(this NthDimension.Rasterizer.DrawElementsType target)
        {
            OpenTK.Graphics.OpenGL.DrawElementsType ret = OpenTK.Graphics.OpenGL.DrawElementsType.UnsignedInt;

            if(!OpenTK.Graphics.OpenGL.DrawElementsType.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr+target.ToString());

            return ret;
        }

        public static OpenTK.Graphics.OpenGL.PrimitiveType ToOpenTK(this NthDimension.Rasterizer.PrimitiveType target)
        {
            OpenTK.Graphics.OpenGL.PrimitiveType ret = OpenTK.Graphics.OpenGL.PrimitiveType.Triangles;

            if (!OpenTK.Graphics.OpenGL.PrimitiveType.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }
        #endregion

        #region Framebuffer Extensions
        public static OpenTK.Graphics.OpenGL.FramebufferErrorCode ToOpenTK(this NthDimension.Rasterizer.FramebufferErrorCode target)
        {
            OpenTK.Graphics.OpenGL.FramebufferErrorCode ret = OpenTK.Graphics.OpenGL.FramebufferErrorCode.FramebufferUndefined;

            if(!OpenTK.Graphics.OpenGL.FramebufferErrorCode.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr+target.ToString());

            return ret;
        }
        public static OpenTK.Graphics.OpenGL.FramebufferTarget ToOpenTK(this NthDimension.Rasterizer.FramebufferTarget target)
        {
            OpenTK.Graphics.OpenGL.FramebufferTarget ret = OpenTK.Graphics.OpenGL.FramebufferTarget.Framebuffer;

            if(!OpenTK.Graphics.OpenGL.FramebufferTarget.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }
        public static OpenTK.Graphics.OpenGL.FramebufferAttachment ToOpenTK(this NthDimension.Rasterizer.FramebufferAttachment target)
        {
            OpenTK.Graphics.OpenGL.FramebufferAttachment ret = OpenTK.Graphics.OpenGL.FramebufferAttachment.ColorAttachment0;//.Color;

            if(!OpenTK.Graphics.OpenGL.FramebufferAttachment.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }

        public static NthDimension.Rasterizer.FramebufferErrorCode ToNthDimension(OpenTK.Graphics.OpenGL.FramebufferErrorCode target)
        {
            NthDimension.Rasterizer.FramebufferErrorCode ret = NthDimension.Rasterizer.FramebufferErrorCode.FramebufferUndefined;

            if (!NthDimension.Rasterizer.FramebufferErrorCode.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }
        #endregion

        #region Renderbuffer Extensions

        public static OpenTK.Graphics.OpenGL.RenderbufferTarget ToOpenTK(this NthDimension.Rasterizer.RenderbufferTarget target)
        {
            OpenTK.Graphics.OpenGL.RenderbufferTarget ret = OpenTK.Graphics.OpenGL.RenderbufferTarget.Renderbuffer;

            if (!OpenTK.Graphics.OpenGL.RenderbufferTarget.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }

        public static OpenTK.Graphics.OpenGL.RenderbufferStorage ToOpenTK(this NthDimension.Rasterizer.RenderbufferStorage target)
        {
            OpenTK.Graphics.OpenGL.RenderbufferStorage ret = OpenTK.Graphics.OpenGL.RenderbufferStorage.Depth24Stencil8;

            if (!OpenTK.Graphics.OpenGL.RenderbufferStorage.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }

        public static OpenTK.Graphics.OpenGL.ReadBufferMode ToOpenTK(this NthDimension.Rasterizer.ReadBufferMode target)
        {
            OpenTK.Graphics.OpenGL.ReadBufferMode ret = OpenTK.Graphics.OpenGL.ReadBufferMode.ColorAttachment0;

            if (!OpenTK.Graphics.OpenGL.ReadBufferMode.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }

        public static OpenTK.Graphics.OpenGL.DrawBufferMode ToOpenTK(this NthDimension.Rasterizer.DrawBufferMode target)
        {
            OpenTK.Graphics.OpenGL.DrawBufferMode ret = OpenTK.Graphics.OpenGL.DrawBufferMode.Back;

            if (!OpenTK.Graphics.OpenGL.DrawBufferMode.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }
        #endregion

        #region GL Extensions

#if OPENTK3
        public static OpenTK.Graphics.OpenGL.BlendingFactor ToOpenTK(this NthDimension.OpenGL.BlendingFactor target)
        {
            OpenTK.Graphics.OpenGL.BlendingFactor ret = OpenTK.Graphics.OpenGL.BlendingFactor.One;

            if (!OpenTK.Graphics.OpenGL.BlendingFactor.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }
        public static OpenTK.Graphics.OpenGL.InternalFormat ToOpenTK(this NthDimension.OpenGL.InternalFormat target)
        {
            OpenTK.Graphics.OpenGL.InternalFormat ret = OpenTK.Graphics.OpenGL.InternalFormat.Alpha8;

            if (!OpenTK.Graphics.OpenGL.InternalFormat.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }
#endif
        public static OpenTK.Graphics.OpenGL.BlendingFactorDest ToOpenTK(this NthDimension.Rasterizer.BlendingFactorDest target)
        {
            OpenTK.Graphics.OpenGL.BlendingFactorDest ret = OpenTK.Graphics.OpenGL.BlendingFactorDest.One;

            if (!OpenTK.Graphics.OpenGL.BlendingFactorDest.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }
        public static OpenTK.Graphics.OpenGL.BlendingFactorSrc ToOpenTK(this NthDimension.Rasterizer.BlendingFactorSrc target)
        {
            OpenTK.Graphics.OpenGL.BlendingFactorSrc ret = OpenTK.Graphics.OpenGL.BlendingFactorSrc.One;

            if (!OpenTK.Graphics.OpenGL.BlendingFactorSrc.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }
        public static OpenTK.Graphics.OpenGL.BlitFramebufferFilter ToOpenTK(this NthDimension.Rasterizer.BlitFramebufferFilter target)
        {
            OpenTK.Graphics.OpenGL.BlitFramebufferFilter ret = OpenTK.Graphics.OpenGL.BlitFramebufferFilter.Linear;

            if (!OpenTK.Graphics.OpenGL.BlitFramebufferFilter.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }
        public static OpenTK.Graphics.OpenGL.ClearBufferMask ToOpenTK(this NthDimension.Rasterizer.ClearBufferMask target)
        {
            OpenTK.Graphics.OpenGL.ClearBufferMask ret = OpenTK.Graphics.OpenGL.ClearBufferMask.ColorBufferBit;//.None;

            if (!OpenTK.Graphics.OpenGL.ClearBufferMask.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }
        public static OpenTK.Graphics.OpenGL.CullFaceMode ToOpenTK(this NthDimension.Rasterizer.CullFaceMode target)
        {
            OpenTK.Graphics.OpenGL.CullFaceMode ret = OpenTK.Graphics.OpenGL.CullFaceMode.FrontAndBack;

            if(!OpenTK.Graphics.OpenGL.CullFaceMode.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr+target.ToString());

            return ret;
        }
        public static OpenTK.Graphics.OpenGL.DepthFunction ToOpenTK(this NthDimension.Rasterizer.DepthFunction target)
        {
            OpenTK.Graphics.OpenGL.DepthFunction ret = OpenTK.Graphics.OpenGL.DepthFunction.Never;

            if (!OpenTK.Graphics.OpenGL.DepthFunction.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }
        public static OpenTK.Graphics.OpenGL.EnableCap ToOpenTK(this NthDimension.Rasterizer.EnableCap target)
        {
            OpenTK.Graphics.OpenGL.EnableCap ret = OpenTK.Graphics.OpenGL.EnableCap.AlphaTest;

            if (!OpenTK.Graphics.OpenGL.EnableCap.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }

        public static OpenTK.Graphics.OpenGL.FrontFaceDirection ToOpenTK(this NthDimension.Rasterizer.FrontFaceDirection target)
        {
            OpenTK.Graphics.OpenGL.FrontFaceDirection ret = OpenTK.Graphics.OpenGL.FrontFaceDirection.Ccw;

            if (!OpenTK.Graphics.OpenGL.FrontFaceDirection.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }
        public static OpenTK.Graphics.OpenGL.GetPName ToOpenTK(this NthDimension.Rasterizer.GetPName target)
        {
            OpenTK.Graphics.OpenGL.GetPName ret = OpenTK.Graphics.OpenGL.GetPName.AutoNormal;

            if(!OpenTK.Graphics.OpenGL.GetPName.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr+target.ToString());

            return ret;
        }
        public static OpenTK.Graphics.OpenGL.StringName ToOpenTK(this NthDimension.Rasterizer.StringName target)
        {
            OpenTK.Graphics.OpenGL.StringName ret = OpenTK.Graphics.OpenGL.StringName.Version;

            if(!OpenTK.Graphics.OpenGL.StringName.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr+target.ToString());

            return ret;
        }

        public static NthDimension.Rasterizer.ErrorCode ToNthDimension(this OpenTK.Graphics.OpenGL.ErrorCode target)
        {
            NthDimension.Rasterizer.ErrorCode ret = ErrorCode.NoError;

            if(!NthDimension.Rasterizer.ErrorCode.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr+target.ToString());

            return ret;
        }

        public static OpenTK.Graphics.OpenGL.MatrixMode ToOpenTK(this MatrixMode target)
        {
            OpenTK.Graphics.OpenGL.MatrixMode ret = OpenTK.Graphics.OpenGL.MatrixMode.Modelview;

            if (!OpenTK.Graphics.OpenGL.MatrixMode.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }
#endregion

#region Pixel Extensions
        public static OpenTK.Graphics.OpenGL.PixelFormat ToOpenTK(this NthDimension.Rasterizer.PixelFormat target)
        {
            OpenTK.Graphics.OpenGL.PixelFormat ret = OpenTK.Graphics.OpenGL.PixelFormat.Bgra;

            if (!OpenTK.Graphics.OpenGL.PixelFormat.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }
        public static OpenTK.Graphics.OpenGL.PixelInternalFormat ToOpenTK(this NthDimension.Rasterizer.PixelInternalFormat target)
        {
            OpenTK.Graphics.OpenGL.PixelInternalFormat ret = OpenTK.Graphics.OpenGL.PixelInternalFormat.Alpha;

            if(!OpenTK.Graphics.OpenGL.PixelInternalFormat.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr+target.ToString());

            return ret;
        }
        public static OpenTK.Graphics.OpenGL.PixelType ToOpenTK(this NthDimension.Rasterizer.PixelType target)
        {
            OpenTK.Graphics.OpenGL.PixelType ret = OpenTK.Graphics.OpenGL.PixelType.Bitmap;

            if (!OpenTK.Graphics.OpenGL.PixelType.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }

        public static OpenTK.Graphics.OpenGL.StencilOp ToOpenTK(this NthDimension.Rasterizer.StencilOp target)
        {
            OpenTK.Graphics.OpenGL.StencilOp ret = OpenTK.Graphics.OpenGL.StencilOp.Zero;

            if (!OpenTK.Graphics.OpenGL.StencilOp.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }

        public static OpenTK.Graphics.OpenGL.StencilFunction ToOpenTK(this NthDimension.Rasterizer.StencilFunction target)
        {
            OpenTK.Graphics.OpenGL.StencilFunction ret = OpenTK.Graphics.OpenGL.StencilFunction.Never;

            if (!OpenTK.Graphics.OpenGL.StencilFunction.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }
#endregion

#region Shader Extension
        public static OpenTK.Graphics.OpenGL.ShaderType ToOpenTK(this NthDimension.Rasterizer.ShaderType target)
        {
            OpenTK.Graphics.OpenGL.ShaderType ret = OpenTK.Graphics.OpenGL.ShaderType.FragmentShader;

            if(!OpenTK.Graphics.OpenGL.ShaderType.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr+target.ToString());

            return ret;
        }

        public static OpenTK.Graphics.OpenGL.ShaderParameter ToOpenTK(this NthDimension.Rasterizer.ShaderParameter target)
        {
            OpenTK.Graphics.OpenGL.ShaderParameter ret = OpenTK.Graphics.OpenGL.ShaderParameter.InfoLogLength;

            if (!OpenTK.Graphics.OpenGL.ShaderParameter.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }

        public static OpenTK.Graphics.OpenGL.GetProgramParameterName ToOpenTK(this NthDimension.Rasterizer.GetProgramParameterName target)
        {
            OpenTK.Graphics.OpenGL.GetProgramParameterName ret = OpenTK.Graphics.OpenGL.GetProgramParameterName.LinkStatus;

            if (!OpenTK.Graphics.OpenGL.GetProgramParameterName.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }
#endregion

#region Texture Extensions

        public static OpenTK.Graphics.OpenGL.GenerateMipmapTarget ToOpenTK(this NthDimension.Rasterizer.GenerateMipmapTarget target)
        {
            OpenTK.Graphics.OpenGL.GenerateMipmapTarget ret = OpenTK.Graphics.OpenGL.GenerateMipmapTarget.Texture1D;

            if(!OpenTK.Graphics.OpenGL.GenerateMipmapTarget.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr+target.ToString());

                return ret;
        }
        public static OpenTK.Graphics.OpenGL.GetTextureParameter ToOpenTK(this NthDimension.Rasterizer.GetTextureParameter target)
        {
            OpenTK.Graphics.OpenGL.GetTextureParameter ret = OpenTK.Graphics.OpenGL.GetTextureParameter.DepthTextureMode;

            if (!OpenTK.Graphics.OpenGL.GetTextureParameter.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }
        public static OpenTK.Graphics.OpenGL.TextureMinFilter ToOpenTK(this NthDimension.Rasterizer.TextureMinFilter target)
        {
            OpenTK.Graphics.OpenGL.TextureMinFilter ret = OpenTK.Graphics.OpenGL.TextureMinFilter.Linear;

            if (!OpenTK.Graphics.OpenGL.TextureMinFilter.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }
        public static OpenTK.Graphics.OpenGL.TextureMagFilter ToOpenTK(this NthDimension.Rasterizer.TextureMagFilter target)
        {
            OpenTK.Graphics.OpenGL.TextureMagFilter ret = OpenTK.Graphics.OpenGL.TextureMagFilter.Linear;

            if (!OpenTK.Graphics.OpenGL.TextureMagFilter.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }
        public static OpenTK.Graphics.OpenGL.TextureParameterName ToOpenTK(this NthDimension.Rasterizer.TextureParameterName target)
        {
            OpenTK.Graphics.OpenGL.TextureParameterName ret = OpenTK.Graphics.OpenGL.TextureParameterName.TextureBaseLevel;

            if (!OpenTK.Graphics.OpenGL.TextureParameterName.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }
        public static OpenTK.Graphics.OpenGL.TextureTarget ToOpenTK(this NthDimension.Rasterizer.TextureTarget target)
        {
            OpenTK.Graphics.OpenGL.TextureTarget ret = OpenTK.Graphics.OpenGL.TextureTarget.ProxyTexture1D;

            if (!OpenTK.Graphics.OpenGL.TextureTarget.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }
        public static OpenTK.Graphics.OpenGL.TextureUnit ToOpenTK(this NthDimension.Rasterizer.TextureUnit target)
        {
            OpenTK.Graphics.OpenGL.TextureUnit ret = OpenTK.Graphics.OpenGL.TextureUnit.Texture0;

            if (!OpenTK.Graphics.OpenGL.TextureUnit.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }
        public static OpenTK.Graphics.OpenGL.TextureWrapMode ToOpenTK(this NthDimension.Rasterizer.TextureWrapMode target)
        {
            OpenTK.Graphics.OpenGL.TextureWrapMode ret = OpenTK.Graphics.OpenGL.TextureWrapMode.Clamp;

            if (!OpenTK.Graphics.OpenGL.TextureWrapMode.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }
        #endregion

        #region Audio (OpenAL) Extension
        public static OpenTK.Audio.OpenAL.ALFormat ToOpenTK(this NthDimension.Rasterizer.AudioBase.ALFormat target)
        {
            OpenTK.Audio.OpenAL.ALFormat ret = OpenTK.Audio.OpenAL.ALFormat.Stereo16;

            if (!OpenTK.Audio.OpenAL.ALFormat.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }
        public static OpenTK.Audio.OpenAL.ALGetSourcei ToOpenTK(this NthDimension.Rasterizer.AudioBase.ALGetSourcei target)
        {
            OpenTK.Audio.OpenAL.ALGetSourcei ret = OpenTK.Audio.OpenAL.ALGetSourcei.Buffer;

            if (!OpenTK.Audio.OpenAL.ALGetSourcei.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }
        public static OpenTK.Audio.OpenAL.ALSourceState ToOpenTK(this NthDimension.Rasterizer.AudioBase.ALSourceState target)
        {
            OpenTK.Audio.OpenAL.ALSourceState ret = OpenTK.Audio.OpenAL.ALSourceState.Stopped;

            if (!OpenTK.Audio.OpenAL.ALSourceState.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }
        public static OpenTK.Audio.OpenAL.ALSourcei ToOpenTK(this NthDimension.Rasterizer.AudioBase.ALSourcei target)
        {
            OpenTK.Audio.OpenAL.ALSourcei ret = OpenTK.Audio.OpenAL.ALSourcei.SourceType;

            if (!OpenTK.Audio.OpenAL.ALSourcei.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }
        public static OpenTK.Audio.OpenAL.ALSourceb ToOpenTK(this NthDimension.Rasterizer.AudioBase.ALSourceb target)
        {
            OpenTK.Audio.OpenAL.ALSourceb ret = OpenTK.Audio.OpenAL.ALSourceb.SourceRelative;

            if (!OpenTK.Audio.OpenAL.ALSourceb.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }
        public static OpenTK.Audio.OpenAL.ALSourcef ToOpenTK(this NthDimension.Rasterizer.AudioBase.ALSourcef target)
        {
            OpenTK.Audio.OpenAL.ALSourcef ret = OpenTK.Audio.OpenAL.ALSourcef.Gain;

            if (!OpenTK.Audio.OpenAL.ALSourcef.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }
        public static OpenTK.Audio.OpenAL.ALSource3f ToOpenTK(this NthDimension.Rasterizer.AudioBase.ALSource3f target)
        {
            OpenTK.Audio.OpenAL.ALSource3f ret = OpenTK.Audio.OpenAL.ALSource3f.Position;

            if (!OpenTK.Audio.OpenAL.ALSource3f.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }
        public static OpenTK.Audio.OpenAL.ALListener3f ToOpenTK(this NthDimension.Rasterizer.AudioBase.ALListener3f target)
        {
            OpenTK.Audio.OpenAL.ALListener3f ret = OpenTK.Audio.OpenAL.ALListener3f.Position;

            if (!OpenTK.Audio.OpenAL.ALListener3f.TryParse(target.ToString(), false, out ret))
                throw new Exception(serr + target.ToString());

            return ret;
        }
        #endregion Audio (OpenAL) Extensions

    }
}
