using NthDimension.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Gui.Widgets.TabStrip
{
    internal class TabStripCloseButton
    {
        #region Fields

        private Rectangle crossRect = Rectangle.Empty;
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
            get { return crossRect; }
            set { crossRect = value; }
        }

        #endregion

        #region Ctor

        internal TabStripCloseButton()
        {
            //this.renderer = renderer;
        }

        #endregion

        #region Methods

        public void DrawCross(GContext gc)
        {
            if (isMouseOver)
            {
                Color fill = Color.FromArgb(35, SystemColors.Highlight); //renderer.ColorTable.ButtonSelectedHighlight;

                gc.FillRectangle(new SolidBrush(fill), crossRect);

                Rectangle borderRect = crossRect;

                borderRect.Width--;
                borderRect.Height--;

                gc.DrawRectangle(new NanoPen(SystemColors.Highlight), borderRect);
            }

            NanoPen pen = new NanoPen(Color.White); //, 1.6f))
            {
                pen.Width = 1.6f;
                gc.DrawLine(pen, crossRect.Left + 3, crossRect.Top + 3,
                    crossRect.Right - 5, crossRect.Bottom - 4);

                gc.DrawLine(pen, crossRect.Right - 5, crossRect.Top + 3,
                    crossRect.Left + 3, crossRect.Bottom - 4);
            }
        }

        #endregion
    }
}
