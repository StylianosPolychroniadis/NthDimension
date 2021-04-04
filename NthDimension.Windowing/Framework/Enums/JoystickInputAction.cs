namespace NthDimension.Windowing.Framework
{
    /// <summary>
    /// Defines event information for <see cref="Framework.GetJoystickButtons"/> and <see cref="Framework.GetJoystickButtonsRaw(int,out int)"/>.
    /// </summary>
    public enum JoystickInputAction : byte
    {
        /// <summary>
        /// The joystick button was released.
        /// </summary>
        Release = 0,

        /// <summary>
        /// The joystick button was pressed.
        /// </summary>
        Press = 1,
    }
}
