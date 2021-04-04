namespace NthDimension.Windowing.Framework
{
    /// <summary>
    /// Used to set the OpenGlProfile attribute.
    /// </summary>
    /// <seealso cref="Framework.WindowHint(WindowHintOpenGlProfile,OpenGlProfile)"/>
    public enum WindowHintOpenGlProfile
    {
        /// <summary>
        /// Indicates the OpenGL profile used by the context.
        /// This is <see cref="GraphicsLibraryFramework.OpenGlProfile.Core"/>
        /// or <see cref="GraphicsLibraryFramework.OpenGlProfile.Compat"/>
        /// if the context uses a known profile, or <see cref="GraphicsLibraryFramework.OpenGlProfile.Any"/>
        /// if the OpenGL profile is unknown or the context is an OpenGL ES context.
        /// Note that the returned profile may not match the profile bits of the context flags,
        /// as GLFW will try other means of detecting the profile when no bits are set.
        /// </summary>
        OpenGlProfile = 0x00022008,
    }
}
