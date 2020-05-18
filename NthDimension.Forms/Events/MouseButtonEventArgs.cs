using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms.Events
{
    public class MouseButtonEventArgs : MouseEventArgs
    {
        public bool IsPressed
        {
            get;
            internal set;
        }

        //
        // Constructors
        //
        public MouseButtonEventArgs()
            : base(MouseButton.None, 0, 0, 0, 0)
        {
            IsPressed = false;
        }

        public MouseButtonEventArgs(int x, int y, MouseButton button, bool pressed)
            : base(button, x, y, 0, 0)
        {
            IsPressed = pressed;
        }

        public MouseButtonEventArgs(MouseButtonEventArgs args)
            : this(args.X, args.Y, args.Button, args.IsPressed)
        {
        }
    }
}
