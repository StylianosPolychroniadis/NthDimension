namespace NthDimension.Windowing.Framework
{
    /// <summary>
    /// Used to set context robustness attribute.
    /// </summary>
    /// <seealso cref="Framework.WindowHint(WindowHintRobustness,Robustness)"/>
    public enum WindowHintRobustness
    {
        /// <summary>
        /// Indicates the robustness strategy used by the context.
        /// This is <see cref="Robustness.LoseContextOnReset"/> or <see cref="Robustness.NoResetNotification"/>
        /// if the window's context supports robustness, or <see cref="Robustness.NoRobustness"/> otherwise.
        /// </summary>
        ContextRobustness = 0x00022005,
    }
}
