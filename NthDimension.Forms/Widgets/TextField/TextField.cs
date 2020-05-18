
using NthDimension.Forms.Events;
using System;

namespace NthDimension.Forms.Widgets
{
    [Obsolete("This TextBox does NOT accept Input")]
    /// <summary>
    /// Description of TextBox.
    /// </summary>
    public class TextField : Label
    {
        public TextField(string text = null)
            : base(text)
        {
        }

        protected override void DoPaint(PaintEventArgs e)
        {
            // base.OnPaint(e);

            e.GC.DrawEditBox(Font, Text, 0, 0, Width, Height);

            
        }
    }


}
