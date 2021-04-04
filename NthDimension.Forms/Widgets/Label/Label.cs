using System;
using System.Drawing;

using NthDimension.Forms.Events;

// TODO:: Fix Namespace. Should be NthDimension.Forms.Widgets
namespace NthDimension.Forms
{
    public class Label : Widget
    {
        private String              m_text              = string.Empty;
        private ETextAlignment      m_textAlign;
        private int                 m_textXOffset;
        protected Point             m_textLocation      = new Point();
        private bool                m_updating          = false;

        #region Properties
        public Size AutoSize
        {
            get;
            private set;
        }
        public override NanoFont Font
        {
            get { return base.Font; }
            set
            {
                base.Font = value;
                UpdateLabel();
            }
        }
        /// <summary>
        /// Text to display
        /// </summary>
        public virtual String Text
        {
            get { return m_text; }
            set
            {
                if (m_text == value)
                    return;

                m_text = value;

                UpdateLabel();
            }
        }
        ///// <summary>
        ///// Text color.
        ///// </summary>
        //public Color FGColor
        //{
        //    get { return base.FGColor; }
        //    set
        //    {
        //        base.FGColor = value;
        //    }
        //}
        public ETextAlignment TextAlign
        {
            get { return m_textAlign; }
            set
            {
                if (m_textAlign == value)
                    return;
                m_textAlign = value;
                UpdateLabel();
            }
        }
        public Size TextMeasure
        {
            get;
            private set;
        }
        protected int TextXOffset
        {
            get { return m_textXOffset; }
            set { m_textXOffset = value; }
        }

        public Point TextLocation
        {
            get { return m_textLocation; }
        }
        #endregion

        #region Ctor/Dtor
        public Label(string text = null)
        {
            m_text = text;
            FGColor = Color.Black;
            BGColor = /*Color.WhiteSmoke;*/ Themes.ThemeEngine.BackColor;
            Location = new Point(10, 20);
            Size = new Size(10, 10);
            TextAlign = ETextAlignment.Left | ETextAlignment.CenterV;
            PaintBackGround = false;
        }
        ~Label()
        {
            // Nothing to do here
        }

        #endregion

        protected void UpdateLabel()
        {
            m_updating = true;

            if (String.IsNullOrEmpty(Text))
                return;

            TextMeasure = WHUD.LibContext.MeasureText(Text, Font);

            if (String.IsNullOrEmpty(Text) == false)
            {
                //if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    //TextXOffset = -1;
                    //	Size = new Size((int)TextMeasure.Width + 9, (int)TextMeasure.Height);
                }
                //else
                AutoSize = new Size(TextMeasure.Width + Padding.Left + Padding.Right,
                                TextMeasure.Height + Padding.Top + Padding.Bottom);
            }

            int nWidth = Width, nHeight = Height;

            m_textLocation.X = m_textLocation.Y = 0;
            int mw;

            if (0 != (TextAlign & ETextAlignment.Right))
            {
                mw = System.Math.Max(TextMeasure.Width, Width);
                m_textLocation.X = (int)System.Math.Ceiling((double)mw - TextMeasure.Width);
            }
            if (0 != (TextAlign & ETextAlignment.Bottom))
                m_textLocation.Y = (int)System.Math.Ceiling((double)nHeight - TextMeasure.Height);

            if (0 != (TextAlign & ETextAlignment.CenterH))
            {
                mw = System.Math.Max(TextMeasure.Width, Width);
                m_textLocation.X = (int)System.Math.Ceiling(((mw - TextMeasure.Width) * 0.5f));
            }

            if (0 != (TextAlign & ETextAlignment.CenterV))
            {
                int mh = System.Math.Max(TextMeasure.Height, Height);

                m_textLocation.Y = (int)System.Math.Ceiling(((mh - TextMeasure.Height) * 0.5f)); // + Font.Descender);
            }
            m_updating = false;

            ParentPerformLayout();
        }

        protected override void OnPaddingChanged()
        {
            base.OnPaddingChanged();

            UpdateLabel();
        }

        protected override void OnLayout()
        {
            base.OnLayout();

            UpdateLabel();
        }

        protected override void OnSizeChanged()
        {

            base.OnSizeChanged();

            if (m_updating)
                return;
            UpdateLabel();
        }

        protected override void DoPaint(PaintEventArgs e)
        {
            int tAscender = (int)System.Math.Ceiling(Font.Ascender);

            e.GC.DrawString(Text, new NanoSolidBrush(FGColor),
                            m_textLocation.X + TextXOffset,
                            m_textLocation.Y + tAscender);
        }

        public override string ToString()
        {
            return string.Format("[Label Text_={0}]", m_text);
        }

    }
}
