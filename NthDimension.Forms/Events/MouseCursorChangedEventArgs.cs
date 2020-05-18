using System;

namespace NthDimension.Forms.Events
{
    public class MouseCursorChangedEventArgs : EventArgs
    {
        public NanoCursor NewCursor;
        public MouseCursorChangedEventArgs(NanoCursor newCursor) : base()
        {
            NewCursor = newCursor;
        }
    }
}
