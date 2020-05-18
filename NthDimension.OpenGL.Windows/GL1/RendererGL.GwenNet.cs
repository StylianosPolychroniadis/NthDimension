using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using OpenTK.Graphics.OpenGL;
using NthDimension.Graphics.Gui;
using NthDimension.Graphics.Gui.Control;
using NthDimension.Rasterizer.GL1.Gui;
using Font = NthDimension.Graphics.Gui.Font;

namespace NthDimension.Rasterizer.GL1
{
    
    /// <summary>
    /// Extensions to the main API in order to Draw 2d User Interface backed on a heavy modification of Gwen.Net
    /// </summary>
    public partial class RendererGL1x
    {

        #region Fields
        [Serializable] // Required by CodeDOM
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Vertex2
        {
            public short x, y;
            public float u, v;
            public byte r, g, b, a;
        }
        
        private Dictionary<int, TrueTypeFont> m_trueTypeFonts = new Dictionary<int, TrueTypeFont>(); 
        private readonly Dictionary<Tuple<String, Font>, TextRenderer> m_StringCache = new Dictionary<Tuple<string, Font>, TextRenderer>();

        private readonly System.Drawing.Graphics m_Graphics = System.Drawing.Graphics.FromImage(new Bitmap(1024, 1024, System.Drawing.Imaging.PixelFormat.Format32bppArgb));

        private Color m_clearColor = Color.Black;

        private const int MaxVerts = 1024;  // TODO:: Increase
        private int m_VertNum;
        //private Color this.DrawColor;
        private Vertex2[] m_Vertices = new Vertex2[MaxVerts];
        private int m_VertexSize;// = Marshal.SizeOf(m_Vertices[0]);

        //private readonly Dictionary<Tuple<String, UserInterface.Font>, TextRenderer> m_StringCache;
        private readonly System.Drawing.Graphics m_gdi;

        private bool m_RestoreRenderState = true;
        private bool m_ClipEnabled;
        private bool m_TextureEnabled;
        private StringFormat m_StringFormat;


        private static int m_LastTextureID;
        private bool m_WasBlendEnabled, m_WasTexture2DEnabled, m_WasDepthTestEnabled;
        private int m_PrevBlendSrc, m_PrevBlendDst, m_PrevAlphaFunc;
        private float m_PrevAlphaRef;
        private int m_DrawCallCount;

        //private static IBuffer<Vector3> m_pointsBuffer; 
        #endregion

        #region Draw
        public void DrawRect(Rectangle rect, float u1 = 0, float v1 = 0, float u2 = 1, float v2 = 1)
        {
            //m_Vertices = new Vertex2[MaxVerts];
            m_VertexSize = Marshal.SizeOf(m_Vertices[0]);

            if (m_VertNum + 4 >= MaxVerts)
            {
                Flush();
            }

            if (m_ClipEnabled)
            {
                // cpu scissors test

                if (rect.Y < ClipRegion.Y)
                {
                    int oldHeight = rect.Height;
                    int delta = ClipRegion.Y - rect.Y;
                    rect.Y = ClipRegion.Y;
                    rect.Height -= delta;

                    if (rect.Height <= 0)
                    {
                        return;
                    }

                    float dv = (float)delta / (float)oldHeight;

                    v1 += dv * (v2 - v1);
                }

                if ((rect.Y + rect.Height) > (ClipRegion.Y + ClipRegion.Height))
                {
                    int oldHeight = rect.Height;
                    int delta = (rect.Y + rect.Height) - (ClipRegion.Y + ClipRegion.Height);

                    rect.Height -= delta;

                    if (rect.Height <= 0)
                    {
                        return;
                    }

                    float dv = (float)delta / (float)oldHeight;

                    v2 -= dv * (v2 - v1);
                }

                if (rect.X < ClipRegion.X)
                {
                    int oldWidth = rect.Width;
                    int delta = ClipRegion.X - rect.X;
                    rect.X = ClipRegion.X;
                    rect.Width -= delta;

                    if (rect.Width <= 0)
                    {
                        return;
                    }

                    float du = (float)delta / (float)oldWidth;

                    u1 += du * (u2 - u1);
                }

                if ((rect.X + rect.Width) > (ClipRegion.X + ClipRegion.Width))
                {
                    int oldWidth = rect.Width;
                    int delta = (rect.X + rect.Width) - (ClipRegion.X + ClipRegion.Width);

                    rect.Width -= delta;

                    if (rect.Width <= 0)
                    {
                        return;
                    }

                    float du = (float)delta / (float)oldWidth;

                    u2 -= du * (u2 - u1);
                }
            }

            //int vertexIndex = m_VertNum;   // Changed Sep-20-15 when implementing 2d
            int vertexIndex = m_VertNum;

            if(vertexIndex >= MaxVerts)
                return;

            m_Vertices[vertexIndex].x = (short)rect.X;
            m_Vertices[vertexIndex].y = (short)rect.Y;
            m_Vertices[vertexIndex].u = u1;
            m_Vertices[vertexIndex].v = v1;
            m_Vertices[vertexIndex].r = this.DrawColor.R;
            m_Vertices[vertexIndex].g = this.DrawColor.G;
            m_Vertices[vertexIndex].b = this.DrawColor.B;
            m_Vertices[vertexIndex].a = this.DrawColor.A;

            vertexIndex++;
            m_Vertices[vertexIndex].x = (short)(rect.X + rect.Width);
            m_Vertices[vertexIndex].y = (short)rect.Y;
            m_Vertices[vertexIndex].u = u2;
            m_Vertices[vertexIndex].v = v1;
            m_Vertices[vertexIndex].r = this.DrawColor.R;
            m_Vertices[vertexIndex].g = this.DrawColor.G;
            m_Vertices[vertexIndex].b = this.DrawColor.B;
            m_Vertices[vertexIndex].a = this.DrawColor.A;

            vertexIndex++;
            m_Vertices[vertexIndex].x = (short)(rect.X + rect.Width);
            m_Vertices[vertexIndex].y = (short)(rect.Y + rect.Height);
            m_Vertices[vertexIndex].u = u2;
            m_Vertices[vertexIndex].v = v2;
            m_Vertices[vertexIndex].r = this.DrawColor.R;
            m_Vertices[vertexIndex].g = this.DrawColor.G;
            m_Vertices[vertexIndex].b = this.DrawColor.B;
            m_Vertices[vertexIndex].a = this.DrawColor.A;

            vertexIndex++;
            m_Vertices[vertexIndex].x = (short)rect.X;
            m_Vertices[vertexIndex].y = (short)(rect.Y + rect.Height);
            m_Vertices[vertexIndex].u = u1;
            m_Vertices[vertexIndex].v = v2;
            m_Vertices[vertexIndex].r = this.DrawColor.R;
            m_Vertices[vertexIndex].g = this.DrawColor.G;
            m_Vertices[vertexIndex].b = this.DrawColor.B;
            m_Vertices[vertexIndex].a = this.DrawColor.A;

            m_VertNum += 4;
        }
        public override void DrawGuiRect(object t, Rectangle rect, float u1 = 0, float v1 = 0, float u2 = 1, float v2 = 1)
        {
            // Missing image, not loaded properly?
            //if (null == (GuiTexture)t.RendererData)
            //{
            //    DrawMissingImage(rect);
            //    return;
            //}

            if (null == t)
            {
                
                DrawMissingImage(rect);
                return;
            }

            int tex = (int)((GuiTexture)t).RendererData;
            rect = Translate(rect);

            bool differentTexture = (tex != m_LastTextureID);
            if (!m_TextureEnabled || differentTexture)
            {
                Flush();
            }

            if (!m_TextureEnabled)
            {
                GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.Texture2D);
                m_TextureEnabled = true;
            }

            if (differentTexture)
            {
                GL.BindTexture(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, tex);
                m_LastTextureID = tex;
            }

            DrawRect(rect, u1, v1, u2, v2);
        }
        public override void DrawText(object font, Point position, string text)
        {

#if _LOG
            Debug.Print(String.Format("RenderText {0}", font.FaceName));
#endif

            // The DrawString(...) below will bind a new texture
            // so make sure everything is rendered!
            Flush();

            System.Drawing.Font sysFont = ((Font)font).RendererData as System.Drawing.Font;

            if (sysFont == null || Math.Abs(((Font)font).RealSize - ((Font)font).Size * Scale) > 2)
            {
                FreeFont(font);
                LoadFont(font);
                sysFont = ((Font)font).RendererData as System.Drawing.Font;
            }

            var key = new Tuple<String, Font>(text, ((Font)font));

            if (!m_StringCache.ContainsKey(key))
            {
                // not cached - create text renderer
#if _LOG
                Debug.Print(String.Format("RenderText: caching \"{0}\", {1}", text, font.FaceName));
#endif

                Point size = MeasureText(font, text);
                //TextRenderer tr = new TextRenderer(size.X, size.Y, this);
                TextRenderer tr = new TextRenderer(this, size.X, size.Y);
                tr.DrawString(text, sysFont, Brushes.White, Point.Empty, m_StringFormat); // renders string on the texture

                DrawGuiRect(tr.GuiTexture, new Rectangle(position.X, position.Y, tr.GuiTexture.Width, tr.GuiTexture.Height));

                m_StringCache[key] = tr;
            }
            else
            {
                TextRenderer tr = m_StringCache[key];
                DrawGuiRect(tr.GuiTexture, new Rectangle(position.X, position.Y, tr.GuiTexture.Width, tr.GuiTexture.Height));
            }
        }
        public override void DrawUserInterface(object uiCanvas, ref NthDimension.Algebra.Matrix4 uiProjection)
        {
            if(null == uiCanvas)
                return;

            //BeginFixedPipeline2D();

            m_ClipEnabled = true;
            m_TextureEnabled = true; 

            OpenTK.Matrix4 mo = uiProjection.ToOpenGL();

            GL.Flush(); // Really keep???
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadMatrix(ref mo);
            GL.FrontFace(FrontFaceDirection.Cw);
                ((Canvas)uiCanvas).RenderCanvas();
            GL.FrontFace(FrontFaceDirection.Ccw);
            GL.PopMatrix();
            GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Modelview);
            GL.PopMatrix();

            uiProjection = mo.ToSyscon();
            //EndFixedPipeline2D();
        }
        #endregion

        #region Translate
        private int TranslateX(int x)
        {
            int x1 = x + RenderOffset.X;
            return Util.Ceil(x1 * Scale);
        }
        private int TranslateY(int y)
        {
            int y1 = y + RenderOffset.Y;
            return Util.Ceil(y1 * Scale);
        }
        /// <summary>
        /// Translates a panel's local drawing coordinate into view space, taking offsets into account.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Translate(ref int x, ref int y)
        {
            x += RenderOffset.X;
            y += RenderOffset.Y;

            x = Util.Ceil(x * Scale);
            y = Util.Ceil(y * Scale);
        }
        /// <summary>
        /// Translates a panel's local drawing coordinate into view space, taking offsets into account.
        /// </summary>
        public Point Translate(Point p)
        {
            int x = p.X;
            int y = p.Y;
            Translate(ref x, ref y);
            return new Point(x, y);
        }
        /// <summary>
        /// Translates a panel's local drawing coordinate into view space, taking offsets into account.
        /// </summary>
        private Rectangle Translate(Rectangle rect)
        {
            return new Rectangle(TranslateX(rect.X), TranslateY(rect.Y), Util.Ceil(rect.Width * Scale), Util.Ceil(rect.Height * Scale));
        }
        #endregion

        #region Font calls
        public override bool LoadFont(object font)     // Hidden base.Call intented
        {
            Debug.Print(String.Format("LoadFont {0}", ((Font)font).FaceName));
            ((Font)font).RealSize = ((Font)font).Size * Scale;
            System.Drawing.Font sysFont = ((Font)font).RendererData as System.Drawing.Font;

            if (sysFont != null)
                sysFont.Dispose();

            // apaprently this can't fail @_@
            // "If you attempt to use a font that is not supported, or the font is not installed on the machine that is running the application, the Microsoft Sans Serif font will be substituted."
            sysFont = new System.Drawing.Font(((Font)font).FaceName, ((Font)font).Size);
            ((Font)font).RendererData = sysFont;

            //SYSCON.Graphics.Gui.Font f = new SYSCON.Graphics.Gui.Font(((Font)font).Name, (int)((Font)font).Size);
            //f.RendererData = font;


            return true;
        }
        public override void FreeFont(object font)
        {
            Debug.Print(String.Format("FreeFont {0}", ((Font)font).FaceName));
            if (((Font)font).RendererData == null)
                return;

            Debug.Print(String.Format("FreeFont {0} - actual free", ((Font)font).FaceName));
            System.Drawing.Font sysFont = ((Font)font).RendererData as System.Drawing.Font;
            if (sysFont == null)
                throw new InvalidOperationException("Freeing empty font");

            sysFont.Dispose();
            ((Font)font).RendererData = null;
        }
        public override Point MeasureText(object font, string text)
        {
            font = (Font)font;// as Font;
            //Debug.Print(String.Format("MeasureText '{0}'", text));
            System.Drawing.Font sysFont = ((Font)font).RendererData as System.Drawing.Font;

            if (sysFont == null || Math.Abs(((Font)font).RealSize - ((Font)font).Size * Scale) > 2)
            {
                FreeFont(font);
                LoadFont(font);
                sysFont = ((Font)font).RendererData as System.Drawing.Font;
            }

            var key = new Tuple<String, Font>(text, (Font)font);

            if (m_StringCache.ContainsKey(key))
            {
                var tex = m_StringCache[key].GuiTexture;
                return new Point(tex.Width, tex.Height);
            }

            SizeF size = m_Graphics.MeasureString(text, sysFont, Point.Empty, m_StringFormat); // TODO:: Supress warning generated when disposing

            return new Point((int)Math.Round(size.Width), (int)Math.Round(size.Height));
        }
        /// <summary>
        /// Clears the text rendering cache. Make sure to call this if cached strings size becomes too big (check TextCacheSize).
        /// </summary>
        public void FlushTextCache()
        {
            // todo: some auto-expiring cache? based on number of elements or age
            foreach (var textRenderer in m_StringCache.Values)
            {
                textRenderer.Dispose();
            }
            m_StringCache.Clear();
        }
        #endregion

        #region Texture calls
        internal static void LoadTextureInternal(GuiTexture t, Bitmap bmp)
        {
            // todo: convert to proper format
            System.Drawing.Imaging.PixelFormat lock_format = System.Drawing.Imaging.PixelFormat.Undefined;
            switch (bmp.PixelFormat)
            {
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    lock_format = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    lock_format = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
                    break;
                default:
                    t.Failed = true;
                    return;
            }

            int glTex;

            // Create the opengl texture
            GL.GenTextures(1, out glTex);
            GL.BindTexture(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, glTex);
            GL.TexParameter(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, OpenTK.Graphics.OpenGL.TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, OpenTK.Graphics.OpenGL.TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            // Sort out our GWEN texture
            t.RendererData = glTex;
            t.Width = bmp.Width;
            t.Height = bmp.Height;

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, lock_format);

            switch (lock_format)
            {
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    GL.TexImage2D(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, 0, OpenTK.Graphics.OpenGL.PixelInternalFormat.Rgba, t.Width, t.Height, 0, global::OpenTK.Graphics.OpenGL.PixelFormat.Bgra, OpenTK.Graphics.OpenGL.PixelType.UnsignedByte, data.Scan0);
                    break;
                default:
                    // invalid
                    break;
            }

            bmp.UnlockBits(data);

            m_LastTextureID = glTex;
        }
        public /* override */ void LoadTexture(GuiTexture t)
        {
            Bitmap bmp;
            try
            {
                //Note:: throws error
                bmp = new Bitmap(t.Name);
            }
            catch (Exception ebmp)
            {
                Debug.Print("Failed loading texture " +
                                                     t.Name +
                                                     "\n" +
                                                     ebmp.Message +
                                                     "\n" +
                                                     ebmp.StackTrace);
                t.Failed = true;
                return;
            }

            LoadTextureInternal(t, bmp);
            bmp.Dispose();
        }
        public /* override */ void LoadTextureStream(GuiTexture t, System.IO.Stream data)
        {
            Bitmap bmp;
            try
            {
                bmp = new Bitmap(data);
            }
            catch (Exception)
            {
                t.Failed = true;
                return;
            }

            LoadTextureInternal(t, bmp);
            bmp.Dispose();
        }
        public /* override */ void LoadTextureRaw(GuiTexture t, byte[] pixelData)
        {
            Bitmap bmp;
            try
            {
                unsafe
                {
                    fixed (byte* ptr = &pixelData[0])
                        bmp = new Bitmap(t.Width, t.Height, 4 * t.Width, System.Drawing.Imaging.PixelFormat.Format32bppArgb, (IntPtr)ptr);
                }
            }
            catch (Exception)
            {
                t.Failed = true;
                return;
            }

            int glTex;

            // Create the opengl texture
            GL.GenTextures(1, out glTex);
            GL.BindTexture(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, glTex);
            GL.TexParameter(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, OpenTK.Graphics.OpenGL.TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, OpenTK.Graphics.OpenGL.TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            // Sort out our GWEN texture
            t.RendererData = glTex;

            var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, 0, OpenTK.Graphics.OpenGL.PixelInternalFormat.Rgba, t.Width, t.Height, 0, global::OpenTK.Graphics.OpenGL.PixelFormat.Rgba, OpenTK.Graphics.OpenGL.PixelType.UnsignedByte, data.Scan0);

            m_LastTextureID = glTex;

            bmp.UnlockBits(data);
            bmp.Dispose();
        }
        public /* override */ void FreeTexture(GuiTexture t)
        {
            if (t.RendererData == null)
                return;
            int tex = (int)t.RendererData;
            if (tex == 0)
                return;
            GL.DeleteTextures(1, ref tex);
            t.RendererData = null;
        }
        public /* override */ unsafe Color PixelColor(GuiTexture texture, uint x, uint y, Color defaultColor)
        {
            if (texture.RendererData == null)
                return defaultColor;

            int tex = (int)texture.RendererData;
            if (tex == 0)
                return defaultColor;

            Color pixel;
            GL.BindTexture(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, tex);
            long offset = 4 * (x + y * texture.Width);
            byte[] data = new byte[4 * texture.Width * texture.Height];
            fixed (byte* ptr = &data[0])
            {
                GL.GetTexImage(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, 0, global::OpenTK.Graphics.OpenGL.PixelFormat.Rgba, OpenTK.Graphics.OpenGL.PixelType.UnsignedByte, (IntPtr)ptr);
                pixel = Color.FromArgb(data[offset + 3], data[offset + 0], data[offset + 1], data[offset + 2]);
            }
            // Retrieving the entire texture for a single pixel read
            // is kind of a waste - maybe cache this pointer in the texture
            // data and then release later on? It's never called during runtime
            // - only during initialization.
            return pixel;
        }
        #endregion
    }
}
