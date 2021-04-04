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

// ------------------------------------------------------------------------------------------
// (C) Copyright 2017, Stylianos N. Polychroniadis
// ------------------------------------------------------------------------------------------

// Hardware Abstraction Layer Interface - Using OpenGL 3.x via OpenTK
// *
// * Revision History
// *    May-31-2017      - Rev.1.0   S.Polychroniadis


using System.Diagnostics.Contracts;
using System.Net.Sockets;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace NthDimension.Rasterizer.Windows
{
    using System;

    using NthDimension.Algebra;
    using NthDimension.Rasterizer;
    public partial class RendererGL3x : RendererBaseGL3
    {
        
        public override void ActiveTexture(TextureUnit texunit)
        {
            OpenTK.Graphics.OpenGL.GL.ActiveTexture(texunit.ToOpenTK());
        }
        public override void AttachShader(int program, int shader)
        {
            OpenTK.Graphics.OpenGL.GL.AttachShader(program, shader);
        }
        public override void BindBuffer(BufferTarget target, int buffer)
        {
            OpenTK.Graphics.OpenGL.GL.BindBuffer(target.ToOpenTK(), buffer);
        }
        public override void BindTexture(TextureTarget target, int texture)
        {
            OpenTK.Graphics.OpenGL.GL.BindTexture(target.ToOpenTK(), texture);
        }
        public override void BindTexture(TextureTarget target, uint texture)
        {
            OpenTK.Graphics.OpenGL.GL.BindTexture(target.ToOpenTK(), texture);
        }
        public override void BindImageTexture(int unit, int texture, int level, bool layered, int layer, TextureAccess access, SizedInternalFormat format)
        { 
            OpenTK.Graphics.OpenGL.GL.BindImageTexture(unit, texture, level, layered, layer, access.ToOpenTK(), format.ToOpenTK());
        }
        public override void BindVertexArray(int array)
        {
            OpenTK.Graphics.OpenGL.GL.BindVertexArray(array);
        }

#if OPENTK3
        public override void BlendFunc(BlendingFactor src, BlendingFactor dest)
        {
            
            OpenTK.Graphics.OpenGL.GL.BlendFunc(src.ToOpenTK(), dest.ToOpenTK());
        }
#else
        public override void BlendFunc(BlendingFactorSrc src, BlendingFactorDest dest)
        {

            OpenTK.Graphics.OpenGL.GL.BlendFunc(src.ToOpenTK(), dest.ToOpenTK());
        }
#endif

        public override void BufferData<T2>(BufferTarget target, IntPtr size, T2[] data, BufferUsageHint usage)
        {
            OpenTK.Graphics.OpenGL.GL.BufferData<T2>(target.ToOpenTK(), size, data, usage.ToOpenTK());
        }
        public override void Clear(ClearBufferMask mask)
        {
            OpenTK.Graphics.OpenGL.GL.Clear(mask.ToOpenTK());
        }
        public override void ClearColor(Color4 color)
        {
            OpenTK.Graphics.OpenGL.GL.ClearColor(color.ToOpenTK());
        }
        public override void ColorMask(bool red, bool green, bool blue, bool alpha)
        {
            OpenTK.Graphics.OpenGL.GL.ColorMask(red, green, blue, alpha);
        }
        public override void CompileShader(int shader)
        {
            OpenTK.Graphics.OpenGL.GL.CompileShader(shader);
        }

#if OPENTK3
        public override void CompressedTexImage2D(TextureTarget target, int level, InternalFormat internalFormat, int width, int height, int border, int imagesize, IntPtr data)
        {
            OpenTK.Graphics.OpenGL.GL.CompressedTexImage2D(target.ToOpenTK(), level, internalFormat.ToOpenTK(), width, height, border, imagesize, data);
        }
#else
        public override void CompressedTexImage2D(TextureTarget target, int level, PixelInternalFormat internalFormat, int width, int height, int border, int imagesize, byte[] data)
        {
            // Revision 2 -> conversion of byte[] to IntPtr due to threaded asset loading DDS errors

            //OpenTK.Graphics.OpenGL.GL.CompressedTexImage2D(target.ToOpenTK(), level, internalFormat.ToOpenTK(), width, height, border, imagesize, data);      // this is Rev.1
            IntPtr unmanagedPointer = System.Runtime.InteropServices.Marshal.AllocHGlobal(data.Length);
            System.Runtime.InteropServices.Marshal.Copy(data, 0, unmanagedPointer, data.Length);
            OpenTK.Graphics.OpenGL.GL.CompressedTexImage2D(target.ToOpenTK(), level, internalFormat.ToOpenTK(), width, height, border, imagesize, unmanagedPointer);
            System.Runtime.InteropServices.Marshal.FreeHGlobal(unmanagedPointer);

        }
#endif
        public override int CreateProgram()
        {
            return OpenTK.Graphics.OpenGL.GL.CreateProgram();
        }
        public override int CreateShader(ShaderType type)
        {
            return OpenTK.Graphics.OpenGL.GL.CreateShader(type.ToOpenTK());
        }
        public override void CullFace(CullFaceMode mode)
        {
            OpenTK.Graphics.OpenGL.GL.CullFace(mode.ToOpenTK());
        }
        public override void DeleteTexture(int texture)
        {
            OpenTK.Graphics.OpenGL.GL.DeleteTexture(texture);
        }
        public override void DeleteTextures(int n, ref uint textures)
        {
            OpenTK.Graphics.OpenGL.GL.DeleteTextures(n, ref textures);
        }
        public override void DeleteFrameBuffer(uint framebuffer)
        {
            OpenTK.Graphics.OpenGL.GL.DeleteFramebuffer(framebuffer);
        }
        public override void DeleteFrameBuffers(int n, ref uint framebuffer)
        {
            OpenTK.Graphics.OpenGL.GL.DeleteFramebuffers(n, ref framebuffer);
        }
        public override void DepthFunc(DepthFunction function)
        {
            OpenTK.Graphics.OpenGL.GL.DepthFunc(function.ToOpenTK());
        }
        public override void DepthMask(bool flag)
        {
            if (flag != depthMask)
            {
                depthMask = flag;
                OpenTK.Graphics.OpenGL.GL.DepthMask(flag);
            }
        }
        public override void DetachShader(int program, int shader)
        {
            OpenTK.Graphics.OpenGL.GL.DetachShader(program, shader);
        }
        protected override void Disable(EnableCap disable)
        {
            OpenTK.Graphics.OpenGL.GL.Disable(disable.ToOpenTK());
        }
        public override void DispatchCompute(int numGroupsX, int numGroupsY, int numGroupsZ)
        {
            throw new NotImplementedException("GLSL 430 not implemented");
        }
        public override void DrawElements(PrimitiveType mode, int count, DrawElementsType type, IntPtr indices) // WARNING: Func Obsolete use PrimitiveType overload
        {
            //if(indices != IntPtr.Zero)
                OpenTK.Graphics.OpenGL.GL.DrawElements(mode.ToOpenTK(), count, type.ToOpenTK(), indices);
        }
        /// <summary>
        /// Experimenting 'Approaching Zero Driver Overhead' draw calls
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="count"></param>
        /// <param name="type"></param>
        /// <param name="indices"></param>
        public override void DrawRangeElements(PrimitiveType mode, int start, int end, int count, DrawElementsType type, IntPtr indices)
        {
            OpenTK.Graphics.OpenGL.GL.DrawRangeElements(mode.ToOpenTK(), start, end, count, type.ToOpenTK(), indices);
        }
        public override void DrawArrays(PrimitiveType primitive, int start, int end)
        {
            OpenTK.Graphics.OpenGL.GL.DrawArrays(primitive.ToOpenTK(), start, end);
        }
        protected override void Enable(EnableCap enable)
        {
            OpenTK.Graphics.OpenGL.GL.Enable(enable.ToOpenTK());
        }
        public override void EnableVertexAttribArray(int index)
        {
            OpenTK.Graphics.OpenGL.GL.EnableVertexAttribArray(index);
        }
        public override void Finish()
        {
            OpenTK.Graphics.OpenGL.GL.Finish();
        }
        public override void Flush()
        {
            OpenTK.Graphics.OpenGL.GL.Flush();
        }
        public override void FramebufferTexture2D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget texTarget, int texture, int level)
        {
            OpenTK.Graphics.OpenGL.GL.FramebufferTexture2D(target.ToOpenTK(),
                                                           attachment.ToOpenTK(),
                                                           texTarget.ToOpenTK(),
                                                           texture,
                                                           level);
        }
        public override void GenBuffers(int n, int[] buffers)
        {
            OpenTK.Graphics.OpenGL.GL.GenBuffers(n, buffers);
        }
        public override void GenBuffers(int n, out int buffers)
        {
            OpenTK.Graphics.OpenGL.GL.GenBuffers(n, out buffers);
        }
        public override void GenerateMipmap(GenerateMipmapTarget target)
        {
            OpenTK.Graphics.OpenGL.GL.GenerateMipmap(target.ToOpenTK());
        }
        public override int GenTexture()
        {
            return OpenTK.Graphics.OpenGL.GL.GenTexture();
        }
        public override void GenTextures(int n, out int textures)
        {
            OpenTK.Graphics.OpenGL.GL.GenTextures(n, out textures);
        }
        public override void GenTextures(int n, out uint textures)
        {
            OpenTK.Graphics.OpenGL.GL.GenTextures(n, out textures);
        }
        public override void GenVertexArrays(int n, out int arrays)
        {
            OpenTK.Graphics.OpenGL.GL.GenVertexArrays(n, out arrays);
        }

        public override void GetBufferParameter(BufferTarget target, BufferParameterName name, out int result)
        {
            OpenTK.Graphics.OpenGL.GL.GetBufferParameter(target.ToOpenTK(), name.ToOpenTK(), out result);
        }

        public override int GetAttribLocation(int program, string name)
        {
            return OpenTK.Graphics.OpenGL.GL.GetAttribLocation(program, name);
        }
        public override ErrorCode GetError()
        {
            return OpenTK.Graphics.OpenGL.GL.GetError().ToNthDimension();
        }
        public override void GetFloat(GetPName pname, float[] @params)
        {
            OpenTK.Graphics.OpenGL.GL.GetFloat(pname.ToOpenTK(), @params);
        }
        public override void GetInteger(GetPName pname, out int @params)
        {
            OpenTK.Graphics.OpenGL.GL.GetInteger(pname.ToOpenTK(), out @params);
        }
        public override void GetInteger(GetPName pname, int[] @params)
        {
            OpenTK.Graphics.OpenGL.GL.GetInteger(pname.ToOpenTK(), @params);
        }
        public override void GetProgram(int program, GetProgramParameterName programParameterName, out int linkStatus)
        {
            OpenTK.Graphics.OpenGL.GL.GetProgram(program, programParameterName.ToOpenTK(), out linkStatus);
        }
        public override string GetProgramInfoLog(int program)
        {
            return OpenTK.Graphics.OpenGL.GL.GetProgramInfoLog(program);
        }
        public override void GetShader(int shader, ShaderParameter pname, out int @params)
        {
            OpenTK.Graphics.OpenGL.GL.GetShader(shader, pname.ToOpenTK(), out @params);
        }
        public override string GetShaderInfoLog(int shader)
        {
            return OpenTK.Graphics.OpenGL.GL.GetShaderInfoLog(shader);
        }
        public override void GetShaderInfoLog(int shader, out string info)
        {
            OpenTK.Graphics.OpenGL.GL.GetShaderInfoLog(shader, out info);
        }

        public override string GetString(StringName name)
        {
            return OpenTK.Graphics.OpenGL.GL.GetString(name.ToOpenTK());
        }
        public override void GetTexLevelParameter(TextureTarget target, int level, GetTextureParameter pname, out int @params)
        {
            OpenTK.Graphics.OpenGL.GL.GetTexLevelParameter(target.ToOpenTK(), level, pname.ToOpenTK(), out @params);
        }
        public override void GetTexParameter(TextureTarget target, GetTextureParameter pname, out int @params)
        {
            OpenTK.Graphics.OpenGL.GL.GetTexParameter(target.ToOpenTK(), pname.ToOpenTK(), out @params);
        }
        public override int GetUniformLocation(int program, string name)
        {
            return OpenTK.Graphics.OpenGL.GL.GetUniformLocation(program, name);
        }
        public override void LinkProgram(int program)
        {
            OpenTK.Graphics.OpenGL.GL.LinkProgram(program);
        }

        /// <summary>
        /// Render multiple sets of primitives by specifying indices of array data elements
        /// </summary>
        /// <param name="mode">The kind od primitives to render</param>
        /// <param name="count">Points to an array of the elements counts</param>
        /// <param name="type">Specifies the type of the values in indices, must be UnsignedByte, UnsignedShort, UnsignedInt</param>
        /// <param name="indices">Specifies a pointer to the location where indices are stored</param>
        /// <param name="arraySize">Specifies the size of the count and indices arrays </param>
        public override void MultiDrawElements(PrimitiveType mode, int[] count, DrawElementsType type, IntPtr indices, int arraySize)
        {
            OpenTK.Graphics.OpenGL.GL.MultiDrawElements(mode.ToOpenTK(), count, type.ToOpenTK(), indices, arraySize);


        }

        public override void PushMatrix()
        {
            OpenTK.Graphics.OpenGL.GL.PushMatrix();
        }
        public override void PopMatrix()
        {
            OpenTK.Graphics.OpenGL.GL.PopMatrix();
        }
        public override void ReadPixels(int x, int y, int width, int height, PixelFormat format, PixelType type, IntPtr pixels)
        {
            OpenTK.Graphics.OpenGL.GL.ReadPixels(x, y, width, height, format.ToOpenTK(), type.ToOpenTK(), pixels);

        }
        public override void ShaderSource(int shader, string @string)
        {
            OpenTK.Graphics.OpenGL.GL.ShaderSource(shader, @string);
        }
        public override void TexImage2D(TextureTarget target, int level, PixelInternalFormat internalformat, int width, int height, int border, PixelFormat format, PixelType type, IntPtr pixels)
        {
            OpenTK.Graphics.OpenGL.GL.TexImage2D(target.ToOpenTK(), level, internalformat.ToOpenTK(), width, height, border, format.ToOpenTK(), type.ToOpenTK(), pixels);
        }
        public override void TexImage3D(TextureTarget target, int level, PixelInternalFormat internalformat, int width, int height, int depth, int border, PixelFormat format, PixelType type, IntPtr pixels)
        {
            OpenTK.Graphics.OpenGL.GL.TexImage3D(target.ToOpenTK(), level, internalformat.ToOpenTK(), width, height, depth, border, format.ToOpenTK(), type.ToOpenTK(), pixels);
        }
        public override void TexParameter(TextureTarget target, TextureParameterName pname, int param)
        {
            OpenTK.Graphics.OpenGL.GL.TexParameter(target.ToOpenTK(), pname.ToOpenTK(), param);
        }
        public override void Uniform1(int location, int v0)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform1(location, v0);
        }
        public override void Uniform1(int location, int count, ref int value)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform1(location, count, ref value);
        }
        public override void Uniform1(int location, int count, ref float value)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform1(location, count, ref value);
        }
        public override void Uniform2(int location, ref Vector2 vector)
        {
            OpenTK.Vector2 ret = vector.ToOpenTK();
            OpenTK.Graphics.OpenGL.GL.Uniform2(location, ref ret);
            vector = ret.ToNthDimension();
        }
        public override void Uniform3(int location, ref Vector3 vector)
        {
            OpenTK.Vector3 ret = vector.ToOpenTK();
            OpenTK.Graphics.OpenGL.GL.Uniform3(location, ref ret);
            vector = ret.ToNthDimension();
        }
        public override void Uniform4(int location, ref Vector4 vector)
        {
            OpenTK.Vector4 ret = vector.ToOpenTK();
            OpenTK.Graphics.OpenGL.GL.Uniform4(location, ref ret);
            vector = ret.ToNthDimension();
        }
        public override void UniformMatrix3(int location, bool transpose, ref Matrix3 matrix)
        {
            OpenTK.Matrix3 ret = matrix.ToOpenTK();
            OpenTK.Graphics.OpenGL.GL.UniformMatrix3(location, transpose, ref ret);
            matrix = ret.ToNthDimension();
        }
        public override void UniformMatrix4(int location, bool transpose, ref Matrix4 matrix)
        {
            OpenTK.Matrix4 ret = matrix.ToOpenTK();
            OpenTK.Graphics.OpenGL.GL.UniformMatrix4(location, transpose, ref ret);
            matrix = ret.ToNthDimension();
        }
        public override void UseProgram(int program, bool force = false)
        {
            if (!force)
                if (LastShaderHandle == program)
                    return;

            LastShaderHandle = program;
            OpenTK.Graphics.OpenGL.GL.UseProgram(program);
        }
        public override void ValidateProgram(int program)
        {
            OpenTK.Graphics.OpenGL.GL.ValidateProgram(program);
        }
        public override void VertexAttribPointer(int index, int size, VertexAttribPointerType type, bool normalized, int stride, IntPtr pointer)
        {
            OpenTK.Graphics.OpenGL.GL.VertexAttribPointer(index, size, type.ToOpenTK(), normalized, stride, pointer);
        }
        public override void Viewport(int x, int y, int width, int height)
        {
            OpenTK.Graphics.OpenGL.GL.Viewport(x, y, width, height);

        }
        // /////////////////////////////////// Ext /////////////////////////////////////////
        public override void BindFramebuffer(FramebufferTarget target, int buffer)
        {
            OpenTK.Graphics.OpenGL.GL.Ext.BindFramebuffer(target.ToOpenTK(),
                                                          buffer);
        }

        public override FramebufferErrorCode CheckFrameBufferStatus(FramebufferTarget target)
        {
            return ConversionsEx.ToNthDimension(OpenTK.Graphics.OpenGL.GL.CheckFramebufferStatus(target.ToOpenTK()));
        }

        public override void FrameBufferTexture2D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget textarget, int texture, int level)
        {
            OpenTK.Graphics.OpenGL.GL.Ext.FramebufferTexture2D(target.ToOpenTK(),
                                                               attachment.ToOpenTK(),
                                                                textarget.ToOpenTK(),
                                                                texture,
                                                                level);
        }

        public override void GenFrameBuffers(int n, out int framebuffers)
        {
            OpenTK.Graphics.OpenGL.GL.GenFramebuffers(n, out framebuffers);
        }

        public override void GenRenderBuffers(int n, out int framebuffers)
        {
            OpenTK.Graphics.OpenGL.GL.GenRenderbuffers(n, out framebuffers);
        }
        // /////////////////////////////////// NanoVG //////////////////////////////////////
        public override void FrontFace(FrontFaceDirection mode)
        {
            OpenTK.Graphics.OpenGL.GL.FrontFace(mode.ToOpenTK());
        }

        public override void StencilMask(int mask)
        {
            OpenTK.Graphics.OpenGL.GL.StencilMask(mask);
        }
        public override void StencilMask(uint mask)
        {
            OpenTK.Graphics.OpenGL.GL.StencilMask(mask);
        }


        public override void StencilOp(StencilOp fail, StencilOp zfail, StencilOp zpass)
        {
            OpenTK.Graphics.OpenGL.GL.StencilOp(fail.ToOpenTK(), zfail.ToOpenTK(), zpass.ToOpenTK());
        }

        public override void StencilFunc(StencilFunction func, int @ref, int mask)
        {
            OpenTK.Graphics.OpenGL.GL.StencilFunc(func.ToOpenTK(), @ref, mask);
        }
        public override void StencilFunc(StencilFunction func, int @ref, uint mask)
        {
            OpenTK.Graphics.OpenGL.GL.StencilFunc(func.ToOpenTK(), @ref, mask);
        }
        // /////////////////////////////////// Helper Routines
        public override bool HasContext()
        {
            if (GraphicsContext.CurrentContext == null)
                return false;
            else
                return true;
        }


        // /////////////////////////////////// Experimental

    }
}
