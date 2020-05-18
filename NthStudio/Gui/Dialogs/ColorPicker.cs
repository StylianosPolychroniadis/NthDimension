using NthDimension.Forms;
using NthDimension.Forms.Dialogs;
using NthStudio.Gui.Widgets;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Gui.Dialogs
{
    public class EnvironmentSettings : DialogBase
    {
        private ColorWheel m_color;
        private Slider m_slider;

        public override ImageList ImgList
        {
            get { return null; }
            set { }
        }


        public EnvironmentSettings()
        {
            this.Size = new System.Drawing.Size(640, 480);
            this.BGColor = Color.DarkGray;
            this.PaintBackGround = true;

            //Panel p = new Panel();
            //p.Dock = EDocking.Fill;
            //p.BGColor = Color.DarkGray;
            //p.PaintBackGround = true;
            //this.Widgets.Add(p);

            m_color = new ColorWheel();
            m_color.Size = new System.Drawing.Size(150, 150);
            m_color.Location = new System.Drawing.Point(0, DialogBase.DEF_HEADER_HEIGHT);
            m_color.ShowBoundsLines = true;

            m_slider = new Slider();
            m_slider.Size = new System.Drawing.Size(150, 50);
            m_slider.Location = new System.Drawing.Point(0, DialogBase.DEF_HEADER_HEIGHT + m_color.Height);
            m_slider.ShowBoundsLines = true;

            //p.Widgets.Add(m_color);
            //p.Widgets.Add(m_slider);

            this.Widgets.Add(m_color);
            this.Widgets.Add(m_slider);
        }
    }
}
