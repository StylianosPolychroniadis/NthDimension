using NthStudio.Gui.Widgets.TextEditor.Document;
using NthStudio.Gui.Widgets.TextEditor.Document.LineManagement;
using NthStudio.Gui.Widgets.TextEditor.Document.Selection;
using System;
using System.Text;

namespace NthStudio.Gui.Widgets.TextEditor.Actions
{
    public class Tab : AbstractEditAction
    {
        public static string GetIndentationString(IDocument document)
        {
            return GetIndentationString(document, null);
        }

        public static string GetIndentationString(IDocument document, TextCanvas pTextCanvas)
        {
            StringBuilder indent = new StringBuilder();

            if (document.TextEditorProperties.ConvertTabsToSpaces)
            {
                int tabIndent = document.TextEditorProperties.IndentationSize;
                if (pTextCanvas != null)
                {
                    int column = pTextCanvas.TextView.GetVisualColumn(pTextCanvas.Caret.Line, pTextCanvas.Caret.Column);
                    indent.Append(new String(' ', tabIndent - column % tabIndent));
                }
                else
                {
                    indent.Append(new String(' ', tabIndent));
                }
            }
            else
            {
                indent.Append('\t');
            }
            return indent.ToString();
        }

        void InsertTabs(IDocument document, ISelection selection, int y1, int y2)
        {
            string indentationString = GetIndentationString(document);
            for (int i = y2; i >= y1; --i)
            {
                LineSegment line = document.GetLineSegment(i);
                if (i == y2 && i == selection.EndPosition.Y && selection.EndPosition.X == 0)
                {
                    continue;
                }

                // this bit is optional - but useful if you are using block tabbing to sort out
                // a source file with a mixture of tabs and spaces
                //				string newLine = document.GetText(line.Offset,line.Length);
                //				document.Replace(line.Offset,line.Length,newLine);

                document.Insert(line.Offset, indentationString);
            }
        }

        void InsertTabAtCaretPosition(TextCanvas pTextCanvas)
        {
            switch (pTextCanvas.Caret.CaretMode)
            {
                case CaretMode.InsertMode:
                    pTextCanvas.InsertString(GetIndentationString(pTextCanvas.Document, pTextCanvas));
                    break;
                case CaretMode.OverwriteMode:
                    string indentStr = GetIndentationString(pTextCanvas.Document, pTextCanvas);
                    pTextCanvas.ReplaceChar(indentStr[0]);
                    if (indentStr.Length > 1)
                    {
                        pTextCanvas.InsertString(indentStr.Substring(1));
                    }
                    break;
            }
            pTextCanvas.SetDesiredColumn();
        }
        /// <remarks>
        /// Executes this edit action
        /// </remarks>
        /// <param name="textArea">The <see cref="ItextArea"/> which is used for callback purposes</param>
        public override void Execute(TextCanvas pTextCanvas)
        {
            if (pTextCanvas.SelectionManager.SelectionIsReadonly)
            {
                return;
            }
            pTextCanvas.Document.UndoStack.StartUndoGroup();
            if (pTextCanvas.SelectionManager.HasSomethingSelected)
            {
                foreach (ISelection selection in pTextCanvas.SelectionManager.SelectionCollection)
                {
                    int startLine = selection.StartPosition.Y;
                    int endLine = selection.EndPosition.Y;
                    if (startLine != endLine)
                    {
                        pTextCanvas.BeginUpdate();
                        InsertTabs(pTextCanvas.Document, selection, startLine, endLine);
                        pTextCanvas.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.LinesBetween, startLine, endLine));
                        pTextCanvas.EndUpdate();
                    }
                    else
                    {
                        InsertTabAtCaretPosition(pTextCanvas);
                        break;
                    }
                }
                pTextCanvas.Document.CommitUpdate();
                pTextCanvas.AutoClearSelection = false;
            }
            else
            {
                InsertTabAtCaretPosition(pTextCanvas);
            }
            pTextCanvas.Document.UndoStack.EndUndoGroup();
        }
    }

    public class ShiftTab : AbstractEditAction
    {
        void RemoveTabs(IDocument document, ISelection selection, int y1, int y2)
        {
            document.UndoStack.StartUndoGroup();
            for (int i = y2; i >= y1; --i)
            {
                LineSegment line = document.GetLineSegment(i);
                if (i == y2 && line.Offset == selection.EndOffset)
                {
                    continue;
                }
                if (line.Length > 0)
                {
                    /**** TextPad Strategy:
					/// first convert leading whitespace to tabs (controversial! - not all editors work like this)
					string newLine = TextUtilities.LeadingWhiteSpaceToTabs(document.GetText(line.Offset,line.Length),document.Properties.Get("TabIndent", 4));
					if(newLine.Length > 0 && newLine[0] == '\t') {
						document.Replace(line.Offset,line.Length,newLine.Substring(1));
						++redocounter;
					}
					else if(newLine.Length > 0 && newLine[0] == ' ') {
						/// there were just some leading spaces but less than TabIndent of them
						int leadingSpaces = 1;
						for(leadingSpaces = 1; leadingSpaces < newLine.Length && newLine[leadingSpaces] == ' '; leadingSpaces++) {
							/// deliberately empty
						}
						document.Replace(line.Offset,line.Length,newLine.Substring(leadingSpaces));
						++redocounter;
					}
					/// else
					/// there were no leading tabs or spaces on this line so do nothing
					/// MS Visual Studio 6 strategy:
					 ****/
                    //					string temp = document.GetText(line.Offset,line.Length);
                    if (line.Length > 0)
                    {
                        int charactersToRemove = 0;
                        if (document.GetCharAt(line.Offset) == '\t')
                        { // first character is a tab - just remove it
                            charactersToRemove = 1;
                        }
                        else if (document.GetCharAt(line.Offset) == ' ')
                        {
                            int leadingSpaces = 1;
                            int tabIndent = document.TextEditorProperties.IndentationSize;
                            for (leadingSpaces = 1; leadingSpaces < line.Length && document.GetCharAt(line.Offset + leadingSpaces) == ' '; leadingSpaces++)
                            {
                                // deliberately empty
                            }
                            if (leadingSpaces >= tabIndent)
                            {
                                // just remove tabIndent
                                charactersToRemove = tabIndent;
                            }
                            else if (line.Length > leadingSpaces && document.GetCharAt(line.Offset + leadingSpaces) == '\t')
                            {
                                // remove the leading spaces and the following tab as they add up
                                // to just one tab stop
                                charactersToRemove = leadingSpaces + 1;
                            }
                            else
                            {
                                // just remove the leading spaces
                                charactersToRemove = leadingSpaces;
                            }
                        }
                        if (charactersToRemove > 0)
                        {
                            document.Remove(line.Offset, charactersToRemove);
                        }
                    }
                }
            }
            document.UndoStack.EndUndoGroup();
        }

        /// <remarks>
        /// Executes this edit action
        /// </remarks>
        /// <param name="textArea">The <see cref="ItextArea"/> which is used for callback purposes</param>
        public override void Execute(TextCanvas pTextCanvas)
        {
            if (pTextCanvas.SelectionManager.HasSomethingSelected)
            {
                foreach (ISelection selection in pTextCanvas.SelectionManager.SelectionCollection)
                {
                    int startLine = selection.StartPosition.Y;
                    int endLine = selection.EndPosition.Y;
                    pTextCanvas.BeginUpdate();
                    RemoveTabs(pTextCanvas.Document, selection, startLine, endLine);
                    pTextCanvas.Document.UpdateQueue.Clear();
                    pTextCanvas.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.LinesBetween, startLine, endLine));
                    pTextCanvas.EndUpdate();

                }
                pTextCanvas.AutoClearSelection = false;
            }
            else
            {
                // Pressing Shift-Tab with nothing selected the cursor will move back to the
                // previous tab stop. It will stop at the beginning of the line. Also, the desired
                // column is updated to that column.
                LineSegment line = pTextCanvas.Document.GetLineSegmentForOffset(pTextCanvas.Caret.Offset);
                string startOfLine = pTextCanvas.Document.GetText(line.Offset, pTextCanvas.Caret.Offset - line.Offset);
                int tabIndent = pTextCanvas.Document.TextEditorProperties.IndentationSize;
                int currentColumn = pTextCanvas.Caret.Column;
                int remainder = currentColumn % tabIndent;
                if (remainder == 0)
                {
                    pTextCanvas.Caret.DesiredColumn = Math.Max(0, currentColumn - tabIndent);
                }
                else
                {
                    pTextCanvas.Caret.DesiredColumn = Math.Max(0, currentColumn - remainder);
                }
                pTextCanvas.SetCaretToDesiredColumn();
            }
        }
    }

    public class ToggleComment : AbstractEditAction
    {
        /// <remarks>
        /// Executes this edit action
        /// </remarks>
        /// <param name="textArea">The <see cref="ItextArea"/> which is used for callback purposes</param>
        public override void Execute(TextCanvas pTextCanvas)
        {
            if (pTextCanvas.Document.ReadOnly)
            {
                return;
            }

            if (pTextCanvas.Document.HighlightingStrategy.Properties.ContainsKey("LineComment"))
            {
                new ToggleLineComment().Execute(pTextCanvas);
            }
            else if (pTextCanvas.Document.HighlightingStrategy.Properties.ContainsKey("BlockCommentBegin") &&
                     pTextCanvas.Document.HighlightingStrategy.Properties.ContainsKey("BlockCommentBegin"))
            {
                new ToggleBlockComment().Execute(pTextCanvas);
            }
        }
    }

    public class ToggleLineComment : AbstractEditAction
    {
        int firstLine;
        int lastLine;

        void RemoveCommentAt(IDocument document, string comment, ISelection selection, int y1, int y2)
        {
            firstLine = y1;
            lastLine = y2;

            for (int i = y2; i >= y1; --i)
            {
                LineSegment line = document.GetLineSegment(i);
                if (selection != null && i == y2 && line.Offset == selection.Offset + selection.Length)
                {
                    --lastLine;
                    continue;
                }

                string lineText = document.GetText(line.Offset, line.Length);
                if (lineText.Trim().StartsWith(comment))
                {
                    document.Remove(line.Offset + lineText.IndexOf(comment), comment.Length);
                }
            }
        }

        void SetCommentAt(IDocument document, string comment, ISelection selection, int y1, int y2)
        {
            firstLine = y1;
            lastLine = y2;

            for (int i = y2; i >= y1; --i)
            {
                LineSegment line = document.GetLineSegment(i);
                if (selection != null && i == y2 && line.Offset == selection.Offset + selection.Length)
                {
                    --lastLine;
                    continue;
                }

                string lineText = document.GetText(line.Offset, line.Length);
                document.Insert(line.Offset, comment);
            }
        }

        bool ShouldComment(IDocument document, string comment, ISelection selection, int startLine, int endLine)
        {
            for (int i = endLine; i >= startLine; --i)
            {
                LineSegment line = document.GetLineSegment(i);
                if (selection != null && i == endLine && line.Offset == selection.Offset + selection.Length)
                {
                    --lastLine;
                    continue;
                }
                string lineText = document.GetText(line.Offset, line.Length);
                if (!lineText.Trim().StartsWith(comment))
                {
                    return true;
                }
            }
            return false;
        }

        /// <remarks>
        /// Executes this edit action
        /// </remarks>
        /// <param name="textArea">The <see cref="ItextArea"/> which is used for callback purposes</param>
        public override void Execute(TextCanvas pTextCanvas)
        {
            if (pTextCanvas.Document.ReadOnly)
            {
                return;
            }

            string comment = null;
            if (pTextCanvas.Document.HighlightingStrategy.Properties.ContainsKey("LineComment"))
            {
                comment = pTextCanvas.Document.HighlightingStrategy.Properties["LineComment"].ToString();
            }

            if (comment == null || comment.Length == 0)
            {
                return;
            }

            pTextCanvas.Document.UndoStack.StartUndoGroup();
            if (pTextCanvas.SelectionManager.HasSomethingSelected)
            {
                bool shouldComment = true;
                foreach (ISelection selection in pTextCanvas.SelectionManager.SelectionCollection)
                {
                    if (!ShouldComment(pTextCanvas.Document, comment, selection, selection.StartPosition.Y, selection.EndPosition.Y))
                    {
                        shouldComment = false;
                        break;
                    }
                }

                foreach (ISelection selection in pTextCanvas.SelectionManager.SelectionCollection)
                {
                    pTextCanvas.BeginUpdate();
                    if (shouldComment)
                    {
                        SetCommentAt(pTextCanvas.Document, comment, selection, selection.StartPosition.Y, selection.EndPosition.Y);
                    }
                    else
                    {
                        RemoveCommentAt(pTextCanvas.Document, comment, selection, selection.StartPosition.Y, selection.EndPosition.Y);
                    }
                    pTextCanvas.Document.UpdateQueue.Clear();
                    pTextCanvas.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.LinesBetween, firstLine, lastLine));
                    pTextCanvas.EndUpdate();
                }
                pTextCanvas.Document.CommitUpdate();
                pTextCanvas.AutoClearSelection = false;
            }
            else
            {
                pTextCanvas.BeginUpdate();
                int caretLine = pTextCanvas.Caret.Line;
                if (ShouldComment(pTextCanvas.Document, comment, null, caretLine, caretLine))
                {
                    SetCommentAt(pTextCanvas.Document, comment, null, caretLine, caretLine);
                }
                else
                {
                    RemoveCommentAt(pTextCanvas.Document, comment, null, caretLine, caretLine);
                }
                pTextCanvas.Document.UpdateQueue.Clear();
                pTextCanvas.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.SingleLine, caretLine));
                pTextCanvas.EndUpdate();
            }
            pTextCanvas.Document.UndoStack.EndUndoGroup();
        }
    }

    public class ToggleBlockComment : AbstractEditAction
    {
        /// <remarks>
        /// Executes this edit action
        /// </remarks>
        /// <param name="textArea">The <see cref="ItextArea"/> which is used for callback purposes</param>
        public override void Execute(TextCanvas pTextCanvas)
        {
            if (pTextCanvas.Document.ReadOnly)
            {
                return;
            }

            string commentStart = null;
            if (pTextCanvas.Document.HighlightingStrategy.Properties.ContainsKey("BlockCommentBegin"))
            {
                commentStart = pTextCanvas.Document.HighlightingStrategy.Properties["BlockCommentBegin"].ToString();
            }

            string commentEnd = null;
            if (pTextCanvas.Document.HighlightingStrategy.Properties.ContainsKey("BlockCommentEnd"))
            {
                commentEnd = pTextCanvas.Document.HighlightingStrategy.Properties["BlockCommentEnd"].ToString();
            }

            if (commentStart == null || commentStart.Length == 0 || commentEnd == null || commentEnd.Length == 0)
            {
                return;
            }

            int selectionStartOffset;
            int selectionEndOffset;

            if (pTextCanvas.SelectionManager.HasSomethingSelected)
            {
                selectionStartOffset = pTextCanvas.SelectionManager.SelectionCollection[0].Offset;
                selectionEndOffset = pTextCanvas.SelectionManager.SelectionCollection[pTextCanvas.SelectionManager.SelectionCollection.Count - 1].EndOffset;
            }
            else
            {
                selectionStartOffset = pTextCanvas.Caret.Offset;
                selectionEndOffset = selectionStartOffset;
            }

            BlockCommentRegion commentRegion = FindSelectedCommentRegion(pTextCanvas.Document, commentStart, commentEnd, selectionStartOffset, selectionEndOffset);

            pTextCanvas.Document.UndoStack.StartUndoGroup();
            if (commentRegion != null)
            {
                RemoveComment(pTextCanvas.Document, commentRegion);
            }
            else if (pTextCanvas.SelectionManager.HasSomethingSelected)
            {
                SetCommentAt(pTextCanvas.Document, selectionStartOffset, selectionEndOffset, commentStart, commentEnd);
            }
            pTextCanvas.Document.UndoStack.EndUndoGroup();

            pTextCanvas.Document.CommitUpdate();
            pTextCanvas.AutoClearSelection = false;
        }

        public static BlockCommentRegion FindSelectedCommentRegion(IDocument document, string commentStart, string commentEnd, int selectionStartOffset, int selectionEndOffset)
        {
            if (document.TextLength == 0)
            {
                return null;
            }

            // Find start of comment in selected text.

            int commentEndOffset = -1;
            string selectedText = document.GetText(selectionStartOffset, selectionEndOffset - selectionStartOffset);

            int commentStartOffset = selectedText.IndexOf(commentStart);
            if (commentStartOffset >= 0)
            {
                commentStartOffset += selectionStartOffset;
            }

            // Find end of comment in selected text.

            if (commentStartOffset >= 0)
            {
                commentEndOffset = selectedText.IndexOf(commentEnd, commentStartOffset + commentStart.Length - selectionStartOffset);
            }
            else
            {
                commentEndOffset = selectedText.IndexOf(commentEnd);
            }

            if (commentEndOffset >= 0)
            {
                commentEndOffset += selectionStartOffset;
            }

            // Find start of comment before or partially inside the
            // selected text.

            int commentEndBeforeStartOffset = -1;
            if (commentStartOffset == -1)
            {
                int offset = selectionEndOffset + commentStart.Length - 1;
                if (offset > document.TextLength)
                {
                    offset = document.TextLength;
                }
                string text = document.GetText(0, offset);
                commentStartOffset = text.LastIndexOf(commentStart);
                if (commentStartOffset >= 0)
                {
                    // Find end of comment before comment start.
                    commentEndBeforeStartOffset = text.IndexOf(commentEnd, commentStartOffset, selectionStartOffset - commentStartOffset);
                    if (commentEndBeforeStartOffset > commentStartOffset)
                    {
                        commentStartOffset = -1;
                    }
                }
            }

            // Find end of comment after or partially after the
            // selected text.

            if (commentEndOffset == -1)
            {
                int offset = selectionStartOffset + 1 - commentEnd.Length;
                if (offset < 0)
                {
                    offset = selectionStartOffset;
                }
                string text = document.GetText(offset, document.TextLength - offset);
                commentEndOffset = text.IndexOf(commentEnd);
                if (commentEndOffset >= 0)
                {
                    commentEndOffset += offset;
                }
            }

            if (commentStartOffset != -1 && commentEndOffset != -1)
            {
                return new BlockCommentRegion(commentStart, commentEnd, commentStartOffset, commentEndOffset);
            }

            return null;
        }


        void SetCommentAt(IDocument document, int offsetStart, int offsetEnd, string commentStart, string commentEnd)
        {
            document.Insert(offsetEnd, commentEnd);
            document.Insert(offsetStart, commentStart);
        }

        void RemoveComment(IDocument document, BlockCommentRegion commentRegion)
        {
            document.Remove(commentRegion.EndOffset, commentRegion.CommentEnd.Length);
            document.Remove(commentRegion.StartOffset, commentRegion.CommentStart.Length);
        }
    }

    public class BlockCommentRegion
    {
        string commentStart = String.Empty;
        string commentEnd = String.Empty;
        int startOffset = -1;
        int endOffset = -1;

        /// <summary>
        /// The end offset is the offset where the comment end string starts from.
        /// </summary>
        public BlockCommentRegion(string commentStart, string commentEnd, int startOffset, int endOffset)
        {
            this.commentStart = commentStart;
            this.commentEnd = commentEnd;
            this.startOffset = startOffset;
            this.endOffset = endOffset;
        }

        public string CommentStart
        {
            get
            {
                return commentStart;
            }
        }

        public string CommentEnd
        {
            get
            {
                return commentEnd;
            }
        }

        public int StartOffset
        {
            get
            {
                return startOffset;
            }
        }

        public int EndOffset
        {
            get
            {
                return endOffset;
            }
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            unchecked
            {
                if (commentStart != null) hashCode += 1000000007 * commentStart.GetHashCode();
                if (commentEnd != null) hashCode += 1000000009 * commentEnd.GetHashCode();
                hashCode += 1000000021 * startOffset.GetHashCode();
                hashCode += 1000000033 * endOffset.GetHashCode();
            }
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            BlockCommentRegion other = obj as BlockCommentRegion;
            if (other == null) return false;
            return this.commentStart == other.commentStart && this.commentEnd == other.commentEnd && this.startOffset == other.startOffset && this.endOffset == other.endOffset;
        }
    }

    public class Backspace : AbstractEditAction
    {
        /// <remarks>
        /// Executes this edit action
        /// </remarks>
        /// <param name="textArea">The <see cref="ItextArea"/> which is used for callback purposes</param>
        public override void Execute(TextCanvas pTextCanvas)
        {
            if (pTextCanvas.SelectionManager.HasSomethingSelected)
            {
                Delete.DeleteSelection(pTextCanvas);
            }
            else
            {
                if (pTextCanvas.Caret.Offset > 0 && !pTextCanvas.IsReadOnly(pTextCanvas.Caret.Offset - 1))
                {
                    pTextCanvas.BeginUpdate();
                    int curLineNr = pTextCanvas.Document.GetLineNumberForOffset(pTextCanvas.Caret.Offset);
                    int curLineOffset = pTextCanvas.Document.GetLineSegment(curLineNr).Offset;

                    if (curLineOffset == pTextCanvas.Caret.Offset)
                    {
                        LineSegment line = pTextCanvas.Document.GetLineSegment(curLineNr - 1);
                        bool lastLine = curLineNr == pTextCanvas.Document.TotalNumberOfLines;
                        int lineEndOffset = line.Offset + line.Length;
                        int lineLength = line.Length;
                        pTextCanvas.Document.Remove(lineEndOffset, curLineOffset - lineEndOffset);
                        pTextCanvas.Caret.Position = new TextLocation(lineLength, curLineNr - 1);
                        pTextCanvas.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.PositionToEnd, new TextLocation(0, curLineNr - 1)));
                    }
                    else
                    {
                        int caretOffset = pTextCanvas.Caret.Offset - 1;
                        pTextCanvas.Caret.Position = pTextCanvas.Document.OffsetToPosition(caretOffset);
                        pTextCanvas.Document.Remove(caretOffset, 1);

                        pTextCanvas.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.PositionToLineEnd, new TextLocation(pTextCanvas.Caret.Offset - pTextCanvas.Document.GetLineSegment(curLineNr).Offset, curLineNr)));
                    }
                    pTextCanvas.EndUpdate();
                }
            }
        }
    }

    public class Delete : AbstractEditAction
    {
        internal static void DeleteSelection(TextCanvas pTextCanvas)
        {
            System.Diagnostics.Debug.Assert(pTextCanvas.SelectionManager.HasSomethingSelected);
            if (pTextCanvas.SelectionManager.SelectionIsReadonly)
                return;
            pTextCanvas.BeginUpdate();
            pTextCanvas.Caret.Position = pTextCanvas.SelectionManager.SelectionCollection[0].StartPosition;
            pTextCanvas.SelectionManager.RemoveSelectedText();
            pTextCanvas.ScrollToCaret();
            pTextCanvas.EndUpdate();
        }

        /// <remarks>
        /// Executes this edit action
        /// </remarks>
        /// <param name="textArea">The <see cref="ItextArea"/> which is used for callback purposes</param>
        public override void Execute(TextCanvas pTextCanvas)
        {
            if (pTextCanvas.SelectionManager.HasSomethingSelected)
            {
                DeleteSelection(pTextCanvas);
            }
            else
            {
                if (pTextCanvas.IsReadOnly(pTextCanvas.Caret.Offset))
                    return;

                if (pTextCanvas.Caret.Offset < pTextCanvas.Document.TextLength)
                {
                    pTextCanvas.BeginUpdate();
                    int curLineNr = pTextCanvas.Document.GetLineNumberForOffset(pTextCanvas.Caret.Offset);
                    LineSegment curLine = pTextCanvas.Document.GetLineSegment(curLineNr);

                    if (curLine.Offset + curLine.Length == pTextCanvas.Caret.Offset)
                    {
                        if (curLineNr + 1 < pTextCanvas.Document.TotalNumberOfLines)
                        {
                            LineSegment nextLine = pTextCanvas.Document.GetLineSegment(curLineNr + 1);

                            pTextCanvas.Document.Remove(pTextCanvas.Caret.Offset, nextLine.Offset - pTextCanvas.Caret.Offset);
                            pTextCanvas.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.PositionToEnd, new TextLocation(0, curLineNr)));
                        }
                    }
                    else
                    {
                        pTextCanvas.Document.Remove(pTextCanvas.Caret.Offset, 1);
                        //						textArea.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.PositionToLineEnd, new TextLocation(textArea.Caret.Offset - textArea.Document.GetLineSegment(curLineNr).Offset, curLineNr)));
                    }
                    pTextCanvas.UpdateMatchingBracket();
                    pTextCanvas.EndUpdate();
                }
            }
        }
    }

    public class MovePageDown : AbstractEditAction
    {
        /// <remarks>
        /// Executes this edit action
        /// </remarks>
        /// <param name="textArea">The <see cref="ItextArea"/> which is used for callback purposes</param>
        public override void Execute(TextCanvas pTextCanvas)
        {
            int curLineNr = pTextCanvas.Caret.Line;
            int requestedLineNumber = Math.Min(pTextCanvas.Document.GetNextVisibleLineAbove(curLineNr, pTextCanvas.TextView.VisibleLineCount), pTextCanvas.Document.TotalNumberOfLines - 1);

            if (curLineNr != requestedLineNumber)
            {
                pTextCanvas.Caret.Position = new TextLocation(0, requestedLineNumber);
                pTextCanvas.SetCaretToDesiredColumn();
            }
        }
    }

    public class MovePageUp : AbstractEditAction
    {
        /// <remarks>
        /// Executes this edit action
        /// </remarks>
        /// <param name="textArea">The <see cref="ItextArea"/> which is used for callback purposes</param>
        public override void Execute(TextCanvas pTextCanvas)
        {
            int curLineNr = pTextCanvas.Caret.Line;
            int requestedLineNumber = Math.Max(pTextCanvas.Document.GetNextVisibleLineBelow(curLineNr, pTextCanvas.TextView.VisibleLineCount), 0);

            if (curLineNr != requestedLineNumber)
            {
                pTextCanvas.Caret.Position = new TextLocation(0, requestedLineNumber);
                pTextCanvas.SetCaretToDesiredColumn();
            }
        }
    }
    public class Return : AbstractEditAction
    {
        /// <remarks>
        /// Executes this edit action
        /// </remarks>
        /// <param name="textArea">The <see cref="TextArea"/> which is used for callback purposes</param>
        public override void Execute(TextCanvas pTextCanvas)
        {
            if (pTextCanvas.Document.ReadOnly)
            {
                return;
            }
            pTextCanvas.BeginUpdate();
            pTextCanvas.Document.UndoStack.StartUndoGroup();
            try
            {
                if (pTextCanvas.HandleKeyPress('\n'))
                    return;

                pTextCanvas.InsertString(Environment.NewLine);

                int curLineNr = pTextCanvas.Caret.Line;
                pTextCanvas.Document.FormattingStrategy.FormatLine(pTextCanvas, curLineNr, pTextCanvas.Caret.Offset, '\n');
                pTextCanvas.SetDesiredColumn();

                pTextCanvas.Document.UpdateQueue.Clear();
                pTextCanvas.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.PositionToEnd, new TextLocation(0, curLineNr - 1)));
            }
            finally
            {
                pTextCanvas.Document.UndoStack.EndUndoGroup();
                pTextCanvas.EndUpdate();
            }
        }
    }

    public class ToggleEditMode : AbstractEditAction
    {
        /// <remarks>
        /// Executes this edit action
        /// </remarks>
        /// <param name="textArea">The <see cref="ItextArea"/> which is used for callback purposes</param>
        public override void Execute(TextCanvas pTextCanvas)
        {
            if (pTextCanvas.Document.ReadOnly)
            {
                return;
            }
            switch (pTextCanvas.Caret.CaretMode)
            {
                case CaretMode.InsertMode:
                    pTextCanvas.Caret.CaretMode = CaretMode.OverwriteMode;
                    break;
                case CaretMode.OverwriteMode:
                    pTextCanvas.Caret.CaretMode = CaretMode.InsertMode;
                    break;
            }
        }
    }

    public class Undo : AbstractEditAction
    {
        /// <remarks>
        /// Executes this edit action
        /// </remarks>
        /// <param name="textArea">The <see cref="ItextArea"/> which is used for callback purposes</param>
        public override void Execute(TextCanvas pTextCanvas)
        {
            pTextCanvas.TextEditor.Undo();
        }
    }

    public class Redo : AbstractEditAction
    {
        /// <remarks>
        /// Executes this edit action
        /// </remarks>
        /// <param name="textArea">The <see cref="ItextArea"/> which is used for callback purposes</param>
        public override void Execute(TextCanvas pTextCanvas)
        {
            pTextCanvas.TextEditor.Redo();
        }
    }

    /// <summary>
    /// handles the ctrl-backspace key
    /// functionality attempts to roughly mimic MS Developer studio
    /// I will implement this as deleting back to the point that ctrl-leftarrow would
    /// take you to
    /// </summary>
    public class WordBackspace : AbstractEditAction
    {
        /// <remarks>
        /// Executes this edit action
        /// </remarks>
        /// <param name="textArea">The <see cref="ItextArea"/> which is used for callback purposes</param>
        public override void Execute(TextCanvas pTextCanvas)
        {
            // if anything is selected we will just delete it first
            if (pTextCanvas.SelectionManager.HasSomethingSelected)
            {
                Delete.DeleteSelection(pTextCanvas);
                return;
            }
            pTextCanvas.BeginUpdate();
            // now delete from the caret to the beginning of the word
            LineSegment line =
                pTextCanvas.Document.GetLineSegmentForOffset(pTextCanvas.Caret.Offset);
            // if we are not at the beginning of a line
            if (pTextCanvas.Caret.Offset > line.Offset)
            {
                int prevWordStart = Util.TextUtility.FindPrevWordStart(pTextCanvas.Document,
                                                                    pTextCanvas.Caret.Offset);
                if (prevWordStart < pTextCanvas.Caret.Offset)
                {
                    if (!pTextCanvas.IsReadOnly(prevWordStart, pTextCanvas.Caret.Offset - prevWordStart))
                    {
                        pTextCanvas.Document.Remove(prevWordStart,
                                                 pTextCanvas.Caret.Offset - prevWordStart);
                        pTextCanvas.Caret.Position = pTextCanvas.Document.OffsetToPosition(prevWordStart);
                    }
                }
            }
            // if we are now at the beginning of a line
            if (pTextCanvas.Caret.Offset == line.Offset)
            {
                // if we are not on the first line
                int curLineNr = pTextCanvas.Document.GetLineNumberForOffset(pTextCanvas.Caret.Offset);
                if (curLineNr > 0)
                {
                    // move to the end of the line above
                    LineSegment lineAbove = pTextCanvas.Document.GetLineSegment(curLineNr - 1);
                    int endOfLineAbove = lineAbove.Offset + lineAbove.Length;
                    int charsToDelete = pTextCanvas.Caret.Offset - endOfLineAbove;
                    if (!pTextCanvas.IsReadOnly(endOfLineAbove, charsToDelete))
                    {
                        pTextCanvas.Document.Remove(endOfLineAbove, charsToDelete);
                        pTextCanvas.Caret.Position = pTextCanvas.Document.OffsetToPosition(endOfLineAbove);
                    }
                }
            }
            pTextCanvas.SetDesiredColumn();
            pTextCanvas.EndUpdate();
            // if there are now less lines, we need this or there are redraw problems
            pTextCanvas.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.PositionToEnd, new TextLocation(0, pTextCanvas.Document.GetLineNumberForOffset(pTextCanvas.Caret.Offset))));
            pTextCanvas.Document.CommitUpdate();
        }
    }

    /// <summary>
    /// handles the ctrl-delete key
    /// functionality attempts to mimic MS Developer studio
    /// I will implement this as deleting forwardto the point that
    /// ctrl-leftarrow would take you to
    /// </summary>
    public class DeleteWord : Delete
    {
        /// <remarks>
        /// Executes this edit action
        /// </remarks>
        /// <param name="textArea">The <see cref="ItextArea"/> which is used for callback purposes</param>
        public override void Execute(TextCanvas pTextCanvas)
        {
            if (pTextCanvas.SelectionManager.HasSomethingSelected)
            {
                DeleteSelection(pTextCanvas);
                return;
            }
            // if anything is selected we will just delete it first
            pTextCanvas.BeginUpdate();
            // now delete from the caret to the beginning of the word
            LineSegment line = pTextCanvas.Document.GetLineSegmentForOffset(pTextCanvas.Caret.Offset);
            if (pTextCanvas.Caret.Offset == line.Offset + line.Length)
            {
                // if we are at the end of a line
                base.Execute(pTextCanvas);
            }
            else
            {
                int nextWordStart = Util.TextUtility.FindNextWordStart(pTextCanvas.Document,
                                                                    pTextCanvas.Caret.Offset);
                if (nextWordStart > pTextCanvas.Caret.Offset)
                {
                    if (!pTextCanvas.IsReadOnly(pTextCanvas.Caret.Offset, nextWordStart - pTextCanvas.Caret.Offset))
                    {
                        pTextCanvas.Document.Remove(pTextCanvas.Caret.Offset, nextWordStart - pTextCanvas.Caret.Offset);
                        // cursor never moves with this command
                    }
                }
            }
            pTextCanvas.UpdateMatchingBracket();
            pTextCanvas.EndUpdate();
            // if there are now less lines, we need this or there are redraw problems
            pTextCanvas.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.PositionToEnd, new TextLocation(0, pTextCanvas.Document.GetLineNumberForOffset(pTextCanvas.Caret.Offset))));
            pTextCanvas.Document.CommitUpdate();
        }
    }

    public class DeleteLine : AbstractEditAction
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            int lineNr = pTextCanvas.Caret.Line;
            LineSegment line = pTextCanvas.Document.GetLineSegment(lineNr);
            if (pTextCanvas.IsReadOnly(line.Offset, line.Length))
                return;
            pTextCanvas.Document.Remove(line.Offset, line.TotalLength);
            pTextCanvas.Caret.Position = pTextCanvas.Document.OffsetToPosition(line.Offset);

            pTextCanvas.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.PositionToEnd, new TextLocation(0, lineNr)));
            pTextCanvas.UpdateMatchingBracket();
            pTextCanvas.Document.CommitUpdate();
        }
    }

    public class DeleteToLineEnd : AbstractEditAction
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            int lineNr = pTextCanvas.Caret.Line;
            LineSegment line = pTextCanvas.Document.GetLineSegment(lineNr);

            int numRemove = (line.Offset + line.Length) - pTextCanvas.Caret.Offset;
            if (numRemove > 0 && !pTextCanvas.IsReadOnly(pTextCanvas.Caret.Offset, numRemove))
            {
                pTextCanvas.Document.Remove(pTextCanvas.Caret.Offset, numRemove);
                pTextCanvas.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.SingleLine, new TextLocation(0, lineNr)));
                pTextCanvas.Document.CommitUpdate();
            }
        }
    }

    public class GotoMatchingBrace : AbstractEditAction
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            Highlight highlight = pTextCanvas.FindMatchingBracketHighlight();
            if (highlight != null)
            {
                TextLocation p1 = new TextLocation(highlight.CloseBrace.X + 1, highlight.CloseBrace.Y);
                TextLocation p2 = new TextLocation(highlight.OpenBrace.X + 1, highlight.OpenBrace.Y);
                if (p1 == pTextCanvas.Caret.Position)
                {
                    if (pTextCanvas.Document.TextEditorProperties.BracketMatchingStyle == BracketMatchingStyle.After)
                    {
                        pTextCanvas.Caret.Position = p2;
                    }
                    else
                    {
                        pTextCanvas.Caret.Position = new TextLocation(p2.X - 1, p2.Y);
                    }
                }
                else
                {
                    if (pTextCanvas.Document.TextEditorProperties.BracketMatchingStyle == BracketMatchingStyle.After)
                    {
                        pTextCanvas.Caret.Position = p1;
                    }
                    else
                    {
                        pTextCanvas.Caret.Position = new TextLocation(p1.X - 1, p1.Y);
                    }
                }
                pTextCanvas.SetDesiredColumn();
            }
        }
    }
}
