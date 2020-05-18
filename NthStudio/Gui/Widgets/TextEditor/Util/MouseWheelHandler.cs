using NthDimension.Forms.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Gui.Widgets.TextEditor.Util
{
    /// <summary>
    /// Accumulates mouse wheel deltas and reports the actual number of lines to scroll.
    /// </summary>
    class MouseWheelHandler
    {
        // CODE DUPLICATION: See ICSharpCode.SharpDevelop.Widgets.MouseWheelHandler

        const int WHEEL_DELTA = 120;

        int mouseWheelDelta;
        int mouseWheelScrollLines = 1;

        public MouseWheelHandler(int mouseWheelScrollLines)
        {
            this.mouseWheelScrollLines = mouseWheelScrollLines;
        }

        public int GetScrollAmount(MouseEventArgs e, out int md)
        {
            // accumulate the delta to support high-resolution mice
            mouseWheelDelta += e.DeltaWheel;
            md = mouseWheelDelta;

            //int linesPerClick = Math.Max(SystemInformation.MouseWheelScrollLines, 1);
            int linesPerClick = Math.Max(mouseWheelScrollLines, 1);

            int scrollDistance = mouseWheelDelta * linesPerClick / WHEEL_DELTA;
            mouseWheelDelta %= Math.Max(1, WHEEL_DELTA / linesPerClick);
            return scrollDistance;
        }
    }
}
