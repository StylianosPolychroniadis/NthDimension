using NthDimension.Forms.Delegates;
using NthDimension.Forms.Events;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace NthDimension.Forms.Widgets
{
    /// <summary>
	/// Description of NeoScrollBar.
	/// </summary>
	public abstract partial class ScrollBarBase : Widget
    {
        public event ScrollEventHandler Scroll = null;

        public event EventHandler ValueChanged = null;

        #region Fields

        protected int clickPoint;

        protected int arrowLength;

        protected Rectangle borderRect;
        protected Rectangle LUArrowRect;
        protected Rectangle RDArrowRect;
        protected Rectangle channelRect;
        protected Rectangle thumbRect;
        protected Rectangle trackRect;

        /// <summary>
        /// Left-Up Arrow Button State
        /// </summary>
        protected EScrollButtonState LUArrowState = EScrollButtonState.NotFocused;
        /// <summary>
        /// Right-Down Arrow Button State
        /// </summary>
        protected EScrollButtonState RDArrowState = EScrollButtonState.NotFocused;

        protected int LargeChange_ = 10;
        protected int SmallChange_ = 1;
        protected int Minimum_ = 0;
        protected int Maximum_ = 100;
        protected int Value_ = 0;

        private System.Timers.Timer timer = new System.Timers.Timer();
        bool decDown;
        private const int repeatDelay = 50;
        private const int startDelay = 300;
        bool incDown;

        #endregion Fields

        static ScrollBarBase()
        {

        }

        protected ScrollBarBase()
        {
            Renderer = new ScrollRenderer();
            //MinimumSize_ = Size = new Size(0, 12);
            ThumbPos = 0;
            ThumbLength = 18;

            this.Name = "NeoScrollBar";

            //DoubleBuffered = true;
            ThumbColor = Color.Red;
            InactiveArrowColor = Color.Black;
            ActiveArrowColor = Color.Red; //TextColor.FromArgb(135, 12, 62);
            setBGColorFromParent = true;
        }

        #region Properties

        public bool ThumbReverse = false;

        protected override void OnSizeChanged()
        {
            forceUpdateThumbPos = true;
        }

        public virtual bool ShowScrollDebugLines
        {
            get;
            set;
        }

        public override Widget Parent
        {
            get
            {
                return base.Parent;
            }
            protected set
            {
                base.Parent = value;
                if (Parent != null && setBGColorFromParent)
                    BGColor = Parent.BGColor;
            }
        }

        bool setBGColorFromParent;

        Size MinimumSize_;
        public virtual Size MinimumSize
        {
            get { return MinimumSize_; }
            set
            {
                if (value == MinimumSize_)
                    return;
                MinimumSize_ = value;
            }
        }


        protected int ThumbPos
        {
            get;
            private set;
        }

        protected int ThumbLength
        {
            get;
            private set;
        }

        ScrollRenderer Renderer
        {
            get;
            set;
        }

        public abstract EScrollOrientation Orientation
        {
            get;
        }

        public Color ThumbColor
        {
            get { return Renderer.ThumbColor; }
            set
            {
                if (value == Renderer.ThumbColor)
                    return;
                Renderer.ThumbColor = value;
            }
        }

        public Color InactiveArrowColor
        {
            get { return Renderer.InactiveArrowColor; }
            set
            {
                if (value == Renderer.InactiveArrowColor)
                    return;
                Renderer.InactiveArrowColor = value;
            }
        }

        public Color ActiveArrowColor
        {
            get { return Renderer.ActiveArrowColor; }
            set
            {
                if (value == Renderer.ActiveArrowColor)
                    return;
                Renderer.ActiveArrowColor = value;
            }
        }

        public int LargeChange
        {
            get { return System.Math.Min(LargeChange_, Maximum_ - Minimum_ + 1); }
            set
            {
                if (LargeChange_ != value)
                {
                    if (value < 0)
                    {
                        throw new ArgumentOutOfRangeException(/* TODO */);
                    }
                    LargeChange_ = value;
                }
            }
        }

        public int SmallChange
        {
            get { return System.Math.Min(SmallChange_, LargeChange_); }
            set
            {
                if (SmallChange_ != value)
                {
                    if (value < 0)
                    {
                        throw new ArgumentOutOfRangeException(/* TODO */);
                    }
                    SmallChange_ = value;
                }
            }
        }

        public int Minimum
        {
            get { return Minimum_; }
            set
            {
                if (Minimum_ != value)
                {
                    if (Maximum_ < value)
                    {
                        Maximum_ = value;
                    }
                    if (value > Value_)
                    {
                        Value_ = value;
                    }
                    Minimum_ = value;
                    ThumbPos = CalcThumbPos(Value);
                    Repaint();
                }
            }
        }

        public int Maximum
        {
            get { return Maximum_; }
            set
            {
                if (Maximum_ != value)
                {
                    if (Minimum_ > value)
                    {
                        Minimum_ = value;
                    }
                    if (value < Value_)
                    {
                        Value = value;
                    }
                    Maximum_ = value;
                    ThumbPos = CalcThumbPos(Value);
                    Repaint();
                }
            }
        }

        private int OldValue
        {
            get;
            set;
        }

        public int Value
        {
            get { return Value_; }
            set
            {
                if (Value_ == value)
                    return;

                if (value < Minimum || value > Maximum)
                {
                    //throw new ArgumentOutOfRangeException("value", "Invalid value in ScrollBar");
                    Rendering.Utilities.ConsoleUtil.errorlog("value", "Invalid value in ScrollBar");
                }
                OldValue = Value_;
                Value_ = value;

                if (ValueChanged != null)
                    ValueChanged(this, new EventArgs());

                forceUpdateThumbPos = true;
                Repaint();
            }
        }

        bool forceUpdateThumbPos = false;

        // Get or set whether the Decrement button is down, start the timer
        private bool DecrementDown
        {
            get { return decDown; }
            set
            {
                if (value == decDown)
                {
                    return;
                }
                decDown = value;
                if (value)
                {
                    timer.Elapsed += Decrement;
                    timer.Interval = startDelay;
                    timer.Start();
                }
                else
                {
                    timer.Stop();
                    timer.Elapsed -= Decrement;
                }
            }
        }

        // Get or set whether the Increment button is down, start the timer
        private bool IncrementDown
        {
            get { return incDown; }
            set
            {
                if (value == incDown)
                {
                    return;
                }
                incDown = value;
                Repaint();
                if (value)
                {
                    timer.Elapsed += Increment;
                    timer.Interval = startDelay;
                    timer.Start();
                }
                else
                {
                    timer.Stop();
                    timer.Elapsed -= Increment;
                }
            }
        }

        #endregion Properties

        #region Methods

        private int CalcThumbLength()
        {
            int result, channelLength;

            if (Orientation == EScrollOrientation.Vertical)
                channelLength = channelRect.Height;
            else
                channelLength = channelRect.Width;

            result = (int)System.Math.Round((double)(channelLength * LargeChange) / (Maximum - Minimum));

            if (result > channelLength)
                result = channelLength;

            return result;
        }

        public void IncrementManual(int inc = 1)
        {
            int v = Value + (SmallChange * inc);

            int v1 = Maximum - LargeChange + 1;
            if (v1 < Minimum)
                v1 = Minimum;
            if (v > v1)
                v = v1;

            Value = v;

            ThumbPos = CalcThumbPos(Value);

            if (Scroll != null)
                Scroll(this, new ScrollEventArgs(ScrollEventType.SmallIncrement,
                                                    OldValue, Value,
                                                    Orientation == EScrollOrientation.Vertical ?
                                                 EScrollOrientation.Vertical :
                                                 EScrollOrientation.Horizontal));

            Repaint();
        }

        public void DecrementManual(int dec = 1)
        {
            int newValue = Value - (SmallChange * dec);
            if (newValue < Minimum)
                newValue = Minimum;

            Value = newValue;

            ThumbPos = CalcThumbPos(Value);

            if (Scroll != null)
                Scroll(this, new ScrollEventArgs(ScrollEventType.SmallDecrement,
                                                    OldValue, Value,
                                                    Orientation == EScrollOrientation.Vertical ?
                                                 EScrollOrientation.Vertical :
                                                 EScrollOrientation.Horizontal));

            Repaint();
        }

        // Called when the increment button is pressed and called from the timer
        private void Increment(Object sender, ElapsedEventArgs e)
        {
            timer.Interval = repeatDelay;
            IncrementManual();
        }

        // Called when decrement button is pushed and called from timer
        private void Decrement(Object sender, ElapsedEventArgs e)
        {
            timer.Interval = repeatDelay;
            DecrementManual();
        }

        private int CalcThumbPos(int value)
        {
            int result = 0, trackLength = 0, auxValue = value;

            if (auxValue > Maximum - LargeChange + 1)
                auxValue = Maximum - LargeChange;
            if (auxValue < Minimum)
                auxValue = Minimum;

            if (Orientation == EScrollOrientation.Vertical)
                trackLength = channelRect.Height - ThumbLength;
            else
                trackLength = channelRect.Width - ThumbLength;

            double percent = (double)trackLength / (Maximum - Minimum - LargeChange + 1);
            result = (int)System.Math.Round(percent * (auxValue - Minimum));

            return result;
        }

        private int CalcValue(int thumbPos)
        {
            int trackLength;
            float result;

            if (Orientation == EScrollOrientation.Vertical)
                trackLength = channelRect.Height - ThumbLength;
            else
                trackLength = channelRect.Width - ThumbLength;

            double percent = (double)trackLength / (Maximum - Minimum - LargeChange + 1);
            result = (float)(thumbPos / percent) + Minimum;

            return (int)System.Math.Round(result);
        }

        #endregion Methdos

        #region OnXXX()

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);

            if (Renderer != null)
            {
                if (BGColor == Renderer.ScrollBarColor)
                    return;

                Renderer.ScrollBarColor = BGColor;
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            if (!IsFocused)
                Focus();

            LUArrowState = EScrollButtonState.Focused;
            RDArrowState = EScrollButtonState.Focused;

            Cursors.Cursor = Cursors.Hand;

            Repaint();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            LUArrowState = EScrollButtonState.Focused;
            RDArrowState = EScrollButtonState.Focused;

            Repaint();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            LUArrowState = EScrollButtonState.NotFocused;
            RDArrowState = EScrollButtonState.NotFocused;

            Repaint();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (IsFocused)
                Parent.Focus();

            //LUArrowState = EButtonState.NotFocused;
            //RDArrowState = EButtonState.NotFocused;

            Cursors.Cursor = Cursors.Default;

            Repaint();
        }

        bool LUArrowMouseHover = false;
        bool RDArrowMouseHover = false;
        bool thumbArrowMouseHover = false;

        void ManualValueUpdate(int value)
        {
            if (Value_ == value)
                return;

            if (value < Minimum || value > Maximum)
            {
                return;
                //throw new ArgumentOutOfRangeException("value", "Invalid value in ScrollBar");
            }
            OldValue = Value_;
            Value_ = value;

            if (ValueChanged != null)
                ValueChanged(this, new EventArgs());
        }

        protected int barDown = 0;
        protected int minBarDown = 0;
        protected int maxBarDown = 0;

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            int newThumbPos = ThumbPos;
            int trackLength, newPos;

            if (MouseFocusCaptured && barDown != -1)
            {
                if (Orientation == EScrollOrientation.Vertical)
                {
                    trackLength = channelRect.Height - ThumbLength;
                    newPos = newThumbPos + e.Y - barDown;
                    barDown = e.Y;
                }
                else
                {
                    trackLength = channelRect.Width - ThumbLength;
                    newPos = newThumbPos + e.X - barDown;
                    barDown = e.X;
                }

                if (newPos < 0)
                {
                    newPos = 0;
                    barDown = minBarDown;
                }
                else if (newPos > trackLength)
                {
                    newPos = trackLength;
                    barDown = maxBarDown;
                }
                if (newThumbPos == newPos)
                    return;
                newThumbPos = newPos;

                int newValue = CalcValue(newThumbPos);
                ThumbPos = newThumbPos;

                if (newValue != Value)
                {
                    ManualValueUpdate(newValue);
                    if (Scroll != null)
                        Scroll(this, new ScrollEventArgs(ScrollEventType.ThumbTrack,
                                                            OldValue, Value,
                                                            Orientation == EScrollOrientation.Vertical ?
                                                         EScrollOrientation.Vertical :
                                                         EScrollOrientation.Horizontal));
                }
                Repaint();
            }
        }

        private int prevDelta = 0;
        public override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            //if (Focus())
            {
                if (e.DeltaWheel <= /*prevDelta*/ 0)
                {                   
                    //RDArrowState = EButtonState.Pressed;
                    IncrementManual(3);
                }
                else
                {                    
                    //LUArrowState = EButtonState.Pressed;
                    DecrementManual(3);
                }
                prevDelta = e.DeltaWheel;

            }
        }

        protected override void OnMouseDown(MouseDownEventArgs e)
        {
            //CaptureFocus(this, false);

            if (e.Button == MouseButton.Left)
            {
                thumbArrowMouseHover = thumbRect.Contains(e.X, e.Y);

                if (thumbArrowMouseHover)
                {
                    CaptureMouseFocus();

                    if (Orientation == EScrollOrientation.Vertical)
                    {
                        barDown = e.Y;
                        minBarDown = e.Y - thumbRect.Y + channelRect.Y;
                        maxBarDown = channelRect.Bottom - (thumbRect.Bottom - e.Y);
                    }
                    else
                    {
                        barDown = e.X;
                        minBarDown = e.X - thumbRect.X + channelRect.X;
                        maxBarDown = channelRect.Right - (thumbRect.Right - e.X);
                    }
                    if (Scroll != null)
                        Scroll(this, new ScrollEventArgs(ScrollEventType.ThumbTrack,
                                                            OldValue, Value,
                                                            Orientation == EScrollOrientation.Vertical ?
                                                         EScrollOrientation.Vertical :
                                                         EScrollOrientation.Horizontal));
                }
                else
                {
                    RDArrowMouseHover = false;
                    LUArrowMouseHover = LUArrowRect.Contains(e.X, e.Y);

                    if (LUArrowMouseHover)
                        LUArrowState = EScrollButtonState.MouseHover;
                    else
                    {
                        RDArrowMouseHover = RDArrowRect.Contains(e.X, e.Y);

                        if (RDArrowMouseHover)
                            RDArrowState = EScrollButtonState.MouseHover;
                    }

                    if (LUArrowMouseHover)
                    {
                        LUArrowState = EScrollButtonState.Pressed;

                        Decrement(null, null);
                        DecrementDown = true;
                    }
                    else if (RDArrowMouseHover)
                    {
                        RDArrowState = EScrollButtonState.Pressed;

                        Increment(null, null);
                        IncrementDown = true;
                    }
                }


                if (LUArrowMouseHover || RDArrowMouseHover)
                    Repaint();
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (incDown)
            {
                IncrementDown = false;
            }
            else if (decDown)
            {
                DecrementDown = false;
            }

            if (LUArrowState == EScrollButtonState.Pressed)
            {
                LUArrowState = EScrollButtonState.MouseHover;
            }
            else if (RDArrowState == EScrollButtonState.Pressed)
            {
                RDArrowState = EScrollButtonState.MouseHover;
            }
            else if (MouseFocusCaptured)
            {
                if (Scroll != null)
                    Scroll(this, new ScrollEventArgs(ScrollEventType.ThumbPosition,
                                                        OldValue, Value,
                                                        Orientation == EScrollOrientation.Vertical ?
                                                     EScrollOrientation.Vertical :
                                                     EScrollOrientation.Horizontal));

                // ajusta la posición a una posición exacta
                ThumbPos = CalcThumbPos(Value);
            }

            if (Scroll != null)
                Scroll(this, new ScrollEventArgs(ScrollEventType.EndScroll,
                                                    Value, Value,
                                                    Orientation == EScrollOrientation.Vertical ?
                                                 EScrollOrientation.Vertical :
                                                 EScrollOrientation.Horizontal));
            //CaptureFocus(this, false);

            Repaint();
        }

#if TEST
		protected Rectangle borderDrawRect;
#endif

        protected override void DoPaint(PaintEventArgs e)
        {
            base.DoPaint(e);

            UpdateScrollBar();

            //SmoothingMode oldSmothing = e.Graphics.SmoothingMode;

            //e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            DrawArrowButton(e.GC, LUArrowRect, LUArrowState);

            if (Orientation == EScrollOrientation.Vertical)
                DrawUpDownArrow(e.GC, LUArrowRect, true);
            else
                DrawLeftRightArrow(e.GC, LUArrowRect, true);

            DrawArrowButton(e.GC, RDArrowRect, RDArrowState);

            if (Orientation == EScrollOrientation.Vertical)
                DrawUpDownArrow(e.GC, RDArrowRect, false);
            else
                DrawLeftRightArrow(e.GC, RDArrowRect, false);



            Renderer.DrawChannel2(e.GC, channelRect, Orientation);

            UpdateThumb();

            Renderer.DrawPressed2(e.GC, thumbRect, Orientation);

            //e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            if (ShowScrollDebugLines)
            {
                e.GC.DrawRectangle(new NanoPen(Color.Red), borderRect);
                e.GC.DrawRectangle(new NanoPen(Color.Blue), LUArrowRect);
                e.GC.DrawRectangle(new NanoPen(Color.Blue), RDArrowRect);
                e.GC.DrawRectangle(new NanoPen(Color.MediumVioletRed), channelRect);
                e.GC.DrawRectangle(new NanoPen(Color.DeepPink), trackRect);
                e.GC.DrawRectangle(new NanoPen(Color.Yellow), thumbRect);
                int realThumbPos = ThumbPos + arrowLength;

                if (Orientation == EScrollOrientation.Vertical)
                    e.GC.DrawLine(new NanoPen(Color.Black), 6, realThumbPos, Width - 1, realThumbPos);
                else
                    e.GC.DrawLine(new NanoPen(Color.Black), realThumbPos, 6, realThumbPos, Height - 1);
            }

            //e.Graphics.SmoothingMode = oldSmothing;
        }

        #endregion OnXXX()

        #region Abstract-Virtual Methods

        protected virtual void UpdateScrollBar()
        {
            if (Orientation == EScrollOrientation.Vertical)
            {
                arrowLength = Width;

                borderRect = new Rectangle(0, 0, Width, Height);
                LUArrowRect = new Rectangle(0, 0, arrowLength, arrowLength);
                RDArrowRect = new Rectangle(0, Height - arrowLength, arrowLength, arrowLength);
                channelRect = new Rectangle(0, arrowLength, Width, Height - (arrowLength * 2));
            }
            else
            {
                arrowLength = Height;

                borderRect = new Rectangle(0, 0, Width, Height);
                LUArrowRect = new Rectangle(0, 0, arrowLength, arrowLength);
                RDArrowRect = new Rectangle(Width - arrowLength, 0, arrowLength, arrowLength);
                channelRect = new Rectangle(arrowLength, 0, Width - (arrowLength * 2), Height);
            }
        }

        protected virtual void UpdateThumb()
        {
            ThumbLength = CalcThumbLength();

            if (forceUpdateThumbPos)
            {
                ThumbPos = CalcThumbPos(Value);
                forceUpdateThumbPos = false;
            }

            if (Orientation == EScrollOrientation.Vertical)
            {
                trackRect = new Rectangle(0, arrowLength, Width, channelRect.Height - ThumbLength);
                thumbRect = new Rectangle(1, arrowLength + ThumbPos, Width - 3, ThumbLength);
            }
            else
            {
                trackRect = new Rectangle(arrowLength, 0, channelRect.Width - ThumbLength, Height);
                thumbRect = new Rectangle(arrowLength + ThumbPos, 1, ThumbLength, Height - 3);
            }
        }

        protected virtual void DrawArrowButton(
            GContext gc,
            Rectangle rect,
            EScrollButtonState state)
        {
            Renderer.DrawCircularButton(gc, rect, EScrollButtonStyle.Circular, state);
        }

        protected virtual void DrawUpDownArrow(
            GContext g,
            Rectangle rect,
            bool isUp)
        {
            Renderer.DrawUpDownAngledArrow(g, rect, isUp,
                                           LUArrowState != EScrollButtonState.NotFocused || RDArrowState != EScrollButtonState.NotFocused);
        }

        protected virtual void DrawLeftRightArrow(
            GContext g,
            Rectangle rect,
            bool isLeft)
        {
            Renderer.DrawLeftRightAngledArrow(g, rect, isLeft,
                                              LUArrowState != EScrollButtonState.NotFocused || RDArrowState != EScrollButtonState.NotFocused);
        }

        #endregion
    }

    /// <summary>
    /// Description of Renderer.
    /// </summary>
    public class ScrollRenderer
    {
        Color cLight;
        Color cDark;
        Color cDarkDark;
        Color bodyColor;

        Color BackgroundBottomColor = Color.FromArgb(163, 137, 186);
        // -20
        Color BackgroundTopColor = Color.FromArgb(231, 222, 239);
        Color BorderBottomColor = Color.FromArgb(234, 225, 243);
        // +10
        Color BorderTopColor = Color.FromArgb(193, 157, 206);

        Color ScrollBarColor_;

        public Color ScrollBarColor
        {
            get { return ScrollBarColor_; }
            set
            {
                if (value == ScrollBarColor_)
                    return;
                ScrollBarColor_ = value;

                bodyColor = StepColor(value, 80);
                cLight = StepColor(bodyColor, 160); // = ButtonColor;
                cDark = StepColor(value, 40);
                cDarkDark = StepColor(value, 5);
            }
        }

        Color ThumbColor_;

        public Color ThumbColor
        {
            get { return ThumbColor_; }
            set
            {
                if (value == ThumbColor_)
                    return;
                ThumbColor_ = value;

                BackgroundTopColor = StepColor(value, 200);
                BackgroundBottomColor = StepColor(value, 20);
                ;
                BorderBottomColor = StepColor(value, 220);
                BorderTopColor = StepColor(value, 20);
                proposedActiveArrowColor = BorderBottomColor;
            }
        }

        Color InactiveArrowColor_;

        public Color InactiveArrowColor
        {
            get { return InactiveArrowColor_; }
            set
            {
                if (value == InactiveArrowColor_)
                    return;
                InactiveArrowColor_ = value;
            }
        }

        Color proposedActiveArrowColor;

        Color ActiveArrowColor_;

        public Color ActiveArrowColor
        {
            get
            {
                if (ActiveArrowColor_.IsEmpty)
                    return proposedActiveArrowColor;
                return ActiveArrowColor_;
            }
            set
            {
                if (value == ActiveArrowColor_)
                    return;
                ActiveArrowColor_ = value;
            }
        }

        #region Circular Button

        public bool DrawCircularButton(
            GContext Gr,
            Rectangle rc,
            EScrollButtonStyle style,
            EScrollButtonState state)
        {
            rc.Width -= 1;
            rc.Height -= 1;

            if (rc.Width <= 0 || rc.Height <= 0)
                return false;

            // Calculate ratio
            float drawRatio = (System.Math.Min(rc.Width, rc.Height)) / 200f;
            if (System.Math.Abs(drawRatio) < float.Epsilon) // if(drawRatio == 0)
                drawRatio = 1f;

            if (state == EScrollButtonState.NotFocused)
            {
                var sb = new NanoSolidBrush(bodyColor);
                Gr.FillEllipse(sb, rc);
            }
            else
            {
                /*LinearGradientBrush br1 = new LinearGradientBrush(rc,
				                                                  cLight,
				                                                  cDark,
				                                                  45);
				if ((style == EButtonStyle.Circular) ||
				    (style == EButtonStyle.Elliptical))
				{
					Gr.FillEllipse(br1, rc);
				}
				else
				{
					GraphicsPath path = RoundedRect(rc, 15F, drawRatio);
					Gr.FillPath(br1, path);
					path.Dispose();
				}

				if (state == EButtonState.Pressed)
				{
					RectangleF _rc = rc;
					_rc.Inflate(-15F * drawRatio, -15F * drawRatio);
					LinearGradientBrush br2 = new LinearGradientBrush(_rc,
					                                                  cDark,
					                                                  cLight,
					                                                  45);
					if ((style == EButtonStyle.Circular) ||
					    (style == EButtonStyle.Elliptical))
					{
						Gr.FillEllipse(br2, _rc);
					}
					else
					{
						GraphicsPath path = RoundedRect(_rc, 10F, drawRatio);
						Gr.FillPath(br2, path);
						path.Dispose();
					}

					br2.Dispose();
				}
				br1.Dispose();*/
            }

            return true;
        }
        /*
		public GraphicsPath RoundedRect(RectangleF rect, float radius, float	drawRatio)
		{
			RectangleF baseRect = rect;
			float diameter = (radius * drawRatio) * 2.0f;
			SizeF sizeF = new SizeF(diameter, diameter);
			RectangleF arc = new RectangleF(baseRect.Location, sizeF);
			GraphicsPath path = new GraphicsPath();

			// top left arc
			path.AddArc(arc, 180, 90);
			// top right arc
			arc.X = baseRect.Right - diameter;
			path.AddArc(arc, 270, 90);
			// bottom right  arc
			arc.Y = baseRect.Bottom - diameter;
			path.AddArc(arc, 0, 90);
			// bottom left arc
			arc.X = baseRect.Left;
			path.AddArc(arc, 90, 90);

			path.CloseFigure();
			return path;
		}
*/
        public double BlendColour(double fg, double bg, double alpha)
        {
            double result = bg + (alpha * (fg - bg));
            if (result < 0.0)
                result = 0.0;
            if (result > 255)
                result = 255;
            return result;
        }

        public Color StepColor(Color clr, int alpha)
        {
            if (alpha == 100)
                return clr;

            byte a = clr.A;
            byte r = clr.R;
            byte g = clr.G;
            byte b = clr.B;
            float bg = 0;

            int _alpha = System.Math.Min(alpha, 200);
            _alpha = System.Math.Max(alpha, 0);
            double ialpha = ((double)(_alpha - 100.0)) / 100.0;

            if (ialpha > 100)
            {
                // blend with white
                bg = 255.0F;
                ialpha = 1.0F - ialpha;  // 0 = transparent fg; 1 = opaque fg
            }
            else
            {
                // blend with black
                bg = 0.0F;
                ialpha = 1.0F + ialpha;  // 0 = transparent fg; 1 = opaque fg
            }

            r = (byte)(BlendColour(r, bg, ialpha));
            g = (byte)(BlendColour(g, bg, ialpha));
            b = (byte)(BlendColour(b, bg, ialpha));

            return Color.FromArgb(a, r, g, b);
        }

        #endregion Circular Button

        #region Draw Arrows

        public void DrawUpDownAngledArrow(GContext g, Rectangle rect, bool isUp, bool isActive)
        {
            // se supone que 'rect.Width == rect.Height', cuadrado
            //SmoothingMode oldSmothing = g.SmoothingMode;
            PointF p1, p2, p3;

            //rect.Width -= 1;
            //rect.Height -= 1;
            float div8 = rect.Width / 8f;

            var penArrow = new NanoPen(isActive ? ActiveArrowColor : InactiveArrowColor);
            {
                penArrow.Width = 2;
                //penArrow.EndCap = LineCap.Square;

                //g.SmoothingMode = SmoothingMode.AntiAlias;

                if (isUp)
                {
                    p1 = new PointF(rect.X + (div8 * 2), rect.Y + (div8 * 5));
                    p2 = new PointF(rect.X + (div8 * 4), rect.Y + (div8 * 3));
                    p3 = new PointF(rect.X + (div8 * 6), rect.Y + (div8 * 5));
                }
                else
                {
                    p1 = new PointF(rect.X + (div8 * 2), rect.Y + (div8 * 3));
                    p2 = new PointF(rect.X + (div8 * 4), rect.Y + (div8 * 5));
                    p3 = new PointF(rect.X + (div8 * 6), rect.Y + (div8 * 3));

                }
                g.DrawLine(penArrow, p1, p2);
                g.DrawLine(penArrow, p3, p2);
            }
            //g.SmoothingMode = oldSmothing;
        }

        public void DrawLeftRightAngledArrow(GContext g, Rectangle rect, bool isLeft, bool isActive)
        {
            // se supone que 'rect.Width == rect.Height', cuadrado
            //SmoothingMode oldSmothing = g.SmoothingMode;
            PointF p1, p2, p3;

            //rect.Width -= 1;
            //rect.Height -= 1;
            float div8 = rect.Width / 8f;

            var penArrow = new NanoPen(isActive ? ActiveArrowColor : InactiveArrowColor);
            {
                penArrow.Width = 2;
                //penArrow.EndCap = LineCap.Square;

                //g.SmoothingMode = SmoothingMode.AntiAlias;

                if (isLeft)
                {
                    p1 = new PointF(rect.X + (div8 * 5), rect.Y + (div8 * 2));
                    p2 = new PointF(rect.X + (div8 * 3), rect.Y + (div8 * 4));
                    p3 = new PointF(rect.X + (div8 * 5), rect.Y + (div8 * 6));
                }
                else
                {
                    p1 = new PointF(rect.X + (div8 * 3), rect.Y + (div8 * 2));
                    p2 = new PointF(rect.X + (div8 * 5), rect.Y + (div8 * 4));
                    p3 = new PointF(rect.X + (div8 * 3), rect.Y + (div8 * 6));

                }
                g.DrawLine(penArrow, p1, p2);
                g.DrawLine(penArrow, p3, p2);
            }
            //g.SmoothingMode = oldSmothing;
        }

        #endregion Draw Arrows

        #region elongated button

        public void DrawPressed2(GContext g, Rectangle rect, EScrollOrientation orientation)
        {
            float r;

            rect.Width -= 1;
            rect.Height -= 1;

            if (rect.Width <= 0 || rect.Height <= 0)
                return;

            if (orientation == EScrollOrientation.Vertical)
            {
                if (rect.Height < rect.Width)
                    return;
                rect.X += 1;
                r = (float)System.Math.Floor(rect.Width / 2f);
            }
            else
            {
                if (rect.Width < rect.Height)
                    return;
                rect.Y += 1;
                r = (float)System.Math.Floor(rect.Height / 2f);
            }

            g.FillRoundedRect(new NanoSolidBrush(ThumbColor), rect.X, rect.Y, rect.Width, rect.Height, r);
        }

        public void DrawPressed(GContext g, Rectangle rect, EScrollOrientation orientation)
        {
            if (rect.Width <= 0 || rect.Height <= 0)
                return;

            if (orientation == EScrollOrientation.Vertical)
            {
                if (rect.Height < rect.Width)
                    return;
            }
            else
            {
                if (rect.Width < rect.Height)
                    return;
            }

            Rectangle rcBorder = rect; // GetBorderRectangle(rect);
            rcBorder.Width -= 1;
            rcBorder.Height -= 1;
            /*GraphicsPath myPath;

			if (rect.Width <= 1 || rect.Height <= 1)
				return;

			if (orientation == EScrollOrientation.Vertical)
				myPath = GetVGraphicPath(rcBorder);
			else
				myPath = GetHGraphicPath(rcBorder);

			LinearGradientBrush brushBackGround = new LinearGradientBrush(rcBorder,
			                                                              BackgroundBottomColor,
			                                                              BackgroundTopColor,
			                                                              orientation == EScrollOrientation.Horizontal ?
			                                                              LinearGradientMode.Vertical
			                                                              : LinearGradientMode.Horizontal);
			float[] relativeIntensisities =
			{
				0f,
				0.22f,
				0.32f,
				1f
			};
			float[] relativePositions =
			{
				0f,
				0.01f,
				0.3f,
				1f
			};
			Blend blend = new Blend();
			blend.Factors = relativeIntensisities;
			blend.Positions = relativePositions;
			brushBackGround.Blend = blend;
			g.FillPath(brushBackGround, myPath);

			LinearGradientBrush brushPen = new LinearGradientBrush(new Rectangle(rect.X, rect.Y, rect.Width + 1, rect.Height + 1),
			                                                       BorderBottomColor,
			                                                       BorderTopColor,
			                                                       LinearGradientMode.ForwardDiagonal);
			brushPen.Blend = blend;
			Pen ps = new Pen(brushPen);
			g.DrawPath(ps, myPath);
*/
            /*if(orientation == EOrientation.Horizontal)
				DrawHBorder(g, ps, rcBorder);
			else
				DrawVBorder(g, ps, rcBorder);*/
        }

        //Create Grahics Path for the elongated buttons
        /*public GraphicsPath GetHGraphicPath(Rectangle rc)
		{
			int adjust = (rc.Height % 2 == 0 ? 0 : 1);
			GraphicsPath Mypath = new GraphicsPath();

			//Add Top Line
			Mypath.AddLine(rc.Left + Convert.ToSingle(rc.Height / 2), rc.Top, rc.Right - Convert.ToSingle(rc.Height / 2), rc.Top);
			//Add Right Semi Circle
			Mypath.AddArc(rc.Right - rc.Height, rc.Top, rc.Height, rc.Height, 270, 180);
			//Add Bottom Line
			Mypath.AddLine(rc.Right - Convert.ToSingle(rc.Height / 2) - adjust, rc.Bottom, rc.Left + Convert.ToSingle(rc.Height / 2) + adjust, rc.Bottom);
			//Add Left Semi Circle
			Mypath.AddArc(rc.Left, rc.Top, rc.Height, rc.Height, 90, 180);

			return Mypath;
		}*/

        /*public GraphicsPath GetVGraphicPath(Rectangle rc)
		{
			int adjust = (rc.Width % 2 == 0 ? 0 : 1);
			float div2 = Convert.ToSingle(rc.Width / 2);
			GraphicsPath Mypath = new GraphicsPath();

			// right line
			Mypath.AddLine(rc.Right, rc.Top + div2, rc.Right, rc.Bottom - div2);
			// bottom arc
			Mypath.AddArc(rc.Left, rc.Bottom - rc.Width, rc.Width, rc.Width, 0, 180);

			// left line
			Mypath.AddLine(rc.Left, rc.Bottom - div2 - adjust, rc.Left, rc.Top + div2 + adjust);
			// top arc
			Mypath.AddArc(rc.Left, rc.Top, rc.Width, rc.Width, 180, 180);

			return Mypath;
		}*/

        public void DrawHBorder(System.Drawing.Graphics g, Pen p, Rectangle rc)
        {
            int adjust = (rc.Height % 2 == 0 ? 0 : 1);
            float div2 = Convert.ToSingle(rc.Height / 2);

            // top - horizontal
            g.DrawLine(p, rc.Left + div2, rc.Top, rc.Right - div2, rc.Top);
            // right - arc
            g.DrawArc(p, rc.Right - rc.Height, rc.Top, rc.Height, rc.Height, 270, 180);

            // Bottom - Horizontal
            g.DrawLine(p,
                       rc.Right - div2 - adjust, rc.Bottom,
                       rc.Left + div2 + adjust, rc.Bottom);
            // left - horizontal
            g.DrawArc(p, rc.Left, rc.Top, rc.Height, rc.Height, 90, 180);
        }

        public void DrawVBorder(System.Drawing.Graphics g, Pen p, Rectangle rc)
        {
            int adjust = (rc.Width % 2 == 0 ? 0 : 1);
            float div2 = Convert.ToSingle(rc.Width / 2);

            // right - vertical
            g.DrawLine(p, rc.Right, rc.Top + div2, rc.Right, rc.Bottom - div2);
            // bottom - vertical
            g.DrawArc(p, rc.Left, rc.Bottom - rc.Width, rc.Width, rc.Width, 0, 180);

            // left - vertical
            g.DrawLine(p,
                       rc.Left, rc.Bottom - div2 - adjust,
                       rc.Left, rc.Top + div2 + adjust);
            // top - vertical
            g.DrawArc(p, rc.Left, rc.Top, rc.Width, rc.Width, 180, 180);
        }

        public void DrawBorder(System.Drawing.Graphics g, Color c, Rectangle rect)
        {
            using (var p = new Pen(c))
            {
                DrawBorder(g, p, rect);
            }
        }

        public void DrawBorder(System.Drawing.Graphics g, Pen p, Rectangle rect)
        {
            // si el ancho de pixel de la línea es impar restar 1
            // if(p.Width % 2 == 0)
            if (!(System.Math.Abs(p.Width % 2) < float.Epsilon))
            {
                rect.Width -= 1;
                rect.Height -= 1;
            }
            g.DrawRectangle(p, rect);
        }

        public Rectangle GetBorderRectangle(Rectangle rect)
        {
            Rectangle rc = rect;
            if (rc.Height % 2 == 0)
                return new Rectangle(rc.X + 1, rc.Y + 1, rc.Width - 3, rc.Height - 2);
            return new Rectangle(rc.X + 1, rc.Y + 1, rc.Width - 3, rc.Height - 3);
        }

        public void DrawChannel2(GContext g, Rectangle rect, EScrollOrientation orientation)
        {
            float r;

            if (orientation == EScrollOrientation.Horizontal)
                r = rect.Height / 2f;
            else
                r = rect.Width / 2f;

            g.FillRoundedRect(new NanoSolidBrush(bodyColor), rect.X, rect.Y, rect.Width, rect.Height, r);
        }

        public void DrawChannel(GContext g, Rectangle rect, EScrollOrientation orientation)
        {
            /*
			rect.Width -= 1;
			rect.Height -= 1;

			if (rect == Rectangle.Empty || rect.Width <= 0 || rect.Height <= 0 ||
			   (orientation == EScrollOrientation.Vertical && rect.Width > rect.Height) ||
			   (orientation == EScrollOrientation.Horizontal && rect.Width < rect.Height))
				return;

			GraphicsPath myPath;

			if (orientation == EScrollOrientation.Vertical)
				myPath = GetVGraphicPath(rect);
			else
				myPath = GetHGraphicPath(rect);

			g.FillPath(new SolidBrush(bodyColor), myPath);

			LinearGradientMode borderGradMode;

			if (Environment.OSVersion.Platform == PlatformID.Unix)
				borderGradMode = orientation == EScrollOrientation.Horizontal ?
					LinearGradientMode.Vertical
					: LinearGradientMode.Horizontal;
			else
				borderGradMode = LinearGradientMode.ForwardDiagonal;


			// ======= solución artefacto - línea negra (rect.Width + 1, rect.Height + 1)
			// ======= al calcular "LinearGradientBrush"

			LinearGradientBrush brushPen =
				new LinearGradientBrush(new Rectangle(rect.X, rect.Y, rect.Width + 1, rect.Height + 1),
				                        cDark,
				                        cLight,
				                        borderGradMode //LinearGradientMode.ForwardDiagonal
				                        //orientation == EOrientation.Vertical
				                        //? LinearGradientMode.Horizontal : LinearGradientMode.Vertical
				);
			Pen ps = new Pen(brushPen);

			g.DrawPath(ps, myPath);

			myPath.Dispose();
			ps.Dispose();*/
        }

        #endregion elongated button
    }
}
