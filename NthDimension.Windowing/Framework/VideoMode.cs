using System.Runtime.InteropServices;

namespace NthDimension.Windowing.Framework
{
    /// <summary>
    /// Replicated handle to a GLFW VideoMode.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VideoMode
    {
        /// <summary>
        /// The width, in screen coordinates, of the <see cref="VideoMode"/>.
        /// </summary>
        public int Width;

        /// <summary>
        /// The height, in screen coordinates, of the <see cref="VideoMode"/>.
        /// </summary>
        public int Height;

        /// <summary>
        /// The bit depth of the red channel of the <see cref="VideoMode"/>.
        /// </summary>
        public int RedBits;

        /// <summary>
        /// The bit depth of the green channel of the <see cref="VideoMode"/>.
        /// </summary>
        public int GreenBits;

        /// <summary>
        /// The bit depth of the blue channel of the <see cref="VideoMode"/>.
        /// </summary>
        public int BlueBits;

        /// <summary>
        /// The refresh rate, in Hz, of the <see cref="VideoMode"/>.
        /// </summary>
        public int RefreshRate;
    }
}
