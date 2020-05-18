using System;


namespace NthDimension.Forms.Docking
{
    public class DockContentEventArgs : EventArgs
    {
        public DockContent Content { get; private set; }

        public DockContentEventArgs(DockContent content)
        {
            Content = content;
        }
    }
}
