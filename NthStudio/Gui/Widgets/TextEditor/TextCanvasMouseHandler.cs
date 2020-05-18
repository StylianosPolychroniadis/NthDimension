using NthDimension;
using NthDimension.Forms;
using NthDimension.Forms.Events;
using NthStudio.Gui.Widgets.TextEditor.Document;
using NthStudio.Gui.Widgets.TextEditor.Document.FoldingStrategy;
using NthStudio.Gui.Widgets.TextEditor.Document.LineManagement;
using NthStudio.Gui.Widgets.TextEditor.Document.Selection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NthDimension.Forms.Widget;

namespace NthStudio.Gui.Widgets.TextEditor
{
    /// <summary>
    /// This class handles all mouse stuff for a textArea.
    /// </summary>
    public class TextCanvasMouseHandler
    {
        TextCanvas iTextCanvas;
        bool doubleclick = false;
        bool clickedOnSelectedText = false;

        MouseButton button;

        static readonly Point nilPoint = new Point(-1, -1);
        Point mousedownpos = nilPoint;
        Point lastmousedownpos = nilPoint;

        bool gotmousedown = false;
        bool dodragdrop = false;

        TextLocation minSelection = TextLocation.Empty;
        TextLocation maxSelection = TextLocation.Empty;

        public TextCanvasMouseHandler(TextCanvas pTextCanvas)
        {
            iTextCanvas = pTextCanvas;
        }

        public void Attach()
        {
            iTextCanvas.MouseClickEvent += TextAreaClick; //new EventHandler(TextAreaClick);
            iTextCanvas.MouseDownEvent += OnMouseDown; //new MouseEventHandler(OnMouseDown);
            iTextCanvas.MouseMoveEvent += TextAreaMouseMove; // new MouseEventHandler(TextAreaMouseMove);
            iTextCanvas.MouseUpEvent += OnMouseUp; //new MouseEventHandler(OnMouseUp);
            iTextCanvas.MouseLeaveEvent += OnMouseLeave; //new EventHandler(OnMouseLeave);

            iTextCanvas.MouseDoubleClickEvent += OnDoubleClick;
            //iTextCanvas.LostFocus += new EventHandler(TextAreaLostFocus);
            //iTextCanvas.ToolTipRequest += new ToolTipRequestEventHandler(OnToolTipRequest);
        }

        void ShowHiddenCursorIfMovedOrLeft()
        {
            iTextCanvas.ShowHiddenCursor(!iTextCanvas.IsFocused
                                         ||
                                         !iTextCanvas.ClientRect.Contains(
                                             WHUD.LibContext.CursorPos));
        }

        void OnMouseLeave(object sender, EventArgs e)
        {
            ShowHiddenCursorIfMovedOrLeft();
            gotmousedown = false;
            mousedownpos = nilPoint;
        }

        void TextAreaClick(object sender, EventArgs e)
        {
            Point mousepos;
            mousepos = iTextCanvas.mousepos;

            if (dodragdrop)
            {
                return;
            }

            if (clickedOnSelectedText
                && iTextCanvas.TextView.DrawingPosition.Contains(mousepos.X, mousepos.Y))
            {
                iTextCanvas.SelectionManager.ClearSelection();

                TextLocation clickPosition = iTextCanvas.TextView.GetLogicalPosition(
                    mousepos.X - iTextCanvas.TextView.DrawingPosition.X,
                    mousepos.Y - iTextCanvas.TextView.DrawingPosition.Y);
                iTextCanvas.Caret.Position = clickPosition;
                iTextCanvas.SetDesiredColumn();
            }
        }

        void OnMouseUp(object sender, MouseEventArgs e)
        {
            iTextCanvas.SelectionManager.selectFrom.where = WhereFrom.None;
            gotmousedown = false;
            mousedownpos = nilPoint;
        }

        void OnMouseDown(Widget sender, MouseDownEventArgs e)
        {
            Point mousepos;
            iTextCanvas.mousepos = e.Location;
            mousepos = e.Location;

            if (dodragdrop)
            {
                return;
            }

            if (doubleclick)
            {
                doubleclick = false;
                return;
            }

            if (iTextCanvas.TextView.DrawingPosition.Contains(mousepos.X, mousepos.Y))
            {
                gotmousedown = true;
                button = e.Button;

                iTextCanvas.SelectionManager.selectFrom.where = WhereFrom.TArea;

                // double-click
                if (button == MouseButton.Left && e.Clicks == 2)
                {
                    int deltaX = Math.Abs(lastmousedownpos.X - e.X);
                    int deltaY = Math.Abs(lastmousedownpos.Y - e.Y);
                    if (deltaX <= SystemInformation.DoubleClickSize.Width &&
                        deltaY <= SystemInformation.DoubleClickSize.Height)
                    {
                        DoubleClickSelectionExtend();
                        lastmousedownpos = new Point(e.X, e.Y);

                        if (iTextCanvas.SelectionManager.selectFrom.where == WhereFrom.Gutter)
                        {
                            if (!minSelection.IsEmpty && !maxSelection.IsEmpty && iTextCanvas.SelectionManager.SelectionCollection.Count > 0)
                            {
                                iTextCanvas.SelectionManager.SelectionCollection[0].StartPosition = minSelection;
                                iTextCanvas.SelectionManager.SelectionCollection[0].EndPosition = maxSelection;
                                iTextCanvas.SelectionManager.SelectionStart = minSelection;

                                minSelection = TextLocation.Empty;
                                maxSelection = TextLocation.Empty;
                            }
                        }
                        return;
                    }
                }

                minSelection = TextLocation.Empty;
                maxSelection = TextLocation.Empty;

                lastmousedownpos = mousedownpos = new Point(e.X, e.Y);

                if (button == MouseButton.Left)
                {
                    FoldMarker marker =
                        iTextCanvas.TextView.GetFoldMarkerFromPosition(
                            mousepos.X - iTextCanvas.TextView.DrawingPosition.X,
                            mousepos.Y - iTextCanvas.TextView.DrawingPosition.Y);


                    if (marker != null && marker.IsFolded)
                    {
                        if (iTextCanvas.SelectionManager.HasSomethingSelected)
                        {
                            clickedOnSelectedText = true;
                        }

                        var startLocation = new TextLocation(marker.StartColumn, marker.StartLine);
                        var endLocation = new TextLocation(marker.EndColumn, marker.EndLine);
                        iTextCanvas.SelectionManager.SetSelection(
                            new DefaultSelection(iTextCanvas.TextView.Document,
                                                 startLocation, endLocation));
                        iTextCanvas.Caret.Position = startLocation;
                        iTextCanvas.SetDesiredColumn();
                        iTextCanvas.Focus();
                        return;
                    }

                    if ((Widget.ModifierKeys & Keys.Shift) == Keys.Shift)
                    {
                        ExtendSelectionToMouse();
                    }
                    else
                    {
                        TextLocation realmousepos =
                            iTextCanvas.TextView.GetLogicalPosition(
                                mousepos.X - iTextCanvas.TextView.DrawingPosition.X,
                                mousepos.Y - iTextCanvas.TextView.DrawingPosition.Y);
                        clickedOnSelectedText = false;

                        int offset = iTextCanvas.Document.PositionToOffset(realmousepos);

                        if (iTextCanvas.SelectionManager.HasSomethingSelected &&
                            iTextCanvas.SelectionManager.IsSelected(offset))
                        {
                            clickedOnSelectedText = true;
                        }
                        else
                        {
                            iTextCanvas.SelectionManager.ClearSelection();
                            if (mousepos.Y > 0 && mousepos.Y < iTextCanvas.TextView.DrawingPosition.Height)
                            {
                                var pos = new TextLocation();
                                pos.Y = Math.Min(iTextCanvas.Document.TotalNumberOfLines - 1, realmousepos.Y);
                                pos.X = realmousepos.X;
                                iTextCanvas.Caret.Position = pos;
                                iTextCanvas.SetDesiredColumn();
                            }
                        }
                    }
                }
                else if (button == MouseButton.Right)
                {
                    // Rightclick sets the cursor to the click position unless
                    // the previous selection was clicked
                    TextLocation realmousepos = iTextCanvas.TextView.GetLogicalPosition(mousepos.X - iTextCanvas.TextView.DrawingPosition.X, mousepos.Y - iTextCanvas.TextView.DrawingPosition.Y);
                    int offset = iTextCanvas.Document.PositionToOffset(realmousepos);
                    if (!iTextCanvas.SelectionManager.HasSomethingSelected ||
                        !iTextCanvas.SelectionManager.IsSelected(offset))
                    {
                        iTextCanvas.SelectionManager.ClearSelection();
                        if (mousepos.Y > 0 && mousepos.Y < iTextCanvas.TextView.DrawingPosition.Height)
                        {
                            var pos = new TextLocation();
                            pos.Y = Math.Min(iTextCanvas.Document.TotalNumberOfLines - 1, realmousepos.Y);
                            pos.X = realmousepos.X;
                            iTextCanvas.Caret.Position = pos;
                            iTextCanvas.SetDesiredColumn();
                        }
                    }
                }
            }
            iTextCanvas.Focus();
        }

        void TextAreaMouseMove(object sender, MouseEventArgs e)
        {
            iTextCanvas.mousepos = e.Location;

            // honour the starting selection strategy
            switch (iTextCanvas.SelectionManager.selectFrom.where)
            {
                case WhereFrom.Gutter:
                    ExtendSelectionToMouse();
                    return;

                case WhereFrom.TArea:
                    break;

            }
            iTextCanvas.ShowHiddenCursor(false);
            if (dodragdrop)
            {
                dodragdrop = false;
                return;
            }

            doubleclick = false;
            iTextCanvas.mousepos = new Point(e.X, e.Y);

            if (clickedOnSelectedText)
            {
                /*if (Math.Abs(mousedownpos.X - e.X) >= SystemInformation.DragSize.Width / 2 ||
				    Math.Abs(mousedownpos.Y - e.Y) >= SystemInformation.DragSize.Height / 2)
				{
					clickedOnSelectedText = false;
					ISelection selection = iTextCanvas.SelectionManager.GetSelectionAt(iTextCanvas.Caret.Offset);
					if (selection != null)
					{
						string text = selection.SelectedText;
						bool isReadOnly = SelectionManager.SelectionIsReadOnly(iTextCanvas.Document, selection);
						if (text != null && text.Length > 0)
						{
							DataObject dataObject = new DataObject();
							dataObject.SetData(DataFormats.UnicodeText, true, text);
							dataObject.SetData(selection);
							dodragdrop = true;
							iTextCanvas.DoDragDrop(dataObject, isReadOnly ? DragDropEffects.All & ~DragDropEffects.Move : DragDropEffects.All);
						}
					}
				}*/

                return;
            }

            if (e.Button == MouseButton.Left)
            {
                if (gotmousedown && iTextCanvas.SelectionManager.selectFrom.where == WhereFrom.TArea)
                {
                    ExtendSelectionToMouse();
                }
            }
        }

        bool IsSelectableChar(char ch)
        {
            return Char.IsLetterOrDigit(ch) || ch == '_';
        }

        void ExtendSelectionToMouse()
        {
            Point mousepos;
            mousepos = iTextCanvas.mousepos;
            TextLocation realmousepos = iTextCanvas.TextView.GetLogicalPosition(
                Math.Max(0, mousepos.X - iTextCanvas.TextView.DrawingPosition.X),
                mousepos.Y - iTextCanvas.TextView.DrawingPosition.Y);
            int y = realmousepos.Y;
            realmousepos = iTextCanvas.Caret.ValidatePosition(realmousepos);
            TextLocation oldPos = iTextCanvas.Caret.Position;
            if (oldPos == realmousepos && iTextCanvas.SelectionManager.selectFrom.where != WhereFrom.Gutter)
            {
                return;
            }

            // the selection is from the gutter
            if (iTextCanvas.SelectionManager.selectFrom.where == WhereFrom.Gutter)
            {
                if (realmousepos.Y < iTextCanvas.SelectionManager.SelectionStart.Y)
                {
                    // the selection has moved above the startpoint
                    iTextCanvas.Caret.Position = new TextLocation(0, realmousepos.Y);
                }
                else
                {
                    // the selection has moved below the startpoint
                    iTextCanvas.Caret.Position = iTextCanvas.SelectionManager.NextValidPosition(realmousepos.Y);
                }
            }
            else
            {
                iTextCanvas.Caret.Position = realmousepos;
            }

            // moves selection across whole words for double-click initiated selection
            if (!minSelection.IsEmpty && iTextCanvas.SelectionManager.SelectionCollection.Count > 0 && iTextCanvas.SelectionManager.selectFrom.where == WhereFrom.TArea)
            {
                // Extend selection when selection was started with double-click
                ISelection selection = iTextCanvas.SelectionManager.SelectionCollection[0];
                TextLocation min = iTextCanvas.SelectionManager.GreaterEqPos(minSelection, maxSelection) ? maxSelection : minSelection;
                TextLocation max = iTextCanvas.SelectionManager.GreaterEqPos(minSelection, maxSelection) ? minSelection : maxSelection;
                if (iTextCanvas.SelectionManager.GreaterEqPos(max, realmousepos) && iTextCanvas.SelectionManager.GreaterEqPos(realmousepos, min))
                {
                    iTextCanvas.SelectionManager.SetSelection(min, max);
                }
                else if (iTextCanvas.SelectionManager.GreaterEqPos(max, realmousepos))
                {
                    int moff = iTextCanvas.Document.PositionToOffset(realmousepos);
                    min = iTextCanvas.Document.OffsetToPosition(FindWordStart(iTextCanvas.Document, moff));
                    iTextCanvas.SelectionManager.SetSelection(min, max);
                }
                else
                {
                    int moff = iTextCanvas.Document.PositionToOffset(realmousepos);
                    max = iTextCanvas.Document.OffsetToPosition(FindWordEnd(iTextCanvas.Document, moff));
                    iTextCanvas.SelectionManager.SetSelection(min, max);
                }
            }
            else
            {
                iTextCanvas.SelectionManager.ExtendSelection(oldPos, iTextCanvas.Caret.Position);
            }
            iTextCanvas.SetDesiredColumn();
        }

        int FindWordStart(IDocument document, int offset)
        {
            LineSegment line = document.GetLineSegmentForOffset(offset);

            if (offset > 0 && Char.IsWhiteSpace(document.GetCharAt(offset - 1)) && Char.IsWhiteSpace(document.GetCharAt(offset)))
            {
                while (offset > line.Offset && Char.IsWhiteSpace(document.GetCharAt(offset - 1)))
                {
                    --offset;
                }
            }
            else if (IsSelectableChar(document.GetCharAt(offset)) || (offset > 0 && Char.IsWhiteSpace(document.GetCharAt(offset)) && IsSelectableChar(document.GetCharAt(offset - 1))))
            {
                while (offset > line.Offset && IsSelectableChar(document.GetCharAt(offset - 1)))
                {
                    --offset;
                }
            }
            else
            {
                if (offset > 0 && !Char.IsWhiteSpace(document.GetCharAt(offset - 1)) && !IsSelectableChar(document.GetCharAt(offset - 1)))
                {
                    return Math.Max(0, offset - 1);
                }
            }
            return offset;
        }

        int FindWordEnd(IDocument document, int offset)
        {
            LineSegment line = document.GetLineSegmentForOffset(offset);
            if (line.Length == 0)
                return offset;
            int endPos = line.Offset + line.Length;
            offset = Math.Min(offset, endPos - 1);

            if (IsSelectableChar(document.GetCharAt(offset)))
            {
                while (offset < endPos && IsSelectableChar(document.GetCharAt(offset)))
                {
                    ++offset;
                }
            }
            else if (Char.IsWhiteSpace(document.GetCharAt(offset)))
            {
                if (offset > 0 && Char.IsWhiteSpace(document.GetCharAt(offset - 1)))
                {
                    while (offset < endPos && Char.IsWhiteSpace(document.GetCharAt(offset)))
                    {
                        ++offset;
                    }
                }
            }
            else
            {
                return Math.Max(0, offset + 1);
            }

            return offset;
        }


        /*
		void OnToolTipRequest(object sender, ToolTipRequestEventArgs e)
		{
			if (e.ToolTipShown)
				return;
			Point mousepos = e.MousePosition;
			FoldMarker marker = iTextCanvas.TextView.GetFoldMarkerFromPosition(mousepos.X - iTextCanvas.TextView.DrawingPosition.X,
			                                                                mousepos.Y - iTextCanvas.TextView.DrawingPosition.Y);
			if (marker != null && marker.IsFolded)
			{
				StringBuilder sb = new StringBuilder(marker.InnerText);
				
				// max 10 lines
				int endLines = 0;
				for (int i = 0; i < sb.Length; ++i)
				{
					if (sb[i] == '\n')
					{
						++endLines;
						if (endLines >= 10)
						{
							sb.Remove(i + 1, sb.Length - i - 1);
							sb.Append(Environment.NewLine);
							sb.Append("...");
							break;
							
						}
					}
				}
				sb.Replace("\t", "    ");
				e.ShowToolTip(sb.ToString());
				return;
			}
			
			List<TextMarker> markers = iTextCanvas.Document.MarkerStrategy.GetMarkers(e.LogicalPosition);
			foreach (TextMarker tm in markers)
			{
				if (tm.ToolTip != null)
				{
					e.ShowToolTip(tm.ToolTip.Replace("\t", "    "));
					return;
				}
			}
		}

		void TextAreaLostFocus(object sender, EventArgs e)
		{
			// The call to ShowHiddenCursorIfMovedOrLeft is delayed
			// until pending messages have been processed
			// so that it can properly detect whether the TextArea
			// has really lost focus.
			// For example, the CodeCompletionWindow gets focus when it is shown,
			// but immediately gives back focus to the TextArea.
			iTextCanvas.BeginInvoke(new MethodInvoker(ShowHiddenCursorIfMovedOrLeft));
		}
		 */
        void DoubleClickSelectionExtend()
        {
            Point mousepos;
            mousepos = iTextCanvas.mousepos;

            iTextCanvas.SelectionManager.ClearSelection();
            if (iTextCanvas.TextView.DrawingPosition.Contains(mousepos.X, mousepos.Y))
            {
                FoldMarker marker = iTextCanvas.TextView.GetFoldMarkerFromPosition(mousepos.X - iTextCanvas.TextView.DrawingPosition.X,
                                                                                   mousepos.Y - iTextCanvas.TextView.DrawingPosition.Y);
                if (marker != null && marker.IsFolded)
                {
                    marker.IsFolded = false;
                    iTextCanvas.TextArea.AdjustScrollBars();
                }
                if (iTextCanvas.Caret.Offset < iTextCanvas.Document.TextLength)
                {
                    switch (iTextCanvas.Document.GetCharAt(iTextCanvas.Caret.Offset))
                    {
                        case '"':
                            if (iTextCanvas.Caret.Offset < iTextCanvas.Document.TextLength)
                            {
                                int next = FindNext(iTextCanvas.Document, iTextCanvas.Caret.Offset + 1, '"');
                                minSelection = iTextCanvas.Caret.Position;
                                if (next > iTextCanvas.Caret.Offset && next < iTextCanvas.Document.TextLength)
                                    next += 1;
                                maxSelection = iTextCanvas.Document.OffsetToPosition(next);
                            }
                            break;
                        default:
                            minSelection = iTextCanvas.Document.OffsetToPosition(FindWordStart(iTextCanvas.Document, iTextCanvas.Caret.Offset));
                            maxSelection = iTextCanvas.Document.OffsetToPosition(FindWordEnd(iTextCanvas.Document, iTextCanvas.Caret.Offset));
                            break;

                    }
                    iTextCanvas.Caret.Position = maxSelection;
                    iTextCanvas.SelectionManager.ExtendSelection(minSelection, maxSelection);
                }

                if (iTextCanvas.SelectionManager.selectionCollection.Count > 0)
                {
                    ISelection selection = iTextCanvas.SelectionManager.selectionCollection[0];

                    selection.StartPosition = minSelection;
                    selection.EndPosition = maxSelection;
                    iTextCanvas.SelectionManager.SelectionStart = minSelection;
                }

                // after a double-click selection, the caret is placed correctly,
                // but it is not positioned internally.  The effect is when the cursor
                // is moved up or down a line, the caret will take on the column first
                // clicked on for the double-click
                iTextCanvas.SetDesiredColumn();

                // HACK WARNING !!!
                // must refresh here, because when a error tooltip is showed and the underlined
                // code is double clicked the textArea don't update corrctly, updateline doesn't
                // work ... but the refresh does.
                // Mike
                iTextCanvas.Repaint();
            }
        }

        int FindNext(IDocument document, int offset, char ch)
        {
            LineSegment line = document.GetLineSegmentForOffset(offset);
            int endPos = line.Offset + line.Length;

            while (offset < endPos && document.GetCharAt(offset) != ch)
            {
                ++offset;
            }
            return offset;
        }

        void OnDoubleClick(object sender, System.EventArgs e)
        {
            if (dodragdrop)
            {
                return;
            }

            iTextCanvas.SelectionManager.selectFrom.where = WhereFrom.TArea;
            doubleclick = true;
        }
    }
}
