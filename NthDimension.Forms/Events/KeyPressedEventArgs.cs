using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms.Events
{
    public class KeyPressedEventArgs : EventArgs
    {
        //
        // Properties
        //
        public bool Handled
        {
            get;
            set;
        }

        public char KeyChar
        {
            get;
            set;
        }

        //
        // Constructors
        //
        public KeyPressedEventArgs(char keyChar)
        {
            this.KeyChar = keyChar;
        }
    }
}
