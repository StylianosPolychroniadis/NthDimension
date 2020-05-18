using System.Drawing;

namespace NthDimension.Forms.Widgets
{
    public partial class ScrollBarH : ScrollBarBase
    {
        public ScrollBarH()
        {
        }

        #region Properties

        protected Size DefaultSize
        {
            get
            {
                return new Size(200, 16);
            }
        }

        public override EScrollOrientation Orientation
        {
            get { return EScrollOrientation.Horizontal; }
        }

        public override Size MinimumSize
        {
            get
            {
                return new Size(0, 12);
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

            if (Size.Height < MinimumSize.Height)
                Size = new Size(Size.Width, MinimumSize.Height);
        }
    }
}
