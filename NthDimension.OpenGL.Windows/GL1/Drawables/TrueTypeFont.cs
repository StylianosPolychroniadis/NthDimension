using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;

using OpenTK.Graphics.OpenGL;
using NthDimension.Algebra;
using NthDimension.Graphics;

namespace NthDimension.Rasterizer.GL1
{
    [Serializable] // Required by CodeDOM
    public class TrueTypeFont : ITrueTypeFont // TODO:: ITransformable
    {
        public const int
            ALIGN_LEFT = 0,
            ALIGN_RIGHT = 1,
            ALIGN_CENTER = 2;
        /** Array that holds necessary information about the font characters */
        private TrueTypeFont.IntObject[] charArray = new TrueTypeFont.IntObject[256];

        /** Map of user defined font characters (Character <-> IntObject) */
        private Dictionary<char, TrueTypeFont.IntObject> customChars = new Dictionary<char, TrueTypeFont.IntObject>();

        /** Boolean flag on whether AntiAliasing is enabled or not */
        private bool antiAlias;

        /** Font's size */
        private int fontSize = 0;

        /** Font's height */
        private int fontHeight = 0;

        /** Texture used to cache the font 0-255 characters */
        private int fontTextureID;

        /** Default font texture width */
        private int textureWidth = 512;

        /** Default font texture height */
        private int textureHeight = 512;

        /** A reference to Java's AWT Font that we create our font texture from */
        private Font font;

        private int correctL = 9, correctR = 8;

        public Vector3 color;                                                               // TODO:: Switch to Color4

        private class IntObject
        {
            public IntObject()
            {
                this.storedX = this.storedY = this.width = this.height = 0;
            }

            /** Character's width */
            public int width;

            /** Character's height */
            public int height;

            /** Character's stored x position */
            public int storedX;

            /** Character's stored y position */
            public int storedY;
        }

        public TrueTypeFont(Font font, bool antiAlias, char[] additionalChars)
        {
            this.font = font;
            this.fontSize = (int)font.Size + 3;
            this.antiAlias = antiAlias;

            color = new Vector3(1, 1, 1);

            createSet(additionalChars);

            fontHeight -= 1;
            if (fontHeight <= 0) fontHeight = 1;
        }

        public TrueTypeFont(Font font, bool antiAlias) :
            this(font, antiAlias, null)
        {
        }

        public void setCorrection(bool on)
        {
            if (on)
            {
                correctL = 2;
                correctR = 1;
            }
            else
            {
                correctL = 0;
                correctR = 0;
            }
        }
        private Bitmap getFontImage(char ch)
        {
            // Create a temporary image to extract the character's size
            Bitmap tempfontImage = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(tempfontImage);
            if (antiAlias == true)
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
            }
            SizeF dims = g.MeasureString(new string(new char[] { ch }), font);

            if (ch == ' ')
            {
                dims = g.MeasureString("l", font);
                //SizeF dims2 = g.MeasureString("aa", font);
                //dims.Width -= dims2.Width;
            }

            int charwidth = (int)dims.Width + 2;

            if (charwidth <= 0)
            {
                charwidth = 7;
            }
            int charheight = (int)dims.Height + 3;
            if (charheight <= 0)
            {
                charheight = fontSize;
            }

            // Create another image holding the character we are creating
            Bitmap fontImage = new Bitmap(charwidth, charheight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            System.Drawing.Graphics gt = System.Drawing.Graphics.FromImage(fontImage);
            if (antiAlias == true)
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
            }

            int charx = 3;
            int chary = 1;
            gt.DrawString(new string(new char[] { ch }), font, new SolidBrush(Color.White), new PointF(charx, chary));

            return fontImage;

        }

        private void createSet(char[] customCharsArray)
        {
            // If there are custom chars then I expand the font texture twice
            if (customCharsArray != null && customCharsArray.Length > 0)
            {
                textureWidth *= 2;
            }

            // In any case this should be done in other way. Texture with size 512x512
            // can maintain only 256 characters with resolution of 32x32. The texture
            // size should be calculated dynamicaly by looking at character sizes.

            try
            {

                Bitmap imgTemp = new Bitmap(textureWidth, textureHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(imgTemp);

                //g.FillRectangle(new SolidBrush(Color.Yellow), 0, 0, textureWidth, textureHeight);

                int rowHeight = 0;
                int positionX = 0;
                int positionY = 0;

                int customCharsLength = (customCharsArray != null) ? customCharsArray.Length : 0;

                for (int i = 0; i < 256 + customCharsLength; i++)
                {

                    // get 0-255 characters and then custom characters
                    char ch = (i < 256) ? (char)i : customCharsArray[i - 256];

                    Bitmap fontImage = getFontImage(ch);

                    TrueTypeFont.IntObject newIntObject = new TrueTypeFont.IntObject();

                    newIntObject.width = fontImage.Width;
                    newIntObject.height = fontImage.Height;

                    if (positionX + newIntObject.width >= textureWidth)
                    {
                        positionX = 0;
                        positionY += rowHeight;
                        rowHeight = 0;
                    }

                    newIntObject.storedX = positionX;
                    newIntObject.storedY = positionY;

                    if (newIntObject.height > fontHeight)
                    {
                        fontHeight = newIntObject.height;
                    }

                    if (newIntObject.height > rowHeight)
                    {
                        rowHeight = newIntObject.height;
                    }

                    // Draw it here
                    g.DrawImage(fontImage, positionX, positionY);

                    positionX += newIntObject.width;

                    if (i < 256)
                    { // standard characters
                        charArray[i] = newIntObject;
                    }
                    else
                    { // custom characters
                        customChars[ch] = newIntObject;
                    }

                    fontImage = null;
                }

                fontTextureID = loadImage(imgTemp);

            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to create font.");
                Console.WriteLine(e.StackTrace);
            }
        }

        private void drawQuad(float drawX, float drawY, float drawX2, float drawY2,
                float srcX, float srcY, float srcX2, float srcY2)
        {
            float DrawWidth = drawX2 - drawX;
            float DrawHeight = drawY2 - drawY;
            float TextureSrcX = srcX / textureWidth;
            float TextureSrcY = srcY / textureHeight;
            float SrcWidth = srcX2 - srcX;
            float SrcHeight = srcY2 - srcY;
            float RenderWidth = (SrcWidth / textureWidth);
            float RenderHeight = (SrcHeight / textureHeight);

            GL.Color3(color.X, color.Y, color.Z);
            GL.TexCoord2(TextureSrcX, TextureSrcY);
            GL.Vertex2(drawX, drawY);
            GL.TexCoord2(TextureSrcX, TextureSrcY + RenderHeight);
            GL.Vertex2(drawX, drawY + DrawHeight);
            GL.TexCoord2(TextureSrcX + RenderWidth, TextureSrcY + RenderHeight);
            GL.Vertex2(drawX + DrawWidth, drawY + DrawHeight);
            GL.TexCoord2(TextureSrcX + RenderWidth, TextureSrcY);
            GL.Vertex2(drawX + DrawWidth, drawY);
        }

        public int getWidth(string whatchars)
        {
            int totalwidth = 0;
            TrueTypeFont.IntObject intObject = null;
            int currentChar = 0;
            for (int i = 0; i < whatchars.Length; i++)
            {
                currentChar = whatchars[i];
                if (currentChar < 256)
                {
                    intObject = charArray[currentChar];
                }
                else
                {
                    intObject = customChars[(char)currentChar];
                }

                if (intObject != null)
                    totalwidth += intObject.width;
            }
            return totalwidth;
        }

        public int getHeight()
        {
            return fontHeight;
        }


        public int getHeight(string HeightString)
        {
            return fontHeight;
        }

        public int getLineHeight()
        {
            return fontHeight;
        }

        public void drawString(float x, float y,
                string whatchars, float scaleX, float scaleY)
        {
            drawString(x, y, whatchars, 0, whatchars.Length - 1, scaleX, scaleY, ALIGN_LEFT);
        }
        public void drawString(float x, float y,
                string whatchars, float scaleX, float scaleY, int format)
        {
            drawString(x, y, whatchars, 0, whatchars.Length - 1, scaleX, scaleY, format);
        }


        public void drawString(float x, float y,
                string whatchars, int startIndex, int endIndex,
                float scaleX, float scaleY,
                int format
                )
        {

            TrueTypeFont.IntObject intObject = null;
            int charCurrent;


            int totalwidth = 0;
            int i = startIndex, d = 1, c = correctL;
            float startY = 0;



            switch (format)
            {
                case ALIGN_RIGHT:
                    d = -1;
                    c = correctR;

                    while (i < endIndex)
                    {
                        if (whatchars[i] == '\n') startY += fontHeight;
                        i++;
                    }
                    break;
                case ALIGN_CENTER:
                    for (int l = startIndex; l <= endIndex; l++)
                    {
                        charCurrent = whatchars[l];
                        if (charCurrent == '\n') break;
                        if (charCurrent < 256)
                        {
                            intObject = charArray[charCurrent];
                        }
                        else
                        {
                            intObject = customChars[(char)charCurrent];
                        }
                        totalwidth += intObject.width - correctL;
                    }
                    totalwidth /= -2;
                    break;
                case ALIGN_LEFT:
                default:
                    d = 1;
                    c = correctL;
                    break;
            }

            GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.Texture2D);
            GL.BindTexture(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, fontTextureID);
            GL.Begin(OpenTK.Graphics.OpenGL.BeginMode.Quads);

            while (i >= startIndex && i <= endIndex)
            {

                charCurrent = whatchars[i];
                if (charCurrent < 256)
                {
                    intObject = charArray[charCurrent];
                }
                else
                {
                    intObject = customChars[(char)charCurrent];
                }

                if (intObject != null)
                {
                    if (d < 0) totalwidth += (intObject.width - c) * d;
                    if (charCurrent == '\n')
                    {
                        startY += fontHeight * d;
                        totalwidth = 0;
                        if (format == ALIGN_CENTER)
                        {
                            for (int l = i + 1; l <= endIndex; l++)
                            {
                                charCurrent = whatchars[l];
                                if (charCurrent == '\n') break;
                                if (charCurrent < 256)
                                {
                                    intObject = charArray[charCurrent];
                                }
                                else
                                {
                                    intObject = customChars[(char)charCurrent];
                                }
                                totalwidth += intObject.width - correctL;
                            }
                            totalwidth /= -2;
                        }
                        //if center get next lines total width/2;
                    }
                    else
                    {
                        drawQuad((totalwidth + intObject.width) * scaleX + x, startY * scaleY + y,
                            totalwidth * scaleX + x,
                            (startY + intObject.height) * scaleY + y, intObject.storedX + intObject.width,
                            intObject.storedY, intObject.storedX,
                            intObject.storedY + intObject.height);
                        if (d > 0) totalwidth += (intObject.width - c) * d;
                    }
                    i += d;

                }
            }
            GL.End();
            GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.Texture2D);
        }

        public static int loadImage(Bitmap bufferedImage)
        {
            try
            {
                short width = (short)bufferedImage.Width;
                short height = (short)bufferedImage.Height;
                //textureLoader.bpp = bufferedImage.getColorModel().hasAlpha() ? (byte)32 : (byte)24;
                int bpp = bufferedImage.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppArgb ? 32 : 24;

                BitmapData bData = bufferedImage.LockBits(new Rectangle(new Point(), bufferedImage.Size),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                int byteCount = bData.Stride * bufferedImage.Height;
                byte[] byteI = new byte[byteCount];
                Marshal.Copy(bData.Scan0, byteI, 0, byteCount);
                bufferedImage.UnlockBits(bData);

                int result = GL.GenTexture();
                GL.BindTexture(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, result);

                GL.TexParameter(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, OpenTK.Graphics.OpenGL.TextureParameterName.TextureWrapS, (int)OpenTK.Graphics.OpenGL.TextureWrapMode.Clamp);
                GL.TexParameter(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, OpenTK.Graphics.OpenGL.TextureParameterName.TextureWrapT, (int)OpenTK.Graphics.OpenGL.TextureWrapMode.Clamp);

                GL.TexParameter(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, OpenTK.Graphics.OpenGL.TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, OpenTK.Graphics.OpenGL.TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);

                GL.TexEnv(OpenTK.Graphics.OpenGL.TextureEnvTarget.TextureEnv, OpenTK.Graphics.OpenGL.TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Modulate);

                GL.TexParameter(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, OpenTK.Graphics.OpenGL.TextureParameterName.GenerateMipmap, 1);

                GL.TexImage2D(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, 0, OpenTK.Graphics.OpenGL.PixelInternalFormat.Rgba, width, height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, OpenTK.Graphics.OpenGL.PixelType.UnsignedByte, byteI);
                return result;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                Environment.Exit(-1);
            }

            return -1;
        }
        public static bool isSupported(string fontname)
        {
            FontFamily[] font = getFonts();
            for (int i = font.Length - 1; i >= 0; i--)
            {
                if (string.Equals(font[i].Name, fontname, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            }
            return false;
        }

        public static FontFamily[] getFonts()
        {
            return (new InstalledFontCollection()).Families;
        }

        public static byte[] intToByteArray(int value)
        {
            return new byte[] {
	                (byte)(value >> 24),
	                (byte)(value >> 16),
	                (byte)(value >> 8),
	                (byte)value};
        }

        public void destroy()
        {
            GL.BindTexture(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, 0);
            GL.DeleteTexture(fontTextureID);
        }
    }
}
