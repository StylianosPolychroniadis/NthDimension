namespace NthDimension.Windowing
{
    /// <summary>
    /// Defines the event data for the <see cref="NativeWindow.MonitorConnected"/> event.
    /// </summary>
    public readonly struct MonitorEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonitorEventArgs"/> struct.
        /// </summary>
        /// <param name="monitor">The <see cref="Monitor"/> which triggered the event.</param>
        /// <param name="isConnected">Whether the <see cref="Monitor"/> is connected.</param>
        public MonitorEventArgs(MonitorHandle monitor, bool isConnected)
        {
            Monitor = monitor;
            IsConnected = isConnected;
        }

        /// <summary>
        /// Gets the <see cref="Monitor"/> which triggered the event.
        /// </summary>
        public MonitorHandle Monitor { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Monitor"/> that triggered this event is connected or not.
        /// </summary>
        public bool IsConnected { get; }
    }
}
