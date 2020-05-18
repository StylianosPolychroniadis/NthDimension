using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GLTerrain {
    public class Texture : IDisposable {
        public string Filename { get; private set; }
        public int ID { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public TextureUnit Unit { get; set; }
        public bool IsDynamic { get; set; }

        public int UnitNumber {
            get {
                string numString = Unit.ToString().Substring(7);

                int num = 0;
                if (Int32.TryParse(numString, out num)) {
                    return num;
                }

                return -1;
            }
        }

        private bool wrap = true;
        public bool Wrap {
            get {
                return wrap;
            }
            set {
                wrap = value;

                GL.PushAttrib(AttribMask.TextureBit);
                Bind();

                if (wrap) {
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Repeat);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                } else {
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                }

                GL.PopAttrib();
            }
        }

        private Bitmap image = null;

        public Texture(Bitmap bitmap, bool dynamic, bool mipMap) : this(bitmap, dynamic, mipMap, TextureUnit.Texture0) {
        }

        public Texture(Bitmap bitmap, bool dynamic, bool mipMap, TextureUnit unit) {
            ID = -1;
            Unit = unit;

            if (dynamic) {
                image = bitmap.Clone(
                    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            } else {
                image = bitmap;
            }

            Width = bitmap.Width;
            Height = bitmap.Height;

            CreateTexture(image, mipMap);

            if (!dynamic) {
                image = null;
            }

            IsDynamic = dynamic;
            Filename = "";
        }

        public Texture(string filename, bool dynamic) : this (filename, dynamic, TextureUnit.Texture0) {
        }

        public Texture(string filename, bool dynamic, TextureUnit unit) {
            ID = -1;
            Unit = unit;

            image = new Bitmap(filename);
            Width = image.Width;
            Height = image.Height;

            CreateTexture(image, true);

            if (!dynamic) {
                image.Dispose();
                image = null;
            }

            IsDynamic = dynamic;
            Filename = filename;
        }

        private void CreateTexture(Bitmap bitmap, bool mipMap) {
            if (ID > -1) {
                GL.DeleteTexture(ID);
            }

            BitmapData data = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.PushAttrib(AttribMask.TextureBit);
            GL.ActiveTexture(Unit);

            ID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, ID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height, 0,
                          OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            // create mipmaps
            if (mipMap) {
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

                GL.TexParameter(TextureTarget.Texture2D,
                                TextureParameterName.TextureMinFilter,
                                (int)TextureMinFilter.LinearMipmapLinear);
            } else {
                GL.TexParameter(TextureTarget.Texture2D,
                                TextureParameterName.TextureMinFilter,
                                (int)TextureMinFilter.Linear);
            }

            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureMagFilter,
                            (int)TextureMagFilter.Linear);

            GL.PopAttrib();

            bitmap.UnlockBits(data);
        }

        public void RefreshTexture() {
            if (IsDynamic && ID > -1) {
                BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), 
                    ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.PushAttrib(AttribMask.TextureBit);

                Bind();
                GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, image.Width, image.Height, 
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                GL.PopAttrib();

                image.UnlockBits(data);
            }
        }

        public Graphics GetImageContext() {
            if (IsDynamic && image != null) {
                return Graphics.FromImage(image);
            }
            return null;
        }

        public Bitmap GetImageData() {
            if (IsDynamic && image != null) {

                BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                    ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                Bind();
                GL.GetTexImage(TextureTarget.Texture2D, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                Unbind();

                image.UnlockBits(data);

                return image;
            } else if (image != null) {
                return null;
            }
            return null;
        }

        public void Bind() {
            GL.ActiveTexture(Unit);
            GL.BindTexture(TextureTarget.Texture2D, ID);
        }

        public void Unbind() {
            GL.ActiveTexture(Unit);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Dispose() {
            GL.DeleteTexture(ID);
            if (IsDynamic && image != null) {
                image.Dispose();
                image = null;
            }
        }
    }
}
