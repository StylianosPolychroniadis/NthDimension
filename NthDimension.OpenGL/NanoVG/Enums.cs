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

namespace NthDimension.Rasterizer.NanoVG
{
    public enum NVGcommands
    {
        NVG_MOVETO = 0,
        NVG_LINETO = 1,
        NVG_BEZIERTO = 2,
        NVG_CLOSE = 3,
        NVG_WINDING = 4,
    }

    public enum NVGpointFlags
    {
        NVG_PT_CORNER = 0x01,
        NVG_PT_LEFT = 0x02,
        NVG_PT_BEVEL = 0x04,
        NVG_PR_INNERBEVEL = 0x08,
    }

    public enum NVGimageFlags
    {        
        NVG_IMAGE_GENERATE_MIPMAPS      = 1 << 0,   // Generate mipmaps during creation of the image.    
        NVG_IMAGE_REPEATX               = 1 << 1,   // Repeat image in X direction.        
        NVG_IMAGE_REPEATY               = 1 << 2,   // Repeat image in Y direction.     
        NVG_IMAGE_FLIPY                 = 1 << 3,   // Flips (inverses) image in Y direction when rendered.
        NVG_IMAGE_PREMULTIPLIED         = 1 << 4,   // Image data has premultiplied alpha.
        NVG_IMAGE_NEAREST               = 1 << 5,	// Image interpolation is Nearest instead Linear
    }

    public enum NVGtexture
    {
        NVG_TEXTURE_ALPHA = 0x01,
        NVG_TEXTURE_RGBA = 0x02,
    }

    public enum NVGcompositeOperation
    {
        NVG_SOURCE_OVER,
        NVG_SOURCE_IN,
        NVG_SOURCE_OUT,
        NVG_ATOP,
        NVG_DESTINATION_OVER,
        NVG_DESTINATION_IN,
        NVG_DESTINATION_OUT,
        NVG_DESTINATION_ATOP,
        NVG_LIGHTER,
        NVG_COPY,
        NVG_XOR,
    }

    public enum NVGblendFactor
    {
        NVG_ZERO = 1 << 0,
        NVG_ONE = 1 << 1,
        NVG_SRC_COLOR = 1 << 2,
        NVG_ONE_MINUS_SRC_COLOR = 1 << 3,
        NVG_DST_COLOR = 1 << 4,
        NVG_ONE_MINUS_DST_COLOR = 1 << 5,
        NVG_SRC_ALPHA = 1 << 6,
        NVG_ONE_MINUS_SRC_ALPHA = 1 << 7,
        NVG_DST_ALPHA = 1 << 8,
        NVG_ONE_MINUS_DST_ALPHA = 1 << 9,
        NVG_SRC_ALPHA_SATURATE = 1 << 10,
    }

    public enum NVGlineCap
    {
        NVG_BUTT,
        NVG_ROUND,
        NVG_SQUARE,
        NVG_BEVEL,
        NVG_MITER,
    }

    public enum NVGalign
    {
        // Horizontal align
        NVG_ALIGN_LEFT = 1 << 0,
        // Default, align text horizontally to left.
        NVG_ALIGN_CENTER = 1 << 1,
        // Align text horizontally to center.
        NVG_ALIGN_RIGHT = 1 << 2,
        // Align text horizontally to right.
        // Vertical align
        NVG_ALIGN_TOP = 1 << 3,
        // Align text vertically to top.
        NVG_ALIGN_MIDDLE = 1 << 4,
        // Align text vertically to middle.
        NVG_ALIGN_BOTTOM = 1 << 5,
        // Align text vertically to bottom.
        NVG_ALIGN_BASELINE = 1 << 6,
        // Default, align text vertically to baseline.
    }

    public enum NVGcreateFlags
    {
        // Flag indicating if geometry based anti-aliasing is used (may not be needed when using MSAA).
        NVG_ANTIALIAS = 1 << 0,
        // Flag indicating if strokes should be drawn using stencil buffer. The rendering will be a little
        // slower, but path overlaps (i.e. self-intersecting or sharp turns) will be drawn just once.
        NVG_STENCIL_STROKES = 1 << 1,
        // Flag indicating that additional debug checks are done.
        NVG_DEBUG = 1 << 2,
    }

    // These are additional flags on top of NVGimageFlags.
    public enum NVGimageFlagsGL
    {
        // Do not delete GL texture handle.
        NVG_IMAGE_NODELETE = 1 << 16,
    }

    public enum GLNVGuniformLoc
    {
        GLNVG_LOC_VIEWSIZE,
        GLNVG_LOC_TEX,
        GLNVG_LOC_FRAG,
        GLNVG_MAX_LOCS
    }

    public enum GLNVGcallType
    {
        GLNVG_NONE = 0,
        GLNVG_FILL,
        GLNVG_CONVEXFILL,
        GLNVG_STROKE,
        GLNVG_TRIANGLES,
    }

    public enum NVGsolidity
    {
        NVG_SOLID = 1,
        // CCW
        NVG_HOLE = 2,
        // CW
    }

    public enum NVGwinding
    {
        NVG_CCW = 1,
        // Winding for solid shapes
        NVG_CW = 2,
        // Winding for holes
    }

    public enum GLNVGshaderType
    {
        NSVG_SHADER_FILLGRAD,
        NSVG_SHADER_FILLIMG,
        NSVG_SHADER_SIMPLE,
        NSVG_SHADER_IMG
    }

    public enum GraphrenderStyle
    {
        GRAPH_RENDER_FPS,
        GRAPH_RENDER_MS,
        GRAPH_RENDER_PERCENT,
        GRAPH_MEGABYTES,
        GRAPH_MINMAX,
        GRAPH_DRAWCALLS,
        GRAPH_VERTICES,
        GRAPH_MATERIALS,
        GRAPH_TEXTURES,
        GRAPH_RATE_TEXTURES,
        GRAPH_GEOMETRIES,
        GRAPH_RATE_GEOMETRIES
    }

    public enum NVGcodepointType
    {
        NVG_SPACE,
        NVG_NEWLINE,
        NVG_CHAR,
    }
}
