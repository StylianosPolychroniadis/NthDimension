namespace NthDimension.Windowing
{
    /// <summary>
    /// Defines the event data for the window maximizing event.
    /// </summary>
    public readonly struct MaximizedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaximizedEventArgs"/> struct.
        /// </summary>
        /// <param name="isMaximized">
        /// A value indicating whether the window is maximized.
        /// </param>
        public MaximizedEventArgs(bool isMaximized)
        {
            IsMaximized = isMaximized;
        }

        /// <summary>
        /// Gets a value indicating whether the window is maximized.
        /// </summary>
        public bool IsMaximized { get; }
    }
}
