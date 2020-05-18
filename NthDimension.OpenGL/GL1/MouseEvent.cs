using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Rasterizer.GL1
{
    public enum enuMouseButton
    {
        // As implemented by System.Windows.Forms

        // Summary:
        //     No mouse button was pressed.
        //////[EnumMember(Value = "None")]
        None = 0,
        //
        // Summary:
        //     The left mouse button was pressed.
        //////[EnumMember(Value = "Left")]
        Left = 1048576,
        //
        // Summary:
        //     The right mouse button was pressed.
        //////[EnumMember(Value = "Right")]
        Right = 2097152,
        //
        // Summary:
        //     The middle mouse button was pressed.
        //////[EnumMember(Value = "Middle")]
        Middle = 4194304,
        //
        // Summary:
        //     The first XButton was pressed.
        //////[EnumMember(Value = "XButton1")]
        XButton1 = 8388608,
        //
        // Summary:
        //     The second XButton was pressed.
        //////[EnumMember(Value = "XButton2")]
        XButton2 = 16777216,
    }

    public class MouseEvent
    {
        public MouseEvent() { }
        public MouseEvent(int X, int Y, int wheel, enuMouseButton buttons)
        {
            this.X = X;
            this.Y = Y;
            this.WheelDelta = wheel;
            this.Button = buttons;
        }

        readonly bool[] button_state = new bool[Enum.GetValues(typeof(enuMouseButton)).Length];
        //public int Clicks { get; }
        public enuMouseButton Button { get; set; }
        public int WheelDelta { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        /// <summary>
        /// Gets a System.Boolean indicating the state of the specified MouseButton.
        /// </summary>
        /// <param name="button">The MouseButton to check.</param>
        /// <returns>True if the MouseButton is pressed, false otherwise.</returns>
        public bool this[enuMouseButton button]
        {
            get
            {
                return button_state[(int)button];
            }
            set
            {
                button_state[(int)button] = value;
            }
        }
    }
}
