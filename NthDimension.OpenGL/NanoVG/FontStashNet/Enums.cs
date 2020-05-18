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

namespace NthDimension.Rasterizer.NanoVG.FontStashNet
{
    public enum FONSerrorCode
    {
        // Font atlas is full.
        FONS_ATLAS_FULL = 1,
        // Scratch memory used to render glyphs is full, requested size reported in 'val', you may need to bump up FONS_SCRATCH_BUF_SIZE.
        FONS_SCRATCH_FULL = 2,
        // Calls to fonsPushState has craeted too large stack, if you need deep state stack bump up FONS_MAX_STATES.
        FONS_STATES_OVERFLOW = 3,
        // Trying to pop too many states fonsPopState().
        FONS_STATES_UNDERFLOW = 4,
    }

    public enum FONSalign
    {
        // Horizontal align

        // Default
        FONS_ALIGN_LEFT = 1 << 0,
        FONS_ALIGN_CENTER = 1 << 1,
        FONS_ALIGN_RIGHT = 1 << 2,
        // Vertical align
        FONS_ALIGN_TOP = 1 << 3,
        FONS_ALIGN_MIDDLE = 1 << 4,
        FONS_ALIGN_BOTTOM = 1 << 5,
        // Default
        FONS_ALIGN_BASELINE = 1 << 6,
    }

    public enum FONSglyphBitmap
    {
        FONS_GLYPH_BITMAP_OPTIONAL = 1,
        FONS_GLYPH_BITMAP_REQUIRED = 2,
    }

    public enum FONSflags
    {
        FONS_ZERO_TOPLEFT = 1,
        FONS_ZERO_BOTTOMLEFT = 2,
    }

    public enum STBTT_PLATFORM_ID
    {
        // platformID
        STBTT_PLATFORM_ID_UNICODE = 0,
        STBTT_PLATFORM_ID_MAC = 1,
        STBTT_PLATFORM_ID_ISO = 2,
        STBTT_PLATFORM_ID_MICROSOFT = 3
    }

    public enum STBTT_PLATFORM_ID_MICROSOFT
    {
        // encodingID for STBTT_PLATFORM_ID_MICROSOFT
        STBTT_MS_EID_SYMBOL = 0,
        STBTT_MS_EID_UNICODE_BMP = 1,
        STBTT_MS_EID_SHIFTJIS = 2,
        STBTT_MS_EID_UNICODE_FULL = 10
    }

    public enum STBTT_vmove
    {
        STBTT_vmove = 1,
        STBTT_vline,
        STBTT_vcurve
    }
}
