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
    public class ColorWheel : Widget
    {
        private bool m_init = false;

        public ColorWheel()
        {

        }

        private void InititalizeComponent()
        {

        }

        protected override void DoPaint(PaintEventArgs e)
        {
            NVGcontext vg = StudioWindow.vg;
            float x = Parent.Location.X + Location.X;
            float y = Parent.Location.Y + Location.Y;
            float w = Size.Width;
            float h = Size.Height;

            float t = 0;


            int i;
            float r0, r1, ax, ay, bx, by, cx, cy, aeps, r;
            float hue = (float)Math.Sin(t * 0.12f);
            NVGpaint paint;

            NanoVG.nvgSave(vg);

            /*	nvgBeginPath(vg);
			nvgRect(vg, x,y,w,h);
			nvgFillColor(vg, nvgRGBA(255,0,0,128));
			nvgFill(vg);*/

            cx = x + w * 0.5f;
            cy = y + h * 0.5f;
            r1 = (w < h ? w : h) * 0.5f - 5.0f;
            r0 = r1 - 20.0f;
            aeps = 0.5f / r1;   // half a pixel arc length in radians (2pi cancels out).

            for (i = 0; i < 6; i++)
            {
                float a0 = (float)i / 6.0f * NanoVG.NVG_PI * 2.0f - aeps;
                float a1 = (float)(i + 1.0f) / 6.0f * NanoVG.NVG_PI * 2.0f + aeps;
                NanoVG.nvgBeginPath(vg);
                NanoVG.nvgArc(vg, cx, cy, r0, a0, a1, (int)NVGwinding.NVG_CW);
                NanoVG.nvgArc(vg, cx, cy, r1, a1, a0, (int)NVGwinding.NVG_CCW);
                NanoVG.nvgClosePath(vg);
                ax = cx + (float)Math.Cos(a0) * (r0 + r1) * 0.5f;
                ay = cy + (float)Math.Sin(a0) * (r0 + r1) * 0.5f;
                bx = cx + (float)Math.Cos(a1) * (r0 + r1) * 0.5f;
                by = cy + (float)Math.Sin(a1) * (r0 + r1) * 0.5f;
                paint = NanoVG.nvgLinearGradient(vg, ax, ay, bx, by,
                                                 NanoVG.nvgHSLA(a0 / (NanoVG.NVG_PI * 2), 1.0f, 0.55f, 255),
                                                 NanoVG.nvgHSLA(a1 / (NanoVG.NVG_PI * 2), 1.0f, 0.55f, 255));
                NanoVG.nvgFillPaint(vg, paint);
                NanoVG.nvgFill(vg);
            }

            NanoVG.nvgBeginPath(vg);
            NanoVG.nvgCircle(vg, cx, cy, r0 - 0.5f);
            NanoVG.nvgCircle(vg, cx, cy, r1 + 0.5f);
            NanoVG.nvgStrokeColor(vg, NanoVG.nvgRGBA(0, 0, 0, 64));
            NanoVG.nvgStrokeWidth(vg, 1.0f);
            NanoVG.nvgStroke(vg);

            // Selector
            NanoVG.nvgSave(vg);
            NanoVG.nvgTranslate(vg, cx, cy);
            NanoVG.nvgRotate(vg, hue * NanoVG.NVG_PI * 2);

            // Marker on
            NanoVG.nvgStrokeWidth(vg, 2.0f);
            NanoVG.nvgBeginPath(vg);
            NanoVG.nvgRect(vg, r0 - 1, -3, r1 - r0 + 2, 6);
            NanoVG.nvgStrokeColor(vg, NanoVG.nvgRGBA(255, 255, 255, 192));
            NanoVG.nvgStroke(vg);

            paint = NanoVG.nvgBoxGradient(vg, r0 - 3, -5, r1 - r0 + 6, 10, 2, 4,
                                          NanoVG.nvgRGBA(0, 0, 0, 128), NanoVG.nvgRGBA(0, 0, 0, 0));
            NanoVG.nvgBeginPath(vg);
            NanoVG.nvgRect(vg, r0 - 2 - 10, -4 - 10, r1 - r0 + 4 + 20, 8 + 20);
            NanoVG.nvgRect(vg, r0 - 2, -4, r1 - r0 + 4, 8);
            NanoVG.nvgPathWinding(vg, (int)NVGsolidity.NVG_HOLE);
            NanoVG.nvgFillPaint(vg, paint);
            NanoVG.nvgFill(vg);

            // Center triangle
            r = r0 - 6;
            ax = (float)Math.Cos(120.0f / 180.0f * NanoVG.NVG_PI) * r;
            ay = (float)Math.Sin(120.0f / 180.0f * NanoVG.NVG_PI) * r;
            bx = (float)Math.Cos(-120.0f / 180.0f * NanoVG.NVG_PI) * r;
            by = (float)Math.Sin(-120.0f / 180.0f * NanoVG.NVG_PI) * r;
            NanoVG.nvgBeginPath(vg);
            NanoVG.nvgMoveTo(vg, r, 0);
            NanoVG.nvgLineTo(vg, ax, ay);
            NanoVG.nvgLineTo(vg, bx, by);
            NanoVG.nvgClosePath(vg);
            paint = NanoVG.nvgLinearGradient(vg, r, 0, ax, ay,
                                             NanoVG.nvgHSLA(hue, 1.0f, 0.5f, 255), NanoVG.nvgRGBA(255, 255, 255, 255));
            NanoVG.nvgFillPaint(vg, paint);
            NanoVG.nvgFill(vg);
            paint = NanoVG.nvgLinearGradient(vg, (r + ax) * 0.5f, (0 + ay) * 0.5f, bx, by,
                                             NanoVG.nvgRGBA(0, 0, 0, 0), NanoVG.nvgRGBA(0, 0, 0, 255));
            NanoVG.nvgFillPaint(vg, paint);
            NanoVG.nvgFill(vg);
            NanoVG.nvgStrokeColor(vg, NanoVG.nvgRGBA(0, 0, 0, 64));
            NanoVG.nvgStroke(vg);

            // Select circle on triangle
            ax = (float)Math.Cos(120.0f / 180.0f * NanoVG.NVG_PI) * r * 0.3f;
            ay = (float)Math.Sin(120.0f / 180.0f * NanoVG.NVG_PI) * r * 0.4f;
            NanoVG.nvgStrokeWidth(vg, 2.0f);
            NanoVG.nvgBeginPath(vg);
            NanoVG.nvgCircle(vg, ax, ay, 5);
            NanoVG.nvgStrokeColor(vg, NanoVG.nvgRGBA(255, 255, 255, 192));
            NanoVG.nvgStroke(vg);

            paint = NanoVG.nvgRadialGradient(vg, ax, ay, 7, 9,
                                             NanoVG.nvgRGBA(0, 0, 0, 64), NanoVG.nvgRGBA(0, 0, 0, 0));
            NanoVG.nvgBeginPath(vg);
            NanoVG.nvgRect(vg, ax - 20, ay - 20, 40, 40);
            NanoVG.nvgCircle(vg, ax, ay, 7);
            NanoVG.nvgPathWinding(vg, (int)NVGsolidity.NVG_HOLE);
            NanoVG.nvgFillPaint(vg, paint);
            NanoVG.nvgFill(vg);

            NanoVG.nvgRestore(vg);

            NanoVG.nvgRestore(vg);

            base.DoPaint(e);
        }

    }
}
