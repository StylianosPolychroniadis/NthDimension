using System;
using System.Drawing;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GLTerrain {
    public class RenderTarget : IDisposable {
        public Texture Target { get; private set; }
        public PixelInternalFormat Format { get; private set; }
        public int ID { get; private set; }
        public int DepthID { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public RenderTarget(Texture tex) {
            Format = PixelInternalFormat.Rgba;

            int id = -1;
            GL.GenFramebuffers(1, out id);
            ID = id;

            ChangeTarget(tex);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, ID);

            int depthbufferID = -1;
            GL.GenRenderbuffers(1, out depthbufferID);
            DepthID = depthbufferID;

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthbufferID);
            GL.RenderbufferStorage(
                RenderbufferTarget.Renderbuffer,
                RenderbufferStorage.DepthComponent32f,
                Width,
                Height);

            GL.FramebufferRenderbuffer(
                FramebufferTarget.Framebuffer,
                FramebufferAttachment.DepthAttachment,
                RenderbufferTarget.Renderbuffer,
                depthbufferID);

            FramebufferErrorCode code = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (code != FramebufferErrorCode.FramebufferComplete) {
                throw new Exception("Frame buffer failed to create! " + code.ToString());
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public RenderTarget(int width, int height, PixelInternalFormat format, TextureUnit unit)
            : this(new Texture(new Bitmap(width, height), true, true))
        {
            Format = format;
        }

        public void ChangeTarget(Texture newTarget) {
            Target = newTarget;

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, ID);
            GL.FramebufferTexture2D(
                    FramebufferTarget.Framebuffer,
                    FramebufferAttachment.ColorAttachment0,
                    TextureTarget.Texture2D,
                    Target.ID,
                    0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            Width = Target.Width;
            Height = Target.Height;
        }

        public void Bind() {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, ID);
            GL.PushAttrib(AttribMask.ViewportBit);
            GL.Viewport(0, 0, Width, Height);
        }

        public void Unbind(bool buildMipmap) {
            GL.PopAttrib();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            if (buildMipmap) {
                Target.Bind();
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                Target.Unbind();
            }
        }

        public void Dispose() {
            int id = ID;
            GL.DeleteFramebuffers(1, ref id);

            id = DepthID;
            GL.DeleteRenderbuffers(1, ref id);

            Target.Dispose();
        }
    }
}
