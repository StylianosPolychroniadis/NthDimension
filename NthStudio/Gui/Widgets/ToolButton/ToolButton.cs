using NthDimension.Forms;
using NthDimension.Forms.Events;
using NthDimension.Rendering.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Gui.Widgets
{
    public class ToolButton : Widget
    {
        
        private string  m_text          = string.Empty;
        private string  m_pictureFile;
        private bool    m_init          = false;
        private bool    m_skiprender    = false;

        private bool                    m_superimpose;

        Picture m_userPicture;
        Label m_userLabel;
        private Picture.enuRectangle    m_rectMode;

        public readonly static Size IconSize = new Size(22, 22);

        int ADelta = 60;
        Color defColor;
       

        public ToolButton(string imgUrl, Picture.enuRectangle rectMode = Picture.enuRectangle.Normal)
        {
            
            this.defColor = this.BGColor = Color.FromArgb(0, 255, 255, 255);
         
            //this.UserId = userId;
            //this.m_imageUrl = imgUrl;
            this.m_rectMode = rectMode;

            this.m_pictureFile = imgUrl;


            this.MouseEnterEvent += delegate { onMouseEnter(); };
            this.MouseLeaveEvent += delegate { onMouseLeave(); };
        }
        public ToolButton(string text, string imgUrl, Picture.enuRectangle rectMode = Picture.enuRectangle.Normal) 
            : this(imgUrl, rectMode)
        {
            this.m_text = text;
        }

        void onMouseEnter()
        {
           
            this.BGColor = Color.FromArgb(defColor.A + ADelta <= 255 ? defColor.A + ADelta : 255, defColor.R, defColor.G, defColor.B);

        }
        void onMouseLeave()
        {
            this.BGColor = defColor; 
        }
     

        private void InitializeComponent()
        {
            if (!string.IsNullOrEmpty(this.m_pictureFile))
            {
                m_userPicture = new Picture(m_pictureFile, false) { RectangleMode = m_rectMode };
                m_userPicture.Location = new Point(0, 0);
                m_userPicture.Size = ToolButton.IconSize;
                //m_userPicture.MouseClickEvent += delegate {  };
                this.Widgets.Add(m_userPicture);
            }

            if (!string.IsNullOrEmpty(this.m_text))
            {
                m_userLabel = new Label(this.m_text);

                bool icon = m_userPicture != null;

                m_userLabel.Location = new Point(icon ? m_userPicture.Width + 5 : 5, 0);
                m_userLabel.Size = new Size(this.Width - (icon ? m_userPicture.Width + 5 : 5), this.Height);
                m_userLabel.Font = new NanoFont(NanoFont.DefaultRegular, 10f);
                m_userLabel.FGColor = Color.WhiteSmoke;
                //m_userLabel.MouseClickEvent += delegate { onMouseDown(); };
                this.Widgets.Add(m_userLabel);
            }

            m_init = true;
        }

        protected override void DoPaint(PaintEventArgs e)
        {
            if (!m_init)
                this.InitializeComponent();

            if (m_skiprender)
                return;          

            base.DoPaint(e);
        }

        public ToolButton CopyShallow()    // TODO :: Convert to DeepCopy
        {
            ToolButton ret = new ToolButton(this.m_pictureFile);
            ret.m_pictureFile = this.m_pictureFile;
            ret.Location = this.Location;
            ret.Size = this.Size;
            return ret;
        }

        public void UpdateImage(string imgUrl)
        {
            this.m_init = false;
            this.m_pictureFile = imgUrl;
            this.Widgets.Clear();
            this.InitializeComponent();
        }
    }
}
