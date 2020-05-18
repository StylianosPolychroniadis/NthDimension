using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms
{
    public class NanoPen
    {
        public NanoPen(Color c, int width = 1)
        {
            Width = width;
            Color = c;
        }
        public NanoPen(System.Drawing.Pen pen, int width = 1)
            :this(pen.Color, width)
        {

        }

        public Color Color
        {
            get;
            set;
        }

        public float Width
        {
            get;
            set;
        }
    }
}
