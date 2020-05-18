using NthDimension.Forms;
using NthDimension.Forms.Events;
using NthDimension.Rasterizer.NanoVG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Gui.Widgets
{
    public class Slider : Widget
    {
        public Slider()
        {

        }

        protected override void DoPaint(PaintEventArgs e)
        {
            NVGcontext vg = StudioWindow.vg;
            NVGpaint bg, knob;

            float pos = .5f;
            float x = Parent.Location.X + Location.X;
            float y = Parent.Location.Y + Location.Y;
            float w = Size.Width;
            float h = Size.Height;

            float cy = y + (int)(h * 0.5f);
            float kr = (int)(h * 0.25f);

            NanoVG.nvgSave(vg);
            //	nvgClearState(vg);

            // Slot
            bg = NanoVG.nvgBoxGradient(vg, x, cy - 2 + 1, w, 4, 2, 2, NanoVG.nvgRGBA(0, 0, 0, 32), NanoVG.nvgRGBA(0, 0, 0, 128));
            NanoVG.nvgBeginPath(vg);
            NanoVG.nvgRoundedRect(vg, x, cy - 2, w, 4, 2);
            NanoVG.nvgFillPaint(vg, bg);
            NanoVG.nvgFill(vg);

            // Knob Shadow
            bg = NanoVG.nvgRadialGradient(vg, x + (int)(pos * w), cy + 1, kr - 3, kr + 3, NanoVG.nvgRGBA(0, 0, 0, 64), NanoVG.nvgRGBA(0, 0, 0, 0));
            NanoVG.nvgBeginPath(vg);
            NanoVG.nvgRect(vg, x + (int)(pos * w) - kr - 5, cy - kr - 5, kr * 2 + 5 + 5, kr * 2 + 5 + 5 + 3);
            NanoVG.nvgCircle(vg, x + (int)(pos * w), cy, kr);
            NanoVG.nvgPathWinding(vg, (int)NVGsolidity.NVG_HOLE);
            NanoVG.nvgFillPaint(vg, bg);
            NanoVG.nvgFill(vg);

            // Knob
            knob = NanoVG.nvgLinearGradient(vg, x, cy - kr, x, cy + kr, NanoVG.nvgRGBA(255, 255, 255, 16), NanoVG.nvgRGBA(0, 0, 0, 16));
            NanoVG.nvgBeginPath(vg);
            NanoVG.nvgCircle(vg, x + (int)(pos * w), cy, kr - 1);
            NanoVG.nvgFillColor(vg, NanoVG.nvgRGBA(40, 43, 48, 255));
            NanoVG.nvgFill(vg);
            NanoVG.nvgFillPaint(vg, knob);
            NanoVG.nvgFill(vg);

            NanoVG.nvgBeginPath(vg);
            NanoVG.nvgCircle(vg, x + (int)(pos * w), cy, kr - 0.5f);
            NanoVG.nvgStrokeColor(vg, NanoVG.nvgRGBA(0, 0, 0, 92));
            NanoVG.nvgStroke(vg);

            NanoVG.nvgRestore(vg);

            base.DoPaint(e);
        }
    }
}
