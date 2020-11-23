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

namespace NthDimension.Rasterizer
{
    using NthDimension.Algebra;
    using System;

    /// <summary>
    /// NthDimension OpenGL GLSL 3.0 Renderer
    /// </summary>
    public abstract partial class RendererBaseGL3
    {
        #region State Switches
        protected bool cullFace;
        protected bool blend;
        protected bool depthTest;
        protected bool multisample;
        protected bool depthMask;
        protected bool texture2d;


        public bool BlendEnabled
        {
            get { return blend; }
            set
            {
                if (value != blend)
                {
                    blend = value;

                    if (blend) Enable(EnableCap.Blend);
                    if (!blend) Disable(EnableCap.Blend);
                }
            }
        }

        public bool CullFaceEnabled
        {
            get { return cullFace; }
            set
            {
                if (value != cullFace)
                {
                    cullFace = value;

                    if (cullFace) Enable(EnableCap.CullFace);
                    if (!cullFace) Disable(EnableCap.CullFace);
                }
            }
        }

        public bool DepthTestEnabled
        {
            get { return depthTest; }
            set
            {
                if (value != depthTest)
                {
                    depthTest = value;

                    if (depthTest) Enable(EnableCap.DepthTest);
                    if (!depthTest) Disable(EnableCap.DepthTest);
                }
            }
        }

        public bool MultisampleEnabled
        {
            get { return multisample; }
            set
            {
                if (value != multisample)
                {
                    multisample = value;

                    if (depthTest) Enable(EnableCap.Multisample);
                    if (!depthTest) Disable(EnableCap.Multisample);
                }
            }
        }

        public bool Texture2DEnabled
        {
            get { return texture2d; }
            set
            {
                if (value != texture2d)
                {
                    texture2d = value;

                    if (texture2d) Enable(EnableCap.Texture2D);
                    if (!texture2d) Disable(EnableCap.Texture2D);
                }
            }
        }


        #endregion

        public static int LastShaderHandle = -256;
        public virtual void ActiveTexture(TextureUnit texunit) { }
        public virtual void AttachShader(int program, int shader) { }
        public virtual void BindBuffer(BufferTarget target, int buffer) { }
        public virtual void BindTexture(TextureTarget target, int texture) { }
        public virtual void BindTexture(TextureTarget target, uint texture) { }
        public virtual void BindImageTexture(int unit, int texture, int level, bool layered, int layer, TextureAccess access, SizedInternalFormat format) { }
        public virtual void BindVertexArray(int array) { }
#if OPENTK3
        public virtual void BlendFunc(BlendingFactor src, BlendingFactor dest) { }
#else
        public virtual void BlendFunc(BlendingFactorSrc src, BlendingFactorDest dest) { }
#endif
        public virtual void BufferData<T2>(BufferTarget target, IntPtr size, T2[] data, BufferUsageHint usage) where T2 : struct { }
        public virtual void Clear(ClearBufferMask mask) { }
        public virtual void ClearColor(Color4 color) { }
        public virtual void ColorMask(bool red, bool green, bool blue, bool alpha) { }
        public virtual void CompileShader(int shader) { }
#if OPENTK3
        public virtual void CompressedTexImage2D(TextureTarget target, int level, InternalFormat internalFormat, int width, int height, int border, int imagesize, IntPtr data) { }
#else
        public virtual void CompressedTexImage2D(TextureTarget target, int level, PixelInternalFormat internalFormat, int width, int height, int border, int imagesize, byte[] data) { }
#endif
        public virtual int CreateProgram() { return -1; }
        public virtual int CreateShader(ShaderType type) { return -1; }
        public virtual void CullFace(CullFaceMode mode) { }
        public virtual void DeleteTexture(int texture) { }
        public virtual void DeleteTextures(int n, ref uint textures) { }
        public virtual void DeleteFrameBuffer(uint framebuffer) { }
        public virtual void DeleteFrameBuffers(int n, ref uint framebuffer) { }
        public virtual void DepthFunc(DepthFunction function) { }
        public virtual void DepthMask(bool flag) { }
        public virtual void DetachShader(int program, int shader) { }
        protected virtual void Disable(EnableCap disable) { }
        public virtual void DispatchCompute(int numGroupsX, int numGroupsY, int numGroupsZ) { }

        public virtual void DrawElements(PrimitiveType mode, int count, DrawElementsType type, IntPtr indices) { }
        public virtual void DrawRangeElements(PrimitiveType mode, int start, int end, int count, DrawElementsType type, IntPtr indices) { }
        public virtual void DrawArrays(PrimitiveType primitive, int start, int end) { }
        protected virtual void Enable(EnableCap enable) { }
        public virtual void EnableVertexAttribArray(int index) { }
        public virtual void Finish() { }
        public virtual void Flush() { }
        public virtual void FramebufferTexture2D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget texTarget, int texture, int level) { }
        public virtual void GenBuffers(int n, int[] buffers) { }
        public virtual void GenBuffers(int n, out int buffers) { buffers = -1; }
        public virtual void GenerateMipmap(GenerateMipmapTarget target) { }
        public virtual int GenTexture() { return -1; }
        public virtual void GenTextures(int n, out int textures) { textures = -1; }
        public virtual void GenTextures(int n, out uint textures) { textures = 0; }
        public virtual void GenVertexArrays(int n, out int arrays) { arrays = -1; }
        public virtual int GetAttribLocation(int program, string name) { return -1; }
        public virtual void GetBufferParameter(BufferTarget target, BufferParameterName name, out int result) { result = -1; } 
        public virtual ErrorCode GetError() { return ErrorCode.NoError; }
        public virtual void GetFloat(GetPName pname, float[] @params) { }
        public virtual void GetInteger(GetPName pname, out int @params) { @params = -1; }
        public virtual void GetInteger(GetPName pname, int[] @params) { }
        public virtual void GetProgram(int program, GetProgramParameterName programParameterName, out int linkStatus)
        {
            linkStatus = -1;
        }
        public virtual string GetProgramInfoLog(int program) { return string.Empty; }
        public virtual void GetShader(int shader, ShaderParameter pname, out int @params)
        {
            @params = -1;
        }
        public virtual string GetShaderInfoLog(int shader) { return string.Empty; }
        public virtual void GetShaderInfoLog(int shader, out string info) { info = string.Empty; }
        public virtual string GetString(StringName name) { return string.Empty; }
        public virtual void GetTexLevelParameter(TextureTarget target, int level, GetTextureParameter pname, out int @params) { @params = -1; }
        public virtual void GetTexParameter(TextureTarget target, GetTextureParameter pname, out int @params) { @params = -1; }
        public virtual int GetUniformLocation(int program, string name) { return -1; }
        public virtual void LinkProgram(int program) { }

        /// <summary>
        /// Render multiple sets of primitives by specifying indices of array data elements
        /// </summary>
        /// <param name="mode">The kind od primitives to render</param>
        /// <param name="count">Points to an array of the elements counts</param>
        /// <param name="type">Specifies the type of the values in indices, must be UnsignedByte, UnsignedShort, UnsignedInt</param>
        /// <param name="indices">Specifies a pointer to the location where indices are stored</param>
        /// <param name="arraySize">Specifies the size of the count and indices arrays </param>
        public virtual void MultiDrawElements(PrimitiveType mode, int[] count, DrawElementsType type, IntPtr indices, int arraySize) { }
        public virtual void PushMatrix() { }
        public virtual void PopMatrix() { }
        public virtual void ReadPixels(int x, int y, int width, int height, PixelFormat format, PixelType type, IntPtr pixels) { }
        public virtual void ShaderSource(int shader, string @string) { }
        public virtual void TexImage2D(TextureTarget target, int level, PixelInternalFormat internalformat, int width, int height, int border, PixelFormat format, PixelType type, IntPtr pixels) { }
        public virtual void TexImage3D(TextureTarget target, int level, PixelInternalFormat internalformat, int width, int height, int depth, int border, PixelFormat format, PixelType type, IntPtr pixels) { }
        public virtual void TexParameter(TextureTarget target, TextureParameterName pname, int param) { }
        public virtual void Uniform1(int location, int v0) { }
        public virtual void Uniform1(int location, int count, ref int value) { }
        public virtual void Uniform1(int location, int count, ref float value) { }
        public virtual void Uniform2(int location, ref Vector2 vector) { }
        public virtual void Uniform3(int location, ref Vector3 vector) { }
        public virtual void Uniform4(int location, ref Vector4 vector) { }
        public virtual void UniformMatrix4(int location, bool transpose, ref Matrix4 matrix) { }
        public virtual void UseProgram(int program, bool force = false) { }
        public virtual void ValidateProgram(int program) { }
        public virtual void VertexAttribPointer(int index, int size, VertexAttribPointerType type, bool normalized, int stride, IntPtr pointer) { }
        public virtual void Viewport(int x, int y, int width, int height) { }

        //public virtual void SwapBuffers() { }
        // ///////////////////////// Ext ///////////////////////////////////////////

        public virtual void BindFramebuffer(FramebufferTarget target, int buffer) { }

        public virtual FramebufferErrorCode CheckFrameBufferStatus(FramebufferTarget target) { return FramebufferErrorCode.FramebufferUnsupported; }

        public virtual void FrameBufferTexture2D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget textarget, int texture, int level) { }

        public virtual void GenFrameBuffers(int n, out int framebuffers) { framebuffers = -1; }

        public virtual void GenRenderBuffers(int n, out int framebuffers) { framebuffers = -1; }
        // /////////////////////// NanoVG //////////////////////////////////////////
        public virtual void FrontFace(FrontFaceDirection mode) { }
        public virtual void StencilMask(int mask) { }
        public virtual void StencilMask(uint mask) { }
        public virtual void StencilOp(StencilOp fail, StencilOp zfail, StencilOp zpass) { }
        public virtual void StencilFunc(StencilFunction func, int @ref, int mask) { }
        public virtual void StencilFunc(StencilFunction func, int @ref, uint mask) { }
    }
}
