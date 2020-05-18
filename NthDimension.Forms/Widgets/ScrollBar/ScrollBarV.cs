using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms.Widgets
{
    public class ScrollBarV : ScrollBarBase
    {
        #region Properties

        protected Size DefaultSize
        {
            get
            {
                return new Size(16, 200);
            }
        }

        public override EScrollOrientation Orientation
        {
            get { return EScrollOrientation.Vertical; }
        }

        public override Size MinimumSize
        {
            get
            {
                return new Size(12, 0);
            }
            set
            {
                base.MinimumSize = value;
            }
        }
        #endregion Properties

        protected override void OnSizeChanged()
        {
            base.OnSizeChanged();

            if (Size.Width < MinimumSize.Width)
                Size = new Size(MinimumSize.Width, Size.Height);
        }

    }
}
