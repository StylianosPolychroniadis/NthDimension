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

using NthDimension.Algebra;
using NthDimension.Rendering.Utilities;
using NthDimension.Rasterizer;

namespace NthDimension.Rendering.Drawables.Framebuffers
{
    public class Framebuffer : ApplicationObject
    {
        private volatile bool m_multisampling = true;

        #region Properties
        public volatile int              FboHandle;

        public volatile int              ColorTexture;           // TODO:: Convert to List<FramebufferAttachment>
        public volatile int              DepthTexture;           // TODO:: Convert to List<FramebufferAttachment>
        public volatile int              DepthTexture1;
        public volatile int              DepthTexture2;
        public volatile int              DepthTexture3;

        public Color4           ClearColor { get; set; }
        public bool             IsDefault               = false;
        public new Vector2      Size { get { return size; } set { size = value; } }
        public override string  Name
        {
            get { return name; }
            set { name = value; }
        }
        public bool             Multisampling
        {
            get { return m_multisampling; }
            set
            {
                m_multisampling = value;

                int sampling = 0;
                if (value)
                {
                    sampling = (int)TextureMinFilter.Linear;
                    //GameBase.Instance.Renderer.Enable(EnableCap.Multisample);
                    ApplicationBase.Instance.Renderer.MultisampleEnabled = true;
                }
                else
                {
                    sampling = (int)TextureMinFilter.Nearest;
                    //GameBase.Instance.Renderer.Disable(EnableCap.Multisample);
                    ApplicationBase.Instance.Renderer.MultisampleEnabled = false;
                }


                ApplicationBase.Instance.Renderer.BindTexture(TextureTarget.Texture2D, ColorTexture);
                ApplicationBase.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, sampling);
                ApplicationBase.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, sampling);
            }
        }
        #endregion

        #region Fields
        protected new Vector2   size;
        private string          name;
        #endregion

        #region Ctor
        public Framebuffer() { this.ClearColor = ApplicationBase.Instance.VAR_ScreenColor.ToColor4(); }
        public Framebuffer(int fboHandle, int colorTexture, int depthTexture, Vector2 size, FramebufferCreator parent)
        {
            this.FboHandle          = fboHandle;
            this.ColorTexture       = colorTexture;
            this.DepthTexture       = depthTexture;
            this.size               = size;

            Parent                  = parent;
        }
        #endregion

        public void Delete()
        {
            // TODO:: Verify: Deleting textures BEFORE deleting Framebuffer????

            uint colortexture =  (uint)this.ColorTexture;
            uint depthtexture =  (uint)this.DepthTexture;
            uint depthtexture1 = (uint)this.DepthTexture1;
            uint depthtexture2 = (uint)this.DepthTexture2;
            uint depthtexture3 = (uint)this.DepthTexture3;

            if (colortexture > 0)
            {
                ApplicationBase.Instance.Renderer.DeleteTextures(1, ref colortexture);
                ApplicationBase.Instance.CheckGlError(string.Format("Delete FrameBuffer {0} ColorTexture {1}", this.FboHandle, colortexture));
            }
            if (depthtexture > 0)
            {
                ApplicationBase.Instance.Renderer.DeleteTextures(1, ref depthtexture);
                ApplicationBase.Instance.CheckGlError(string.Format("Delete FrameBuffer {0} DepthTexture {1} ", this.FboHandle, depthtexture));
            }
            //Game.Instance.Renderer.DeleteTextures(1, ref depthtexture1);
            //Game.Instance.checkGlError(string.Format("Delete FrameBuffer {0} DepthTexture1 {1} ", this.FboHandle, depthtexture));
            //Game.Instance.Renderer.DeleteTextures(1, ref depthtexture2);
            //Game.Instance.checkGlError(string.Format("Delete FrameBuffer {0} DepthTexture2 {1} ", this.FboHandle, depthtexture));
            //Game.Instance.Renderer.DeleteTextures(1, ref depthtexture3);
            //Game.Instance.checkGlError(string.Format("Delete FrameBuffer {0} DepthTexture3 {1} ", this.FboHandle, depthtexture));

            ApplicationBase.Instance.TextureLoader.textures.RemoveAll(t => t.identifier == colortexture);
            ApplicationBase.Instance.TextureLoader.textures.RemoveAll(t => t.identifier == depthtexture);

            ApplicationBase.Instance.Renderer.DeleteFrameBuffer((uint)this.FboHandle);
            ApplicationBase.Instance.CheckGlError(string.Format("Delete FrameBuffer {0} ", this.FboHandle));
        }

        public virtual void enable(bool wipe)
        {
            ApplicationBase.Instance.CheckGlError(string.Format("(!) OpenGL Error: Enable {0}:{1}", this.GetType(), Name));

            ApplicationBase.Instance.Renderer.BindFramebuffer(FramebufferTarget.FramebufferExt, FboHandle);
            ApplicationBase.Instance.Renderer.FrameBufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, ColorTexture, 0);
            ApplicationBase.Instance.Renderer.FrameBufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachment,  TextureTarget.Texture2D, DepthTexture, 0);
            ApplicationBase.Instance.Renderer.Viewport(0, 0, (int)(size.X), (int)(size.Y));
            
            if (wipe)
            {
                
                ApplicationBase.Instance.Renderer.ClearColor(ClearColor);                                                  // clear the screen in red, to make it very obvious what the clear affected. only the FBO, not the real framebuffer
                ApplicationBase.Instance.Renderer.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            }

            ApplicationBase.Instance.VAR_ScreenSize_Current = size;
            ApplicationBase.Instance.CheckGlError(string.Format("(!) OpenGL Error: Enable {0}:{1}", this.GetType(), Name));
        }

        // DEBUG
        public enum enuDebugDrawTexture
        {
            ColorTexture,
            DepthTexture
        }
        public void CopyToFramebuffer(enuDebugDrawTexture texture, int fboIdx, int destX0, int destY0, int destX1, int destY1)
        {
            bool displayDebug = false;
            int hwidth = ApplicationBase.Instance.Width / 2;
            int hheight = ApplicationBase.Instance.Height / 2;

            // Bind READ
            ApplicationBase.Instance.Renderer.BindFramebuffer(FramebufferTarget.ReadFramebuffer, this.FboHandle);
            if(displayDebug) ApplicationBase.Instance.CheckGlError(string.Format("CopyToFrameBuffer Step 0/4. Debug FrameBuffer Bind READ source FBO {0}", this.Name));

            // READ
            if (texture == enuDebugDrawTexture.ColorTexture)
                ApplicationBase.Instance.Renderer.ReadBuffer(ReadBufferMode.ColorAttachment0 + this.ColorTexture);
            if (texture == enuDebugDrawTexture.DepthTexture)
                ApplicationBase.Instance.Renderer.ReadBuffer(ReadBufferMode.ColorAttachment0 + this.DepthTexture);
            if (displayDebug) ApplicationBase.Instance.CheckGlError(string.Format("CopyToFrameBuffer Step 1/4. Debug FrameBuffer READ {0}", this.Name));


            //Bind DRAW
            ApplicationBase.Instance.Renderer.BindFramebuffer(FramebufferTarget.DrawFramebuffer, fboIdx); //Game.Instance.FBOSet_Main.SceneFramebuffer.FboHandle);//->Draws on Main Framebuffer
            if (displayDebug) ApplicationBase.Instance.CheckGlError(string.Format("CopyToFrameBuffer Step 2/4. Debug FrameBuffer Bind DRAW fboIndex: {1} {0} ", this.Name, fboIdx));

            //Game.Instance.Renderer.DrawBuffer(DrawBufferMode.None);


            ApplicationBase.Instance.Renderer.BlitFramebuffer(0, 0,
                                                   (int)this.Size.X, (int)this.Size.Y,
                                                   destX0, destY0,
                                                   destX1, destY1,
                                                   texture == enuDebugDrawTexture.ColorTexture ? ClearBufferMask.ColorBufferBit //| ClearBufferMask.DepthBufferBit 
                                                                                               : ClearBufferMask.DepthBufferBit, //| ClearBufferMask.StencilBufferBit,
                                                   texture == enuDebugDrawTexture.DepthTexture ? BlitFramebufferFilter.Linear 
                                                                                               : BlitFramebufferFilter.Nearest);
            if (displayDebug) ApplicationBase.Instance.CheckGlError(string.Format("CopyToFrameBuffer Step 3/4. Blit {0}", this.Name));
            ApplicationBase.Instance.Renderer.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0);
            if (displayDebug) ApplicationBase.Instance.CheckGlError(string.Format("CopyToFrameBuffer Step 4/4. Bind 0 {0}", this.Name));
        }
        public void CopyToTexture(enuDebugDrawTexture texture, int tex, int destX0, int destY0, int destX1, int destY1)
        {
            //int fbo = -1;

            bool displayDebug = true;
            int hwidth = ApplicationBase.Instance.Width / 2;
            int hheight = ApplicationBase.Instance.Height / 2;

            // Bind READ
            ApplicationBase.Instance.Renderer.BindFramebuffer(FramebufferTarget.ReadFramebuffer, this.FboHandle);                                                                                          // Was ReadFramebuffer
                if (displayDebug) ApplicationBase.Instance.CheckGlError(string.Format("CopyToTexture Step 0/5. Bind READ srcFBO {0}", this.Name));

            if (texture == enuDebugDrawTexture.ColorTexture)
                ApplicationBase.Instance.Renderer.ReadBuffer(ReadBufferMode.ColorAttachment0); // +this.ColorTexture);
            if (texture == enuDebugDrawTexture.DepthTexture)
                ApplicationBase.Instance.Renderer.ReadBuffer(ReadBufferMode.ColorAttachment0 + this.DepthTexture);
            if (displayDebug) ApplicationBase.Instance.CheckGlError(string.Format("CopyToTexture Step 1/5. READ {0}", this.Name));

            ApplicationBase.Instance.Renderer.BlitFramebuffer(0, 0,
                (int)this.Size.X, (int)this.Size.Y,
                destX0, destY0,
                destX1, destY1,
                texture == enuDebugDrawTexture.ColorTexture
                        ? ClearBufferMask.ColorBufferBit //| ClearBufferMask.DepthBufferBit 
                        : ClearBufferMask.DepthBufferBit, //| ClearBufferMask.StencilBufferBit,
                texture == enuDebugDrawTexture.DepthTexture
                        ? BlitFramebufferFilter.Linear
                        : BlitFramebufferFilter.Nearest);
            if (displayDebug) ApplicationBase.Instance.CheckGlError(string.Format("CopyToTexture Step 2/5. Blit {0}", this.Name));

            ApplicationBase.Instance.Renderer.FramebufferTexture2D(FramebufferTarget.ReadFramebuffer,
                texture == enuDebugDrawTexture.ColorTexture
                    ? FramebufferAttachment.ColorAttachment0
                    : FramebufferAttachment.DepthAttachment,
                TextureTarget.Texture2D,
                texture == enuDebugDrawTexture.ColorTexture
                    ? this.ColorTexture
                    : this.DepthTexture,
                0);
                if (displayDebug) ApplicationBase.Instance.CheckGlError(string.Format("CopyToTexture Step 3/5. READ For Texture2D 0 {0}", this.Name));

            

            ApplicationBase.Instance.Renderer.FramebufferTexture2D(FramebufferTarget.DrawFramebuffer,
                FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D, tex,
                0);
            if (displayDebug) ApplicationBase.Instance.CheckGlError(string.Format("CopyToTexture Step 4/5. DRAW For Texture2D: {1} {0}", this.Name, tex));
            ApplicationBase.Instance.Renderer.DrawBuffer(DrawBufferMode.ColorAttachment0);
                if (displayDebug) ApplicationBase.Instance.CheckGlError(string.Format("CopyToTexture Step 5/5. DRAW For ColorAttachment1: {1} {0}", this.Name, tex));

        }
        //public void CopyToTexture(int texId, int x, int y, int width, int height)
        //{
        //    ApplicationBase.Instance.Renderer.BindTexture(TextureTarget.ProxyTexture2D, texId);
        //    //BindToRead(ReadBufferMode.ColorAttachment0 + DepthTexture, width, height);
        //    enable(false);

        //    ApplicationBase.Instance.Renderer.CopyTexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb8, 0, 0, width, height, 0);
        //}
        public System.Drawing.Bitmap CopyToBitmap(int AttachmentIndex = 0)
        {
            //if (AttachmentIndex > fbo_colorBuffer.Count - 1)
            //    throw new SystemIndexOutOfRangeException();

            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap((int)this.size.X, (int)this.size.Y);
            System.Drawing.Imaging.BitmapData bits = bitmap.LockBits(new System.Drawing.Rectangle(0, 
                                                                                                  0,
                                                                    (int)this.size.X, (int)this.size.Y), 
                                                                    System.Drawing.Imaging.ImageLockMode.WriteOnly, 
                                                                    System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            BindToRead(ReadBufferMode.ColorAttachment0 + AttachmentIndex, (int)this.size.X, (int)this.size.Y);




            ApplicationBase.Instance.Renderer.ReadPixels(0, 0, (int)this.size.X, (int)this.size.Y, PixelFormat.Bgr, PixelType.UnsignedByte, bits.Scan0);
            bitmap.UnlockBits(bits);

            return bitmap;
        }
        public void BindToRead(ReadBufferMode Mode, int width, int height)
        {
            ApplicationBase.Instance.Renderer.BindFramebuffer(FramebufferTarget.ReadFramebuffer, this.FboHandle);
            ApplicationBase.Instance.Renderer.ReadBuffer(Mode);
        }

        //public void BindForWriting()
        //{
        //    Game.Instance.Renderer.BindFramebuffer(FramebufferTarget.DrawFramebuffer, this.FboHandle);
        //}

        //public void BindForReading()
        //{
        //    Game.Instance.Renderer.BindFramebuffer(FramebufferTarget.ReadFramebuffer, this.FboHandle);
        //}

        //public void SetReadBuffer(int textureType = 0)
        //{
        //    Game.Instance.Renderer.ReadBuffer(ReadBufferMode.ColorAttachment0 + textureType);
        //}
    }
}
