namespace NthDimension.Windowing.Framework
{
    /// <summary>
    /// Used to specify the release behavior used by the local context.
    /// </summary>
    /// <seealso cref="Framework.WindowHint(WindowHintReleaseBehavior,ReleaseBehavior)"/>
    public enum WindowHintReleaseBehavior
    {
        /// <summary>
        /// Specifies the release behavior to be used by the context.
        /// Possible values are one of <see cref="ReleaseBehavior.Any"/>,
        /// <see cref="ReleaseBehavior.Flush"/> or <see cref="ReleaseBehavior.None"/>.
        /// If the behavior is <see cref="ReleaseBehavior"/>, the default behavior
        /// of the context creation API will be used.
        /// If the behavior is <see cref="ReleaseBehavior.Flush"/>, the pipeline will be flushed
        /// whenever the context is released from being the current one.
        /// If the behavior is <see cref="ReleaseBehavior.None"/>, the pipeline will not be flushed on release.
        /// </summary>
        ContextReleaseBehavior = 0x00022009,
    }
}
