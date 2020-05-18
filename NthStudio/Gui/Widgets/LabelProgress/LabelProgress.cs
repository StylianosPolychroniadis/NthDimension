using NthDimension.Forms;
using System.Drawing;

namespace NthStudio.Gui.Widgets
{
    public class LabelProgress : Label
    {
        public float ProgressPerCent = 0f;
        public Color ProgressColor = Color.White;


        public LabelProgress(string text) : base(text) { }
        public LabelProgress(string text, Color color) : this(text)
        {
            this.ProgressColor = color;
        }

        protected override void OnPaintBackground(GContext gc)
        {

            base.OnPaintBackground(gc);
            
            float pWidth = (ProgressPerCent * this.Width) / 100f;
            gc.FillRectangle(new SolidBrush(ProgressColor), new Rectangle(0, 0, (int)pWidth, Height));
            
        }

    }
}
