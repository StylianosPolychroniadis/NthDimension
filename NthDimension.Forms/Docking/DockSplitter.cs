using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms.Docking
{
    public class DockSplitter
    {
        private Widget                  _parentControl;
        private Widget                  _control;

        private EDockSplitterType       _splitterType;

        private int                     _minimum;
        private int                     _maximum;
        //private DarkTranslucentForm _overlayForm;
        private Overlay                 _overlayForm;

        #region Properties
        public System.Drawing.Rectangle Bounds { get; set; }

        //public Cursor ResizeCursor { get; private set; }
        #endregion Properties

        public DockSplitter(Widget parentControl, Widget control, EDockSplitterType splitterType)
        {
            _parentControl = parentControl;
            _control = control;
            _splitterType = splitterType;

            switch (_splitterType)
            {
                case EDockSplitterType.Left:
                case EDockSplitterType.Right:
                    throw new NotImplementedException("ResizeCursor");
                    //ResizeCursor = Cursors.SizeWE;
                    break;
                case EDockSplitterType.Top:
                case EDockSplitterType.Bottom:
                    throw new NotImplementedException("ResizeCursor");
                    //ResizeCursor = Cursors.SizeNS;
                    break;
            }
        }

        public void ShowOverlay()
        {
            //_overlayForm = new DarkTranslucentForm(Color.Black);
            _overlayForm = new Overlay();
            _overlayForm.BGColor = System.Drawing.Color.Black;
            //_overlayForm.Visible = true;
            _overlayForm.Show();

            UpdateOverlay(new Point(0, 0));
        }

        public void HideOverlay()
        {
            //_overlayForm.Visible = false;
            _overlayForm.Hide();
        }

        public void UpdateOverlay(Point difference)
        {
            var bounds = new Rectangle(Bounds.Location, Bounds.Size);

            switch (_splitterType)
            {
                case EDockSplitterType.Left:
                    var leftX = System.Math.Max(bounds.Location.X - difference.X, _minimum);

                    if (_maximum != 0 && leftX > _maximum)
                        leftX = _maximum;

                    bounds.Location = new Point(leftX, bounds.Location.Y);
                    break;
                case EDockSplitterType.Right:
                    var rightX = System.Math.Max(bounds.Location.X - difference.X, _minimum);

                    if (_maximum != 0 && rightX > _maximum)
                        rightX = _maximum;

                    bounds.Location = new Point(rightX, bounds.Location.Y);
                    break;
                case EDockSplitterType.Top:
                    var topY = System.Math.Max(bounds.Location.Y - difference.Y, _minimum);

                    if (_maximum != 0 && topY > _maximum)
                        topY = _maximum;

                    bounds.Location = new Point(bounds.Location.X, topY);
                    break;
                case EDockSplitterType.Bottom:
                    var bottomY = System.Math.Max(bounds.Location.Y - difference.Y, _minimum);

                    if (_maximum != 0 && bottomY > _maximum)
                        topY = _maximum;

                    bounds.Location = new Point(bounds.Location.X, bottomY);
                    break;
            }

            _overlayForm.Bounds = bounds;
        }

        public void Move(Point difference)
        {
            switch (_splitterType)
            {
                case EDockSplitterType.Left:
                    //_control.Width += difference.X;
                    _control.Size = new Size(_control.Width + difference.X, _control.Height);
                    break;
                case EDockSplitterType.Right:
                    //_control.Width -= difference.X;
                    _control.Size = new Size(_control.Width - difference.X, _control.Height);
                    break;
                case EDockSplitterType.Top:
                    //_control.Height += difference.Y;
                    _control.Size = new Size(_control.Width, _control.Height + difference.X);
                    break;
                case EDockSplitterType.Bottom:
                    //_control.Height -= difference.Y;
                    _control.Size = new Size(_control.Width, _control.Height - difference.X);
                    break;
            }

            UpdateBounds();
        }
        public void UpdateBounds()
        {
            throw new NotImplementedException("RectangleToScreen: Cast each via PointToScreen");
            //var bounds = _parentControl.RectangleToScreen(_control.Bounds);

            //switch (_splitterType)
            //{
            //    case EDockSplitterType.Left:
            //        Bounds = new Rectangle(bounds.Left - 2, bounds.Top, 5, bounds.Height);
            //        _maximum = bounds.Right - 2 - _control.MinimumSize.Width;
            //        break;
            //    case EDockSplitterType.Right:
            //        Bounds = new Rectangle(bounds.Right - 2, bounds.Top, 5, bounds.Height);
            //        _minimum = bounds.Left - 2 + _control.MinimumSize.Width;
            //        break;
            //    case EDockSplitterType.Top:
            //        Bounds = new Rectangle(bounds.Left, bounds.Top - 2, bounds.Width, 5);
            //        _maximum = bounds.Bottom - 2 - _control.MinimumSize.Height;
            //        break;
            //    case EDockSplitterType.Bottom:
            //        Bounds = new Rectangle(bounds.Left, bounds.Bottom - 2, bounds.Width, 5);
            //        _minimum = bounds.Top - 2 + _control.MinimumSize.Height;
            //        break;
            //}
        }
    }
}
