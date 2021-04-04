namespace NthDimension.Windowing.Framework
{
    /// <summary>
    /// Context related client API attribute.
    /// </summary>
    /// <seealso cref="Framework.WindowHint(WindowHintClientApi,ClientApi)"/>
    public enum WindowHintClientApi
    {
        /// <summary>
        /// Indicates the client API provided by the window's context;
        /// either <see cref="GraphicsLibraryFramework.ClientApi.OpenGlApi"/>,
        /// <see cref="GraphicsLibraryFramework.ClientApi.OpenGlEsApi"/> or
        /// <see cref="GraphicsLibraryFramework.ClientApi.NoApi"/>.
        /// </summary>
        ClientApi = 0x00022001,
    }
}
