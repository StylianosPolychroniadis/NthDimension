using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using NthDimension.Algebra;

namespace NthDimension.Rasterizer.GL1
{
    public partial class RendererBase
    {
        #region -- Required for Gwen.Net Ui

        internal Point m_RenderOffset;
        internal Rectangle m_ClipRegion;


        #region Font

        /// <summary>
        /// Loads the specified font.
        /// </summary>
        /// <param name="font">Font to load.</param>
        /// <returns>True if succeeded.</returns>
        public virtual bool LoadFont(object font)
        {
            return false;
        }

        /// <summary>
        /// Frees the specified font.
        /// </summary>
        /// <param name="font">Font to free.</param>
        public virtual void FreeFont(object font)
        {
        }

        /// <summary>
        /// Returns dimensions of the text using specified font.
        /// </summary>
        /// <param name="font">Font to use.</param>
        /// <param name="text">Text to measure.</param>
        /// <returns>Width and height of the rendered text.</returns>
        public virtual Point MeasureText(object font, String text)
        {
            int fontSize = (int)((Font) font).Size;
            Point p = new Point((int) (fontSize*Scale*text.Length*0.4f), (int) (fontSize*Scale));

            return p;
        }

        #endregion


        /// <summary>
        /// Adds a rectangle to the clipping region.
        /// </summary>
        /// <param name="rect">Rectangle to add.</param>
        public void AddClipRegion(Rectangle rect)
        {

            rect.X = m_RenderOffset.X;
            rect.Y = m_RenderOffset.Y;

            Rectangle r = rect;
            if (rect.X < m_ClipRegion.X)
            {
                r.Width -= (m_ClipRegion.X - r.X);
                r.X = m_ClipRegion.X;
            }

            if (rect.Y < m_ClipRegion.Y)
            {
                r.Height -= (m_ClipRegion.Y - r.Y);
                r.Y = m_ClipRegion.Y;
            }

            if (rect.Right > m_ClipRegion.Right)
            {
                r.Width = m_ClipRegion.Right - r.X;
            }

            if (rect.Bottom > m_ClipRegion.Bottom)
            {
                r.Height = m_ClipRegion.Bottom - r.Y;
            }

            m_ClipRegion = r;
        }
        /// <summary>
        /// Adds a point to the render offset.
        /// </summary>
        /// <param name="offset">Point to add.</param>
        public void AddRenderOffset(Rectangle offset)
        {
            m_RenderOffset = new Point(m_RenderOffset.X + offset.X, m_RenderOffset.Y + offset.Y);
        }

        /// <summary>
        /// Cache to texture provider.
        /// </summary>
        public dynamic CacheToTexture
        {
            get { return null; }
        }

        /// <summary>
        /// Draws textured rectangle.
        /// </summary>
        /// <param name="t">Texture to use (ie GuiTexture).</param>
        /// <param name="targetRect">Rectangle bounds.</param>
        /// <param name="u1">Texture coordinate u1.</param>
        /// <param name="v1">Texture coordinate v1.</param>
        /// <param name="u2">Texture coordinate u2.</param>
        /// <param name="v2">Texture coordinate v2.</param>
        public virtual void DrawGuiRect(object texture, Rectangle targetRect, float u1 = 0, float v1 = 0, float u2 = 1, float v2 = 1)
        { }

        /// <summary>
        /// Draws "missing image" default texture.
        /// </summary>
        /// <param name="rect">Target rectangle.</param>
        public virtual void DrawMissingImage(Rectangle rect)
        {
            //DrawColor = Color.FromArgb(255, rnd.Next(0,255), rnd.Next(0,255), rnd.Next(0, 255));
            //DrawColor = Color.Red;
            DrawFilledRect(rect, Color.Red);
        }
        
        /// <summary>
        /// Draws a single pixel. Very slow, do not use. :P
        /// </summary>
        /// <param name="x">X.</param>
        /// <param name="y">Y.</param>
        public virtual void DrawPixel(int x, int y, Color color)
        {
            // [omeg] amazing ;)
            DrawFilledRect(new Rectangle(x, y, 1, 1), color);
        }
        
        /// <summary>
        /// Draws a round-corner rectangle.
        /// </summary>
        /// <param name="rect">Target rectangle.</param>
        /// <param name="slight"></param>
        public virtual void DrawShavedCornerRect(Rectangle rect, Color color, bool slight = false)
        {
            // Draw INSIDE the w/h.
            rect.Width -= 1;
            rect.Height -= 1;

            if (slight)
            {
                DrawFilledRect(new Rectangle(rect.X + 1, rect.Y, rect.Width - 1, 1), color);
                DrawFilledRect(new Rectangle(rect.X + 1, rect.Y + rect.Height, rect.Width - 1, 1), color);

                DrawFilledRect(new Rectangle(rect.X, rect.Y + 1, 1, rect.Height - 1), color);
                DrawFilledRect(new Rectangle(rect.X + rect.Width, rect.Y + 1, 1, rect.Height - 1), color);
                return;
            }

            DrawPixel(rect.X + 1, rect.Y + 1, color);
            DrawPixel(rect.X + rect.Width - 1, rect.Y + 1, color);

            DrawPixel(rect.X + 1, rect.Y + rect.Height - 1, color);
            DrawPixel(rect.X + rect.Width - 1, rect.Y + rect.Height - 1, color);

            DrawFilledRect(new Rectangle(rect.X + 2, rect.Y, rect.Width - 3, 1), color);
            DrawFilledRect(new Rectangle(rect.X + 2, rect.Y + rect.Height, rect.Width - 3, 1), color);

            DrawFilledRect(new Rectangle(rect.X, rect.Y + 2, 1, rect.Height - 3), color);
            DrawFilledRect(new Rectangle(rect.X + rect.Width, rect.Y + 2, 1, rect.Height - 3), color);
        }

        /// <summary>
        /// Renders text using specified font.
        /// </summary>
        /// <param name="font">Font to use.</param>
        /// <param name="position">Top-left corner of the text.</param>
        /// <param name="text">Text to render.</param>
        public virtual void DrawText(object font, Point position, String text)
        {
            float size = ((Font)font).Size * Scale;

            for (int i = 0; i < text.Length; i++)
            {
                char chr = text[i];

                if (chr == ' ')
                    continue;

                Rectangle r = Util.FloatRect(position.X + i * size * 0.4f, position.Y, size * 0.4f - 1, size);

                /*
                    This isn't important, it's just me messing around changing the
                    shape of the rect based on the letter.. just for fun.
                */
                if (chr == 'l' || chr == 'i' || chr == '!' || chr == 't')
                {
                    r.Width = 1;
                }
                else if (chr >= 'a' && chr <= 'z')
                {
                    r.Y = (int)(r.Y + size * 0.5f);
                    r.Height = (int)(r.Height - size * 0.4f);
                }
                else if (chr == '.' || chr == ',')
                {
                    r.X += 2;
                    r.Y += r.Height - 2;
                    r.Width = 2;
                    r.Height = 2;
                }
                else if (chr == '\'' || chr == '`' || chr == '"')
                {
                    r.X += 3;
                    r.Width = 2;
                    r.Height = 2;
                }

                //if (chr == 'o' || chr == 'O' || chr == '0')
                //    DrawLinedRect(r);
                //else
                //    DrawFilledRect(r);
            }
        }

        public virtual void DrawUserInterface(object uiCanvas, ref Matrix4 uiProjection)
        {
            //GL.Flush(); // Is this really required here? Does it NOT corrupt OpenGL stack???
        }

        /// <summary>
        /// Starts Clipping
        /// </summary>
        public virtual  void BeginClip()
        {
            // ToDo:: Am I missing EndClip() ??? Most likelly yes!!!
            // Todo:: See If can be switched to non-
            

#if DEBUG
            //throw new NotImplementedException();
#endif
        }

        /// <summary>
        /// Stops clipping.
        /// </summary>
        public virtual void EndClip()
        {
            // ToDo:: Am I missing BeginClip() ??? Most likelly yes!!!
            // Todo:: See If can be switched to non-
#if DEBUG
            //throw new NotImplementedException();
#endif
        }

        /// <summary>
        /// Gets or sets the current drawing color.
        /// </summary>
        public Color DrawColor { get; set; }

        // TODO :: Drop as useless

        // used for 2D Graphics
        public float Scale { get; set; }

        /// <summary>
        /// Rendering offset. No need to touch it usually.
        /// </summary>
        public Point RenderOffset
        {
            get { return m_RenderOffset; }
            set { m_RenderOffset = value; }
        }

        /// <summary>
        /// Clipping rectangle.
        /// </summary>
        public Rectangle ClipRegion
        {
            get { return m_ClipRegion; }
            set { m_ClipRegion = value; }
        }

        /// <summary>
        /// Indicates whether the clip region is visible.
        /// </summary>
        public bool ClipRegionVisible
        {
            get
            {
                if (m_ClipRegion.Width <= 0 || m_ClipRegion.Height <= 0)
                    return false;

                return true;
            }
        }

        public string RendererApi
        {
            get { return "No Device"; }
        }

        #endregion
    }
}
