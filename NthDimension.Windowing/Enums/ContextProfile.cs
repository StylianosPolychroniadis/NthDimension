﻿namespace NthDimension.Windowing
{
    /// <summary>
    /// Selects the profile for the context's graphics API.
    /// For versions below 3.2 the <see cref="Any"/> option needs to be used.
    /// </summary>
    public enum ContextProfile
    {
        /// <summary>
        /// Used for unknown OpenGL profile or OpenGL ES.
        /// </summary>
        Any,

        /// <summary>
        /// Selects compatability profile. You should only use this if maintaining legacy code.
        /// </summary>
        Compatability,

        /// <summary>
        /// Selects core profile. All new projects should use this unless they have a good reason not to.
        /// </summary>
        Core,
    }
}
