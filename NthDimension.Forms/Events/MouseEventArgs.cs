using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms.Events
{
    public class MouseEventArgs : BaseEventArgs
    {
        public MouseButton Button
        {
            get;
            private set;
        }

        public int Clicks
        {
            get;
            private set;
        }

        public int DeltaWheel
        {
            get;
            private set;
        }

        public int DeltaX
        {
            get;
            private set;
        }

        public int DeltaY
        {
            get;
            private set;
        }

        public Point Location
        {
            get
            {
                return new Point(X, Y);
            }
        }

        public int X
        {
            get;
            private set;
        }

        public int Y
        {
            get;
            private set;
        }

        //
        // Constructors
        /// <summary>
        /// X y Y, posición del puntero del ratón en coordenadas del Widget
        /// </summary>
        /// <param name="button">Button.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="deltaX">Delta x.</param>
        /// <param name="deltaY">Delta y.</param>
        /// <param name = "deltaWheel"></param>
        /// <param name = "clicks"></param>
        public MouseEventArgs(MouseButton button, int x, int y, int deltaX,
                              int deltaY, int deltaWheel = 0, int clicks = 1)
        {
            this.Clicks = clicks;
            this.Button = button;
            this.DeltaX = deltaX;
            this.DeltaY = deltaY;
            this.X = x;
            this.Y = y;
            this.DeltaWheel = deltaWheel;
        }

        public MouseEventArgs(MouseDownEventArgs mea)
                    : this(mea.Button, mea.X, mea.Y,
                   mea.DeltaX, mea.DeltaY, mea.DeltaWheel, mea.Clicks)

        {
        }

        public MouseEventArgs(MouseEventArgs mea)
                    : this(mea.Button, mea.X, mea.Y,
                   mea.DeltaX, mea.DeltaY, mea.DeltaWheel, mea.Clicks)

        {
        }
    }
}
