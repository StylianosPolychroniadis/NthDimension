using System;
using System.Drawing;

namespace NthDimension.Forms.Events
{
    public class PaintEventArgs : EventArgs
    {
        //
        // Properties
        //
        public Rectangle ClipRect
        {
            get;
            private set;
        }

        public GContext GC
        {
            get;
            private set;
        }

        //
        // Constructors
        //

        public PaintEventArgs(GContext gc)
            : this(gc, Rectangle.Empty)
        {
        }

        public PaintEventArgs(GContext gc, Rectangle rect)
        {
            this.GC = gc;
            ClipRect = rect;
        }
    }
}
