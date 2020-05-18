using System;

namespace NthDimension.Rasterizer.GL1
{

    public partial class RendererGL1x
    {
        public override void TexParameter(TextureTarget target, TextureParameterName pname, int param)
        {
            OpenTK.Graphics.OpenGL.GL.TexParameter(target.ToOpenGL(), pname.ToOpenGL(), param); GetError(true);
        }

        public override int GenTexture()
        {
            return OpenTK.Graphics.OpenGL.GL.GenTexture(); GetError(true);
        }
        public override void TexImage2D(TextureTarget target, int level, PixelInternalFormat internalformat, int width, int height, int border, PixelFormat format, PixelType type, IntPtr pixels)
        {
            OpenTK.Graphics.OpenGL.GL.TexImage2D(target.ToOpenGL(), level, internalformat.ToOpenGL(), width, height, border, format.ToOpenGL(), type.ToOpenGL(), pixels); GetError(true);
        }
        public override void TexSubImage2D(TextureTarget target, int level, int xoffset, int yoffset, int width, int height, PixelFormat format, PixelType type, IntPtr pixels)
        {
            OpenTK.Graphics.OpenGL.GL.TexSubImage2D(target.ToOpenGL(), level, xoffset, yoffset, width, height, format.ToOpenGL(), type.ToOpenGL(), pixels); GetError(true);
        }
        public override void GetTexImage(TextureTarget target, int level, PixelFormat format, PixelType type, IntPtr pixels)
        {
            OpenTK.Graphics.OpenGL.GL.GetTexImage(target.ToOpenGL(), level, format.ToOpenGL(), type.ToOpenGL(), pixels); GetError(true);
        }

        public override void GenerateMipmap(GenerateMipmapTarget target)
        {
            OpenTK.Graphics.OpenGL.GL.GenerateMipmap(target.ToOpenGL()); GetError(true);
        }

    }
}
