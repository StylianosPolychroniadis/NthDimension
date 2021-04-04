using System.Drawing;

using NthDimension.Forms;
using NthDimension.Forms.Dialogs;
using NthStudio.Gui.Widgets;
using NthDimension.Rendering.Drawables.Framebuffers;
using NthDimension.Rendering;

namespace NthStudio.Gui
{
    public class TextureViewer : DialogBase
    {
        int m_texture;
        bool m_init = false;
        bool m_isDepth;
        Picture m_picture;
        //Texture texture;

        public override ImageList ImgList
        {
            get { return null; }
            set { }
        }

        public TextureViewer(int textureId, bool isDepth = false)
        {
            m_texture = textureId;
            Title = "Main FBO";
            Size = new System.Drawing.Size(640, 480);
            m_isDepth = isDepth;

        }
        public TextureViewer(Texture tex)
        {
            Title = "Framebuffer Depth";
            //fbo = StudioWindow.Instance.FramebufferCreator.createFrameBuffer((int)fbo.Size.X, (int)fbo.Size.Y);
            m_texture = tex.texture;

            Size = new System.Drawing.Size(640, 480);
            m_isDepth = true;
        }

        private void InitializeComponent()
        {
            this.BGColor = Color.Black;

            //m_picture = new Picture(StudioWindow.Instance.FBOSet_Main.SceneFramebuffer.ColorTexture);
            m_picture = new Picture(m_texture, true);
            m_picture.Size = new Size(this.Width, this.Height - Window.DEF_HEADER_HEIGHT);
            m_picture.Location = new Point(0, Window.DEF_HEADER_HEIGHT);
            m_picture.ShowBoundsLines = true;

            Widgets.Add(m_picture);

            m_init = true;
        }

        protected override void DoPaint(GContext parentGContext)
        {
            if (!m_init)
                this.InitializeComponent();

            base.DoPaint(parentGContext);
        }
    }
}
