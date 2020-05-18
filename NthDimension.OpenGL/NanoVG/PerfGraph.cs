/* LICENSE
 * Copyright (C) 2008 - 2018 SYSCON Technologies, Hellas - All Rights Reserved
 * Written by Stylianos N. Polychroniadis (info@polytronic.gr) http://www.polytronic.gr
 * 
 * This file is part of nthDimension Platform
 * 
 * WARNING! Commercial Software, All Use Must Be Licensed
 * This software is protected by Hellenic Copyright Law and International Treaties. 
 * Unauthorized use, duplication, reverse engineering, any form of redistribution, or 
 * use in part or in whole other than by prior, express, printed and signed license 
 * for use is subject to civil and criminal prosecution. 
*/

using System;
using System.Drawing;

namespace NthDimension.Rasterizer.NanoVG
{
    public class PerfGraph
    {

        public const int GRAPH_HISTORY_COUNT = 100;
        public NVGcolor FillColor = NanoVG.nvgRGBA(255, 192, 0, 128);   // ORANGE
        public float Width = 200;
        public float Height = 35;

        public string MinMaxTextFormat = string.Empty;
        //int style;
        GraphrenderStyle style;
        string name;
        float[] values;
        private float value2 = 1f;

        int head;

        public void InitGraph(GraphrenderStyle style, string name)
        {
            this.style = style;
            this.name = name;
            values = new float[GRAPH_HISTORY_COUNT];
            head = 0;
        }

        public void UpdateGraph(float fval, float vMax = 0f)
        {
            head = (head + 1) % GRAPH_HISTORY_COUNT;

            values[head] = fval;


            value2 = vMax;
        }
        public void UpdateGraph(float fval)
        {
            head = (head + 1) % GRAPH_HISTORY_COUNT;

            values[head] = fval;
        }

        public float GetGraphAverage()
        {
            int i;
            float avg = 0;
            for (i = 0; i < GRAPH_HISTORY_COUNT; i++)
            {
                avg += values[i];
            }
            return avg / (float)GRAPH_HISTORY_COUNT;
        }

        private float drawCallsMax = 0f;
        public void RenderGraph(NVGcontext vg, float x, float y)
        {
            int i;
            float avg, w, h;
            string str;

            avg = GetGraphAverage();

            w = Width;
            h = Height;

            NanoVG.nvgSave(vg);

            NanoVG.nvgBeginPath(vg);
            NanoVG.nvgRect(vg, x, y, w, h);
            NanoVG.nvgFillColor(vg, NanoVG.nvgRGBA(0, 0, 0, 128));  // Black
            NanoVG.nvgFill(vg);

            NanoVG.nvgBeginPath(vg);
            NanoVG.nvgMoveTo(vg, x, y + h);

            switch (style)
            {

                case GraphrenderStyle.GRAPH_RATE_TEXTURES:
                case GraphrenderStyle.GRAPH_RATE_GEOMETRIES:
                    for (i = 0; i < GRAPH_HISTORY_COUNT; i++)
                    {
                        float v = values[(head + i) % GRAPH_HISTORY_COUNT] * 1.0f;
                        float vx, vy;

                        if (value2 == 0) value2 = 1f;
                        if (value2 < v)
                            value2 = v;

                        vx = x + ((float)i / (GRAPH_HISTORY_COUNT - 1)) * w;
                        //vy = y + h - (((value2 - v) / value2) * (value2 / h));
                        vy = y + h - (v * (value2 / h));
                        if (vy < y)
                            vy = y;
                        NanoVG.nvgLineTo(vg, vx, vy);
                    }
                    break;
                case GraphrenderStyle.GRAPH_DRAWCALLS:
                    for (i = 0; i < GRAPH_HISTORY_COUNT; i++)
                    {
                        float v = values[(head + i) % GRAPH_HISTORY_COUNT] * 1.0f;
                        float vx, vy;

                        if (v > drawCallsMax)
                            drawCallsMax = value2 = v;
                        vx = x + ((float)i / (GRAPH_HISTORY_COUNT - 1)) * w;
                        vy = y + h - ((v / drawCallsMax) * h);
                        NanoVG.nvgLineTo(vg, vx, vy);
                    }
                    break;
                case GraphrenderStyle.GRAPH_VERTICES:
                case GraphrenderStyle.GRAPH_MATERIALS:
                case GraphrenderStyle.GRAPH_TEXTURES:
                case GraphrenderStyle.GRAPH_GEOMETRIES:
                case GraphrenderStyle.GRAPH_MINMAX:
                    for (i = 0; i < GRAPH_HISTORY_COUNT; i++)
                    {
                        float v = values[(head + i) % GRAPH_HISTORY_COUNT] * 1.0f;
                        float vx, vy;

                        if (v > value2)
                            v = value2;
                        vx = x + ((float)i / (GRAPH_HISTORY_COUNT - 1)) * w;
                        vy = y + h - ((v / value2) * h);
                        NanoVG.nvgLineTo(vg, vx, vy);
                    }
                    break;
                case GraphrenderStyle.GRAPH_RENDER_FPS:
                    {
                        value2 = 80; // 80 Frames Max
                        for (i = 0; i < GRAPH_HISTORY_COUNT; i++)
                        {
                            float v = 1.0f / (0.00001f + values[(head + i) % GRAPH_HISTORY_COUNT]);
                            float vx, vy;

                            if (v > value2)
                                v = value2;

                            vx = x + ((float)i / (GRAPH_HISTORY_COUNT - 1)) * w;
                            vy = y + h - ((v / value2) * h);
                            NanoVG.nvgLineTo(vg, vx, vy);
                        }
                        break;
                    }
                case GraphrenderStyle.GRAPH_RENDER_PERCENT:
                    {
                        {
                            for (i = 0; i < GRAPH_HISTORY_COUNT; i++)
                            {
                                float v = values[(head + i) % GRAPH_HISTORY_COUNT] * 1.0f;
                                float vx, vy;
                                if (v > 100.0f)
                                    v = 100.0f;
                                vx = x + ((float)i / (GRAPH_HISTORY_COUNT - 1)) * w;
                                vy = y + h - ((v / 100.0f) * h);
                                NanoVG.nvgLineTo(vg, vx, vy);
                            }
                        }
                        break;
                    }
                case GraphrenderStyle.GRAPH_MEGABYTES:
                    {
                        for (i = 0; i < GRAPH_HISTORY_COUNT; i++)
                        {
                            float v = values[(head + i) % GRAPH_HISTORY_COUNT];
                            float vx, vy;
                            if (v > value2)
                                v = value2;
                            vx = x + ((float)i / (GRAPH_HISTORY_COUNT - 1)) * w;
                            vy = y + h - ((v / value2) * h);
                            NanoVG.nvgLineTo(vg, vx, vy);
                        }
                    }
                    break;
                default:
                    {
                        for (i = 0; i < GRAPH_HISTORY_COUNT; i++)
                        {
                            float v = values[(head + i) % GRAPH_HISTORY_COUNT] * 1000.0f;
                            float vx, vy;
                            if (v > 100.0f)
                                v = 100.0f;
                            vx = x + ((float)i / (GRAPH_HISTORY_COUNT - 1)) * w;
                            vy = y + h - ((v / 100.0f) * h);
                            NanoVG.nvgLineTo(vg, vx, vy);
                        }
                    }
                    break;

            }
            NanoVG.nvgLineTo(vg, x + w, y + h);
            //NanoVG.nvgFillColor(vg, NanoVG.nvgRGBA(255, 192, 0, 128));  // ORANGE
            NanoVG.nvgFillColor(vg, FillColor);  // ORANGE
            NanoVG.nvgFill(vg);

            NanoVG.nvgFontFace(vg, "sans");

            if (name[0] != '\0')
            {
                NanoVG.nvgFontSize(vg, 14.0f);
                NanoVG.nvgTextAlign(vg, (int)(NVGalign.NVG_ALIGN_LEFT | NVGalign.NVG_ALIGN_TOP));
                NanoVG.nvgFillColor(vg, NanoVG.nvgRGBA(240, 240, 240, 192));    // LIGHT GREY
                NanoVG.nvgText(vg, x + 3, y + 1, name);
            }


            switch (style)
            {
                case GraphrenderStyle.GRAPH_RATE_TEXTURES:
                case GraphrenderStyle.GRAPH_RATE_GEOMETRIES:
                    {
                        NanoVG.nvgFontSize(vg, 18.0f);
                        NanoVG.nvgTextAlign(vg, (int)(NVGalign.NVG_ALIGN_RIGHT | NVGalign.NVG_ALIGN_TOP));
                        NanoVG.nvgFillColor(vg, NanoVG.nvgRGBA(240, 240, 240, 255)); // LIGHT GREY

                        //if (!String.IsNullOrEmpty(MinMaxTextFormat))
                        //    str = String.Format(MinMaxTextFormat, avg);
                        //else
                        str = String.Format("{0} MB/Frame", (int)avg);

                        NanoVG.nvgText(vg, x + w - 3, y + 1, str);

                        //NanoVG.nvgFontSize(vg, 15.0f);
                        //NanoVG.nvgTextAlign(vg, (int)(NVGalign.NVG_ALIGN_RIGHT | NVGalign.NVG_ALIGN_BOTTOM));
                        //NanoVG.nvgFillColor(vg, NanoVG.nvgRGBA(240, 240, 240, 160)); // LIGHT GREY
                        //                                                             //if (!String.IsNullOrEmpty(MinMaxTextFormat))
                        //                                                             //    str = String.Format(MinMaxTextFormat, value2);
                        //                                                             //else
                        //str = String.Format("{0}", value2);
                        //NanoVG.nvgText(vg, x + w - 3, y + h - 1, str);
                    }
                    break;
                case GraphrenderStyle.GRAPH_MATERIALS:
                case GraphrenderStyle.GRAPH_TEXTURES:
                case GraphrenderStyle.GRAPH_GEOMETRIES:
                case GraphrenderStyle.GRAPH_DRAWCALLS:
                case GraphrenderStyle.GRAPH_VERTICES:
                case GraphrenderStyle.GRAPH_MINMAX:
                    {
                        NanoVG.nvgFontSize(vg, 18.0f);
                        NanoVG.nvgTextAlign(vg, (int)(NVGalign.NVG_ALIGN_RIGHT | NVGalign.NVG_ALIGN_TOP));
                        NanoVG.nvgFillColor(vg, NanoVG.nvgRGBA(240, 240, 240, 255)); // LIGHT GREY

                        //if (!String.IsNullOrEmpty(MinMaxTextFormat))
                        //    str = String.Format(MinMaxTextFormat, avg);
                        //else
                        str = String.Format("{0}", (int)avg);

                        NanoVG.nvgText(vg, x + w - 3, y + 1, str);

                        NanoVG.nvgFontSize(vg, 15.0f);
                        NanoVG.nvgTextAlign(vg, (int)(NVGalign.NVG_ALIGN_RIGHT | NVGalign.NVG_ALIGN_BOTTOM));
                        NanoVG.nvgFillColor(vg, NanoVG.nvgRGBA(240, 240, 240, 160)); // LIGHT GREY
                                                                                     //if (!String.IsNullOrEmpty(MinMaxTextFormat))
                                                                                     //    str = String.Format(MinMaxTextFormat, value2);
                                                                                     //else
                        str = String.Format("{0}", value2);
                        NanoVG.nvgText(vg, x + w - 3, y + h - 1, str);
                    }
                    break;
                case GraphrenderStyle.GRAPH_RENDER_FPS:
                    {
                        NanoVG.nvgFontSize(vg, 18.0f);
                        NanoVG.nvgTextAlign(vg, (int)(NVGalign.NVG_ALIGN_RIGHT | NVGalign.NVG_ALIGN_TOP));
                        NanoVG.nvgFillColor(vg, NanoVG.nvgRGBA(240, 240, 240, 255)); // LIGHT GREY
                        str = String.Format("{0:0.00} FPS", 1.0f / avg);
                        NanoVG.nvgText(vg, x + w - 3, y + 1, str);

                        NanoVG.nvgFontSize(vg, 15.0f);
                        NanoVG.nvgTextAlign(vg, (int)(NVGalign.NVG_ALIGN_RIGHT | NVGalign.NVG_ALIGN_BOTTOM));
                        NanoVG.nvgFillColor(vg, NanoVG.nvgRGBA(240, 240, 240, 160)); // LIGHT GREY
                        str = String.Format("{0:0.00} ms", avg * 1000.0f);
                        NanoVG.nvgText(vg, x + w - 3, y + h - 1, str);
                    }
                    break;
                case GraphrenderStyle.GRAPH_RENDER_PERCENT:
                    {
                        NanoVG.nvgFontSize(vg, 18.0f);
                        NanoVG.nvgTextAlign(vg, (int)(NVGalign.NVG_ALIGN_RIGHT | NVGalign.NVG_ALIGN_TOP));
                        NanoVG.nvgFillColor(vg, NanoVG.nvgRGBA(240, 240, 240, 255)); // LIGHT GREY
                        str = String.Format("{0:0.0} %", avg * 1.0f);
                        NanoVG.nvgText(vg, x + w - 3, y + 1, str);
                    }
                    break;
                case GraphrenderStyle.GRAPH_MEGABYTES:
                    {
                        NanoVG.nvgFontSize(vg, 18.0f);
                        NanoVG.nvgTextAlign(vg, (int)(NVGalign.NVG_ALIGN_RIGHT | NVGalign.NVG_ALIGN_TOP));
                        NanoVG.nvgFillColor(vg, NanoVG.nvgRGBA(240, 240, 240, 255)); // LIGHT GREY
                        str = String.Format("{0:0.0} MB", avg * 1.0f);
                        NanoVG.nvgText(vg, x + w - 3, y + 1, str);

                        NanoVG.nvgFontSize(vg, 15.0f);
                        NanoVG.nvgTextAlign(vg, (int)(NVGalign.NVG_ALIGN_RIGHT | NVGalign.NVG_ALIGN_BOTTOM));
                        NanoVG.nvgFillColor(vg, NanoVG.nvgRGBA(240, 240, 240, 160)); // LIGHT GREY
                        str = String.Format("{0:0.00} MB", value2);
                        NanoVG.nvgText(vg, x + w - 3, y + h - 1, str);
                    }
                    break;
                default:
                    {
                        NanoVG.nvgFontSize(vg, 18.0f);
                        NanoVG.nvgTextAlign(vg, (int)(NVGalign.NVG_ALIGN_RIGHT | NVGalign.NVG_ALIGN_TOP));
                        NanoVG.nvgFillColor(vg, NanoVG.nvgRGBA(240, 240, 240, 255)); // LIGHT GREY
                        str = String.Format("{0:0.00} ms", avg * 1000.0f);
                        NanoVG.nvgText(vg, x + w - 3, y + 1, str);
                    }
                    break;
            }

            NanoVG.nvgRestore(vg);
        }
    }
}
