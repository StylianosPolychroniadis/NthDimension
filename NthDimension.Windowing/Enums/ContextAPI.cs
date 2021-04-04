namespace NthDimension.Windowing
{
    /// <summary>
    /// Describes the graphics API you want the context to use.
    /// </summary>
    public enum ContextAPI
    {
        /// <summary>
        /// Indicates that an API has not been specifically requested for context creation.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is primarily for integrating an external API with this window, such as Vulkan.
        /// </para>
        /// </remarks>
        NoAPI,

        /// <summary>
        /// Indicates that the context should be created for OpenGL ES.
        /// </summary>
        OpenGLES,

        /// <summary>
        /// Indicates that the context should be created for OpenGL.
        /// </summary>
        OpenGL,
    }
}
