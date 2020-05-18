using System.Collections.Generic;

namespace NthDimension.Rasterizer.GL1
{
    using NthDimension.Algebra.Compatibility;
    using System;

    public partial class RendererBase
    {
        /// <summary>
        /// The font outline format.
        /// </summary>
        public enum FontOutlineFormat
        {
            /// <summary>
            /// Render using lines.
            /// </summary>
            Lines = 0,

            /// <summary>
            /// Render using polygons.
            /// </summary>
            Polygons = 1
        }
        public enum TextWeight
        {
            DontCare = 0,
            Thin = 100,
            ExtraLight = 200,
            UltraLight = 200,
            Light = 300,
            Normal = 400,           // Normal Font
            Regular = 400,
            Medium = 500,
            SemiBold = 600,
            DemiBold = 600,
            Bold = 700,             // Bold Font
            ExtraBold = 800,
            UltraBold = 800,
            Heavy = 900,
            Black = 900
        }

        [Serializable] // Required by CodeDOM
        public class FontBitmapEntry        // 2D Text
        {
            public IntPtr HDC
            {
                get;
                set;
            }

            public IntPtr HRC
            {
                get;
                set;
            }

            public string FaceName
            {
                get;
                set;
            }

            public int Height
            {
                get;
                set;
            }

            public uint ListBase
            {
                get;
                set;
            }

            public uint ListCount
            {
                get;
                set;
            }

            public bool Bold { get; set; }
            public bool Italic { get; set; }
            public bool Strikeout { get; set; }
        }

        [Serializable] // Required by CodeDOM
        public class FontOutlineEntry       // 3D Text
        {
            /// <summary>
            /// Gets or sets the HDC.
            /// </summary>
            /// <value>
            /// The HDC.
            /// </value>
            public IntPtr HDC
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the HRC.
            /// </summary>
            /// <value>
            /// The HRC.
            /// </value>
            public IntPtr HRC
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the name of the face.
            /// </summary>
            /// <value>
            /// The name of the face.
            /// </value>
            public string FaceName
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the height.
            /// </summary>
            /// <value>
            /// The height.
            /// </value>
            public int Height
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the list base.
            /// </summary>
            /// <value>
            /// The list base.
            /// </value>
            public uint ListBase
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the list count.
            /// </summary>
            /// <value>
            /// The list count.
            /// </value>
            public uint ListCount
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the deviation.
            /// </summary>
            /// <value>
            /// The deviation.
            /// </value>
            public float Deviation
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the extrusion.
            /// </summary>
            /// <value>
            /// The extrusion.
            /// </value>
            public float Extrusion
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the font outline format.
            /// </summary>
            /// <value>
            /// The font outline format.
            /// </value>
            public FontOutlineFormat FontOutlineFormat
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the glyph metrics.
            /// </summary>
            /// <value>
            /// The glyph metrics.
            /// </value>
            public GLYPHMETRICSFLOAT[] GlyphMetrics
            {
                get; set;
            }

            public bool Bold { get; set; }
            public bool Italic { get; set; }
            public bool Strikeout { get; set; }
        }
        /// <summary>
        /// The GLYPHMETRICSFLOAT structure contains information about the placement and orientation of a glyph in a character cell.
        /// </summary>

        [Serializable] // Required by CodeDOM
        public struct GLYPHMETRICSFLOAT
        {
            /// <summary>
            /// Specifies the width of the smallest rectangle (the glyph's black box) that completely encloses the glyph..
            /// </summary>
            public float gmfBlackBoxX;
            /// <summary>
            /// Specifies the height of the smallest rectangle (the glyph's black box) that completely encloses the glyph.
            /// </summary>
            public float gmfBlackBoxY;
            /// <summary>
            /// Specifies the x and y coordinates of the upper-left corner of the smallest rectangle that completely encloses the glyph.
            /// </summary>
            public POINTFLOAT gmfptGlyphOrigin;
            /// <summary>
            /// Specifies the horizontal distance from the origin of the current character cell to the origin of the next character cell.
            /// </summary>
            public float gmfCellIncX;
            /// <summary>
            /// Specifies the vertical distance from the origin of the current character cell to the origin of the next character cell.
            /// </summary>
            public float gmfCellIncY;
        }

        //public virtual void DrawText(string facename, float fontsize ,float deviation, float extrusion, string text)
        //{

        //}
        //public virtual void DrawText(string faceName, float fontSize, float deviation, float extrusion, string text, out GLYPHMETRICSFLOAT[] glyphMetrics)
        //{
        //    glyphMetrics = null;
        //}

        //public virtual void DrawText2D(RendererBase renderer, int x, int y, float r, float g, float b, string faceName, float fontSize, string text)
        //{

        //}

        //public virtual void DrawText3D(RendererBase renderer, string faceName, float fontSize, float deviation, float extrusion, string text)
        //{

        //}

        public virtual void RasterPos2(int x, int y) { }
        public virtual void RasterPos2d(double x, double y) { }

        public abstract int GenLists(int n);

        /// <summary>
        /// 2D Fonts
        /// </summary>
        public abstract FontBitmapEntry CreateFontBitmapEntry(string faceName, int height, TextWeight weight, bool italic, bool underline, bool strikethrough, ref List<FontBitmapEntry> fontBitmapEntries);

        /// <summary>
        /// 3D Fonts
        /// </summary>
        public abstract FontOutlineEntry CreateFontOutlineEntry(string faceName, 
                                                                int height, 
                                                                TextWeight weight, 
                                                                bool italic, 
                                                                bool underline, 
                                                                bool strikethrough,
                                                                float deviation, 
                                                                float extrusion, 
                                                                FontOutlineFormat fontOutlineFormat, 
                                                                ref List<FontOutlineEntry> fontOutlineEntries);

        
    }
}
