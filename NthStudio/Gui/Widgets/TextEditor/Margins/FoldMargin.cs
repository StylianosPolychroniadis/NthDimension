using NthDimension;
using NthDimension.Forms;
using NthStudio.Gui.Widgets.TextEditor.Document.FoldingStrategy;
using NthStudio.Gui.Widgets.TextEditor.Document.HighlightStrategy;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Gui.Widgets.TextEditor.Margins
{
    /// <summary>
    /// This class views the line numbers and folding markers.
    /// </summary>
    public class FoldMargin : AbstractMargin
    {
        int selectedFoldLine = -1;

        public override Size Size
        {
            get
            {
                return new Size((int)(TextCanvas.TextView.LineHeight), -1);
            }
        }

        public override bool IsVisible
        {
            get
            {
                return TextCanvas.TextEditorProperties.EnableFolding;
            }
        }

        public FoldMargin(TextCanvas pTextCanvas)
            : base(pTextCanvas)
        {
        }

        Color backColor;

        public override void Paint(GContext gc, Rectangle rect)
        {
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                return;
            }
            HighlightColor lineNumberPainterColor = iTextCanvas.Document.HighlightingStrategy.GetColorFor("LineNumbers");


            for (int y = 0; y < (DrawingPosition.Height + iTextCanvas.TextView.VisibleLineDrawingRemainder) / iTextCanvas.TextView.LineHeight + 1; ++y)
            {
                var markerRectangle = new Rectangle(DrawingPosition.X,
                                                          DrawingPosition.Top + y * iTextCanvas.TextView.LineHeight - iTextCanvas.TextView.VisibleLineDrawingRemainder,
                                                          DrawingPosition.Width,
                                                          iTextCanvas.TextView.LineHeight);

                if (rect.IntersectsWith(markerRectangle))
                {
                    // draw dotted separator line
                    if (iTextCanvas.Document.TextEditorProperties.ShowLineNumbers)
                    {
                        //TextColor backColor = TextColor.WhiteSmoke;  //iTextCanvas.Enabled ? lineNumberPainterColor.BackgroundColor : SystemColors.InactiveBorder;
                        backColor = iTextCanvas.Document.HighlightingStrategy.GetColorFor("Default").BackgroundColor;
                        backColor = Utilities.ColorUtil.ChangeColorBrightness(backColor, 0.03f);

                        gc.FillRectangle(new NanoSolidBrush(backColor),
                                         markerRectangle);

                        gc.DrawLine(new NanoPen(lineNumberPainterColor.Color),
                                    base.drawingPosition.X,
                                    markerRectangle.Y,
                                    base.drawingPosition.X,
                                    markerRectangle.Bottom);
                    }
                    else
                    {
                        gc.FillRectangle(new NanoSolidBrush(iTextCanvas.Enabled ? lineNumberPainterColor.BackgroundColor : SystemColors.InactiveBorder), markerRectangle);
                    }

                    int currentLine = iTextCanvas.Document.GetFirstLogicalLine(iTextCanvas.TextView.FirstPhysicalLine + y);
                    if (currentLine < iTextCanvas.Document.TotalNumberOfLines)
                    {
                        PaintFoldMarker(gc, currentLine, markerRectangle);
                    }
                }
            }
        }

        bool SelectedFoldingFrom(List<FoldMarker> list)
        {
            if (list != null)
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    if (this.selectedFoldLine == list[i].StartLine)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        void PaintFoldMarker(GContext gc, int lineNumber, Rectangle drawingRectangle)
        {
            HighlightColor foldLineColor = iTextCanvas.Document.HighlightingStrategy.GetColorFor("FoldLine");
            HighlightColor tSelectedFoldLine = iTextCanvas.Document.HighlightingStrategy.GetColorFor("SelectedFoldLine");

            List<FoldMarker> foldingsWithStart = iTextCanvas.Document.FoldingManager.GetFoldingsWithStart(lineNumber);
            List<FoldMarker> foldingsBetween = iTextCanvas.Document.FoldingManager.GetFoldingsContainsLineNumber(lineNumber);
            List<FoldMarker> foldingsWithEnd = iTextCanvas.Document.FoldingManager.GetFoldingsWithEnd(lineNumber);

            bool isFoldStart = foldingsWithStart.Count > 0;
            bool isBetween = foldingsBetween.Count > 0;
            bool isFoldEnd = foldingsWithEnd.Count > 0;

            bool isStartSelected = SelectedFoldingFrom(foldingsWithStart);
            bool isBetweenSelected = SelectedFoldingFrom(foldingsBetween);
            bool isEndSelected = SelectedFoldingFrom(foldingsWithEnd);

            //int foldMarkerSize1 = (int)Math.Round(iTextCanvas.TextView.LineHeight * 0.57f);
            int foldMarkerSize = (int)Math.Ceiling(iTextCanvas.TextView.LineHeight * 0.57f);
            foldMarkerSize -= ((foldMarkerSize) % 2 > 0) ? 0 : 1;
            int foldMarkerYPos = drawingRectangle.Y + (int)((drawingRectangle.Height - foldMarkerSize) / 2);
            int xPos = drawingRectangle.X + (drawingRectangle.Width - foldMarkerSize) / 2 + foldMarkerSize / 2;


            if (isFoldStart)
            {
                bool isVisible = true;
                bool moreLinedOpenFold = false;
                foreach (FoldMarker foldMarker in foldingsWithStart)
                {
                    if (foldMarker.IsFolded)
                    {
                        isVisible = false;
                    }
                    else
                    {
                        moreLinedOpenFold = foldMarker.EndLine > foldMarker.StartLine;
                    }
                }

                bool isFoldEndFromUpperFold = false;
                foreach (FoldMarker foldMarker in foldingsWithEnd)
                {
                    if (foldMarker.EndLine > foldMarker.StartLine && !foldMarker.IsFolded)
                    {
                        isFoldEndFromUpperFold = true;
                    }
                }

                DrawFoldMarker(gc, new RectangleF(drawingRectangle.X + (drawingRectangle.Width - foldMarkerSize) / 2,
                                                    foldMarkerYPos,
                                                    foldMarkerSize,
                                                    foldMarkerSize),
                               isVisible,
                               isStartSelected
                );

                // draw line above fold marker
                if (isBetween || isFoldEndFromUpperFold)
                {
                    gc.DrawLine(new NanoPen(isBetweenSelected ? tSelectedFoldLine.Color : foldLineColor.Color),
                               xPos,
                               drawingRectangle.Top,
                               xPos,
                               foldMarkerYPos - 1);
                }

                // draw line below fold marker
                if (isBetween || moreLinedOpenFold)
                {
                    gc.DrawLine(new NanoPen(isEndSelected || (isStartSelected && isVisible) || isBetweenSelected ? tSelectedFoldLine.Color : foldLineColor.Color),
                               xPos,
                               foldMarkerYPos + foldMarkerSize + 1,
                               xPos,
                               drawingRectangle.Bottom);
                }
            }
            else
            {
                if (isFoldEnd)
                {
                    int midy = drawingRectangle.Top + drawingRectangle.Height / 2;

                    // draw fold end marker
                    gc.DrawLine(new NanoPen(isEndSelected ? tSelectedFoldLine.Color : foldLineColor.Color),
                               xPos,
                               midy,
                               xPos + foldMarkerSize / 2,
                               midy);

                    // draw line above fold end marker
                    // must be drawn after fold marker because it might have a different color than the fold marker
                    gc.DrawLine(new NanoPen(isBetweenSelected || isEndSelected ? tSelectedFoldLine.Color : foldLineColor.Color),
                               xPos,
                               drawingRectangle.Top,
                               xPos,
                               midy);

                    // draw line below fold end marker
                    if (isBetween)
                    {
                        gc.DrawLine(new NanoPen(isBetweenSelected ? tSelectedFoldLine.Color : foldLineColor.Color),
                                   xPos,
                                   midy + 1,
                                   xPos,
                                   drawingRectangle.Bottom);
                    }
                }
                else if (isBetween)
                {
                    // just draw the line :)
                    gc.DrawLine(new NanoPen(isBetweenSelected ? tSelectedFoldLine.Color : foldLineColor.Color),
                               xPos,
                               drawingRectangle.Top,
                               xPos,
                               drawingRectangle.Bottom);
                }
            }
        }

        public override void HandleMouseMove(Point mousepos, MouseButton mouseButtons)
        {
            bool showFolding = iTextCanvas.Document.TextEditorProperties.EnableFolding;
            int physicalLine = +(int)((mousepos.Y + iTextCanvas.VirtualTop.Y) / iTextCanvas.TextView.LineHeight);
            int realline = iTextCanvas.Document.GetFirstLogicalLine(physicalLine);

            if (!showFolding || realline < 0 || realline + 1 >= iTextCanvas.Document.TotalNumberOfLines)
            {
                return;
            }

            List<FoldMarker> foldMarkers = iTextCanvas.Document.FoldingManager.GetFoldingsWithStart(realline);
            int oldSelection = selectedFoldLine;
            if (foldMarkers.Count > 0)
            {
                selectedFoldLine = realline;
            }
            else
            {
                selectedFoldLine = -1;
            }
            if (oldSelection != selectedFoldLine)
            {
                iTextCanvas.Refresh(this);
            }
        }

        public override void HandleMouseDown(Point mousepos, MouseButton mouseButtons)
        {
            bool showFolding = iTextCanvas.Document.TextEditorProperties.EnableFolding;
            int physicalLine = +(int)((mousepos.Y + iTextCanvas.VirtualTop.Y) / iTextCanvas.TextView.LineHeight);
            int realline = iTextCanvas.Document.GetFirstLogicalLine(physicalLine);

            // focus the textarea if the user clicks on the line number view
            iTextCanvas.Focus();

            if (!showFolding || realline < 0 || realline + 1 >= iTextCanvas.Document.TotalNumberOfLines)
            {
                return;
            }

            List<FoldMarker> foldMarkers = iTextCanvas.Document.FoldingManager.GetFoldingsWithStart(realline);
            foreach (FoldMarker fm in foldMarkers)
            {
                fm.IsFolded = !fm.IsFolded;
            }
            iTextCanvas.Document.FoldingManager.NotifyFoldingsChanged(EventArgs.Empty);
        }

        public override void HandleMouseLeave(EventArgs e)
        {
            if (selectedFoldLine != -1)
            {
                selectedFoldLine = -1;
                iTextCanvas.Refresh(this);
            }
        }

        #region Drawing functions

        void DrawFoldMarker(GContext gc, RectangleF rectangle, bool isOpened, bool isSelected)
        {
            HighlightColor foldMarkerColor = iTextCanvas.Document.HighlightingStrategy.GetColorFor("FoldMarker");
            HighlightColor foldLineColor = iTextCanvas.Document.HighlightingStrategy.GetColorFor("FoldLine");
            HighlightColor tSelectedFoldLine = iTextCanvas.Document.HighlightingStrategy.GetColorFor("SelectedFoldLine");

            var intRect = new Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
            //gc.FillRectangle(new NanoSolidBrush(foldMarkerColor.BackgroundColor), intRect);
            gc.FillRectangle(new NanoSolidBrush(backColor), intRect);
            gc.DrawRectangle(new NanoPen(isSelected ? tSelectedFoldLine.Color : foldLineColor.Color), intRect);

            int space = (int)Math.Round(((double)rectangle.Height) / 8d) + 1;
            int mid = intRect.Height / 2 + intRect.Height % 2;

            // draw minus
            gc.DrawLine(new NanoPen(foldMarkerColor.Color),
                        (int)(rectangle.X + space),
                        (int)(rectangle.Y + mid - 1),
                        (int)(rectangle.X + rectangle.Width) - space,
                        (int)rectangle.Y + mid - 1);

            // draw plus
            if (!isOpened)
            {
                gc.DrawLine(new NanoPen(foldMarkerColor.Color),
                            (int)(rectangle.X + mid - 1),
                            (int)(rectangle.Y + space),
                            (int)(rectangle.X + mid - 1),
                            (int)(rectangle.Y + rectangle.Height) - space);
            }
        }

        #endregion
    }
}
