namespace NthDimension.Windowing.Framework
{
    /// <summary>
    /// Used to get window related attributes.
    /// </summary>
    /// <seealso cref="Framework.GetWindowAttrib(Window*, WindowAttributeGetContextApi)"/>
    public enum WindowAttributeGetContextApi
    {
        /// <summary>
        /// Indicates the context creation API used to create the window's context;
        /// either <see cref="ContextApi.NativeContextApi"/> or <see cref="ContextApi.EglContextApi"/>.
        /// </summary>
        ContextCreationApi = WindowHintContextApi.ContextCreationApi
    }
}
