using NthDimension.Forms.Delegates;
using NthDimension.Forms.Events;
using NthDimension.Forms.Layout;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

#pragma warning disable CS0067

namespace NthDimension.Forms
{
    public partial class Widget : IDisposable
    {
        #region Old Implementation

        #region Events

        public event PaintHandler PaintEvent;

        public event HideChangedHandler HideChangedEvent;
        public event SizeChangedHandler SizeChangedEvent;

        public event KeyDownHandler KeyDownEvent;
        public event KeyPressHandler KeyPressEvent;
        public event KeyUpHandler KeyUpEvent;

        public event MouseDownHandler MouseDownEvent;
        public event MouseUpHandler MouseUpEvent;
        public event MouseMoveHandler MouseMoveEvent;
        public event MouseEnterHandler MouseEnterEvent;
        public event MouseLeaveHandler MouseLeaveEvent;
        public event MouseWheelHandler MouseWheelEvent;
        public event EventHandler GotFocusEvent;
        public event EventHandler LostFocusEvent;

        public event MouseClickHandler MouseClickEvent;
        public event MouseDoubleClickHandler MouseDoubleClickEvent;

        #endregion Events

        static bool layoutEnabled = true;
        protected NanoPen widPen;
        protected NanoSolidBrush widBrush;
        protected LinearGradientBrush linNanoBrush;                       // TODO Kill Ref: Uses System.Drawing.Drawing2D

        ////protected NanoGradientBrush widGradientBrush;

        int mouseLocalX, mouseLocalY;

        DragDropPackage m_DragAndDrop_Package;

        #region Static-Constructor

        static Widget()
        {
            BackLayoutsToUpdate = new LayoutList();
        }

        #endregion Static-Constructor


        

        #region Properties

        public int TabIndex = 0;

        public DateTime Timestamp { get; set; }// = DateTime.Now;


        private List<RunOnNextPaintUpdate> CallMethodsAfterNextPaintUpdate
        {
            get;
            set;
        }

        /// <summary>
        /// Use this! Drop NanoFont equivalent references wherever possible
        /// </summary>
		public int FontHeight
        {
            get { return WHUD.LibContext.MeasureText("A", Font).Height; }
        }

        /// <summary>
        /// Use this! Drop NanoFont equivalent references wherever possible
        /// </summary>
        public int FontGlyphHeight
        {
            get { return WHUD.LibContext.MeasureText("A", FontGlyph).Height; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:NthDimension.Forms.Widgets.Widget"/>
        /// is notified for mouse move.
        /// Default value is 'false'.
        /// </summary>
        /// <value><c>true</c>, if notify for mouse move; otherwise, <c>false</c>.</value>
        public bool NotifyForMouseMove
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:NthDimension.Forms.Widgets.Widget"/>
        /// is notified for mouse down.
        /// Default value is 'false'.
        /// </summary>
        /// <value><c>true</c>, if notify for mouse down; otherwise, <c>false</c>.</value>
        public bool NotifyForMouseDown
        {
            get;
            set;
        }

        public bool NotifyForMouseUp
        {
            get;
            set;
        }

        public object Tag
        {
            get;
            set;
        }

        NanoFont Font_;
        NanoFont FontGlyph_;

        public virtual NanoFont Font
        {
            get
            {
                if (Font_ == null)
                    Font_ = NanoFont.DefaultRegular;
                return Font_;
            }
            set
            {
                if (Font_ == value)
                    return;
                Font_ = value;
            }
        }

        public virtual NanoFont FontGlyph
        {
            get
            {
                if (FontGlyph_ == null)
                    FontGlyph_ = NanoFont.DefaultIcons;
                return FontGlyph_;
            }
            set
            {
                if (FontGlyph_ == value)
                    return;
                FontGlyph_ = value;
            }
        }

        internal Spacing AnchorSpaces
        {
            get;
            private set;
        }

        public static Keys ModifierKeys
        {
            get;
            private set;
        }

        public bool Enabled
        {
            get;
            set;
        }
        /// <summary>
        /// True by default.
        /// </summary>
        public bool PaintBackGround
        {
            get;
            set;
        }

        static Widget HoveredWidget_;
        /// <summary>
        /// Control currently hovered by mouse.
        /// </summary>
        public static Widget HoveredWidget
        {
            get { return HoveredWidget_; }
            private set
            {
                if (HoveredWidget_ == value)
                    return;
                HoveredWidget_ = value;
            }
        }

        public static Window WindowDialog
        {
            get;
            protected internal set;
        }

        static bool MouseFocusCaptured_;

        protected static bool MouseFocusCaptured
        {
            get { return MouseFocusCaptured_; }
        }

        protected void CaptureMouseFocus()
        {
            MouseFocusCaptured_ = true;
        }
        /// <summary>
        /// Ideally it should be only called in 'WHUD.ProcessMouseUp()'.
        /// </summary>
        private void ReleaseCapturedMouseFocus()
        {
            MouseFocusCaptured_ = false;
        }

        internal static LayoutList BackLayoutsToUpdate
        {
            get;
            private set;
        }

        public bool DoRepaint
        {
            get;
            internal set;
        }

        public static bool DoRepaintTree
        {
            get;
            internal set;
        }

        EAnchorStyle Anchor_;

        public EAnchorStyle Anchor
        {
            get { return Anchor_; }
            set
            {
                if (value == Anchor_)
                    return;
                Anchor_ = value;
                if (value != EAnchorStyle.None)
                {
                    Dock_ = EDocking.None;
                    if (Parent != null && layoutEnabled)
                        IngreseLayoutToUpdate(Parent.Layout);
                }
            }
        }

        EDocking Dock_;

        /// <summary>
        /// Dock position.
        /// </summary>
        public EDocking Dock
        {
            get { return Dock_; }
            set
            {
                if (Dock_ == value)
                    return;

                Dock_ = value;
                if (value != EDocking.None)
                {
                    Anchor_ = EAnchorStyle.None;
                    if (Parent != null && !Parent.LayoutRunning)
                        IngreseLayoutToUpdate(Parent.Layout);
                    //if(!LayoutRunning)
                    //	IngreseLayoutToUpdate(Layout);
                }
            }
        }

        LayoutBase Layout_;

        public virtual LayoutBase Layout
        {
            get
            {
                if (Layout_ == null)
                    Layout_ = new DockAnchorLayout(this);
                return Layout_;
            }
        }
        /// <summary>
        /// Gets or sets the name. Only for debug.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        // A Widget can only have a parent Widget.
        public virtual Widget Parent
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets a value that indicates whether the control and all its secondary controls are displayed.
        /// This value is updated in the trimming check within the method 'DoPaint()'.
        /// </summary>
        public bool VisibleAfterClipping
        {
            get;
            //private set;
            set;
        }

        bool LookUpIsHide(Widget w)
        {
            var wParent = w;

            while (wParent != null)
            {
                if (wParent.IsHide_)
                    return true;
                wParent = wParent.Parent;
            }
            return false;
        }

        void SetHideValue()
        {
            IsHide_ = !IsHide_;

            if (IsHide_ && IsFocused)
                Focus(true);

            if (IsHide_)
                UpdateOnEnter(this);

            OnHideChanged(EventArgs.Empty);
        }

        bool IsHide_;
        /// <summary>
        /// get/set: Indicates if the Widget is hidden. If any of the container Widgets is hidden, this one will be hidden too.
        /// </summary>
        public bool IsHide
        {
            get { return LookUpIsHide(this); }
            set
            {
                if (Parent == null)
                {
                    IsHide_ = value;
                }
                else if (value != IsHide_)
                {
                    SetHideValue();
                    IngreseLayoutToUpdate(Parent.Layout);
                    PerformLayout();
                    Repaint();
                }
            }
        }

        public bool IsVisible
        {
            get { return !IsHide; }
        }

        /// <summary>
        /// Sets the hide. This prevents 'OnEnter' failure after hiding a widget
        /// </summary>
        /// <param name="w">The widget.</param>
        void UpdateOnEnter(Widget w)
        {
            foreach (Widget sw in w.Widgets)
                UpdateOnEnter(sw);
            w.onEnter = false;
        }

       

        private bool LayoutRunning
        {
            get;
            set;
        }

        #region Location

        Rectangle _outer;           //Refactoring from Point to Rectangle (GLGUI)
        Rectangle _inner;
        Size _sizeMin;
        Size _sizeMax;

        public Point Location
        {
            get { return _outer.Location; }
            set
            {
                if (_outer.Location == value)
                    return;

                if (layoutEnabled)
                {
                    if (!LayoutRunning && Parent != null)
                        IngreseLayoutToUpdate(Parent.Layout);

                    if (Widgets.Count > 0)
                        IngreseLayoutToUpdate(Layout);

                    _outer.Location = value;
                    if (!LayoutRunning)
                        CalcAnchorSpaces();
                }
                else
                {
                    _outer.Location = value;
                    CalcAnchorSpaces();
                }

                CalcClientRect();
                Repaint();
            }
        }

        #endregion Location

        #region Size

        public Size MinimumSize { get; set; }

        public Size Size
        {
            get { return _outer.Size; }
            set
            {
                if (value.Width < 0)
                    value.Width = 0;
                if (value.Height < 0)
                    value.Height = 0;

                if (value.Width < MinimumSize.Width)
                    value.Width = MinimumSize.Width;
                if (value.Height < MinimumSize.Height)
                    value.Height = MinimumSize.Height;



                if (_outer.Size == value)
                    return;

                if (layoutEnabled)
                {
                    if (!LayoutRunning
                        && Parent != null
                        && !Parent.LayoutRunning)
                    {
                        IngreseLayoutToUpdate(Parent.Layout);
                    }
                    // change: added "!LayoutRunning"
                    if (!LayoutRunning
                        && Widgets.Count > 0)
                    {
                        IngreseLayoutToUpdate(Layout);
                    }
                    _outer.Size = value;
                    if (!LayoutRunning)
                        CalcAnchorSpaces();
                }
                else
                {
                    _outer.Size = value;
                    CalcAnchorSpaces();
                }
                CalcClientRect();
                OnSizeChanged();
                Repaint();
            }
        }

        #endregion Size

        public Rectangle Bounds
        {
            get { return new Rectangle(X, Y, Width, Height); }
            set
            {
                if (value.Width < 0 || value.Height < 0)
                    return;

                if (value.X != X || value.Y != Y)
                    Location = new Point(value.X, value.Y);
                if (value.Width != Width || value.Height != Height)
                    Size = new Size(value.Width, value.Height);
            }
        }

        public int X
        {
            get { return _outer.X; }
        }

        public int Y
        {
            get { return _outer.Y; }
        }

        public virtual int Left
        {
            get
            {
                return _outer.X;
            }
        }

        public virtual int Right
        {
            get
            {
                return Left + Width;
            }
        }

        public virtual int Top
        {
            get
            {
                return _outer.Y;
            }
        }

        public virtual int Bottom
        {
            get
            {
                return Top + Height;
            }
        }

        public virtual int Width
        {
            get { return Size.Width; }
        }

        public virtual int Height
        {
            get { return Size.Height; }
        }

        public WidgetList Widgets
        {
            get;
            protected set;
            //set;
        }

        Color BGColor_;

        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>The color of the background.</value>
        public Color BGColor
        {
            get { return BGColor_; }
            set
            {
                if (value == BGColor_)
                    return;
                BGColor_ = value;
                OnBackColorChanged(EventArgs.Empty);
            }
        }
        Color FGColor_;
        ///// <summary>
        ///// Gets or sets the color of the foreground.
        ///// </summary>
        ///// <value>The color of the foreground.</value>
        public Color FGColor
        {
            get { return FGColor_; }
            set
            {
                if (value == FGColor_)
                    return;
                FGColor_ = value;
                OnForeColorChanged(EventArgs.Empty);
            }
        }
        /// <summary>
        /// Gets or sets the gradient start color.
        /// </summary>
        /// <value>The gradient start color.</value>
        Color STColor_;
        public Color STColor
        {
            get { return STColor_; }
            set
            {
                if (value == STColor_)
                    return;
                STColor_ = value;
                OnBackColorChanged(EventArgs.Empty);
            }
        }

        Color EDColor_;
        public Color EDColor
        {
            get { return EDColor_; }
            set
            {
                if (value == EDColor_)
                    return;
                EDColor_ = value;
                OnBackColorChanged(EventArgs.Empty);
            }
        }

        Rectangle STEDRect_;
        public Rectangle STEDRect
        {
            get { return STEDRect_; }
            set
            {
                if (value == STEDRect_)
                    return;
                STEDRect_ = value;
                OnBackColorChanged(EventArgs.Empty);
            }
        }

        //public Rectangle STEDRect
        //{
        //    get { return _STEDRect = new Rectangle(this.Location, this.Size); }
        //    set { _STEDRect = value; }
        //}

        //public LinearGradientBrush LinBrush { get; set; }

        //public LinearGradientMode LinBrushMode { get; set; }







        Spacing Margin_;

        /// <summary>
        /// Current margin - outer spacing.
        /// </summary>
        public Spacing Margin
        {
            get { return Margin_; }
            set
            {
                if (Margin_ == value)
                    return;

                Margin_ = value;

                if (Parent != null)
                    Parent.PerformLayout();
                CalcClientRect();
                OnMarginChanged();
            }
        }

        Spacing Padding_;

        /// <summary>
        /// Current padding - inner spacing. 
        /// It only affects the Docking operations of this Widget.
        /// </summary>
        public Spacing Padding
        {
            get { return Padding_; }
            set
            {
                if (Padding_ == value)
                    return;

                Padding_ = value;
                PerformLayout();
                CalcClientRect();
                OnPaddingChanged();
            }
        }

        /// <summary>
        /// The client rect. Local coordinates to the Widget and position (0, 0).
        /// </summary>
        public Rectangle ClientRect
        {
            get;
            internal set;
        }

        public Rectangle HitRect
        {
            get;
            internal set;
        }

        internal Spacing LayoutMargin
        {
            get;
            set;
        }

        #region Debug-util

        public bool ShowBoundsLines { get; set; }

        public Color BoundsOutlineColor { get; set; }

        public bool ShowPaddingLines { get; set; }

        public Color PaddingOutlineColor { get; set; }

        public bool ShowMarginLines { get; set; }

        public Color MarginOutlineColor { get; set; }

        #endregion Debug-util

        /// <summary>
        /// Widget that currently has mouse focus.
        /// A single <see cref="Widget"/> can acquire the focus of the mouse, when
        /// is assigned directly with the Focus () method
        /// </summary>
        public static Widget FocusedWidget
        {
            get;
            protected set;
        }

        public bool IsFocused
        {
            get { return this == FocusedWidget; }
        }

        public WHUD WindowHUD
        {
            get
            {
                Widget wid = this;

                while (wid.Parent != null)
                    wid = wid.Parent;

                if (wid.GetType().IsSubclassOf(typeof(WHUD)))
                    return (WHUD)wid;
                else
                    return null;
            }
        }

        #endregion Properties

        #region Internal-Protected methods

        protected virtual void RaiseGotFocusEvent(EventArgs e)
        {
            if (GotFocusEvent != null)
                GotFocusEvent(this, e);
        }

        protected virtual void RaiseLostFocusEvent(EventArgs e)
        {
            if (LostFocusEvent != null)
                LostFocusEvent(this, e);
        }

        protected virtual bool ProcessDialogKey(Keys keyData)
        {
            return false;
        }

        protected void Repaint(Rectangle r)
        {
            throw new NotImplementedException();
        }

        #region SearchClipRectangle
        /*
		internal Rectangle SearchClipRect()
		{
			bool isVisible;
			Rectangle clipRect;

			if (Parent != null)
			{
				clipRect = Parent.SearchClipRect();
				var baseRect = new Rectangle(LocalToWindow(), new Size(Width, Height));
				clipRect = CpuScissors2(baseRect, clipRect, out isVisible);
			}
			else
				clipRect = new Rectangle(0, 0, Width, Height);

			return clipRect;
		}

		internal Rectangle SearchClipRect(out bool isVisible)
		{
			Rectangle clipRect;

			if (Parent != null)
			{
				clipRect = Parent.SearchClipRect(out isVisible);
				var baseRect = new Rectangle(LocalToWindow(), new Size(Width, Height));
				clipRect = CpuScissors2(baseRect, clipRect, out isVisible);
			}
			else
			{
				clipRect = new Rectangle(0, 0, Width, Height);
				isVisible = true;
			}

			return clipRect;
		}
		 */
        #endregion SearchClipRectangle

        #endregion Internal-Protected methods

        #region Private methods

        // TODO it must be 'private'
        public void SetParent(Widget parent)
        {
            Parent = parent;

            OnParentChanged();
        }

        private void CalcAnchorSpaces()
        {
            if (Anchor != EAnchorStyle.None)
            {
                int b = Parent == null ? 0 : Parent.Height - (Y + Height);
                int t = Parent == null ? 0 : Y;
                int l = Parent == null ? 0 : X;
                int r = Parent == null ? 0 : Parent.Width - (X + Width);

                if (!Anchor.HasFlag(EAnchorStyle.Bottom))
                    b = 0;
                else if (!Anchor.HasFlag(EAnchorStyle.Top))
                    t = 0;
                else if (!Anchor.HasFlag(EAnchorStyle.Left))
                    l = 0;
                else if (!Anchor.HasFlag(EAnchorStyle.Right))
                    r = 0;

                AnchorSpaces = new Spacing(l, t, r, b);
            }
        }

        private void CalcClientRect()
        {
            ClientRect = new Rectangle(Padding.Left, Padding.Top,
                                       Width - Padding.Left - Padding.Right,
                                       Height - Padding.Top - Padding.Bottom);
        }

        private void DrawDebugMarginLines(GContext gc)
        {
            widPen.Color = MarginOutlineColor;
            gc.DrawRectangle(widPen,
                             X - Margin.Left,
                             Y - Margin.Top,
                             Width + Margin.Right + Margin.Left,
                             Height + Margin.Bottom + Margin.Top);
        }

        private void DrawDebugLines(GContext gc)
        {
            if (ShowPaddingLines && Padding != Spacing.Zero)
            {
                widPen.Color = PaddingOutlineColor;
                gc.DrawRectangle(widPen,
                                 Padding.Left, Padding.Top,
                                 Width - Padding.Right - Padding.Left,
                                 Height - Padding.Bottom - Padding.Top);
            }

            if (ShowBoundsLines)
            {
                widPen.Color = BoundsOutlineColor;
                gc.DrawRectangle(widPen,
                                 0, 0,
                                 Width, Height);
            }
        }

        /// <summary>
        /// Intersect rectangles, all in window coordinates
        /// </summary>
        /// <param name="baseRect"></param>
        /// <param name="parentClipRect"></param>
        /// <param name="isVisible"></param>
        /// <returns></returns>
        private Rectangle CpuScissors2(Rectangle baseRect, Rectangle parentClipRect, out bool isVisible)
        {
            Rectangle result = Rectangle.Intersect(baseRect, parentClipRect);
            isVisible = result != Rectangle.Empty;
            return result;
        }

        #endregion Private methods

        #region Event Handlers

        void HandleWidgetAdded(Widget w)
        {
            if ((w.Dock != EDocking.None
                 || w.Anchor != EAnchorStyle.None)
                && !LayoutRunning)
                IngreseLayoutToUpdate(Layout);
            // A widget can be added to another after having set its limits.
            w.CalcAnchorSpaces();
            w.CalcClientRect();
        }

        #endregion Event Handlers

        #region OnXXX() methods

        protected virtual void OnTextChanged(EventArgs e)
        {
        }

        /// <summary>
        /// Raises the layout event. Calculate the client area and call DoDockLayout () and DoAnchorLayout () if DoAnchorLayout equals true.
        /// </summary>
        protected virtual void OnLayout()
        {
        }

        protected virtual void OnBeforeParentLayout()
        {
        }

        protected virtual void OnSizeChanged()
        {
            if (SizeChangedEvent != null)
                SizeChangedEvent(this, EventArgs.Empty);
        }

        protected virtual void OnHideChanged(EventArgs ea)
        {
            if (HideChangedEvent != null)
                HideChangedEvent(this, EventArgs.Empty);
        }

        protected virtual void OnPaddingChanged()
        {
            //throw new NotImplementedException();
        }

        protected virtual void OnMarginChanged()
        {
            //throw new NotImplementedException();
        }

        protected virtual void OnPaintBackground(GContext gc)
        {
            gc.Clear();
        }

        protected virtual void OnParentChanged()
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Raises the paint event. By default, it paints the widget area of the background color.
        /// </summary>
        protected virtual void DoPaint(PaintEventArgs e)
        {
            //throw new NotImplementedException();
        }

        bool onEnter = false;

        protected virtual void OnMouseEnter(EventArgs e)
        {
            if (MouseEnterEvent != null)
                MouseEnterEvent(this, e);
        }

        protected virtual void OnMouseMove(MouseEventArgs e)
        {
            if (MouseMoveEvent != null)
                MouseMoveEvent(this, e);
        }

        public virtual void OnMouseWheel(MouseEventArgs e)
        {
            if (MouseWheelEvent != null)
                MouseWheelEvent(this, e);
        }

        protected virtual void OnMouseLeave(EventArgs e)
        {
            if (MouseLeaveEvent != null)
                MouseLeaveEvent(this, e);
        }

        protected virtual void OnNotifyMouseDown(MouseEventArgs e)
        {
            OnMouseDown(new MouseDownEventArgs(e.Button, e.X, e.Y, e.DeltaX, e.DeltaY, e.DeltaWheel, e.Clicks));
        }

        protected virtual void OnMouseDown(MouseDownEventArgs e)
        {
            if (MouseDownEvent != null)
                MouseDownEvent(this, e);
        }

        public virtual void OnMouseClick(MouseEventArgs e)
        {
            if (MouseClickEvent != null)
                MouseClickEvent(this, e);
        }

        public virtual void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (MouseDoubleClickEvent != null)
                MouseDoubleClickEvent(this, e);
        }

        protected virtual void OnMouseUp(MouseEventArgs e)
        {
            if (MouseUpEvent != null)
                MouseUpEvent(this, e);
        }

        protected virtual void OnGotFocus(EventArgs e)
        {
            if (GotFocusEvent != null)
                GotFocusEvent(this, e);
        }

        protected virtual void OnLostFocus(EventArgs e)
        {
            if (LostFocusEvent != null)
                LostFocusEvent(this, e);
        }

        protected virtual void OnBackColorChanged(EventArgs e)
        {
        }

        protected virtual void OnForeColorChanged(EventArgs e)
        {
        }

        protected virtual void OnStartColorChanged(EventArgs e)
        {
        }

        protected virtual void OnEndColorChanged(EventArgs e)
        {
        }

        protected virtual void OnKeyDown(KeyEventArgs e)
        {
            if (KeyDownEvent != null)
                KeyDownEvent(this, e);
        }

        protected virtual void OnKeyPress(KeyPressedEventArgs e)
        {
            if (KeyPressEvent != null)
                KeyPressEvent(this, e);
        }

        protected virtual void OnKeyUp(KeyEventArgs e)
        {
            if (KeyUpEvent != null)
                KeyUpEvent(this, e);
        }
        #endregion OnXXX() methods

        #region Update-Logic

        /// <summary>
        /// Suspends the layout. It is only necessary when the property is used <see cref="Anchor"/>
        /// </summary>
        public static void SuspendLayout()
        {
            layoutEnabled = false;
        }
        /// <summary>
        /// Resumes the layout. It is only necessary when the property is used <see cref="Anchor"/>
        /// </summary>
        public static void ResumeLayout()
        {
            layoutEnabled = true;
        }

        protected void ParentPerformLayout()
        {
            if (Parent != null && !Parent.LayoutRunning)
                IngreseLayoutToUpdate(Parent.Layout);
        }

        protected void PerformLayout()
        {
            IngreseLayoutToUpdate(Layout);
        }

        void IngreseLayoutToUpdate(LayoutBase lb)
        {
            if (!LayoutRunning)
            {
                BackLayoutsToUpdate.Add(lb);
            }
        }

        protected void RunAfterNextPaintUpdate(RunOnNextPaintUpdate runMethod)
        {
            CallMethodsAfterNextPaintUpdate.Add(runMethod);
            Repaint();
        }
        #endregion Update-Logic

        #region Draw-Logic

        /// <summary>
        /// Show this instance.
        /// </summary>
        protected virtual void DoPaint(GContext parentGContext)
        {
            GContext fpgc = parentGContext.CreateGContext(this);
            VisibleAfterClipping = fpgc.IsVisible;
            DoRepaint = false;

            if (VisibleAfterClipping && !IsHide)
            {
                if (this is Overlay)
                    parentGContext.FillBorderShadow(X, Y, Width, Height);

                if (ShowMarginLines)
                    DrawDebugMarginLines(parentGContext);

                InternalPaint(fpgc);
                foreach (RunOnNextPaintUpdate ronpu in CallMethodsAfterNextPaintUpdate)
                    ronpu();
                CallMethodsAfterNextPaintUpdate.Clear();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gc"><see cref="GContext"/> of this <see cref="Widget"/></param>
        internal void InternalPaint(GContext gc)
        {
            //NanoVG.nvgSave(gc.);

            if (PaintBackGround)
                OnPaintBackground(gc);
            DoPaint(new PaintEventArgs(gc, gc.ClipRect));

            // ?? It is necessary to retrace the list in reverse order so that when two widgets 
            // are superimposed, one on top of the other, the order in which the "HoveretWidget" 
            // widget is identified and the painting order match. ??
            for (int cont = Widgets.Count - 1; cont >= 0; cont--)
            {
                try
                {
                    Widget w = Widgets[cont];
                    w.DoPaint(gc);
                }
                catch /*(Exception ipE)*/// (Exception ipE)
                {
                }
            }

            if (ShowBoundsLines || ShowPaddingLines)
                DrawDebugLines(gc);

            //NanoVG.nvgRestore(gc);

            gc.Dispose();


        }

        #endregion Draw-Logic

        #region API
        public virtual void Show()
        {
            IsHide = false;
        }

        public virtual void Hide()
        {
            //if (null != WindowHUD)
            //{
            //    if (WindowHUD.Overlays.Contains(this))
            //    {
            //        WindowHUD.Overlays.Remove(this);
            //        WindowHUD.OverlayCurrent = null;
            //    }
            //}
            IsHide = true;
        }

        public void Repaint()
        {
            DoRepaint = true;
            DoRepaintTree = true;
            if (Parent != null)
                Parent.Repaint();
        }

        public void Invalidate()
        {
            Repaint();
        }

        public void Invalidate(Rectangle r)
        {
            Repaint();
        }

        public virtual Widget GetControlAt(int x, int y, out int lx, out int ly)
        {
            if (VisibleAfterClipping == false || x < 0 || y < 0 || x >= Width || y >= Height || null == Widgets)
            {
                lx = 0;
                ly = 0;
                return null;
            }

            foreach (Widget child in Widgets)
            {
                Widget found = child.GetControlAt(x - child.X, y - child.Y, out lx, out ly);

                if (found != null && !found.IsHide)
                {
                    return found;
                }
            }



            lx = x;
            ly = y;
            return this;
        }

        public bool Focus(bool forceLostFocus = false)
        {
            if (forceLostFocus)
            {
                //ReleaseFocus();
                if (IsFocused)
                    OnLostFocus(EventArgs.Empty);
                FocusedWidget = null;
                return false;
            }

            if (this != FocusedWidget && !MouseFocusCaptured)
            {
                //if (MouseFocus == null)
                //	MouseFocus = HoveredWidget;
                //else
                {
                    if (FocusedWidget != null)
                        FocusedWidget.OnLostFocus(EventArgs.Empty);
                    FocusedWidget = this;
                }

                FocusedWidget.OnGotFocus(EventArgs.Empty);

                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format("[Widget Name={0}]", Name);
        }
        /*
		public Point LocalToWindowLocation0()
		{
			if (Parent != null)
			{
				Point spoint = Parent.LocalToWindowLocation0();

				return new Point(spoint.X + X, spoint.Y + Y);
			}
			return new Point(X, Y);
		}
		 */
        public Point LocalToWindow()
        {
            var p = new Point(X, Y);

            if (Parent == null)
                return p;
            Point p2 = Parent.LocalToWindow();
            return new Point(p2.X + X, p2.Y + Y);
        }


        /// <summary>
        /// Converts Window coordinates to local coordinates.
        /// </summary>
        /// <returns>The to local.</returns>
        /// <param name="pnt">Pnt.</param>
        public virtual Point WindowToLocal(Point pnt)
        {
            return WindowToLocal(pnt.X, pnt.Y);
        }

        /// <summary>
        /// Converts Window coordinates to local coordinates.
        /// </summary>
        /// <param name="wx">X WHUD coordinates.</param>
        /// <param name="wy">Y WHUD coordinates.</param>
        /// <returns>Local coordinates.</returns>
        public virtual Point WindowToLocal(int wx, int wy)
        {
            if (Parent != null)
            {
                int x = wx - X;
                int y = wy - Y;

                return Parent.WindowToLocal(x, y);
            }

            return new Point(wx, wy);
        }

        public bool PackChildren(int w, int h, out int x, out int y)
        {
            for (int i = 0; i < Widgets.Count; i++)
            {
                if (w <= Widgets[i].Width && h <= Widgets[i].Height)
                {
                    Widget node = Widgets[i];
                    Widgets.RemoveAt(i);

                    x = node.X;
                    y = node.Y;

                    int r = x + w;  // bottom
                    int b = y + h;  // right


                }

            }


            x = 0;
            y = 0;
            return false;
        }

        public void WidgetsSortBy(string variableName)
        {
            WidgetList ws = Widgets.OrderBy(o => o.GetType().GetProperty(variableName).GetValue(o)) as WidgetList;

            if (null != ws)
                Widgets = ws;
        }
        public void WidgetsSortByDescending(string variableName)
        {
            WidgetList ws = Widgets.OrderByDescending(o => o.GetType().GetProperty(variableName).GetValue(o)) as WidgetList;

            if (null != ws)
                Widgets = ws;
        }
        #endregion API

        /// NEW API

        #region new OnXXXX
        /// <summary>
        /// Handler for keyboard events.
        /// </summary>
        /// <param name="key">Key pressed.</param>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>True if handled.</returns>
        protected virtual bool OnKeyPressed(/*enuKey key,*/ bool down = true)
        {
            throw new NotImplementedException();
            //bool handled = false;
            ////switch (key)
            ////{
            ////    case enuKey.Tab: handled = OnKeyTab(down); break;
            ////    case enuKey.Space: handled = OnKeySpace(down); break;
            ////    case enuKey.Home: handled = OnKeyHome(down); break;
            ////    case enuKey.End: handled = OnKeyEnd(down); break;
            ////    case enuKey.Return: handled = OnKeyReturn(down); break;
            ////    case enuKey.Back: handled = OnKeyBackspace(down); break;
            ////    case enuKey.Delete: handled = OnKeyDelete(down); break;
            ////    case enuKey.Right: handled = OnKeyRight(down); break;
            ////    case enuKey.Left: handled = OnKeyLeft(down); break;
            ////    case enuKey.Up: handled = OnKeyUp(down); break;
            ////    case enuKey.Down: handled = OnKeyDown(down); break;
            ////    case enuKey.Escape: handled = OnKeyEscape(down); break;
            ////    default: break;
            ////}

            ////if (!handled && Parent != null)
            ////    Parent.OnKeyPressed(key, down);

            //return handled =false;
        }
        #endregion

        #region DragDrop
        // Note: New Funcs
        public event Func<Widget, DragDropPackage, bool> DragAndDropCanAcceptPackage;
        public event Func<Widget, DragDropPackage, int, int, bool> DragAndDropHandleDrop;

        // giver
        public virtual DragDropPackage DragAndDrop_GetPackage(int x, int y)
        {
            return m_DragAndDrop_Package;
        }
        // giver
        public virtual bool DragAndDrop_Draggable()
        {
            if (m_DragAndDrop_Package == null)
                return false;

            return m_DragAndDrop_Package.IsDraggable;
        }
        // giver
        public virtual void DragAndDrop_SetPackage(bool draggable, String name = "", object userData = null)
        {
            if (m_DragAndDrop_Package == null)
            {
                m_DragAndDrop_Package = new DragDropPackage();
                m_DragAndDrop_Package.IsDraggable = draggable;
                m_DragAndDrop_Package.Name = name;
                m_DragAndDrop_Package.UserData = userData;
            }
        }
        // giver
        public virtual bool DragAndDrop_ShouldStartDrag()
        {
            return true;
        }
        // giver
        public virtual void DragAndDrop_StartDragging(DragDropPackage package, int x, int y)
        {
            //throw new NotImplementedException();
            //{
            //    //// TODO:: WindowsGame.Instance.PointToScreen
            //    //package.HoldOffset = CanvasPosToLocal(new Point(x, y));
            //    //package.DrawControl = this;
            //}

            package.HoldOffset = this.WindowToLocal(new Point(x, y)); //CanvasPosToLocal(new Point(x, y));
            package.DrawControl = this;
        }
        // giver
        public virtual void DragAndDrop_EndDragging(bool success, int x, int y)
        {
        }
        // receiver
        public virtual bool DragAndDrop_HandleDrop(DragDropPackage p, int x, int y)
        {
            DragAndDrop.SourceControl.Parent = this;
            return true;
        }
        // receiver
        public virtual void DragAndDrop_HoverEnter(DragDropPackage p, int x, int y)
        {

        }
        // receiver
        public virtual void DragAndDrop_HoverLeave(DragDropPackage p)
        {

        }
        // receiver
        public virtual void DragAndDrop_Hover(DragDropPackage p, int x, int y)
        {

        }
        // receiver
        public virtual bool DragAndDrop_CanAcceptPackage(DragDropPackage p)
        {
            return false;
        }
        #endregion

        /// <summary>
        /// Resizes the control to fit its children.
        /// </summary>
        /// <param name="width">Determines whether to change control's width.</param>
        /// <param name="height">Determines whether to change control's height.</param>
        /// <returns>True if bounds changed.</returns>
        public virtual bool SizeToChildren(bool width = true, bool height = true)
        {
            Point size = GetChildrenSize();
            size.X += Padding.Right;
            size.Y += Padding.Bottom;
            //return SetSize(width ? size.X : Width, height ? size.Y : Height);
            throw new NotImplementedException();
        }
        /// <summary>
        /// Returns the total width and height of all children.
        /// </summary>
        /// <remarks>Default implementation returns maximum size of children since the layout is unknown.
        /// Implement this in derived compound controls to properly return their size.</remarks>
        /// <returns></returns>
        public virtual Point GetChildrenSize()
        {
            Point size = Point.Empty;

            foreach (Widget child in this.Widgets)
            {
                if (child.IsHide)
                    continue;

                size.X = Math.Max(size.X, child.Right);
                size.Y = Math.Max(size.Y, child.Bottom);
            }

            return size;
        }

        #endregion Old Implementation

        #region Constructor

        public Widget()
        {
            IsHide_ = false;
            CallMethodsAfterNextPaintUpdate = new List<RunOnNextPaintUpdate>();

            PaintBackGround = true;
            Name = GetType().ToString();
            Enabled = true;
            Widgets = new WidgetList(this);
            _outer = new Rectangle(0, 0, 0, 0);
            _inner = new Rectangle(0, 0, 0, 0);
            _sizeMin = new Size(0, 0);
            _sizeMax = new Size(int.MaxValue, int.MaxValue);

            Padding_ = Spacing.Zero;
            Margin_ = Spacing.Zero;

            ShowBoundsLines = false;
            BoundsOutlineColor = Color.Red;
            MarginOutlineColor = Color.Green;
            PaddingOutlineColor = Color.Blue;

            widPen = new NanoPen(FGColor);
            widBrush = new NanoSolidBrush(EDColor);

            Anchor_ = EAnchorStyle.None;
            Dock_ = EDocking.None;

            Repaint();

            Widgets.WidgetAddedEvent += HandleWidgetAdded;
        }
        #endregion Constructor

        #region Destructor

        // Flag: Has Dispose already been called?
        bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //
            if (null != Widgets)
                Widgets.Clear();

            disposed = true;

            // Call the base class implementation.
            //base.Dispose(disposing);
        }

        ~Widget()
        {
            Dispose(false);
        }

        #endregion Destructor

        /// <summary>
        /// Updates the HitRect 
        /// </summary>
        public virtual void UpdateHitRectangle()
        {
            this.HitRect = new Rectangle(this.X, this.Y, this.Width, this.Height);
        }

    }
}
