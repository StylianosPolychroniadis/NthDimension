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
    public class RadioButton : Widget
    {
        public delegate void CheckedChanged();
        private string m_text = string.Empty;
        private string m_pictureFile;
        private bool m_init = false;
        private bool m_skiprender = false;

        private bool m_superimpose;

        Picture m_userPicture;
        Label m_userLabel;
        private Picture.enuRectangle m_rectMode;

        public readonly static Size IconSize = new Size(22, 22);

        bool hover;
        int ADelta = 60;
        Color defColor;
        
        Color checkedColor;
        // bool ignoreCheckedColor = false;
        Color hoverColor;
        Color hoverCheckedColor;
        Color hoverNotCheckedColor;


        object lockChecked = new object();
        public event CheckedChanged OnCheckedChanged;
        public bool Checkable = false;
        private bool m_checked = false;
        public bool Checked
        {
            get { if (Checkable) return m_checked; return false; }
            set
            {
                if (Checkable)
                {

                    m_checked = value;
                    this.BGColor = checkedColor;

                    if(value)   // Added Jan-21-2020
                    {
                        if (null != Parent)
                            foreach (Widget w in Parent.Widgets)
                                if (w is RadioButton && w != this)
                                {
                                    if (((RadioButton)w).m_checked)
                                        ((RadioButton)w).m_checked = false;
                                }
                    }

                    bool raise = m_checked != value;
                    if (raise && null != OnCheckedChanged)
                        this.OnCheckedChanged();
                }
            }
        }
        public new Color FGColor
        {
            get { return base.FGColor; }
            set { base.FGColor = m_userLabel.FGColor = value; }
        }

        public RadioButton(string imgUrl, Picture.enuRectangle rectMode = Picture.enuRectangle.Normal)
        {

            this.defColor = this.BGColor = Color.FromArgb(0, 255, 255, 255);
            this.checkedColor = Color.FromArgb(defColor.A,
                                               defColor.R - 60,
                                               defColor.B,
                                               defColor.G - 60);
           
            this.hoverColor             = Color.FromArgb(checkedColor.A + ADelta <= 255 ? checkedColor.A + ADelta : 255, checkedColor.R, checkedColor.G, checkedColor.B);
            this.hoverCheckedColor      = Color.FromArgb(255 - (checkedColor.A + ADelta <= 255 ? checkedColor.A + ADelta : 255), checkedColor.R, checkedColor.G, checkedColor.B);
            this.hoverNotCheckedColor   = Color.FromArgb(defColor.A + ADelta <= 255 ? defColor.A + ADelta : 255, defColor.R, defColor.G, defColor.B);

            //this.UserId = userId;
            //this.m_imageUrl = imgUrl;
            this.m_rectMode = rectMode;

            this.m_pictureFile = imgUrl;


            this.MouseEnterEvent += delegate { onMouseEnter(); };
            this.MouseLeaveEvent += delegate { onMouseLeave(); };
            this.MouseDownEvent += delegate { onMouseDown(); };
            this.MouseUpEvent += delegate { onMouseUp(); };

        }
        public RadioButton(string text, string imgUrl, Picture.enuRectangle rectMode = Picture.enuRectangle.Normal)
            : this(imgUrl, rectMode)
        {
            this.m_text = text;
        }

        void onMouseEnter()
        {
            //if (Checkable && Checked) 
            //    this.BGColor = hoverCheckedColor;
            //else
            //    this.BGColor = hoverColor;
            hover = true;

        }
        void onMouseLeave()
        {
            //if (Checkable && Checked)
            //    this.BGColor = checkedColor;
            //else
            //    this.BGColor = defColor; 
            hover = false;
        }
        void onMouseDown()
        {
            bool chk = Checked;

            if (Checkable) Checked = !chk; 
            
            

            //ConsoleUtil.log(string.Format("{0} is now {1}", this.m_text, this.m_checked));
        }
        void onMouseUp()
        {
            //if (Checkable && Checked) return;
            //this.BGColor = defColor;
        }

        private void InitializeComponent()
        {
            if (!string.IsNullOrEmpty(this.m_pictureFile))
            {
                m_userPicture = new Picture(m_pictureFile, false) { RectangleMode = m_rectMode };
                m_userPicture.Location = new Point(0, 0);
                m_userPicture.Size = RadioButton.IconSize;
                m_userPicture.MouseClickEvent += delegate { onMouseDown(); };
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
                m_userLabel.MouseClickEvent += delegate { onMouseDown(); };
                this.Widgets.Add(m_userLabel);
            }

            m_init = true;
        }

        protected override void DoPaint(PaintEventArgs e)
        {
            if (!m_init)
                this.InitializeComponent();

            //if (Checkable && Checked &&
            //    this.BGColor != hoverColor && 
            //    this.BGColor != hoverCheckedColor)
            //        this.BGColor = checkedColor;
            //if (Checkable && !Checked &&
            //    this.BGColor != hoverColor &&
            //    this.BGColor != hoverNotCheckedColor)
            //        this.BGColor = defColor;



            if (m_skiprender)
                return;

            if (hover)
                BGColor = hoverColor;
            else
                BGColor = defColor;

            if (Checkable && Checked)
            {
                e.GC.DrawRectangle(new NanoPen(hoverCheckedColor, 1), this.ClientRect);


            }

            base.DoPaint(e);
        }

        public RadioButton CopyShallow()    // TODO :: Convert to DeepCopy
        {
            RadioButton ret = new RadioButton(this.m_pictureFile);
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
