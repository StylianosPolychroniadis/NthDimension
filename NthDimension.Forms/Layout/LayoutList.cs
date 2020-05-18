using NthDimension.Forms.Delegates;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms.Layout
{
    public class LayoutList : IList<LayoutBase>
    {
        public event LayoutAddHandler               LayoutAddedEvent;
        public event LayoutRemoveHandler            LayoutRemoveEvent;

        private readonly List<LayoutBase>           layouts = new List<LayoutBase>();

        #region IList implementation

        public int IndexOf(LayoutBase item)
        {
            return layouts.IndexOf(item);
        }

        public void Insert(int index, LayoutBase item)
        {
            layouts.Insert(index, item);

            if (LayoutAddedEvent != null)
                LayoutAddedEvent(item);
        }

        public void RemoveAt(int index)
        {
            LayoutBase lb = layouts[index];
            layouts.RemoveAt(index);

            if (LayoutRemoveEvent != null)
                LayoutRemoveEvent(lb);
        }

        public LayoutBase this[int index]
        {
            get
            {
                return layouts[index];
            }
            set
            {
                LayoutBase lb;

                if (index > 0)
                {
                    lb = layouts[index - 1];
                    if (lb.Owner == value.Owner)
                        return;
                }
                if (index < layouts.Count - 1)
                {
                    lb = layouts[index + 1];
                    if (lb.Owner == value.Owner)
                        return;
                }
                layouts[index] = value;

                if (LayoutAddedEvent != null)
                    LayoutAddedEvent(value);
            }
        }

        #endregion

        #region ICollection<Widget> implementation

        public void Add(LayoutBase item)
        {
            try
            {
                if (layouts.Count > 0 && layouts[layouts.Count - 1].Owner == item.Owner)
                    return;
                layouts.Add(item);

                if (LayoutAddedEvent != null)
                    LayoutAddedEvent(item);
            }
            catch
            {

            }
        }

        public void Clear()
        {
            layouts.Clear();
        }

        public bool Contains(LayoutBase item)
        {
            return layouts.Contains(item);
        }

        public void CopyTo(LayoutBase[] array, int arrayIndex)
        {
            layouts.CopyTo(array, arrayIndex);
        }

        public bool Remove(LayoutBase item)
        {
            bool r = layouts.Remove(item);
            if (r && LayoutRemoveEvent != null)
            {
                LayoutRemoveEvent(item);
            }
            return r;
        }

        public int Count
        {
            get
            {
                return layouts.Count;
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

        #region IEnumerable<LayoutBase> implementation

        public IEnumerator<LayoutBase> GetEnumerator()
        {
            return layouts.GetEnumerator();
        }

        #endregion

        #region IEnumerable implementation

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)layouts.GetEnumerator();
        }

        #endregion

        #region Public methods

        #endregion Public methods
    }
}
