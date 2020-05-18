
using System;
using System.Collections.Generic;
using System.Drawing;

using NthDimension.Forms;
using static NthDimension.Forms.Widget;
using NthStudio.Gui.Widgets.TextEditor.Document;
using NthStudio.Gui.Widgets.TextEditor.Document.FoldingStrategy;
using NthStudio.Gui.Widgets.TextEditor.Document.HighlightStrategy;
using NthStudio.Gui.Widgets.TextEditor.Document.LineManagement;
using NthStudio.Gui.Widgets.TextEditor.Document.MarkingStrategy;
using NthStudio.Gui.Widgets.TextEditor.Document.Selection;


namespace NthStudio.Gui.Widgets.TextEditor.Margins
{
    public class TextViewMargin : AbstractMargin
    {
        public TextViewMargin(TextCanvas pTextCanvas)
            : base(pTextCanvas)
        {
        }

        #region Properties

        public int FontH
        {
            get;
            private set;
        }

        public int LineHeight
        {
            get;
            private set;
        }

        public override NanoCursor Cursor
        {
            get
            {
                return Cursors.Text;
            }
        }

        int wideSpaceWidth;

        /// <summary>
        /// Gets the width of a 'wide space' (=one quarter of a tab, if tab is set to 4 spaces).
        /// On monospaced fonts, this is the same value as spaceWidth.
        /// </summary>
        public int WideSpaceWidth
        {
            get
            {
                return wideSpaceWidth;
            }
        }

        public int VisibleColumnCount
        {
            get
            {
                return (int)(DrawingPosition.Width / WideSpaceWidth) - 1;
            }
        }

        int spaceWidth;

        /// <summary>
        /// Gets the width of a space character.
        /// This value can be quite small in some fonts - consider using WideSpaceWidth instead.
        /// </summary>
        public int SpaceWidth
        {
            get
            {
                return spaceWidth;
            }
        }

        public int LineHeightRemainder
        {
            get
            {
                return iTextCanvas.VirtualTop.Y % LineHeight;
            }
        }

        public int FirstPhysicalLine
        {
            get
            {
                return iTextCanvas.VirtualTop.Y / LineHeight;
            }
        }

        /// <summary>Gets the first visible <b>logical</b> line.</summary>
        public int FirstVisibleLine
        {
            get
            {
                return iTextCanvas.Document.GetFirstLogicalLine(iTextCanvas.VirtualTop.Y / LineHeight);
            }
            set
            {
                if (FirstVisibleLine != value)
                {
                    iTextCanvas.VirtualTop =
                        new Point(iTextCanvas.VirtualTop.X,
                               iTextCanvas.Document.GetVisibleLine(value) * LineHeight);
                }
            }
        }

        public int VisibleLineCount
        {
            get
            {
                return 1 + DrawingPosition.Height / LineHeight;
            }
        }

        public Highlight Highlight
        {
            get;
            set;
        }

        #endregion Properties

        /// <summary>
        /// returns line/column for a visual point position
        /// </summary>
        public TextLocation GetLogicalPosition(Point mousePosition)
        {
            FoldMarker dummy;
            return GetLogicalColumn(GetLogicalLine(mousePosition.Y), mousePosition.X, out dummy);
        }

        /// <summary>
        /// returns line/column for a visual point position
        /// </summary>
        public TextLocation GetLogicalPosition(int visualPosX, int visualPosY)
        {
            FoldMarker dummy;
            return GetLogicalColumn(GetLogicalLine(visualPosY), visualPosX, out dummy);
        }

        internal TextLocation GetLogicalColumn(int lineNumber, int visualPosX, out FoldMarker inFoldMarker)
        {
            visualPosX += iTextCanvas.VirtualTop.X;

            inFoldMarker = null;
            if (lineNumber >= Document.TotalNumberOfLines)
            {
                return new TextLocation((int)(visualPosX / WideSpaceWidth), lineNumber);
            }
            if (visualPosX <= 0)
            {
                return new TextLocation(0, lineNumber);
            }

            int start = 0; // column
            int posX = 0; // visual position

            int result;
            //using (Graphics g = iTextCanvas.CreateGraphics())
            {
                // call GetLogicalColumnInternal to skip over text,
                // then skip over fold markers
                // and repeat as necessary.
                // The loop terminates once the correct logical column is reached in
                // GetLogicalColumnInternal or inside a fold marker.
                while (true)
                {

                    LineSegment line = Document.GetLineSegment(lineNumber);
                    FoldMarker nextFolding = FindNextFoldedFoldingOnLineAfterColumn(lineNumber, start - 1);
                    int end = nextFolding != null ? nextFolding.StartColumn : int.MaxValue;
                    result = GetLogicalColumnInternal(line, start, end, ref posX, visualPosX);

                    // break when GetLogicalColumnInternal found the result column
                    if (result < end)
                        break;

                    // reached fold marker
                    lineNumber = nextFolding.EndLine;
                    start = nextFolding.EndColumn;
                    int strw = (int)WHUD.LibContext.MeasureTextWidth(nextFolding.FoldText, TextEditorProperties.RegularFont);
                    int newPosX = posX + 1 + strw;
                    if (newPosX >= visualPosX)
                    {
                        inFoldMarker = nextFolding;
                        if (IsNearerToAThanB(visualPosX, posX, newPosX))
                            return new TextLocation(nextFolding.StartColumn, nextFolding.StartLine);
                        else
                            return new TextLocation(nextFolding.EndColumn, nextFolding.EndLine);
                    }
                    posX = newPosX;
                }
            }
            return new TextLocation(result, lineNumber);
        }

        int GetLogicalColumnInternal(LineSegment line, int start, int end, ref int drawingPos, int targetVisualPosX)
        {
            if (start == end)
                return end;
            System.Diagnostics.Debug.Assert(start < end);
            System.Diagnostics.Debug.Assert(drawingPos < targetVisualPosX);

            int tabIndent = Document.TextEditorProperties.TabIndent;

            // OLD_CODE
            {
                /*float spaceWidth = SpaceWidth;
				float drawingPos = 0;
				LineSegment currentLine = Document.GetLineSegment(logicalLine);
				List<TextWord> words = currentLine.Words;
				if (words == null) return 0;
				int wordCount = words.Count;
				int wordOffset = 0;
				FontContainer fontContainer = TextEditorProperties.FontContainer;
				*/
            }
            ITextEditorProperties fontContainer = TextEditorProperties;

            List<TextWord> words = line.Words;
            if (words == null)
                return 0;
            int wordOffset = 0;

            foreach (var word in words)
            {
                if (wordOffset >= end)
                {
                    return wordOffset;
                }
                if (wordOffset + word.Length >= start)
                {
                    int newDrawingPos;
                    switch (word.Type)
                    {
                        case TextWordType.Space:
                            newDrawingPos = drawingPos + spaceWidth;
                            if (newDrawingPos >= targetVisualPosX)
                                return IsNearerToAThanB(targetVisualPosX, drawingPos, newDrawingPos) ?
                                    wordOffset : wordOffset + 1;
                            break;
                        case TextWordType.Tab:
                            // go to next tab position
                            drawingPos = (int)((drawingPos + MinTabWidth) / tabIndent / WideSpaceWidth)
                                * tabIndent * WideSpaceWidth;
                            newDrawingPos = drawingPos + tabIndent * WideSpaceWidth;
                            if (newDrawingPos >= targetVisualPosX)
                                return IsNearerToAThanB(targetVisualPosX, drawingPos, newDrawingPos) ?
                                    wordOffset : wordOffset + 1;
                            break;
                        case TextWordType.Word:
                            int wordStart = Math.Max(wordOffset, start);
                            int wordLength = Math.Min(wordOffset + word.Length, end) - wordStart;
                            string text = Document.GetText(line.Offset + wordStart, wordLength);
                            NanoFont font = word.GetFont(fontContainer) ?? fontContainer.RegularFont;
                            int strw = (int)WHUD.LibContext.MeasureTextWidth(text, font);
                            newDrawingPos = drawingPos + strw;

                            if (newDrawingPos >= targetVisualPosX)
                            {
                                for (int j = 0; j < text.Length; j++)
                                {
                                    strw = (int)WHUD.LibContext.MeasureTextWidth(text[j].ToString(), font);
                                    newDrawingPos = drawingPos + strw;
                                    if (newDrawingPos >= targetVisualPosX)
                                    {
                                        if (IsNearerToAThanB(targetVisualPosX, drawingPos, newDrawingPos))
                                            return wordStart + j;
                                        else
                                            return wordStart + j + 1;
                                    }
                                    drawingPos = newDrawingPos;
                                }
                                return wordStart + text.Length;
                            }
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                    drawingPos = newDrawingPos;
                }
                wordOffset += word.Length;
            }
            return wordOffset;
        }

        static bool IsNearerToAThanB(int num, int a, int b)
        {
            return Math.Abs(a - num) < Math.Abs(b - num);
        }

        /// <summary>
        /// returns logical line number for a visual point
        /// </summary>
        public int GetLogicalLine(int visualPosY)
        {
            int clickedVisualLine = Math.Max(0, (visualPosY + this.iTextCanvas.VirtualTop.Y) / LineHeight);
            return Document.GetFirstLogicalLine(clickedVisualLine);
        }

        public int GetVisualColumnFast(LineSegment line, int logicalColumn)
        {
            int lineOffset = line.Offset;
            int tabIndent = Document.TextEditorProperties.TabIndent;
            int guessedColumn = 0;
            for (int i = 0; i < logicalColumn; ++i)
            {
                char ch;
                if (i >= line.Length)
                {
                    ch = ' ';
                }
                else
                {
                    ch = Document.GetCharAt(lineOffset + i);
                }
                switch (ch)
                {
                    case '\t':
                        guessedColumn += tabIndent;
                        guessedColumn = (guessedColumn / tabIndent) * tabIndent;
                        break;
                    default:
                        ++guessedColumn;
                        break;
                }
            }
            return guessedColumn;
        }

        public int VisibleLineDrawingRemainder
        {
            get
            {
                return iTextCanvas.VirtualTop.Y % LineHeight;
            }
        }

        NanoFont lastFont;

        static int GetFontHeight(NanoFont font)
        {
            int height1 = WHUD.LibContext.MeasureText("_", font).Height;
            int height2 = (int)Math.Ceiling(font.GetHeight());

            return Math.Max(height1, height2) + 1;
        }

        public void OptionsChanged()
        {
            this.lastFont = TextEditorProperties.RegularFont;

            // OLD_CODE
            {
                /*this.LineHeight = GetFontHeight(lastFont);
				// use minimum width - in some fonts, space has no width but kerning is used instead
				// -> DivideByZeroException
				this.spaceWidth = Math.Max((int)WHUD.LibContext.MeasureTextWidth(" ", lastFont), 1);
				// tab should have the width of 4*'x'
				this.wideSpaceWidth = Math.Max(spaceWidth, (int)WHUD.LibContext.MeasureTextWidth("x", lastFont));
				*/
            }

            #region Set LineSpacing

            // Descender es negativo
            FontH = (int)Math.Ceiling(iTextCanvas.Font.Ascender - iTextCanvas.Font.Descender);
            // Simulate lineSpacing equal to (Descender / 4)
            // when lineGap in stb_truetype == 0
            if (FontH == (int)Math.Ceiling(iTextCanvas.Font.Height))
                LineHeight = (int)Math.Ceiling(FontH - (iTextCanvas.Font.Descender / 4));
            else
                LineHeight = (int)Math.Ceiling(iTextCanvas.Font.Height);

            #endregion Set LineSpacing

            // use minimum width - in some fonts, space has no width but kerning is used instead
            // -> DivideByZeroException
            spaceWidth = (int)WHUD.LibContext.MeasureTextWidth(" ", iTextCanvas.Font);
            this.spaceWidth = Math.Max(spaceWidth, 1);
            // tab should have the width of 4*'x'
            this.wideSpaceWidth = Math.Max(spaceWidth, (int)WHUD.LibContext.MeasureTextWidth("x", iTextCanvas.Font));
        }

        public override void Paint(GContext gc, Rectangle rect)
        {
            if (rect.Width <= 0 || rect.Height <= 0 || iTextCanvas.Document == null)
            {
                return;
            }

            // Just to ensure that fontHeight and char widths are always correct...
            if (lastFont != TextEditorProperties.RegularFont)
            {
                OptionsChanged();
                iTextCanvas.Invalidate();
            }

            int horizontalDelta = iTextCanvas.VirtualTop.X;
            if (horizontalDelta > 0)
            {
                gc.SetClip(this.DrawingPosition);
            }

            for (int y = 0; y < (DrawingPosition.Height + VisibleLineDrawingRemainder)
            / LineHeight + 1; ++y)
            {
                var lineRectangle = new Rectangle(DrawingPosition.X - horizontalDelta,
                                                  DrawingPosition.Top + (y * LineHeight) - VisibleLineDrawingRemainder,
                                                  DrawingPosition.Width + horizontalDelta,
                                                  LineHeight);

                if (rect.IntersectsWith(lineRectangle))
                {
                    int fvl = iTextCanvas.Document.GetVisibleLine(FirstVisibleLine);
                    int currentLine = iTextCanvas.Document.GetFirstLogicalLine(fvl + y);
                    PaintDocumentLine(gc, currentLine, lineRectangle);
                }
            }

            DrawMarkerDraw(gc);

            if (horizontalDelta > 0)
            {
                //g.ResetClip();
            }

            Color bgColor = GetBgColor(1);
            Color backgroundColor = iTextCanvas.Enabled ? bgColor : SystemColors.InactiveBorder;
            Color cComp = (384 - backgroundColor.R - backgroundColor.G - backgroundColor.B) > 0 ? Color.White : Color.Black;

            iTextCanvas.Caret.PaintCaret(gc, cComp);
        }

        // used for calculating physical column during paint
        int physicalColumn = 0;

        void PaintDocumentLine(GContext gc, int lineNumber, Rectangle lineRectangle)
        {
            System.Diagnostics.Debug.Assert(lineNumber >= 0);
            /*Brush bgColorBrush = GetBgColorBrush(lineNumber);
			Brush backgroundBrush = textArea.Enabled ? bgColorBrush : SystemBrushes.InactiveBorder;
*/
            Color bgColor = GetBgColor(lineNumber);
            Color backgroundColor = iTextCanvas.Enabled ? bgColor : SystemColors.InactiveBorder;

            if (lineNumber >= iTextCanvas.Document.TotalNumberOfLines)
            {
                //g.FillRectangle(backgroundBrush, lineRectangle);
                gc.FillRectangle(new NanoSolidBrush(backgroundColor), lineRectangle);

                if (TextEditorProperties.ShowInvalidLines)
                {
                    //DrawInvalidLineMarker(g, lineRectangle.Left, lineRectangle.Top);
                }
                if (TextEditorProperties.ShowVerticalRuler)
                {
                    DrawVerticalRuler(gc, lineRectangle);
                }
                // bgColorBrush.Dispose();
                return;
            }

            int physicalXPos = lineRectangle.X;
            // there can't be a folding wich starts in an above line and ends here, because the line is a new one,
            // there must be a return before this line.
            int column = 0;
            physicalColumn = 0;
            if (TextEditorProperties.EnableFolding)
            {
                while (true)
                {
                    List<FoldMarker> starts = iTextCanvas.Document.FoldingManager.GetFoldedFoldingsWithStartAfterColumn(lineNumber, column - 1);
                    if (starts == null || starts.Count <= 0)
                    {
                        if (lineNumber < iTextCanvas.Document.TotalNumberOfLines)
                        {
                            physicalXPos = PaintLinePart(gc, lineNumber, column, iTextCanvas.Document.GetLineSegment(lineNumber).Length, lineRectangle, physicalXPos);
                        }
                        break;
                    }
                    // search the first starting folding
                    var firstFolding = (FoldMarker)starts[0];
                    foreach (FoldMarker fm in starts)
                    {
                        if (fm.StartColumn < firstFolding.StartColumn)
                        {
                            firstFolding = fm;
                        }
                    }
                    starts.Clear();

                    physicalXPos = PaintLinePart(gc, lineNumber, column, firstFolding.StartColumn, lineRectangle, physicalXPos);
                    column = firstFolding.EndColumn;
                    lineNumber = firstFolding.EndLine;
                    if (lineNumber >= iTextCanvas.Document.TotalNumberOfLines)
                    {
                        System.Diagnostics.Debug.Assert(false, "Folding ends after document end");
                        break;
                    }

                    ColumnRange selectionRange2 = iTextCanvas.SelectionManager.GetSelectionAtLine(lineNumber);
                    bool drawSelected = ColumnRange.WholeColumn.Equals(selectionRange2) || firstFolding.StartColumn >= selectionRange2.StartColumn && firstFolding.EndColumn <= selectionRange2.EndColumn;

                    physicalXPos = PaintFoldingText(gc, lineNumber, physicalXPos, lineRectangle, firstFolding.FoldText, drawSelected);
                }
            }
            else
            {
                physicalXPos = PaintLinePart(gc, lineNumber, 0, iTextCanvas.Document.GetLineSegment(lineNumber).Length, lineRectangle, physicalXPos);
            }

            if (lineNumber < iTextCanvas.Document.TotalNumberOfLines)
            {
                // Paint things after end of line
                ColumnRange selectionRange = iTextCanvas.SelectionManager.GetSelectionAtLine(lineNumber);
                LineSegment currentLine = iTextCanvas.Document.GetLineSegment(lineNumber);
                HighlightColor selectionColor = iTextCanvas.Document.HighlightingStrategy.GetColorFor("Selection");

                bool selectionBeyondEOL = selectionRange.EndColumn > currentLine.Length
                                          || ColumnRange.WholeColumn.Equals(selectionRange);

                /*if (TextEditorProperties.ShowEOLMarker)
				{
					HighlightColor eolMarkerColor = textArea.Document.HighlightingStrategy.GetColorFor("EOLMarkers");
					physicalXPos += DrawEOLMarker(g, eolMarkerColor.TextColor, selectionBeyondEOL ? bgColorBrush : backgroundBrush, physicalXPos, lineRectangle.Y);
				}
				else*/
                {
                    // Selección más allá de EOL
                    if (selectionBeyondEOL)
                    {
                        //g.FillRectangle(BrushRegistry.GetBrush(selectionColor.BackgroundColor), new RectangleF(physicalXPos, lineRectangle.Y, WideSpaceWidth, lineRectangle.Height));
                        gc.FillRectangle(new NanoSolidBrush(selectionColor.BackgroundColor),
                                         new Rectangle(physicalXPos,
                                                       lineRectangle.Y,
                                                       WideSpaceWidth,
                                                       lineRectangle.Height));

                        physicalXPos += WideSpaceWidth;
                    }
                }

                //Brush fillBrush = selectionBeyondEOL && TextEditorProperties.AllowCaretBeyondEOL ? bgColorBrush : backgroundBrush;
                Color fillColor = selectionBeyondEOL && TextEditorProperties.AllowCaretBeyondEOL ? bgColor : backgroundColor;
                // Rellena la parte derecha de una línea, a continuación del texto.
                gc.FillRectangle(new NanoSolidBrush(fillColor),
                                 new Rectangle(physicalXPos, lineRectangle.Y, lineRectangle.Width - physicalXPos + lineRectangle.X, lineRectangle.Height));
            }

            if (TextEditorProperties.ShowVerticalRuler)
            {
                DrawVerticalRuler(gc, lineRectangle);
            }
            //			bgColorBrush.Dispose();
        }

        void DrawVerticalRuler(GContext gc, Rectangle lineRectangle)
        {
            int xpos = WideSpaceWidth
                       * TextEditorProperties.VerticalRulerRow
                       - iTextCanvas.VirtualTop.X;
            if (xpos <= 0)
            {
                return;
            }
            HighlightColor vRulerColor = iTextCanvas.Document.HighlightingStrategy.GetColorFor("VRuler");

            gc.DrawLine(new NanoPen(vRulerColor.Color),
                        drawingPosition.Left + xpos,
                        lineRectangle.Top,
                        drawingPosition.Left + xpos,
                        lineRectangle.Bottom);
        }

        bool DrawLineMarkerAtLine(int lineNumber)
        {
            return lineNumber == base.TextCanvas.Caret.Line && iTextCanvas.TextArea.TextEditorProperties.LineViewerStyle == LineViewerStyle.FullRow;
        }

        Color GetBgColor(int lineNumber)
        {
            if (DrawLineMarkerAtLine(lineNumber))
            {
                HighlightColor caretLine = iTextCanvas.Document.HighlightingStrategy.GetColorFor("CaretMarker");
                return caretLine.Color;
            }
            HighlightColor background = iTextCanvas.Document.HighlightingStrategy.GetColorFor("Default");
            Color bgColor = background.BackgroundColor;
            return bgColor;
        }

        /// <summary>
        /// Get the marker brush (for solid block markers) at a given position.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <param name="markers">All markers that have been found.</param>
        /// <returns>The Brush or null when no marker was found.</returns>
        Color GetMarkerBrushAt(int offset, int length, ref Color foreColor, out IList<TextMarker> markers)
        {
            markers = Document.MarkerStrategy.GetMarkers(offset, length);
            foreach (TextMarker marker in markers)
            {
                if (marker.TextMarkerType == TextMarkerType.SolidBlock)
                {
                    if (marker.OverrideForeColor)
                    {
                        foreColor = marker.ForeColor;
                    }
                    return marker.Color;
                }
            }
            return Color.Empty;
        }

        const int MinTabWidth = 4;

        int PaintLinePart(GContext gc, int lineNumber, int startColumn, int endColumn, Rectangle lineRectangle, int physicalXPos)
        {
            bool drawLineMarker = DrawLineMarkerAtLine(lineNumber);
            //Brush backgroundBrush = textArea.Enabled ? GetBgColorBrush(lineNumber) : SystemBrushes.InactiveBorder;
            Color backgroundColor = iTextCanvas.Enabled ? GetBgColor(lineNumber) : SystemColors.InactiveBorder;
            HighlightColor selectionColor = iTextCanvas.Document.HighlightingStrategy.GetColorFor("Selection");
            ColumnRange selectionRange = iTextCanvas.SelectionManager.GetSelectionAtLine(lineNumber);
            HighlightColor tabMarkerColor = iTextCanvas.Document.HighlightingStrategy.GetColorFor("TabMarkers");
            HighlightColor spaceMarkerColor = iTextCanvas.Document.HighlightingStrategy.GetColorFor("SpaceMarkers");

            LineSegment currentLine = iTextCanvas.Document.GetLineSegment(lineNumber);

            //Brush selectionBackgroundBrush = BrushRegistry.GetBrush(selectionColor.BackgroundColor);
            Color selectionBackgroundColor = selectionColor.BackgroundColor;

            if (currentLine.Words == null)
            {
                return physicalXPos;
            }

            int currentWordOffset = 0; // we cannot use currentWord.Offset because it is not set on space words

            TextWord currentWord;
            TextWord nextCurrentWord = null;
            ITextEditorProperties fontContainer = TextEditorProperties;
            for (int wordIdx = 0; wordIdx < currentLine.Words.Count; wordIdx++)
            {
                currentWord = currentLine.Words[wordIdx];
                if (currentWordOffset < startColumn)
                {
                    // TODO: maybe we need to split at startColumn when we support fold markers
                    // inside words
                    currentWordOffset += currentWord.Length;
                    continue;
                }
            repeatDrawCurrentWord:
                //physicalXPos += 10; // leave room between drawn words - useful for debugging the drawing code
                if (currentWordOffset >= endColumn || physicalXPos >= lineRectangle.Right)
                {
                    break;
                }
                int currentWordEndOffset = currentWordOffset + currentWord.Length - 1;
                TextWordType currentWordType = currentWord.Type;

                IList<TextMarker> markers;
                Color wordForeColor;

                if (currentWordType == TextWordType.Space)
                    wordForeColor = spaceMarkerColor.Color;
                else if (currentWordType == TextWordType.Tab)
                    wordForeColor = tabMarkerColor.Color;
                else
                    wordForeColor = currentWord.Color;

                Color wordBackColor = GetMarkerBrushAt(currentLine.Offset + currentWordOffset, currentWord.Length, ref wordForeColor, out markers);

                // It is possible that we have to split the current word because a marker/the selection begins/ends inside it
                if (currentWord.Length > 1)
                {
                    int splitPos = int.MaxValue;
                    if (Highlight != null)
                    {
                        // split both before and after highlight
                        if (Highlight.OpenBrace.Y == lineNumber)
                        {
                            if (Highlight.OpenBrace.X >= currentWordOffset && Highlight.OpenBrace.X <= currentWordEndOffset)
                            {
                                splitPos = Math.Min(splitPos, Highlight.OpenBrace.X - currentWordOffset);
                            }
                        }
                        if (Highlight.CloseBrace.Y == lineNumber)
                        {
                            if (Highlight.CloseBrace.X >= currentWordOffset && Highlight.CloseBrace.X <= currentWordEndOffset)
                            {
                                splitPos = Math.Min(splitPos, Highlight.CloseBrace.X - currentWordOffset);
                            }
                        }
                        if (splitPos == 0)
                        {
                            splitPos = 1; // split after highlight
                        }
                    }

                    if (endColumn < currentWordEndOffset)
                    { // split when endColumn is reached
                        splitPos = Math.Min(splitPos, endColumn - currentWordOffset);
                    }

                    if (selectionRange.StartColumn > currentWordOffset && selectionRange.StartColumn <= currentWordEndOffset)
                    {
                        splitPos = Math.Min(splitPos, selectionRange.StartColumn - currentWordOffset);
                    }
                    else if (selectionRange.EndColumn > currentWordOffset && selectionRange.EndColumn <= currentWordEndOffset)
                    {
                        splitPos = Math.Min(splitPos, selectionRange.EndColumn - currentWordOffset);
                    }

                    foreach (TextMarker marker in markers)
                    {
                        int markerColumn = marker.Offset - currentLine.Offset;
                        int markerEndColumn = marker.EndOffset - currentLine.Offset + 1; // make end offset exclusive
                        if (markerColumn > currentWordOffset && markerColumn <= currentWordEndOffset)
                        {
                            splitPos = Math.Min(splitPos, markerColumn - currentWordOffset);
                        }
                        else if (markerEndColumn > currentWordOffset && markerEndColumn <= currentWordEndOffset)
                        {
                            splitPos = Math.Min(splitPos, markerEndColumn - currentWordOffset);
                        }
                    }

                    if (splitPos != int.MaxValue)
                    {
                        if (nextCurrentWord != null)
                            throw new ApplicationException("split part invalid: first part cannot be splitted further");
                        nextCurrentWord = TextWord.Split(ref currentWord, splitPos);
                        goto repeatDrawCurrentWord; // get markers for first word part
                    }
                }

                // get colors from selection status:
                if (ColumnRange.WholeColumn.Equals(selectionRange)
                    || (selectionRange.StartColumn <= currentWordOffset
                    && selectionRange.EndColumn > currentWordEndOffset))
                {
                    // word is completely selected
                    //wordBackBrush = selectionBackgroundBrush;
                    wordBackColor = selectionBackgroundColor;

                    if (selectionColor.HasForeground)
                    {
                        wordForeColor = selectionColor.Color;
                    }
                }
                else if (drawLineMarker)
                {
                    wordBackColor = backgroundColor;
                }

                if (wordBackColor == Color.Empty)
                { // use default background if no other background is set
                    if (currentWord.SyntaxColor != null
                        && currentWord.SyntaxColor.HasBackground)
                        wordBackColor = currentWord.SyntaxColor.BackgroundColor; //BrushRegistry.GetBrush(currentWord.SyntaxColor.BackgroundColor);
                    else
                        wordBackColor = backgroundColor;
                }

                Rectangle wordRectangle;

                if (currentWord.Type == TextWordType.Space)
                {
                    ++physicalColumn;

                    wordRectangle = new Rectangle(physicalXPos, lineRectangle.Y, SpaceWidth, lineRectangle.Height);
                    gc.FillRectangle(new NanoSolidBrush(wordBackColor), wordRectangle);

                    if (TextEditorProperties.ShowSpaces)
                    {
                        DrawSpaceMarker(gc, wordForeColor, physicalXPos, lineRectangle.Y);
                    }
                    physicalXPos += SpaceWidth;
                }
                else if (currentWord.Type == TextWordType.Tab)
                {
                    int tabIndent = TextEditorProperties.TabIndent;
                    physicalColumn += tabIndent;
                    physicalColumn = (physicalColumn / tabIndent) * tabIndent;
                    // go to next tabstop
                    int physicalTabEnd = ((physicalXPos + MinTabWidth - lineRectangle.X)
                                         / WideSpaceWidth / tabIndent)
                                         * WideSpaceWidth * tabIndent + lineRectangle.X;
                    physicalTabEnd += WideSpaceWidth * tabIndent;

                    wordRectangle = new Rectangle(physicalXPos, lineRectangle.Y, physicalTabEnd - physicalXPos, lineRectangle.Height);
                    gc.FillRectangle(new NanoSolidBrush(wordBackColor), wordRectangle);

                    if (TextEditorProperties.ShowTabs)
                    {
                        DrawTabMarker(gc, wordForeColor, physicalXPos, lineRectangle.Y);
                    }
                    physicalXPos = physicalTabEnd;
                }
                else
                {
                    int wordWidth = DrawDocumentWord(gc,
                                                     currentWord.Word,
                                                     new Point(physicalXPos, lineRectangle.Y),
                                                     currentWord.GetFont(fontContainer),
                                                     wordForeColor,
                                                     new SolidBrush(wordBackColor)); // wordBackBrush);
                    wordRectangle = new Rectangle(physicalXPos, lineRectangle.Y, wordWidth, lineRectangle.Height);

                    physicalXPos += wordWidth;
                }
                foreach (TextMarker marker in markers)
                {
                    if (marker.TextMarkerType != TextMarkerType.SolidBlock)
                    {
                        DrawMarker(gc, marker, wordRectangle);
                    }
                }

                // draw bracket highlight
                if (Highlight != null)
                {
                    if (Highlight.OpenBrace.Y == lineNumber && Highlight.OpenBrace.X == currentWordOffset ||
                        Highlight.CloseBrace.Y == lineNumber && Highlight.CloseBrace.X == currentWordOffset)
                    {
                        DrawBracketHighlight(gc, new Rectangle((int)wordRectangle.X, lineRectangle.Y, (int)wordRectangle.Width, lineRectangle.Height));
                    }
                }

                currentWordOffset += currentWord.Length;
                if (nextCurrentWord != null)
                {
                    currentWord = nextCurrentWord;
                    nextCurrentWord = null;
                    goto repeatDrawCurrentWord;
                }
            }
            if (physicalXPos < lineRectangle.Right && endColumn >= currentLine.Length)
            {
                // draw markers at line end
                IList<TextMarker> markers = Document.MarkerStrategy.GetMarkers(currentLine.Offset + currentLine.Length);
                foreach (TextMarker marker in markers)
                {
                    if (marker.TextMarkerType != TextMarkerType.SolidBlock)
                    {
                        DrawMarker(gc, marker, new RectangleF(physicalXPos, lineRectangle.Y, WideSpaceWidth, lineRectangle.Height));
                    }
                }
            }
            return physicalXPos;
        }

        // split words after 1000 characters. Fixes GDI+ crash on very longs words, for example
        // a 100 KB Base64-file without any line breaks.
        const int MaximumWordLength = 1000;
        const int MaximumCacheSize = 2000;

        int DrawDocumentWord(GContext gc, string word, Point position, NanoFont font, Color foreColor, SolidBrush backBrush)
        {
            if (string.IsNullOrEmpty(word))
            {
                return 0;
            }

            if (word.Length > MaximumWordLength)
            {
                int width = 0;
                for (int i = 0; i < word.Length; i += MaximumWordLength)
                {
                    Point pos = position;
                    pos.X += width;
                    if (i + MaximumWordLength < word.Length)
                        width += DrawDocumentWord(gc, word.Substring(i, MaximumWordLength), pos, font, foreColor, backBrush);
                    else
                        width += DrawDocumentWord(gc, word.Substring(i, word.Length - i), pos, font, foreColor, backBrush);
                }
                return width;
            }

            int wordWidth = (int)WHUD.LibContext.MeasureTextWidth(word, font);

            // OLD_CODE num = ++num % 3;
            gc.FillRectangle(backBrush, // OLD_CODE num == 0 ? Brushes.LightBlue : num == 1 ? Brushes.LightGreen : Brushes.Yellow,
                             new Rectangle(position.X, position.Y, wordWidth + 1, LineHeight));

            int tAscender = (int)Math.Ceiling(iTextCanvas.Font.Ascender);
            int textPoxY = position.Y + tAscender;

            DrawString(gc,
                       word,
                       font,
                       foreColor,
                       position.X,
                       textPoxY);

            return wordWidth;
        }

        struct WordFontPair
        {
            readonly string word;
            readonly NanoFont font;

            public WordFontPair(string word, NanoFont font)
            {
                this.word = word;
                this.font = font;
            }

            public override bool Equals(object obj)
            {
                var myWordFontPair = (WordFontPair)obj;
                if (!word.Equals(myWordFontPair.word))
                {
                    return false;
                }
                return font.Equals(myWordFontPair.font);
            }

            public override int GetHashCode()
            {
                return word.GetHashCode() ^ font.GetHashCode();
            }
        }

        #region DrawHelper functions

        void DrawMarkerDraw(GContext gc)
        {
            foreach (MarkerToDraw m in markersToDraw)
            {
                TextMarker marker = m.marker;
                RectangleF drawingRect = m.drawingRect;
                float drawYPos = drawingRect.Bottom - 1;
                switch (marker.TextMarkerType)
                {
                    case TextMarkerType.Underlined:
                        //g.DrawLine(BrushRegistry.GetPen(marker.TextColor), drawingRect.X, drawYPos, drawingRect.Right, drawYPos);
                        gc.DrawLine(new NanoPen(marker.Color),
                                    (int)drawingRect.X,
                                    (int)drawYPos,
                                    (int)drawingRect.Right,
                                    (int)drawYPos);
                        break;
                    case TextMarkerType.WaveLine:
                        int reminder = ((int)drawingRect.X) % 6;
                        for (float i = (int)drawingRect.X - reminder; i < drawingRect.Right; i += 6)
                        {
                            //g.DrawLine(BrushRegistry.GetPen(marker.TextColor), i, drawYPos + 3 - 4, i + 3, drawYPos + 1 - 4);
                            gc.DrawLine(new NanoPen(marker.Color),
                                        (int)i,
                                        (int)drawYPos + 3 - 4,
                                        (int)i + 3,
                                        (int)drawYPos + 1 - 4);
                            if (i + 3 < drawingRect.Right)
                            {
                                //g.DrawLine(BrushRegistry.GetPen(marker.TextColor), i + 3, drawYPos + 1 - 4, i + 6, drawYPos + 3 - 4);
                                gc.DrawLine(new NanoPen(marker.Color),
                                            (int)i + 3,
                                            (int)drawYPos + 1 - 4,
                                            (int)i + 6,
                                            (int)drawYPos + 3 - 4);
                            }
                        }
                        break;
                    case TextMarkerType.SolidBlock:
                        //g.FillRectangle(BrushRegistry.GetBrush(marker.TextColor), drawingRect);
                        var r = new Rectangle((int)drawingRect.X,
                                                    (int)drawingRect.Y,
                                                    (int)drawingRect.Width,
                                                    (int)drawingRect.Height);
                        gc.FillRectangle(new NanoSolidBrush(marker.Color), r);
                        break;
                }
            }
            markersToDraw.Clear();
        }

        int PaintFoldingText(GContext gc, int lineNumber, int physicalXPos, Rectangle lineRectangle, string text, bool drawSelected)
        {
            // TODO: get font and color from the highlighting file
            HighlightColor selectionColor = iTextCanvas.Document.HighlightingStrategy.GetColorFor("Selection");
            Color bgColor = drawSelected ? selectionColor.BackgroundColor : GetBgColor(lineNumber);
            Color backgroundColor = iTextCanvas.Enabled ? bgColor : SystemColors.InactiveBorder;

            NanoFont font = iTextCanvas.TextEditorProperties.RegularFont;

            int wordWidth = (int)WHUD.LibContext.MeasureTextWidth(text, font) + additionalFoldTextSize;
            var rect = new Rectangle(physicalXPos, lineRectangle.Y, wordWidth + 1, lineRectangle.Height);

            gc.FillRectangle(new NanoSolidBrush(backgroundColor), rect);

            physicalColumn += text.Length;

            int tAscender = (int)Math.Ceiling(iTextCanvas.Font.Ascender);
            int textPoxY = rect.Y + tAscender;

            DrawString(gc,
                       text,
                       font,
                       drawSelected ? selectionColor.Color : Color.Gray,
                       rect.X + 1, textPoxY);
            gc.DrawRectangle(new NanoPen(drawSelected ? Color.DarkGray : Color.Gray),
                             rect.X,
                             rect.Y,
                             rect.Width,
                             rect.Height);

            return physicalXPos + wordWidth + 1;
        }

        void DrawBracketHighlight(GContext gc, Rectangle rect)
        {
            gc.FillRectangle(new NanoSolidBrush(Color.FromArgb(50, 0, 0, 255)), rect);
            gc.DrawRectangle(new NanoPen(Color.Blue), rect);
        }

        struct MarkerToDraw
        {
            internal TextMarker marker;
            internal RectangleF drawingRect;

            public MarkerToDraw(TextMarker marker, RectangleF drawingRect)
            {
                this.marker = marker;
                this.drawingRect = drawingRect;
            }
        }

        List<MarkerToDraw> markersToDraw = new List<MarkerToDraw>();

        void DrawMarker(GContext gc, TextMarker marker, RectangleF drawingRect)
        {
            // draw markers later so they can overdraw the following text
            markersToDraw.Add(new MarkerToDraw(marker, drawingRect));
        }

        void DrawSpaceMarker(GContext gc, Color color, int x, int y)
        {
            HighlightColor spaceMarkerColor = iTextCanvas.Document.HighlightingStrategy.GetColorFor("SpaceMarkers");
            DrawString(gc, "\u00B7", spaceMarkerColor.GetFont(TextEditorProperties), color, x, y);
        }

        void DrawTabMarker(GContext gc, Color color, int x, int y)
        {
            HighlightColor tabMarkerColor = iTextCanvas.Document.HighlightingStrategy.GetColorFor("TabMarkers");
            DrawString(gc, "\u00BB", tabMarkerColor.GetFont(TextEditorProperties), color, x, y);
        }

        float DrawString(GContext gc, string text, NanoFont font, Color color, int x, int y)
        {
            //TextRenderer.DrawText(g, text, font, new Point(x, y), color, textFormatFlags);
            return gc.DrawString(text, font, new NanoSolidBrush(color), x, y);
        }

        #endregion DrawHelper functions

        public int GetVisualColumn(int logicalLine, int logicalColumn)
        {
            int column = 0;
            CountColumns(ref column, 0, logicalColumn, logicalLine);

            return column;
        }

        FoldMarker FindNextFoldedFoldingOnLineAfterColumn(int lineNumber, int column)
        {
            List<FoldMarker> list = Document.FoldingManager.GetFoldedFoldingsWithStartAfterColumn(lineNumber, column);
            if (list.Count != 0)
                return list[0];
            else
                return null;
        }

        /// <summary>
        /// returns line/column for a visual point position
        /// </summary>
        public FoldMarker GetFoldMarkerFromPosition(int visualPosX, int visualPosY)
        {
            FoldMarker foldMarker;
            GetLogicalColumn(GetLogicalLine(visualPosY), visualPosX, out foldMarker);
            return foldMarker;
        }

        float CountColumns(ref int column, int start, int end, int logicalLine)
        {
            if (start > end)
                throw new ArgumentException("start > end");
            if (start == end)
                return 0;

            float drawingPos = 0;
            int tabIndent = Document.TextEditorProperties.TabIndent;
            LineSegment currentLine = Document.GetLineSegment(logicalLine);
            List<TextWord> words = currentLine.Words;

            if (words == null)
                return 0;

            int wordCount = words.Count;
            int wordOffset = 0;
            ITextEditorProperties fontContainer = TextEditorProperties;

            for (int i = 0; i < wordCount; i++)
            {
                TextWord word = words[i];
                if (wordOffset >= end)
                    break;
                if (wordOffset + word.Length >= start)
                {
                    switch (word.Type)
                    {
                        case TextWordType.Space:
                            drawingPos += SpaceWidth;
                            break;
                        case TextWordType.Tab:
                            // go to next tab position
                            drawingPos = (int)((drawingPos + MinTabWidth) / tabIndent / WideSpaceWidth) * tabIndent * WideSpaceWidth;
                            drawingPos += tabIndent * WideSpaceWidth;
                            break;
                        case TextWordType.Word:
                            int wordStart = Math.Max(wordOffset, start);
                            int wordLength = Math.Min(wordOffset + word.Length, end) - wordStart;
                            string text = Document.GetText(currentLine.Offset + wordStart, wordLength);
                            //drawingPos += MeasureStringWidth(g, text, word.GetFont(fontContainer) ?? fontContainer.RegularFont);
                            drawingPos += WHUD.LibContext.MeasureTextWidth(text, word.GetFont(fontContainer) ?? fontContainer.RegularFont);
                            break;
                    }
                }
                wordOffset += word.Length;
            }
            for (int j = currentLine.Length; j < end; j++)
            {
                drawingPos += WideSpaceWidth;
            }
            // add one pixel in column calculation to account for floating point calculation errors
            column += (int)((drawingPos + 1) / WideSpaceWidth);

            /* OLD Code (does not work for fonts like Verdana)
			{
			for (int j = start; j < end; ++j) {
				char ch;
				if (j >= line.Length) {
					ch = ' ';
				} else {
					ch = Document.GetCharAt(line.Offset + j);
				}
				
				switch (ch) {
					case '\t':
						int oldColumn = column;
						column += tabIndent;
						column = (column / tabIndent) * tabIndent;
						drawingPos += (column - oldColumn) * spaceWidth;
						break;
					default:
						++column;
						TextWord word = line.GetWord(j);
						if (word == null || word.Font == null) {
							drawingPos += GetWidth(ch, TextEditorProperties.Font);
						} else {
							drawingPos += GetWidth(ch, word.Font);
						}
						break;
				}
			}
			}
			*/
            return drawingPos;
        }

        #region Conversion

        const int additionalFoldTextSize = 1;

        public int GetDrawingXPos(int logicalLine, int logicalColumn)
        {
            List<FoldMarker> foldings = Document.FoldingManager.GetTopLevelFoldedFoldings();
            int i = 0;
            FoldMarker f = null;
            // search the last folding that's interresting
            for (i = foldings.Count - 1; i >= 0; --i)
            {
                f = foldings[i];
                if (f.StartLine < logicalLine || f.StartLine == logicalLine && f.StartColumn < logicalColumn)
                {
                    break;
                }
                FoldMarker f2 = foldings[i / 2];
                if (f2.StartLine > logicalLine || f2.StartLine == logicalLine && f2.StartColumn >= logicalColumn)
                {
                    i /= 2;
                }
            }
            int lastFolding = 0;
            int firstFolding = 0;
            int column = 0;
            int tabIndent = Document.TextEditorProperties.TabIndent;
            float drawingPos;
            //graphics g = iTextCanvas.CreateGraphics();
            // if no folding is interresting
            if (f == null
                || !(f.StartLine < logicalLine
                || f.StartLine == logicalLine
                && f.StartColumn < logicalColumn))
            {
                drawingPos = CountColumns(ref column, 0, logicalColumn, logicalLine);
                return (int)(drawingPos - iTextCanvas.VirtualTop.X);
            }

            // if logicalLine/logicalColumn is in folding
            if (f.EndLine > logicalLine || f.EndLine == logicalLine && f.EndColumn > logicalColumn)
            {
                logicalColumn = f.StartColumn;
                logicalLine = f.StartLine;
                --i;
            }
            lastFolding = i;

            // search backwards until a new visible line is reched
            for (; i >= 0; --i)
            {
                f = (FoldMarker)foldings[i];
                if (f.EndLine < logicalLine)
                { // reached the begin of a new visible line
                    break;
                }
            }
            firstFolding = i + 1;

            if (lastFolding < firstFolding)
            {
                drawingPos = CountColumns(ref column, 0, logicalColumn, logicalLine);
                return (int)(drawingPos - iTextCanvas.VirtualTop.X);
            }

            int foldEnd = 0;
            drawingPos = 0;
            for (i = firstFolding; i <= lastFolding; ++i)
            {
                f = foldings[i];
                drawingPos += CountColumns(ref column, foldEnd, f.StartColumn, f.StartLine);
                foldEnd = f.EndColumn;
                column += f.FoldText.Length;
                drawingPos += additionalFoldTextSize;
                //drawingPos += MeasureStringWidth(g, f.FoldText, TextEditorProperties.FontContainer.RegularFont);
                drawingPos += WHUD.LibContext.MeasureTextWidth(f.FoldText, TextEditorProperties.RegularFont);
            }
            drawingPos += CountColumns(ref column, foldEnd, logicalColumn, logicalLine);
            //g.Dispose();
            return (int)(drawingPos - iTextCanvas.VirtualTop.X);
        }

        #endregion Conversion
    }
}
