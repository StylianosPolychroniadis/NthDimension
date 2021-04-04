using NthDimension;
using NthDimension.Forms;
using NthDimension.Forms.Delegates;
using NthDimension.Forms.Events;
using NthStudio.Gui.Widgets.TextEditor.Actions;
using NthStudio.Gui.Widgets.TextEditor.Document;
using NthStudio.Gui.Widgets.TextEditor.Document.FoldingStrategy;
using NthStudio.Gui.Widgets.TextEditor.Document.LineManagement;
using NthStudio.Gui.Widgets.TextEditor.Document.Selection;
using NthStudio.Gui.Widgets.TextEditor.Margins;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Gui.Widgets.TextEditor
{
    public class TextCanvas : Widget
    {
        public event KeyEventHandler KeyEventHandler;
        public event DialogKeyProcessor DoProcessDialogKey;

        TextViewMargin iTextView;
        TextEditor iTextEditor;
        TextArea iTextArea;
        GutterMargin gutterMargin;
        FoldMargin foldMargin;
        IconBarMargin iconBarMargin;
        SelectionManager selectionManager;

        List<AbstractMargin> leftMargins = new List<AbstractMargin>();

        public TextCanvas(TextEditor pTextEditor, TextArea pTextArea)
        {
            PaintBackGround = false;
            mouseWheelHandler =
                new Util.MouseWheelHandler( pTextEditor.TextEditorProperties.MouseWheelScrollLines);

            Name = "TextCanvas";
            iTextEditor = pTextEditor;
            iTextArea = pTextArea;
            caret = new Caret(this);
            selectionManager = new SelectionManager(Document, this);

            iTextView = new TextViewMargin(this);

            gutterMargin = new GutterMargin(this);
            foldMargin = new FoldMargin(this);
            iconBarMargin = new IconBarMargin(this);
            leftMargins.AddRange(new AbstractMargin[] { iconBarMargin, gutterMargin, foldMargin });
            OptionsChanged();

            new TextCanvasMouseHandler(this).Attach();
            //new TextAreaDragDropHandler().Attach(this);

            bracketshemes.Add(new BracketHighlightingSheme('{', '}'));
            bracketshemes.Add(new BracketHighlightingSheme('(', ')'));
            bracketshemes.Add(new BracketHighlightingSheme('[', ']'));           

            caret.PositionChanged += SearchMatchingBracket;
            Document.TextContentChanged += TextContentChanged;
            Document.FoldingManager.FoldingsChanged += DocumentFoldingsChanged;
        }

        public SelectionManager SelectionManager
        {
            get
            {
                return selectionManager;
            }
        }

        bool autoClearSelection = false;

        public bool AutoClearSelection
        {
            get
            {
                return autoClearSelection;
            }
            set
            {
                autoClearSelection = value;
            }
        }

        public IList<AbstractMargin> LeftMargins
        {
            get
            {
                return leftMargins.AsReadOnly();
            }
        }

        public GutterMargin GutterMargin
        {
            get
            {
                return gutterMargin;
            }
        }

        Caret caret;

        public Caret Caret
        {
            get
            {
                return caret;
            }
        }

        public ITextEditorProperties TextEditorProperties
        {
            get
            {
                return iTextEditor.TextEditorProperties;
            }
        }

        public TextEditor TextEditor
        {
            get
            {
                return iTextEditor;
            }
        }

        public TextArea TextArea
        {
            get
            {
                return iTextArea;
            }
        }

        public TextViewMargin TextView
        {
            get
            {
                return iTextView;
            }
        }

        public IDocument Document
        {
            get
            {
                return iTextEditor.Document;
            }
        }

        Point virtualTop = new Point(0, 0);

        public Point VirtualTop
        {
            get
            {
                return virtualTop;
            }
            set
            {
                var newVirtualTop = new Point(value.X, Math.Min(MaxVScrollValue, Math.Max(0, value.Y)));
                if (virtualTop != newVirtualTop)
                {
                    virtualTop = newVirtualTop;
                    iTextArea.ScrollBarV.Value = virtualTop.Y;
                    Invalidate();
                }
                //caret.UpdateCaretPosition();
            }
        }

        public int MaxVScrollValue
        {
            get
            {
                return (Document.GetVisibleLine(Document.TotalNumberOfLines - 1) + 1 + iTextView.VisibleLineCount * 2 / 3) * iTextView.LineHeight;
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            //Cursors.Cursor = Cursors.Text;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            //Cursors.Cursor = Cursors.Default;
        }

        public void BeginUpdate()
        {
            iTextEditor.BeginUpdate();
        }

        public void EndUpdate()
        {
            iTextEditor.EndUpdate();
        }

        AbstractMargin lastMouseInMargin;

        // external interface to the attached event
        internal void RaiseMouseMove(MouseEventArgs e)
        {
            OnMouseMove(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            /*if (!toolTipRectangle.Contains(e.Location)) {
				toolTipRectangle = Rectangle.Empty;
				if (toolTipActive)
					RequestToolTip(e.Location);
			}*/

            Point localXY = new Point(e.X, e.Y); // WindowToLocal(e.X, e.Y);

            foreach (AbstractMargin margin in leftMargins)
            {
                if (margin.DrawingPosition.Contains(localXY.X, localXY.Y))
                {
                    Cursors.Cursor = margin.Cursor;

                    margin.HandleMouseMove(new Point(localXY.X, localXY.Y), e.Button);
                    if (lastMouseInMargin != margin)
                    {
                        if (lastMouseInMargin != null)
                        {
                            lastMouseInMargin.HandleMouseLeave(EventArgs.Empty);
                        }
                        lastMouseInMargin = margin;
                    }
                    return;
                }
            }
            if (lastMouseInMargin != null)
            {
                lastMouseInMargin.HandleMouseLeave(EventArgs.Empty);
                lastMouseInMargin = null;
            }

            if (iTextView.DrawingPosition.Contains(localXY.X, localXY.Y))
            {
                TextLocation realmousepos = iTextView.GetLogicalPosition(e.X - iTextView.DrawingPosition.X, e.Y - iTextView.DrawingPosition.Y);
                if (SelectionManager.IsSelected(Document.PositionToOffset(realmousepos))
                    && e.Button == MouseButton.None)
                {
                    // mouse is hovering over a selection, so show default mouse
                    Cursors.Cursor = Cursors.Default;
                }
                else
                {
                    // mouse is hovering over text area, not a selection, so show the textView cursor
                    Cursors.Cursor = iTextView.Cursor;
                }
                return;
            }
            Cursors.Cursor = Cursors.Default;
        }

        AbstractMargin updateMargin = null;

        protected override void DoPaint(PaintEventArgs e)
        {
            int currentXPos = 0;
            int currentYPos = 0;
            bool adjustScrollBars = false;

            GContext gc = e.GC;
            Rectangle clipRectangle = this.ClientRect;

            bool isFullRepaint = clipRectangle.X == 0 && clipRectangle.Y == 0
                                 && clipRectangle.Width == this.Width && clipRectangle.Height == this.Height;

            if (updateMargin != null)
            {
                updateMargin.Paint(gc, updateMargin.DrawingPosition);
                //				clipRectangle.Intersect(updateMargin.DrawingPosition);
            }

            if (clipRectangle.Width <= 0 || clipRectangle.Height <= 0)
            {
                return;
            }

            foreach (AbstractMargin margin in leftMargins)
            {
                if (margin.IsVisible)
                {
                    Rectangle marginRectangle = new Rectangle(currentXPos, currentYPos, margin.Size.Width, Height - currentYPos);
                    if (marginRectangle != margin.DrawingPosition)
                    {
                        // margin changed size
                        if (!isFullRepaint && !clipRectangle.Contains(marginRectangle))
                        {
                            Invalidate(); // do a full repaint
                        }
                        adjustScrollBars = true;
                        margin.DrawingPosition = marginRectangle;
                    }
                    currentXPos += margin.DrawingPosition.Width;
                    if (clipRectangle.IntersectsWith(marginRectangle))
                    {
                        marginRectangle.Intersect(clipRectangle);
                        if (!marginRectangle.IsEmpty)
                        {
                            margin.Paint(gc, marginRectangle);
                        }
                    }
                }
            }

            Rectangle textViewArea = new Rectangle(currentXPos, currentYPos, Width - currentXPos, Height - currentYPos);
            if (textViewArea != iTextView.DrawingPosition)
            {
                adjustScrollBars = true;
                iTextView.DrawingPosition = textViewArea;
                // update caret position (but outside of WM_PAINT!)
                ///*BeginInvoke((MethodInvoker)*/caret.UpdateCaretPosition();
            }
            if (clipRectangle.IntersectsWith(textViewArea))
            {
                textViewArea.Intersect(clipRectangle);
                if (!textViewArea.IsEmpty)
                {
                    iTextView.Paint(gc, textViewArea);
                }
            }

            if (adjustScrollBars)
            {
                iTextArea.AdjustScrollBars();
            }

            // we cannot update the caret position here, it's not allowed to call the caret API inside WM_PAINT
            //Caret.UpdateCaretPosition();

            base.DoPaint(e);
        }

        bool hiddenMouseCursor = false;
        /// <summary>
        /// The position where the mouse cursor was when it was hidden. Sometimes the text editor gets MouseMove
        /// events when typing text even if the mouse is not moved.
        /// </summary>
        Point mouseCursorHidePosition;

        /// <summary>
        /// Shows the mouse cursor if it has been hidden.
        /// </summary>
        /// <param name="forceShow"><c>true</c> to always show the cursor or <c>false</c> to show it only if it has been moved since it was hidden.</param>
        internal void ShowHiddenCursor(bool forceShow)
        {
            if (hiddenMouseCursor)
            {
                if (mouseCursorHidePosition != WHUD.LibContext.CursorPos || forceShow)
                {
                    //Cursor.Show();
                    WHUD.LibContext.ShowCursor = true;
                    hiddenMouseCursor = false;
                }
            }
        }

        internal Point mousepos = new Point(0, 0);

        protected override void OnMouseDown(MouseDownEventArgs e)
        {
            base.OnMouseDown(e);

            Point localPos = new Point(e.X, e.Y);
            mousepos = localPos;

            //CloseToolTip();

            foreach (AbstractMargin margin in leftMargins)
            {
                if (margin.DrawingPosition.Contains(localPos))
                {
                    margin.HandleMouseDown(localPos, e.Button);
                }
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            Repaint();
        }

        Util.MouseWheelHandler mouseWheelHandler;

        int prevDelta = 0;
        const int WHEEL_DELTA = 2;

        int mouseWheelDelta;
        int mouseWheelScrollLines = 1;
        int GetScrollAmount(MouseEventArgs e, out int md)
        {
            // accumulate the delta to support high-resolution mice
            mouseWheelDelta += e.DeltaWheel;
            md = mouseWheelDelta;

            //int linesPerClick = Math.Max(SystemInformation.MouseWheelScrollLines, 1);
            int linesPerClick = Math.Max(mouseWheelScrollLines, 3);

            int scrollDistance = mouseWheelDelta * linesPerClick / WHEEL_DELTA;
            mouseWheelDelta %= Math.Max(1, WHEEL_DELTA / linesPerClick);
            return scrollDistance;
        }
        public override void OnMouseWheel(MouseEventArgs e)
        {
            //base.OnMouseWheel(e);

            //if (Focus())
            {
                if (e.DeltaWheel </*=*/ 0)
                {
                    //RDArrowState = EButtonState.Pressed;
                    int move = 0;
                    GetScrollAmount(e, out move);
                    //iTextArea.ScrollBarV.IncrementManual(move /*3*/);
                    iTextArea.ScrollBarV.DecrementManual(move /*3*/);
                }
                else
                {
                    //LUArrowState = EButtonState.Pressed;
                    int move = 0;
                    GetScrollAmount(e, out move);
                    //iTextArea.ScrollBarV.DecrementManual(move /*3*/);
                    iTextArea.ScrollBarV.IncrementManual(move /*3*/);
                }
                prevDelta = e.DeltaWheel;
                caret.UpdateCaretPosition();
            }
            /*int md;
			int scrollDistance = mouseWheelHandler.GetScrollAmount(e, out md);

			TopLevelWindow.Title = "md = " + md.ToString();

			if (scrollDistance == 0)
				return;
			if ((Control.ModifierKeys & Keys.Control) != 0 
			    && TextEditorProperties.MouseWheelTextZoom)
			{
				if (scrollDistance > 0)
				{
					iTextEditor.Font = new Font(iTextEditor.Font.Name,
					                                        iTextEditor.Font.Size + 1);
				}
				else
				{
					iTextEditor.Font = new Font(iTextEditor.Font.Name,
					                                        Math.Max(6, iTextEditor.Font.Size - 1));
				}
			}
			else
			{
				if (TextEditorProperties.MouseWheelScrollDown)
					scrollDistance = -scrollDistance;
				int newValue = iTextArea.ScrollBarV.Value + 
					iTextArea.ScrollBarV.SmallChange * scrollDistance;
				iTextArea.ScrollBarV.Value = Math.Max(iTextArea.ScrollBarV.Minimum, 
				                                      Math.Min(iTextArea.ScrollBarV.Maximum 
				                                               - iTextArea.ScrollBarV.LargeChange + 1, 
				                                               newValue));
			}
			*/
        }

        #region UPDATE Commands

        internal void UpdateLine(int line)
        {
            UpdateLines(0, line, line);
        }

        internal void UpdateLines(int lineBegin, int lineEnd)
        {
            UpdateLines(0, lineBegin, lineEnd);
        }

        internal void UpdateToEnd(int lineBegin)
        {
            //			if (lineBegin > FirstPhysicalLine + iTextView.VisibleLineCount) {
            //				return;
            //			}

            lineBegin = Document.GetVisibleLine(lineBegin);
            int y = Math.Max(0, (int)(lineBegin * iTextView.LineHeight));
            y = Math.Max(0, y - this.virtualTop.Y);
            Rectangle r = new Rectangle(0,
                                        y,
                                        Width,
                                        Height - y);
            Invalidate(r);
        }

        internal void UpdateLineToEnd(int lineNr, int xStart)
        {
            UpdateLines(xStart, lineNr, lineNr);
        }

        internal void UpdateLine(int line, int begin, int end)
        {
            UpdateLines(line, line);
        }

        int FirstPhysicalLine
        {
            get
            {
                return VirtualTop.Y / iTextView.LineHeight;
            }
        }

        internal void UpdateLines(int xPos, int lineBegin, int lineEnd)
        {
            //			if (lineEnd < FirstPhysicalLine || lineBegin > FirstPhysicalLine + iTextView.VisibleLineCount) {
            //				return;
            //			}

            InvalidateLines((int)(xPos * this.iTextView.WideSpaceWidth), lineBegin, lineEnd);
        }

        void InvalidateLines(int xPos, int lineBegin, int lineEnd)
        {
            lineBegin = Math.Max(Document.GetVisibleLine(lineBegin), FirstPhysicalLine);
            lineEnd = Math.Min(Document.GetVisibleLine(lineEnd), FirstPhysicalLine + iTextView.VisibleLineCount);
            int y = Math.Max(0, (int)(lineBegin * iTextView.LineHeight));
            int height = Math.Min(iTextView.DrawingPosition.Height, (int)((1 + lineEnd - lineBegin) * (iTextView.LineHeight + 1)));

            Rectangle r = new Rectangle(0,
                                        y - 1 - this.virtualTop.Y,
                                        Width,
                                        height + 3);

            Invalidate(r);
        }

        #endregion

        public void ScrollToCaret()
        {
            iTextArea.ScrollToCaret();
        }

        public void SetCaretToDesiredColumn()
        {
            FoldMarker dummy;
            Caret.Position = iTextView.GetLogicalColumn(Caret.Line, Caret.DesiredColumn + VirtualTop.X, out dummy);
        }

        public void SetDesiredColumn()
        {
            Caret.DesiredColumn = iTextView.GetDrawingXPos(Caret.Line, Caret.Column) + VirtualTop.X;
        }

        void DocumentFoldingsChanged(object sender, EventArgs e)
        {
            Caret.UpdateCaretPosition();
            Invalidate();
            this.iTextArea.AdjustScrollBars();
        }

        public void OptionsChanged()
        {
            UpdateMatchingBracket();
            iTextView.OptionsChanged();
            caret.RecreateCaret();
            caret.UpdateCaretPosition();
            Repaint();
        }

        void TextContentChanged(object sender, EventArgs e)
        {
            Caret.Position = new TextLocation(0, 0);
            SelectionManager.SelectionCollection.Clear();
        }

        List<BracketHighlightingSheme> bracketshemes = new List<BracketHighlightingSheme>();

        public Highlight FindMatchingBracketHighlight()
        {
            if (Caret.Offset == 0)
                return null;
            foreach (BracketHighlightingSheme bracketsheme in bracketshemes)
            {
                Highlight highlight = bracketsheme.GetHighlight(Document, Caret.Offset - 1);
                if (highlight != null)
                {
                    return highlight;
                }
            }
            return null;
        }

        public void UpdateMatchingBracket()
        {
            SearchMatchingBracket(null, null);
        }

        void SearchMatchingBracket(object sender, EventArgs e)
        {
            if (!TextEditorProperties.ShowMatchingBracket)
            {
                iTextView.Highlight = null;
                return;
            }
            int oldLine1 = -1, oldLine2 = -1;

            if (iTextView.Highlight != null
                && iTextView.Highlight.OpenBrace.Y >= 0
                && iTextView.Highlight.OpenBrace.Y < Document.TotalNumberOfLines)
            {
                oldLine1 = iTextView.Highlight.OpenBrace.Y;
            }
            if (iTextView.Highlight != null
                && iTextView.Highlight.CloseBrace.Y >= 0
                && iTextView.Highlight.CloseBrace.Y < Document.TotalNumberOfLines)
            {
                oldLine2 = iTextView.Highlight.CloseBrace.Y;
            }
            iTextView.Highlight = FindMatchingBracketHighlight();

            if (oldLine1 >= 0)
                UpdateLine(oldLine1);

            if (oldLine2 >= 0 && oldLine2 != oldLine1)
                UpdateLine(oldLine2);

            if (iTextView.Highlight != null)
            {
                int newLine1 = iTextView.Highlight.OpenBrace.Y;
                int newLine2 = iTextView.Highlight.CloseBrace.Y;
                if (newLine1 != oldLine1 && newLine1 != oldLine2)
                    UpdateLine(newLine1);
                if (newLine2 != oldLine1 && newLine2 != oldLine2 && newLine2 != newLine1)
                    UpdateLine(newLine2);
            }
        }

        #region keyboard handling methods

        /// <summary>
        /// This method is called on each Keypress
        /// </summary>
        /// <returns>
        /// True, if the key is handled by this method and should NOT be
        /// inserted in the textarea.
        /// </returns>
        protected internal virtual bool HandleKeyPress(char ch)
        {
            if (KeyEventHandler != null)
            {
                return KeyEventHandler(ch);
            }
            return false;
        }

        // Fixes SD2-747: Form containing the text editor and a button with a shortcut
        protected bool IsInputChar(char charCode)
        {
            return true;
        }

        internal bool IsReadOnly(int offset)
        {
            if (Document.ReadOnly)
            {
                return true;
            }
            if (TextEditorProperties.SupportReadOnlySegments)
            {
                return Document.MarkerStrategy.GetMarkers(offset).Exists(m => m.IsReadOnly);
            }
            else
            {
                return false;
            }
        }

        internal bool IsReadOnly(int offset, int length)
        {
            if (Document.ReadOnly)
            {
                return true;
            }
            if (TextEditorProperties.SupportReadOnlySegments)
            {
                return Document.MarkerStrategy.GetMarkers(offset, length).Exists(m => m.IsReadOnly);
            }
            else
            {
                return false;
            }
        }

        public void SimulateKeyPress(char ch)
        {
            if (SelectionManager.HasSomethingSelected)
            {
                if (SelectionManager.SelectionIsReadonly)
                    return;
            }
            else if (IsReadOnly(Caret.Offset))
            {
                return;
            }

            if (ch < ' ')
            {
                return;
            }

            if (!hiddenMouseCursor && TextEditorProperties.HideMouseCursor)
            {
                if (this.ClientRect.Contains(WHUD.LibContext.CursorPos))
                {
                    mouseCursorHidePosition = WHUD.LibContext.CursorPos;
                    hiddenMouseCursor = true;
                    //Cursor.Hide();
                    WHUD.LibContext.ShowCursor = false;
                }
            }
            //CloseToolTip();

            BeginUpdate();
            Document.UndoStack.StartUndoGroup();
            try
            {
                // INSERT char
                if (!HandleKeyPress(ch))
                {
                    switch (Caret.CaretMode)
                    {
                        case CaretMode.InsertMode:
                            InsertChar(ch);
                            break;
                        case CaretMode.OverwriteMode:
                            ReplaceChar(ch);
                            break;
                        default:
                            System.Diagnostics.Debug.Assert(false, "Unknown caret mode " + Caret.CaretMode);
                            break;
                    }
                }

                int currentLineNr = Caret.Line;
                Document.FormattingStrategy.FormatLine(this, currentLineNr, Document.PositionToOffset(Caret.Position), ch);

                EndUpdate();
            }
            finally
            {
                Document.UndoStack.EndUndoGroup();
            }
        }

        protected override void OnKeyPress(KeyPressedEventArgs e)
        {
            base.OnKeyPress(e);
            SimulateKeyPress(e.KeyChar);
            e.Handled = true;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            return ExecuteDialogKey(keyData) || base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// This method executes a dialog key
        /// </summary>
        public bool ExecuteDialogKey(Keys keyData)
        {
            // try, if a dialog key processor was set to use this
            if (DoProcessDialogKey != null && DoProcessDialogKey(keyData))
            {
                return true;
            }

            // if not (or the process was 'silent', use the standard edit actions
            IEditAction action = iTextEditor.GetEditAction(keyData);
            AutoClearSelection = true;
            if (action != null)
            {
                BeginUpdate();
                try
                {
                    lock (Document)
                    {
                        action.Execute(this);
                        if (SelectionManager.HasSomethingSelected && AutoClearSelection /*&& caretchanged*/)
                        {
                            if (Document.TextEditorProperties.DocumentSelectionMode == DocumentSelectionMode.Normal)
                            {
                                SelectionManager.ClearSelection();
                            }
                        }
                    }
                }
                finally
                {
                    EndUpdate();
                    Caret.UpdateCaretPosition();
                }
                return true;
            }
            return false;
        }

        #endregion

        #region Insert-replace-string-chars

        string GenerateWhitespaceString(int length)
        {
            return new String(' ', length);
        }

        /// <remarks>
        /// Inserts a single character at the caret position
        /// </remarks>
        public void InsertChar(char ch)
        {
            bool updating = iTextEditor.IsInUpdate;
            if (!updating)
            {
                BeginUpdate();
            }

            // filter out forgein whitespace chars and replace them with standard space (ASCII 32)
            if (Char.IsWhiteSpace(ch) && ch != '\t' && ch != '\n')
            {
                ch = ' ';
            }

            Document.UndoStack.StartUndoGroup();
            if (Document.TextEditorProperties.DocumentSelectionMode == DocumentSelectionMode.Normal &&
                SelectionManager.SelectionCollection.Count > 0)
            {
                Caret.Position = SelectionManager.SelectionCollection[0].StartPosition;
                SelectionManager.RemoveSelectedText();
            }
            LineSegment caretLine = Document.GetLineSegment(Caret.Line);
            int offset = Caret.Offset;
            // use desired column for generated whitespaces
            int dc = Caret.Column;
            if (caretLine.Length < dc && ch != '\n')
            {
                Document.Insert(offset, GenerateWhitespaceString(dc - caretLine.Length) + ch);
            }
            else
            {
                Document.Insert(offset, ch.ToString());
            }
            Document.UndoStack.EndUndoGroup();
            ++Caret.Column;

            if (!updating)
            {
                EndUpdate();
                UpdateLineToEnd(Caret.Line, Caret.Column);
            }

            // I prefer to set NOT the standard column, if you type something
            //			++Caret.DesiredColumn;
        }

        /// <remarks>
        /// Inserts a whole string at the caret position
        /// </remarks>
        public void InsertString(string str)
        {
            bool updating = iTextEditor.IsInUpdate;
            if (!updating)
            {
                BeginUpdate();
            }
            try
            {
                Document.UndoStack.StartUndoGroup();
                if (Document.TextEditorProperties.DocumentSelectionMode == DocumentSelectionMode.Normal &&
                    SelectionManager.SelectionCollection.Count > 0)
                {
                    Caret.Position = SelectionManager.SelectionCollection[0].StartPosition;
                    SelectionManager.RemoveSelectedText();
                }

                int oldOffset = Document.PositionToOffset(Caret.Position);
                int oldLine = Caret.Line;
                LineSegment caretLine = Document.GetLineSegment(Caret.Line);
                if (caretLine.Length < Caret.Column)
                {
                    int whiteSpaceLength = Caret.Column - caretLine.Length;
                    Document.Insert(oldOffset, GenerateWhitespaceString(whiteSpaceLength) + str);
                    Caret.Position = Document.OffsetToPosition(oldOffset + str.Length + whiteSpaceLength);
                }
                else
                {
                    Document.Insert(oldOffset, str);
                    Caret.Position = Document.OffsetToPosition(oldOffset + str.Length);
                }
                Document.UndoStack.EndUndoGroup();
                if (oldLine != Caret.Line)
                {
                    UpdateToEnd(oldLine);
                }
                else
                {
                    UpdateLineToEnd(Caret.Line, Caret.Column);
                }
            }
            finally
            {
                if (!updating)
                {
                    EndUpdate();
                }
            }
        }

        /// <remarks>
        /// Replaces a char at the caret position
        /// </remarks>
        public void ReplaceChar(char ch)
        {
            bool updating = iTextEditor.IsInUpdate;
            if (!updating)
            {
                BeginUpdate();
            }
            if (Document.TextEditorProperties.DocumentSelectionMode == DocumentSelectionMode.Normal && SelectionManager.SelectionCollection.Count > 0)
            {
                Caret.Position = SelectionManager.SelectionCollection[0].StartPosition;
                SelectionManager.RemoveSelectedText();
            }

            int lineNr = Caret.Line;
            LineSegment line = Document.GetLineSegment(lineNr);
            int offset = Document.PositionToOffset(Caret.Position);
            if (offset < line.Offset + line.Length)
            {
                Document.Replace(offset, 1, ch.ToString());
            }
            else
            {
                Document.Insert(offset, ch.ToString());
            }
            if (!updating)
            {
                EndUpdate();
                UpdateLineToEnd(lineNr, Caret.Column);
            }
            ++Caret.Column;
            //			++Caret.DesiredColumn;
        }

        #endregion Insert-replace-string-chars

        public void Refresh(AbstractMargin margin)
        {
            updateMargin = margin;
            Invalidate(updateMargin.DrawingPosition);
            //Update();
            updateMargin = null;
        }
    }
}
