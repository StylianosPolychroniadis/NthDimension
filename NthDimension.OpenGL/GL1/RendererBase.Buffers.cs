namespace NthDimension.Rasterizer.GL1
{
    using Geometry;
    using Buffers;
    using System;
    public abstract partial class RendererBase
    {
		public virtual void DeleteBuffer(int buffers)
		{
		    
		}
        public virtual void DeleteBuffers(int n, ref int buffers)
        {
            buffers = -1;
        }
		public virtual void DrawArrays(enuPolygonMode mode, int first, int count){}
		public virtual void DrawArraysInstanced(enuPolygonMode mode, int first, int count, int instancecount){}
        public virtual void BindBuffer(BufferTarget target, int buffer)
        {
        }
        //public virtual int  GenBuffer()
        //{
        //    return -1;
        //}
        public virtual void GenBuffers(int n, out int buffers) { buffers = -1; }
        public virtual int GenVertexArray()
        {
            return -1;
        }
        public virtual void BindVertexArray(int vao)
        {

        }
        public virtual void BufferData<T2>(BufferTarget target, IntPtr size, T2[] data, BufferUsageHint usage) where T2 : struct
        {
        }
        public virtual void GetBufferParameter(BufferTarget target, BufferParameterName name, out int result) { result = -1; }
        public virtual void PushClientAttrib(ClientAttribMask mask)
        {
        }
		public virtual void PopClientAttrib()
        {
        }

        ////////////////////////////////////////
        public abstract void drawVertexBuffer(SYSCON.Graphics.Modelling.Model model);
        public abstract void drawVertexBuffer(SYSCON.Graphics.Geometry.enuPolygonMode primitive, VboBuffer vbo, Mesh mesh);
        public abstract void drawVertexArray(SYSCON.Graphics.Modelling.Model model);
        public abstract void drawVertexArray(SYSCON.Graphics.Geometry.enuPolygonMode mode, VboBuffer vbo);

        /////////////////////////////////////// FrameBuffer /////////////////////////////////////////////////

        public abstract void GenFramebuffers(int n, out int framebuffers);
        public abstract void GenRenderbuffers(int n, out int renderbuffers);
        public abstract void BindFramebuffer(FramebufferTarget target, int framebuffer);
        public abstract void BindRenderbuffer(RenderbufferTarget target, int renderbuffer);
        public abstract void DeleteFramebuffers(int n, ref int framebuffers);
        public abstract void DeleteRenderbuffers(int n, ref int renderbuffers);
        public abstract void FramebufferTexture2D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget textarget, int texture, int level);
        public abstract void FramebufferRenderbuffer(FramebufferTarget target, FramebufferAttachment attachment, RenderbufferTarget renderbuffertarget, int renderbuffer);
        public abstract void RenderbufferStorage(RenderbufferTarget target, RenderbufferStorage internalformat, int width, int heigth);
        public abstract FramebufferErrorCode CheckFramebufferStatus(FramebufferTarget target);
    }
}
