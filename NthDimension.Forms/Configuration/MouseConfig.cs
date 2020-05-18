using System.Drawing;

namespace NthDimension.Forms.Configuration
{
    public static class MouseConfig
    {
        static MouseConfig()
        {
            DragSize = new Size(4, 4);
            DoubleClickSize = new Size(4, 4);
        }

        public static Size DoubleClickSize
        {
            get;
            private set;
        }

        public static Size DragSize
        {
            get;
            private set;
        }

        public static int VerticalScrollBarWidth
        {
            get
            {
                return 17;
            }
        }

        public static int HorizontalScrollBarHeight
        {
            get
            {
                return 17;
            }
        }

        public static int MouseWheelScrollLines
        {
            get { return 3; }
        }
    }
}
