using System;
using System.Collections;
using System.Collections.Generic;

namespace NthDimension.Forms.Widgets
{
    public delegate void ItemsCollectionChangedHandler(object sender, ListViewItemParameter itemParm);

    /// <summary>
    /// Advanced ListView Items Collection, can rise events when items added, removed ... etc
    /// </summary>
    public class ListViewItemsCollection : ICollection<ListViewItem>
    {
        private List<ListViewItem> items = new List<ListViewItem>();
        //Events
        /// <summary>
        /// Rised whan an item added to the collection
        /// </summary>
        public event ItemsCollectionChangedHandler ItemAddedEvent;
        /// <summary>
        /// Rised when an item removed from the collection
        /// </summary>
        public event ItemsCollectionChangedHandler ItemRemovedEvent;
        /// <summary>
        /// Rised when the collection get cleared
        /// </summary>
        public event EventHandler CollectionClearEvent;
        /// <summary>
        /// Advanced ListView Items Collection
        /// </summary>
        /// <param name="index">The item index</param>
        /// <returns><see cref="ListViewItem"/></returns>
        public ListViewItem this[int index]
        {
            get { if (index < items.Count && index >= 0) return items[index]; else return null; }
            set { if (index < items.Count && index >= 0) items[index] = value; }
        }
        /// <summary>
        /// Add item to the collection
        /// </summary>
        /// <param name="item"><see cref="ManagedListViewItem"/></param>
        public void Add(ListViewItem item)
        {
            items.Add(item);
            if (ItemAddedEvent != null)
                ItemAddedEvent(this, new ListViewItemParameter(item));
        }
        /// <summary>
        /// Clear this collection
        /// </summary>
        public void Clear()
        {
            items.Clear();
            if (CollectionClearEvent != null)
                CollectionClearEvent(this, new EventArgs());
        }
        /// <summary>
        /// Get whether an item exist in this collection
        /// </summary>
        /// <param name="item"><see cref="ManagedListViewItem"/></param>
        /// <returns>True if the item exists otherwise false</returns>
        public bool Contains(ListViewItem item)
        {
            return items.Contains(item);
        }
        /// <summary>
        /// Copy this collection to an array
        /// </summary>
        /// <param name="array">The target array to copy into</param>
        /// <param name="arrayIndex">The index within the target array to start with</param>
        public void CopyTo(ListViewItem[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }
        /// <summary>
        /// Get the items count in this collecion
        /// </summary>
        public int Count
        {
            get { return items.Count; }
        }
        /// <summary>
        /// Get whether this collection is read only
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }
        /// <summary>
        /// Remove an item from this collection
        /// </summary>
        /// <param name="item"><see cref="ManagedListViewItem"/> to remove</param>
        /// <returns>True if removed successfuly otherwise false.</returns>
        public bool Remove(ListViewItem item)
        {
            if (ItemRemovedEvent != null)
                ItemRemovedEvent(this, new ListViewItemParameter(item));
            return items.Remove(item);
        }
        /// <summary>
        /// Get the index of given item within this collection
        /// </summary>
        /// <param name="item">The item to get index of</param>
        /// <returns>The index of given item if found otherwise false</returns>
        public int IndexOf(ListViewItem item)
        {
            return items.IndexOf(item);
        }
        /// <summary>
        /// Get Enumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }
        IEnumerator<ListViewItem> IEnumerable<ListViewItem>.GetEnumerator()
        {
            return items.GetEnumerator();
        }
        /// <summary>
        /// Insert item to this collection at given index
        /// </summary>
        /// <param name="index">The index to insert the item at</param>
        /// <param name="item">The item to insert</param>
        public void Insert(int index, ListViewItem item)
        {
            items.Insert(index, item);
            if (ItemAddedEvent != null)
                ItemAddedEvent(this, new ListViewItemParameter(item));
        }
        /// <summary>
        /// Sort the items collection
        /// </summary>
        public void Sort()
        {
            items.Sort();
        }
        /// <summary>
        /// Sort the items collection using comparer
        /// </summary>
        /// <param name="comparer">The comparer to use in compare operation</param>
        public void Sort(IComparer<ListViewItem> comparer)
        {
            items.Sort(comparer);
        }
        /// <summary>
        /// Sort the items collection using comparer
        /// </summary>
        /// <param name="index">The start index to start with</param>
        /// <param name="count">The count of items</param>
        /// <param name="comparer">The comparer to use in compare operation</param>
        public void Sort(int index, int count, IComparer<ListViewItem> comparer)
        {
            items.Sort(index, count, comparer);
        }
    }
}
