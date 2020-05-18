


using NthDimension;
using NthDimension.Forms;
using NthStudio.Gui.Widgets.TextEditor.Document.HighlightStrategy;

using System;
using System.Drawing;
using System.Drawing.Drawing2D;


namespace NthStudio.Gui.Widgets.TextEditor.Margins
{
    /// <summary>
    /// This class views the line numbers and folding markers.
    /// WARNING: Win32/64 only (references System.Drawing.Drawing2D)
    /// </summary>
    public class IconBarMargin : AbstractMargin
    {
        const int iconBarWidth = 18;

        static readonly Size iconBarSize = new Size(iconBarWidth, -1);

        public override Size Size
        {
            get
            {
                return iconBarSize;
            }
        }

        public override bool IsVisible
        {
            get
            {
                return TextCanvas.TextEditorProperties.IsIconBarVisible;
            }
        }


        public IconBarMargin(TextCanvas pTextCanvas) : base(pTextCanvas)
        {
        }

        public override void Paint(GContext gc, Rectangle rect)
        {
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                return;
            }
            // paint background
            //gc.FillRectangle(SystemBrushes.Control, new Rectangle(drawingPosition.X, rect.Top, drawingPosition.Width - 1, rect.Height));
            //gc.DrawLine(SystemPens.ControlDark, base.drawingPosition.Right - 1, rect.Top, base.drawingPosition.Right - 1, rect.Bottom);


            Color backColor = iTextCanvas.Document.HighlightingStrategy.GetColorFor("Default").BackgroundColor;
            backColor = Utilities.ColorUtil.ChangeColorBrightness(backColor, 0.03f);

            //gc.FillRectangle(new NanoSolidBrush(SystemColors.ButtonHighlight), new Rectangle(drawingPosition.X, rect.Top, drawingPosition.Width - 1, rect.Height));
            gc.FillRectangle(new NanoSolidBrush(backColor), new Rectangle(drawingPosition.X, rect.Top, drawingPosition.Width - 1, rect.Height));

            HighlightColor lineNumberPainterColor = iTextCanvas.Document.HighlightingStrategy.GetColorFor("LineNumbers");

            gc.DrawLine(new NanoPen(lineNumberPainterColor.Color), base.drawingPosition.Right - 1, rect.Top, base.drawingPosition.Right - 1, rect.Bottom);

            // paint icons
            /*foreach (Bookmark mark in textArea.Document.BookmarkManager.Marks) {
				int lineNumber = textArea.Document.GetVisibleLine(mark.LineNumber);
				int lineHeight = textArea.TextView.LineHeight;
				int yPos = (int)(lineNumber * lineHeight) - textArea.VirtualTop.Y;
				if (IsLineInsideRegion(yPos, yPos + lineHeight, rect.Y, rect.Bottom)) {
					if (lineNumber == textArea.Document.GetVisibleLine(mark.LineNumber - 1)) {
						// marker is inside folded region, do not draw it
						continue;
					}
					mark.Draw(this, g, new Point(0, yPos));
				}
			}*/
            base.Paint(gc, rect);
        }

        public override void HandleMouseDown(Point mousePos, MouseButton mouseButtons)
        {
            int clickedVisibleLine = (mousePos.Y + TextCanvas.VirtualTop.Y) / TextCanvas.TextView.LineHeight;
            int lineNumber = TextCanvas.Document.GetFirstLogicalLine(clickedVisibleLine);

            if ((mouseButtons & MouseButton.Right) == MouseButton.Right)
            {
                //if (TextCanvas.Caret.Line != lineNumber) {
                //	TextCanvas.Caret.SetLine(lineNumber);
                //}
            }

            /*IList<Bookmark> marks = textArea.Document.BookmarkManager.Marks;
			List<Bookmark> marksInLine = new List<Bookmark>();
			int oldCount = marks.Count;
			foreach (Bookmark mark in marks) {
				if (mark.LineNumber == lineNumber) {
					marksInLine.Add(mark);
				}
			}
			for (int i = marksInLine.Count - 1; i >= 0; i--) {
				Bookmark mark = marksInLine[i];
				if (mark.Click(textArea, new MouseEventArgs(mouseButtons, 1, mousePos.X, mousePos.Y, 0))) {
					if (oldCount != marks.Count) {
						textArea.UpdateLine(lineNumber);
					}
					return;
				}
			}*/
            base.HandleMouseDown(mousePos, mouseButtons);
        }

        #region Drawing functions
        public void DrawBreakpoint(GContext gc, int y, bool isEnabled, bool isHealthy)
        {
            int diameter = Math.Min(iconBarWidth - 2, TextCanvas.TextView.LineHeight);
            Rectangle rect = new Rectangle(1,
                                           y + (TextCanvas.TextView.LineHeight - diameter) / 2,
                                           diameter,
                                           diameter);


            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(rect);
                using (PathGradientBrush pthGrBrush = new PathGradientBrush(path))
                {
                    pthGrBrush.CenterPoint = new PointF(rect.Left + rect.Width / 3, rect.Top + rect.Height / 3);
                    pthGrBrush.CenterColor = Color.MistyRose;
                    Color[] colors = { isHealthy ? Color.Firebrick : Color.Olive };
                    pthGrBrush.SurroundColors = colors;

                    /*if (isEnabled) {
						gc.FillEllipse(pthGrBrush, rect);
					} else {
						gc.FillEllipse(SystemBrushes.Control, rect);
						using (Pen pen = new Pen(pthGrBrush)) {
							gc.DrawEllipse(pen, new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2));
						}
					}*/
                }
            }
        }

        public void DrawBookmark(GContext gc, int y, bool isEnabled)
        {
            int delta = TextCanvas.TextView.LineHeight / 8;
            Rectangle rect = new Rectangle(1, y + delta, base.drawingPosition.Width - 4, TextCanvas.TextView.LineHeight - delta * 2);

            /*if (isEnabled) {
				using (Brush brush = new LinearGradientBrush(new Point(rect.Left, rect.Top),
				                                             new Point(rect.Right, rect.Bottom),
				                                             TextColor.SkyBlue,
				                                             TextColor.White)) {
					FillRoundRect(g, brush, rect);
				}
			} else {
				FillRoundRect(g, Brushes.White, rect);
			}
			using (Brush brush = new LinearGradientBrush(new Point(rect.Left, rect.Top),
			                                             new Point(rect.Right, rect.Bottom),
			                                             TextColor.SkyBlue,
			                                             TextColor.Blue)) {
				using (Pen pen = new Pen(brush)) {
					DrawRoundRect(g, pen, rect);
				}
			}*/
        }

        public void DrawArrow(GContext gc, int y)
        {
            int delta = TextCanvas.TextView.LineHeight / 8;
            Rectangle rect = new Rectangle(1, y + delta, base.drawingPosition.Width - 4, TextCanvas.TextView.LineHeight - delta * 2);
            using (Brush brush = new LinearGradientBrush(new Point(rect.Left, rect.Top),
                                                         new Point(rect.Right, rect.Bottom),
                                                         Color.LightYellow,
                                                         Color.Yellow))
            {
                //FillArrow(gc, brush, rect);
            }

            using (Brush brush = new LinearGradientBrush(new Point(rect.Left, rect.Top),
                                                         new Point(rect.Right, rect.Bottom),
                                                         Color.Yellow,
                                                         Color.Brown))
            {
                using (Pen pen = new Pen(brush))
                {
                    //DrawArrow(gc, pen, rect);
                }
            }
        }

        GraphicsPath CreateArrowGraphicsPath(Rectangle r)
        {
            GraphicsPath gp = new GraphicsPath();
            int halfX = r.Width / 2;
            int halfY = r.Height / 2;
            gp.AddLine(r.X, r.Y + halfY / 2, r.X + halfX, r.Y + halfY / 2);
            gp.AddLine(r.X + halfX, r.Y + halfY / 2, r.X + halfX, r.Y);
            gp.AddLine(r.X + halfX, r.Y, r.Right, r.Y + halfY);
            gp.AddLine(r.Right, r.Y + halfY, r.X + halfX, r.Bottom);
            gp.AddLine(r.X + halfX, r.Bottom, r.X + halfX, r.Bottom - halfY / 2);
            gp.AddLine(r.X + halfX, r.Bottom - halfY / 2, r.X, r.Bottom - halfY / 2);
            gp.AddLine(r.X, r.Bottom - halfY / 2, r.X, r.Y + halfY / 2);
            gp.CloseFigure();
            return gp;
        }

        GraphicsPath CreateRoundRectGraphicsPath(Rectangle r)
        {
            GraphicsPath gp = new GraphicsPath();
            int radius = r.Width / 2;
            gp.AddLine(r.X + radius, r.Y, r.Right - radius, r.Y);
            gp.AddArc(r.Right - radius, r.Y, radius, radius, 270, 90);

            gp.AddLine(r.Right, r.Y + radius, r.Right, r.Bottom - radius);
            gp.AddArc(r.Right - radius, r.Bottom - radius, radius, radius, 0, 90);

            gp.AddLine(r.Right - radius, r.Bottom, r.X + radius, r.Bottom);
            gp.AddArc(r.X, r.Bottom - radius, radius, radius, 90, 90);

            gp.AddLine(r.X, r.Bottom - radius, r.X, r.Y + radius);
            gp.AddArc(r.X, r.Y, radius, radius, 180, 90);

            gp.CloseFigure();
            return gp;
        }

        void DrawRoundRect(Graphics g, Pen p, Rectangle r)
        {
            using (GraphicsPath gp = CreateRoundRectGraphicsPath(r))
            {
                g.DrawPath(p, gp);
            }
        }

        void FillRoundRect(Graphics g, Brush b, Rectangle r)
        {
            using (GraphicsPath gp = CreateRoundRectGraphicsPath(r))
            {
                g.FillPath(b, gp);
            }
        }

        void DrawArrow(Graphics g, Pen p, Rectangle r)
        {
            using (GraphicsPath gp = CreateArrowGraphicsPath(r))
            {
                g.DrawPath(p, gp);
            }
        }

        void FillArrow(Graphics g, Brush b, Rectangle r)
        {
            using (GraphicsPath gp = CreateArrowGraphicsPath(r))
            {
                g.FillPath(b, gp);
            }
        }

        #endregion

        static bool IsLineInsideRegion(int top, int bottom, int regionTop, int regionBottom)
        {
            if (top >= regionTop && top <= regionBottom)
            {
                // Region overlaps the line's top edge.
                return true;
            }
            else if (regionTop > top && regionTop < bottom)
            {
                // Region's top edge inside line.
                return true;
            }
            return false;
        }
    }
}
