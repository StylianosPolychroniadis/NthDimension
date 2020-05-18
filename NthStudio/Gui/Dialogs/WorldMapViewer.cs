using System.Drawing;
using System.Threading;

using NthDimension.Forms;
using NthDimension.Forms.Dialogs;
using NthDimension.Forms.Widgets;
using NthDimension.Rendering.Geometry;
using NthDimension.Rendering;
using NthDimension.Rendering.Shaders;
using NthDimension.Rendering.Drawables.Framebuffers;
using NthDimension.Rendering.Utilities;

using NthDimension.Forms.Events;
using NthDimension.Rasterizer;
using System.Drawing.Imaging;
using NthStudio.Gui.Widgets;

namespace NthStudio.Gui
{
    public class WorldMapViewer : DialogBase
    {
        private bool m_init;
        private Picture         m_textureControl;
        private Texture         m_texture;

        Thread T_WorldMap;
       

        public override ImageList ImgList
        {
            get { return null; }
            set { }
        }


        public WorldMapViewer()
        {
            Title = "World";
            Size = new Size(900, 600);
            //this.InitializeComponent();
            
        }

        private void InitializeComponent()
        {
            if (m_init) return;

            //Size mapSize = new Size(Properties.Resources.EquirMap_2048.Width,
            //                        Properties.Resources.EquirMap_2048.Height);

            Bitmap map = Properties.Resources.EquirMap_2048;

            m_texture = new Texture()
            {
                name    = "worldmap",
                texture = NthStudio.StudioWindow.Instance.Renderer.GenTexture(),
                type    = Texture.Type.fromFramebuffer,
                bitmap  = map
            };

            this.updateTexture(map);

            m_texture.loaded            = true;

            m_textureControl            = new Picture(m_texture.texture, true, NthDimension.Rasterizer.NanoVG.NVGimageFlags.NVG_IMAGE_NEAREST);
            m_textureControl.Dock       = EDocking.Fill;

            Widgets.Add(m_textureControl);
            m_init = true;
        }

        private void updateTexture(Bitmap map)
        {
            BitmapData bmp_data = /*m_texture.bitmap*/map.LockBits(
                            new Rectangle(0, 0, m_texture.bitmap.Width, m_texture.bitmap.Height),
                            System.Drawing.Imaging.ImageLockMode.ReadOnly,
                            System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            ApplicationBase.Instance.Renderer.BindTexture(TextureTarget.Texture2D, m_texture.texture);
            ApplicationBase.Instance.Renderer.TexImage2D(TextureTarget.Texture2D,
                                                         0,
                                                         PixelInternalFormat.Rgba,
                                                         bmp_data.Width,
                                                         bmp_data.Height,
                                                         0,
                                                         NthDimension.Rasterizer.PixelFormat.Bgra,
                                                         PixelType.UnsignedByte,
                                                         bmp_data.Scan0);
            ApplicationBase.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            ApplicationBase.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            ApplicationBase.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            ApplicationBase.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);

            /*m_texture.bitmap*/
            map.UnlockBits(bmp_data);
        }

        protected override void DoPaint(GContext parentGContext)
        {
            if (!m_init)
                this.InitializeComponent();

            base.DoPaint(parentGContext);

        }
    }
}
