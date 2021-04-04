﻿using NthDimension.Windowing.Framework;

namespace NthDimension.Windowing
{
    /// <summary>
    /// Defines the event data for <see cref="INativeWindow.MouseDown" />
    /// and <see cref="INativeWindow.MouseUp" /> events.
    /// </summary>
    public readonly struct MouseButtonEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MouseButtonEventArgs"/> struct.
        /// </summary>
        /// <param name="button">The mouse button for the event.</param>
        /// <param name="action">The action of the mouse button.</param>
        /// <param name="modifiers">The key modifiers held during the mouse button's action.</param>
        public MouseButtonEventArgs(NthDimension.Windowing.Framework.MouseButton button, InputAction action, KeyModifiers modifiers)
        {
            Button = button;
            Action = action;
            Modifiers = modifiers;
        }

        /// <summary>
        /// Gets the <see cref="MouseButton" /> that triggered this event.
        /// </summary>
        public NthDimension.Windowing.Framework.MouseButton Button { get; }

        /// <summary>
        /// Gets the <see cref="InputAction"/> of the pressed button.
        /// </summary>
        public InputAction Action { get; }

        /// <summary>
        /// Gets the active <see cref="KeyModifiers"/> of the pressed button.
        /// </summary>
        public KeyModifiers Modifiers { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Button"/> which triggered this event was pressed or released.
        /// </summary>
        public bool IsPressed => Action != InputAction.Release;
    }
}
