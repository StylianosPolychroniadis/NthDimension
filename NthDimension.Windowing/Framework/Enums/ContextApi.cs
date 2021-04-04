namespace NthDimension.Windowing.Framework
{
    /// <summary>
    /// The context API used to create the window context.
    /// </summary>
    public enum ContextApi : int
    {
        /// <summary>
        /// Uses the native context API to create the window context.
        /// </summary>
        NativeContextApi = 0x00036001,

        /// <summary>
        /// Uses Egl to create the window context.
        /// </summary>
        EglContextApi = 0x00036002
    }
}
