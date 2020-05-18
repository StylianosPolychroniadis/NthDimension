using System;

namespace NthDimension.Forms.Events
{
    public class ScrollEventArgs : EventArgs
    {
        //
        // Properties
        //
        public int NewValue
        {
            get;
            set;
        }

        public int OldValue
        {
            get;
            private set;
        }

        public EScrollOrientation ScrollOrientation
        {
            get;
            private set;
        }

        public ScrollEventType Type
        {
            get;
            private set;
        }

        //
        // Constructors
        //
        public ScrollEventArgs(ScrollEventType type, int newValue)
        {
        }

        public ScrollEventArgs(ScrollEventType type, int oldValue, int newValue)
        {
        }

        public ScrollEventArgs(ScrollEventType type, int newValue, EScrollOrientation scroll)
        {
        }

        public ScrollEventArgs(ScrollEventType type, int oldValue, int newValue, EScrollOrientation scroll)
        {
        }
    }
}
