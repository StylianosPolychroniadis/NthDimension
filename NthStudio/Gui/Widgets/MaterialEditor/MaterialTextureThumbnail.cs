using System;

using NthDimension.Forms;
using NthDimension.Rendering;

namespace NthStudio.Gui.Widgets
{
    public class MaterialTextureThumbnail : Panel
    {
        private Label lbTextureType;
        private Picture picTexture;
        private Texture m_texture;
        private string m_description = string.Empty;
        public string Description
        {
            get { return m_description; }
            set { m_description = value;
                  lbTextureType.Text = value;
            }
        }

        public MaterialTextureThumbnail(Texture texture, string description)
        {
            this.m_texture = texture;
            this.m_description = description;

            this.Size = new System.Drawing.Size(80, 64 + 20);
            this.BGColor = System.Drawing.Color.FromArgb(16, 16, 16);

            this.lbTextureType = new Label()
            {
                Size = new System.Drawing.Size(this.Width, 18),
                Dock = EDocking.Top,
                TextAlign = ETextAlignment.Center,
                BGColor = System.Drawing.Color.Transparent,
                FGColor = System.Drawing.Color.White,
                Text = description
            };
            this.Widgets.Add(lbTextureType);

            this.picTexture = new Picture(texture.texture, false, NthDimension.Rasterizer.NanoVG.NVGimageFlags.NVG_IMAGE_FLIPY)
            {
                Size = new System.Drawing.Size(this.Width, 64),
                Dock = EDocking.Fill
            };
            this.Widgets.Add(picTexture);
        }
     
    }
}
