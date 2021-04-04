using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NthDimension.Forms.Events;

namespace NthDimension.Forms.Widgets
{
    public class SplitterBox : Widget
    {
        ESplitterType splitterType;
        // panel Left or Top
        Widget panelLT;
        // panel Right or Bottom
        Widget panelRB;
        int markersCount;
        int halfSplitterSize;
        int dividerBarPos;
        readonly NanoSolidBrush sbrush;

        public SplitterBox(ESplitterType splitterType)
        {
            PaintBackGround = false;
            Name = "SplitBox";

            sbrush = new NanoSolidBrush(BGColor);

            this.splitterType = splitterType;
            SplitterSize = 6;

            panelLT = new Widget();
            panelLT.PaintBackGround = false;
            if (splitterType == ESplitterType.HorizontalScroll)
                panelLT.Dock = EDocking.Left;
            else
                panelLT.Dock = EDocking.Top;
            panelLT.Name = "panel0-LT";

            panelRB = new Widget();
            panelRB.PaintBackGround = false;
            if (splitterType == ESplitterType.HorizontalScroll)
                panelRB.Dock = EDocking.Right;
            else
                panelRB.Dock = EDocking.Bottom;
            panelRB.Name = "panel1_RB";

            SplitterBarLocation = 0.5f;

            Widgets.Add(panelLT);
            Widgets.Add(panelRB);
        }
        #region Properties

        /// <summary>
        /// Gets the panel0. Panel Izquierdo o Superior
        /// </summary>
        /// <value>The panel0.</value>
        public Widget Panel0
        {
            get { return panelLT; }
        }

        /// <summary>
        /// Gets the panel1. Panel Derecho o Inferior
        /// </summary>
        /// <value>The panel1.</value>
        public Widget Panel1
        {
            get { return panelRB; }
        }

        int SplitterSize_;

        public int SplitterSize
        {
            get { return SplitterSize_; }
            set
            {
                if (value < 4)
                    value = 4;
                if (SplitterSize_ == value)
                    return;
                SplitterSize_ = value;
                halfSplitterSize = (int)(value * 0.5f);
            }
        }

        float SplitterBarLocation_;

        public float SplitterBarLocation
        {
            get { return SplitterBarLocation_; }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 1f)
                    value = 1f;
                SplitterBarLocation_ = value;
                PerformLayout();
            }
        }

        #endregion Properties

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            if (splitterType == ESplitterType.HorizontalScroll)
                Cursors.Cursor = Cursors.HSplit;
            else
                Cursors.Cursor = Cursors.VSplit;
        }
        /*
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            Cursors.Cursor = Cursors.Default;
        }
         */

        bool mouseDown;

        protected override void OnMouseDown(MouseDownEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButton.Left)
            {
                Focus();
                mouseDown = true;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            mouseDown = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (MouseFocusCaptured == false || !mouseDown)
                return;

            Point local = e.Location;

            // (local.X - halfSplitterSize) para que el cursor se mantenga en el centro de la barra de desplazamiento

            //TopLevelWindow.Title = String.Format("localX = {0}", local.X);

            if (splitterType == ESplitterType.HorizontalScroll)
                SplitterBarLocation = (float)(local.X - halfSplitterSize) / (Width - SplitterSize);
            else
                SplitterBarLocation = (float)(local.Y - halfSplitterSize) / (Height - SplitterSize);
        }

        void UpdateSplitter()
        {
            if (panelLT == null || panelRB == null)
                return;
            int markerItem;

            if (splitterType == ESplitterType.HorizontalScroll)
            {
                markersCount = (int)System.Math.Ceiling((float)Width / SplitterSize);
                markerItem = (int)System.Math.Ceiling(SplitterBarLocation * (markersCount - 1));

                dividerBarPos = markerItem * SplitterSize;

                if (dividerBarPos > Width - SplitterSize)
                    dividerBarPos = Width - SplitterSize;

                panelLT.Size = new Size(dividerBarPos, Height);

                panelRB.Size = new Size(Width - dividerBarPos - SplitterSize, Height);
            }
            else // splitterType == ESplitterType.VerticalScroll
            {
                markersCount = (int)System.Math.Ceiling((float)Height / SplitterSize);
                markerItem = (int)System.Math.Ceiling(SplitterBarLocation * (markersCount - 1));

                dividerBarPos = markerItem * SplitterSize;

                if (dividerBarPos > Height - SplitterSize)
                    dividerBarPos = Height - SplitterSize;

                panelLT.Size = new Size(Width, dividerBarPos);

                panelRB.Size = new Size(Width, Height - dividerBarPos - SplitterSize);
            }
        }

        protected override void OnLayout()
        {
            base.OnLayout();

            UpdateSplitter();
        }

        protected override void OnPaintBackground(GContext gc)
        {
            if (Widgets.Count <= 2)
                base.OnPaintBackground(gc);
        }

        float[] dashValues = new float[] { 2, 3 };

        protected override void DoPaint(PaintEventArgs e)
        {
            base.DoPaint(e);

            int mid1 = 0;
            int mid2 = 0;

            if (splitterType == ESplitterType.HorizontalScroll)
                mid1 = Height / 2;
            else
                mid1 = Width / 2;

            mid2 = dividerBarPos + (SplitterSize / 2) - 1;
            sbrush.Color = Color.FromArgb(82, 82, 82); // TextColor.DarkGray;
            //widPen.DashPattern = dashValues;

            if (splitterType == ESplitterType.HorizontalScroll)
            {
                e.GC.FillRectangle(sbrush, dividerBarPos, 0, SplitterSize, Height);
                widPen.Color = Color.WhiteSmoke;
                e.GC.DrawLine(widPen, mid2, mid1 - 16, mid2, mid1 + 16);
                mid2++;
                mid1++;
                widPen.Color = Color.FromArgb(42, 42, 42);
                e.GC.DrawLine(widPen, mid2, mid1 - 16, mid2, mid1 + 16);
            }
            else
            {
                e.GC.FillRectangle(sbrush, 0, dividerBarPos, Width, SplitterSize);
                widPen.Color = Color.WhiteSmoke;
                e.GC.DrawLine(widPen, mid1 - 16, mid2, mid1 + 16, mid2);
                mid2++;
                mid1++;
                widPen.Color = Color.FromArgb(42, 42, 42);
                e.GC.DrawLine(widPen, mid1 - 16, mid2, mid1 + 16, mid2);
            }
        }
    }
}
