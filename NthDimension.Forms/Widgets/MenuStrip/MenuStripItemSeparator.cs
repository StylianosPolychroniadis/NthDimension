using System.Drawing;
using NthDimension.Forms.Events;

namespace NthDimension.Forms.Widgets
{
    public class MenuStripItemSeparator : Widget
    {
        public MenuStripItemSeparator()
        {
            Size = new Size(0, 8);
            Dock = EDocking.Top;
        }

        protected override void OnMouseDown(MouseDownEventArgs e)
        {
            e.FocusedLostFocusOnMouseDown = false;

            base.OnMouseDown(e);
        }

        protected override void DoPaint(PaintEventArgs e)
        {
            base.DoPaint(e);

            GContext gc = e.GC;

            if (Parent != null)
            {
                if (Parent is MenuStrip)
                    // Línea vertical
                {

                }
                else if (Parent is ContextMenuStrip)
                    // Línea horizontal
                {
                    gc.DrawLine(new NanoPen(Color.FromArgb(29, 39, 66)),
                        0, 3, Width, 3);
                    gc.DrawLine(new NanoPen(Color.FromArgb(72, 75, 85)),
                        0, 4, Width, 4);
                }
            }
        }
    }
}
