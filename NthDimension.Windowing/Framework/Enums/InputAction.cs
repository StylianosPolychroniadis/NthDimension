namespace NthDimension.Windowing.Framework
{
    /// <summary>
    /// Defines event information for <see cref="GLFWCallbacks.KeyCallback"/>
    /// or <see cref="GLFWCallbacks.MouseButtonCallback"/>.
    /// </summary>
    public enum InputAction
    {
        /// <summary>
        /// The key or mouse button was released.
        /// </summary>
        Release = 0,

        /// <summary>
        /// The key or mouse button was pressed.
        /// </summary>
        Press = 1,

        /// <summary>
        /// The key was held down until it repeated.
        /// </summary>
        Repeat = 2
    }
}
