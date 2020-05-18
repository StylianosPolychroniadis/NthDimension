using NthStudio.Gui.Widgets.TextEditor.Document;
using NthStudio.Gui.Widgets.TextEditor.Document.FoldingStrategy;
using NthStudio.Gui.Widgets.TextEditor.Document.LineManagement;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace NthStudio.Gui.Widgets.TextEditor.Actions
{
    public class CaretLeft : AbstractEditAction
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            TextLocation position = pTextCanvas.Caret.Position;
            List<FoldMarker> foldings = pTextCanvas.Document.FoldingManager.GetFoldedFoldingsWithEnd(position.Y);
            FoldMarker justBeforeCaret = null;
            foreach (FoldMarker fm in foldings)
            {
                if (fm.EndColumn == position.X)
                {
                    justBeforeCaret = fm;
                    break; // the first folding found is the folding with the smallest Startposition
                }
            }

            if (justBeforeCaret != null)
            {
                position.Y = justBeforeCaret.StartLine;
                position.X = justBeforeCaret.StartColumn;
            }
            else
            {
                if (position.X > 0)
                {
                    --position.X;
                }
                else if (position.Y > 0)
                {
                    LineSegment lineAbove = pTextCanvas.Document.GetLineSegment(position.Y - 1);
                    position = new TextLocation(lineAbove.Length, position.Y - 1);
                }
            }

            pTextCanvas.Caret.Position = position;
            pTextCanvas.SetDesiredColumn();
        }
    }

    public class CaretRight : AbstractEditAction
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            LineSegment curLine = pTextCanvas.Document.GetLineSegment(pTextCanvas.Caret.Line);
            TextLocation position = pTextCanvas.Caret.Position;
            List<FoldMarker> foldings = pTextCanvas.Document.FoldingManager.GetFoldedFoldingsWithStart(position.Y);
            FoldMarker justBehindCaret = null;
            foreach (FoldMarker fm in foldings)
            {
                if (fm.StartColumn == position.X)
                {
                    justBehindCaret = fm;
                    break;
                }
            }
            if (justBehindCaret != null)
            {
                position.Y = justBehindCaret.EndLine;
                position.X = justBehindCaret.EndColumn;
            }
            else
            { // no folding is interesting
                if (position.X < curLine.Length || pTextCanvas.TextEditorProperties.AllowCaretBeyondEOL)
                {
                    ++position.X;
                }
                else if (position.Y + 1 < pTextCanvas.Document.TotalNumberOfLines)
                {
                    ++position.Y;
                    position.X = 0;
                }
            }
            pTextCanvas.Caret.Position = position;
            pTextCanvas.SetDesiredColumn();
        }
    }

    public class CaretUp : AbstractEditAction
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            TextLocation position = pTextCanvas.Caret.Position;
            int lineNr = position.Y;
            int visualLine = pTextCanvas.Document.GetVisibleLine(lineNr);
            if (visualLine > 0)
            {
                Point pos = new Point(pTextCanvas.TextView.GetDrawingXPos(lineNr, position.X),
                                      pTextCanvas.TextView.DrawingPosition.Y + (visualLine - 1) * pTextCanvas.TextView.LineHeight - pTextCanvas.VirtualTop.Y);
                pTextCanvas.Caret.Position = pTextCanvas.TextView.GetLogicalPosition(pos);
                pTextCanvas.SetCaretToDesiredColumn();
            }
            //			if (textArea.Caret.Line  > 0) {
            //				textArea.SetCaretToDesiredColumn(textArea.Caret.Line - 1);
            //			}
        }
    }

    public class CaretDown : AbstractEditAction
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            TextLocation position = pTextCanvas.Caret.Position;
            int lineNr = position.Y;
            int visualLine = pTextCanvas.Document.GetVisibleLine(lineNr);
            if (visualLine < pTextCanvas.Document.GetVisibleLine(pTextCanvas.Document.TotalNumberOfLines))
            {
                Point pos = new Point(pTextCanvas.TextView.GetDrawingXPos(lineNr, position.X),
                                      pTextCanvas.TextView.DrawingPosition.Y
                                      + (visualLine + 1) * pTextCanvas.TextView.LineHeight
                                      - pTextCanvas.VirtualTop.Y);
                pTextCanvas.Caret.Position = pTextCanvas.TextView.GetLogicalPosition(pos);
                pTextCanvas.SetCaretToDesiredColumn();
            }
            //			if (textArea.Caret.Line + 1 < textArea.Document.TotalNumberOfLines) {
            //				textArea.SetCaretToDesiredColumn(textArea.Caret.Line + 1);
            //			}
        }
    }

    public class WordRight : CaretRight
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            LineSegment line = pTextCanvas.Document.GetLineSegment(pTextCanvas.Caret.Position.Y);
            TextLocation oldPos = pTextCanvas.Caret.Position;
            TextLocation newPos;
            if (pTextCanvas.Caret.Column >= line.Length)
            {
                newPos = new TextLocation(0, pTextCanvas.Caret.Line + 1);
            }
            else
            {
                int nextWordStart = Util.TextUtility.FindNextWordStart(pTextCanvas.Document, pTextCanvas.Caret.Offset);
                newPos = pTextCanvas.Document.OffsetToPosition(nextWordStart);
            }

            // handle fold markers
            List<FoldMarker> foldings = pTextCanvas.Document.FoldingManager.GetFoldingsFromPosition(newPos.Y, newPos.X);
            foreach (FoldMarker marker in foldings)
            {
                if (marker.IsFolded)
                {
                    if (oldPos.X == marker.StartColumn && oldPos.Y == marker.StartLine)
                    {
                        newPos = new TextLocation(marker.EndColumn, marker.EndLine);
                    }
                    else
                    {
                        newPos = new TextLocation(marker.StartColumn, marker.StartLine);
                    }
                    break;
                }
            }

            pTextCanvas.Caret.Position = newPos;
            pTextCanvas.SetDesiredColumn();
        }
    }

    public class WordLeft : CaretLeft
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            TextLocation oldPos = pTextCanvas.Caret.Position;
            if (pTextCanvas.Caret.Column == 0)
            {
                base.Execute(pTextCanvas);
            }
            else
            {
                //LineSegment line = pTextCanvas.Document.GetLineSegment(pTextCanvas.Caret.Position.Y);

                int prevWordStart = Util.TextUtility.FindPrevWordStart(pTextCanvas.Document, pTextCanvas.Caret.Offset);

                TextLocation newPos = pTextCanvas.Document.OffsetToPosition(prevWordStart);

                // handle fold markers
                List<FoldMarker> foldings = pTextCanvas.Document.FoldingManager.GetFoldingsFromPosition(newPos.Y, newPos.X);
                foreach (FoldMarker marker in foldings)
                {
                    if (marker.IsFolded)
                    {
                        if (oldPos.X == marker.EndColumn && oldPos.Y == marker.EndLine)
                        {
                            newPos = new TextLocation(marker.StartColumn, marker.StartLine);
                        }
                        else
                        {
                            newPos = new TextLocation(marker.EndColumn, marker.EndLine);
                        }
                        break;
                    }
                }
                pTextCanvas.Caret.Position = newPos;
                pTextCanvas.SetDesiredColumn();
            }
        }
    }

    public class ScrollLineUp : AbstractEditAction
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            pTextCanvas.AutoClearSelection = false;

            pTextCanvas.TextArea.ScrollBarV.Value = Math.Max(pTextCanvas.TextArea.ScrollBarV.Minimum,
                                                             pTextCanvas.VirtualTop.Y - pTextCanvas.TextView.LineHeight);
        }
    }

    public class ScrollLineDown : AbstractEditAction
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            pTextCanvas.AutoClearSelection = false;
            pTextCanvas.TextArea.ScrollBarV.Value = Math.Min(pTextCanvas.TextArea.ScrollBarV.Maximum,
                                                             pTextCanvas.VirtualTop.Y + pTextCanvas.TextView.LineHeight);
        }
    }
}
