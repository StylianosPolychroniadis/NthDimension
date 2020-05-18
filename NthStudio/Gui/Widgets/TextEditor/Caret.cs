using NthDimension.Forms;
using NthStudio.Gui.Widgets.TextEditor.Document;
using NthStudio.Gui.Widgets.TextEditor.Document.FoldingStrategy;
using NthStudio.Gui.Widgets.TextEditor.Document.HighlightStrategy;
using NthStudio.Gui.Widgets.TextEditor.Document.LineManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace NthStudio.Gui.Widgets.TextEditor
{
    public class Caret : System.IDisposable
    {
        int Line_ = 0;
        int Column_ = 0;
        int desiredXPos = 0;
        CaretMode caretMode;

        static bool caretCreated = false;
        bool hidden = true;
        TextCanvas iTextCanvas;
        Point currentPos = new Point(-1, -1);
        //Ime ime = null;
        readonly CaretImplementation caretImplementation;

        public Caret(TextCanvas pTextCanvas)
        {
            this.iTextCanvas = pTextCanvas;
            pTextCanvas.GotFocusEvent += TextCanvas_GotFocus;
            pTextCanvas.LostFocusEvent += TextCanvas_LostFocus;

            //if (Environment.OSVersion.Platform == PlatformID.Unix)
            caretImplementation = new ManagedCaret(this);
            //else
            //caretImplementation = new Win32Caret(this);
            //throw new NotImplementedException("Caret implementation only for Linux");

        }

        /// <value>
        /// The 'prefered' xPos in which the caret moves, when it is moved
        /// up/down. Measured in pixels, not in characters!
        /// </value>
        public int DesiredColumn
        {
            get
            {
                return desiredXPos;
            }
            set
            {
                desiredXPos = value;
            }
        }

        /// <value>
        /// The current caret mode.
        /// </value>
        public CaretMode CaretMode
        {
            get
            {
                return caretMode;
            }
            set
            {
                caretMode = value;
                OnCaretModeChanged(EventArgs.Empty);
            }
        }
        /// <summary>
        /// Gets or sets the line where the cursor is.
        /// </summary>
        /// <value>The line.</value>
        public int Line
        {
            get
            {
                return Line_;
            }
            set
            {
                Line_ = value;
                ValidateCaretPos();
                UpdateCaretPosition();
                OnPositionChanged(EventArgs.Empty);
            }
        }

        public int Column
        {
            get
            {
                return Column_;
            }
            set
            {
                Column_ = value;
                ValidateCaretPos();
                UpdateCaretPosition();
                OnPositionChanged(EventArgs.Empty);
            }
        }

        public TextLocation Position
        {
            get
            {
                return new TextLocation(Column_, Line_);
            }

            set
            {
                Line_ = value.Y;
                Column_ = value.X;
                ValidateCaretPos();
                UpdateCaretPosition();
                OnPositionChanged(EventArgs.Empty);
            }
        }

        public int Offset
        {
            get
            {
                return iTextCanvas.Document.PositionToOffset(Position);
            }
        }

        void TextCanvas_GotFocus(object sender, EventArgs args)
        {
            Log("GotFocus, IsInUpdate=" + iTextCanvas.TextEditor.IsInUpdate);
            hidden = false;
            if (!iTextCanvas.TextEditor.IsInUpdate)
            {
                CreateCaret();
                UpdateCaretPosition();
            }
        }

        void TextCanvas_LostFocus(object sender, EventArgs args)
        {
            Log("LostFocus");
            hidden = true;
            DisposeCaret();
        }

        public void Dispose()
        {
            iTextCanvas.GotFocusEvent -= TextCanvas_GotFocus;
            iTextCanvas.LostFocusEvent -= TextCanvas_LostFocus;
            iTextCanvas = null;
            caretImplementation.Dispose();
        }


        public TextLocation ValidatePosition(TextLocation pos)
        {
            int tLine = Math.Max(0, Math.Min(iTextCanvas.Document.TotalNumberOfLines - 1, pos.Y));
            int tColumn = Math.Max(0, pos.X);

            if (tColumn == int.MaxValue || !iTextCanvas.TextEditorProperties.AllowCaretBeyondEOL)
            {
                LineSegment lineSegment = iTextCanvas.Document.GetLineSegment(tLine);
                tColumn = Math.Min(tColumn, lineSegment.Length);
            }
            return new TextLocation(tColumn, tLine);
        }

        /// <remarks>
        /// If the caret position is outside the document text bounds
        /// it is set to the correct position by calling ValidateCaretPos.
        /// </remarks>
        public void ValidateCaretPos()
        {
            Line_ = Math.Max(0, Math.Min(iTextCanvas.Document.TotalNumberOfLines - 1, Line_));
            Column_ = Math.Max(0, Column_);

            if (Column_ == int.MaxValue || !iTextCanvas.TextEditorProperties.AllowCaretBeyondEOL)
            {
                LineSegment lineSegment = iTextCanvas.Document.GetLineSegment(Line_);
                Column_ = Math.Min(Column_, lineSegment.Length);
            }
        }

        void CreateCaret()
        {
            while (!caretCreated)
            {
                switch (caretMode)
                {
                    case CaretMode.InsertMode:
                        caretCreated = caretImplementation.Create(2, iTextCanvas.TextView.LineHeight);
                        break;
                    case CaretMode.OverwriteMode:
                        caretCreated = caretImplementation.Create((int)iTextCanvas.TextView.SpaceWidth, iTextCanvas.TextView.LineHeight);
                        break;
                }
            }
            if (currentPos.X < 0)
            {
                ValidateCaretPos();
                currentPos = GetScreenPosition();
            }
            caretImplementation.SetPosition(currentPos.X, currentPos.Y);
            caretImplementation.Show();
        }

        public void RecreateCaret()
        {
            Log("RecreateCaret");
            DisposeCaret();
            if (!hidden)
            {
                CreateCaret();
            }
        }

        void DisposeCaret()
        {
            if (caretCreated)
            {
                caretCreated = false;
                caretImplementation.Hide();
                caretImplementation.Destroy();
            }
        }

        public Point GetScreenPosition()
        {
            int xpos = iTextCanvas.TextView.GetDrawingXPos(this.Line_, this.Column_);
            return new Point(iTextCanvas.TextView.DrawingPosition.X + xpos,
                             iTextCanvas.TextView.DrawingPosition.Y
                             + (iTextCanvas.Document.GetVisibleLine(this.Line_)) * iTextCanvas.TextView.LineHeight
                             - iTextCanvas.TextView.TextCanvas.VirtualTop.Y);
        }

        int oldLine = -1;
        bool outstandingUpdate;

        internal void OnEndUpdate()
        {
            if (outstandingUpdate)
                UpdateCaretPosition();
        }

        void PaintCaretLine(GContext gc)
        {
            if (!iTextCanvas.Document.TextEditorProperties.CaretLine)
                return;

            HighlightColor caretLineColor = iTextCanvas.Document.HighlightingStrategy.GetColorFor("CaretLine");

            /*g.DrawLine(BrushRegistry.GetDotPen(caretLineColor.TextColor),
			           currentPos.X,
			           0,
			           currentPos.X,
			           iTextCanvas.DisplayRectangle.Height);*/
            var np = new NanoPen(caretLineColor.Color);
            //np.Width = 2;
            gc.DrawLine(np,
                        currentPos.X,
                        0,
                        currentPos.X,
                        iTextCanvas.Bounds.Height);
        }

        public void UpdateCaretPosition()
        {
            Log("UpdateCaretPosition");

            if (iTextCanvas.TextEditorProperties.CaretLine)
            {
                iTextCanvas.Invalidate();
            }
            else
            {
                if (caretImplementation.RequireRedrawOnPositionChange)
                {
                    iTextCanvas.UpdateLine(oldLine);
                    if (Line_ != oldLine)
                        iTextCanvas.UpdateLine(Line_);
                }
                else
                {
                    if (iTextCanvas.TextArea.TextEditorProperties.LineViewerStyle == LineViewerStyle.FullRow && oldLine != Line_)
                    {
                        iTextCanvas.UpdateLine(oldLine);
                        iTextCanvas.UpdateLine(Line_);
                    }
                }
            }
            oldLine = Line_;


            if (hidden || iTextCanvas.TextEditor.IsInUpdate)
            {
                outstandingUpdate = true;
                return;
            }
            else
            {
                outstandingUpdate = false;
            }
            ValidateCaretPos();
            int lineNr = this.Line_;
            int xpos = iTextCanvas.TextView.GetDrawingXPos(lineNr, this.Column_);
            // OLD_CODE LineSegment lineSegment = textArea.Document.GetLineSegment(lineNr);
            Point pos = GetScreenPosition();
            if (xpos >= 0)
            {
                CreateCaret();
                bool success = caretImplementation.SetPosition(pos.X, pos.Y);
                if (!success)
                {
                    caretImplementation.Destroy();
                    caretCreated = false;
                    UpdateCaretPosition();
                }
            }
            else
            {
                caretImplementation.Destroy();
            }

            // set the input method editor location
            /*if (ime == null)
			{
				ime = new Ime(textArea.Handle, textArea.Document.TextEditorProperties.Font);
			}
			else
			{
				ime.HWnd = textArea.Handle;
				ime.Font = textArea.Document.TextEditorProperties.Font;
			}
			ime.SetIMEWindowLocation(pos.X, pos.Y);*/

            currentPos = pos;
        }

        [Conditional("DEBUG")]
        static void Log(string text)
        {
            // OLD_CODE Console.WriteLine(text);
        }

        #region Caret implementation

        internal void PaintCaret(GContext gc, Color cc = default(Color))
        {
            caretImplementation.PaintCaret(gc, cc);
            PaintCaretLine(gc);
        }

        abstract class CaretImplementation : IDisposable
        {
            public bool RequireRedrawOnPositionChange;

            public abstract bool Create(int width, int height);

            public abstract void Hide();

            public abstract void Show();

            public abstract bool SetPosition(int x, int y);

            public abstract void PaintCaret(GContext gc, Color cc);

            public abstract void Destroy();

            public virtual void Dispose()
            {
                Destroy();
            }
        }
        /*	
		class Win32Caret : CaretImplementation
		{
			[DllImport("User32.dll")]
			static extern bool CreateCaret(IntPtr hWnd, int hBitmap, int nWidth, int nHeight);

			[DllImport("User32.dll")]
			static extern bool SetCaretPos(int x, int y);

			[DllImport("User32.dll")]
			static extern bool DestroyCaret();

			[DllImport("User32.dll")]
			static extern bool ShowCaret(IntPtr hWnd);

			[DllImport("User32.dll")]
			static extern bool HideCaret(IntPtr hWnd);

			TextArea textArea;

			public Win32Caret(Caret caret)
			{
				this.textArea = caret.textArea;
			}

			public override bool Create(int width, int height)
			{
				return CreateCaret(textArea.Handle, 0, width, height);
			}

			public override void Hide()
			{
				HideCaret(textArea.Handle);
			}

			public override void Show()
			{
				ShowCaret(textArea.Handle);
			}

			public override bool SetPosition(int x, int y)
			{
				return SetCaretPos(x, y);
			}

			public override void PaintCaret(Graphics g)
			{
			}

			public override void Destroy()
			{
				DestroyCaret();
			}
		}
*/

        class ManagedCaret : CaretImplementation
        {
            readonly System.Timers.Timer timer = new System.Timers.Timer { Interval = 300 };
            bool visible;
            bool blink = true;
            int x, y, width, height;
            TextCanvas iTextCanvas;
            Caret parentCaret;

            public ManagedCaret(Caret caret)
            {
                base.RequireRedrawOnPositionChange = true;
                this.iTextCanvas = caret.iTextCanvas;
                this.parentCaret = caret;
                timer.Elapsed += CaretTimerTick;
            }

            void CaretTimerTick(object sender, System.Timers.ElapsedEventArgs e)
            {
                blink = !blink;
                if (visible)
                    iTextCanvas.UpdateLine(parentCaret.Line);
            }

            public override bool Create(int width, int height)
            {
                this.visible = true;
                this.width = width - 2;
                this.height = height;
                timer.Enabled = true;
                return true;
            }

            public override void Hide()
            {
                visible = false;
            }

            public override void Show()
            {
                visible = true;
            }

            public override bool SetPosition(int x, int y)
            {
                this.x = x;// - 1;
                this.y = y;
                return true;
            }

            public override void PaintCaret(GContext gc, Color cc = default(Color))
            {
                if (visible && blink)
                {
                    var np = new NanoPen(cc);
                    //np.Width = 2;

                    //gc.DrawRectangle(new NanoPen(TextColor.Gray), x, y, width, height);
                    gc.DrawLine(np, x, y, x, y + height);
                }
            }

            public override void Destroy()
            {
                visible = false;
                timer.Enabled = false;
            }

            public override void Dispose()
            {
                base.Dispose();
                timer.Dispose();
            }
        }

        #endregion

        bool firePositionChangedAfterUpdateEnd;

        void FirePositionChangedAfterUpdateEnd(object sender, EventArgs e)
        {
            OnPositionChanged(EventArgs.Empty);
        }

        protected virtual void OnPositionChanged(EventArgs e)
        {
            if (this.iTextCanvas.TextEditor.IsInUpdate)
            {
                if (firePositionChangedAfterUpdateEnd == false)
                {
                    firePositionChangedAfterUpdateEnd = true;
                    this.iTextCanvas.Document.UpdateCommited += FirePositionChangedAfterUpdateEnd;
                }
                return;
            }
            else if (firePositionChangedAfterUpdateEnd)
            {
                this.iTextCanvas.Document.UpdateCommited -= FirePositionChangedAfterUpdateEnd;
                firePositionChangedAfterUpdateEnd = false;
            }

            List<FoldMarker> foldings = iTextCanvas.Document.FoldingManager.GetFoldingsFromPosition(Line_, Column_);
            bool shouldUpdate = false;
            foreach (FoldMarker foldMarker in foldings)
            {
                shouldUpdate |= foldMarker.IsFolded;
                foldMarker.IsFolded = false;
            }

            if (shouldUpdate)
            {
                iTextCanvas.Document.FoldingManager.NotifyFoldingsChanged(EventArgs.Empty);
            }

            if (PositionChanged != null)
            {
                PositionChanged(this, EventArgs.Empty);
            }
            iTextCanvas.ScrollToCaret();
        }

        protected virtual void OnCaretModeChanged(EventArgs e)
        {
            if (CaretModeChanged != null)
            {
                CaretModeChanged(this, new EventArgs());
            }
            caretImplementation.Hide();
            caretImplementation.Destroy();
            caretCreated = false;
            CreateCaret();
            caretImplementation.Show();
        }

        /// <remarks>
        /// Is called each time the caret is moved.
        /// </remarks>
        public event EventHandler PositionChanged;

        /// <remarks>
        /// Is called each time the CaretMode has changed.
        /// </remarks>
        public event EventHandler CaretModeChanged;
    }
}
