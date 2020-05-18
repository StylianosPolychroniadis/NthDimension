using System.Collections.Generic;

namespace NthStudio.Gui.Widgets.ObjectExplorer
{
    enum ExplorerItemType
    {
        Class, Method, Property, Event
    }
    class ExplorerItem
    {
        public ExplorerItemType type;
        public string title;
        public int position;
    }
    class ExplorerItemComparer : IComparer<ExplorerItem>
    {
        public int Compare(ExplorerItem x, ExplorerItem y)
        {
            return x.title.CompareTo(y.title);
        }
    }
}
