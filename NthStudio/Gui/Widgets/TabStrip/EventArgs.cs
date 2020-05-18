using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Gui.Widgets.TabStrip
{
    public class TabStripItemChangedEventArgs : EventArgs
    {
        TabStripItem itm;
        TabStripItemChangeTypes changeType;

        public TabStripItemChangedEventArgs(TabStripItem item, TabStripItemChangeTypes type)
        {
            changeType = type;
            itm = item;
        }

        public TabStripItemChangeTypes ChangeType
        {
            get { return changeType; }
        }

        public TabStripItem Item
        {
            get { return itm; }
        }
    }

    public class TabStripItemClosingEventArgs : EventArgs
    {
        public TabStripItemClosingEventArgs(TabStripItem item)
        {
            _item = item;
        }

        private bool _cancel = false;
        private TabStripItem _item;

        public TabStripItem Item
        {
            get { return _item; }
            set { _item = value; }
        }

        public bool Cancel
        {
            get { return _cancel; }
            set { _cancel = value; }
        }

    }
}
