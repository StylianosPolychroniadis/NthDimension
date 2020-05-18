using NthDimension.Rasterizer.NanoVG;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms.Docking
{
    public class DockTab
    {
        #region Properties
        public DockContent DockContent { get; set; }

        public Rectangle ClientRectangle { get; set; }

        public Rectangle CloseButtonRectangle { get; set; }

        public bool Hot { get; set; }

        public bool CloseButtonHot { get; set; }

        public bool ShowSeparator { get; set; }
        #endregion Properties

        public DockTab(DockContent content)
        {
            DockContent = content;
        }

        public int CalculateWidth(NVGcontext g, NanoFont font)
        {
            
            var width = (int)NanoFont.MeasureText(g, DockContent.DockText, font).Width;
            width += 10;

            return width;
        }
    }
}
