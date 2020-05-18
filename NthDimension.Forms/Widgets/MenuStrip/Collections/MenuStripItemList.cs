using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NthDimension.Forms.Delegates;

namespace NthDimension.Forms.Widgets
{
    public class MenuStripItemList : IList<Widget>
    {
        public event NeoMenuItemAddHandler MenuItemAddedEvent;
        public event NeoMenuItemRemoveHandler MenuItemRemoveEvent;

        readonly List<Widget> items = new List<Widget>();
        //Widget owner;

        public MenuStripItemList() //Widget owner)
        {
            //this.owner = owner;
        }

        #region IList implementation

        public int IndexOf(Widget item)
        {
            return items.IndexOf(item);
        }

        public void Insert(int index, Widget item)
        {
            if (items.Contains(item) == false)
            {
                items.Insert(index, item);
                //item.SetParent(owner);
                if (MenuItemAddedEvent != null)
                    MenuItemAddedEvent(item);
            }
        }

        public void RemoveAt(int index)
        {
            Widget wr = items[index];
            items.RemoveAt(index);
            //wr.SetParent(null);
            if (MenuItemRemoveEvent != null)
                MenuItemRemoveEvent(wr);
        }

        public Widget this[int index]
        {
            get
            {
                return items[index];
            }
            set
            {
                if (items.Contains(value) == false)
                {
                    //menuItems[index].SetParent(null);
                    items[index] = value;
                    //value.SetParent(owner);
                    if (MenuItemAddedEvent != null)
                        MenuItemAddedEvent(value);
                }
            }
        }

        #endregion

        #region ICollection<MenuItem> implementation

        public void Add(Widget item)
        {
            if (((Widget)item).Parent != null)
                throw new Exception("Widget ya añadido a otro Widget.");
            //if (item is WHUD)
            //	throw new Exception("El Widget 'WHUD' no puede añadirse a otro Widget.");

            if (items.Contains(item) == false)
            {
                items.Add(item);
                //item.SetParent(owner);
                if (MenuItemAddedEvent != null)
                    MenuItemAddedEvent(item);
            }
        }

        public void Clear()
        {
            //foreach (Widget w in menuItems)
            //	w.SetParent(null);
            items.Clear();
        }

        public bool Contains(Widget item)
        {
            return items.Contains(item);
        }

        public void CopyTo(Widget[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        public bool Remove(Widget item)
        {
            bool r = items.Remove(item);
            if (r && MenuItemRemoveEvent != null)
            {
                //item.SetParent(null);
                MenuItemRemoveEvent(item);
            }
            return r;
        }

        public int Count
        {
            get
            {
                return items.Count;
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
            return items.GetEnumerator();
        }

        #endregion

        #region IEnumerable implementation

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }

        #endregion

        #region Public methods

        #endregion Public method
    }
}
