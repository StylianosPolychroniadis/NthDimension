using NthDimension.Forms;
using NthDimension.Forms.Events;
using NthDimension.Forms.Layout;
using NthDimension.Forms.Widgets;
using NthStudio.Gui.Widgets.TextEditor.Document;
using NthStudio.Gui.Widgets.TextEditor.Document.LineManagement;
using NthStudio.Gui.Widgets.TextEditor.Document.Selection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Gui.Widgets.TextEditor
{
    public class TextArea : Widget
    {
        ScrollBarH iScrollBarH;
        ScrollBarV iScrollBarV;
        TextCanvas iTextCanvas;
        TextEditor iTextEditor;

        #region Constructor

        public TextArea(TextEditor pTextEditor)
        {
            iTextEditor = pTextEditor;
            PaintBackGround = false;

            BGColor = Color.FromArgb(51, 51, 51);

            iScrollBarH = new ScrollBarH();
            iScrollBarH.Size = new Size(0, 13);
            iScrollBarH.Margin = new Spacing(0, 0, 13, 0);
            iScrollBarH.Dock = EDocking.Bottom;
            iScrollBarH.ThumbColor = Color.FromArgb(77, 77, 77);

            iScrollBarV = new ScrollBarV();
            iScrollBarV.Size = new Size(0, 13);
            iScrollBarV.Dock = EDocking.Right;
            iScrollBarV.ThumbColor = Color.FromArgb(77, 77, 77);

            iTextCanvas = new TextCanvas(iTextEditor, this);
            iTextCanvas.BGColor = Color.White;
            iTextCanvas.Dock = EDocking.Fill;

            iScrollBarV.ValueChanged += ScrollBarVValueChanged;
            iScrollBarH.ValueChanged += ScrollBarHValueChanged;

            Widgets.Add(iScrollBarH);
            Widgets.Add(iScrollBarV);
            Widgets.Add(iTextCanvas);

            Document.TextContentChanged += DocumentTextContentChanged;
            Document.DocumentChanged += AdjustScrollBarsOnDocumentChange;
            Document.UpdateCommited += DocumentUpdateCommitted;
        }
        #endregion Constructor

        public SelectionManager SelectionManager
        {
            get
            {
                return iTextCanvas.SelectionManager;
            }
        }

        public ITextEditorProperties TextEditorProperties
        {
            get
            {
                if (iTextEditor != null)
                    return iTextEditor.TextEditorProperties;
                return null;
            }
        }

        public ScrollBarV ScrollBarV
        {
            get
            {
                return iScrollBarV;
            }
        }

        public TextCanvas TextCanvas
        {
            get { return iTextCanvas; }
        }

        public TextEditor TextEditor
        {
            get { return iTextEditor; }
        }

        public ScrollBarH ScrollBarH
        {
            get
            {
                return iScrollBarH;
            }
        }

        public IDocument Document
        {
            get
            {
                if (iTextEditor != null)
                    return iTextEditor.Document;
                return null;
            }
        }

        public Caret Caret
        {
            get
            {
                return iTextCanvas.Caret;
            }
        }

        void DocumentTextContentChanged(object sender, EventArgs e)
        {
            // after the text content is changed abruptly, we need to validate the
            // caret position - otherwise the caret position is invalid for a short amount
            // of time, which can break client code that expects that the caret position is always valid
            Caret.ValidateCaretPos();
        }

        bool adjustScrollBarsOnNextUpdate;

        void AdjustScrollBarsOnDocumentChange(object sender, DocumentEventArgs e)
        {
            if (iTextEditor.IsInUpdate == false)
            {
                AdjustScrollBarsClearCache();
                AdjustScrollBars();
            }
            else
            {
                adjustScrollBarsOnNextUpdate = true;
            }
        }

        int[] lineLengthCache;
        const int LineLengthCacheAdditionalSize = 100;

        internal void AdjustScrollBarsClearCache()
        {
            if (lineLengthCache != null)
            {
                if (lineLengthCache.Length < this.Document.TotalNumberOfLines + 2 * LineLengthCacheAdditionalSize)
                {
                    lineLengthCache = null;
                }
                else
                {
                    Array.Clear(lineLengthCache, 0, lineLengthCache.Length);
                }
            }
        }

        internal void AdjustScrollBars()
        {
            //running on mono a bug exists whereby AdjustScrollBars is called when textarea is null
            if (iTextCanvas == null)
            {
                return;
            }

            adjustScrollBarsOnNextUpdate = false;
            iScrollBarV.Minimum = 0;
            // number of visible lines in document (folding!)
            iScrollBarV.Maximum = iTextCanvas.MaxVScrollValue;
            int max = 0;

            int firstLine = iTextCanvas.TextView.FirstVisibleLine;
            int lastLine = this.Document.GetFirstLogicalLine(iTextCanvas.TextView.FirstPhysicalLine
                               + iTextCanvas.TextView.VisibleLineCount);
            if (lastLine >= this.Document.TotalNumberOfLines)
                lastLine = this.Document.TotalNumberOfLines - 1;

            if (lineLengthCache == null || lineLengthCache.Length <= lastLine)
            {
                lineLengthCache = new int[lastLine + LineLengthCacheAdditionalSize];
            }

            for (int lineNumber = firstLine; lineNumber <= lastLine; lineNumber++)
            {
                LineSegment lineSegment = this.Document.GetLineSegment(lineNumber);

                if (Document.FoldingManager.IsLineVisible(lineNumber))
                {
                    if (lineLengthCache[lineNumber] > 0)
                    {
                        max = Math.Max(max, lineLengthCache[lineNumber]);
                    }
                    else
                    {
                        int visualLength = iTextCanvas.TextView.GetVisualColumnFast(lineSegment, lineSegment.Length);
                        lineLengthCache[lineNumber] = Math.Max(1, visualLength);
                        max = Math.Max(max, visualLength);
                    }
                }
            }
            iScrollBarH.Minimum = 0;
            iScrollBarH.Maximum = (Math.Max(max + 20, iTextCanvas.TextView.VisibleColumnCount - 1));

            iScrollBarV.LargeChange = Math.Max(0, iTextCanvas.TextView.DrawingPosition.Height);
            iScrollBarV.SmallChange = Math.Max(0, iTextCanvas.TextView.LineHeight);

            iScrollBarH.LargeChange = Math.Max(0, iTextCanvas.TextView.VisibleColumnCount - 1);
            iScrollBarH.SmallChange = Math.Max(0, (int)iTextCanvas.TextView.SpaceWidth);
        }

        public void OptionsChanged()
        {
            iTextCanvas.OptionsChanged();

            /*if (iTextCanvas.TextEditorProperties.ShowHorizontalRuler)
			{
				if (hRuler == null)
				{
					hRuler = new HRuler(iTextCanvas);
					Controls.Add(hRuler);
					ResizeTextArea();
				}
				else
				{
					hRuler.Invalidate();
				}
			}
			else
			{
				if (hRuler != null)
				{
					Controls.Remove(hRuler);
					hRuler.Dispose();
					hRuler = null;
					ResizeTextArea();
				}
			}*/

            AdjustScrollBars();
        }

        void DocumentUpdateCommitted(object sender, EventArgs e)
        {
            if (iTextEditor.IsInUpdate == false)
            {
                Caret.ValidateCaretPos();

                // OLD_CODE AdjustScrollBarsOnCommittedUpdate
                if (!scrollToPosOnNextUpdate.IsEmpty)
                {
                    ScrollTo(scrollToPosOnNextUpdate.Y, scrollToPosOnNextUpdate.X);
                }
                if (adjustScrollBarsOnNextUpdate)
                {
                    AdjustScrollBarsClearCache();
                    AdjustScrollBars();
                }
            }
        }

        void ScrollBarVValueChanged(object sender, EventArgs e)
        {
            iTextCanvas.VirtualTop = new Point(iTextCanvas.VirtualTop.X, iScrollBarV.Value);
            iTextCanvas.Invalidate();
            AdjustScrollBars();
        }

        void ScrollBarHValueChanged(object sender, EventArgs e)
        {
            iTextCanvas.VirtualTop = new Point(iScrollBarH.Value * iTextCanvas.TextView.WideSpaceWidth, iTextCanvas.VirtualTop.Y);
            iTextCanvas.Invalidate();
        }

        public void ScrollToCaret()
        {
            ScrollTo(iTextCanvas.Caret.Line, iTextCanvas.Caret.Column);
        }

        const int scrollMarginHeight = 3;

        /// <summary>
        /// Ensure that <paramref name="line"/> is visible.
        /// </summary>
        public void ScrollTo(int line)
        {
            line = Math.Max(0, Math.Min(Document.TotalNumberOfLines - 1, line));
            line = Document.GetVisibleLine(line);
            int curLineMin = iTextCanvas.TextView.FirstPhysicalLine;
            if (iTextCanvas.TextView.LineHeightRemainder > 0)
            {
                curLineMin++;
            }

            if (line - scrollMarginHeight + 3 < curLineMin)
            {
                this.ScrollBarV.Value = Math.Max(0, Math.Min(this.ScrollBarV.Maximum, (line - scrollMarginHeight + 3)
                                                             * iTextCanvas.TextView.LineHeight));
                ScrollBarVValueChanged(this, EventArgs.Empty);
            }
            else
            {
                int curLineMax = curLineMin + this.iTextCanvas.TextView.VisibleLineCount;
                if (line + scrollMarginHeight - 1 > curLineMax)
                {
                    if (this.iTextCanvas.TextView.VisibleLineCount == 1)
                    {
                        this.ScrollBarV.Value = Math.Max(0, Math.Min(this.ScrollBarV.Maximum, (line - scrollMarginHeight - 1)
                                                                     * iTextCanvas.TextView.LineHeight));
                    }
                    else
                    {
                        this.ScrollBarV.Value = Math.Min(this.ScrollBarV.Maximum,
                                                         (line - this.iTextCanvas.TextView.VisibleLineCount + scrollMarginHeight - 1)
                                                         * iTextCanvas.TextView.LineHeight);
                    }
                    ScrollBarVValueChanged(this, EventArgs.Empty);
                }
            }
        }

        Point scrollToPosOnNextUpdate;

        public void ScrollTo(int line, int column)
        {
            if (iTextEditor.IsInUpdate)
            {
                scrollToPosOnNextUpdate = new Point(column, line);
                return;
            }
            else
            {
                scrollToPosOnNextUpdate = Point.Empty;
            }

            ScrollTo(line);

            int curCharMin = (int)(this.ScrollBarH.Value - this.ScrollBarH.Minimum);
            int curCharMax = curCharMin + iTextCanvas.TextView.VisibleColumnCount;

            int pos = iTextCanvas.TextView.GetVisualColumn(line, column);

            if (iTextCanvas.TextView.VisibleColumnCount < 0)
            {
                ScrollBarH.Value = 0;
            }
            else
            {
                if (pos < curCharMin)
                {
                    ScrollBarH.Value = (int)(Math.Max(0, pos - scrollMarginHeight));
                }
                else
                {
                    if (pos > curCharMax)
                    {
                        ScrollBarH.Value = (int)Math.Max(0, Math.Min(ScrollBarH.Maximum,
                                                                     (pos - iTextCanvas.TextView.VisibleColumnCount
                                                                      + scrollMarginHeight)));
                    }
                }
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            // SD2-1072 - Make sure the caret line is valid if anyone
            // has handlers for the Enter event.
            Caret.ValidateCaretPos();

            base.OnMouseEnter(e);
        }

        protected override void DoPaint(PaintEventArgs e)
        {
            base.DoPaint(e);

            if (!ScrollBarV.IsHide && !ScrollBarH.IsHide)
            {
                Rectangle corner = new Rectangle(ScrollBarH.Right, ScrollBarV.Bottom,
                                                 ScrollBarV.Width, ScrollBarH.Height);
                if (e.ClipRect.IntersectsWith(corner))
                    e.GC.FillRectangle(new SolidBrush(BGColor),
                            corner);
            }
        }
    }
}
