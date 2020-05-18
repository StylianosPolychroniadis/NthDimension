namespace NthDimension.Rasterizer.GL1
{
    using NthDimension.Graphics.Renderer;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public partial class RendererGL1x : RendererBase
    {
        public override void RasterPos2(int x, int y)
        {
            OpenTK.Graphics.OpenGL.GL.RasterPos2(x, y); GetError(true);
        }
        public override void RasterPos2d(double x, double y)
        {
            OpenTK.Graphics.OpenGL.GL.RasterPos2(x, y); GetError(true);
        }

        #region Text3D
        public override FontOutlineEntry CreateFontOutlineEntry(string faceName, int height, TextWeight weight, bool italic, bool underline, bool strikethrough,
                float deviation, float extrusion, FontOutlineFormat fontOutlineFormat, ref List<FontOutlineEntry> fontOutlineEntries)
        {
            this.MakeCurrent(); //  Make the OpenGL instance current.

            //  Create the font based on the face name.
            //var hFont = Win32.CreateFont(height, 0, 0, 0, Win32.FW_DONTCARE, 0, 0, 0, Win32.DEFAULT_CHARSET,
            var hFont = Win32.CreateFont(height, 0, 0, 0, (uint)weight, italic ? 1u : 0u, underline ? 1u : 0u, strikethrough? 1u : 0u, Win32.DEFAULT_CHARSET,
                Win32.OUT_OUTLINE_PRECIS, Win32.CLIP_DEFAULT_PRECIS, Win32.CLEARTYPE_QUALITY, Win32.VARIABLE_PITCH, faceName);

            //  Select the font handle.
            var hOldObject = Win32.SelectObject(this.DeviceContextHandle, hFont); //RendererGL.currentRenderingContext.GraphicsMode.Index.Value


            var listBase = this.GenLists(1);

            //  Create space for the glyph metrics.
            var glyphMetrics = new GLYPHMETRICSFLOAT[255];

            //  Create the font bitmaps.
            bool result = Win32.wglUseFontOutlines(this.DeviceContextHandle, 0, 255, (uint)listBase,
                deviation, extrusion, (int)fontOutlineFormat, glyphMetrics);

            this.GetError(true);

            //  Reselect the old font.
            Win32.SelectObject(this.DeviceContextHandle, hOldObject);

            //  Free the font.
            Win32.DeleteObject(hFont);

            //  Create the font bitmap entry.
            var foe = new FontOutlineEntry()
            {
                HDC = this.DeviceContextHandle,
                HRC = this.RenderContextHandle,
                FaceName = faceName,
                Height = height,
                ListBase = (uint)listBase,
                ListCount = 255,
                Deviation = deviation,
                Extrusion = extrusion,
                FontOutlineFormat = fontOutlineFormat,
                GlyphMetrics = glyphMetrics
            };

            //  Add the font bitmap entry to the internal list.
            fontOutlineEntries.Add(foe);

            return foe;
        }        
        #endregion

        #region Text 2D
        public override RendererBase.FontBitmapEntry CreateFontBitmapEntry(string faceName, int height, TextWeight weight, bool italic, bool underline, bool strikethrough, ref List<FontBitmapEntry> fontBitmapEntries)
        {
            this.MakeCurrent();

            //  Create the font based on the face name.
            //var hFont = Win32.CreateFont(height, 0, 0, 0, Win32.FW_DONTCARE, 0, 0, 0, Win32.DEFAULT_CHARSET,
            var hFont = Win32.CreateFont(height, 0, 0, 0, (uint)weight, italic ? 1u : 0u, underline ? 1u : 0u, strikethrough ? 1u : 0u, Win32.DEFAULT_CHARSET,
                Win32.OUT_OUTLINE_PRECIS, Win32.CLIP_DEFAULT_PRECIS, Win32.CLEARTYPE_QUALITY, Win32.VARIABLE_PITCH, faceName);

            //  Select the font handle.
            var hOldObject = Win32.SelectObject(this.DeviceContextHandle, hFont);

            ////  Create the list base.
            //DisplayList listBase = this.CreateDisplayList();
            //listBase.Generate(1);
            ////  Create the font bitmaps.
            //bool result = Win32.wglUseFontBitmaps(this.DeviceContextHandle, 0, 255, (uint)listBase.ListId);

            var listBase = this.GenLists(1);
            bool result = Win32.wglUseFontBitmaps(this.DeviceContextHandle, 0, 255, (uint)listBase);

            //  Reselect the old font.
            Win32.SelectObject(this.DeviceContextHandle, hOldObject);

            //  Free the font.
            Win32.DeleteObject(hFont);

            //  Create the font bitmap entry.
            FontBitmapEntry fbe = new FontBitmapEntry()
            {
                HDC = this.DeviceContextHandle,
                HRC = this.RenderContextHandle,
                FaceName = faceName,
                Height = height,
                //ListBase = (uint)listBase.ListId,
                ListBase = (uint)listBase,
                ListCount = 255
            };

            //  Add the font bitmap entry to the internal list.
            fontBitmapEntries.Add(fbe);

            return fbe;
        }
        #endregion

    }
}
