using System;

namespace NthDimension.Forms.Events
{
    public class KeyEventArgs : EventArgs
    {
        //
        // Properties
        //
        public virtual bool Alt
        {
            get;
            private set;
        }

        public bool Control
        {
            get;
            private set;
        }

        public bool Handled
        {
            get;
            set;
        }

        public Keys KeyCode
        {
            get;
            private set;
        }

        public Keys KeyData
        {
            get;
            private set;
        }

        public int KeyValue
        {
            get;
            private set;
        }

        public Keys Modifiers
        {
            get;
            private set;
        }

        public virtual bool Shift
        {
            get;
            private set;
        }

        public bool SuppressKeyPress
        {
            get;
            set;
        }

        //
        // Constructors
        //
        public KeyEventArgs(Keys keyData)
        {
            this.KeyData = keyData;
        }
    }
}
