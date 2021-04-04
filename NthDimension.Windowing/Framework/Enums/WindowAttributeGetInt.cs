namespace NthDimension.Windowing.Framework
{
    /// <summary>
    /// Used to get window related attributes.
    /// </summary>
    /// <seealso cref="Framework.GetWindowAttrib(Window*, WindowAttributeGetInt)"/>
    public enum WindowAttributeGetInt
    {
        /// <summary>
        /// Indicate the client API version(major part) of the window's context.
        /// </summary>
        ContextVersionMajor = WindowHintInt.ContextVersionMajor,

        /// <summary>
        /// Indicate the client API version(minor part) of the window's context.
        /// </summary>
        ContextVersionMinor = WindowHintInt.ContextVersionMinor,

        /// <summary>
        /// Indicate the client API version(revision part) of the window's context.
        /// </summary>
        ContextVersionRevision = WindowHintInt.ContextRevision
    }
}
