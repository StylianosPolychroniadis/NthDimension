using NthDimension.Algebra;

namespace NthDimension.Windowing
{
    /// <summary>
    /// Defines the event data for the window resize event.
    /// </summary>
    public readonly struct ResizeEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResizeEventArgs"/> struct.
        /// </summary>
        /// <param name="size">The new window size.</param>
        public ResizeEventArgs(Vector2i size)
        {
            Size = size;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResizeEventArgs"/> struct.
        /// </summary>
        /// <param name="width">The new window width.</param>
        /// <param name="height">The new window height.</param>
        public ResizeEventArgs(int width, int height)
            : this(new Vector2i(width, height))
        {
        }

        /// <summary>
        /// Gets the new window size.
        /// </summary>
        public Vector2i Size { get; }

        /// <summary>
        /// Gets the new window width.
        /// </summary>
        public int Width => Size.X;

        /// <summary>
        /// Gets the new window height.
        /// </summary>
        public int Height => Size.Y;
    }
}
