namespace NthDimension.Rasterizer.GL1
{
    using System;

    public class RenderTarget : IDisposable
    {
        protected RendererBase          _renderer;
        public Texture                  Target { get; private set; }
        public PixelInternalFormat      Format { get; private set; }
        public int                      ID { get; private set; }
        public int                      DepthID { get; private set; }
        public int                      Width { get; private set; }
        public int                      Height { get; private set; }

        public RenderTarget(RendererBase renderer, Texture tex)
        {
            _renderer = renderer;
            Format = PixelInternalFormat.Rgba;

            int id = -1;
            _renderer.GenFramebuffers(1, out id);
            ID = id;

            ChangeTarget(tex);
            _renderer.BindFramebuffer(FramebufferTarget.Framebuffer, ID);

            int depthbufferID = -1;
            _renderer.GenRenderbuffers(1, out depthbufferID);
            DepthID = depthbufferID;

            _renderer.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthbufferID);
            _renderer.RenderbufferStorage(
                RenderbufferTarget.Renderbuffer,
                RenderbufferStorage.DepthComponent32f,
                Width,
                Height);

            _renderer.FramebufferRenderbuffer(
                FramebufferTarget.Framebuffer,
                FramebufferAttachment.DepthAttachment,
                RenderbufferTarget.Renderbuffer,
                depthbufferID);

            FramebufferErrorCode code = _renderer.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (code != FramebufferErrorCode.FramebufferComplete)
            {
                throw new Exception("Frame buffer failed to create! " + code.ToString());
            }

            _renderer.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public RenderTarget(RendererBase renderer, int width, int height, PixelInternalFormat format, TextureUnit unit)
            : this(renderer, new Texture(renderer, new System.Drawing.Bitmap(width, height), true, true))
        {
            Format = format;
        }

        public void ChangeTarget(Texture newTarget)
        {
            Target = newTarget;

            _renderer.BindFramebuffer(FramebufferTarget.Framebuffer, ID);
            _renderer.FramebufferTexture2D(
                    FramebufferTarget.Framebuffer,
                    FramebufferAttachment.ColorAttachment0,
                    TextureTarget.Texture2D,
                    Target.TextureID,
                    0);
            _renderer.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            Width = Target.TextureWidth;
            Height = Target.TextureHeight;
        }

        public void Bind()
        {
            _renderer.BindFramebuffer(FramebufferTarget.Framebuffer, ID);
            _renderer.PushAttrib(AttribMask.ViewportBit);
            _renderer.Viewport(0, 0, Width, Height);
        }

        public void Unbind(bool buildMipmap)
        {
            _renderer.PopAttrib();
            _renderer.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            if (buildMipmap)
            {
                Target.Bind();
                _renderer.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                Target.UnBind();
            }
        }

        public void Dispose()
        {
            int id = ID;
            _renderer.DeleteFramebuffers(1, ref id);

            id = DepthID;
            _renderer.DeleteRenderbuffers(1, ref id);

            Target.Dispose();
        }
    }
}
