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

namespace NthDimension.Rasterizer.NanoVG.FontStashNet
{
    public static class Stb_truetype
    {
        const int FIXSHIFT = 10;
        const int FIX = (1 << FIXSHIFT);
        const int FIXMASK = (FIX - 1);

        static byte ttBYTE(byte[] p, int offset)
        {
            return (byte)(p[0 + offset]);
        }

        static sbyte ttCHAR(byte[] p, int offset)
        {
            return (sbyte)(p[0 + offset]);
        }

        static ushort ttUSHORT(byte[] p, int offset)
        {
            return (ushort)(p[0 + offset] * 256 + p[1 + offset]);
        }

        static short ttSHORT(byte[] p, int offset)
        {
            return (short)(p[0 + offset] * 256 + p[1 + offset]);
        }

        static uint ttULONG(byte[] p, int offset)
        {
            return (uint)((p[0 + offset] << 24) + (p[1 + offset] << 16) + (p[2 + offset] << 8) + p[3 + offset]);
        }

        static int ttLONG(byte[] p, int offset)
        {
            return (p[0 + offset] << 24) + (p[1 + offset] << 16) + (p[2 + offset] << 8) + p[3 + offset];
        }

        static bool stbtt_tag4(byte[] p, int offset, char c0, char c1, char c2, char c3)
        {
            return (p[0 + offset] == c0 &&
            p[1 + offset] == c1 &&
            p[2 + offset] == c2 &&
            p[3 + offset] == c3);
        }

        static bool stbtt_tag(byte[] p, int offset, string str)
        {
            return stbtt_tag4(p, offset, str[0], str[1], str[2], str[3]);
        }

        // @OPTIMIZE: binary search
        static int stbtt__find_table(byte[] data, int fontstart, string tag)
        {
            int num_tables = ttUSHORT(data, fontstart + 4);
            uint tabledir = (uint)(fontstart + 12);
            int i;
            for (i = 0; i < num_tables; ++i)
            {
                uint loc = (uint)(tabledir + 16 * i);
                if (stbtt_tag(data, (int)loc + 0, tag))
                    return (int)ttULONG(data, (int)(loc + 8));
            }
            return 0;
        }

        public static int stbtt_InitFont(ref stbtt_fontinfo info, byte[] data2, int fontstart)
        {
            byte[] data = data2;
            int cmap, t;
            int i, numTables;

            info.data = data;
            info.fontstart = fontstart;

            cmap = stbtt__find_table(data, fontstart, "cmap");       // required
            info.loca = stbtt__find_table(data, fontstart, "loca"); // required
            info.head = stbtt__find_table(data, fontstart, "head"); // required
            info.glyf = stbtt__find_table(data, fontstart, "glyf"); // required
            info.hhea = stbtt__find_table(data, fontstart, "hhea"); // required
            info.hmtx = stbtt__find_table(data, fontstart, "hmtx"); // required
            info.kern = stbtt__find_table(data, fontstart, "kern"); // not required
            if (!(cmap != 0) || !(info.loca != 0) || !(info.head != 0) ||
                !(info.glyf != 0) || !(info.hhea != 0) || !(info.hmtx != 0))
                return 0;

            t = stbtt__find_table(data, fontstart, "maxp");
            if (t != 0)
                info.numGlyphs = ttUSHORT(data, t + 4);
            else
                info.numGlyphs = 0xffff;

            // find a cmap encoding table we understand *now* to avoid searching
            // later. (todo: could make this installable)
            // the same regardless of glyph.
            numTables = ttUSHORT(data, cmap + 2);
            info.index_map = 0;
            for (i = 0; i < numTables; ++i)
            {
                int encoding_record = cmap + 4 + 8 * i;
                // find an encoding we understand:
                switch ((STBTT_PLATFORM_ID)ttUSHORT(data, encoding_record))
                {
                    case STBTT_PLATFORM_ID.STBTT_PLATFORM_ID_MICROSOFT:
                        switch ((STBTT_PLATFORM_ID_MICROSOFT)ttUSHORT(data, encoding_record + 2))
                        {
                            case STBTT_PLATFORM_ID_MICROSOFT.STBTT_MS_EID_UNICODE_BMP:
                            case STBTT_PLATFORM_ID_MICROSOFT.STBTT_MS_EID_UNICODE_FULL:
                                // MS/Unicode
                                info.index_map = (int)(cmap + ttULONG(data, encoding_record + 4));
                                break;
                        }
                        break;
                }
            }
            if (info.index_map == 0)
                return 0;

            info.indexToLocFormat = ttUSHORT(data, info.head + 50);

            return 1;
        }

        public static void stbtt_GetFontVMetrics(ref stbtt_fontinfo info,
                                                 ref int ascent, ref int descent, ref int lineGap)
        {
            ascent = ttSHORT(info.data, info.hhea + 4);
            descent = ttSHORT(info.data, info.hhea + 6);
            lineGap = ttSHORT(info.data, info.hhea + 8);
        }

        public static float stbtt_ScaleForPixelHeight(ref stbtt_fontinfo info, float height)
        {
            int fheight = ttSHORT(info.data, info.hhea + 4) - ttSHORT(info.data, info.hhea + 6);
            return (float)height / fheight;
        }

        public static void stbtt_GetGlyphHMetrics(ref stbtt_fontinfo info, int glyph_index,
                                                  ref int advanceWidth, ref int leftSideBearing)
        {
            ushort numOfLongHorMetrics = ttUSHORT(info.data, info.hhea + 34);
            if (glyph_index < numOfLongHorMetrics)
            {
                advanceWidth = ttSHORT(info.data, info.hmtx + 4 * glyph_index);
                leftSideBearing = ttSHORT(info.data, info.hmtx + 4 * glyph_index + 2);
            }
            else
            {
                advanceWidth = ttSHORT(info.data, info.hmtx + 4 * (numOfLongHorMetrics - 1));
                leftSideBearing = ttSHORT(info.data, info.hmtx + 4 * numOfLongHorMetrics + 2 * (glyph_index - numOfLongHorMetrics));
            }
        }

        public static int stbtt_FindGlyphIndex(ref stbtt_fontinfo info, int unicode_codepoint)
        {
            byte[] data = info.data;
            uint index_map = (uint)info.index_map;

            ushort format = ttUSHORT(data, (int)(index_map + 0));
            if (format == 0)
            { // apple byte encoding
                int bytes = ttUSHORT(data, (int)(index_map + 2));
                if (unicode_codepoint < bytes - 6)
                    return ttBYTE(data, (int)(index_map + 6 + unicode_codepoint));
                return 0;
            }
            else if (format == 6)
            {
                uint first = ttUSHORT(data, (int)(index_map + 6));
                uint count = ttUSHORT(data, (int)(index_map + 8));
                if ((uint)unicode_codepoint >= first && (uint)unicode_codepoint < first + count)
                    return ttUSHORT(data, (int)(index_map + 10 + (unicode_codepoint - first) * 2));
                return 0;
            }
            else if (format == 2)
            {
                {
                    //STBTT_assert(0); // @TODO: high-byte mapping for japanese/chinese/korean
                }
                return 0;
            }
            else if (format == 4)
            { // standard mapping for windows fonts: binary search collection of ranges
                ushort segcount = (ushort)(ttUSHORT(data, (int)(index_map + 6)) >> 1);
                ushort searchRange = (ushort)(ttUSHORT(data, (int)(index_map + 8)) >> 1);
                ushort entrySelector = ttUSHORT(data, (int)(index_map + 10));
                ushort rangeShift = (ushort)(ttUSHORT(data, (int)(index_map + 12)) >> 1);
                ushort item, offset, start, end;

                // do a binary search of the segments
                uint endCount = index_map + 14;
                uint search = endCount;

                if (unicode_codepoint > 0xffff)
                    return 0;

                // they lie from endCount .. endCount + segCount
                // but searchRange is the nearest power of two, so...
                if (unicode_codepoint >= (int)(ttUSHORT(data, (int)(search + rangeShift * 2))))
                    search += (uint)(rangeShift * 2);

                // now decrement to bias correctly to find smallest
                search -= 2;
                while (entrySelector != 0)
                {
                    //ushort start, end;
                    searchRange >>= 1;
                    start = ttUSHORT(data, (int)(search + 2 + segcount * 2 + 2));
                    end = ttUSHORT(data, (int)(search + 2));
                    start = ttUSHORT(data, (int)(search + searchRange * 2 + segcount * 2 + 2));
                    end = ttUSHORT(data, (int)(search + searchRange * 2));
                    if (unicode_codepoint > end)
                        search += (uint)(searchRange * 2);
                    --entrySelector;
                }
                search += 2;

                item = (ushort)((search - endCount) >> 1);

                //STBTT_assert(unicode_codepoint <= ttUSHORT(data + endCount + 2*item));
                start = ttUSHORT(data, (int)(index_map + 14 + segcount * 2 + 2 + 2 * item));
                end = ttUSHORT(data, (int)(index_map + 14 + 2 + 2 * item));
                if (unicode_codepoint < start)
                    return 0;

                offset = ttUSHORT(data, (int)(index_map + 14 + segcount * 6 + 2 + 2 * item));
                if (offset == 0)
                    return (ushort)(unicode_codepoint + ttSHORT(data, (int)(index_map + 14 + segcount * 4 + 2 + 2 * item)));

                return ttUSHORT(data, (int)(offset + (unicode_codepoint - start) * 2 + index_map + 14 + segcount * 6 + 2 + 2 * item));
            }
            else if (format == 12 || format == 13)
            {
                uint ngroups = ttULONG(data, (int)(index_map + 12));
                int low, high;
                low = 0;
                high = (int)ngroups;
                // Binary search the right group.
                while (low < high)
                {
                    int mid = low + ((high - low) >> 1); // rounds down, so low <= mid < high
                    uint start_char = ttULONG(data, (int)(index_map + 16 + mid * 12));
                    uint end_char = ttULONG(data, (int)(index_map + 16 + mid * 12 + 4));
                    if ((uint)unicode_codepoint < start_char)
                        high = mid;
                    else if ((uint)unicode_codepoint > end_char)
                        low = mid + 1;
                    else
                    {
                        uint start_glyph = ttULONG(data, (int)(index_map + 16 + mid * 12 + 8));
                        if (format == 12)
                            return (int)(start_glyph + unicode_codepoint - start_char);
                        else // format == 13
                            return (int)start_glyph;
                    }
                }
                return 0; // not found
            }
            // @TODO
            //STBTT_assert(0);
            return 0;
        }

        public static int stbtt_GetGlyphShape(ref stbtt_fontinfo info, int glyph_index,
                                              out stbtt_vertex[] pvertices)
        {
            int num_vertices = 0;
            short numberOfContours;
            byte[] endPtsOfContours;
            byte[] data = info.data;
            stbtt_vertex[] vertices = null;
            int g = stbtt__GetGlyfOffset(ref info, glyph_index);

            pvertices = null;

            if (g < 0)
                return 0;

            numberOfContours = ttSHORT(data, g);

            if (numberOfContours > 0)
            {
                byte flags = 0, flagcount;
                int ins, i, j = 0, m, n, next_move, was_off = 0, off, start_off = 0;
                int x, y, cx, cy, sx, sy, scx, scy;
                byte[] points;
                endPtsOfContours = data;
                int iData = g + 10;
                ins = ttUSHORT(data, g + 10 + numberOfContours * 2);
                points = data;
                int iPoints = g + 10 + numberOfContours * 2 + 2 + ins;

                n = 1 + ttUSHORT(endPtsOfContours, numberOfContours * 2 - 2 + iData);

                m = n + 2 * numberOfContours;
                // a loose bound on how many vertices we might need
                //vertices = (stbtt_vertex *) STBTT_malloc(m * sizeof(vertices[0]), info->userdata);
                vertices = new stbtt_vertex[m];

                if (vertices == null)
                    return 0;

                next_move = 0;
                flagcount = 0;

                // in first pass, we load uninterpreted data into the allocated array
                // above, shifted to the end of the array so we won't overwrite it when
                // we create our final data starting from the front

                off = m - n; // starting offset for uninterpreted data, regardless of how m ends up being calculated

                // first load flags

                for (i = 0; i < n; ++i)
                {
                    if (flagcount == 0)
                    {
                        flags = points[iPoints++];
                        if ((flags & 8) != 0)
                            flagcount = points[iPoints++];
                    }
                    else
                        --flagcount;
                    vertices[off + i].type = (STBTT_vmove)flags;
                }

                // now load x coordinates
                x = 0;
                for (i = 0; i < n; ++i)
                {
                    flags = (byte)vertices[off + i].type;
                    if ((flags & 2) != 0)
                    {
                        short dx = points[iPoints++];
                        x += (flags & 16) != 0 ? dx : -dx; // ???
                    }
                    else
                    {
                        if (!((flags & 16) != 0))
                        {
                            x = x + (short)((points[0 + iPoints] * 256) + points[1 + iPoints]);
                            iPoints += 2;
                        }
                    }
                    vertices[off + i].x = (short)x;
                }

                // now load y coordinates
                y = 0;
                for (i = 0; i < n; ++i)
                {
                    flags = (byte)vertices[off + i].type;
                    if ((flags & 4) != 0)
                    {
                        short dy = points[iPoints++];
                        y += (flags & 32) != 0 ? dy : -dy; // ???
                    }
                    else
                    {
                        if (!((flags & 32) != 0))
                        {
                            y = y + (short)((points[0 + iPoints] * 256) + points[1 + iPoints]);
                            iPoints += 2;
                        }
                    }
                    vertices[off + i].y = (short)y;
                }

                // now convert them to our format
                num_vertices = 0;
                sx = sy = cx = cy = scx = scy = 0;

                for (i = 0; i < n; ++i)
                {
                    flags = (byte)vertices[off + i].type;
                    x = (short)vertices[off + i].x;
                    y = (short)vertices[off + i].y;

                    if (next_move == i)
                    {
                        if (i != 0)
                            num_vertices = stbtt__close_shape(vertices, num_vertices, was_off, start_off, sx, sy, scx, scy, cx, cy);

                        // now start the new one               
                        start_off = 0 == (flags & 1) ? 1 : 0;
                        if (start_off != 0)
                        {
                            // if we start off with an off-curve point, then when we need to find a point on the curve
                            // where we can start, and we need to save some state for when we wraparound.
                            scx = x;
                            scy = y;
                            if (!(((int)vertices[off + i + 1].type & 1) != 0))
                            {
                                // next point is also a curve point, so interpolate an on-point curve
                                sx = (x + (int)vertices[off + i + 1].x) >> 1;
                                sy = (y + (int)vertices[off + i + 1].y) >> 1;
                            }
                            else
                            {
                                // otherwise just use the next point as our start point
                                sx = (int)vertices[off + i + 1].x;
                                sy = (int)vertices[off + i + 1].y;
                                ++i; // we're using point i+1 as the starting point, so skip it
                            }
                        }
                        else
                        {
                            sx = x;
                            sy = y;
                        }
                        stbtt_setvertex(ref vertices[num_vertices++], num_vertices, STBTT_vmove.STBTT_vmove, sx, sy, 0, 0);
                        was_off = 0;
                        next_move = 1 + ttUSHORT(endPtsOfContours, iData + j * 2);
                        ++j;
                    }
                    else
                    {
                        if (!((flags & 1) != 0))
                        { // if it's a curve
                            if (was_off != 0) // two off-curve control points in a row means interpolate an on-curve midpoint
                                stbtt_setvertex(ref vertices[num_vertices++], num_vertices, STBTT_vmove.STBTT_vcurve, (cx + x) >> 1, (cy + y) >> 1, cx, cy);
                            cx = x;
                            cy = y;
                            was_off = 1;
                        }
                        else
                        {
                            if (was_off != 0)
                                stbtt_setvertex(ref vertices[num_vertices++], num_vertices, STBTT_vmove.STBTT_vcurve, x, y, cx, cy);
                            else
                                stbtt_setvertex(ref vertices[num_vertices++], num_vertices, STBTT_vmove.STBTT_vline, x, y, 0, 0);
                            was_off = 0;
                        }
                    }

                }

                num_vertices = stbtt__close_shape(vertices, num_vertices, was_off, start_off, sx, sy, scx, scy, cx, cy);
            }
            else if (numberOfContours == -1)
            {
                // Compound shapes.
                int more = 1;
                byte[] comp = data;
                int iComp = g + 10;
                num_vertices = 0;
                vertices = null;
                while (more != 0)
                {
                    ushort flags, gidx;
                    int comp_num_verts = 0, i;
                    stbtt_vertex[] comp_verts = null, tmp = null;
                    float[] mtx = { 1, 0, 0, 1, 0, 0 };
                    float m, n;

                    flags = (ushort)ttSHORT(comp, iComp);
                    iComp += 2;
                    gidx = (ushort)ttSHORT(comp, iComp);
                    iComp += 2;

                    if ((flags & 2) != 0)
                    { // XY values
                        if ((flags & 1) != 0)
                        { // shorts
                            mtx[4] = ttSHORT(comp, iComp);
                            iComp += 2;
                            mtx[5] = ttSHORT(comp, iComp);
                            iComp += 2;
                        }
                        else
                        {
                            mtx[4] = ttCHAR(comp, iComp);
                            iComp += 1;
                            mtx[5] = ttCHAR(comp, iComp);
                            iComp += 1;
                        }
                    }
                    else
                    {
                        // @TODO handle matching point
                        //STBTT_assert(0);
                    }
                    if ((flags & (1 << 3)) != 0)
                    { // WE_HAVE_A_SCALE
                        mtx[0] = mtx[3] = ttSHORT(comp, iComp) / 16384.0f;
                        iComp += 2;
                        mtx[1] = mtx[2] = 0;
                    }
                    else if ((flags & (1 << 6)) != 0)
                    { // WE_HAVE_AN_X_AND_YSCALE
                        mtx[0] = ttSHORT(comp, iComp) / 16384.0f;
                        iComp += 2;
                        mtx[1] = mtx[2] = 0;
                        mtx[3] = ttSHORT(comp, iComp) / 16384.0f;
                        iComp += 2;
                    }
                    else if ((flags & (1 << 7)) != 0)
                    { // WE_HAVE_A_TWO_BY_TWO
                        mtx[0] = ttSHORT(comp, iComp) / 16384.0f;
                        iComp += 2;
                        mtx[1] = ttSHORT(comp, iComp) / 16384.0f;
                        iComp += 2;
                        mtx[2] = ttSHORT(comp, iComp) / 16384.0f;
                        iComp += 2;
                        mtx[3] = ttSHORT(comp, iComp) / 16384.0f;
                        iComp += 2;
                    }

                    // Find transformation scales.
                    m = (float)System.Math.Sqrt(mtx[0] * mtx[0] + mtx[1] * mtx[1]);
                    n = (float)System.Math.Sqrt(mtx[2] * mtx[2] + mtx[3] * mtx[3]);

                    // Get indexed glyph.
                    comp_num_verts = stbtt_GetGlyphShape(ref info, gidx, out comp_verts);
                    if (comp_num_verts > 0)
                    {
                        // Transform vertices.
                        for (i = 0; i < comp_num_verts; ++i)
                        {
                            stbtt_vertex v = comp_verts[i];
                            short x, y; // stbtt_vertex_type = short;
                            x = v.x;
                            y = v.y;
                            v.x = (short)(m * (mtx[0] * x + mtx[2] * y + mtx[4]));
                            v.y = (short)(n * (mtx[1] * x + mtx[3] * y + mtx[5]));
                            x = v.cx;
                            y = v.cy;
                            v.cx = (short)(m * (mtx[0] * x + mtx[2] * y + mtx[4]));
                            v.cy = (short)(n * (mtx[1] * x + mtx[3] * y + mtx[5]));
                        }

                        // Append vertices.
                        //tmp = (stbtt_vertex*)STBTT_malloc((num_vertices+comp_num_verts)*sizeof(stbtt_vertex), info->userdata);
                        tmp = new stbtt_vertex[num_vertices + comp_num_verts];
                        if (tmp == null)
                        {
                            //if (vertices) STBTT_free(vertices, info->userdata);
                            //if (comp_verts) STBTT_free(comp_verts, info->userdata);
                            return 0;
                        }
                        if (num_vertices > 0)
                            //memcpy(tmp, vertices, num_vertices*sizeof(stbtt_vertex));
                            Array.Copy(vertices, tmp, num_vertices);
                        //memcpy(tmp+num_vertices, comp_verts, comp_num_verts*sizeof(stbtt_vertex));
                        Array.Copy(comp_verts, 0, tmp, num_vertices, comp_num_verts);
                        //if (vertices) 
                        //	STBTT_free(vertices, info->userdata);
                        vertices = tmp;
                        //STBTT_free(comp_verts, info->userdata);
                        num_vertices += comp_num_verts;
                    }
                    // More components ?
                    more = flags & (1 << 5);

                }
            }
            else if (numberOfContours < 0)
            {
                // @TODO other compound variations?
                //STBTT_assert(0);
            }
            else
            {
                // numberOfCounters == 0, do nothing
            }

            pvertices = vertices;

            return num_vertices;
        }

        static void stbtt_setvertex(ref stbtt_vertex v, int numVert, STBTT_vmove type, int x, int y, int cx, int cy)
        {
            if (numVert == 15)
            {
                numVert = 15;
            }

            v.type = type;
            v.x = (short)x;
            v.y = (short)y;
            v.cx = (short)cx;
            v.cy = (short)cy;
        }

        static int stbtt__close_shape(stbtt_vertex[] vertices, int num_vertices, int was_off, int start_off,
                                      int sx, int sy, int scx, int scy, int cx, int cy)
        {
            if (start_off != 0)
            {
                if (was_off != 0)
                    stbtt_setvertex(ref vertices[num_vertices++], num_vertices,
                        STBTT_vmove.STBTT_vcurve, (cx + scx) >> 1, (cy + scy) >> 1, cx, cy);
                stbtt_setvertex(ref vertices[num_vertices++], num_vertices,
                    STBTT_vmove.STBTT_vcurve, sx, sy, scx, scy);
            }
            else
            {
                if (was_off != 0)
                    stbtt_setvertex(ref vertices[num_vertices++], num_vertices,
                        STBTT_vmove.STBTT_vcurve, sx, sy, cx, cy);
                else
                    stbtt_setvertex(ref vertices[num_vertices++], num_vertices,
                        STBTT_vmove.STBTT_vline, sx, sy, 0, 0);
            }
            return num_vertices;
        }

        public static void stbtt_MakeGlyphBitmap(ref stbtt_fontinfo info, byte[] output, int iOutput, int out_w, int out_h, int out_stride, float scale_x, float scale_y, int glyph)
        {
            stbtt_MakeGlyphBitmapSubpixel(ref info, output, iOutput, out_w, out_h, out_stride, scale_x, scale_y, 0.0f, 0.0f, glyph);
        }

        public static void stbtt_MakeGlyphBitmapSubpixel(ref stbtt_fontinfo info, byte[] output, int iOutput, int out_w, int out_h, int out_stride, float scale_x, float scale_y, float shift_x, float shift_y, int glyph)
        {
            int ix0 = 0, iy0 = 0, v1 = 0, v2 = 0;
            stbtt_vertex[] vertices = null;
            int num_verts = stbtt_GetGlyphShape(ref info, glyph, out vertices);
            stbtt__bitmap gbm;

            stbtt_GetGlyphBitmapBoxSubpixel(ref info, glyph, scale_x, scale_y, shift_x, shift_y, ref ix0, ref iy0, ref v1, ref v2);
            gbm.pixels = output;
            gbm.iPixels = iOutput;
            gbm.w = out_w;
            gbm.h = out_h;
            gbm.stride = out_stride;

            if (gbm.w != 0 && gbm.h != 0)
                stbtt_Rasterize(ref gbm, 0.35f, vertices, num_verts, scale_x, scale_y, shift_x, shift_y, ix0, iy0, 1, info.userdata);

            //STBTT_free(vertices, info.userdata);
        }

        static void stbtt_Rasterize(ref stbtt__bitmap result, float flatness_in_pixels,
                                    stbtt_vertex[] vertices, int num_verts, float scale_x, float scale_y, float shift_x, float shift_y, int x_off, int y_off, int invert, object userdata)
        {
            float scale = scale_x > scale_y ? scale_y : scale_x;
            int winding_count = 0;
            int[] winding_lengths = null;
            stbtt__point[] windings = stbtt_FlattenCurves(vertices, num_verts, flatness_in_pixels / scale, out winding_lengths, ref winding_count, userdata);
            if (windings != null)
            {
                stbtt__rasterize(ref result, windings, winding_lengths, winding_count, scale_x, scale_y, shift_x, shift_y, x_off, y_off, invert, userdata);
                //STBTT_free(winding_lengths, userdata);
                //STBTT_free(windings, userdata);
            }
        }

        static void stbtt__rasterize(ref stbtt__bitmap result, stbtt__point[] pts, int[] wcount, int windings,
                                     float scale_x, float scale_y, float shift_x, float shift_y, int off_x, int off_y,
                                     int invert, object userdata)
        {
            float y_scale_inv = invert != 0 ? -scale_y : scale_y;
            stbtt__edge[] e;
            int n, i, j, k, m;
            int vsubsample = result.h < 8 ? 15 : 5;
            // vsubsample should divide 255 evenly; otherwise we won't reach full opacity

            // now we have to blow out the windings into explicit edge lists
            n = 0;
            for (i = 0; i < windings; ++i)
                n += wcount[i];

            //e = (stbtt__edge) STBTT_malloc(sizeof(*e) * (n+1), userdata); // add an extra one as a sentinel
            e = new stbtt__edge[n + 1];
            for (int cont = 0; cont < n + 1; cont++)
                e[cont] = new stbtt__edge();

            if (e == null)
                return;
            n = 0;

            m = 0;
            for (i = 0; i < windings; ++i)
            {
                stbtt__point[] p = pts;
                int pi = m;
                m += wcount[i];
                j = wcount[i] - 1;
                for (k = 0; k < wcount[i]; j = k++)
                {
                    int a = k, b = j;
                    // skip the edge if horizontal
                    if (p[j + pi].y == p[k + pi].y)
                        continue;
                    // add edge from j to k to the list
                    e[n].invert = 0;
                    if (invert != 0 ? p[j + pi].y > p[k + pi].y : p[j + pi].y < p[k + pi].y)
                    {
                        e[n].invert = 1;
                        a = j;
                        b = k;
                    }
                    e[n].x0 = p[a + pi].x * scale_x + shift_x;
                    e[n].y0 = p[a + pi].y * y_scale_inv * vsubsample + shift_y;
                    e[n].x1 = p[b + pi].x * scale_x + shift_x;
                    e[n].y1 = p[b + pi].y * y_scale_inv * vsubsample + shift_y;

                    ++n;
                }
            }

            // now sort the edges by their highest point (should snap to integer, and then by x)
            //STBTT_sort(e, n, sizeof(e[0]), stbtt__edge_compare);
            // Sort the array
            //Quicksort(e, 0, n);
            Array.Sort(e, 0, n);

            // now, traverse the scanlines and find the intersections on each scanline, use xor winding rule
            stbtt__rasterize_sorted_edges(ref result, e, n, vsubsample, off_x, off_y, userdata);

            //STBTT_free(e, userdata);
        }

        static void stbtt__rasterize_sorted_edges(ref stbtt__bitmap result, stbtt__edge[] e,
                                                  int n, int vsubsample, int off_x, int off_y, object userdata)
        {
            stbtt__active_edge activeIsNext = new stbtt__active_edge();
            ;
            int y, j = 0, eIndex = 0;
            int max_weight = (255 / vsubsample);  // weight per vertical scanline
            int s; // vertical subsample index
            byte[] scanline_data = new byte[512], scanline;

            if (result.w > 512)
            {
                scanline = (byte[])userdata;
                Array.Resize(ref scanline, result.w);
            }
            else
                scanline = scanline_data;

            y = off_y * vsubsample;
            e[n].y0 = (off_y + result.h) * (float)vsubsample + 1;

            while (j < result.h)
            {
                STBTT_memset(scanline, 0, result.w);
                for (s = 0; s < vsubsample; ++s)
                {
                    // find center of pixel for this scanline
                    float scan_y = y + 0.5f;
                    stbtt__active_edge stepIsNext = activeIsNext;

                    // update all active edges;
                    // remove all active edges that terminate before the center of this scanline
                    while (stepIsNext.next != null)
                    {
                        stbtt__active_edge z = stepIsNext.next;
                        if (z.ey <= scan_y)
                        {
                            stepIsNext.next = z.next; // delete from list
                                                      //STBTT_assert(z->valid);
                            z.valid = 0;
                            //STBTT_free(z, userdata);
                        }
                        else
                        {
                            z.x += z.dx; // advance to position for current scanline
                            stepIsNext = stepIsNext.next; // advance through list
                        }
                    }

                    // resort the list if needed
                    for (;;)
                    {
                        bool changed = false;
                        stepIsNext = activeIsNext;
                        while (stepIsNext.next != null && stepIsNext.next.next != null)
                        {
                            if (stepIsNext.next.x > stepIsNext.next.next.x)
                            {
                                stbtt__active_edge t = stepIsNext.next;
                                stbtt__active_edge q = t.next;

                                t.next = q.next;
                                q.next = t;
                                stepIsNext.next = q;
                                changed = true;
                            }
                            stepIsNext = stepIsNext.next;
                        }
                        if (!changed)
                            break;
                    }

                    // insert all edges that start before the center of this scanline -- omit ones that also end on this scanline
                    while (e[eIndex].y0 <= scan_y)
                    {
                        if (e[eIndex].y1 > scan_y)
                        {
                            stbtt__active_edge z = new_active(e[eIndex], off_x, scan_y, userdata);
                            // find insertion point
                            if (activeIsNext.next == null)
                                activeIsNext.next = z;
                            else if (z.x < activeIsNext.next.x)
                            {
                                // insert at front
                                z.next = activeIsNext.next;
                                activeIsNext.next = z;
                            }
                            else
                            {
                                // find thing to insert AFTER
                                stbtt__active_edge p = activeIsNext.next;
                                while (p.next != null && p.next.x < z.x)
                                    p = p.next;
                                // at this point, p->next->x is NOT < z->x
                                z.next = p.next;
                                p.next = z;
                            }
                        }
                        ++eIndex;
                    }

                    // now process all active edges in XOR fashion
                    if (activeIsNext.next != null)
                        stbtt__fill_active_edges(scanline, result.w, activeIsNext.next, max_weight);

                    ++y;
                }
                //STBTT_memcpy(result.pixels + j * result.stride, scanline, result.w);
                for (int cont = 0; cont < result.w; cont++)
                    result.pixels[result.iPixels + cont + (j * result.stride)] = scanline[cont];
                ++j;
            }

            /*while (active != null)
			{
				//stbtt__active_edge z = active;
				active = active.next;
				//STBTT_free(z, userdata);
			}*/

            //if (scanline != scanline_data)
            //	STBTT_free(scanline, userdata);
        }

        // note: this routine clips fills that extend off the edges... ideally this
        // wouldn't happen, but it could happen if the truetype glyph bounding boxes
        // are wrong, or if the user supplies a too-small bitmap
        static void stbtt__fill_active_edges(byte[] scanline, int len, stbtt__active_edge e, int max_weight)
        {
            byte ab = 0;
            // non-zero winding fill
            int x0 = 0, w = 0;

            while (e != null)
            {
                if (w == 0)
                {
                    // if we're currently at zero, we need to record the edge start point
                    x0 = e.x;
                    w += e.valid;
                }
                else
                {
                    int x1 = e.x;
                    w += e.valid;
                    // if we went to zero, we need to draw
                    if (w == 0)
                    {
                        int i = x0 >> FIXSHIFT;
                        int j = x1 >> FIXSHIFT;

                        if (i < len && j >= 0)
                        {
                            if (i == j)
                            {
                                // x0,x1 are the same pixel, so compute combined coverage
                                ab = (byte)(scanline[i] + (byte)((x1 - x0) * max_weight >> FIXSHIFT));
                                scanline[i] = ab;
                            }
                            else
                            {
                                if (i >= 0) // add antialiasing for x0
                                {
                                    ab = (byte)(scanline[i] + (byte)(((FIX - (x0 & FIXMASK)) * max_weight) >> FIXSHIFT));
                                    scanline[i] = ab;
                                }
                                else
                                    i = -1; // clip

                                if (j < len) // add antialiasing for x1
                                {
                                    ab = (byte)(scanline[j] + (byte)(((x1 & FIXMASK) * max_weight) >> FIXSHIFT));
                                    scanline[j] = ab;
                                }
                                else
                                    j = len; // clip

                                for (++i; i < j; ++i) // fill pixels between x0 and x1
                                {
                                    ab = (byte)(scanline[i] + (byte)max_weight);
                                    scanline[i] = ab;
                                }
                            }
                        }
                    }
                }

                e = e.next;
            }
        }

        static stbtt__active_edge new_active(stbtt__edge e, int off_x, float start_point, object userdata)
        {
            //stbtt__active_edge z = (stbtt__active_edge *) STBTT_malloc(sizeof(*z), userdata); // @TODO: make a pool of these!!!
            stbtt__active_edge z = new stbtt__active_edge();
            float dxdy = (e.x1 - e.x0) / (e.y1 - e.y0);
            //STBTT_assert(e->y0 <= start_point);
            if (z == null)
                return z;
            // round dx down to avoid going too far
            if (dxdy < 0)
                z.dx = -STBTT_ifloor(FIX * -dxdy);
            else
                z.dx = STBTT_ifloor(FIX * dxdy);
            z.x = STBTT_ifloor(FIX * (e.x0 + dxdy * (start_point - e.y0)));
            z.x -= off_x * FIX;
            z.ey = e.y1;
            z.next = null;
            z.valid = e.invert != 0 ? 1 : -1;
            return z;
        }

        public static void Quicksort(IComparable[] elements, int left, int right)
        {
            int i = left, j = right;
            IComparable pivot = elements[(left + right) / 2];

            while (i <= j)
            {
                while (elements[i].CompareTo(pivot) < 0)
                {
                    i++;
                }

                while (elements[j].CompareTo(pivot) > 0)
                {
                    j--;
                }

                if (i <= j)
                {
                    // Swap
                    IComparable tmp = elements[i];
                    elements[i] = elements[j];
                    elements[j] = tmp;

                    i++;
                    j--;
                }
            }

            // Recursive calls
            if (left < j)
            {
                Quicksort(elements, left, j);
            }

            if (i < right)
            {
                Quicksort(elements, i, right);
            }
        }

        // returns number of contours
        static stbtt__point[] stbtt_FlattenCurves(stbtt_vertex[] vertices, int num_verts, float objspace_flatness,
                                                  out int[] contour_lengths, ref int num_contours, object userdata)
        {
            contour_lengths = null;
            stbtt__point[] points = null;
            int num_points = 0;

            float objspace_flatness_squared = objspace_flatness * objspace_flatness;
            int i, n = 0, start = 0, pass;

            // count how many "moves" there are to get the contour count
            for (i = 0; i < num_verts; ++i)
                if (vertices[i].type == STBTT_vmove.STBTT_vmove)
                    ++n;

            num_contours = n;
            if (n == 0)
                return null;

            //contour_lengths = (int *) STBTT_malloc(sizeof(**contour_lengths) * n, userdata);
            contour_lengths = new int[n];

            if (contour_lengths == null)
            {
                num_contours = 0;
                return null;
            }

            // make two passes through the points so we don't need to realloc
            for (pass = 0; pass < 2; ++pass)
            {
                float x = 0, y = 0;
                if (pass == 1)
                {
                    //points = (stbtt__point *) STBTT_malloc(num_points * sizeof(points[0]), userdata);
                    if (num_points == 0)
                    {
                        contour_lengths = null;
                        num_contours = 0;

                        return null;
                    }
                    points = new stbtt__point[num_points];
                }
                num_points = 0;
                n = -1;
                for (i = 0; i < num_verts; ++i)
                {
                    switch (vertices[i].type)
                    {
                        case STBTT_vmove.STBTT_vmove:
                            // start the next contour
                            if (n >= 0)
                                contour_lengths[n] = num_points - start;
                            ++n;
                            start = num_points;

                            x = vertices[i].x;
                            y = vertices[i].y;
                            stbtt__add_point(points, num_points++, x, y);
                            break;
                        case STBTT_vmove.STBTT_vline:
                            x = vertices[i].x;
                            y = vertices[i].y;
                            stbtt__add_point(points, num_points++, x, y);
                            break;
                        case STBTT_vmove.STBTT_vcurve:
                            stbtt__tesselate_curve(points, ref num_points, x, y,
                                vertices[i].cx, vertices[i].cy,
                                vertices[i].x, vertices[i].y,
                                objspace_flatness_squared, 0);
                            x = vertices[i].x;
                            y = vertices[i].y;
                            break;
                    }
                }
                contour_lengths[n] = num_points - start;
            }

            return points;
        }

        // tesselate until threshhold p is happy... @TODO warped to compensate for non-linear stretching
        static int stbtt__tesselate_curve(stbtt__point[] points, ref int num_points, float x0, float y0, float x1, float y1, float x2, float y2, float objspace_flatness_squared, int n)
        {
            // midpoint
            float mx = (x0 + 2 * x1 + x2) / 4;
            float my = (y0 + 2 * y1 + y2) / 4;
            // versus directly drawn line
            float dx = (x0 + x2) / 2 - mx;
            float dy = (y0 + y2) / 2 - my;
            if (n > 16) // 65536 segments on one curve better be enough!
                return 1;
            if (dx * dx + dy * dy > objspace_flatness_squared)
            { // half-pixel error allowed... need to be smaller if AA
                stbtt__tesselate_curve(points, ref num_points, x0, y0, (x0 + x1) / 2.0f, (y0 + y1) / 2.0f, mx, my, objspace_flatness_squared, n + 1);
                stbtt__tesselate_curve(points, ref num_points, mx, my, (x1 + x2) / 2.0f, (y1 + y2) / 2.0f, x2, y2, objspace_flatness_squared, n + 1);
            }
            else
            {
                stbtt__add_point(points, num_points, x2, y2);
                num_points = num_points + 1;
            }
            return 1;
        }

        static void stbtt__add_point(stbtt__point[] points, int n, float x, float y)
        {
            if (points == null)
                return; // during first pass, it's unallocated
            points[n].x = x;
            points[n].y = y;
        }

        //////////////////////////////////////////////////////////////////////////////
        //
        // antialiasing software rasterizer
        //

        static void STBTT_memset(byte[] data, int value, int firstCount)
        {
            for (int cont = 0; cont < firstCount; cont++)
                data[cont] = (byte)value;
        }

        static int STBTT_ifloor(float x)
        {
            return (int)System.Math.Floor(x);
        }

        static int STBTT_iceil(float x)
        {
            return (int)System.Math.Ceiling(x);
        }

        static void stbtt_GetGlyphBitmapBoxSubpixel(ref stbtt_fontinfo font, int glyph, float scale_x, float scale_y, float shift_x, float shift_y,
                                                    ref int ix0, ref int iy0, ref int ix1, ref int iy1)
        {
            int x0 = 0, y0 = 0, x1 = 0, y1 = 0;
            if (!(stbtt_GetGlyphBox(ref font, glyph, ref x0, ref y0, ref x1, ref y1) != 0))
                x0 = y0 = x1 = y1 = 0; // e.g. space character
                                       // now move to integral bboxes (treating pixels as little squares, what pixels get touched)?
            ix0 = STBTT_ifloor(x0 * scale_x + shift_x);
            iy0 = -STBTT_iceil(y1 * scale_y + shift_y);
            ix1 = STBTT_iceil(x1 * scale_x + shift_x);
            iy1 = -STBTT_ifloor(y0 * scale_y + shift_y);
        }

        static int stbtt_GetGlyphBox(ref stbtt_fontinfo info, int glyph_index,
                                     ref int x0, ref int y0, ref int x1, ref int y1)
        {
            int g = stbtt__GetGlyfOffset(ref info, glyph_index);
            if (g < 0)
                return 0;

            x0 = ttSHORT(info.data, g + 2);
            y0 = ttSHORT(info.data, g + 4);
            x1 = ttSHORT(info.data, g + 6);
            y1 = ttSHORT(info.data, g + 8);
            return 1;
        }

        static int stbtt__GetGlyfOffset(ref stbtt_fontinfo info, int glyph_index)
        {
            int g1, g2;

            if (glyph_index >= info.numGlyphs)
                return -1; // glyph index out of range
            if (info.indexToLocFormat >= 2)
                return -1; // unknown index->glyph map format

            if (info.indexToLocFormat == 0)
            {
                g1 = info.glyf + ttUSHORT(info.data, info.loca + glyph_index * 2) * 2;
                g2 = info.glyf + ttUSHORT(info.data, info.loca + glyph_index * 2 + 2) * 2;
            }
            else
            {
                g1 = (int)(info.glyf + ttULONG(info.data, info.loca + glyph_index * 4));
                g2 = (int)(info.glyf + ttULONG(info.data, info.loca + glyph_index * 4 + 4));
            }

            return g1 == g2 ? -1 : g1; // if length is 0, return -1
        }

        public static void stbtt_GetGlyphBitmapBox(ref stbtt_fontinfo font, int glyph, float scale_x, float scale_y,
                                                   ref int ix0, ref int iy0, ref int ix1, ref int iy1)
        {
            stbtt_GetGlyphBitmapBoxSubpixel(ref font, glyph, scale_x, scale_y, 0.0f, 0.0f, ref ix0, ref iy0, ref ix1, ref iy1);
        }

        public static int stbtt_GetGlyphKernAdvance(ref stbtt_fontinfo info, int glyph1, int glyph2)
        {
            byte[] data = info.data;
            int index = info.kern;
            int needle, straw;
            int l, r, m;

            // we only look at the first table. it must be 'horizontal' and format 0.
            if (!(info.kern != 0))
                return 0;
            if (ttUSHORT(data, 2 + index) < 1) // number of tables, need at least 1
                return 0;
            if (ttUSHORT(data, 8 + index) != 1) // horizontal flag must be set in format
                return 0;

            l = 0;
            r = ttUSHORT(data, 10 + index) - 1;
            needle = glyph1 << 16 | glyph2;
            while (l <= r)
            {
                m = (l + r) >> 1;
                straw = (int)ttULONG(data, 18 + (m * 6) + index); // note: unaligned read
                if (needle < straw)
                    r = m - 1;
                else if (needle > straw)
                    l = m + 1;
                else
                    return ttSHORT(data, 22 + (m * 6) + index);
            }
            return 0;
        }
    }

    // The following structure is defined publically so you can declare one on
    // the stack or as a global or etc, but you should treat it as opaque.
    public struct stbtt_fontinfo
    {
        public object userdata;
        // pointer to .ttf file
        public byte[] data;
        // offset of start of font
        public int fontstart;

        // number of glyphs, needed for range checking
        public int numGlyphs;

        // table locations as offset from start of .ttf
        public int loca, head, glyf, hhea, hmtx, kern;
        // a cmap mapping for our chosen character encoding
        public int index_map;
        // format needed to map from glyph index to glyph
        public int indexToLocFormat;
    }

    public struct stbtt_vertex
    {
        public short x, y, cx, cy;
        public STBTT_vmove type;
        public byte padding;
    }

    public struct stbtt__bitmap
    {
        public int w, h, stride;
        public byte[] pixels;
        public int iPixels;
    }

    public struct stbtt__point
    {
        public float x, y;
    }

    public class stbtt__active_edge
    {
        public int x, dx;
        public float ey;
        public stbtt__active_edge next;
        public int valid;
    }

    public class stbtt__edge : IComparable
    {
        public float x0, y0, x1, y1;
        public int invert;

        public int CompareTo(object obj)
        {
            //stbtt__edge *a = (stbtt__edge *) p;
            stbtt__edge b = (stbtt__edge)obj;

            if (this.y0 < b.y0)
                return -1;
            if (this.y0 > b.y0)
                return 1;
            return 0;
        }
    }
}
