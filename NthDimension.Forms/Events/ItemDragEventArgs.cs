using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms.Events
{
    public class ItemDragEventArgs : EventArgs
    {
        private readonly MouseButton button;

        private readonly object item;

        public MouseButton Button
        {
            get
            {
                return this.button;
            }
        }

        public object Item
        {
            get
            {
                return this.item;
            }
        }

        public ItemDragEventArgs(MouseButton button)
        {
            this.button = button;
            this.item = null;
        }

        public ItemDragEventArgs(MouseButton button, object item)
        {
            this.button = button;
            this.item = item;
        }
    }
}
