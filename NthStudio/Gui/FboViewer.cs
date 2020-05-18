using System.Drawing;

using NthDimension.Forms;
using NthDimension.Forms.Dialogs;
using NthStudio.Gui.Widgets;
using NthDimension.Rendering.Drawables.Framebuffers;
using NthDimension.Rendering;

namespace NthStudio.Gui
{
    public class FboViewer : DialogBase
    {
        int m_texture;
        bool m_init = false;
        bool m_isDepth;
        Picture m_picture;
        Button m_buttonSwitch;

        Framebuffer fboDepth;
        Framebuffer fbo;

        public override ImageList ImgList
        {
            get { return null; }
            set { }
        }

        public FboViewer(string title, int fboTextureId, bool isDepth = false)
        {
            m_texture = fboTextureId;
            Title = title;
            Size = new System.Drawing.Size(640, 510);
            m_isDepth = isDepth;

        }
        public FboViewer(string title, Framebuffer depth)
        {
            Title = "Framebuffer Depth";
            fboDepth = depth;
            fbo             = StudioWindow.Instance.FramebufferCreator.createFrameBuffer((int)fbo.Size.X, (int)fbo.Size.Y);
            fbo.ClearColor  = NthDimension.Algebra.Color4.White;
            m_texture       = fbo.ColorTexture;
            ////fboCopyDepth = StudioWindow.Instance.FramebufferCreator.createFrameBuffer((int)depth.Size.X, (int)depth.Size.Y, NthDimension.Rasterizer.PixelInternalFormat.R32f, false);
            //////StudioWindow.Instance.Renderer.BindFramebuffer( NthDimension.Rasterizer.FramebufferTarget.Framebuffer, fboCopyDepth.FboHandle);
            ////fboDepth.CopyToFramebuffer(Framebuffer.enuDebugDrawTexture.DepthTexture, fboCopyDepth.FboHandle, 0, 0, (int)fboDepth.Size.X, (int)fboDepth.Size.Y);
            //////StudioWindow.Instance.Renderer.BindFramebuffer(0, fboCopyDepth.FboHandle);

            ////m_texture = fboCopyDepth.ColorTexture;



            //Bitmap bmp = fboDepth.CopyToBitmap(depth.DepthTexture);
            //bmp.Save(System.IO.Path.Combine(NthDimension.Utilities.DirectoryUtil.Documents_Temporary, string.Format("depth{0}.png", fboDepth.DepthTexture)), System.Drawing.Imaging.ImageFormat.Png);

            //// m_texture = StudioWindow.Instance.Renderer.GenTexture();
            ////// StudioWindow.Instance.Renderer.BindTexture(NthDimension.Rasterizer.TextureTarget.Texture2D, m_texture);
            //// fboDepth.CopyToTexture(Framebuffer.enuDebugDrawTexture.DepthTexture, m_texture, 0, 0, (int)fboDepth.Size.X, (int)fboDepth.Size.Y);
            ////// StudioWindow.Instance.Renderer.BindTexture(NthDimension.Rasterizer.TextureTarget.Texture2D, 0);



            Size = new System.Drawing.Size(640, 480);
            m_isDepth = true;
        }
        
        private void InitializeComponent()
        {
            this.BGColor = Color.Black;

            m_buttonSwitch = new Button(m_isDepth ? "View color" : "View depth");
            m_buttonSwitch.Size = new Size(this.Width, 30);
            m_buttonSwitch.Location = new Point(0, Window.DEF_HEADER_HEIGHT);
            m_buttonSwitch.MouseClickEvent += delegate
            {
                if(m_isDepth && null != fbo)
                {
                    if (m_texture == fbo.DepthTexture)
                    { m_texture = fbo.ColorTexture; m_buttonSwitch.Text = "View depth"; }
                    if (m_texture == fbo.ColorTexture)
                    { m_texture = fbo.DepthTexture; m_buttonSwitch.Text = "View Color"; }
                }
            };
            //m_buttonSwitch.Dock = EDocking.Top;

            //m_picture = new Picture(StudioWindow.Instance.FBOSet_Main.SceneFramebuffer.ColorTexture);
            m_picture = new Picture(m_texture, true);
            m_picture.Size = new Size(this.Width, this.Height - Window.DEF_HEADER_HEIGHT  - m_buttonSwitch.Height);
            m_picture.Location = new Point(0, Window.DEF_HEADER_HEIGHT + m_buttonSwitch.Height);
            m_picture.ShowBoundsLines = true;
            //m_picture.Dock = EDocking.Fill;

            Widgets.Add(m_buttonSwitch);
            Widgets.Add(m_picture);

            m_init = true;
        }

        protected override void DoPaint(GContext parentGContext)
        {
            if (!m_init)
                this.InitializeComponent();

            base.DoPaint(parentGContext);

            
            if (m_isDepth && fbo != null && fboDepth != null)
            {
                //fbo.enable(true);
                //StudioWindow.Instance.Scene.RenderSurface.draw(enuShaderTypes.copycatShader,
                //                new int[]
                //                {
                //                    fbo.ColorTexture
                //                });

            // TODO:: Disable fbo -OR- Do not return to rendering
            }

        }

    }
}
