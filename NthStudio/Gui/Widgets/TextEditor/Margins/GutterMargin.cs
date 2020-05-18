using NthDimension;
using NthDimension.Forms;
using NthDimension.Forms.Events;
using NthStudio.Gui.Widgets.TextEditor.Document;
using NthStudio.Gui.Widgets.TextEditor.Document.HighlightStrategy;
using NthStudio.Gui.Widgets.TextEditor.Document.Selection;
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
    public class GutterMargin : AbstractMargin, IDisposable
    {
        StringFormat numberStringFormat = (StringFormat)StringFormat.GenericTypographic.Clone();

        public static NanoCursor RightLeftCursor;

        static GutterMargin()
        {
            RightLeftCursor = Cursors.ArrowRight; // new Cursor(cursorStream);
        }

        public int RightOffset
        {
            get;
            set;
        }

        public void Dispose()
        {
            numberStringFormat.Dispose();
        }

        public override NanoCursor Cursor
        {
            get
            {
                return RightLeftCursor;
            }
        }

        public override Size Size
        {
            get
            {
                return new Size(
                    (int)(TextCanvas.TextView.WideSpaceWidth
                    * Math.Max(3, (int)(Math.Log10(TextCanvas.Document.TotalNumberOfLines) + 1))) + 2 + RightOffset, -1);
            }
        }

        public override bool IsVisible
        {
            get
            {
                return TextCanvas.TextEditorProperties.ShowLineNumbers;
            }
        }

        public GutterMargin(TextCanvas pTextCanvas)
            : base(pTextCanvas)
        {
            numberStringFormat.LineAlignment = StringAlignment.Far;
            numberStringFormat.FormatFlags = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.FitBlackBox |
            StringFormatFlags.NoWrap | StringFormatFlags.NoClip;
            RightOffset = 3;
        }

        public override void Paint(GContext gc, Rectangle rect)
        {
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                return;
            }
            Color numberColor;

            HighlightColor lineNumberPainterColor = TextCanvas.Document.HighlightingStrategy.GetColorFor("LineNumbers");
            Color lineNumberColor = lineNumberPainterColor.Color;
            Color resaltLineNumberColor = Utilities.ColorUtil.ChangeColorBrightness(lineNumberColor, 0.3f);

            int fontHeight = TextCanvas.TextView.LineHeight;
            //Brush fillBrush = TextCanvas.Enabled ? BrushRegistry.GetBrush(lineNumberPainterColor.BackgroundColor) : SystemBrushes.InactiveBorder;
            //TextColor fillColor = TextColor.Pink; //TextCanvas.Enabled ? lineNumberPainterColor.BackgroundColor : SystemColors.InactiveBorder;
            Color fillColor = iTextCanvas.Document.HighlightingStrategy.GetColorFor("Default").BackgroundColor;
            fillColor = Utilities.ColorUtil.ChangeColorBrightness(fillColor, 0.03f);

            //Brush drawBrush = BrushRegistry.GetBrush(lineNumberPainterColor.TextColor);
            for (int y = 0; y < (DrawingPosition.Height + TextCanvas.TextView.VisibleLineDrawingRemainder) / fontHeight + 1; ++y)
            {
                int ypos = drawingPosition.Y + (fontHeight * y) - TextCanvas.TextView.VisibleLineDrawingRemainder;
                int tAscender = (int)Math.Ceiling(iTextCanvas.Font.Ascender);
                int textypos = ypos + tAscender;

                var backgroundRectangle = new Rectangle(drawingPosition.X, ypos, drawingPosition.Width, fontHeight);

                if (rect.IntersectsWith(backgroundRectangle))
                {
                    gc.FillRectangle(new NanoSolidBrush(fillColor) /*fillBrush*/, backgroundRectangle);
                    int curLine = TextCanvas.Document.GetFirstLogicalLine(TextCanvas.Document.GetVisibleLine(TextCanvas.TextView.FirstVisibleLine) + y);
                    Rectangle numTextRec = backgroundRectangle;
                    // Ajusta el valor de X para justificar a la derecha los números de línea
                    string ln = (curLine + 1).ToString();
                    int ws = (int)(iTextCanvas.WindowHUD as IUiContext).MeasureTextWidth(ln, lineNumberPainterColor.GetFont(TextEditorProperties));
                    numTextRec.X += Size.Width - ws - RightOffset;
                    numTextRec.Y = textypos;
                    if (curLine < TextCanvas.Document.TotalNumberOfLines)
                    {
                        NanoSolidBrush sbrush;
                        if (curLine == TextCanvas.Caret.Line)
                            sbrush = new NanoSolidBrush(resaltLineNumberColor);
                        else
                            sbrush = new NanoSolidBrush(lineNumberColor);

                        gc.DrawString(ln,
                                      lineNumberPainterColor.GetFont(TextEditorProperties),
                                      sbrush, // drawBrush,
                                      numTextRec.X, numTextRec.Y);
                        //numberStringFormat);
                    }
                }
            }
        }

        public override void HandleMouseDown(Point mousepos, MouseButton mouseButtons)
        {
            TextLocation selectionStartPos;

            TextCanvas.SelectionManager.selectFrom.where = WhereFrom.Gutter;
            int realline = TextCanvas.TextView.GetLogicalLine(mousepos.Y);

            if (realline >= 0 && realline < TextCanvas.Document.TotalNumberOfLines)
            {
                // shift-select
                if ((Widget.ModifierKeys & Keys.Shift) != 0)
                {
                    if (!TextCanvas.SelectionManager.HasSomethingSelected
                        && realline != TextCanvas.Caret.Position.Y)
                    {
                        if (realline >= TextCanvas.Caret.Position.Y)
                        { // at or below starting selection, place the cursor on the next line
                          // nothing is selected so make a new selection from cursor
                            selectionStartPos = TextCanvas.Caret.Position;
                            // whole line selection - start of line to start of next line
                            if (realline < TextCanvas.Document.TotalNumberOfLines - 1)
                            {
                                TextCanvas.SelectionManager.SetSelection(new DefaultSelection(TextCanvas.Document, selectionStartPos, new TextLocation(0, realline + 1)));
                                TextCanvas.Caret.Position = new TextLocation(0, realline + 1);
                            }
                            else
                            {
                                TextCanvas.SelectionManager.SetSelection(new DefaultSelection(TextCanvas.Document, selectionStartPos,
                                                                                              new TextLocation(TextCanvas.Document.GetLineSegment(realline).Length + 1, realline)));
                                TextCanvas.Caret.Position = new TextLocation(TextCanvas.Document.GetLineSegment(realline).Length + 1, realline);
                            }
                        }
                        else
                        { // prior lines to starting selection, place the cursor on the same line as the new selection
                          // nothing is selected so make a new selection from cursor
                            selectionStartPos = TextCanvas.Caret.Position;
                            // whole line selection - start of line to start of next line
                            TextCanvas.SelectionManager.SetSelection(new DefaultSelection(TextCanvas.Document, selectionStartPos, new TextLocation(selectionStartPos.X, selectionStartPos.Y)));
                            TextCanvas.SelectionManager.ExtendSelection(new TextLocation(selectionStartPos.X, selectionStartPos.Y), new TextLocation(0, realline));
                            TextCanvas.Caret.Position = new TextLocation(0, realline);
                        }
                    }
                    else
                    {
                        // let MouseMove handle a shift-click in a gutter
                        MouseEventArgs e = new MouseEventArgs(mouseButtons, 1, mousepos.X, mousepos.Y, 0);
                        TextCanvas.RaiseMouseMove(e);
                    }
                }
                else
                { // this is a new selection with no shift-key
                  // sync the textareamousehandler mouse location
                  // (fixes problem with clicking out into a menu then back to the gutter whilst
                  // there is a selection)
                    TextCanvas.mousepos = mousepos;

                    selectionStartPos = new TextLocation(0, realline);
                    TextCanvas.SelectionManager.ClearSelection();
                    // whole line selection - start of line to start of next line
                    if (realline < TextCanvas.Document.TotalNumberOfLines - 1)
                    {
                        TextCanvas.SelectionManager.SetSelection(
                            new DefaultSelection(TextCanvas.Document,
                                                 selectionStartPos,
                                                 new TextLocation(selectionStartPos.X,
                                                                  selectionStartPos.Y + 1)));
                        TextCanvas.Caret.Position =
                            new TextLocation(selectionStartPos.X, selectionStartPos.Y + 1);
                    }
                    else
                    {
                        TextCanvas.SelectionManager.SetSelection(
                            new DefaultSelection(TextCanvas.Document,
                                                 new TextLocation(0, realline),
                                                 new TextLocation(
                                                     TextCanvas.Document.GetLineSegment(realline).Length + 1,
                                                     selectionStartPos.Y)));
                        TextCanvas.Caret.Position =
                            new TextLocation(
                            TextCanvas.Document.GetLineSegment(realline).Length + 1,
                            selectionStartPos.Y);
                    }
                }
            }
        }
    }
}
