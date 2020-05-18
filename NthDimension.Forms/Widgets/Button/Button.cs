using System;
using System.Drawing;

using NthDimension.Forms.Events;

namespace NthDimension.Forms
{
    public /// <summary>
           /// Description of Button.
           /// </summary>
        class Button : Label
    {
        bool downPaint;

        public Button(string text)
            : base(text)
        {
            Font = new NanoFont(NanoFont.DefaultRegular, 12f);
            FGColor = Color.Black;
            // Azul = TextColor.FromArgb(0, 96, 128)
        }

        protected override void DoPaint(PaintEventArgs e)
        {
            e.GC.PaintButton(Text, Font, BGColor, 0, 0, Width, Height, 4f, downPaint);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            Cursors.Cursor = Cursors.AeroHand;
        }

        protected override void OnMouseDown(MouseDownEventArgs e)
        {
            base.OnMouseDown(e);

            if (!e.IsNotification)
            {
                // Esto es necesario para que se produzca el evento
                // 'MouseClickEvent'
                Focus();
                downPaint = true;
                Repaint();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (!e.IsNotification)
            {
                downPaint = false;
                Repaint();
            }
        }
    }
}
