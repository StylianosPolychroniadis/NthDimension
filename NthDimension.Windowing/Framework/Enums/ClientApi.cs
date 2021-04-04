namespace NthDimension.Windowing.Framework
{
    /// <summary>
    /// The context client APIs.
    /// </summary>
    /// <seealso cref="Framework.WindowHint(WindowHintClientApi,ClientApi)"/>
    public enum ClientApi : int
    {
        /// <summary>
        /// No context API is created.
        /// </summary>
        NoApi = 0,

        /// <summary>
        /// OpenGL context is created.
        /// </summary>
        OpenGlApi = 0x00030001,

        /// <summary>
        /// OpenGL ES context is created.
        /// </summary>
        OpenGlEsApi = 0x00030002
    }
}
