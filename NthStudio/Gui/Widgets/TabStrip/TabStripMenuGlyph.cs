
using System.Drawing;
using System.Linq;

using NthDimension.Forms;


namespace NthStudio.Gui.Widgets.TabStrip
{
    internal class TabStripMenuGlyph
    {
        #region Fields

        private Rectangle glyphRect = Rectangle.Empty;
        private bool isMouseOver = false;
        //private ToolStripProfessionalRenderer renderer;

        #endregion

        #region Props

        public bool IsMouseOver
        {
            get { return isMouseOver; }
            set { isMouseOver = value; }
        }

        public Rectangle Bounds
        {
            get { return glyphRect; }
            set { glyphRect = value; }
        }

        #endregion

        #region Ctor

        internal TabStripMenuGlyph()
        {
            //this.renderer = renderer;
        }

        #endregion

        #region Methods

        public void DrawGlyph(GContext gc, TabStrip pTabStrip)
        {
            if (isMouseOver)
            {
                Color fill = Color.FromArgb(35, SystemColors.Highlight); //renderer.ColorTable.ButtonSelectedHighlight; //TextColor.FromArgb(35, SystemColors.Highlight);
                gc.FillRectangle(new SolidBrush(fill), glyphRect);
                Rectangle borderRect = glyphRect;

                borderRect.Width--;
                borderRect.Height--;

                gc.DrawRectangle(new NanoPen(SystemColors.Highlight), borderRect);
            }

            //SmoothingMode bak = gc.SmoothingMode;

            //gc.SmoothingMode = SmoothingMode.Default;

            var pen = new NanoPen(Color.White);
            {
                pen.Width = 2;

                gc.DrawLine(pen, new Point(glyphRect.Left + (glyphRect.Width / 3) - 2, glyphRect.Height / 2 - 1),
                            new Point(glyphRect.Right - (glyphRect.Width / 3), glyphRect.Height / 2 - 1));
            }

            if (pTabStrip.Widgets.Count > 0)
            {
                gc.FillPolygon(new NanoSolidBrush(Color.White), new Point[]{
                                   new Point(glyphRect.Left + (glyphRect.Width / 3)-2, glyphRect.Height / 2+2),
                                   new Point(glyphRect.Right - (glyphRect.Width / 3), glyphRect.Height / 2+2),
                                   new Point(glyphRect.Left + glyphRect.Width / 2-1,glyphRect.Bottom-4)});
            }
            //gc.SmoothingMode = bak;
        }

        #endregion
    }
}
