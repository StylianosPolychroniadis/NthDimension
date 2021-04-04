namespace NthDimension.Windowing.Framework
{
    /// <summary>
    /// The OpenGL context profiles.
    /// </summary>
    public enum OpenGlProfile : int
    {
        /// <summary>
        /// Used for unknown OpenGL profile or OpenGL ES.
        /// </summary>
        Any = 0,

        /// <summary>
        /// Known OpenGL Core profile.
        /// </summary>
        Core = 0x00032001,

        /// <summary>
        /// Known OpenGL compatibility profile.
        /// </summary>
        Compat = 0x00032002
    }
}
