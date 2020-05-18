using NthDimension.Forms.Delegates;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NthDimension.Forms.Widgets
{
    public class ToolStripItemList : IList<IToolStripItem>
    {
        public event ToolStripItemAddHandler ToolStripItemAddedEvent;
        public event ToolStripItemRemoveHandler ToolStripItemRemoveEvent;

        readonly List<IToolStripItem> menuItems = new List<IToolStripItem>();
        //Widget owner;

        public ToolStripItemList() //Widget owner)
        {
            //this.owner = owner;
        }

        #region IList implementation

        public int IndexOf(IToolStripItem item)
        {
            return menuItems.IndexOf(item);
        }

        public void Insert(int index, IToolStripItem item)
        {
            if (menuItems.Contains(item) == false)
            {
                menuItems.Insert(index, item);
                //item.SetParent(owner);
                if (ToolStripItemAddedEvent != null)
                    ToolStripItemAddedEvent(item);
            }
        }

        public void RemoveAt(int index)
        {
            IToolStripItem wr = menuItems[index];
            menuItems.RemoveAt(index);
            //wr.SetParent(null);
            if (ToolStripItemRemoveEvent != null)
                ToolStripItemRemoveEvent(wr);
        }

        public IToolStripItem this[int index]
        {
            get
            {
                return menuItems[index];
            }
            set
            {
                if (menuItems.Contains(value) == false)
                {
                    //menuItems[index].SetParent(null);
                    menuItems[index] = value;
                    //value.SetParent(owner);
                    if (ToolStripItemAddedEvent != null)
                        ToolStripItemAddedEvent(value);
                }
            }
        }

        #endregion

        #region ICollection<MenuItem> implementation

        public void Add(IToolStripItem item)
        {
            if (((Widget)item).Parent != null)
                throw new Exception("Widget ya añadido a otro Widget.");
            //if (item is WHUD)
            //	throw new Exception("El Widget 'WHUD' no puede añadirse a otro Widget.");

            if (menuItems.Contains(item) == false)
            {
                menuItems.Add(item);
                //item.SetParent(owner);
                if (ToolStripItemAddedEvent != null)
                    ToolStripItemAddedEvent(item);
            }
        }

        public void Clear()
        {
            //foreach (Widget w in menuItems)
            //	w.SetParent(null);
            menuItems.Clear();
        }

        public bool Contains(IToolStripItem item)
        {
            return menuItems.Contains(item);
        }

        public void CopyTo(IToolStripItem[] array, int arrayIndex)
        {
            menuItems.CopyTo(array, arrayIndex);
        }

        public bool Remove(IToolStripItem item)
        {
            bool r = menuItems.Remove(item);
            if (r && ToolStripItemRemoveEvent != null)
            {
                //item.SetParent(null);
                ToolStripItemRemoveEvent(item);
            }
            return r;
        }

        public int Count
        {
            get
            {
                return menuItems.Count;
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

        public IEnumerator<IToolStripItem> GetEnumerator()
        {
            return menuItems.GetEnumerator();
        }

        #endregion

        #region IEnumerable implementation

        IEnumerator IEnumerable.GetEnumerator()
        {
            return menuItems.GetEnumerator();
        }

        #endregion

        #region Public methods

        #endregion Public method
    }
}
