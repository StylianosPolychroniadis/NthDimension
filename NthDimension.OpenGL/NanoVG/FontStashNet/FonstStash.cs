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
using System.Text;

namespace NthDimension.Rasterizer.NanoVG.FontStashNet
{
    public delegate void HandleErrorHandler(object uptr, FONSerrorCode error, int val);

    public delegate int RenderCreateHandler(object uptr, int width, int height);
    public delegate int RenderResizeHandler(object uptr, int width, int height);
    public delegate void RenderUpdateHandler(object uptr, ref int[] rect, byte[] data);
    public delegate void RenderDrawHandler(object uptr, float[] verts, float[] tcoords,
                                           uint[] colors, int nverts);
    public delegate void RenderDeleteHandler(object uptr);

    public static class FontStash
    {
        public const uint FONS_SCRATCH_BUF_SIZE = 16000;
        public const uint FONS_HASH_LUT_SIZE = 256;
        public const uint FONS_INIT_FONTS = 4;
        public const uint FONS_INIT_GLYPHS = 256;
        public const uint FONS_INIT_ATLAS_NODES = 256;
        public const uint FONS_VERTEX_COUNT = 1024;
        public const uint FONS_MAX_STATES = 20;
        public const int FONS_INVALID = -1;
        public const int FONS_MAX_FALLBACKS = 20;

        const byte FONS_UTF8_ACCEPT = 0;
        const byte FONS_UTF8_REJECT = 12;

        const byte APREC = 16;
        const byte ZPREC = 7;

        static readonly byte[] utf8d = {
			// The first part of the table maps bytes to character classes that
			// to reduce the size of the transition table and create bitmasks.
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,  0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,  0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,  0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,  0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,  9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
            7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,  7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
            8, 8, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,  2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            10, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 11, 6, 6, 6, 5, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,

			// The second part is a transition table that maps a combination
			// of a state of the automaton and a character class to a state.
			0, 12, 24, 36, 60, 96, 84, 12, 12, 12, 48, 72, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12,
            12, 0, 12, 12, 12, 12, 12, 0, 12, 0, 12, 12, 12, 24, 12, 12, 12, 12, 12, 24, 12, 24, 12, 12,
            12, 12, 12, 12, 12, 12, 12, 24, 12, 12, 12, 12, 12, 24, 12, 12, 12, 12, 12, 12, 12, 24, 12, 12,
            12, 12, 12, 12, 12, 12, 12, 36, 12, 36, 12, 12, 12, 36, 12, 12, 12, 12, 12, 36, 12, 36, 12, 12,
            12, 36, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12,
        };

        #region Public-methods

        public static void fonsDrawDebug(FONScontext stash, float x, float y)
        {
            int i;
            int w = stash.@params.width;
            int h = stash.@params.height;
            float u = w == 0 ? 0 : (1.0f / w);
            float v = h == 0 ? 0 : (1.0f / h);

            if (stash.nverts + 6 + 6 > FONS_VERTEX_COUNT)
                fons__flush(stash);

            // Draw background
            fons__vertex(stash, x + 0, y + 0, u, v, 0x0fffffff);
            fons__vertex(stash, x + w, y + h, u, v, 0x0fffffff);
            fons__vertex(stash, x + w, y + 0, u, v, 0x0fffffff);

            fons__vertex(stash, x + 0, y + 0, u, v, 0x0fffffff);
            fons__vertex(stash, x + 0, y + h, u, v, 0x0fffffff);
            fons__vertex(stash, x + w, y + h, u, v, 0x0fffffff);

            // Draw texture
            fons__vertex(stash, x + 0, y + 0, 0, 0, 0xffffffff);
            fons__vertex(stash, x + w, y + h, 1, 1, 0xffffffff);
            fons__vertex(stash, x + w, y + 0, 1, 0, 0xffffffff);

            fons__vertex(stash, x + 0, y + 0, 0, 0, 0xffffffff);
            fons__vertex(stash, x + 0, y + h, 0, 1, 0xffffffff);
            fons__vertex(stash, x + w, y + h, 1, 1, 0xffffffff);

            // Drawbug draw atlas
            for (i = 0; i < stash.atlas.nnodes; i++)
            {
                FONSatlasNode n = stash.atlas.nodes[i];

                if (stash.nverts + 6 > FONS_VERTEX_COUNT)
                    fons__flush(stash);

                fons__vertex(stash, x + n.x + 0, y + n.y + 0, u, v, 0xc00000ff);
                fons__vertex(stash, x + n.x + n.width, y + n.y + 1, u, v, 0xc00000ff);
                fons__vertex(stash, x + n.x + n.width, y + n.y + 0, u, v, 0xc00000ff);

                fons__vertex(stash, x + n.x + 0, y + n.y + 0, u, v, 0xc00000ff);
                fons__vertex(stash, x + n.x + 0, y + n.y + 1, u, v, 0xc00000ff);
                fons__vertex(stash, x + n.x + n.width, y + n.y + 1, u, v, 0xc00000ff);
            }

            fons__flush(stash);
        }

        public static int fonsAddFallbackFont(FONScontext stash, int base_, int fallback)
        {
            FONSfont baseFont = stash.fonts[base_];
            if (baseFont.nfallbacks < FONS_MAX_FALLBACKS)
            {
                baseFont.fallbacks[baseFont.nfallbacks++] = fallback;
                return 1;
            }
            return 0;
        }

        public static int fonsAddFont(FONScontext stash, string name, string path)
        {
            if (!File.Exists(path))
                return FONS_INVALID;
            byte[] data = File.ReadAllBytes(path);
            uint dataSize = (uint)data.Length;

            return fonsAddFontMem(stash, name, data, dataSize, 1);
        }

        public static FONScontext fonsCreateInternal(ref FONSparams fparams)
        {
            FONScontext stash = new FONScontext();

            stash.@params = fparams;

            // Allocate scratch buffer.
            stash.scratch = new byte[FONS_SCRATCH_BUF_SIZE];

            // Initialize implementation library
            if (!fons__tt_init(stash))
                throw new Exception("Error initialize implementation library");
            if (stash.@params.renderCreate != null)
            {
                if (stash.@params.renderCreate(stash.@params.userPtr,
                        stash.@params.width,
                        stash.@params.height) == 0)
                    throw new Exception("Error init renderCreate");
            }

            stash.atlas = fons__allocAtlas(stash.@params.width, stash.@params.height,
                FONS_INIT_ATLAS_NODES);

            // Allocate space for fonts.
            stash.fonts = new FONSfont[FONS_INIT_FONTS];
            for (int cont = 0; cont < FONS_INIT_FONTS; cont++)
                stash.fonts[cont] = new FONSfont();

            stash.cfonts = FONS_INIT_FONTS;
            stash.nfonts = 0;

            // Create texture for the cache.
            stash.itw = 1.0f / stash.@params.width;
            stash.ith = 1.0f / stash.@params.height;
            stash.texData = new byte[stash.@params.width * stash.@params.height];

            stash.dirtyRect[0] = stash.@params.width;
            stash.dirtyRect[1] = stash.@params.height;
            stash.dirtyRect[2] = 0;
            stash.dirtyRect[3] = 0;

            // Add white rect at 0,0 for debug drawing.
            fons__addWhiteRect(ref stash, 2, 2);

            fonsPushState(ref stash);
            fonsClearState(ref stash);

            return stash;
        }

        public static void fonsDeleteInternal(FONScontext stash)
        {
            int i;
            if (stash == null)
                return;

            if (stash.@params.renderDelete != null)
                stash.@params.renderDelete(stash.@params.userPtr);

            for (i = 0; i < stash.nfonts; ++i)
                fons__freeFont(stash.fonts[i]);

            //if (stash.atlas != null) 
            fons__deleteAtlas(stash.atlas);
            //if (stash.fonts) 
            //	free(stash->fonts);
            //if (stash.texData) 
            //	free(stash->texData);
            //if (stash.scratch) 
            //	free(stash->scratch);
            //free(stash);
            stash = null;
        }

        public static void fonsClearState(ref FONScontext stash)
        {
            FONSstate state;
            fons__getState(stash, out state);
            state.size = 12.0f;
            state.color = 0xffffffff;
            state.font = 0;
            state.blur = 0;
            state.spacing = 0;
            state.align = FONSalign.FONS_ALIGN_LEFT | FONSalign.FONS_ALIGN_BASELINE;
        }

        public static float fonsDrawText(ref FONScontext stash,
                                         float x, float y,
                                         byte[] str)
        {
            FONSstate state;
            fons__getState(stash, out state);
            uint codepoint = 0;
            //uint utf8state = 0;
            FONSglyph glyph = null;
            FONSquad q = new FONSquad();
            int prevGlyphIndex = -1;
            short isize = (short)(state.size * 10.0f);
            short iblur = (short)state.blur;
            float scale;
            FONSfont font;
            float width;

            if (stash == null)
                return x;
            if (state.font < 0 || state.font >= stash.nfonts)
                return x;
            font = stash.fonts[state.font];
            if (font.data == null)
                return x;

            scale = fons__tt_getPixelHeightScale(ref font.font, (float)isize / 10.0f);

            // Align horizontally
            if ((state.align & FONSalign.FONS_ALIGN_LEFT) != 0)
            {
                // empty
            }
            else if ((state.align & FONSalign.FONS_ALIGN_RIGHT) != 0)
            {
                width = fonsTextBounds(ref stash, x, y, str, null);
                x -= width;
            }
            else if ((state.align & FONSalign.FONS_ALIGN_CENTER) != 0)
            {
                width = fonsTextBounds(ref stash, x, y, str, null);
                x -= width * 0.5f;
            }
            // Align vertically.
            y += fons__getVertAlign(stash, font, state.align, isize);

            foreach (char c in str)
            {
                /*byte[] bytes = Encoding.UTF8.GetBytes(c.ToString());

				if (fons__decutf8(ref utf8state, ref codepoint, bytes[0]) != 0)
					continue;*/
                codepoint = (uint)c;

                glyph = fons__getGlyph(stash, font, codepoint, isize, iblur, (int)FONSglyphBitmap.FONS_GLYPH_BITMAP_REQUIRED);
                if (glyph != null)
                {
                    fons__getQuad(stash, font, prevGlyphIndex, glyph, scale, state.spacing, ref x, ref y, ref q);

                    if (stash.nverts + 6 > FONS_VERTEX_COUNT)
                        fons__flush(stash);

                    fons__vertex(stash, q.x0, q.y0, q.s0, q.t0, state.color);
                    fons__vertex(stash, q.x1, q.y1, q.s1, q.t1, state.color);
                    fons__vertex(stash, q.x1, q.y0, q.s1, q.t0, state.color);

                    fons__vertex(stash, q.x0, q.y0, q.s0, q.t0, state.color);
                    fons__vertex(stash, q.x0, q.y1, q.s0, q.t1, state.color);
                    fons__vertex(stash, q.x1, q.y1, q.s1, q.t1, state.color);
                }
                prevGlyphIndex = glyph != null ? glyph.index : -1;
            }
            fons__flush(stash);

            return x;
        }

        public static void fonsSetSpacing(ref FONScontext stash, float spacing)
        {
            //fons__getState(stash)->spacing = spacing;
            FONSstate fs;
            fons__getState(stash, out fs);
            fs.spacing = spacing;
        }

        public static void fonsSetBlur(ref FONScontext stash, float blur)
        {
            //fons__getState(stash)->blur = blur;
            FONSstate fs;
            fons__getState(stash, out fs);
            fs.blur = blur;
        }

        public static void fonsSetAlign(FONScontext stash, FONSalign align)
        {
            FONSstate fs;
            fons__getState(stash, out fs);
            fs.align = align;
        }

        public static void fonsLineBounds(FONScontext stash, float y, ref float miny, ref float maxy)
        {
            FONSfont font;
            FONSstate state;
            fons__getState(stash, out state);
            short isize;

            if (stash == null)
                return;
            if (state.font < 0 || state.font >= stash.nfonts)
                return;
            font = stash.fonts[state.font];
            isize = (short)(state.size * 10.0f);
            if (font.data == null)
                return;

            y += fons__getVertAlign(stash, font, state.align, isize);

            if (((int)stash.@params.flags & (int)FONSflags.FONS_ZERO_TOPLEFT) != 0)
            {
                miny = y - font.ascender * (float)isize / 10.0f;
                maxy = miny + font.lineh * isize / 10.0f;
            }
            else
            {
                maxy = y + font.descender * (float)isize / 10.0f;
                miny = maxy - font.lineh * isize / 10.0f;
            }
        }

        public static float fonsTextBounds(ref FONScontext stash,
                                           float x, float y,
                                           byte[] str,
                                           float[] bounds)
        {
            FONSstate state;
            fons__getState(stash, out state);
            uint codepoint = 0;
            uint utf8state = 0;
            FONSquad q = new FONSquad();
            FONSglyph glyph = null;
            int prevGlyphIndex = -1;
            short isize = (short)(state.size * 10.0f);
            short iblur = (short)state.blur;
            float scale;
            FONSfont font;
            float startx, advance;
            float minx, miny, maxx, maxy;

            if (stash == null) return 0;        // Jun-15-18 S.P. NOTE: Was commented in C# Port
            if (state.font < 0 || state.font >= stash.nfonts) return 0;

            font = stash.fonts[state.font];
            if (font.data == null) return 0;

            scale = fons__tt_getPixelHeightScale(ref font.font, (float)isize / 10.0f);

            // Align vertically.
            y += fons__getVertAlign(stash, font, state.align, isize);

            minx = maxx = x;
            miny = maxy = y;
            startx = x;

            //if (end == null)                // Jun-15-18 S.P. NOTE: Was commented in C# Port
            //    end = str + str.Length;     // Jun-15-18 S.P. NOTE: Was commented in C# Port

            if (null == str)
                return 0f;

            //foreach (char c in str)
            for (int i = 0; str.Length > i; ++i)
            {
                //byte[] bytes = Encoding.UTF8.GetBytes(c.ToString());

                //if (fons__decutf8(ref utf8state, ref codepoint, bytes[0]) != 0)
                //	continue;
                if (fons__decutf8(ref utf8state, ref codepoint, (uint)str[i]) > 0)
                    continue;
                //codepoint = (uint)c;

                glyph = fons__getGlyph(stash, font, codepoint, isize, iblur, (int)FONSglyphBitmap.FONS_GLYPH_BITMAP_OPTIONAL);
                if (glyph != null)
                {
                    fons__getQuad(stash, font, prevGlyphIndex, glyph, scale, state.spacing, ref x, ref y, ref q);
                    if (q.x0 < minx)
                        minx = q.x0;
                    if (q.x1 > maxx)
                        maxx = q.x1;
                    if (stash.@params.flags == FONSflags.FONS_ZERO_TOPLEFT)
                    {
                        if (q.y0 < miny)
                            miny = q.y0;
                        if (q.y1 > maxy)
                            maxy = q.y1;
                    }
                    else
                    {
                        if (q.y1 < miny)
                            miny = q.y1;
                        if (q.y0 > maxy)
                            maxy = q.y0;
                    }
                }
                prevGlyphIndex = glyph != null ? glyph.index : -1;
            }

            advance = x - startx;

            // Align horizontally
            if ((state.align & FONSalign.FONS_ALIGN_LEFT) != 0)
            {
                // empty
            }
            else if ((state.align & FONSalign.FONS_ALIGN_RIGHT) != 0)
            {
                minx -= advance;
                maxx -= advance;
            }
            else if ((state.align & FONSalign.FONS_ALIGN_CENTER) != 0)
            {
                minx -= advance * 0.5f;
                maxx -= advance * 0.5f;
            }

            if (bounds != null)
            {
                bounds[0] = minx;
                bounds[1] = miny;
                bounds[2] = maxx;
                bounds[3] = maxy;
            }

            return advance;
        }

        public static void fonsVertMetrics(ref FONScontext stash,
                                           ref float ascender, ref float descender, ref float lineh)
        {
            FONSfont font;
            FONSstate state;
            fons__getState(stash, out state);
            short isize;

            if (state.font < 0 || state.font >= stash.nfonts)
                return;
            font = stash.fonts[state.font];
            isize = (short)(state.size * 10.0f);
            if (font.data == null)
                return;

            ascender = font.ascender * isize / 10.0f;
            descender = font.descender * isize / 10.0f;
            lineh = font.lineh * isize / 10.0f;
        }

        public static void fonsSetSize(ref FONScontext stash, float size)
        {
            FONSstate fs;
            fons__getState(stash, out fs);
            fs.size = size;
        }

        public static void fonsSetFont(ref FONScontext stash, int font)
        {
            FONSstate fs;
            fons__getState(stash, out fs);
            fs.font = font;
        }

        public static void fonsSetColor(ref FONScontext stash, uint color)
        {
            FONSstate fs;
            fons__getState(stash, out fs);
            fs.color = color;
        }


        public static int fonsGetFontByName(FONScontext s, string name)
        {
            int i;
            for (i = 0; i < s.nfonts; i++)
            {
                if (String.Compare(s.fonts[i].name, name, true) == 0)
                    return i;
            }
            return FONS_INVALID;
        }

        public static byte[] fonsGetTextureData(FONScontext stash, ref int width, ref int height)
        {
            width = stash.@params.width;
            height = stash.@params.height;
            return stash.texData;
        }

        public static int fonsValidateTexture(FONScontext stash, int[] dirty)
        {
            if (stash.dirtyRect[0] < stash.dirtyRect[2] && stash.dirtyRect[1] < stash.dirtyRect[3])
            {
                dirty[0] = stash.dirtyRect[0];
                dirty[1] = stash.dirtyRect[1];
                dirty[2] = stash.dirtyRect[2];
                dirty[3] = stash.dirtyRect[3];
                // Reset dirty rect
                stash.dirtyRect[0] = stash.@params.width;
                stash.dirtyRect[1] = stash.@params.height;
                stash.dirtyRect[2] = 0;
                stash.dirtyRect[3] = 0;
                return 1;
            }
            return 0;
        }

        public static int fonsTextIterInit(FONScontext stash, ref FONStextIter iter,
                                           float x, float y, byte[] str)
        {
            FONSstate state;
            fons__getState(stash, out state);
            float width;

            //memset(iter, 0, sizeof(*iter));
            iter = new FONStextIter();

            if (stash == null)
                return 0;
            if (state.font < 0 || state.font >= stash.nfonts)
                return 0;
            iter.font = stash.fonts[state.font];
            if (iter.font.data == null)
                return 0;

            iter.isize = (short)(state.size * 10.0f);
            iter.iblur = (short)state.blur;
            iter.scale = fons__tt_getPixelHeightScale(ref iter.font.font, (float)iter.isize / 10.0f);

            // Align horizontally
            if ((state.align & FONSalign.FONS_ALIGN_LEFT) != 0)
            {
                // empty
            }
            else if ((state.align & FONSalign.FONS_ALIGN_RIGHT) != 0)
            {
                width = fonsTextBounds(ref stash, x, y, str, null);
                x -= width;
            }
            else if ((state.align & FONSalign.FONS_ALIGN_CENTER) != 0)
            {
                width = fonsTextBounds(ref stash, x, y, str, null);
                x -= width * 0.5f;
            }
            // Align vertically.
            y += fons__getVertAlign(stash, iter.font, state.align, iter.isize);

            //if (end == null)
            //	end = str + str.Length;

            iter.x = iter.nextx = x;
            iter.y = iter.nexty = y;
            iter.spacing = state.spacing;
            iter.str = str;
            iter.iNext = 0;
            //iter.end = end;
            iter.codepoint = 0;
            iter.prevGlyphIndex = -1;

            return 1;
        }

        public static int fonsTextIterNext(FONScontext stash, ref FONStextIter iter, ref FONSquad quad)
        {
            int cont = 0;
            FONSglyph glyph = null;

            if (iter.iNext == iter.str.Length)
                return 0;

            iter.iStr = iter.iNext;

            for (cont = iter.iNext; cont < iter.str.Length; cont++)
            {
                if (fons__decutf8(ref iter.utf8state, ref iter.codepoint, (uint)iter.str[cont]) != 0)
                    continue;
                //if (fons__decutf8(ref iter.utf8state, ref iter.codepoint, (byte)iter.str[cont]) != 0)
                //	continue;
                //byte[] bytes = Encoding.UTF8.GetBytes(iter.str[cont].ToString());
                //if (bytes.Length > 1)
                //	continue;
                //else
                //	iter.codepoint = (uint)iter.str[cont];

                // TODO
                //if (iter.codepoint == 55357)
                //{
                //	//uint cp1 = (uint)iter.str[1];
                //	iter.codepoint = 128269;
                //}

                //if (iter.codepoint == 0)
                //	continue;
                //str++;
                // Get glyph and quad
                iter.x = iter.nextx;
                iter.y = iter.nexty;
                glyph = fons__getGlyph(stash, iter.font, iter.codepoint, iter.isize, iter.iblur, iter.bitmapOption);
                if (glyph != null)
                    fons__getQuad(stash, iter.font, iter.prevGlyphIndex, glyph, iter.scale, iter.spacing,
                        ref iter.nextx, ref iter.nexty, ref quad);
                iter.prevGlyphIndex = glyph != null ? glyph.index : -1;
                break;
            }
            iter.iNext = ++cont;

            return 1;
        }

        public static int fonsResetAtlas(FONScontext stash, int width, int height)
        {
            int i, j;
            if (stash == null)
                return 0;

            // Flush pending glyphs.
            fons__flush(stash);

            // Create new texture
            if (stash.@params.renderResize != null)
            {
                if (stash.@params.renderResize(stash.@params.userPtr, width, height) == 0)
                    return 0;
            }

            // Reset atlas
            fons__atlasReset(ref stash.atlas, width, height);

            // Clear texture data.
            stash.texData = new byte[width * height];
            if (stash.texData == null)
                return 0;
            //memset(stash->texData, 0, width * height);
            Array.Clear(stash.texData, 0, width * height);

            // Reset dirty rect
            stash.dirtyRect[0] = width;
            stash.dirtyRect[1] = height;
            stash.dirtyRect[2] = 0;
            stash.dirtyRect[3] = 0;

            // Reset cached glyphs
            for (i = 0; i < stash.nfonts; i++)
            {
                FONSfont font = stash.fonts[i];
                font.nglyphs = 0;
                for (j = 0; j < FONS_HASH_LUT_SIZE; j++)
                    font.lut[j] = -1;
            }

            stash.@params.width = width;
            stash.@params.height = height;
            stash.@itw = 1.0f / stash.@params.width;
            stash.@ith = 1.0f / stash.@params.height;

            // Add white rect at 0,0 for debug drawing.
            fons__addWhiteRect(ref stash, 2, 2);

            return 1;
        }

        #endregion Public-methods

        #region Private-methods

        public static int fonsAddFontMem(FONScontext stash, string name, byte[] data, uint dataSize, int freeData = 1)
        {
            int i, ascent = 0, descent = 0, fh, lineGap = 0;
            FONSfont font;

            int idx = fons__allocFont(stash);
            if (idx == FONS_INVALID)
                return FONS_INVALID;

            font = stash.fonts[idx];
            font.name = name;

            // Init hash lookup.
            for (i = 0; i < FontStash.FONS_HASH_LUT_SIZE; ++i)
                font.lut[i] = -1;

            // Read in the font data.
            font.dataSize = (int)dataSize;
            font.data = data;
            font.freeData = (byte)freeData;

            // Init font
            stash.nscratch = 0;
            if (fons__tt_loadFont(stash, ref font.font, data, (int)dataSize) != 0)
            {
                // Store normalized line height. The real line height is got
                // by multiplying the lineh by font size.
                fons__tt_getFontVMetrics(ref font.font, ref ascent, ref descent, ref lineGap);
                fh = ascent - descent;
                font.ascender = (float)ascent / (float)fh;
                font.descender = (float)descent / (float)fh;
                font.lineh = (float)(fh + lineGap) / (float)fh;

                return idx;
            }

            return FONS_INVALID;
        }

        static int fons__tt_getGlyphIndex(ref FONSttFontImpl font, int codepoint)
        {
            return Stb_truetype.stbtt_FindGlyphIndex(ref font.font, codepoint);
        }

        static int fons__tt_getGlyphKernAdvance(ref FONSttFontImpl font, int glyph1, int glyph2)
        {
            return Stb_truetype.stbtt_GetGlyphKernAdvance(ref font.font, glyph1, glyph2);
        }

        static void fons__tt_getFontVMetrics(ref FONSttFontImpl font, ref int ascent, ref int descent, ref int lineGap)
        {
            Stb_truetype.stbtt_GetFontVMetrics(ref font.font, ref ascent, ref descent, ref lineGap);
        }

        static int fons__tt_loadFont(FONScontext context, ref FONSttFontImpl font, byte[] data, int dataSize)
        {
            int stbError;
            //FONS_NOTUSED(dataSize);

            font.font.userdata = context;
            stbError = Stb_truetype.stbtt_InitFont(ref font.font, data, 0);
            return stbError;
        }

        static int fons__allocFont(FONScontext stash)
        {
            FONSfont font = new FONSfont();

            if (stash.nfonts + 1 > stash.cfonts)
            {
                stash.cfonts = stash.cfonts == 0 ? 8 : stash.cfonts * 2;
                //stash.fonts = (FONSfont**)realloc(stash->fonts, sizeof(FONSfont*) * stash->cfonts);
                Array.Resize(ref stash.fonts, (int)stash.cfonts);
                if (stash.fonts == null)
                    return -1;
            }

            font.glyphs = new FONSglyph[FONS_INIT_GLYPHS];
            for (int cont = 0; cont < FONS_INIT_GLYPHS; cont++)
                font.glyphs[cont] = new FONSglyph();

            if (font.glyphs != null)
            {
                font.cglyphs = (int)FontStash.FONS_INIT_GLYPHS;
                font.nglyphs = 0;

                stash.fonts[stash.nfonts++] = font;
                return stash.nfonts - 1;
            }
            return FONS_INVALID;
        }

        static void fons__freeFont(FONSfont font)
        {
            if (font == null)
                return;
            //if (font.glyphs) 
            //	free(font->glyphs);
            //if (font.freeData && font.data) 
            //	free(font->data);
            //free(font);
            font = null;
        }

        static FONSglyph fons__getGlyph0(FONScontext stash, FONSfont font, uint codepoint,
                                        short isize, short iblur, 
                                        int bitmapOption) // int bitmapOption Added Jun-15-18
        {
            int i, g = 0, advance = 0, lsb = 0, x0 = 0, y0 = 0, x1 = 0, y1 = 0, gw, gh, gx = 0, gy = 0, x, y;
            float scale;
            FONSglyph glyph = null;
            uint h;
            float size = isize / 10.0f;
            int pad, added;
            byte[] bdst;
            byte[] dst;
            FONSfont renderFont = font;

            if (isize < 2)
                return null;
            if (iblur > 20)
                iblur = 20;
            pad = iblur + 2;

            // Reset allocator.
            stash.nscratch = 0;

            // Find code point and size.
            h = fons__hashint(codepoint) & (FONS_HASH_LUT_SIZE - 1);
            i = font.lut[h];
            while (i != -1)
            {
                if (font.glyphs[i].codepoint == codepoint && font.glyphs[i].size == isize && font.glyphs[i].blur == iblur)
                {
                    glyph = font.glyphs[i];

                    if (bitmapOption == (int)FONSglyphBitmap.FONS_GLYPH_BITMAP_OPTIONAL || (glyph.x0 >= 0 && glyph.y0 >= 0))
                        return glyph;
                    
                    // At this point, glyph exists but the bitmap data is not yet created.
                    break;

                }
                i = font.glyphs[i].next;
            }

            // Create a new glyph or rasterize bitmap data for a cached glyph.
            g = fons__tt_getGlyphIndex(ref font.font, (int)codepoint);
            // Try to find the glyph in fallback fonts.
            if (g == 0)
            {
                for(i = 0; i < font.nfallbacks; i++)
                {
                    FONSfont fallbackFont = stash.fonts[font.fallbacks[i]];
                    int fallbackIndex = fons__tt_getGlyphIndex(ref fallbackFont.font, (int)codepoint);
                    if(fallbackIndex != 0)
                    {
                        g = fallbackIndex;
                        renderFont = fallbackFont;
                        break;
                    }
                }
                // It is possible that we did not find a fallback glyph.
                // In that case the glyph index 'g' is 0, and we'll proceed below and cache empty glyph.
            }

            scale = fons__tt_getPixelHeightScale(ref font.font, size);
            fons__tt_buildGlyphBitmap(ref renderFont.font, g, size, scale, ref advance, ref lsb, ref x0, ref y0, ref x1, ref y1);
            gw = x1 - x0 + pad * 2;
            gh = y1 - y0 + pad * 2;

            // Determines the spot to draw glyph in the atlas.
            if(bitmapOption == (int)FONSglyphBitmap.FONS_GLYPH_BITMAP_REQUIRED)
            {
                // Find free spot for the rect in the atlas
                added = fons__atlasAddRect(ref stash.atlas, gw, gh, ref gx, ref gy);
                if (added == 0 && stash.handleError != null)
                {
                    // Atlas is full, let the user to resize the atlas (or not), and try again.
                    stash.handleError(stash.errorUptr, FONSerrorCode.FONS_ATLAS_FULL, 0);
                    added = fons__atlasAddRect(ref stash.atlas, gw, gh, ref gx, ref gy);
                }
                if (added == 0)
                    return null;
            }
            else
            {
                // Negative coordinate indicates there is no bitmap data created.
                gx = -1;
                gy = -1;
            }

            // Init glyph.
            if(glyph == null)
            {
                fons__allocGlyph(font, out glyph);
                glyph.codepoint = codepoint;
                glyph.size = isize;
                glyph.blur = iblur;
                glyph.next = 0;
                // Insert char to hash lookup.
                glyph.next = font.lut[h];
                font.lut[h] = font.nglyphs - 1;
            }
            glyph.index = g;
            glyph.x0 = (short)gx;
            glyph.y0 = (short)gy;
            glyph.x1 = (short)(glyph.x0 + gw);
            glyph.y1 = (short)(glyph.y0 + gh);
            glyph.xadv = (short)(scale * advance * 10.0f);
            glyph.xoff = (short)(x0 - pad);
            glyph.yoff = (short)(y0 - pad);

            if (null == glyph)
                throw new Exception("Glyph failed");

            if (bitmapOption == (int)FONSglyphBitmap.FONS_GLYPH_BITMAP_OPTIONAL)
                return glyph;


            // Rasterize
            dst = stash.texData;
            int index = (glyph.x0 + pad) + (glyph.y0 + pad) * stash.@params.width;
            fons__tt_renderGlyphBitmap(ref font.font, dst, index, gw - pad * 2, gh - pad * 2, stash.@params.width, scale, scale, g);

            // Make sure there is one pixel empty border.
            dst = stash.texData;
            index = glyph.x0 + glyph.y0 * stash.@params.width;
            for (y = 0; y < gh; y++)
            {
                dst[y * stash.@params.width + index] = 0;
                dst[gw - 1 + y * stash.@params.width + index] = 0;
            }
            for (x = 0; x < gw; x++)
            {
                dst[x + index] = 0;
                dst[x + (gh - 1) * stash.@params.width + index] = 0;
            }

            // Debug code to color the glyph background
            /*	unsigned char* fdst = &stash->texData[glyph->x0 + glyph->y0 * stash->params.width];
	for (y = 0; y < gh; y++) {
		for (x = 0; x < gw; x++) {
			int a = (int)fdst[x+y*stash->params.width] + 20;
			if (a > 255) a = 255;
			fdst[x+y*stash->params.width] = a;
		}
	}*/

            // Blur
            if (iblur > 0)
            {
                stash.nscratch = 0;
                bdst = stash.texData;
                index = glyph.x0 + glyph.y0 * stash.@params.width;
                fons__blur(stash, bdst, index, gw, gh, stash.@params.width, iblur);
            }

            stash.dirtyRect[0] = fons__mini(stash.dirtyRect[0], glyph.x0);
            stash.dirtyRect[1] = fons__mini(stash.dirtyRect[1], glyph.y0);
            stash.dirtyRect[2] = fons__maxi(stash.dirtyRect[2], glyph.x1);
            stash.dirtyRect[3] = fons__maxi(stash.dirtyRect[3], glyph.y1);

            return glyph;
        }

        static FONSglyph fons__getGlyph(FONScontext stash, FONSfont font, uint codepoint,
                                        short isize, short iblur,
                                        int bitmapOption) // bitmapOption NOT USED  Added Jun-15-18
        {
            int i, g, advance = 0, lsb = 0, x0 = 0, y0 = 0, x1 = 0, y1 = 0, gw, gh, gx = 0, gy = 0, x, y;
            float scale;
            FONSglyph glyph;
            uint h;
            float size = isize / 10.0f;
            int pad, added;
            byte[] bdst;
            byte[] dst;

            if (isize < 2)
                return null;
            if (iblur > 20)
                iblur = 20;
            pad = iblur + 2;

            // Reset allocator.
            stash.nscratch = 0;

            // Find code point and size.
            h = fons__hashint(codepoint) & (FONS_HASH_LUT_SIZE - 1);
            i = font.lut[h];
            while (i != -1)
            {
                if (font.glyphs[i].codepoint == codepoint && font.glyphs[i].size == isize && font.glyphs[i].blur == iblur)
                    return font.glyphs[i];
                i = font.glyphs[i].next;
            }

            // Could not find glyph, create it.
            scale = fons__tt_getPixelHeightScale(ref font.font, size);
            g = fons__tt_getGlyphIndex(ref font.font, (int)codepoint);
            fons__tt_buildGlyphBitmap(ref font.font, g, size, scale, ref advance, ref lsb,
                ref x0, ref y0, ref x1, ref y1);
            gw = x1 - x0 + pad * 2;
            gh = y1 - y0 + pad * 2;

            // Find free spot for the rect in the atlas
            added = fons__atlasAddRect(ref stash.atlas, gw, gh, ref gx, ref gy);
            if (added == 0 && stash.handleError != null)
            {
                // Atlas is full, let the user to resize the atlas (or not), and try again.
                stash.handleError(stash.errorUptr, FONSerrorCode.FONS_ATLAS_FULL, 0);
                added = fons__atlasAddRect(ref stash.atlas, gw, gh, ref gx, ref gy);
            }
            if (added == 0)
                return null;

            // Init glyph.
            fons__allocGlyph(font, out glyph);

            if (null == glyph)
                throw new Exception("Glyph failed");


            glyph.codepoint = codepoint;
            glyph.size = isize;
            glyph.blur = iblur;
            glyph.index = g;
            glyph.x0 = (short)gx;
            glyph.y0 = (short)gy;
            glyph.x1 = (short)(glyph.x0 + gw);
            glyph.y1 = (short)(glyph.y0 + gh);
            glyph.xadv = (short)(scale * advance * 10.0f);
            glyph.xoff = (short)(x0 - pad);
            glyph.yoff = (short)(y0 - pad);
            glyph.next = 0;

            // Insert char to hash lookup.
            glyph.next = font.lut[h];
            font.lut[h] = font.nglyphs - 1;

            // Rasterize
            dst = stash.texData;
            int index = (glyph.x0 + pad) + (glyph.y0 + pad) * stash.@params.width;
            fons__tt_renderGlyphBitmap(ref font.font, dst, index, gw - pad * 2, gh - pad * 2, stash.@params.width, scale, scale, g);

            // Make sure there is one pixel empty border.
            dst = stash.texData;
            index = glyph.x0 + glyph.y0 * stash.@params.width;
            for (y = 0; y < gh; y++)
            {
                dst[y * stash.@params.width + index] = 0;
                dst[gw - 1 + y * stash.@params.width + index] = 0;
            }
            for (x = 0; x < gw; x++)
            {
                dst[x + index] = 0;
                dst[x + (gh - 1) * stash.@params.width + index] = 0;
            }

            // Debug code to color the glyph background
            /*	unsigned char* fdst = &stash->texData[glyph->x0 + glyph->y0 * stash->params.width];
	for (y = 0; y < gh; y++) {
		for (x = 0; x < gw; x++) {
			int a = (int)fdst[x+y*stash->params.width] + 20;
			if (a > 255) a = 255;
			fdst[x+y*stash->params.width] = a;
		}
	}*/

            // Blur
            if (iblur > 0)
            {
                stash.nscratch = 0;
                bdst = stash.texData;
                index = glyph.x0 + glyph.y0 * stash.@params.width;
                fons__blur(stash, bdst, index, gw, gh, stash.@params.width, iblur);
            }

            stash.dirtyRect[0] = fons__mini(stash.dirtyRect[0], glyph.x0);
            stash.dirtyRect[1] = fons__mini(stash.dirtyRect[1], glyph.y0);
            stash.dirtyRect[2] = fons__maxi(stash.dirtyRect[2], glyph.x1);
            stash.dirtyRect[3] = fons__maxi(stash.dirtyRect[3], glyph.y1);

            return glyph;
        }

        static void fons__blurCols(byte[] dst, int iDst, int w, int h, int dstStride, int alpha)
        {
            int x, y;
            for (y = 0; y < h; y++)
            {
                int z = 0; // force zero border
                for (x = 1; x < w; x++)
                {
                    z += (alpha * (((int)(dst[x + iDst]) << ZPREC) - z)) >> APREC;
                    dst[x + iDst] = (byte)(z >> ZPREC);
                }
                dst[w - 1 + iDst] = 0; // force zero border
                z = 0;
                for (x = w - 2; x >= 0; x--)
                {
                    z += (alpha * (((int)(dst[x + iDst]) << ZPREC) - z)) >> APREC;
                    dst[x + iDst] = (byte)(z >> ZPREC);
                }
                dst[iDst] = 0; // force zero border
                iDst += dstStride;
            }
        }

        static void fons__blurRows(byte[] dst, int iDst, int w, int h, int dstStride, int alpha)
        {
            int x, y;
            for (x = 0; x < w; x++)
            {
                int z = 0; // force zero border
                for (y = dstStride; y < h * dstStride; y += dstStride)
                {
                    z += (alpha * (((int)(dst[y + iDst]) << ZPREC) - z)) >> APREC;
                    dst[y + iDst] = (byte)(z >> ZPREC);
                }
                dst[iDst + ((h - 1) * dstStride)] = 0; // force zero border
                z = 0;
                for (y = (h - 2) * dstStride; y >= 0; y -= dstStride)
                {
                    z += (alpha * (((int)(dst[y + iDst]) << ZPREC) - z)) >> APREC;
                    dst[y + iDst] = (byte)(z >> ZPREC);
                }
                dst[iDst] = 0; // force zero border
                iDst++;
            }
        }


        static void fons__blur(FONScontext stash, byte[] dst, int iDst, int w, int h, int dstStride, int blur)
        {
            int alpha;
            float sigma;
            //(void)stash;

            if (blur < 1)
                return;
            // Calculate the alpha such that 90% of the kernel is within the radius. (Kernel extends to infinity)
            sigma = (float)blur * 0.57735f; // 1 / sqrt(3)
            alpha = (int)((1 << APREC) * (1.0f - Math.Exp(-2.3f / (sigma + 1.0f))));
            fons__blurRows(dst, iDst, w, h, dstStride, alpha);
            fons__blurCols(dst, iDst, w, h, dstStride, alpha);
            fons__blurRows(dst, iDst, w, h, dstStride, alpha);
            fons__blurCols(dst, iDst, w, h, dstStride, alpha);
            //	fons__blurrows(dst, w, h, dstStride, alpha);
            //	fons__blurcols(dst, w, h, dstStride, alpha);
        }

        static void fons__allocGlyph(FONSfont font, out FONSglyph glyph)
        {
            if (font.nglyphs + 1 > font.cglyphs)
            {
                font.cglyphs = font.cglyphs == 0 ? 8 : font.cglyphs * 2;
                Array.Resize(ref font.glyphs, font.cglyphs);
                if (font.glyphs == null)
                {
                    glyph = null;
                    return;
                }
            }
            font.nglyphs++;
            glyph = font.glyphs[font.nglyphs - 1];
        }

        static void fons__tt_renderGlyphBitmap(ref FONSttFontImpl font, byte[] output, int iOutput,
                                               int outWidth, int outHeight, int outStride,
                                               float scaleX, float scaleY, int glyph)
        {
            Stb_truetype.stbtt_MakeGlyphBitmap(ref font.font, output, iOutput, outWidth, outHeight, outStride, scaleX, scaleY, glyph);
        }

        static int fons__tt_buildGlyphBitmap(ref FONSttFontImpl font, int glyph, float size, float scale,
                                             ref int advance, ref int lsb, ref int x0, ref int y0, ref int x1, ref int y1)
        {
            //FONS_NOTUSED(size);
            Stb_truetype.stbtt_GetGlyphHMetrics(ref font.font, glyph, ref advance, ref lsb);
            Stb_truetype.stbtt_GetGlyphBitmapBox(ref font.font, glyph, scale, scale, ref x0, ref y0, ref x1, ref y1);
            return 1;
        }

        static uint fons__hashint(uint a)
        {
            a += ~(a << 15);
            a ^= (a >> 10);
            a += (a << 3);
            a ^= (a >> 6);
            a += ~(a << 11);
            a ^= (a >> 16);
            return a;
        }

        static void fons__flush(FONScontext stash)
        {
            // Flush texture
            if (stash.dirtyRect[0] < stash.dirtyRect[2] && stash.dirtyRect[1] < stash.dirtyRect[3])
            {
                // TODO
                if (stash.@params.renderUpdate != null)
                    stash.@params.renderUpdate(stash.@params.userPtr, ref stash.dirtyRect, stash.texData);
                // Reset dirty rect
                stash.dirtyRect[0] = stash.@params.width;
                stash.dirtyRect[1] = stash.@params.height;
                stash.dirtyRect[2] = 0;
                stash.dirtyRect[3] = 0;
            }

            // Flush triangles
            if (stash.nverts > 0)
            {
                if (stash.@params.renderDraw != null)
                    stash.@params.renderDraw(stash.@params.userPtr, stash.verts, stash.tcoords, stash.colors, stash.nverts);
                stash.nverts = 0;
            }
        }

        static void fons__getQuad(FONScontext stash, FONSfont font,
                                  int prevGlyphIndex, FONSglyph glyph,
                                  float scale, float spacing, ref float x, ref float y, ref FONSquad q)
        {
            float rx, ry, xoff, yoff, x0, y0, x1, y1;

            if (prevGlyphIndex != -1)
            {
                float adv = fons__tt_getGlyphKernAdvance(ref font.font, prevGlyphIndex, glyph.index) * scale;
                x += (int)(adv + spacing + 0.5f);
            }

            // Each glyph has 2px border to allow good interpolation,
            // one pixel to prevent leaking, and one to allow good interpolation for rendering.
            // Inset the texture region by one pixel for corret interpolation.
            xoff = (short)(glyph.xoff + 1);
            yoff = (short)(glyph.yoff + 1);
            x0 = (float)(glyph.x0 + 1);
            y0 = (float)(glyph.y0 + 1);
            x1 = (float)(glyph.x1 - 1);
            y1 = (float)(glyph.y1 - 1);

            if (stash.@params.flags == FONSflags.FONS_ZERO_TOPLEFT)
            {
                rx = (float)(int)(x + xoff);
                ry = (float)(int)(y + yoff);

                q.x0 = rx;
                q.y0 = ry;
                q.x1 = rx + x1 - x0;
                q.y1 = ry + y1 - y0;

                q.s0 = x0 * stash.itw;
                q.t0 = y0 * stash.ith;
                q.s1 = x1 * stash.itw;
                q.t1 = y1 * stash.ith;
            }
            else
            {
                rx = (float)(int)(x + xoff);
                ry = (float)(int)(y - yoff);

                q.x0 = rx;
                q.y0 = ry;
                q.x1 = rx + x1 - x0;
                q.y1 = ry - y1 + y0;

                q.s0 = x0 * stash.itw;
                q.t0 = y0 * stash.ith;
                q.s1 = x1 * stash.itw;
                q.t1 = y1 * stash.ith;
            }

            x += (int)(glyph.xadv / 10.0f + 0.5f);
        }


        static uint fons__decutf8(ref uint state, ref uint codep, uint byte_)
        {
            uint type = utf8d[byte_];

            codep = (uint)((state != FONS_UTF8_ACCEPT) ?
                ((int)byte_ & 0x3fu) | (codep << 6) :
                (0xff >> (int)type) & ((int)byte_));

            state = utf8d[256 + state + type];

            return state;
        }

        static float fons__getVertAlign(FONScontext stash, FONSfont font, FONSalign align, short isize)
        {
            if (stash.@params.flags == FONSflags.FONS_ZERO_TOPLEFT)
            {
                if ((align & FONSalign.FONS_ALIGN_TOP) != 0)
                {
                    return font.ascender * (float)isize / 10.0f;
                }
                else if ((align & FONSalign.FONS_ALIGN_MIDDLE) != 0)
                {
                    return (font.ascender + font.descender) / 2.0f * (float)isize / 10.0f;
                }
                else if ((align & FONSalign.FONS_ALIGN_BASELINE) != 0)
                {
                    return 0.0f;
                }
                else if ((align & FONSalign.FONS_ALIGN_BOTTOM) != 0)
                {
                    return font.descender * (float)isize / 10.0f;
                }
            }
            else
            {
                if ((align & FONSalign.FONS_ALIGN_TOP) != 0)
                {
                    return -font.ascender * (float)isize / 10.0f;
                }
                else if ((align & FONSalign.FONS_ALIGN_MIDDLE) != 0)
                {
                    return -(font.ascender + font.descender) / 2.0f * (float)isize / 10.0f;
                }
                else if ((align & FONSalign.FONS_ALIGN_BASELINE) != 0)
                {
                    return 0.0f;
                }
                else if ((align & FONSalign.FONS_ALIGN_BOTTOM) != 0)
                {
                    return -font.descender * (float)isize / 10.0f;
                }
            }
            return 0f;
        }

        static void fons__vertex(FONScontext stash, float x, float y, float s, float t, uint c)
        {
            stash.verts[stash.nverts * 2 + 0] = x;
            stash.verts[stash.nverts * 2 + 1] = y;
            stash.tcoords[stash.nverts * 2 + 0] = s;
            stash.tcoords[stash.nverts * 2 + 1] = t;
            stash.colors[stash.nverts] = c;
            stash.nverts++;
        }


        static void fons__getState(FONScontext stash, out FONSstate state)
        {
            state = stash.states[stash.nstates - 1];
        }

        static void fonsPushState(ref FONScontext stash)
        {
            if (stash.nstates >= FONS_MAX_STATES)
            {
                if (stash.handleError != null)
                    stash.handleError(stash.errorUptr, FONSerrorCode.FONS_STATES_OVERFLOW, 0);
                return;
            }
            if (stash.nstates > 0)
                stash.states[stash.nstates] = stash.states[stash.nstates - 1];
            stash.nstates++;
        }

        static void fonsPopState(ref FONScontext stash)
        {
            if (stash.nstates <= 1)
            {
                if (stash.handleError != null)
                    stash.handleError(stash.errorUptr, FONSerrorCode.FONS_STATES_UNDERFLOW, 0);
                return;
            }
            stash.nstates--;
        }

        static float fons__tt_getPixelHeightScale(ref FONSttFontImpl font, float size)
        {
            return Stb_truetype.stbtt_ScaleForPixelHeight(ref font.font, size);
        }

        static void fons__addWhiteRect(ref FONScontext stash, int w, int h)
        {
            int x, y, gx = 0, gy = 0;
            byte[] dst;
            if (fons__atlasAddRect(ref stash.atlas, w, h, ref gx, ref gy) == 0)
                return;

            // Rasterize
            dst = stash.texData; //[gx + gy * stash.@params.width];
            int pointer = gx + gy * stash.@params.width;
            for (y = 0; y < h; y++)
            {
                for (x = 0; x < w; x++)
                    dst[pointer + x] = (byte)0xff;
                pointer += (byte)stash.@params.width;
            }

            stash.dirtyRect[0] = fons__mini(stash.dirtyRect[0], gx);
            stash.dirtyRect[1] = fons__mini(stash.dirtyRect[1], gy);
            stash.dirtyRect[2] = fons__maxi(stash.dirtyRect[2], gx + w);
            stash.dirtyRect[3] = fons__maxi(stash.dirtyRect[3], gy + h);
        }

        static int fons__atlasAddRect(ref FONSatlas atlas, int rw, int rh, ref int rx, ref int ry)
        {
            int besth = atlas.height, bestw = atlas.width, besti = -1;
            int bestx = -1, besty = -1, i;

            // Bottom left fit heuristic.
            for (i = 0; i < atlas.nnodes; i++)
            {
                int y = fons__atlasRectFits(ref atlas, i, rw, rh);
                if (y != -1)
                {
                    short nw = atlas.nodes[i].width;
                    if (y + rh < besth || (y + rh == besth && nw < bestw))
                    {
                        besti = i;
                        bestw = atlas.nodes[i].width;
                        besth = y + rh;
                        bestx = atlas.nodes[i].x;
                        besty = y;
                    }
                }
            }

            if (besti == -1)
                return 0;

            // Perform the actual packing.
            if (fons__atlasAddSkylineLevel(ref atlas, besti, bestx, besty, rw, rh) == 0)
                return 0;

            rx = bestx;
            ry = besty;

            return 1;
        }

        static int fons__atlasAddSkylineLevel(ref FONSatlas atlas, int idx, int x, int y, int w, int h)
        {
            int i;

            // Insert new node
            if (fons__atlasInsertNode(ref atlas, idx, x, y + h, w) == 0)
                return 0;

            // Delete skyline segments that fall under the shaodw of the new segment.
            for (i = idx + 1; i < atlas.nnodes; i++)
            {
                if (atlas.nodes[i].x < atlas.nodes[i - 1].x + atlas.nodes[i - 1].width)
                {
                    int shrink = atlas.nodes[i - 1].x + atlas.nodes[i - 1].width - atlas.nodes[i].x;
                    atlas.nodes[i].x += (short)shrink;
                    atlas.nodes[i].width -= (short)shrink;
                    if (atlas.nodes[i].width <= 0)
                    {
                        fons__atlasRemoveNode(ref atlas, i);
                        i--;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            // Merge same height skyline segments that are next to each other.
            for (i = 0; i < atlas.nnodes - 1; i++)
            {
                if (atlas.nodes[i].y == atlas.nodes[i + 1].y)
                {
                    atlas.nodes[i].width += atlas.nodes[i + 1].width;
                    fons__atlasRemoveNode(ref atlas, i + 1);
                    i--;
                }
            }

            return 1;
        }

        static int fons__atlasInsertNode(ref FONSatlas atlas, int idx, int x, int y, int w)
        {
            int i;
            // Insert node
            if (atlas.nnodes + 1 > atlas.cnodes)
            {
                atlas.cnodes = atlas.cnodes == 0 ? 8 : atlas.cnodes * 2;
                Array.Resize(ref atlas.nodes, (int)atlas.cnodes);
            }
            for (i = atlas.nnodes; i > idx; i--)
            {
                atlas.nodes[i].x = atlas.nodes[i - 1].x;
                atlas.nodes[i].y = atlas.nodes[i - 1].y;
                atlas.nodes[i].width = atlas.nodes[i - 1].width;
            }
            atlas.nodes[idx].x = (short)x;
            atlas.nodes[idx].y = (short)y;
            atlas.nodes[idx].width = (short)w;
            atlas.nnodes++;

            return 1;
        }

        static void fons__atlasRemoveNode(ref FONSatlas atlas, int idx)
        {
            int i;
            if (atlas.nnodes == 0)
                return;
            for (i = idx; i < atlas.nnodes - 1; i++)
            {
                //atlas.nodes[i] = atlas.nodes[i + 1];

                atlas.nodes[i].x = atlas.nodes[i + 1].x;
                atlas.nodes[i].y = atlas.nodes[i + 1].y;
                atlas.nodes[i].width = atlas.nodes[i + 1].width;
            }
            atlas.nnodes--;
        }

        static int fons__atlasRectFits(ref FONSatlas atlas, int i, int w, int h)
        {
            // Checks if there is enough space at the location of skyline span 'i',
            // and return the max height of all skyline spans under that at that location,
            // (think tetris block being dropped at that position). Or -1 if no space found.
            int x = atlas.nodes[i].x;
            int y = atlas.nodes[i].y;
            int spaceLeft;
            if (x + w > atlas.width)
                return -1;
            spaceLeft = w;
            while (spaceLeft > 0)
            {
                if (i == atlas.nnodes)
                    return -1;
                y = fons__maxi(y, atlas.nodes[i].y);
                if (y + h > atlas.height)
                    return -1;
                spaceLeft -= atlas.nodes[i].width;
                ++i;
            }
            return y;
        }

        static int fons__mini(int a, int b)
        {
            return a < b ? a : b;
        }

        static int fons__maxi(int a, int b)
        {
            return a > b ? a : b;
        }

        static bool fons__tt_init(FONScontext context)
        {
            //FONS_NOTUSED(context);
            return true;
        }

        static void fons__deleteAtlas(FONSatlas atlas)
        {
            //if (atlas == null)
            //	return;
            //if (atlas.nodes != null)
            //	free(atlas->nodes);
            //free(atlas);
        }

        // Atlas based on Skyline Bin Packer by Jukka Jylänki

        static FONSatlas fons__allocAtlas(int w, int h, uint nnodes)
        {
            FONSatlas atlas = new FONSatlas();

            atlas.width = w;
            atlas.height = h;

            // Allocate space for skyline nodes
            atlas.nodes = new FONSatlasNode[nnodes];
            for (int cont = 0; cont < nnodes; cont++)
                atlas.nodes[cont] = new FONSatlasNode();

            atlas.nnodes = 0;
            atlas.cnodes = nnodes;

            // Init root node.
            atlas.nodes[0].x = 0;
            atlas.nodes[0].y = 0;
            atlas.nodes[0].width = (short)w;
            atlas.nnodes++;

            return atlas;
        }

        static void fons__atlasReset(ref FONSatlas atlas, int w, int h)
        {
            atlas.width = w;
            atlas.height = h;
            atlas.nnodes = 0;

            // Init root node.
            atlas.nodes[0].x = 0;
            atlas.nodes[0].y = 0;
            atlas.nodes[0].width = (short)w;
            atlas.nnodes++;
        }

        static void fons__atlasExpand(ref FONSatlas atlas, int w, int h)
        {
            // Insert node for empty space
            if (w > atlas.width)
                fons__atlasInsertNode(ref atlas, atlas.nnodes, atlas.width, 0, w - atlas.width);
            atlas.width = w;
            atlas.height = h;
        }

        static void fonsGetAtlasSize(FONScontext stash, ref int width, ref int height)
        {
            if (stash == null)
                return;
            width = stash.@params.width;
            height = stash.@params.height;
        }

        static int fonsExpandAtlas(FONScontext stash, int width, int height)
        {
            int i, maxy = 0;
            byte[] data = null;
            if (stash == null)
                return 0;

            width = fons__maxi(width, stash.@params.width);
            height = fons__maxi(height, stash.@params.height);

            if (width == stash.@params.width && height == stash.@params.height)
                return 1;

            // Flush pending glyphs.
            fons__flush(stash);

            // Create new texture
            if (stash.@params.renderResize != null)
            {
                if (stash.@params.renderResize(stash.@params.userPtr, width, height) == 0)
                    return 0;
            }
            // Copy old texture data over.
            data = new byte[width * height];
            if (data == null)
                return 0;
            for (i = 0; i < stash.@params.height; i++)
            {
                byte[] dst = data;
                byte[] src = stash.texData;

                //memcpy(dst, src, stash->params.width);
                Array.Copy(src, dst, stash.@params.width);
                if (width > stash.@params.width)
                    //memset(dst+stash->params.width, 0, width - stash->params.width);
                    Array.Clear(dst, stash.@params.width, width - stash.@params.width);
            }
            if (height > stash.@params.height)
                //memset(&data[stash.@params.height * width], 0, (height - stash.@params.height) * width);
                Array.Clear(data, stash.@params.height * width, (height - stash.@params.height) * width);

            //free(stash->texData);
            stash.texData = data;

            // Increase atlas size
            fons__atlasExpand(ref stash.atlas, width, height);

            // Add axisting data as dirty.
            for (i = 0; i < stash.atlas.nnodes; i++)
                maxy = fons__maxi(maxy, stash.atlas.nodes[i].y);
            stash.dirtyRect[0] = 0;
            stash.dirtyRect[1] = 0;
            stash.dirtyRect[2] = stash.@params.width;
            stash.dirtyRect[3] = maxy;

            stash.@params.width = width;
            stash.@params.height = height;
            stash.itw = 1.0f / stash.@params.width;
            stash.ith = 1.0f / stash.@params.height;

            return 1;
        }

        #endregion Private-methods

        #region Extensions
        public static bool fonsFontCreated(FONScontext stash, string internalFontName)
        {
            int i;
            for (i = 0; i < stash.nfonts; i++)
            {
                if (String.Compare(stash.fonts[i].name, internalFontName, StringComparison.OrdinalIgnoreCase) == 0)
                    return true;
            }
            return false;
        }
        public static bool fonsFontCreated(FONScontext stash, int idFont)
        {
            bool r = idFont >= 0 && idFont <= stash.nfonts - 1;
            return r;
        }
   
        #endregion
    }

    #region Auxiliary-classes-structs

    public struct FONStextIter
    {
        public float x, y, nextx, nexty, scale, spacing;
        public uint codepoint;
        public short isize, iblur;
        public FONSfont font;
        public int prevGlyphIndex;
        public byte[] str;
        public int iStr;
        public int iNext;
        public char end;
        public uint utf8state;
        public int bitmapOption;
    }

    public struct FONSquad
    {
        public float x0, y0, s0, t0;
        public float x1, y1, s1, t1;
    }

    public struct FONSparams
    {
        public int width, height;
        public FONSflags flags;
        public object userPtr;
        public RenderCreateHandler renderCreate;
        public RenderResizeHandler renderResize;
        public RenderUpdateHandler renderUpdate;
        public RenderDrawHandler renderDraw;
        public RenderDeleteHandler renderDelete;
    }

    public struct FONSttFontImpl
    {
        public stbtt_fontinfo font;
    }

    public class FONSglyph
    {
        public uint codepoint;
        public int index;
        public int next;
        public short size, blur;
        public short x0, y0, x1, y1;
        public short xadv, xoff, yoff;

        public FONSglyph()
        {
        }

        public FONSglyph(FONSglyph master)
        {
            codepoint = master.codepoint;
            index = master.index;
            next = master.next;
            size = master.size;
            blur = master.blur;
            x0 = master.x0;
            y0 = master.y0;
            x1 = master.x1;
            y1 = master.y1;
            xadv = master.xadv;
            xoff = master.xoff;
            yoff = master.yoff;
        }
    }

    public class FONSfont
    {
        public FONSttFontImpl font;
        // char name[64];
        public string name;
        // unsigned char* data;
        public byte[] data;
        public int dataSize;
        // unsigned char freeData;
        public byte freeData;
        public float ascender;
        public float descender;
        public float lineh;
        public FONSglyph[] glyphs;
        public int cglyphs;
        public int nglyphs;
        // int lut[FONS_HASH_LUT_SIZE];
        public int[] lut;

        public int[] fallbacks;
        //[FONS_MAX_FALLBACKS];
        public int nfallbacks;

        public FONSfont()
        {
            lut = new int[FontStash.FONS_HASH_LUT_SIZE];
            fallbacks = new int[FontStash.FONS_MAX_FALLBACKS];
        }
    }

    public class FONSstate
    {
        public int font;
        public FONSalign align;
        public float size;
        public uint color;
        public float blur;
        public float spacing;
    }

    public class FONSatlasNode
    {
        public short x, y, width;
    }

    public struct FONSatlas
    {
        public int width, height;
        public FONSatlasNode[] nodes;
        public int nnodes;
        public uint cnodes;
    }

    public class FONScontext
    {
        public FONSparams @params;
        public float itw, ith;
        public byte[] texData;
        //int[4];
        public int[] dirtyRect;
        // FONSfont** fonts;
        public FONSfont[] fonts;
        public FONSatlas atlas;
        public uint cfonts;
        public int nfonts;
        //[FONS_VERTEX_COUNT*2];
        public float[] verts;
        //[FONS_VERTEX_COUNT*2];
        public float[] tcoords;
        //[FONS_VERTEX_COUNT];
        public uint[] colors;
        public int nverts;
        public byte[] scratch;
        public int nscratch;
        //[FONS_MAX_STATES];
        public FONSstate[] states;
        public int nstates;
        public HandleErrorHandler handleError;
        public object errorUptr;

        public FONScontext()
        {
            dirtyRect = new int[4];
            verts = new float[FontStash.FONS_VERTEX_COUNT * 2];
            tcoords = new float[FontStash.FONS_VERTEX_COUNT * 2];
            colors = new uint[FontStash.FONS_VERTEX_COUNT];
            states = new FONSstate[FontStash.FONS_MAX_STATES];
            for (int cont = 0; cont < FontStash.FONS_MAX_STATES; cont++)
                states[cont] = new FONSstate();
        }
    }
    #endregion Auxiliary-classes-structs
}
