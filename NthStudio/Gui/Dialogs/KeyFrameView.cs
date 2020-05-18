using NthDimension.Forms;
using NthDimension.Forms.Dialogs;
using NthDimension.Forms.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Gui.Dialogs
{
    public class KeyFrameView : DialogBase
    {
        public override ImageList ImgList { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private TimeLine    m_timeline;
        private SplitterBox m_spliter;

        public KeyFrameView()
        {
            this.Size = new System.Drawing.Size(450, 100);
            this.BorderStyle = enuBorderStyle.Sizable;
            this.IsDialog = false;
            //this.BGColor = System.Drawing.Color.Gray;
            this.PaintBackGround = true;
            this.ShowBoundsLines = true;

            m_spliter = new SplitterBox(ESplitterType.HorizontalScroll);
            m_spliter.Size = this.Size;
            m_spliter.Dock = EDocking.Fill;
            m_spliter.SplitterBarLocation = 0.75f;

            m_timeline = new TimeLine();
            m_timeline.Dock = EDocking.Fill;
            m_spliter.Panel0.Widgets.Add(m_timeline);

            SuspendLayout();
            this.Widgets.Add(m_spliter);
            ResumeLayout();
        }


    }
}
