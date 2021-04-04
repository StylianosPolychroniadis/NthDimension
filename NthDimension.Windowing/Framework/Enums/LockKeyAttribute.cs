namespace NthDimension.Windowing.Framework
{
    /// <summary>
    /// Attribute for setting caps and num lock bits on when setting the input mode.
    /// </summary>
    /// <seealso cref="Framework.SetInputMode(Window*,LockKeyModAttribute,bool)"/>
    /// <seealso cref="Framework.GetInputMode(Window*,LockKeyModAttribute)"/>
    public enum LockKeyModAttribute
    {
        /// <summary>
        /// Specify whether the lock key bits should be set or not.
        /// </summary>
        LockKeyMods = 0x00033004
    }
}
