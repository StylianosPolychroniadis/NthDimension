using NthDimension.Rasterizer.NanoVG;
using System;
using System.Drawing;

namespace NthDimension.Forms
{
    // TODO:: Look into a better Context Handling implementation and use here



    public abstract class GContext : IDisposable
    {
        protected Widget owner;
        /// <summary>
        /// Location in coordinates of "Client-rectangle of Application-Window".
        /// </summary>
        protected Point topWinPos;
        // Rectangle of clipping in coordinates of "Rectangle-Client of the Window-Application", 
        // for the Widget associated with this graphic context. topWinClipRect can represent 
        // a rectangle smaller than 'owner.Bounds'.
        protected Rectangle topWinClipRect;

        protected Rectangle ClipRect_;


        public Point TopWinPos { get { return topWinPos; } }
        #region Constructors

        protected GContext(Widget owner, Rectangle parentClipRect)
        {
            this.owner = owner;
            topWinPos = owner.LocalToWindow();
            bool checkVisibility;
            topWinClipRect = CpuScissors(new Rectangle(topWinPos, new Size(owner.Width, owner.Height)),
                                         parentClipRect, out checkVisibility);

            Point localPos = new Point(topWinClipRect.Location.X - topWinPos.X,
                                       topWinClipRect.Location.Y - topWinPos.Y);

            if (localPos.X < 0)
                localPos.X = 0;
            if (localPos.Y < 0)
                localPos.Y = 0;

            ClipRect_ = new Rectangle(localPos, new Size(topWinClipRect.Width,
                                                         topWinClipRect.Height));

            IsVisible = checkVisibility;
            BGColor_ = owner.BGColor;///////////////////
            STColor_ = owner.STColor;
            EDColor_ = owner.EDColor;
            ////STEDRect_ = owner.STEDRect;

        }
        #endregion Constructors

        public abstract GContext CreateGContext(Widget owner);

        #region Properties

        public Rectangle ScreenClipRect
        {
            get { return topWinClipRect; }
        }

        public Rectangle ClipRect
        {
            get { return ClipRect_; }
        }

        public bool IsVisible
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>The color of the background.</value>
        Color BGColor_;
        public virtual Color BGColor
        {
            get { return BGColor_; }
            set
            {
                BGColor_ = value;
            }
        }

        Color STColor_;
        public virtual Color STColor
        {
            get { return STColor_; }
            set
            {
                STColor_ = value;
            }
        }

        Color EDColor_;
        public virtual Color EDColor
        {
            get { return EDColor_; }
            set
            {
                EDColor_ = value;
            }
        }

        Rectangle STEDRect_;
        public virtual Rectangle STEDRect
        {
            get { return STEDRect_; }
            set
            {
                STEDRect_ = value;
            }
        }
        #endregion Properties

        public abstract void SetScissor();
        public abstract void SetScissor(Rectangle rect);

        public abstract void SetClip(Rectangle rect);
        protected abstract void SetClip2(Rectangle rect);

        public abstract void Clear();

        ////public abstract void Clear();

        public abstract void Clear(NanoSolidBrush sb);

        ////public abstract void Clear(LinearGradientBrush lgb);

        public virtual void DrawBezier(NanoPen pen, Point p1, Point p2, Point p3, Point p4)
        {
            Algebra.BezierPath curve = new Algebra.BezierPath();
            curve.SetControlPoints(new System.Collections.Generic.List<Algebra.Vector3>()
            {
                new Algebra.Vector3(p1.X, p1.Y, 0),
                new Algebra.Vector3(p2.X, p2.Y, 0),
                new Algebra.Vector3(p3.X, p3.Y, 0),
                new Algebra.Vector3(p4.X, p4.Y, 0),
            });

            //Algebra.Vector3[] cp = curve.GetControlPoints().ToArray();
            Algebra.Vector3[] dp = curve.GetDrawingPoints0().ToArray();

            for(int i = 1; i < dp.Length; i++)
            {
                DrawLine(pen, new PointF(dp[i-1].X, dp[i-1].Y), new PointF(dp[i].X, dp[i].Y));
            }
            
        }
        public virtual void DrawEllipse(Pen pen, float x, float y, float w, float h)
        {
            //throw new NotImplementedException();
        }

        public virtual void DrawLine(NanoPen p, PointF p1, PointF p2)
        {
            DrawLine(p, (int)p1.X, (int)p1.Y, (int)p2.X, (int)p2.Y);
        }

        public virtual void DrawLine(NanoPen p, Point p1, Point p2)
        {
            DrawLine(p, p1.X, p1.Y, p2.X, p2.Y);
        }

        public virtual void DrawLine(NanoPen p, int x1, int y1, int x2, int y2)
        {
        }

        public virtual void DrawRectangle(NanoPen p, Rectangle rect)
        {
            DrawRectangle(p, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public virtual void DrawEditBox(NanoFont font, string text, int x, int y, int w, int h, bool topWinLocation = false)
        {
        }
        public virtual void DrawSearchBox(NanoFont font, string text, int x, int y, int w, int h)
        {
        }

        public virtual void DrawImage(Image img, int x, int y)
        {
            DrawImage(img, x, y, img.Width, img.Height);
        }

        public virtual void DrawImage(Image img, Rectangle rect)
        {
            DrawImage(img, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public virtual void DrawImage(Image img, int x, int y, int width, int height)
        {
        }
        public virtual void DrawImage(int imgIndex, int x, int y, int width, int height)
        {
        }
        public virtual void DrawImageRounded(int imgIndex, int x, int y, int width, int height, float radius)
        {
        }
        public virtual void DrawImageScissored(int imgIndex, int x, int y, int width, int height)
        {
        }
        public virtual void DrawImageRoundedScissored(int imgIndex, int x, int y, int width, int height, float radius)
        {
        }

        public virtual void DrawImageCircle(Image img, int x, int y, int width, int height, int radius) { }
        public virtual void DrawImageCircle(int imgIndex, int x, int y, int width, int height, int radius) { }
        public virtual void DrawImageCircleScissored(int imgIndex, int x, int y, int width, int height, int radius) { }

        /// <summary>
        /// Draw the rectangle. Draw a rectangle with lines of a color specified by <paramref name="p"/>.
        /// </summary>
        /// <param name="p">defines the lines color.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        public virtual void DrawRectangle(NanoPen p, int x, int y, int width, int height)
        {
        }

        public virtual void DrawRoundedRect(NanoPen p, int x, int y, int width, int height, float radius)
        {
        }

        public virtual void FillBorderShadow(int x, int y, int width, int height, float cornerRadius = 0f, NanoSolidBrush sb1 = null, NanoSolidBrush sb2 = null)
        {
        }


        //public virtual void FillGradientRectangle(LinearGradientBrush brush, Rectangle rect)
        public virtual void FillGradientRectangleVG(NVGcontext vgc, float x, float y, float w, float h, NVGcolor stc, NVGcolor edc)
        {
            //FillGradientRectangle(nb, rect);
            //FillGradientRectangle(rect, linearGradientBrush);
            //FillGradientRectangle(new LinearGradientBrush(rect, stcolor, edcolor, mode));
            //FillGradientRectangle(rect, stcolor, edcolor, mode);
            //FillGradientRectangle(brush, rect);
            //FillGradientRectangle(new LinearGradientBrush(rect, Color.AntiqueWhite, Color.Aquamarine, .5f));
            NVGpaint nVGpaint = NanoVG.nvgLinearGradient(vgc, x, y, w, h, stc, edc);
            NanoVG.nvgBeginPath(vgc);
            NanoVG.nvgRect(vgc, x, y, w, h);
            NanoVG.nvgFillPaint(vgc, nVGpaint);
            NanoVG.nvgFill(vgc);
        }

        public virtual void FillRectangle(SolidBrush sb, Rectangle rect)
        {
            FillRectangle(new NanoSolidBrush(sb.Color), rect);
        }

        public virtual void FillRectangle(NanoSolidBrush sb, Rectangle rect)
        {
            FillRectangle(sb, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public virtual void FillRectangle(SolidBrush sb, int x, int y, int width, int height)
        {
            FillRectangle(new NanoSolidBrush(sb.Color), x, y, width, height);
        }

        public virtual void FillRectangle(NanoSolidBrush sb, int x, int y, int width, int height)
        {
        }

        public virtual void FillRoundedRect(NanoSolidBrush sb, int x, int y, int width, int height, float radius, bool smoothing = true)
        {
        }

        public virtual void FillEllipse(NanoSolidBrush sb, Rectangle rc)
        {
        }

        public virtual void FillPolygon(NanoSolidBrush sb, Point[] points)
        {
        }

        public virtual void FillPolygon(NanoSolidBrush sb, PointF[] points)
        {
        }

        /// <summary>
        /// Draws the string <paramref name="text"/>. X and Y, define position of baseline.
        /// </summary>
        /// <param name="text">Text.</param>
        /// <param name="sb"></param>
        /// <param name="x">The x coordinate, bottom-left, of the frame containing the text.</param>
        /// <param name="y">The y coordinate, bottom-left, of the frame containing the text.</param>
        public virtual float DrawString(string text, NanoSolidBrush sb, float x, float y)
        {
            return x;
        }
        public virtual float DrawGlyph(byte[] text, NanoSolidBrush sb, float x, float y)
        {
            return x;
        }

        public virtual float DrawString(string text, NanoFont font, NanoSolidBrush sb, float x, float y)
        {
            return x;
        }
        public virtual float DrawGlyph(byte[] text, NanoFont font, NanoSolidBrush sb, float x, float y)
        {
            return x;
        }

        public virtual float DrawString(string text, NanoFont font, NanoSolidBrush sb, int y, Rectangle rect)
        {
            return rect.X;
        }
        public virtual float DrawGlyph(byte[] text, NanoFont font, NanoSolidBrush sb, int y, Rectangle rect)
        {
            return rect.X;
        }

        public virtual void PaintButton(string text, NanoFont font, Color c, int x, int y, int width, int height,
                                        float radius = 4f, bool clicked = false)
        {
        }

        public virtual void PaintLinearGradientRect(NanoSolidBrush beginColor, NanoSolidBrush endColor, Rectangle rect)
        {
        }

        public NVGcolor ColorToNVGcolor(Color c)
        {
            var nvgc = new NVGcolor();
            nvgc.a = c.A / 255f;
            nvgc.r = c.R / 255f;
            nvgc.g = c.G / 255f;
            nvgc.b = c.B / 255f;

            return nvgc;
        }

        #region Util methods

        private Rectangle CpuScissors2(Rectangle baseRect, Rectangle parentClipRect, out bool isVisible)
        {
            Rectangle result = Rectangle.Intersect(parentClipRect, baseRect);
            isVisible = result != Rectangle.Empty;
            return result;
        }

        private Rectangle CpuScissors(Rectangle baseRect, Rectangle parentClipRect, out bool isVisible)
        {
            isVisible = true;
            Rectangle newClipRect = baseRect;

            if (newClipRect.Y < parentClipRect.Y)
            {
                int oldHeight = newClipRect.Height;
                int delta = parentClipRect.Y - newClipRect.Y;
                newClipRect.Y = parentClipRect.Y;
                newClipRect.Height -= delta;

                if (newClipRect.Height <= 0)
                {
                    isVisible = false;
                    return newClipRect;
                }
            }

            if ((newClipRect.Y + newClipRect.Height) > (parentClipRect.Y + parentClipRect.Height))
            {
                int oldHeight = newClipRect.Height;
                int delta = (newClipRect.Y + newClipRect.Height) - (parentClipRect.Y + parentClipRect.Height);

                newClipRect.Height -= delta;

                if (newClipRect.Height <= 0)
                {
                    isVisible = false;
                    return newClipRect;
                }
            }

            if (newClipRect.X < parentClipRect.X)
            {
                int oldWidth = newClipRect.Width;
                int delta = parentClipRect.X - newClipRect.X;
                newClipRect.X = parentClipRect.X;
                newClipRect.Width -= delta;

                if (newClipRect.Width <= 0)
                {
                    isVisible = false;
                    return newClipRect;
                }
            }

            if ((newClipRect.X + newClipRect.Width) > (parentClipRect.X + parentClipRect.Width))
            {
                int oldWidth = newClipRect.Width;
                int delta = (newClipRect.X + newClipRect.Width) - (parentClipRect.X + parentClipRect.Width);

                newClipRect.Width -= delta;

                if (newClipRect.Width <= 0)
                {
                    isVisible = false;
                    return newClipRect;
                }
            }
            return newClipRect;
        }
        #endregion Util methods

        #region Destruction

        // Flag: Has Dispose already been called?
        bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
            }

            // Free any unmanaged objects here.
            //
            disposed = true;

            // Call the base class implementation.
            //base.Dispose(disposing);
        }

        ~GContext()
        {
            Dispose(false);
        }

        #endregion Destruction

    }
}
