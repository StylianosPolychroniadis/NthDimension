namespace NthDimension.Windowing.Framework
{
    /// <summary>
    /// Attribute for setting <see cref="CursorModeValue"/> of the cursor.
    /// </summary>
    /// <seealso cref="Framework.SetInputMode(Window*,CursorStateAttribute,CursorModeValue)"/>
    /// <seealso cref="Framework.GetInputMode(Window*,CursorStateAttribute)"/>
    public enum CursorStateAttribute
    {
        /// <summary>
        /// Attribute for setting <see cref="CursorModeValue"/> of the cursor.
        /// </summary>
        Cursor = 0x00033001,
    }
}
