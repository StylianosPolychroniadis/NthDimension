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
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using NthDimension.Rasterizer.NanoVG.FontStashNet;
using System.Text;
using System.Collections.Generic;

namespace NthDimension.Rasterizer.NanoVG
{
    public delegate int RenderCreateHandler(object uprt);
    public delegate int RenderCreateTextureHandler(object uptr, int type, int w, int h, int imageFlags, byte[] data);
    public delegate int RenderCreateTextureHandler2(object uptr, int type, int w, int h, int imageFlags, Bitmap bmp);
    public delegate int RenderCreateTextureFromIndexHandler(object uptr, int type, int w, int h, int imageFlags, int index);
    public delegate void RenderViewportHandler(object uptr, int width, int height, float devicePixelRatio);
    public delegate void RenderFlushHandler(object uptr, NVGcompositeOperationState compositeOperation);
    public delegate void RenderFillHandler(object uptr, ref NVGpaint paint, ref NVGscissor scissor, float fringe, float[] bounds, NVGpath[] paths, int npaths);
    public delegate void RenderStrokeHandler(object uptr, ref NVGpaint paint, ref NVGscissor scissor, float fringe, float strokeWidth, NVGpath[] paths, int npaths);
    public delegate void RenderTrianglesHandler(object uptr, ref NVGpaint paint, ref NVGscissor scissor,
                                                NVGvertex[] verts, int nverts);
    public delegate int RenderUpdateTextureHandler(object uptr, int image, int x, int y, int w, int h, byte[] data);
    public delegate int RenderGetTextureSizeHandler(object uptr, int image, ref int w, ref int h);
    public delegate int RenderDeleteTexture(object uptr, int image);
    public delegate void RenderDelete(object uptr);
    public delegate void RenderCancel(object uptr);

    public static class NanoVG
    {
        public const float          NVG_PI                      = 3.14159265358979323846264338327f;

        const int                   NVG_INIT_FONTIMAGE_SIZE     = 512;
        const int                   NVG_MAX_FONTIMAGE_SIZE      = 2048;
        public const int            NVG_MAX_FONTIMAGES          = 4;

        const int                   NVG_INIT_COMMANDS_SIZE      = 256;
        const int                   NVG_INIT_POINTS_SIZE        = 128;
        const int                   NVG_INIT_PATHS_SIZE         = 16;
        const int                   NVG_INIT_VERTS_SIZE         = 256;
        public const int            NVG_MAX_STATES              = 32;
        // Length proportional to radius of a cubic bezier handle for 90deg arcs.
        const float                 NVG_KAPPA90                 = 0.5522847493f;

        //if defined NANOVG_GL2_IMPLEMENTATION
        public const int            NANOVG_GL_UNIFORMARRAY_SIZE = 11;

        public static bool nvgIsImageIcon(int value)
        {
            return (1024 > value);
        }

        public static bool nvgIsFontIcon(int value)
        {
            return (1024 <= value);
        }

        static int NVG_COUNTOF(int arr)
        {
            //return (sizeof(arr) / sizeof(0[arr]))
            throw new Exception("int NVG_COUNTOF(int arr)");
        }

        static float nvg__sqrtf(float a)
        {
            return (float)System.Math.Sqrt(a);
        }

        static float nvg__modf(float a, float b)
        {
            return a % b;
        }

        static float nvg__sinf(float a)
        {
            return (float)System.Math.Sin(a);
        }

        static float nvg__cosf(float a)
        {
            return (float)System.Math.Cos(a);
        }

        static float nvg__tanf(float a)
        {
            return (float)System.Math.Tan(a);
        }

        static float nvg__atan2f(float a, float b)
        {
            return (float)System.Math.Atan2(a, b);
        }

        static float nvg__acosf(float a)
        {
            return (float)System.Math.Acos(a);
        }

        static int nvg__mini(int a, int b)
        {
            return a < b ? a : b;
        }

        static int nvg__maxi(int a, int b)
        {
            return a > b ? a : b;
        }

        static int nvg__clampi(int a, int mn, int mx)
        {
            return a < mn ? mn : (a > mx ? mx : a);
        }

        static float nvg__minf(float a, float b)
        {
            return a < b ? a : b;
        }

        static float nvg__maxf(float a, float b)
        {
            return a > b ? a : b;
        }

        static float nvg__absf(float a)
        {
            return a >= 0.0f ? a : -a;
        }

        static float nvg__signf(float a)
        {
            return a >= 0.0f ? 1.0f : -1.0f;
        }

        static float nvg__clampf(float a, float mn, float mx)
        {
            return a < mn ? mn : (a > mx ? mx : a);
        }

        static float nvg__cross(float dx0, float dy0, float dx1, float dy1)
        {
            return dx1 * dy0 - dx0 * dy1;
        }


        static float nvg__normalize(ref float x, ref float y)
        {
            float d = nvg__sqrtf(x * x + y * y);
            if (d > 1e-6f)
            {
                float id = 1.0f / d;
                x *= id;
                y *= id;
            }
            return d;
        }

        static void nvg__deletePathCache(ref NVGpathCache c)
        {
            c.points = null;
            c.paths = null;
            c.verts = null;
        }

        static void nvg__allocPathCache(out NVGpathCache c)
        {
            c = new NVGpathCache();
            c.points = new NVGpoint[NVG_INIT_POINTS_SIZE];
            c.npoints = 0;
            c.cpoints = NVG_INIT_POINTS_SIZE;

            c.paths = new NVGpath[NVG_INIT_PATHS_SIZE];
            c.npaths = 0;
            c.cpaths = NVG_INIT_PATHS_SIZE;

            c.verts = new NVGvertex[NVG_INIT_VERTS_SIZE];
            c.nverts = 0;
            c.cverts = NVG_INIT_VERTS_SIZE;
        }

        static void nvg__setDevicePixelRatio(ref NVGcontext ctx, float ratio)
        {
            ctx.tessTol = 0.25f / ratio;
            ctx.distTol = 0.01f / ratio;
            ctx.fringeWidth = 1.0f / ratio;
            ctx.devicePxRatio = ratio;
        }

        static NVGcompositeOperationState nvg__compositeOperationState(int op)
        {
            int sfactor = 0, dfactor = 0;

            if (op == (int)NVGcompositeOperation.NVG_SOURCE_OVER)
            {
                sfactor = (int)NVGblendFactor.NVG_ONE;
                dfactor = (int)NVGblendFactor.NVG_ONE_MINUS_SRC_ALPHA;
            }
            else if (op == (int)NVGcompositeOperation.NVG_SOURCE_IN)
            {
                sfactor = (int)NVGblendFactor.NVG_DST_ALPHA;
                dfactor = (int)NVGblendFactor.NVG_ZERO;
            }
            else if (op == (int)NVGcompositeOperation.NVG_SOURCE_OUT)
            {
                sfactor = (int)NVGblendFactor.NVG_ONE_MINUS_DST_ALPHA;
                dfactor = (int)NVGblendFactor.NVG_ZERO;
            }
            else if (op == (int)NVGcompositeOperation.NVG_ATOP)
            {
                sfactor = (int)NVGblendFactor.NVG_DST_ALPHA;
                dfactor = (int)NVGblendFactor.NVG_ONE_MINUS_SRC_ALPHA;
            }
            else if (op == (int)NVGcompositeOperation.NVG_DESTINATION_OVER)
            {
                sfactor = (int)NVGblendFactor.NVG_ONE_MINUS_DST_ALPHA;
                dfactor = (int)NVGblendFactor.NVG_ONE;
            }
            else if (op == (int)NVGcompositeOperation.NVG_DESTINATION_IN)
            {
                sfactor = (int)NVGblendFactor.NVG_ZERO;
                dfactor = (int)NVGblendFactor.NVG_SRC_ALPHA;
            }
            else if (op == (int)NVGcompositeOperation.NVG_DESTINATION_OUT)
            {
                sfactor = (int)NVGblendFactor.NVG_ZERO;
                dfactor = (int)NVGblendFactor.NVG_ONE_MINUS_SRC_ALPHA;
            }
            else if (op == (int)NVGcompositeOperation.NVG_DESTINATION_ATOP)
            {
                sfactor = (int)NVGblendFactor.NVG_ONE_MINUS_DST_ALPHA;
                dfactor = (int)NVGblendFactor.NVG_SRC_ALPHA;
            }
            else if (op == (int)NVGcompositeOperation.NVG_LIGHTER)
            {
                sfactor = (int)NVGblendFactor.NVG_ONE;
                dfactor = (int)NVGblendFactor.NVG_ONE;
            }
            else if (op == (int)NVGcompositeOperation.NVG_COPY)
            {
                sfactor = (int)NVGblendFactor.NVG_ONE;
                dfactor = (int)NVGblendFactor.NVG_ZERO;
            }
            else if (op == (int)NVGcompositeOperation.NVG_XOR)
            {
                sfactor = (int)NVGblendFactor.NVG_ONE_MINUS_DST_ALPHA;
                dfactor = (int)NVGblendFactor.NVG_ONE_MINUS_SRC_ALPHA;
            }

            NVGcompositeOperationState state;
            state.srcRGB = sfactor;
            state.dstRGB = dfactor;
            state.srcAlpha = sfactor;
            state.dstAlpha = dfactor;
            return state;
        }

        static NVGstate nvg__getState(NVGcontext ctx)
        {
            return ctx.states[ctx.nstates - 1];
        }

        // State setting
        public static void nvgFontSize(NVGcontext ctx, float size)
        {
            NVGstate state = nvg__getState(ctx);
            state.fontSize = size;
        }

        public static void nvgFontBlur(NVGcontext ctx, float blur)
        {
            NVGstate state = nvg__getState(ctx);
            state.fontBlur = blur;
        }

        public static void nvgFontFace(NVGcontext ctx, string font)
        {
            NVGstate state = nvg__getState(ctx);
            state.fontId = FontStash.fonsGetFontByName(ctx.fs, font);
        }
        public static void nvgFontFace(NVGcontext ctx, int fontId)
        {
            NVGstate state = nvg__getState(ctx);
            state.fontId = fontId;
        }
        public static NVGcolor nvgRGBA(byte r, byte g, byte b, byte a)
        {
            NVGcolor color = default(NVGcolor);
            // Use longer initialization to suppress warning.
            color.r = r / 255.0f;
            color.g = g / 255.0f;
            color.b = b / 255.0f;
            color.a = a / 255.0f;

            return color;
        }

        static NVGcolor nvgRGBAf(float r, float g, float b, float a)
        {
            NVGcolor color = default(NVGcolor);
            // Use longer initialization to suppress warning.
            color.r = r;
            color.g = g;
            color.b = b;
            color.a = a;
            return color;
        }

        static float nvg__getAverageScale(float[] t)
        {
            float sx = (float)System.Math.Sqrt(t[0] * t[0] + t[2] * t[2]);
            float sy = (float)System.Math.Sqrt(t[1] * t[1] + t[3] * t[3]);
            return (sx + sy) * 0.5f;
        }

        static int nvg__curveDivs(float r, float arc, float tol)
        {
            float da = (float)System.Math.Acos(r / (r + tol)) * 2.0f;
            return nvg__maxi(2, (int)System.Math.Ceiling(arc / da));
        }

        static void nvg__buttCapStart(NVGvertex[] dst, ref int idst, NVGpoint p,
                                      float dx, float dy, float w, float d, float aa)
        {
            float px = p.x - dx * d;
            float py = p.y - dy * d;
            float dlx = dy;
            float dly = -dx;
            nvg__vset(ref dst[idst], px + dlx * w - dx * aa, py + dly * w - dy * aa, 0, 0);
            idst++;
            nvg__vset(ref dst[idst], px - dlx * w - dx * aa, py - dly * w - dy * aa, 1, 0);
            idst++;
            nvg__vset(ref dst[idst], px + dlx * w, py + dly * w, 0, 1);
            idst++;
            nvg__vset(ref dst[idst], px - dlx * w, py - dly * w, 1, 1);
            idst++;
        }

        static void nvg__roundCapStart(NVGvertex[] dst, ref int idst, NVGpoint p,
                                       float dx, float dy, float w, int ncap, float aa)
        {
            int i;
            float px = p.x;
            float py = p.y;
            float dlx = dy;
            float dly = -dx;
            //NVG_NOTUSED(aa);
            for (i = 0; i < ncap; i++)
            {
                float a = i / (float)(ncap - 1) * NVG_PI;
                float ax = (float)System.Math.Cos(a) * w, ay = (float)System.Math.Sin(a) * w;
                nvg__vset(ref dst[idst], px - dlx * ax - dx * ay, py - dly * ax - dy * ay, 0, 1);
                idst++;
                nvg__vset(ref dst[idst], px, py, 0.5f, 1);
                idst++;
            }
            nvg__vset(ref dst[idst], px + dlx * w, py + dly * w, 0, 1);
            idst++;
            nvg__vset(ref dst[idst], px - dlx * w, py - dly * w, 1, 1);
            idst++;
        }

        static void nvg__buttCapEnd(NVGvertex[] dst, ref int idst, NVGpoint p,
                                    float dx, float dy, float w, float d, float aa)
        {
            float px = p.x + dx * d;
            float py = p.y + dy * d;
            float dlx = dy;
            float dly = -dx;
            nvg__vset(ref dst[idst], px + dlx * w, py + dly * w, 0, 1);
            idst++;
            nvg__vset(ref dst[idst], px - dlx * w, py - dly * w, 1, 1);
            idst++;
            nvg__vset(ref dst[idst], px + dlx * w + dx * aa, py + dly * w + dy * aa, 0, 0);
            idst++;
            nvg__vset(ref dst[idst], px - dlx * w + dx * aa, py - dly * w + dy * aa, 1, 0);
            idst++;
        }

        static void nvg__roundCapEnd(NVGvertex[] dst, ref int idst, NVGpoint p,
                                     float dx, float dy, float w, int ncap, float aa)
        {
            int i;
            float px = p.x;
            float py = p.y;
            float dlx = dy;
            float dly = -dx;
            //NVG_NOTUSED(aa);
            nvg__vset(ref dst[idst], px + dlx * w, py + dly * w, 0, 1);
            idst++;
            nvg__vset(ref dst[idst], px - dlx * w, py - dly * w, 1, 1);
            idst++;
            for (i = 0; i < ncap; i++)
            {
                float a = i / (float)(ncap - 1) * NVG_PI;
                float ax = (float)System.Math.Cos(a) * w, ay = (float)System.Math.Sin(a) * w;
                nvg__vset(ref dst[idst], px, py, 0.5f, 1);
                idst++;
                nvg__vset(ref dst[idst], px - dlx * ax + dx * ay, py - dly * ax + dy * ay, 0, 1);
                idst++;
            }
        }

        static void nvg__roundJoin(NVGvertex[] dst, ref int idst, NVGpoint p0, NVGpoint p1,
                                   float lw, float rw, float lu, float ru, int ncap, float fringe)
        {
            int i, n;
            float dlx0 = p0.dy;
            float dly0 = -p0.dx;
            float dlx1 = p1.dy;
            float dly1 = -p1.dx;
            //NVG_NOTUSED(fringe);

            if ((p1.flags & (int)NVGpointFlags.NVG_PT_LEFT) != 0)
            {
                float lx0 = 0, ly0 = 0, lx1 = 0, ly1 = 0, a0, a1;
                nvg__chooseBevel(p1.flags & (int)NVGpointFlags.NVG_PR_INNERBEVEL, p0, p1, lw,
                    ref lx0, ref ly0, ref lx1, ref ly1);
                a0 = (float)System.Math.Atan2(-dly0, -dlx0);
                a1 = (float)System.Math.Atan2(-dly1, -dlx1);
                if (a1 > a0)
                    a1 -= NVG_PI * 2;

                nvg__vset(ref dst[idst], lx0, ly0, lu, 1);
                idst++;
                nvg__vset(ref dst[idst], p1.x - dlx0 * rw, p1.y - dly0 * rw, ru, 1);
                idst++;

                n = nvg__clampi((int)System.Math.Ceiling(((a0 - a1) / NVG_PI) * ncap), 2, ncap);
                for (i = 0; i < n; i++)
                {
                    float u = i / (float)(n - 1);
                    float a = a0 + u * (a1 - a0);
                    float rx = (float)(p1.x + System.Math.Cos(a) * rw);
                    float ry = (float)(p1.y + System.Math.Sin(a) * rw);
                    nvg__vset(ref dst[idst], p1.x, p1.y, 0.5f, 1);
                    idst++;
                    nvg__vset(ref dst[idst], rx, ry, ru, 1);
                    idst++;
                }

                nvg__vset(ref dst[idst], lx1, ly1, lu, 1);
                idst++;
                nvg__vset(ref dst[idst], p1.x - dlx1 * rw, p1.y - dly1 * rw, ru, 1);
                idst++;

            }
            else
            {
                float rx0 = 0, ry0 = 0, rx1 = 0, ry1 = 0, a0, a1;
                nvg__chooseBevel(p1.flags & (int)NVGpointFlags.NVG_PR_INNERBEVEL, p0, p1, -rw,
                    ref rx0, ref ry0, ref rx1, ref ry1);
                a0 = (float)System.Math.Atan2(dly0, dlx0);
                a1 = (float)System.Math.Atan2(dly1, dlx1);
                if (a1 < a0)
                    a1 += NVG_PI * 2;

                nvg__vset(ref dst[idst], p1.x + dlx0 * rw, p1.y + dly0 * rw, lu, 1);
                idst++;
                nvg__vset(ref dst[idst], rx0, ry0, ru, 1);
                idst++;

                n = nvg__clampi((int)System.Math.Ceiling(((a1 - a0) / NVG_PI) * ncap), 2, ncap);
                for (i = 0; i < n; i++)
                {
                    float u = i / (float)(n - 1);
                    float a = a0 + u * (a1 - a0);
                    float lx = (float)(p1.x + System.Math.Cos(a) * lw);
                    float ly = (float)(p1.y + System.Math.Sin(a) * lw);
                    nvg__vset(ref dst[idst], lx, ly, lu, 1);
                    idst++;
                    nvg__vset(ref dst[idst], p1.x, p1.y, 0.5f, 1);
                    idst++;
                }

                nvg__vset(ref dst[idst], p1.x + dlx1 * rw, p1.y + dly1 * rw, lu, 1);
                idst++;
                nvg__vset(ref dst[idst], rx1, ry1, ru, 1);
                idst++;

            }
            //return dst;
        }

        static int nvg__expandStroke(NVGcontext ctx, float w, int lineCap, int lineJoin, float miterLimit)
        {
            NVGpathCache cache = ctx.cache;
            NVGvertex[] verts;
            NVGvertex[] dst;
            int cverts, i, j;
            float aa = ctx.fringeWidth;
            int ncap = nvg__curveDivs(w, NVG_PI, ctx.tessTol);  // Calculate divisions per half circle.

            nvg__calculateJoins(ctx, w, lineJoin, miterLimit);

            // only for debug
#if ONLY_FOR_DEBUG
			Console.WriteLine("[nvg__expandStroke()]");
			for (int cont = 0; cont < cache.npoints; cont++)
			{
				Console.Write(String.Format("Cache-Points-index {0}: ", cont));
				Console.WriteLine(String.Format("\tvalueX: {0}\tvalueY: {1} \tflags: {2}",
				                                cache.points[cont].x, cache.points[cont].y, cache.points[cont].flags));
			}
#endif


            // Calculate max vertex usage.
            cverts = 0;
            for (i = 0; i < cache.npaths; i++)
            {
                NVGpath path = cache.paths[i];
                int loop = (path.closed == 0) ? 0 : 1;
                if (lineJoin == (int)NVGlineCap.NVG_ROUND)
                    cverts += (path.count + path.nbevel * (ncap + 2) + 1) * 2; // plus one for loop
                else
                    cverts += (path.count + path.nbevel * 5 + 1) * 2; // plus one for loop
                if (loop == 0)
                {
                    // space for caps
                    if (lineCap == (int)NVGlineCap.NVG_ROUND)
                    {
                        cverts += (ncap * 2 + 2) * 2;
                    }
                    else
                    {
                        cverts += (3 + 3) * 2;
                    }
                }
            }

            verts = nvg__allocTempVerts(ctx, cverts);

            if (verts == null)
                return 0;

            for (i = 0; i < cache.npaths; i++)
            {
                NVGpath path = cache.paths[i];
                int ipts = path.first;
                NVGpoint[] pts = cache.points;
                NVGpoint p0;
                int ip0 = 0;
                NVGpoint p1;
                int ip1 = 0;
                int s, e, loop;
                float dx, dy;
                int iverts = 0;

                path.fill = null;
                path.nfill = 0;
                path.ifill = 0;

                // Calculate fringe or stroke
                loop = (path.closed == 0) ? 0 : 1;
                dst = verts;
                int idst = iverts;
                path.stroke = dst;
                path.istroke = idst;

                if (loop != 0)
                {
                    // Looping
                    ip0 = ipts + path.count - 1;
                    p0 = pts[ip0];
                    ip1 = ipts + 0;
                    p1 = pts[ip1];
                    s = 0;
                    e = path.count;
                }
                else
                {
                    // Add cap
                    ip0 = ipts + 0;
                    p0 = pts[ip0];
                    ip1 = ipts + 1;
                    p1 = pts[ip1];
                    s = 1;
                    e = path.count - 1;
                }

                if (loop == 0)
                {
                    // Add cap
                    dx = p1.x - p0.x;
                    dy = p1.y - p0.y;
                    nvg__normalize(ref dx, ref dy);
                    if (lineCap == (int)NVGlineCap.NVG_BUTT)
                        nvg__buttCapStart(dst, ref idst, p0, dx, dy, w, -aa * 0.5f, aa);
                    else if (lineCap == (int)NVGlineCap.NVG_BUTT || lineCap == (int)NVGlineCap.NVG_SQUARE)
                        nvg__buttCapStart(dst, ref idst, p0, dx, dy, w, w - aa, aa);
                    else if (lineCap == (int)NVGlineCap.NVG_ROUND)
                        nvg__roundCapStart(dst, ref idst, p0, dx, dy, w, ncap, aa);

                }

                for (j = s; j < e; ++j)
                {
                    if ((p1.flags & (int)(NVGpointFlags.NVG_PT_BEVEL | NVGpointFlags.NVG_PR_INNERBEVEL)) != 0)
                    {
                        if (lineJoin == (int)NVGlineCap.NVG_ROUND)
                        {
                            nvg__roundJoin(dst, ref idst, p0, p1, w, w, 0, 1, ncap, aa);
                        }
                        else
                        {
                            nvg__bevelJoin(dst, ref idst, p0, p1, w, w, 0, 1, aa);
                        }
                    }
                    else
                    {
                        nvg__vset(ref dst[idst], p1.x + (p1.dmx * w), p1.y + (p1.dmy * w), 0, 1);
                        idst++;
                        nvg__vset(ref dst[idst], p1.x - (p1.dmx * w), p1.y - (p1.dmy * w), 1, 1);
                        idst++;
                    }
                    p0 = p1;
                    ip1 += 1;
                    p1 = pts[ip1];
                }

                if (loop != 0)
                {
                    // Loop it
                    nvg__vset(ref dst[idst], verts[0].x, verts[0].y, 0, 1);
                    idst++;
                    nvg__vset(ref dst[idst], verts[1].x, verts[1].y, 1, 1);
                    idst++;
                }
                else
                {
                    // Add cap
                    dx = p1.x - p0.x;
                    dy = p1.y - p0.y;
                    nvg__normalize(ref dx, ref dy);
                    if (lineCap == (int)NVGlineCap.NVG_BUTT)
                        nvg__buttCapEnd(dst, ref idst, p1, dx, dy, w, -aa * 0.5f, aa);
                    else if (lineCap == (int)NVGlineCap.NVG_BUTT || lineCap == (int)NVGlineCap.NVG_SQUARE)
                        nvg__buttCapEnd(dst, ref idst, p1, dx, dy, w, w - aa, aa);
                    else if (lineCap == (int)NVGlineCap.NVG_ROUND)
                        nvg__roundCapEnd(dst, ref idst, p1, dx, dy, w, ncap, aa);
                }

                path.nstroke = (int)(idst - iverts);

                verts = dst;
                iverts = idst;
            }

            //ctx.cache.verts = verts;

            return 1;
        }

        public static NVGpaint nvgBoxGradient(NVGcontext ctx,
                                              float x, float y, float w, float h, float r, float f,
                                              NVGcolor icol, NVGcolor ocol)
        {
            NVGpaint p = new NVGpaint();
            //NVG_NOTUSED(ctx);
            //memset(&p, 0, sizeof(p));

            nvgTransformIdentity(p.xform);
            p.xform[4] = x + w * 0.5f;
            p.xform[5] = y + h * 0.5f;

            p.extent[0] = w * 0.5f;
            p.extent[1] = h * 0.5f;

            p.radius = r;

            p.feather = nvg__maxf(1.0f, f);

            p.innerColor = icol;
            p.outerColor = ocol;

            return p;
        }

        public static void nvgClosePath(NVGcontext ctx)
        {
            float[] vals = new float[] { (float)NVGcommands.NVG_CLOSE };
            nvg__appendCommands(ctx, vals, NVG_COUNTOF(vals));
        }

        public static void nvgPathWinding(NVGcontext ctx, int dir)
        {
            float[] vals = new float[] { (float)NVGcommands.NVG_WINDING, (float)dir };
            nvg__appendCommands(ctx, vals, NVG_COUNTOF(vals));
        }

        public static void nvgStroke(NVGcontext ctx)
        {
            NVGstate state = nvg__getState(ctx);
            float scale = nvg__getAverageScale(state.xform);
            float strokeWidth = nvg__clampf(state.strokeWidth * scale, 0.0f, 200.0f);
            NVGpaint strokePaint = state.stroke.Clone();
            NVGpath path;
            int i;

            if (strokeWidth < ctx.fringeWidth)
            {
                // If the stroke width is less than pixel size, use alpha to emulate coverage.
                // Since coverage is area, scale by alpha*alpha.
                float alpha = nvg__clampf(strokeWidth / ctx.fringeWidth, 0.0f, 1.0f);
                strokePaint.innerColor.a *= alpha * alpha;
                strokePaint.outerColor.a *= alpha * alpha;
                strokeWidth = ctx.fringeWidth;
            }

            // Apply global alpha
            strokePaint.innerColor.a *= state.alpha;
            strokePaint.outerColor.a *= state.alpha;

             nvg__flattenPaths(ctx);

            if (ctx.params_.edgeAntiAlias != 0)
                nvg__expandStroke(ctx, strokeWidth * 0.5f + ctx.fringeWidth * 0.5f,
                    state.lineCap, state.lineJoin, state.miterLimit);
            else
                nvg__expandStroke(ctx, strokeWidth * 0.5f, state.lineCap, state.lineJoin, state.miterLimit);

            ctx.params_.renderStroke(ctx.params_.userPtr, ref strokePaint, ref state.scissor, ctx.fringeWidth,
                strokeWidth, ctx.cache.paths, ctx.cache.npaths);

            // Count triangles
            for (i = 0; i < ctx.cache.npaths; i++)
            {
                path = ctx.cache.paths[i];
                ctx.strokeTriCount += path.nstroke - 2;
                ctx.drawCallCount++;
            }
        }

        // State handling
        public static void nvgSave(NVGcontext ctx)
        {
            if (ctx.nstates >= NVG_MAX_STATES)
                return;
            if (ctx.nstates > 0)
                //memcpy(&ctx->states[ctx->nstates], &ctx->states[ctx->nstates-1], sizeof(NVGstate));
                ctx.states[ctx.nstates] = ctx.states[ctx.nstates - 1].Clone();
            ctx.nstates++;
        }

        public static void nvgRestore(NVGcontext ctx)
        {
            if (ctx.nstates <= 1)
                return;
            ctx.nstates--;
        }

        static void nvgTransformPremultiply(float[] t, float[] s)
        {
            float[] s2 = new float[6];
            //memcpy(s2, s, sizeof(float)*6);
            Array.Copy(s, s2, 6);
            nvgTransformMultiply(s2, t);
            //memcpy(t, s2, sizeof(float)*6);
            Array.Copy(s2, t, 6);
        }

        public static void nvgTransformRotate(float[] t, float a)
        {
            float cs = nvg__cosf(a), sn = nvg__sinf(a);
            t[0] = cs;
            t[1] = sn;
            t[2] = -sn;
            t[3] = cs;
            t[4] = 0.0f;
            t[5] = 0.0f;
        }

        public static void nvgTransformTranslate(float[] t, float tx, float ty)
        {
            t[0] = 1.0f;
            t[1] = 0.0f;
            t[2] = 0.0f;
            t[3] = 1.0f;
            t[4] = tx;
            t[5] = ty;
        }

        public static float nvgDegToRad(float deg)
        {
            return deg / 180.0f * NVG_PI;
        }

        // Scissoring
        public static void nvgScissor(NVGcontext ctx, float x, float y, float w, float h)
        {
            NVGstate state = nvg__getState(ctx);

            w = nvg__maxf(0.0f, w);
            h = nvg__maxf(0.0f, h);

            nvgTransformIdentity(state.scissor.xform);
            state.scissor.xform[4] = x + w * 0.5f;
            state.scissor.xform[5] = y + h * 0.5f;
            nvgTransformMultiply(state.scissor.xform, state.xform);

            state.scissor.extent[0] = w * 0.5f;
            state.scissor.extent[1] = h * 0.5f;
        }

        static void nvg__isectRects(float[] dst,
                                    float ax, float ay, float aw, float ah,
                                    float bx, float by, float bw, float bh)
        {
            float minx = nvg__maxf(ax, bx);
            float miny = nvg__maxf(ay, by);
            float maxx = nvg__minf(ax + aw, bx + bw);
            float maxy = nvg__minf(ay + ah, by + bh);
            dst[0] = minx;
            dst[1] = miny;
            dst[2] = nvg__maxf(0.0f, maxx - minx);
            dst[3] = nvg__maxf(0.0f, maxy - miny);
        }

        public static void nvgIntersectScissor(NVGcontext ctx, float x, float y, float w, float h)
        {
            NVGstate state = nvg__getState(ctx);
            float[] pxform = new float[6], invxorm = new float[6];
            float[] rect = new float[4];
            float ex, ey, tex, tey;

            // If no previous scissor has been set, set the scissor as current scissor.
            if (state.scissor.extent[0] < 0)
            {
                nvgScissor(ctx, x, y, w, h);
                return;
            }

            // Transform the current scissor rect into current transform space.
            // If there is difference in rotation, this will be approximation.
            //memcpy(pxform, state->scissor.xform, sizeof(float)*6);
            Array.Copy(state.scissor.xform, pxform, 6);
            ex = state.scissor.extent[0];
            ey = state.scissor.extent[1];
            nvgTransformInverse(invxorm, state.xform);
            nvgTransformMultiply(pxform, invxorm);
            tex = ex * nvg__absf(pxform[0]) + ey * nvg__absf(pxform[2]);
            tey = ex * nvg__absf(pxform[1]) + ey * nvg__absf(pxform[3]);

            // Intersect rects.
            nvg__isectRects(rect, pxform[4] - tex, pxform[5] - tey, tex * 2, tey * 2, x, y, w, h);

            nvgScissor(ctx, rect[0], rect[1], rect[2], rect[3]);
        }

        public static void nvgResetScissor(NVGcontext ctx)
        {
            NVGstate state = nvg__getState(ctx);
            //memset(state->scissor.xform, 0, sizeof(state->scissor.xform));
            for (int cont = 0; cont < state.scissor.xform.Length; cont++)
                state.scissor.xform[cont] = 0f;
            state.scissor.extent[0] = -1.0f;
            state.scissor.extent[1] = -1.0f;
        }

        public static void nvgRotate(NVGcontext ctx, float angle)
        {
            NVGstate state = nvg__getState(ctx);
            float[] t = new float[6];
            nvgTransformRotate(t, angle);
            nvgTransformPremultiply(state.xform, t);
        }

        public static void nvgScale(NVGcontext ctx, float x, float y)
        {
            NVGstate state = nvg__getState(ctx);
            float[] t = new float[6];
            nvgTransformScale(t, x, y);
            nvgTransformPremultiply(state.xform, t);
        }

        static void nvg__setPaintColor(ref NVGpaint p, NVGcolor color)
        {
            p = new NVGpaint();
            // la anterior línea de código equivale a "memset(p, 0, sizeof(*p));", es
            // necesario de lo contrario aparece un degradado de color no uniforme
            nvgTransformIdentity(p.xform);
            p.radius = 0.0f;
            p.feather = 1.0f;
            p.innerColor = color;
            p.outerColor = color;
        }

        public static void nvgTranslate(NVGcontext ctx, float x, float y)
        {
            NVGstate state = nvg__getState(ctx);
            float[] t = new float[6];
            nvgTransformTranslate(t, x, y);
            nvgTransformPremultiply(state.xform, t);
        }

        static void nvgReset(NVGcontext ctx)
        {
            NVGstate state = nvg__getState(ctx);

            nvg__setPaintColor(ref state.fill, nvgRGBA(255, 255, 255, 255));
            nvg__setPaintColor(ref state.stroke, nvgRGBA(0, 0, 0, 255));
            state.compositeOperation = nvg__compositeOperationState((int)NVGcompositeOperation.NVG_SOURCE_OVER);
            state.strokeWidth = 1.0f;
            state.miterLimit = 10.0f;
            state.lineCap = (int)NVGlineCap.NVG_BUTT;
            state.lineJoin = (int)NVGlineCap.NVG_MITER;
            state.alpha = 1.0f;
            nvgTransformIdentity(state.xform);

            state.scissor.extent[0] = -1.0f;
            state.scissor.extent[1] = -1.0f;

            state.fontSize = 16.0f;
            state.letterSpacing = 0.0f;
            state.lineHeight = 1.0f;
            state.fontBlur = 0.0f;
            state.textAlign = (int)NVGalign.NVG_ALIGN_LEFT | (int)NVGalign.NVG_ALIGN_BASELINE;
            state.fontId = 0;
        }

        public static void nvgCreateInternal(ref NVGparams params_, out NVGcontext ctx)
        {
            FONSparams fontParams = new FONSparams();
            ctx = new NVGcontext();
            int i;

            ctx.params_ = params_;
            for (i = 0; i < NVG_MAX_FONTIMAGES; i++)
                ctx.fontImages[i] = 0;

            ctx.commands = new float[NVG_INIT_COMMANDS_SIZE];
            ctx.ncommands = 0;
            ctx.ccommands = NVG_INIT_COMMANDS_SIZE;

            nvg__allocPathCache(out ctx.cache);

            nvgSave(ctx);
            nvgReset(ctx);

            nvg__setDevicePixelRatio(ref ctx, 1.0f);

            if (ctx.params_.renderCreate(ctx.params_.userPtr) == 0)
                return;

            // Init font rendering
            //memset(&fontParams, 0, sizeof(fontParams));
            fontParams.width = NVG_INIT_FONTIMAGE_SIZE;
            fontParams.height = NVG_INIT_FONTIMAGE_SIZE;
            fontParams.flags = FONSflags.FONS_ZERO_TOPLEFT;
            fontParams.renderCreate = null;
            fontParams.renderUpdate = null;
            fontParams.renderDraw = null;
            fontParams.renderDelete = null;
            fontParams.userPtr = null;
            ctx.fs = FontStash.fonsCreateInternal(ref fontParams);

            // Create font texture
            ctx.fontImages[0] = ctx.params_.renderCreateTexture(ctx.params_.userPtr, (int)NVGtexture.NVG_TEXTURE_ALPHA,
                fontParams.width, fontParams.height, 0, null);
            if (ctx.fontImages[0] == 0)
                throw new Exception("NanoVG.nvgCreateInternal(): Error, creating font texture");
            ctx.fontImageIdx = 0;
        }

        public static void nvgDeleteImage(NVGcontext ctx, int image)
        {
            ctx.params_.renderDeleteTexture(ctx.params_.userPtr, image);
        }

        static void nvgCancelFrame(NVGcontext ctx)
        {
            ctx.params_.renderCancel(ctx.params_.userPtr);
        }

        static void nvgDeleteInternal(NVGcontext ctx)
        {
            int i;
            if (ctx == null)
                return;
            //if (ctx.commands != null)
            //	free(ctx->commands);
            if (ctx.cache != null)
                nvg__deletePathCache(ref ctx.cache);

            if (ctx.fs != null)
                FontStash.fonsDeleteInternal(ctx.fs);

            for (i = 0; i < NVG_MAX_FONTIMAGES; i++)
            {
                if (ctx.fontImages[i] != 0)
                {
                    nvgDeleteImage(ctx, ctx.fontImages[i]);
                    ctx.fontImages[i] = 0;
                }
            }

            if (ctx.params_.renderDelete != null)
                ctx.params_.renderDelete(ctx.params_.userPtr);

            //free(ctx);
            ctx = null;
        }

        public static void nvgEndFrame(NVGcontext ctx)
        {
            NVGstate state = nvg__getState(ctx);
            //Corrige(state);
            ctx.params_.renderFlush(ctx.params_.userPtr, state.compositeOperation);
            if (ctx.fontImageIdx != 0)
            {
                int fontImage = ctx.fontImages[ctx.fontImageIdx];
                int i, j, iw = 0, ih = 0;
                // delete images that smaller than current one
                if (fontImage == 0)
                    return;
                nvgImageSize(ctx, fontImage, ref iw, ref ih);
                for (i = j = 0; i < ctx.fontImageIdx; i++)
                {
                    if (ctx.fontImages[i] != 0)
                    {
                        int nw = 0, nh = 0;
                        nvgImageSize(ctx, ctx.fontImages[i], ref nw, ref nh);
                        if (nw < iw || nh < ih)
                            nvgDeleteImage(ctx, ctx.fontImages[i]);
                        else
                            ctx.fontImages[j++] = ctx.fontImages[i];
                    }
                }
                // make current font image to first
                ctx.fontImages[j++] = ctx.fontImages[0];
                ctx.fontImages[0] = fontImage;
                ctx.fontImageIdx = 0;
                // clear all images after j
                for (i = j; i < NVG_MAX_FONTIMAGES; i++)
                    ctx.fontImages[i] = 0;
            }
        }


        // Draw
        public static void nvgBeginPath(NVGcontext ctx)
        {
            ctx.ncommands = 0;
            nvg__clearPathCache(ctx);
        }

        static void nvg__clearPathCache(NVGcontext ctx)
        {
            ctx.cache.npoints = 0;
            ctx.cache.npaths = 0;
        }

        static void nvgTransformPoint(ref float dx, ref float dy, float[] t, float sx, float sy)
        {
            dx = sx * t[0] + sy * t[2] + t[4];
            dy = sx * t[1] + sy * t[3] + t[5];
        }

        static void nvg__appendCommands(NVGcontext ctx, float[] vals, int nvals)
        {
            NVGstate state = nvg__getState(ctx);
            int i;

            if (ctx.ncommands + nvals > ctx.ccommands)
            {
                int ccommands = ctx.ncommands + nvals + ctx.ccommands / 2;
                //commands = (float*)realloc(ctx->commands, sizeof(float)*ccommands);
                Array.Resize<float>(ref ctx.commands, ccommands);
                ctx.ccommands = ccommands;
            }

            if ((int)vals[0] != (int)NVGcommands.NVG_CLOSE &&
                (int)vals[0] != (int)NVGcommands.NVG_WINDING)
            {
                ctx.commandx = vals[nvals - 2];
                ctx.commandy = vals[nvals - 1];
            }

            // transform commands
            i = 0;
            while (i < nvals)
            {
                int cmd = (int)vals[i];
                switch (cmd)
                {
                    case (int)NVGcommands.NVG_MOVETO:
                        nvgTransformPoint(ref vals[i + 1], ref vals[i + 2], state.xform, vals[i + 1], vals[i + 2]);
                        i += 3;
                        break;
                    case (int)NVGcommands.NVG_LINETO:
                        nvgTransformPoint(ref vals[i + 1], ref vals[i + 2], state.xform, vals[i + 1], vals[i + 2]);
                        i += 3;
                        break;
                    case (int)NVGcommands.NVG_BEZIERTO:
                        nvgTransformPoint(ref vals[i + 1], ref vals[i + 2], state.xform, vals[i + 1], vals[i + 2]);
                        nvgTransformPoint(ref vals[i + 3], ref vals[i + 4], state.xform, vals[i + 3], vals[i + 4]);
                        nvgTransformPoint(ref vals[i + 5], ref vals[i + 6], state.xform, vals[i + 5], vals[i + 6]);
                        i += 7;
                        break;
                    case (int)NVGcommands.NVG_CLOSE:
                        i++;
                        break;
                    case (int)NVGcommands.NVG_WINDING:
                        i += 2;
                        break;
                    default:
                        i++;
                        break;
                }
            }

            //memcpy(&ctx->commands[ctx->ncommands], vals, nvals * sizeof(float));

            Array.Copy(vals, 0, ctx.commands, ctx.ncommands, nvals);

            // only for debug
#if ONLY_FOR_DEBUG
			Console.WriteLine("C#");
			for (int cont = 0; cont < nvals; cont++)
			{
				Console.Write(String.Format("index {0}: ", cont));
				Console.WriteLine(String.Format("value: {0}", ctx.commands[ctx.ncommands + cont]));
			}
#endif

            ctx.ncommands += nvals;
        }

        static int NVG_COUNTOF(float[] arr)
        {
            return arr.Length; //(sizeof(arr) / sizeof(0[arr]));
        }

        static void nvg__addPath(NVGcontext ctx)
        {
            NVGpath path;
            if (ctx.cache.npaths + 1 > ctx.cache.cpaths)
            {
                int cpaths = ctx.cache.npaths + 1 + ctx.cache.cpaths / 2;
                //paths = (NVGpath*)realloc(ctx->cache->paths, sizeof(NVGpath)*cpaths);
                Array.Resize<NVGpath>(ref ctx.cache.paths, cpaths);
                NVGpath[] paths = ctx.cache.paths;
                if (paths == null)
                    return;
                ctx.cache.paths = paths;
                ctx.cache.cpaths = cpaths;
            }
            path = ctx.cache.paths[ctx.cache.npaths];
            if (path == null)
            {
                path = new NVGpath();
                ctx.cache.paths[ctx.cache.npaths] = path;
            }
            else
            {
                path.closed = 0;
                path.convex = 0;
                path.count = 0;
                path.fill = null;
                path.ifill = 0;
                path.first = 0;
                path.nbevel = 0;
                path.nfill = 0;
                path.nstroke = 0;
                path.stroke = null;
                path.istroke = 0;
                path.winding = 0;
            }

            path.first = ctx.cache.npoints;
            path.winding = (int)NVGwinding.NVG_CCW;

            ctx.cache.npaths++;
        }

        static NVGpoint nvg__lastPoint(NVGcontext ctx)
        {
            if (ctx.cache.npoints > 0)
                return ctx.cache.points[ctx.cache.npoints - 1];
            return null;
        }

        static NVGpath nvg__lastPath(NVGcontext ctx)
        {
            if (ctx.cache.npaths > 0)
                return ctx.cache.paths[ctx.cache.npaths - 1];
            return null;
        }

        static bool nvg__ptEquals(float x1, float y1, float x2, float y2, float tol)
        {
            float dx = x2 - x1;
            float dy = y2 - y1;
            return (dx * dx + dy * dy) < (tol * tol);
        }

        static void nvg__addPoint(NVGcontext ctx, float x, float y, int flags)
        {
            NVGpoint pt;
            NVGpath path = nvg__lastPath(ctx);
            if (path == null)
                return;

            if (path.count > 0 && ctx.cache.npoints > 0)
            {
                pt = nvg__lastPoint(ctx);
                if (nvg__ptEquals(pt.x, pt.y, x, y, ctx.distTol))
                {
                    pt.flags |= (byte)flags;
                    return;
                }
            }

            if (ctx.cache.npoints + 1 > ctx.cache.cpoints)
            {
                int cpoints = ctx.cache.npoints + 1 + ctx.cache.cpoints / 2;
                //points = (NVGpoint*)realloc(ctx->cache->points, sizeof(NVGpoint)*cpoints);
                Array.Resize<NVGpoint>(ref ctx.cache.points, cpoints);
                NVGpoint[] points = ctx.cache.points;

                if (points == null)
                    return;
                ctx.cache.points = points;
                ctx.cache.cpoints = cpoints;
            }

            pt = new NVGpoint();

            ctx.cache.points[ctx.cache.npoints] = pt;
            //memset(pt, 0, sizeof(*pt));
            pt.x = x;
            pt.y = y;
            pt.flags = (byte)flags;

            //only for debug
#if ONLY_FOR_DEBUG
			Console.WriteLine(String.Format("Added point: {0}", pt));
#endif

            ctx.cache.npoints++;
            path.count++;
        }

        static void nvg__closePath(NVGcontext ctx)
        {
            NVGpath path = nvg__lastPath(ctx);
            if (path == null)
                return;
            path.closed = 1;
        }

        static void nvg__pathWinding(NVGcontext ctx, int winding)
        {
            NVGpath path = nvg__lastPath(ctx);
            if (path == null)
                return;
            path.winding = winding;
        }

        static float nvg__triarea2(float ax, float ay, float bx, float by, float cx, float cy)
        {
            float abx = bx - ax;
            float aby = by - ay;
            float acx = cx - ax;
            float acy = cy - ay;
            return acx * aby - abx * acy;
        }

        static float nvg__polyArea(NVGpoint[] pts, int ipts, int npts)
        {
            int i;
            float area = 0;
            for (i = 2; i < npts; i++)
            {
                NVGpoint a = pts[0 + ipts];
                NVGpoint b = pts[i - 1 + ipts];
                NVGpoint c = pts[i + ipts];
                area += nvg__triarea2(a.x, a.y, b.x, b.y, c.x, c.y);
            }
            return area * 0.5f;
        }

        static void nvg__polyReverse(NVGpoint[] pts, int ipts, int npts)
        {
            NVGpoint tmp;
            int i = 0, j = npts - 1;
            while (i < j)
            {
                tmp = pts[i + ipts].Clone();
                pts[i + ipts] = pts[j + ipts].Clone();
                pts[j + ipts] = tmp;
                i++;
                j--;
            }
        }

        static void nvg__tesselateBezier(NVGcontext ctx,
                                         float x1, float y1, float x2, float y2,
                                         float x3, float y3, float x4, float y4,
                                         int level, int type)
        {
            float x12, y12, x23, y23, x34, y34, x123, y123, x234, y234, x1234, y1234;
            float dx, dy, d2, d3;

            if (level > 10)
                return;

            x12 = (x1 + x2) * 0.5f;
            y12 = (y1 + y2) * 0.5f;
            x23 = (x2 + x3) * 0.5f;
            y23 = (y2 + y3) * 0.5f;
            x34 = (x3 + x4) * 0.5f;
            y34 = (y3 + y4) * 0.5f;
            x123 = (x12 + x23) * 0.5f;
            y123 = (y12 + y23) * 0.5f;

            dx = x4 - x1;
            dy = y4 - y1;
            d2 = nvg__absf(((x2 - x4) * dy - (y2 - y4) * dx));
            d3 = nvg__absf(((x3 - x4) * dy - (y3 - y4) * dx));

            if ((d2 + d3) * (d2 + d3) < ctx.tessTol * (dx * dx + dy * dy))
            {
                nvg__addPoint(ctx, x4, y4, type);
                return;
            }

            /*	if (nvg__absf(x1+x3-x2-x2) + nvg__absf(y1+y3-y2-y2) + nvg__absf(x2+x4-x3-x3) + nvg__absf(y2+y4-y3-y3) < ctx->tessTol) {
					nvg__addPoint(ctx, x4, y4, type);
				return;
			}*/

            x234 = (x23 + x34) * 0.5f;
            y234 = (y23 + y34) * 0.5f;
            x1234 = (x123 + x234) * 0.5f;
            y1234 = (y123 + y234) * 0.5f;

            nvg__tesselateBezier(ctx, x1, y1, x12, y12, x123, y123, x1234, y1234, level + 1, 0);
            nvg__tesselateBezier(ctx, x1234, y1234, x234, y234, x34, y34, x4, y4, level + 1, type);
        }

        static void nvg__flattenPaths(NVGcontext ctx)
        {
            NVGpathCache cache = ctx.cache;
            //	NVGstate* state = nvg__getState(ctx);
            NVGpoint last;
            NVGpoint p0;
            NVGpoint p1;
            int ip1 = 0;
            NVGpoint[] pts;
            int ipts = 0;
            NVGpath path;
            int i, j;
            float[] cp1;
            int icp1 = 0;
            float[] cp2;
            int icp2 = 0;
            float[] p;
            int ip = 0;
            float area;

            if (cache.npaths > 0)
                return;

            // Flatten
            i = 0;
            while (i < ctx.ncommands)
            {
                int cmd = (int)ctx.commands[i];

                switch (cmd)
                {
                    case (int)NVGcommands.NVG_MOVETO:
                        nvg__addPath(ctx);
                        p = ctx.commands;
                        ip = i + 1;
                        nvg__addPoint(ctx, p[0 + ip], p[1 + ip], (int)NVGpointFlags.NVG_PT_CORNER);
                        i += 3;
                        break;
                    case (int)NVGcommands.NVG_LINETO:
                        p = ctx.commands;
                        ip = i + 1;
                        nvg__addPoint(ctx, p[0 + ip], p[1 + ip], (int)NVGpointFlags.NVG_PT_CORNER);
                        i += 3;
                        break;
                    case (int)NVGcommands.NVG_BEZIERTO:
                        last = nvg__lastPoint(ctx);
                        if (last != null)
                        {
                            cp1 = ctx.commands;
                            icp1 = i + 1;
                            cp2 = ctx.commands;
                            icp2 = i + 3;
                            p = ctx.commands;
                            ip = i + 5;
                            nvg__tesselateBezier(ctx, last.x, last.y,
                                cp1[0 + icp1], cp1[1 + icp1],
                                cp2[0 + icp2], cp2[1 + icp2],
                                p[0 + ip],
                                p[1 + ip],
                                0, (int)NVGpointFlags.NVG_PT_CORNER);
                        }
                        i += 7;
                        break;
                    case (int)NVGcommands.NVG_CLOSE:
                        nvg__closePath(ctx);
                        i++;
                        break;
                    case (int)NVGcommands.NVG_WINDING:
                        nvg__pathWinding(ctx, (int)ctx.commands[i + 1]);
                        i += 2;
                        break;
                    default:
                        i++;
                        break;
                }
            }

            cache.bounds[0] = cache.bounds[1] = 1e6f;
            cache.bounds[2] = cache.bounds[3] = -1e6f;

            // Calculate the direction and length of line segments.
            for (j = 0; j < cache.npaths; j++)
            {
                path = cache.paths[j];
                pts = cache.points;
                ipts = path.first;

                // If the first and last points are the same, remove the last, mark as closed path.
                p0 = pts[ipts + path.count - 1];
                ip1 = 0 + ipts;
                p1 = pts[ip1];

                if (nvg__ptEquals(p0.x, p0.y, p1.x, p1.y, ctx.distTol))
                {
                    path.count--;

                    if (ipts + path.count - 1 <= pts.Length && path.count - 1 > 0)
                    {
                        p0 = pts[ipts + path.count - 1];
                        path.closed = 1;
                    }
                }

                // Enforce winding.
                if (path.count > 2)
                {
                    area = nvg__polyArea(pts, ipts, path.count);
                    if (path.winding == (int)NVGwinding.NVG_CCW && area < 0.0f)
                    {
                        nvg__polyReverse(pts, ipts, path.count);
                        p0 = pts[ipts + path.count - 1];
                        p1 = pts[ip1];
                    }
                    if (path.winding == (int)NVGwinding.NVG_CW && area > 0.0f)
                    {
                        nvg__polyReverse(pts, ipts, path.count);
                        p0 = pts[ipts + path.count - 1];
                        p1 = pts[ip1];
                    }
                }

                for (i = 0; i < path.count; i++)
                {
                    // Calculate segment direction and length
                    p0.dx = p1.x - p0.x;
                    p0.dy = p1.y - p0.y;
                    p0.len = nvg__normalize(ref p0.dx, ref p0.dy);
                    // Update bounds
                    cache.bounds[0] = nvg__minf(cache.bounds[0], p0.x);
                    cache.bounds[1] = nvg__minf(cache.bounds[1], p0.y);
                    cache.bounds[2] = nvg__maxf(cache.bounds[2], p0.x);
                    cache.bounds[3] = nvg__maxf(cache.bounds[3], p0.y);
                    // Advance
                    p0 = p1;
                    ip1 += 1;
                    p1 = pts[ip1];
                }
            }
        }

        public static void nvgTransformMultiply(float[] t, float[] s)
        {
            float t0 = t[0] * s[0] + t[1] * s[2];
            float t2 = t[2] * s[0] + t[3] * s[2];
            float t4 = t[4] * s[0] + t[5] * s[2] + s[4];
            t[1] = t[0] * s[1] + t[1] * s[3];
            t[3] = t[2] * s[1] + t[3] * s[3];
            t[5] = t[4] * s[1] + t[5] * s[3] + s[5];
            t[0] = t0;
            t[2] = t2;
            t[4] = t4;
        }

        public static void nvgLineJoin(NVGcontext ctx, int join)
        {
            NVGstate state = nvg__getState(ctx);
            state.lineJoin = join;
        }

        public static void nvgMoveTo(NVGcontext ctx, float x, float y)
        {
            float[] vals = new float[] { (float)NVGcommands.NVG_MOVETO, x, y };
            nvg__appendCommands(ctx, vals, NVG_COUNTOF(vals));
        }

        public static void nvgBezierTo(NVGcontext ctx, float c1x, float c1y, float c2x, float c2y, float x, float y)
        {
            float[] vals = new float[] { (float)NVGcommands.NVG_BEZIERTO, c1x, c1y, c2x, c2y, x, y };
            nvg__appendCommands(ctx, vals, NVG_COUNTOF(vals));
        }

        public static void nvgLineTo(NVGcontext ctx, float x, float y)
        {
            float[] vals = new float[] { (float)NVGcommands.NVG_LINETO, x, y };
            nvg__appendCommands(ctx, vals, NVG_COUNTOF(vals));
        }

        public static void nvgLineCap(NVGcontext ctx, int cap)
        {
            NVGstate state = nvg__getState(ctx);
            state.lineCap = cap;
        }

        public static void nvgFillPaint(NVGcontext ctx, NVGpaint paint)
        {
            NVGstate state = nvg__getState(ctx);
            state.fill = paint.Clone();
            nvgTransformMultiply(state.fill.xform, state.xform);
        }

        public static void nvgFillColor(NVGcontext ctx, NVGcolor color)
        {
            NVGstate state = nvg__getState(ctx);
            nvg__setPaintColor(ref state.fill, color);
        }

        public static void nvgStrokeColor(NVGcontext ctx, NVGcolor color)
        {
            NVGstate state = nvg__getState(ctx);
            nvg__setPaintColor(ref state.stroke, color);
        }

        // State setting
        public static void nvgStrokeWidth(NVGcontext ctx, float width)
        {
            NVGstate state = nvg__getState(ctx);
            state.strokeWidth = width;
        }

        static void nvg__vset(ref NVGvertex vtx, float x, float y, float u, float v)
        {
            vtx.x = x;
            vtx.y = y;
            vtx.u = u;
            vtx.v = v;
        }

        static void nvg__calculateJoins(NVGcontext ctx, float w, int lineJoin, float miterLimit)
        {
            NVGpathCache cache = ctx.cache;
            int i, j;
            float iw = 0.0f;

            if (w > 0.0f)
                iw = 1.0f / w;

            // Calculate which joins needs extra vertices to append, and gather vertex count.
            for (i = 0; i < cache.npaths; i++)
            {
                NVGpath path = cache.paths[i];

                int ipts = path.first;
                NVGpoint[] pts = cache.points;

                if (ipts + path.count - 1 > pts.Length || ipts + path.count - 1 < 0)
                    continue;

                NVGpoint p0 = pts[ipts + path.count - 1];

                int ip1 = ipts;
                NVGpoint p1 = pts[ip1 + 0];

                int nleft = 0;

                path.nbevel = 0;

                for (j = 0; j < path.count; j++)
                {
                    float dlx0, dly0, dlx1, dly1, dmr2, cross, limit;
                    dlx0 = p0.dy;
                    dly0 = -p0.dx;
                    dlx1 = p1.dy;
                    dly1 = -p1.dx;
                    // Calculate extrusions
                    p1.dmx = (dlx0 + dlx1) * 0.5f;
                    p1.dmy = (dly0 + dly1) * 0.5f;
                    dmr2 = p1.dmx * p1.dmx + p1.dmy * p1.dmy;
                    if (dmr2 > 0.000001f)
                    {
                        float scale = 1.0f / dmr2;
                        if (scale > 600.0f)
                        {
                            scale = 600.0f;
                        }
                        p1.dmx *= scale;
                        p1.dmy *= scale;
                    }

                    // Clear flags, but keep the corner.
                    p1.flags = (byte)(((p1.flags &
                    (int)NVGpointFlags.NVG_PT_CORNER) != 0) ? (int)NVGpointFlags.NVG_PT_CORNER : 0);

                    // Keep track of left turns.
                    cross = p1.dx * p0.dy - p0.dx * p1.dy;
                    if (cross > 0.0f)
                    {
                        nleft++;
                        p1.flags |= (int)NVGpointFlags.NVG_PT_LEFT;
                    }

                    // Calculate if we should use bevel or miter for inner join.
                    limit = nvg__maxf(1.01f, nvg__minf(p0.len, p1.len) * iw);
                    if ((dmr2 * limit * limit) < 1.0f)
                        p1.flags |= (int)NVGpointFlags.NVG_PR_INNERBEVEL;

                    // Check to see if the corner needs to be beveled.
                    if ((p1.flags & (int)NVGpointFlags.NVG_PT_CORNER) != 0)
                    {
                        if ((dmr2 * miterLimit * miterLimit) < 1.0f ||
                            lineJoin == (int)NVGlineCap.NVG_BEVEL ||
                            lineJoin == (int)NVGlineCap.NVG_ROUND)
                        {
                            p1.flags |= (int)NVGpointFlags.NVG_PT_BEVEL;
                        }
                    }

                    if ((p1.flags & ((int)NVGpointFlags.NVG_PT_BEVEL | (int)NVGpointFlags.NVG_PR_INNERBEVEL)) != 0)
                        path.nbevel++;

                    p0 = p1;
                    ip1 += 1;
                    p1 = pts[ip1];
                }

                path.convex = (nleft == path.count) ? 1 : 0;
            }
        }

        static NVGvertex[] nvg__allocTempVerts(NVGcontext ctx, int nverts)
        {
            if (nverts > ctx.cache.cverts)
            {
                int cverts = (nverts + 0xff) & ~0xff; // Round up to prevent allocations when things change just slightly.
                                                      //verts = (NVGvertex*)realloc(ctx->cache->verts, sizeof(NVGvertex)*cverts);
                Array.Resize<NVGvertex>(ref ctx.cache.verts, cverts);
                ctx.cache.cverts = cverts;
            }

            return ctx.cache.verts;
        }

        static void nvg__chooseBevel(int bevel, NVGpoint[] p0, int ip0, NVGpoint[] p1, int ip1, float w,
                                     ref float x0, ref float y0, ref float x1, ref float y1)
        {
            if (bevel != 0)
            {
                x0 = p1[ip1].x + p0[ip0].dy * w;
                y0 = p1[ip1].y - p0[ip0].dx * w;
                x1 = p1[ip1].x + p1[ip1].dy * w;
                y1 = p1[ip1].y - p1[ip1].dx * w;
            }
            else
            {
                x0 = p1[ip1].x + p1[ip1].dmx * w;
                y0 = p1[ip1].y + p1[ip1].dmy * w;
                x1 = p1[ip1].x + p1[ip1].dmx * w;
                y1 = p1[ip1].y + p1[ip1].dmy * w;
            }
        }

        static void nvg__chooseBevel(int bevel, NVGpoint p0, NVGpoint p1, float w,
                                     ref float x0, ref float y0, ref float x1, ref float y1)
        {
            if (bevel != 0)
            {
                x0 = p1.x + p0.dy * w;
                y0 = p1.y - p0.dx * w;
                x1 = p1.x + p1.dy * w;
                y1 = p1.y - p1.dx * w;
            }
            else
            {
                x0 = p1.x + p1.dmx * w;
                y0 = p1.y + p1.dmy * w;
                x1 = p1.x + p1.dmx * w;
                y1 = p1.y + p1.dmy * w;
            }
        }

        static void nvg__bevelJoin(NVGvertex[] dst, ref int idst,
                                   NVGpoint p0, NVGpoint p1, float lw, float rw, float lu, float ru, float fringe)
        {
            float rx0 = 0, ry0 = 0, rx1 = 0, ry1 = 0;
            float lx0 = 0, ly0 = 0, lx1 = 0, ly1 = 0;
            float dlx0 = p0.dy;
            float dly0 = -p0.dx;
            float dlx1 = p1.dy;
            float dly1 = -p1.dx;
            //NVG_NOTUSED(fringe);

            if ((p1.flags & (int)NVGpointFlags.NVG_PT_LEFT) != 0)
            {
                nvg__chooseBevel(p1.flags & (int)NVGpointFlags.NVG_PR_INNERBEVEL,
                    p0, p1, lw, ref lx0, ref ly0, ref lx1, ref ly1);

                nvg__vset(ref dst[idst], lx0, ly0, lu, 1);
                idst++;
                nvg__vset(ref dst[idst], p1.x - dlx0 * rw, p1.y - dly0 * rw, ru, 1);
                idst++;

                if ((p1.flags & (int)NVGpointFlags.NVG_PT_BEVEL) != 0)
                {
                    nvg__vset(ref dst[idst], lx0, ly0, lu, 1);
                    idst++;
                    nvg__vset(ref dst[idst], p1.x - dlx0 * rw, p1.y - dly0 * rw, ru, 1);
                    idst++;

                    nvg__vset(ref dst[idst], lx1, ly1, lu, 1);
                    idst++;
                    nvg__vset(ref dst[idst], p1.x - dlx1 * rw, p1.y - dly1 * rw, ru, 1);
                    idst++;
                }
                else
                {
                    rx0 = p1.x - p1.dmx * rw;
                    ry0 = p1.y - p1.dmy * rw;

                    nvg__vset(ref dst[idst], p1.x, p1.y, 0.5f, 1);
                    idst++;
                    nvg__vset(ref dst[idst], p1.x - dlx0 * rw, p1.y - dly0 * rw, ru, 1);
                    idst++;

                    nvg__vset(ref dst[idst], rx0, ry0, ru, 1);
                    idst++;
                    nvg__vset(ref dst[idst], rx0, ry0, ru, 1);
                    idst++;

                    nvg__vset(ref dst[idst], p1.x, p1.y, 0.5f, 1);
                    idst++;
                    nvg__vset(ref dst[idst], p1.x - dlx1 * rw, p1.y - dly1 * rw, ru, 1);
                    idst++;
                }

                nvg__vset(ref dst[idst], lx1, ly1, lu, 1);
                idst++;
                nvg__vset(ref dst[idst], p1.x - dlx1 * rw, p1.y - dly1 * rw, ru, 1);
                idst++;

            }
            else
            {
                nvg__chooseBevel(p1.flags & (int)NVGpointFlags.NVG_PR_INNERBEVEL,
                    p0, p1, -rw, ref rx0, ref ry0, ref rx1, ref ry1);

                nvg__vset(ref dst[idst], p1.x + dlx0 * lw, p1.y + dly0 * lw, lu, 1);
                idst++;
                nvg__vset(ref dst[idst], rx0, ry0, ru, 1);
                idst++;

                if ((p1.flags & (int)NVGpointFlags.NVG_PT_BEVEL) != 0)
                {
                    nvg__vset(ref dst[idst], p1.x + dlx0 * lw, p1.y + dly0 * lw, lu, 1);
                    idst++;
                    nvg__vset(ref dst[idst], rx0, ry0, ru, 1);
                    idst++;

                    nvg__vset(ref dst[idst], p1.x + dlx1 * lw, p1.y + dly1 * lw, lu, 1);
                    idst++;
                    nvg__vset(ref dst[idst], rx1, ry1, ru, 1);
                    idst++;
                }
                else
                {
                    lx0 = p1.x + p1.dmx * lw;
                    ly0 = p1.y + p1.dmy * lw;

                    nvg__vset(ref dst[idst], p1.x + dlx0 * lw, p1.y + dly0 * lw, lu, 1);
                    idst++;
                    nvg__vset(ref dst[idst], p1.x, p1.y, 0.5f, 1);
                    idst++;

                    nvg__vset(ref dst[idst], lx0, ly0, lu, 1);
                    idst++;
                    nvg__vset(ref dst[idst], lx0, ly0, lu, 1);
                    idst++;

                    nvg__vset(ref dst[idst], p1.x + dlx1 * lw, p1.y + dly1 * lw, lu, 1);
                    idst++;
                    nvg__vset(ref dst[idst], p1.x, p1.y, 0.5f, 1);
                    idst++;
                }

                nvg__vset(ref dst[idst], p1.x + dlx1 * lw, p1.y + dly1 * lw, lu, 1);
                idst++;
                nvg__vset(ref dst[idst], rx1, ry1, ru, 1);
                idst++;
            }

            //return dst[idst];
        }

        static void nvg__bevelJoin(NVGvertex[] dst, ref int idst,
                                   NVGpoint[] p0, int ip0, NVGpoint[] p1, int ip1,
                                   float lw, float rw, float lu, float ru, float fringe)
        {
            float rx0 = 0, ry0 = 0, rx1 = 0, ry1 = 0;
            float lx0 = 0, ly0 = 0, lx1 = 0, ly1 = 0;
            float dlx0 = p0[ip0].dy;
            float dly0 = -p0[ip0].dx;
            float dlx1 = p1[ip1].dy;
            float dly1 = -p1[ip1].dx;
            //NVG_NOTUSED(fringe);

            if ((p1[ip1].flags & (int)NVGpointFlags.NVG_PT_LEFT) != 0)
            {
                nvg__chooseBevel(p1[ip1].flags & (int)NVGpointFlags.NVG_PR_INNERBEVEL,
                    p0, ip0, p1, ip1, lw, ref lx0, ref ly0, ref lx1, ref ly1);

                nvg__vset(ref dst[idst], lx0, ly0, lu, 1);
                idst++;
                nvg__vset(ref dst[idst], p1[ip1].x - dlx0 * rw, p1[ip1].y - dly0 * rw, ru, 1);
                idst++;

                if ((p1[ip1].flags & (int)NVGpointFlags.NVG_PT_BEVEL) != 0)
                {
                    nvg__vset(ref dst[idst], lx0, ly0, lu, 1);
                    idst++;
                    nvg__vset(ref dst[idst], p1[ip1].x - dlx0 * rw, p1[ip1].y - dly0 * rw, ru, 1);
                    idst++;

                    nvg__vset(ref dst[idst], lx1, ly1, lu, 1);
                    idst++;
                    nvg__vset(ref dst[idst], p1[ip1].x - dlx1 * rw, p1[ip1].y - dly1 * rw, ru, 1);
                    idst++;
                }
                else
                {
                    rx0 = p1[ip1].x - p1[ip1].dmx * rw;
                    ry0 = p1[ip1].y - p1[ip1].dmy * rw;

                    nvg__vset(ref dst[idst], p1[ip1].x, p1[ip1].y, 0.5f, 1);
                    idst++;
                    nvg__vset(ref dst[idst], p1[ip1].x - dlx0 * rw, p1[ip1].y - dly0 * rw, ru, 1);
                    idst++;

                    nvg__vset(ref dst[idst], rx0, ry0, ru, 1);
                    idst++;
                    nvg__vset(ref dst[idst], rx0, ry0, ru, 1);
                    idst++;

                    nvg__vset(ref dst[idst], p1[ip1].x, p1[ip1].y, 0.5f, 1);
                    idst++;
                    nvg__vset(ref dst[idst], p1[ip1].x - dlx1 * rw, p1[ip1].y - dly1 * rw, ru, 1);
                    idst++;
                }

                nvg__vset(ref dst[idst], lx1, ly1, lu, 1);
                idst++;
                nvg__vset(ref dst[idst], p1[ip1].x - dlx1 * rw, p1[ip1].y - dly1 * rw, ru, 1);
                idst++;

            }
            else
            {
                nvg__chooseBevel(p1[ip1].flags & (int)NVGpointFlags.NVG_PR_INNERBEVEL,
                    p0, ip0, p1, ip1, -rw, ref rx0, ref ry0, ref rx1, ref ry1);

                nvg__vset(ref dst[idst], p1[ip1].x + dlx0 * lw, p1[ip1].y + dly0 * lw, lu, 1);
                idst++;
                nvg__vset(ref dst[idst], rx0, ry0, ru, 1);
                idst++;

                if ((p1[ip1].flags & (int)NVGpointFlags.NVG_PT_BEVEL) != 0)
                {
                    nvg__vset(ref dst[idst], p1[ip1].x + dlx0 * lw, p1[ip1].y + dly0 * lw, lu, 1);
                    idst++;
                    nvg__vset(ref dst[idst], rx0, ry0, ru, 1);
                    idst++;

                    nvg__vset(ref dst[idst], p1[ip1].x + dlx1 * lw, p1[ip1].y + dly1 * lw, lu, 1);
                    idst++;
                    nvg__vset(ref dst[idst], rx1, ry1, ru, 1);
                    idst++;
                }
                else
                {
                    lx0 = p1[ip1].x + p1[ip1].dmx * lw;
                    ly0 = p1[ip1].y + p1[ip1].dmy * lw;

                    nvg__vset(ref dst[idst], p1[ip1].x + dlx0 * lw, p1[ip1].y + dly0 * lw, lu, 1);
                    idst++;
                    nvg__vset(ref dst[idst], p1[ip1].x, p1[ip1].y, 0.5f, 1);
                    idst++;

                    nvg__vset(ref dst[idst], lx0, ly0, lu, 1);
                    idst++;
                    nvg__vset(ref dst[idst], lx0, ly0, lu, 1);
                    idst++;

                    nvg__vset(ref dst[idst], p1[ip1].x + dlx1 * lw, p1[ip1].y + dly1 * lw, lu, 1);
                    idst++;
                    nvg__vset(ref dst[idst], p1[ip1].x, p1[ip1].y, 0.5f, 1);
                    idst++;
                }

                nvg__vset(ref dst[idst], p1[ip1].x + dlx1 * lw, p1[ip1].y + dly1 * lw, lu, 1);
                idst++;
                nvg__vset(ref dst[idst], rx1, ry1, ru, 1);
                idst++;
            }

            //return dst[idst];
        }

        static int nvg__expandFill(NVGcontext ctx, float w, int lineJoin, float miterLimit)
        {
            NVGpathCache cache = ctx.cache;
            NVGvertex[] verts;
            int iverts = 0;
            NVGvertex[] dst;
            int idst = 0;
            int cverts, i, j;
            bool convex = false;
            float aa = ctx.fringeWidth;
            bool fringe = w > 0.0f;

            nvg__calculateJoins(ctx, w, lineJoin, miterLimit);

            // Calculate max vertex usage.
            cverts = 0;
            for (i = 0; i < cache.npaths; i++)
            {
                NVGpath path = cache.paths[i];
                cverts += path.count + path.nbevel + 1;
                if (fringe)
                    cverts += (path.count + path.nbevel * 5 + 1) * 2; // plus one for loop
            }

            verts = nvg__allocTempVerts(ctx, cverts);
            if (verts == null)
                return 0;

            convex = cache.npaths == 1 && cache.paths[0].convex != 0;

            NVGpath path2;

            for (i = 0; i < cache.npaths; i++)
            {
                path2 = cache.paths[i];
                NVGpoint[] pts = cache.points;
                int ipts = path2.first;
                NVGpoint p0;
                int ip0 = 0;
                NVGpoint p1;
                int ip1 = 0;
                float rw, lw, woff;
                float ru, lu;

                // Calculate shape vertices.
                woff = 0.5f * aa;
                dst = verts;
                idst = iverts;
                path2.fill = dst;
                path2.ifill = idst;

                if (fringe)
                {
                    // Looping
                    ip0 = ipts + path2.count - 1;

                    if (ip0 < 0 || ip0 > pts.Length)
                        continue;

                    p0 = pts[ip0];
                    ip1 = ipts + 0;
                    p1 = pts[ip1];

                    for (j = 0; j < path2.count; ++j)
                    {
                        if ((p1.flags & (int)NVGpointFlags.NVG_PT_BEVEL) != 0)
                        {
                            float dlx0 = p0.dy;
                            float dly0 = -p0.dx;
                            float dlx1 = p1.dy;
                            float dly1 = -p1.dx;
                            if ((p1.flags & (int)NVGpointFlags.NVG_PT_LEFT) != 0)
                            {
                                float lx = p1.x + p1.dmx * woff;
                                float ly = p1.y + p1.dmy * woff;
                                nvg__vset(ref dst[idst], lx, ly, 0.5f, 1);
                                idst++;
                            }
                            else
                            {
                                float lx0 = p1.x + dlx0 * woff;
                                float ly0 = p1.y + dly0 * woff;
                                float lx1 = p1.x + dlx1 * woff;
                                float ly1 = p1.y + dly1 * woff;
                                nvg__vset(ref dst[idst], lx0, ly0, 0.5f, 1);
                                idst++;
                                nvg__vset(ref dst[idst], lx1, ly1, 0.5f, 1);
                                idst++;
                            }
                        }
                        else
                        {
                            nvg__vset(ref dst[idst], p1.x + (p1.dmx * woff), p1.y + (p1.dmy * woff), 0.5f, 1);
                            idst++;
                        }
                        p0 = p1;
                        ip1 += 1;
                        p1 = pts[ip1];
                    }
                }
                else
                {
                    for (j = 0; j < path2.count; ++j)
                    {
                        nvg__vset(ref dst[idst], pts[j + ipts].x, pts[j + ipts].y, 0.5f, 1);
                        idst++;
                    }
                }

                path2.nfill = (int)(idst - iverts);
                verts = dst;
                iverts = idst;

                // Calculate fringe (Calcula flecos)
                if (fringe)
                {
                    lw = w + woff;
                    rw = w - woff;
                    lu = 0;
                    ru = 1;
                    idst = iverts;
                    dst = verts;
                    path2.stroke = dst;
                    path2.istroke = idst;

                    // Create only half a fringe for convex shapes so that
                    // the shape can be rendered without stenciling.
                    if (convex)
                    {
                        lw = woff;  // This should generate the same vertex as fill inset above.
                        lu = 0.5f;  // Set outline fade at middle.
                    }

                    // Looping
                    ip0 = ipts + path2.count - 1;
                    p0 = pts[ip0];
                    ip1 = 0 + ipts;
                    p1 = pts[ip1];

                    for (j = 0; j < path2.count; ++j)
                    {
                        if ((p1.flags &
                            ((int)NVGpointFlags.NVG_PT_BEVEL | (int)NVGpointFlags.NVG_PR_INNERBEVEL)) != 0)
                        {
                            nvg__bevelJoin(dst, ref idst, p0, p1, lw, rw, lu, ru, ctx.fringeWidth);
                        }
                        else
                        {
                            nvg__vset(ref dst[idst], p1.x + (p1.dmx * lw), p1.y + (p1.dmy * lw), lu, 1);
                            idst++;
                            nvg__vset(ref dst[idst], p1.x - (p1.dmx * rw), p1.y - (p1.dmy * rw), ru, 1);
                            idst++;
                        }
                        p0 = p1;
                        ip1 += 1;
                        p1 = pts[ip1];
                    }

                    // Loop it
                    nvg__vset(ref dst[idst], verts[0 + iverts].x, verts[0 + iverts].y, lu, 1);
                    idst++;
                    nvg__vset(ref dst[idst], verts[1 + iverts].x, verts[1 + iverts].y, ru, 1);
                    idst++;

                    path2.nstroke = (int)(idst - iverts);
                    iverts = idst;
                    verts = dst;
                }
                else
                {
                    path2.stroke = null;
                    path2.nstroke = 0;
                }

#if ONLY_FOR_DEBUG
				for(int cont=path2.istroke, cont2=0; cont < path2.nstroke; cont++, cont2++)
					Console.WriteLine(String.Format("Index-stroke[{0}] x={1} y={2} u={3} v={4}",
						cont2, path2.stroke[cont].x,
						path2.stroke[cont].y,
						path2.stroke[cont].u,
						path2.stroke[cont].v));
#endif
            }

            //ctx.cache.verts = verts;

            return 1;
        }

        public static void nvgTransformScale(float[] t, float sx, float sy)
        {
            t[0] = sx;
            t[1] = 0.0f;
            t[2] = 0.0f;
            t[3] = sy;
            t[4] = 0.0f;
            t[5] = 0.0f;
        }

        public static int nvgTransformInverse(float[] inv, float[] t)
        {
            double invdet, det = (double)t[0] * t[3] - (double)t[2] * t[1];
            if (det > -1e-6 && det < 1e-6)
            {
                nvgTransformIdentity(inv);
                return 0;
            }
            invdet = 1.0 / det;
            inv[0] = (float)(t[3] * invdet);
            inv[2] = (float)(-t[2] * invdet);
            inv[4] = (float)(((double)t[2] * t[5] - (double)t[3] * t[4]) * invdet);
            inv[1] = (float)(-t[1] * invdet);
            inv[3] = (float)(t[0] * invdet);
            inv[5] = (float)(((double)t[1] * t[4] - (double)t[0] * t[5]) * invdet);
            return 1;
        }

        public static void nvgFill(NVGcontext ctx)
        {
            NVGstate state = nvg__getState(ctx);
            NVGpath path;
            NVGpaint fillPaint = state.fill.Clone();
            int i;

            nvg__flattenPaths(ctx);

            if (ctx.params_.edgeAntiAlias != 0)
                nvg__expandFill(ctx, ctx.fringeWidth, (int)NVGlineCap.NVG_MITER, 2.4f);
            else
                nvg__expandFill(ctx, 0.0f, (int)NVGlineCap.NVG_MITER, 2.4f);

            // Apply global alpha
            fillPaint.innerColor.a *= state.alpha;
            fillPaint.outerColor.a *= state.alpha;

            ctx.params_.renderFill(ctx.params_.userPtr, ref fillPaint, ref state.scissor, ctx.fringeWidth,
                ctx.cache.bounds, ctx.cache.paths, ctx.cache.npaths);

            // Count triangles
            for (i = 0; i < ctx.cache.npaths; i++)
            {
                path = ctx.cache.paths[i];
                ctx.fillTriCount += path.nfill - 2;
                ctx.fillTriCount += path.nstroke - 2;
                ctx.drawCallCount += 2;
            }
        }

        public static void nvgRect(NVGcontext ctx, float x, float y, float w, float h)
        {
            float[] vals =
            {
                (float)NVGcommands.NVG_MOVETO, x, y,
                (float)NVGcommands.NVG_LINETO, x, y + h,
                (float)NVGcommands.NVG_LINETO, x + w, y + h,
                (float)NVGcommands.NVG_LINETO, x + w, y,
                (float)NVGcommands.NVG_CLOSE
            };
            nvg__appendCommands(ctx, vals, NVG_COUNTOF(vals));
        }

        public static void nvgArc(NVGcontext ctx, float cx, float cy, float r, float a0, float a1, int dir)
        {
            float a = 0, da = 0, hda = 0, kappa = 0;
            float dx = 0, dy = 0, x = 0, y = 0, tanx = 0, tany = 0;
            float px = 0, py = 0, ptanx = 0, ptany = 0;
            float[] vals = new float[3 + 5 * 7 + 100];
            int i, ndivs, nvals;
            int move = ctx.ncommands > 0 ? (int)NVGcommands.NVG_LINETO : (int)NVGcommands.NVG_MOVETO;

            // Clamp angles
            da = a1 - a0;
            if (dir == (int)NVGwinding.NVG_CW)
            {
                if (nvg__absf(da) >= NVG_PI * 2)
                {
                    da = NVG_PI * 2;
                }
                else
                {
                    while (da < 0.0f)
                        da += NVG_PI * 2;
                }
            }
            else
            {
                if (nvg__absf(da) >= NVG_PI * 2)
                {
                    da = -NVG_PI * 2;
                }
                else
                {
                    while (da > 0.0f)
                        da -= NVG_PI * 2;
                }
            }

            // Split arc into max 90 degree segments.
            ndivs = nvg__maxi(1, nvg__mini((int)(nvg__absf(da) / (NVG_PI * 0.5f) + 0.5f), 5));
            hda = (da / (float)ndivs) / 2.0f;
            kappa = nvg__absf(4.0f / 3.0f * (1.0f - nvg__cosf(hda)) / nvg__sinf(hda));

            if (dir == (int)NVGwinding.NVG_CCW)
                kappa = -kappa;

            nvals = 0;
            for (i = 0; i <= ndivs; i++)
            {
                a = a0 + da * (i / (float)ndivs);
                dx = nvg__cosf(a);
                dy = nvg__sinf(a);
                x = cx + dx * r;
                y = cy + dy * r;
                tanx = -dy * r * kappa;
                tany = dx * r * kappa;

                if (i == 0)
                {
                    vals[nvals++] = (float)move;
                    vals[nvals++] = x;
                    vals[nvals++] = y;
                }
                else
                {
                    vals[nvals++] = (float)NVGcommands.NVG_BEZIERTO;
                    vals[nvals++] = px + ptanx;
                    vals[nvals++] = py + ptany;
                    vals[nvals++] = x - tanx;
                    vals[nvals++] = y - tany;
                    vals[nvals++] = x;
                    vals[nvals++] = y;
                }
                px = x;
                py = y;
                ptanx = tanx;
                ptany = tany;
            }

            nvg__appendCommands(ctx, vals, nvals);
        }

        public static void nvgRoundedRect(NVGcontext ctx, float x, float y, float w, float h, float r)
        {
            if (r < 0.1f)
            {
                nvgRect(ctx, x, y, w, h);
                return;
            }
            else
            {
                float rx = nvg__minf(r, nvg__absf(w) * 0.5f) * nvg__signf(w), ry = nvg__minf(r, nvg__absf(h) * 0.5f) * nvg__signf(h);
                float[] vals =
                {
                    (float)NVGcommands.NVG_MOVETO, x, y + ry,
                    (float)NVGcommands.NVG_LINETO, x, y + h - ry,
                    (float)NVGcommands.NVG_BEZIERTO, x, y + h - ry * (1 - NVG_KAPPA90), x + rx * (1 - NVG_KAPPA90), y + h, x + rx, y + h,
                    (float)NVGcommands.NVG_LINETO, x + w - rx, y + h,
                    (float)NVGcommands.NVG_BEZIERTO, x + w - rx * (1 - NVG_KAPPA90), y + h, x + w, y + h - ry * (1 - NVG_KAPPA90), x + w, y + h - ry,
                    (float)NVGcommands.NVG_LINETO, x + w, y + ry,
                    (float)NVGcommands.NVG_BEZIERTO, x + w, y + ry * (1 - NVG_KAPPA90), x + w - rx * (1 - NVG_KAPPA90), y, x + w - rx, y,
                    (float)NVGcommands.NVG_LINETO, x + rx, y,
                    (float)NVGcommands.NVG_BEZIERTO, x + rx * (1 - NVG_KAPPA90), y, x, y + ry * (1 - NVG_KAPPA90), x, y + ry,
                    (float)NVGcommands.NVG_CLOSE
                };
                nvg__appendCommands(ctx, vals, NVG_COUNTOF(vals));
            }
        }

        public static NVGpaint nvgLinearGradient(NVGcontext ctx,
                                                 float sx, float sy, float ex, float ey,
                                                 NVGcolor icol, NVGcolor ocol)
        {
            NVGpaint p = new NVGpaint();
            float dx, dy, d;
            const float large = (float)1e5;
            //NVG_NOTUSED(ctx);
            //memset(&p, 0, sizeof(p));

            // Calculate transform aligned to the line
            dx = ex - sx;
            dy = ey - sy;
            d = (float)System.Math.Sqrt(dx * dx + dy * dy);
            if (d > 0.0001f)
            {
                dx /= d;
                dy /= d;
            }
            else
            {
                dx = 0;
                dy = 1;
            }

            p.xform[0] = dy;
            p.xform[1] = -dx;
            p.xform[2] = dx;
            p.xform[3] = dy;
            p.xform[4] = sx - dx * large;
            p.xform[5] = sy - dy * large;

            p.extent[0] = large;
            p.extent[1] = large + d * 0.5f;

            p.radius = 0.0f;

            p.feather = nvg__maxf(1.0f, d);

            p.innerColor = icol;
            p.outerColor = ocol;

            return p;
        }

        static float nvg__quantize(float a, float d)
        {
            return ((int)(a / d + 0.5f)) * d;
        }

        public static NVGpaint nvgRadialGradient(NVGcontext ctx,
                                                 float cx, float cy, float inr, float outr,
                                                 NVGcolor icol, NVGcolor ocol)
        {
            NVGpaint p = new NVGpaint();
            float r = (inr + outr) * 0.5f;
            float f = (outr - inr);
            //NVG_NOTUSED(ctx);
            //memset(&p, 0, sizeof(p));

            nvgTransformIdentity(p.xform);
            p.xform[4] = cx;
            p.xform[5] = cy;

            p.extent[0] = r;
            p.extent[1] = r;

            p.radius = r;

            p.feather = nvg__maxf(1.0f, f);

            p.innerColor = icol;
            p.outerColor = ocol;

            return p;
        }

        public static void nvgEllipse(NVGcontext ctx, float cx, float cy, float rx, float ry)
        {
            float[] vals =
            {
                (float)NVGcommands.NVG_MOVETO, cx - rx, cy,
                (float)NVGcommands.NVG_BEZIERTO, cx - rx, cy + ry * NVG_KAPPA90, cx - rx * NVG_KAPPA90, cy + ry, cx, cy + ry,
                (float)NVGcommands.NVG_BEZIERTO, cx + rx * NVG_KAPPA90, cy + ry, cx + rx, cy + ry * NVG_KAPPA90, cx + rx, cy,
                (float)NVGcommands.NVG_BEZIERTO, cx + rx, cy - ry * NVG_KAPPA90, cx + rx * NVG_KAPPA90, cy - ry, cx, cy - ry,
                (float)NVGcommands.NVG_BEZIERTO, cx - rx * NVG_KAPPA90, cy - ry, cx - rx, cy - ry * NVG_KAPPA90, cx - rx, cy,
                (float)NVGcommands.NVG_CLOSE
            };
            nvg__appendCommands(ctx, vals, NVG_COUNTOF(vals));
        }

        public static void nvgCircle(NVGcontext ctx, float cx, float cy, float r)
        {
            nvgEllipse(ctx, cx, cy, r, r);
        }

        public static int nvgTextGlyphPositions(NVGcontext ctx, float x, float y, string string_,
                                                        NVGglyphPosition[] positions, int maxPositions)
        {
            NVGstate state = nvg__getState(ctx);
            float scale = nvg__getFontScale(state) * ctx.devicePxRatio;
            float invscale = 1.0f / scale;
            FONStextIter iter = new FONStextIter(), prevIter = new FONStextIter();
            FONSquad q = new FONSquad();
            int npos = 0;

            if (state.fontId == FontStash.FONS_INVALID)
                return 0;

            //if (end == NULL)
            //	end = string + strlen(string);

            //if (string_ == end)
            //	return 0;

            FontStash.fonsSetSize(ref ctx.fs, state.fontSize * scale);
            FontStash.fonsSetSpacing(ref ctx.fs, state.letterSpacing * scale);
            FontStash.fonsSetBlur(ref ctx.fs, state.fontBlur * scale);
            FontStash.fonsSetAlign(ctx.fs, (FONSalign)state.textAlign);
            FontStash.fonsSetFont(ref ctx.fs, state.fontId);
            byte[] str = System.Text.Encoding.UTF8.GetBytes(string_);
            FontStash.fonsTextIterInit(ctx.fs, ref iter, x * scale, y * scale, str);
            prevIter = iter;
            while (FontStash.fonsTextIterNext(ctx.fs, ref iter, ref q) != 0)
            {
                if (iter.prevGlyphIndex < 0 && nvg__allocTextAtlas(ctx) > 0)
                { // can not retrieve glyph?
                    iter = prevIter;
                    FontStash.fonsTextIterNext(ctx.fs, ref iter, ref q); // try again
                }
                prevIter = iter;
                positions[npos].str = iter.iStr;
                positions[npos].x = iter.x * invscale;
                positions[npos].minx = nvg__minf(iter.x, q.x0) * invscale;
                positions[npos].maxx = nvg__maxf(iter.nextx, q.x1) * invscale;
                npos++;
                if (npos >= maxPositions)
                    break;
            }

            return npos;
        }

        public static void nvgTextBox(NVGcontext ctx, float x, float y, float breakRowWidth, string string_)
        {
            NVGstate state = nvg__getState(ctx);
            NVGtextRow[] rows = new NVGtextRow[2];
            int nrows = 0, i;
            int oldAlign = state.textAlign;
            int haling = state.textAlign & ((int)NVGalign.NVG_ALIGN_LEFT | (int)NVGalign.NVG_ALIGN_CENTER | (int)NVGalign.NVG_ALIGN_RIGHT);
            int valign = state.textAlign & ((int)NVGalign.NVG_ALIGN_TOP | (int)NVGalign.NVG_ALIGN_MIDDLE | (int)NVGalign.NVG_ALIGN_BOTTOM | (int)NVGalign.NVG_ALIGN_BASELINE);
            float lineh = 0;
            float fnull = 0;

            if (state.fontId == FontStash.FONS_INVALID)
                return;

            nvgTextMetrics(ctx, ref fnull, ref fnull, ref lineh);

            state.textAlign = (int)NVGalign.NVG_ALIGN_LEFT | valign;

            while ((nrows = nvgTextBreakLines(ctx, string_, breakRowWidth, rows, 2)) > 0)
            {
                for (i = 0; i < nrows; i++)
                {
                    string str;
                    NVGtextRow row = rows[i];
                    if ((haling & (int)NVGalign.NVG_ALIGN_LEFT) != 0)
                    {
                        if (row.end > string_.Length) row.end = string_.Length;// - 1;      // Added Dec-04-17
                        if (row.start > string_.Length) row.start = 0;                   // Added Dec-05-17
                        str = string_.Substring(row.start, row.end - row.start);
                        nvgText(ctx, x, y, str);
                    }
                    else if ((haling & (int)NVGalign.NVG_ALIGN_CENTER) != 0)
                    {
                        if (row.end > string_.Length) row.end = string_.Length; // - 1;      // Added Dec-04-17
                        if (row.start > string_.Length) row.start = 0;                   // Added Dec-05-17
                        str = string_.Substring(row.start, row.end - row.start);
                        nvgText(ctx, x + breakRowWidth * 0.5f - row.width * 0.5f, y, str);
                    }
                    else if ((haling & (int)NVGalign.NVG_ALIGN_RIGHT) != 0)
                    {
                        if (row.end > string_.Length) row.end = string_.Length; // - 1;      // Added Dec-04-17
                        if (row.start > string_.Length) row.start = 0;                   // Added Dec-05-17
                        str = string_.Substring(row.start, row.end - row.start);
                        nvgText(ctx, x + breakRowWidth - row.width, y, str);
                    }
                    y += lineh * state.lineHeight;
                }
                
                int l = string_.Length;
                //string_ = string_.Substring(rows[nrows - 1].next);  // Original
                if (rows[nrows - 1].next > string_.Length) rows[nrows - 1].next = string_.Length - 1;     // Added Dec-05-17
                string_ = string_.Substring(rows[nrows - 1].next);                      // Added Dec-05-17

                if (string_.Length == 1)
                    string_ = "";
            }

            state.textAlign = oldAlign;
        }

        public static void nvgTextBoxBounds(NVGcontext ctx, float x, float y, float breakRowWidth, string string_, float[] bounds)
        {
            NVGstate state = nvg__getState(ctx);
            NVGtextRow[] rows = new NVGtextRow[2];
            float scale = nvg__getFontScale(state) * ctx.devicePxRatio;
            float invscale = 1.0f / scale;
            int nrows = 0, i;
            int oldAlign = state.textAlign;
            int haling = state.textAlign & ((int)NVGalign.NVG_ALIGN_LEFT | (int)NVGalign.NVG_ALIGN_CENTER | (int)NVGalign.NVG_ALIGN_RIGHT);
            int valign = state.textAlign & ((int)NVGalign.NVG_ALIGN_TOP | (int)NVGalign.NVG_ALIGN_MIDDLE | (int)NVGalign.NVG_ALIGN_BOTTOM | (int)NVGalign.NVG_ALIGN_BASELINE);
            float lineh = 0, rminy = 0, rmaxy = 0;
            float minx, miny, maxx, maxy;
            float fnull = 0;

            if (state.fontId == FontStash.FONS_INVALID)
            {
                if (bounds != null)
                    bounds[0] = bounds[1] = bounds[2] = bounds[3] = 0.0f;
                return;
            }

            NanoVG.nvgTextMetrics(ctx, ref fnull, ref fnull, ref lineh);

            state.textAlign = (int)NVGalign.NVG_ALIGN_LEFT | valign;

            minx = maxx = x;
            miny = maxy = y;

            FontStash.fonsSetSize(ref ctx.fs, state.fontSize * scale);
            FontStash.fonsSetSpacing(ref ctx.fs, state.letterSpacing * scale);
            FontStash.fonsSetBlur(ref ctx.fs, state.fontBlur * scale);
            FontStash.fonsSetAlign(ctx.fs, (FONSalign)state.textAlign);
            FontStash.fonsSetFont(ref ctx.fs, state.fontId);
            FontStash.fonsLineBounds(ctx.fs, 0, ref rminy, ref rmaxy);
            rminy *= invscale;
            rmaxy *= invscale;

            while ((nrows = nvgTextBreakLines(ctx, string_, breakRowWidth, rows, 2)) > 0)
            {
                for (i = 0; i < nrows; i++)
                {
                    NVGtextRow row = rows[i];
                    float rminx, rmaxx, dx = 0;
                    // Horizontal bounds
                    if ((haling & (int)NVGalign.NVG_ALIGN_LEFT) != 0)
                        dx = 0;
                    else if ((haling & (int)NVGalign.NVG_ALIGN_CENTER) != 0)
                        dx = breakRowWidth * 0.5f - row.width * 0.5f;
                    else if ((haling & (int)NVGalign.NVG_ALIGN_RIGHT) != 0)
                        dx = breakRowWidth - row.width;
                    rminx = x + row.minx + dx;
                    rmaxx = x + row.maxx + dx;
                    minx = nvg__minf(minx, rminx);
                    maxx = nvg__maxf(maxx, rmaxx);
                    // Vertical bounds.
                    miny = nvg__minf(miny, y + rminy);
                    maxy = nvg__maxf(maxy, y + rmaxy);

                    y += lineh * state.lineHeight;
                }
                string_ = string_.Substring(rows[nrows - 1].next);

                if (string_.Length == 1)
                    string_ = "";
            }

            state.textAlign = oldAlign;

            if (bounds != null)
            {
                bounds[0] = minx;
                bounds[1] = miny;
                bounds[2] = maxx;
                bounds[3] = maxy;
            }
        }

        public static float nvgTextBounds(NVGcontext ctx, float x, float y, string string_, float[] bounds = null)
        {
            float ret = 0f;

            try
            {
                byte[] str = System.Text.Encoding.UTF8.GetBytes(string_);
                ret = nvgTextBounds(ctx, x, y, str, bounds);
            }
            catch { }

            return ret;
        }

        public static float nvgTextBounds(NVGcontext ctx, float x, float y, byte[] str, float[] bounds)
        {
            float ret = 1f;
            try
            {
                NVGstate state = nvg__getState(ctx);
                float scale = nvg__getFontScale(state) * ctx.devicePxRatio;
                float invscale = 1.0f / scale;
                float width = 0f;

                if (state.fontId == FontStash.FONS_INVALID)
                    return 0;

                FontStash.fonsSetSize(ref ctx.fs, state.fontSize * scale);
                FontStash.fonsSetSpacing(ref ctx.fs, state.letterSpacing * scale);
                FontStash.fonsSetBlur(ref ctx.fs, state.fontBlur * scale);
                FontStash.fonsSetAlign(ctx.fs, (FONSalign)state.textAlign);
                FontStash.fonsSetFont(ref ctx.fs, state.fontId);

                try
                {
                    width = FontStash.fonsTextBounds(ref ctx.fs, x * scale, y * scale, str, bounds);
                }
                catch { }

                if (bounds != null)
                {
                    // Use line bounds for height.
                    FontStash.fonsLineBounds(ctx.fs, y * scale, ref bounds[1], ref bounds[3]);
                    bounds[0] *= invscale;
                    bounds[1] *= invscale;
                    bounds[2] *= invscale;
                    bounds[3] *= invscale;
                }
                return width * invscale;
            }
            catch
            {

            }
            return ret;
        }

        public static void nvgTextMetrics(NVGcontext ctx, ref float ascender, ref float descender, ref float lineh)
        {
            NVGstate state = nvg__getState(ctx);
            float scale = nvg__getFontScale(state) * ctx.devicePxRatio;
            float invscale = 1.0f / scale;

            if (state.fontId == FontStash.FONS_INVALID)
                return;

            FontStash.fonsSetSize(ref ctx.fs, state.fontSize * scale);
            FontStash.fonsSetSpacing(ref ctx.fs, state.letterSpacing * scale);
            FontStash.fonsSetBlur(ref ctx.fs, state.fontBlur * scale);
            FontStash.fonsSetAlign(ctx.fs, (FONSalign)state.textAlign);
            FontStash.fonsSetFont(ref ctx.fs, state.fontId);

            FontStash.fonsVertMetrics(ref ctx.fs, ref ascender, ref descender, ref lineh);
            ascender *= invscale;
            descender *= invscale;
            lineh *= invscale;
        }

        public static int nvgTextBreakLines(NVGcontext ctx, string string_,
                                     float breakRowWidth, NVGtextRow[] rows, int maxRows)
        {
            NVGstate        state               = nvg__getState(ctx);
            float           scale               = nvg__getFontScale(state) * ctx.devicePxRatio;
            float           invscale            = 1.0f / scale;
            FONStextIter    iter                = new FONStextIter();
            FONStextIter    prevIter            = new FONStextIter();
            FONSquad        q                   = new FONSquad();
            int             nrows               = 0;
            float           rowStartX           = 0;
            float           rowWidth            = 0;
            float           rowMinX             = 0;
            float           rowMaxX             = 0;
            int             rowStart            = -1;
            int             rowEnd              = -1;
            int             wordStart           = -1;
            float           wordStartX          = 0;
            float           wordMinX            = 0;
            int             breakEnd            = -1;
            float           breakWidth          = 0;
            float           breakMaxX           = 0;
            int             type_               = (int)NVGcodepointType.NVG_SPACE;
            int             ptype               = (int)NVGcodepointType.NVG_SPACE;
            uint            pcodepoint          = 0;

            if (string_ == null)
                return 0;
            int end = string_.Length - 1;

            if (maxRows == 0)
                return 0;
            if (state.fontId == FontStash.FONS_INVALID)
                return 0;

            FontStash.fonsSetSize(ref ctx.fs, state.fontSize * scale);
            FontStash.fonsSetSpacing(ref ctx.fs, state.letterSpacing * scale);
            FontStash.fonsSetBlur(ref ctx.fs, state.fontBlur * scale);
            FontStash.fonsSetAlign(ctx.fs, (FONSalign)state.textAlign);
            FontStash.fonsSetFont(ref ctx.fs, state.fontId);

            breakRowWidth *= scale;
            byte[] str = System.Text.Encoding.UTF8.GetBytes(string_);
            FontStash.fonsTextIterInit(ctx.fs, ref iter, 0, 0, str);
            prevIter = iter;
            while (FontStash.fonsTextIterNext(ctx.fs, ref iter, ref q) != 0)
            {
                // can not retrieve glyph?
                if (iter.prevGlyphIndex < 0 && nvg__allocTextAtlas(ctx) > 0)
                {
                    iter = prevIter;
                    FontStash.fonsTextIterNext(ctx.fs, ref iter, ref q); // try again
                }
                prevIter = iter;
                switch (iter.codepoint)
                {
                    case 9:         // \t
                    case 11:        // \v
                    case 12:        // \f
                    case 32:        // space
                    case 0x00a0:    // NBSP
                        type_ = (int)NVGcodepointType.NVG_SPACE;
                        break;
                    case 10:        // \n
                        type_ = pcodepoint == 13 ? (int)NVGcodepointType.NVG_SPACE : (int)NVGcodepointType.NVG_NEWLINE;
                        break;
                    case 13:        // \r
                        type_ = pcodepoint == 10 ? (int)NVGcodepointType.NVG_SPACE : (int)NVGcodepointType.NVG_NEWLINE;
                        break;
                    case 0x0085:    // NEL
                        type_ = (int)NVGcodepointType.NVG_NEWLINE;
                        break;
                    default:
                        type_ = (int)NVGcodepointType.NVG_CHAR;
                        break;
                }

                if (type_ == (int)NVGcodepointType.NVG_NEWLINE)
                {
                    // Always handle new lines.
                    rows[nrows].start = rowStart >= 0 ? rowStart : iter.iStr;
                    string rs = string_.Substring(rows[nrows].start);

                    rows[nrows].end = rowEnd >= 0 ? rowEnd : iter.iStr;
                    string re = string_.Substring(rows[nrows].end);

                    rows[nrows].width = rowWidth * invscale;
                    rows[nrows].minx = rowMinX * invscale;
                    rows[nrows].maxx = rowMaxX * invscale;
                    rows[nrows].next = iter.iNext;
                    string inx = string_.Substring(rows[nrows].next);

                    nrows++;
                    if (nrows >= maxRows)
                        return nrows;
                    
                    // Set null break point
                    breakEnd = rowStart;
                    breakWidth = 0.0f;
                    breakMaxX = 0.0f;
                    // Indicate to skip the white space at the beginning of the row.
                    rowStart = -1;
                    rowEnd = -1;
                    rowWidth = 0;
                    rowMinX = rowMaxX = 0;
                }
                else
                {
                    if (rowStart < 0)
                    {
                        // Skip white space until the beginning of the line
                        if (type_ == (int)NVGcodepointType.NVG_CHAR)
                        {
                            // The current char is the row so far
                            rowStartX = iter.x;
                            rowStart = iter.iStr;
                            string rs = string_.Substring(rowStart);

                            rowEnd = iter.iNext;

                            if (rowEnd > string_.Length) rowEnd = string_.Length;

                            string re = string_.Substring(rowEnd);

                            rowWidth = iter.nextx - rowStartX; // q.x1 - rowStartX;
                            rowMinX = q.x0 - rowStartX;
                            rowMaxX = q.x1 - rowStartX;
                            wordStart = iter.iStr;
                            string ws = string_.Substring(wordStart);

                            wordStartX = iter.x;
                            wordMinX = q.x0 - rowStartX;
                            // Set null break point
                            breakEnd = rowStart;
                            breakWidth = 0.0f;
                            breakMaxX = 0.0f;
                        }
                    }
                    else
                    {
                        float nextWidth = iter.nextx - rowStartX;

                        // track last non-white space character
                        if (type_ == (int)NVGcodepointType.NVG_CHAR)
                        {
                            rowEnd = iter.iNext;
                            rowWidth = iter.nextx - rowStartX;
                            rowMaxX = q.x1 - rowStartX;
                        }
                        // track last end of a word
                        if (ptype == (int)NVGcodepointType.NVG_CHAR && type_ == (int)NVGcodepointType.NVG_SPACE)
                        {
                            breakEnd = iter.iStr;
                            breakWidth = rowWidth;
                            breakMaxX = rowMaxX;
                        }
                        // track last beginning of a word
                        if (ptype == (int)NVGcodepointType.NVG_SPACE && type_ == (int)NVGcodepointType.NVG_CHAR)
                        {
                            wordStart = iter.iStr;
                            wordStartX = iter.x;
                            wordMinX = q.x0 - rowStartX;
                        }

                        // Break to new line when a character is beyond break width.
                        if (type_ == (int)NVGcodepointType.NVG_CHAR && nextWidth > breakRowWidth)
                        {
                            // The run length is too long, need to break to new line.
                            if (breakEnd == rowStart)
                            {

                                //try   // Dec-04-17 Handler
                                {

                                    // The current word is longer than the row length, just break it from here.
                                    rows[nrows].start = rowStart;
                                    rows[nrows].end = iter.iStr;
                                    rows[nrows].width = rowWidth * invscale;
                                    rows[nrows].minx = rowMinX * invscale;
                                    rows[nrows].maxx = rowMaxX * invscale;
                                    rows[nrows].next = iter.iStr;
                                    nrows++;
                                    if (nrows >= maxRows)
                                        return nrows;
                                    rowStartX = iter.x;
                                    rowStart = iter.iStr;
                                    rowEnd = iter.iNext;
                                    rowWidth = iter.nextx - rowStartX;
                                    rowMinX = q.x0 - rowStartX;
                                    rowMaxX = q.x1 - rowStartX;
                                    wordStart = iter.iStr;
                                    wordStartX = iter.x;
                                    wordMinX = q.x0 - rowStartX;


                                }
                                //catch { }  // Dec-04-17 Handler

                            }
                            else
                            {

                                //try   // Dec-04-17 Handler
                                {

                                    // Break the line from the end of the last word, and start new line from the beginning of the new.
                                    rows[nrows].start = rowStart;
                                    rows[nrows].end = breakEnd;
                                    rows[nrows].width = breakWidth * invscale;
                                    rows[nrows].minx = rowMinX * invscale;
                                    rows[nrows].maxx = breakMaxX * invscale;
                                    rows[nrows].next = wordStart;
                                    nrows++;
                                    if (nrows >= maxRows)
                                        return nrows;
                                    rowStartX = wordStartX;
                                    rowStart = wordStart;
                                    rowEnd = iter.iNext;
                                    rowWidth = iter.nextx - rowStartX;
                                    rowMinX = wordMinX;
                                    rowMaxX = q.x1 - rowStartX;
                                    // No change to the word start

                                }
                                //catch { }  // Dec-04-17 Handler

                            }
                            // Set null break point
                            breakEnd = rowStart;
                            breakWidth = 0.0f;
                            breakMaxX = 0.0f;
                        }
                    }
                }

                pcodepoint = iter.codepoint;
                ptype = type_;
            }

            // Break the line from the end of the last word, and start new line from the beginning of the new.
            if (rowStart >= 0)
            {
                rows[nrows].start = rowStart;
                rows[nrows].end = rowEnd;
                rows[nrows].width = rowWidth * invscale;
                rows[nrows].minx = rowMinX * invscale;
                rows[nrows].maxx = rowMaxX * invscale;
                rows[nrows].next = end;
                nrows++;
            }

            return nrows;
        }

        public static void nvgTextLineHeight(NVGcontext ctx, float lineHeight)
        {
            NVGstate state = nvg__getState(ctx);
            state.lineHeight = lineHeight;
        }

        public static void nvgTextAlign(NVGcontext ctx, int align)
        {
            NVGstate state = nvg__getState(ctx);
            state.textAlign = align;
        }

        static float nvg__getFontScale(NVGstate state)
        {
            return nvg__minf(nvg__quantize(nvg__getAverageScale(state.xform), 0.01f), 4.0f);
        }

        public static void nvgImageSize(NVGcontext ctx, int image, ref int w, ref int h)
        {
            ctx.params_.renderGetTextureSize(ctx.params_.userPtr, image, ref w, ref h);
        }

        public static NVGpaint nvgImagePattern(NVGcontext ctx,
                                               float cx, float cy, float w, float h, 
                                               int image, 
                                               float angle = 0f, float alpha = 1f)
        {
            NVGpaint p = new NVGpaint();
            //NVG_NOTUSED(ctx);
            //memset(&p, 0, sizeof(p));

            nvgTransformRotate(p.xform, angle);
            p.xform[4] = cx;
            p.xform[5] = cy;

            p.extent[0] = w;
            p.extent[1] = h;

            p.image = image;

            p.innerColor = p.outerColor = nvgRGBAf(1, 1, 1, alpha);

            return p;
        }

        static int nvg__allocTextAtlas(NVGcontext ctx)
        {
            int iw = 0, ih = 0;
            nvg__flushTextTexture(ctx);
            if (ctx.fontImageIdx >= NVG_MAX_FONTIMAGES - 1)
                return 0;
            // if next fontImage already have a texture
            if (ctx.fontImages[ctx.fontImageIdx + 1] != 0)
                nvgImageSize(ctx, ctx.fontImages[ctx.fontImageIdx + 1], ref iw, ref ih);
            else
            { // calculate the new font image size and create it.
                nvgImageSize(ctx, ctx.fontImages[ctx.fontImageIdx], ref iw, ref ih);
                if (iw > ih)
                    ih *= 2;
                else
                    iw *= 2;
                if (iw > NVG_MAX_FONTIMAGE_SIZE || ih > NVG_MAX_FONTIMAGE_SIZE)
                    iw = ih = NVG_MAX_FONTIMAGE_SIZE;
                ctx.fontImages[ctx.fontImageIdx + 1] = ctx.params_.renderCreateTexture(ctx.params_.userPtr,
                    (int)NVGtexture.NVG_TEXTURE_ALPHA, iw, ih, 0, null);
            }
            ++ctx.fontImageIdx;
            FontStash.fonsResetAtlas(ctx.fs, iw, ih);
            return 1;
        }

        public static float nvgText(NVGcontext ctx, float x, float y, string string_)
        {
            float ret = 0f;

            if (String.IsNullOrEmpty(string_))
                return ret;

            try
            {
                byte[] str = System.Text.Encoding.UTF8.GetBytes(string_);
                ret = nvgText(ctx, x, y, str);
            }
            catch { }


            return ret;
        }

        public static float nvgText(NVGcontext ctx, float x, float y, byte[] str)
        {
            NVGstate state = nvg__getState(ctx);
            FONStextIter iter = new FONStextIter(), prevIter = new FONStextIter();
            FONSquad q = new FONSquad();
            NVGvertex[] verts;
            float scale = nvg__getFontScale(state) * ctx.devicePxRatio;
            float invscale = 1.0f / scale;
            int cverts = 0;
            int nverts = 0;

            //int end = string_.Length;
            int end = str.Length;

            if (state.fontId == FontStash.FONS_INVALID)
                return x;

            FontStash.fonsSetSize(ref ctx.fs, state.fontSize * scale);
            FontStash.fonsSetSpacing(ref ctx.fs, state.letterSpacing * scale);
            FontStash.fonsSetBlur(ref ctx.fs, state.fontBlur * scale);
            FontStash.fonsSetAlign(ctx.fs, (FONSalign)state.textAlign);
            FontStash.fonsSetFont(ref ctx.fs, state.fontId);

            cverts = nvg__maxi(2, end) * 6; // conservative estimate.
            verts = nvg__allocTempVerts(ctx, cverts);
            if (verts == null)
                return x;

            FontStash.fonsTextIterInit(ctx.fs, ref iter, x * scale, y * scale, str);
            prevIter = iter;
            while (FontStash.fonsTextIterNext(ctx.fs, ref iter, ref q) != 0)
            {
                float[] c = new float[4 * 2];
                if (iter.prevGlyphIndex == -1)
                { // can not retrieve glyph?
                    if (nvg__allocTextAtlas(ctx) == 0)
                        break; // no memory :(
                    if (nverts != 0)
                    {
                        nvg__renderText(ctx, verts, nverts);
                        nverts = 0;
                    }
                    iter = prevIter;
                    FontStash.fonsTextIterNext(ctx.fs, ref iter, ref q); // try again
                    if (iter.prevGlyphIndex == -1) // still can not find glyph?
                        break;
                }
                prevIter = iter;
                // Transform corners.
                nvgTransformPoint(ref c[0], ref c[1], state.xform, q.x0 * invscale, q.y0 * invscale);
                nvgTransformPoint(ref c[2], ref c[3], state.xform, q.x1 * invscale, q.y0 * invscale);
                nvgTransformPoint(ref c[4], ref c[5], state.xform, q.x1 * invscale, q.y1 * invscale);
                nvgTransformPoint(ref c[6], ref c[7], state.xform, q.x0 * invscale, q.y1 * invscale);
                // Create triangles
                if (nverts + 6 <= cverts)
                {
                    nvg__vset(ref verts[nverts], c[0], c[1], q.s0, q.t0);
                    nverts++;
                    nvg__vset(ref verts[nverts], c[4], c[5], q.s1, q.t1);
                    nverts++;
                    nvg__vset(ref verts[nverts], c[2], c[3], q.s1, q.t0);
                    nverts++;
                    nvg__vset(ref verts[nverts], c[0], c[1], q.s0, q.t0);
                    nverts++;
                    nvg__vset(ref verts[nverts], c[6], c[7], q.s0, q.t1);
                    nverts++;
                    nvg__vset(ref verts[nverts], c[4], c[5], q.s1, q.t1);
                    nverts++;
                }
            }

            //ctx.cache.verts = verts;

            // TODO: add back-end bit to do this just once per frame.
            nvg__flushTextTexture(ctx);

            nvg__renderText(ctx, verts, nverts);

            return iter.x;
        }

        static void nvg__renderText(NVGcontext ctx, NVGvertex[] verts, int nverts)
        {
            NVGstate state = nvg__getState(ctx);
            // last change
            NVGpaint paint = state.fill.Clone();

            // Render triangles.
            paint.image = ctx.fontImages[ctx.fontImageIdx];

            // Apply global alpha
            paint.innerColor.a *= state.alpha;
            paint.outerColor.a *= state.alpha;

            ctx.params_.renderTriangles(ctx.params_.userPtr, ref paint, ref state.scissor, verts, nverts);

            ctx.drawCallCount++;
            ctx.textTriCount += nverts / 3;
        }

        static void nvg__flushTextTexture(NVGcontext ctx)
        {
            int[] dirty = new int[4];

            if (FontStash.fonsValidateTexture(ctx.fs, dirty) != 0)
            {
                int fontImage = ctx.fontImages[ctx.fontImageIdx];
                // Update texture
                if (fontImage != 0)
                {
                    int iw = 0, ih = 0;
                    byte[] data = FontStash.fonsGetTextureData(ctx.fs, ref iw, ref ih);
                    int x = dirty[0];
                    int y = dirty[1];
                    int w = dirty[2] - dirty[0];
                    int h = dirty[3] - dirty[1];
                    ctx.params_.renderUpdateTexture(ctx.params_.userPtr, fontImage, x, y, w, h, data);
                }
            }
        }

        public static void nvgGlobalAlpha(NVGcontext ctx, float alpha)
        {
            NVGstate state = nvg__getState(ctx);
            state.alpha = alpha;
        }

        static void nvgTransformIdentity(float[] t)
        {
            t[0] = 1.0f;
            t[1] = 0.0f;
            t[2] = 0.0f;
            t[3] = 1.0f;
            t[4] = 0.0f;
            t[5] = 0.0f;
        }

        static float nvg__hue(float h, float m1, float m2)
        {
            if (h < 0)
                h += 1;
            if (h > 1)
                h -= 1;
            if (h < 1.0f / 6.0f)
                return m1 + (m2 - m1) * h * 6.0f;
            else if (h < 3.0f / 6.0f)
                return m2;
            else if (h < 4.0f / 6.0f)
                return m1 + (m2 - m1) * (2.0f / 3.0f - h) * 6.0f;
            return m1;
        }

        public static NVGcolor nvgHSLA(float h, float s, float l, byte a)
        {
            float m1, m2;
            NVGcolor col;
            h = nvg__modf(h, 1.0f);
            if (h < 0.0f)
                h += 1.0f;
            s = nvg__clampf(s, 0.0f, 1.0f);
            l = nvg__clampf(l, 0.0f, 1.0f);
            m2 = l <= 0.5f ? (l * (1 + s)) : (l + s - l * s);
            m1 = 2 * l - m2;
            col.r = nvg__clampf(nvg__hue(h + 1.0f / 3.0f, m1, m2), 0.0f, 1.0f);
            col.g = nvg__clampf(nvg__hue(h, m1, m2), 0.0f, 1.0f);
            col.b = nvg__clampf(nvg__hue(h - 1.0f / 3.0f, m1, m2), 0.0f, 1.0f);
            col.a = a / 255.0f;
            return col;
        }

        public static int nvgAddFallbackFontId(NVGcontext ctx, int baseFont, int fallbackFont)
        {
            if (baseFont == -1 || fallbackFont == -1)
                return 0;
            return FontStash.fonsAddFallbackFont(ctx.fs, baseFont, fallbackFont);
        }

        public static void nvgBeginFrame(NVGcontext ctx, int windowWidth, int windowHeight, float devicePixelRatio)
        {
            if (null == ctx.params_.userPtr) return; // Line Added 12-Jan-2020

            ctx.nstates = 0;
            nvgSave(ctx);
            nvgReset(ctx);

            nvg__setDevicePixelRatio(ref ctx, devicePixelRatio);

            ctx.params_.renderViewport(ctx.params_.userPtr, windowWidth, windowHeight, devicePixelRatio);

            ctx.drawCallCount = 0;
            ctx.fillTriCount = 0;
            ctx.strokeTriCount = 0;
            ctx.textTriCount = 0;
        }

        /// <summary>
        /// Create font from *.ttf <see cref="fileName"/>.
        /// </summary>
        /// <returns>The create font id.</returns>
        /// <param name="ctx">NanoVG context.</param>
        /// <param name="internalFontName">Internal font name.</param>
        /// <param name="fileName">File name of *.ttf font file (can include a path).</param>
        public static int nvgCreateFont(NVGcontext ctx, string internalFontName, string fileName)
        {
            return FontStash.fonsAddFont(ctx.fs, internalFontName, fileName);
        }

        public static byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }

        static int nvgCreateImageRGBA(ref NVGcontext ctx, int w, int h, int imageFlags, byte[] data)
        {
            return ctx.params_.renderCreateTexture(ctx.params_.userPtr, (int)NVGtexture.NVG_TEXTURE_RGBA, w, h, imageFlags, data);
        }

        static int nvgCreateImageRGBA(ref NVGcontext ctx, int w, int h, int imageFlags, Bitmap bmp)
        {
            return ctx.params_.renderCreateTexture2(ctx.params_.userPtr, (int)NVGtexture.NVG_TEXTURE_RGBA, w, h, imageFlags, bmp);
        }

        static int nvgCreateImageRGBAFromIndex(ref NVGcontext ctx, int w, int h, int imageFlags, int textureIndex)
        {
            return ctx.params_.renderCreateTextureFBO(ctx.params_.userPtr, (int)NVGtexture.NVG_TEXTURE_RGBA, w, h, imageFlags, textureIndex);
        }

       

        /// <summary>
        /// Convert a bitmap to a byte array
        /// </summary>
        /// <param name="bitmap">image to convert</param>
        /// <returns>image as bytes</returns>
        private static byte[] ConvertBitmap(Bitmap bitmap)
        {
            //Code excerpted from Microsoft Robotics Studio v1.5
            BitmapData raw = null;  //used to get attributes of the image
            byte[] rawImage = null; //the image as a byte[]

            try
            {
                //Freeze the image in memory
                raw = bitmap.LockBits(
                    new Rectangle(0, 0, (int)bitmap.Width, (int)bitmap.Height),
                    ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb
                );

                int size = raw.Height * raw.Stride;
                rawImage = new byte[size];

                //Copy the image into the byte[]
                System.Runtime.InteropServices.Marshal.Copy(raw.Scan0, rawImage, 0, size);
            }
            finally
            {
                if (raw != null)
                {
                    //Unfreeze the memory for the image
                    bitmap.UnlockBits(raw);
                }
            }
            return rawImage;
        }

        public static int nvgCreateImage(ref NVGcontext ctx, string filename, int imageFlags)
        {
            int imageIdx = -1;

            Image bmp = null;

            try
            {

                if (ImagesLookupTable.ContainsKey(filename))
                    return ImagesLookupTable[filename];
                
                using (FileStream fs = new FileStream(filename, FileMode.Open))
                {
                    bmp = Image.FromStream(fs);

                    imageIdx = nvgCreateImageRGBA(ref ctx, bmp.Width, bmp.Height, imageFlags, (Bitmap)bmp);

                    if (imageIdx > 0 && !ImagesLookupTable.ContainsKey(filename))
                        ImagesLookupTable.Add(filename, imageIdx);

                    if (imageIdx == 0)
                        throw new Exception(string.Format("Failed to Create OpenGL Image {0}", filename));
                }
            }
            catch (Exception cE)
            {
                throw new Exception(string.Format("nvgCreateImage.Exception {0}\n{1}", cE.Message, cE.StackTrace));       
            }
            
            return imageIdx;
        }

        
        #region Extended (Used By Pulxui)
        public static int CreateImage(NVGcontext ctx, Bitmap bmp, int imageFlags = 0)
        {
            int image = 0;



            image = nvgCreateImageRGBA(ref ctx, bmp.Width, bmp.Height, imageFlags, bmp);

            

            return image;
        }
        public static int CreateImage(NVGcontext ctx, int textureIndex, int width, int height, int imageFlags = 0)
        {
            int image = 0;

            image = nvgCreateImageRGBAFromIndex(ref ctx, width, height, imageFlags, textureIndex);

            return image;
        }

        //public static int nvgCreateImage(NVGcontext ctx, uint texture, int imageflags = 0)
        //{
        //    int image = 0;

        //    ctx.params_.renderCreateTexture2
        //    nvglCreateImageFromHandleGL3()

        //    return image;
        //}

        public static int CreateFont(NVGcontext ctx, string internalFontName, string fileName)
        {
            return FontStash.fonsAddFont(ctx.fs, internalFontName, fileName);
        }

        public static int CreateFont(NVGcontext ctx, string internalFontName, byte[] data)
        {
            return FontStash.fonsAddFontMem(ctx.fs, internalFontName, data, (uint)data.Length);
        }

        public static bool FontCreated(NVGcontext ctx, string internalFontName)
        {
            return FontStash.fonsFontCreated(ctx.fs, internalFontName);
        }

        public static bool FontCreated(NVGcontext ctx, int idFont)
        {
            return FontStash.fonsFontCreated(ctx.fs, idFont);
        }
        #endregion Extended

        public static Dictionary<string, int> ImagesLookupTable = new Dictionary<string, int>();

    }

    #region Auxiliary-classes-structs

    public class NVGpoint
    {
        public float x, y;
        public float dx, dy;
        public float len;
        public float dmx, dmy;
        public byte flags;

        public override string ToString()
        {
            return string.Format("[NVGpoint]x={0}, y={1}, dx={2}, dy={3}, len={4}, dmx={5}, dmy={6}",
                x, y, dx, dy, len);
        }

        public NVGpoint Clone()
        {
            NVGpoint newpoint = new NVGpoint();

            newpoint.x = this.x;
            newpoint.y = this.y;
            newpoint.dx = this.dx;
            newpoint.dy = this.dy;
            newpoint.len = this.len;
            newpoint.dmx = this.dmx;
            newpoint.dmy = this.dmy;
            newpoint.flags = this.flags;

            return newpoint;
        }
    }

    public struct NVGvertex
    {
        public float x;
        public float y;
        public float u;
        public float v;

        public override string ToString()
        {
            return string.Format("[NVGvertex] x={0} y={1} u={2} v={3}", x, y, u, v);
        }
    }

    public class NVGpath
    {
        public int first;
        public int count;
        public byte closed;
        public int nbevel;
        public NVGvertex[] fill;
        public int ifill;
        public int nfill;
        public NVGvertex[] stroke;
        public int istroke;
        public int nstroke;
        public int winding;
        public int convex;
    }

    public class NVGpathCache
    {
        public NVGpoint[] points;
        public int npoints;
        public int cpoints;
        public NVGpath[] paths;
        public int npaths;
        public int cpaths;
        public NVGvertex[] verts;
        public int nverts;
        public int cverts;
        //[4];
        public float[] bounds;

        public NVGpathCache()
        {
            bounds = new float[4];
        }
    }

    public struct NVGparams
    {
        public object userPtr;
        public int edgeAntiAlias;
        public RenderCreateHandler                          renderCreate;

        public RenderCreateTextureHandler                   renderCreateTexture;
        public RenderCreateTextureHandler2                  renderCreateTexture2;
        public RenderCreateTextureFromIndexHandler          renderCreateTextureFBO;
        public RenderViewportHandler                        renderViewport;
        public RenderFlushHandler                           renderFlush;
        public RenderFillHandler                            renderFill;
        public RenderStrokeHandler                          renderStroke;
        public RenderTrianglesHandler                       renderTriangles;
        public RenderUpdateTextureHandler                   renderUpdateTexture;
        public RenderGetTextureSizeHandler                  renderGetTextureSize;
        public RenderDeleteTexture                          renderDeleteTexture;
        public RenderCancel                                 renderCancel;
        public RenderDelete                                 renderDelete;
    }

    public class NVGcontext
    {
        public NVGparams params_;
        public float[] commands;
        public int ccommands;
        public int ncommands;
        public float commandx, commandy;
        //[NVG_MAX_STATES];
        public NVGstate[] states;
        public int nstates;
        public NVGpathCache cache;
        public float tessTol;
        public float distTol;
        public float fringeWidth;
        public float devicePxRatio;
        public FONScontext fs;
        //[NVG_MAX_FONTIMAGES];
        public int[] fontImages;
        public int fontImageIdx;
        public int drawCallCount;
        public int fillTriCount;
        public int strokeTriCount;
        public int textTriCount;

        public NVGcontext()
        {
            states = new NVGstate[NanoVG.NVG_MAX_STATES];
            for (int cont = 0; cont < states.Length; cont++)
                states[cont] = new NVGstate();
            fontImages = new int[NanoVG.NVG_MAX_FONTIMAGES];
        }
    }

    public struct NVGcompositeOperationState
    {
        public int srcRGB;
        public int dstRGB;
        public int srcAlpha;
        public int dstAlpha;
    }

    //[StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct Rgba
    {
        //[FieldOffset(0)]
        public float r;
        //[FieldOffset(4)]
        public float g;
        //[FieldOffset(8)]
        public float b;
        //[FieldOffset(16)]
        public float a;
    }

    public struct NVGcolor
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public override string ToString()
        {
            return string.Format("[NVGcolor: r={0}, g={1}, b={2}, a={3}]", r, g, b, a);
        }
    }

    public class NVGscissor
    {
        //[6];
        public float[] xform;
        //[2];
        public float[] extent;

        public NVGscissor()
        {
            xform = new float[6];
            extent = new float[2];
        }

        public NVGscissor Clone()
        {
            NVGscissor newScissor = new NVGscissor();

            Array.Copy(this.xform, newScissor.xform, this.xform.Length);
            Array.Copy(this.extent, newScissor.extent, this.extent.Length);

            return newScissor;
        }
    }

    public class NVGstate
    {
        public NVGcompositeOperationState compositeOperation;
        public NVGpaint fill;
        public NVGpaint stroke;
        public float strokeWidth;
        public float miterLimit;
        public int lineJoin;
        public int lineCap;
        public float alpha;
        //[6];
        public float[] xform;
        public NVGscissor scissor;
        public float fontSize;
        public float letterSpacing;
        public float lineHeight;
        public float fontBlur;
        public int textAlign;
        public int fontId;

        public NVGstate()
        {
            xform = new float[6];
            scissor = new NVGscissor();
            fill = new NVGpaint();
            stroke = new NVGpaint();
        }

        public NVGstate Clone()
        {
            NVGstate newState = new NVGstate();
            newState.compositeOperation = this.compositeOperation;
            newState.fill = this.fill.Clone();
            newState.stroke = this.stroke.Clone();
            newState.strokeWidth = this.strokeWidth;
            newState.miterLimit = this.miterLimit;
            newState.lineJoin = this.lineJoin;
            newState.lineCap = this.lineCap;
            newState.alpha = this.alpha;

            Array.Copy(this.xform, newState.xform, this.xform.Length);

            newState.scissor = this.scissor.Clone();
            newState.fontSize = this.fontSize;
            newState.letterSpacing = this.letterSpacing;
            newState.lineHeight = this.lineHeight;
            newState.fontBlur = this.fontBlur;
            newState.textAlign = this.textAlign;
            newState.fontId = this.fontId;

            return newState;
        }
    }

    public class NVGpaint
    {
        //[6];
        public float[] xform;
        //[2];
        public float[] extent;
        public float radius;
        public float feather;
        public NVGcolor innerColor;
        public NVGcolor outerColor;
        public int image;

        public NVGpaint()
        {
            xform = new float[6];
            extent = new float[2];
        }

        public NVGpaint Clone()
        {
            NVGpaint newPaint = new NVGpaint();

            Array.Copy(this.xform, newPaint.xform, this.xform.Length);
            Array.Copy(this.extent, newPaint.extent, this.extent.Length);
            newPaint.radius = this.radius;
            newPaint.feather = this.feather;
            newPaint.innerColor = this.innerColor;
            newPaint.outerColor = this.outerColor;
            newPaint.image = this.image;

            return newPaint;
        }
    }

    public struct NVGtextRow
    {
        public int start;
        // Pointer to the input text where the row starts.
        public int end;
        // Pointer to the input text where the row ends (one past the last character).
        public int next;
        // Pointer to the beginning of the next row.
        public float width;
        // Logical width of the row.
        public float minx, maxx;
        // Actual bounds of the row. Logical with and bounds can differ because of kerning and some parts over extending.
    }

    public struct NVGglyphPosition
    {
        public int str;
        // Position of the glyph in the input string.
        public float x;
        // The x-coordinate of the logical glyph position.
        public float minx, maxx;
        // The bounds of the glyph shape.
    }
    #endregion Auxiliary-classes-structs
}
