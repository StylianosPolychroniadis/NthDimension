using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms.Events
{
    public class MouseDownEventArgs : MouseEventArgs
    {
        /// <summary>
        /// El Widget que tiene el foco lo pierde si se pulsa en otro Widget.
        /// Por defecto es 'true'.
        /// </summary>
        public bool FocusedLostFocusOnMouseDown
        {
            get;
            set;
        }

        public MouseDownEventArgs(MouseButton button, int x, int y, int deltaX, int deltaY,
                                  int deltaWheel = 0, int clicks = 1)
            : base(button, x, y, deltaX, deltaY, deltaWheel, clicks)
        {
            FocusedLostFocusOnMouseDown = true;
        }
    }
}
