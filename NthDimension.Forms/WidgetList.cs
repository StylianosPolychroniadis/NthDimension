using NthDimension.Forms.Delegates;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms
{
    public class WidgetList : IList<Widget>
    {
        public event WidgetAddHandler WidgetAddedEvent;
        public event WidgetRemoveHandler WidgetRemoveEvent;

        readonly List<Widget> widgets = new List<Widget>();
        Widget owner;

        public WidgetList(Widget owner)
        {
            this.owner = owner;
        }

        #region IList implementation

        public int IndexOf(Widget item)
        {
            return widgets.IndexOf(item);
        }

        public void Insert(int index, Widget item)
        {
            if (widgets.Contains(item) == false)
            {
                widgets.Insert(index, item);
                item.TabIndex = widgets.Count;
                item.SetParent(owner);
                if (WidgetAddedEvent != null)
                    WidgetAddedEvent(item);
            }
        }

        public void RemoveAt(int index)
        {
            Widget wr = widgets[index];
            widgets.RemoveAt(index);
            wr.SetParent(null);
            if (WidgetRemoveEvent != null)
                WidgetRemoveEvent(wr);
        }

        public Widget this[int index]
        {
            get
            {
                try
                {
                    return widgets[index];
                }
                catch
                {
                }
                return null;
            }
            set
            {
                if (widgets.Contains(value) == false)
                {
                    widgets[index].SetParent(null);
                    widgets[index] = value;
                    value.SetParent(owner);
                    if (WidgetAddedEvent != null)
                        WidgetAddedEvent(value);
                }
            }
        }

        #endregion

        #region ICollection<Widget> implementation

        public void Add(Widget item)
        {
            if (item is Widget.WHUD)
                throw new Exception("The Widget 'WHUD' can not be added to another Widget");

            if (widgets.Contains(item) == false)
            {
                if (item.Parent != null)
                    throw new Exception("Widget already added to another Widget");

                widgets.Add(item);
                item.TabIndex = widgets.Count;
                item.SetParent(owner);
                if (WidgetAddedEvent != null)
                    WidgetAddedEvent(item);
            }
        }

        public void AddRange(Widget[] items)
        {
            foreach(Widget w in items)
                try
                {
                    this.Add(w);
                }
                catch/*(Exception iE)*/
                {

                }
        }
        public void AddRange(List<Widget> items)
        {
            this.AddRange(items.ToArray());
        }

        public void Clear()
        {
            foreach (Widget w in widgets)
                w.SetParent(null);
            widgets.Clear();
        }

        public bool Contains(Widget item)
        {
            return widgets.Contains(item);
        }

        public void CopyTo(Widget[] array, int arrayIndex)
        {
            widgets.CopyTo(array, arrayIndex);
        }

        public bool Remove(Widget item)
        {
            bool r = widgets.Remove(item);
            if (r && WidgetRemoveEvent != null)
            {
                item.SetParent(null);
                WidgetRemoveEvent(item);
            }
            return r;
        }

        public int Count
        {
            get
            {
                return widgets.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region IEnumerable<Widget> implementation

        public IEnumerator<Widget> GetEnumerator()
        {
            return widgets.GetEnumerator();
        }

        #endregion

        #region IEnumerable implementation

        IEnumerator IEnumerable.GetEnumerator()
        {
            return widgets.GetEnumerator();
        }

        #endregion



    }
}
