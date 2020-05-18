using NthStudio.Gui.Widgets.TextEditor.Document;
using NthStudio.Gui.Widgets.TextEditor.Document.FoldingStrategy;
using NthStudio.Gui.Widgets.TextEditor.Document.LineManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Gui.Widgets.TextEditor.Actions
{
    public class Home : AbstractEditAction
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            LineSegment curLine;
            TextLocation newPos = pTextCanvas.Caret.Position;
            bool jumpedIntoFolding = false;
            do
            {
                curLine = pTextCanvas.Document.GetLineSegment(newPos.Y);

                if (Util.TextUtility.IsEmptyLine(pTextCanvas.Document, newPos.Y))
                {
                    if (newPos.X != 0)
                    {
                        newPos.X = 0;
                    }
                    else
                    {
                        newPos.X = curLine.Length;
                    }
                }
                else
                {
                    int firstCharOffset = Util.TextUtility.GetFirstNonWSChar(pTextCanvas.Document, curLine.Offset);
                    int firstCharColumn = firstCharOffset - curLine.Offset;

                    if (newPos.X == firstCharColumn)
                    {
                        newPos.X = 0;
                    }
                    else
                    {
                        newPos.X = firstCharColumn;
                    }
                }
                List<FoldMarker> foldings = pTextCanvas.Document.FoldingManager.GetFoldingsFromPosition(newPos.Y, newPos.X);
                jumpedIntoFolding = false;
                foreach (FoldMarker foldMarker in foldings)
                {
                    if (foldMarker.IsFolded)
                    {
                        newPos = new TextLocation(foldMarker.StartColumn, foldMarker.StartLine);
                        jumpedIntoFolding = true;
                        break;
                    }
                }

            } while (jumpedIntoFolding);

            if (newPos != pTextCanvas.Caret.Position)
            {
                pTextCanvas.Caret.Position = newPos;
                pTextCanvas.SetDesiredColumn();
            }
        }
    }

    public class End : AbstractEditAction
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            LineSegment curLine;
            TextLocation newPos = pTextCanvas.Caret.Position;
            bool jumpedIntoFolding = false;
            do
            {
                curLine = pTextCanvas.Document.GetLineSegment(newPos.Y);
                newPos.X = curLine.Length;

                List<FoldMarker> foldings = pTextCanvas.Document.FoldingManager.GetFoldingsFromPosition(newPos.Y, newPos.X);
                jumpedIntoFolding = false;
                foreach (FoldMarker foldMarker in foldings)
                {
                    if (foldMarker.IsFolded)
                    {
                        newPos = new TextLocation(foldMarker.EndColumn, foldMarker.EndLine);
                        jumpedIntoFolding = true;
                        break;
                    }
                }
            } while (jumpedIntoFolding);

            if (newPos != pTextCanvas.Caret.Position)
            {
                pTextCanvas.Caret.Position = newPos;
                pTextCanvas.SetDesiredColumn();
            }
        }
    }


    public class MoveToStart : AbstractEditAction
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            if (pTextCanvas.Caret.Line != 0 || pTextCanvas.Caret.Column != 0)
            {
                pTextCanvas.Caret.Position = new TextLocation(0, 0);
                pTextCanvas.SetDesiredColumn();
            }
        }
    }


    public class MoveToEnd : AbstractEditAction
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            TextLocation endPos = pTextCanvas.Document.OffsetToPosition(pTextCanvas.Document.TextLength);
            if (pTextCanvas.Caret.Position != endPos)
            {
                pTextCanvas.Caret.Position = endPos;
                pTextCanvas.SetDesiredColumn();
            }
        }
    }
}
