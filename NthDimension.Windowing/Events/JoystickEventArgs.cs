namespace NthDimension.Windowing
{
    /// <summary>
    /// Defines the event data for the <see cref="NativeWindow.JoystickConnected"/> event.
    /// </summary>
    public readonly struct JoystickEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JoystickEventArgs"/> struct.
        /// </summary>
        /// <param name="joystickId">The Id of the joystick which triggered this event.</param>
        /// <param name="isConnected">
        /// A value indicating whether the joystick which triggered this event was connected.
        /// </param>
        public JoystickEventArgs(int joystickId, bool isConnected)
        {
            JoystickId = joystickId;
            IsConnected = isConnected;
        }

        /// <summary>
        /// Gets the Id of the joystick which triggered this event.
        /// </summary>
        public int JoystickId { get; }

        /// <summary>
        /// Gets a value indicating whether the joystick which triggered this event was connected.
        /// </summary>
        public bool IsConnected { get; }
    }
}
