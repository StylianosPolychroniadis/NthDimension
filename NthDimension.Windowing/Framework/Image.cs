using System;
using System.Runtime.InteropServices;

namespace NthDimension.Windowing.Framework
{
    /// <summary>
    ///     Contains GLFW Image data.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Image
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Image"/> struct.
        /// </summary>
        /// <param name="width">The width of the image in pixels.</param>
        /// <param name="height">The height of the image in pixels.</param>
        /// <param name="pixels"><see cref="IntPtr"/> pointing to the RGBA pixel data of the image.</param>
        public Image(int width, int height, byte* pixels)
        {
            Width = width;
            Height = height;
            Pixels = pixels;
        }

        /// <summary>
        /// The width, in pixels, of this <see cref="Image"/>.
        /// </summary>
        public int Width;

        /// <summary>
        /// The height, in pixels, of this <see cref="Image"/>.
        /// </summary>
        public int Height;

        /// <summary>
        /// A <see cref="byte"/> pointer pointing to the RGBA pixel data.
        /// </summary>
        public byte* Pixels;
    }
}
