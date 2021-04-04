using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Windowing.Framework
{
    /// <summary>
    /// This describes the input state of a gamepad.
    /// </summary>
    public struct GamepadState
    {
        /// <summary>
        /// State of each of the 15 gamepad buttons, equal to <see cref="InputAction.Press"/> or <see cref="InputAction.Release"/>.
        /// </summary>
        public unsafe fixed byte Buttons[15];

        /// <summary>
        /// State of each of the 6 gamepad axes, ranging from -1.0 to 1.0.
        /// </summary>
        public unsafe fixed float Axes[6];
    }
}
