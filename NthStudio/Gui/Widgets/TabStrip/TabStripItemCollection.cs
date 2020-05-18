
using System;
using System.ComponentModel;
//using System.ComponentModel;

using NthDimension.Collections;

namespace NthStudio.Gui.Widgets.TabStrip
{
    public class CollectionChangeEventArgs : EventArgs
    {
        CollectionChangeAction Action_;
        //
        // Properties
        //
        public virtual CollectionChangeAction Action
        {
            get { return Action_; }
        }
        object Element_;
        public virtual object Element
        {
            get { return Element_; }
        }

        //
        // Constructors
        //
        public CollectionChangeEventArgs(CollectionChangeAction action, object element)
        {
            Action_ = action;
            Element_ = element;
        }
    }

    public class TabStripItemCollection : CollectionWithEvents
    {
        #region Fields

        [Browsable(false)]
        public event CollectionChangeEventHandler CollectionChanged;

        private int lockUpdate;

        #endregion

        #region Ctor

        public TabStripItemCollection()
        {
            lockUpdate = 0;
        }

        #endregion

        #region Props

        public TabStripItem this[int index]
        {
            get
            {
                if (index < 0 || List.Count - 1 < index)
                    return null;

                return (TabStripItem)List[index];
            }
            set
            {
                List[index] = value;
            }
        }

        [Browsable(false)]
        public virtual int DrawnCount
        {
            get
            {
                int count = Count, res = 0;
                if (count == 0) return 0;
                for (int n = 0; n < count; n++)
                {
                    if (this[n].IsDrawn)
                        res++;
                }
                return res;
            }
        }

        public virtual TabStripItem LastVisible
        {
            get
            {
                for (int n = Count - 1; n > 0; n--)
                {
                    if (this[n].IsVisible)
                        return this[n];
                }

                return null;
            }
        }

        public virtual TabStripItem FirstVisible
        {
            get
            {
                for (int n = 0; n < Count; n++)
                {
                    if (this[n].IsVisible)
                        return this[n];
                }

                return null;
            }
        }

        [Browsable(false)]
        public virtual int VisibleCount
        {
            get
            {
                int count = Count, res = 0;
                if (count == 0) return 0;
                for (int n = 0; n < count; n++)
                {
                    if (this[n].IsVisible)
                        res++;
                }
                return res;
            }
        }

        #endregion

        #region Methods

        protected virtual void OnCollectionChanged(CollectionChangeEventArgs e)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, e);
        }

        protected virtual void BeginUpdate()
        {
            lockUpdate++;
        }

        protected virtual void EndUpdate()
        {
            if (--lockUpdate == 0)
                OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
        }

        public virtual void AddRange(TabStripItem[] items)
        {
            BeginUpdate();
            try
            {
                foreach (TabStripItem item in items)
                {
                    List.Add(item);
                }
            }
            finally
            {
                EndUpdate();
            }
        }

        public virtual void Assign(TabStripItemCollection collection)
        {
            BeginUpdate();
            try
            {
                Clear();
                for (int n = 0; n < collection.Count; n++)
                {
                    TabStripItem item = collection[n];
                    TabStripItem newItem = new TabStripItem();
                    newItem.Assign(item);
                    Add(newItem);
                }
            }
            finally
            {
                EndUpdate();
            }
        }

        public virtual int Add(TabStripItem item)
        {
            int res = IndexOf(item);
            if (res == -1)
                res = List.Add(item);
            return res;
        }

        public virtual void Remove(TabStripItem item)
        {
            if (List.Contains(item))
            {
                // TODO Crear una clase 'Widget.WidgetCollection' para poder
                // acceder al método privado 'SetParent()', para no utilizar 'item.SetParent(null)'
                item.SetParent(null);
                List.Remove(item);
            }
        }

        public virtual TabStripItem MoveTo(int newIndex, TabStripItem item)
        {
            int currentIndex = List.IndexOf(item);
            if (currentIndex >= 0)
            {
                RemoveAt(currentIndex);
                // TODO Crear una clase 'Widget.WidgetCollection' para poder
                // acceder al método privado 'SetParent()', para no utilizar 'item.SetParent(null)'
                item.SetParent(null);
                Insert(0, item);

                return item;
            }

            return null;
        }

        public virtual int IndexOf(TabStripItem item)
        {
            return List.IndexOf(item);
        }

        public virtual bool Contains(TabStripItem item)
        {
            return List.Contains(item);
        }

        public virtual void Insert(int index, TabStripItem item)
        {
            if (Contains(item))
                return;
            List.Insert(index, item);
        }

        protected override void OnInsertComplete(int index, object item)
        {
            TabStripItem itm = item as TabStripItem;
            itm.Changed += new EventHandler(OnItem_Changed);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, item));
        }

        protected override void OnRemove(int index, object item)
        {
            base.OnRemove(index, item);
            TabStripItem itm = item as TabStripItem;
            itm.Changed -= new EventHandler(OnItem_Changed);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, item));
        }

        protected override void OnClear()
        {
            if (Count == 0) return;
            BeginUpdate();
            try
            {
                for (int n = Count - 1; n >= 0; n--)
                {
                    RemoveAt(n);
                }
            }
            finally
            {
                EndUpdate();
            }
        }

        protected virtual void OnItem_Changed(object sender, EventArgs e)
        {
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, sender));
        }

        #endregion
    }
}
