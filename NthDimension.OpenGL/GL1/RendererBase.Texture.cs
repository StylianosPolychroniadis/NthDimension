using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NthDimension.Rasterizer.GL1
{
    public partial class RendererBase
    {
        public virtual void TexParameter(TextureTarget target, TextureParameterName pname, int param) { }

        // As used by NthDimension.Rasterizer.GL1.Shader and terrain mesh
        public virtual int GenTexture() { return -1; }
        public virtual void TexImage2D(TextureTarget target, int level, PixelInternalFormat internalformat, int width, int height, int border, PixelFormat format, PixelType type, IntPtr pixels) { }
        public virtual void TexSubImage2D(TextureTarget target, int level, int xoffset, int yoffset, int width, int height, PixelFormat format, PixelType type, IntPtr pixels) { }
        public virtual void GetTexImage(TextureTarget target, int level, PixelFormat format, PixelType type, IntPtr pixels) { }

        public virtual void GenerateMipmap(GenerateMipmapTarget target) { }
    }
}
