using System;
using System.Collections;
using System.Collections.Generic;

namespace NthDimension.Forms.Widgets
{
    public delegate void ColumnsCollectionChangedHandler(object sender, ListViewColumnParameter columnParm);

    /// <summary>
    /// The columns collection, can rise events when columns added, removed ... etc
    /// </summary>
    public class ListViewColumnsCollection : ICollection<ListViewColumn>
    {
        private List<ListViewColumn> columns = new List<ListViewColumn>();
        //Events
        /// <summary>
        /// Rised when a column added to the collection.
        /// </summary>
        public event ColumnsCollectionChangedHandler ColumnAddedEvent;
        /// <summary>
        /// Rised when a column remove from the collection.
        /// </summary>
        public event ColumnsCollectionChangedHandler ColumnRemovedEvent;
        /// <summary>
        /// Rised when the collection get cleared.
        /// </summary>
        public event EventHandler CollectionClear;
        /// <summary>
        /// The columns collection.
        /// </summary>
        /// <param name="index">The column index within this collection.</param>
        /// <returns><see cref="ManagedListViewColumn"/></returns>
        public ListViewColumn this[int index]
        { get { return columns[index]; } set { columns[index] = value; } }
        /// <summary>
        /// Add column to this collection
        /// </summary>
        /// <param name="item"><see cref="ManagedListViewColumn"/></param>
        public void Add(ListViewColumn item)
        {
            columns.Add(item);
            if (ColumnAddedEvent != null)
                ColumnAddedEvent(this, new ListViewColumnParameter(item));
        }
        /// <summary>
        /// Insert column to this collection
        /// </summary>
        /// <param name="index">The index to insert at</param>
        /// <param name="item"><see cref="ManagedListViewColumn"/></param>
        public void Insert(int index, ListViewColumn item)
        {
            columns.Insert(index, item);
            if (ColumnAddedEvent != null)
                ColumnAddedEvent(this, new ListViewColumnParameter(item));
        }
        /// <summary>
        /// Clear this collection
        /// </summary>
        public void Clear()
        {
            columns.Clear();
            if (CollectionClear != null)
                CollectionClear(this, new EventArgs());
        }
        /// <summary>
        /// Get value indecate whether a column exists in this collection.
        /// </summary>
        /// <param name="item"><see cref="ManagedListViewColumn"/></param>
        /// <returns>True if given column exists in this collection otherwise false</returns>
        public bool Contains(ListViewColumn item)
        {
            return columns.Contains(item);
        }
        /// <summary>
        /// Copy this collection to an array.
        /// </summary>
        /// <param name="array">The target array to copy into</param>
        /// <param name="arrayIndex">The index within the target array to start with</param>
        public void CopyTo(ListViewColumn[] array, int arrayIndex)
        {
            columns.CopyTo(array, arrayIndex);
        }
        /// <summary>
        /// Get columns count in this collection
        /// </summary>
        public int Count
        {
            get { return columns.Count; }
        }
        /// <summary>
        /// Get a value indecate whether this collection is read only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }
        /// <summary>
        /// Remove a column from this collection
        /// </summary>
        /// <param name="item">The <see cref="ManagedListViewColumn"/> to remove</param>
        /// <returns>True if column removed successfuly otherwise false.</returns>
        public bool Remove(ListViewColumn item)
        {
            if (ColumnRemovedEvent != null)
                ColumnRemovedEvent(this, new ListViewColumnParameter(item));
            return columns.Remove(item);
        }
        /// <summary>
        /// Get Enumerator
        /// </summary>
        /// <returns>The enumerator of this collection</returns>
        public IEnumerator<ListViewColumn> GetEnumerator()
        {
            return columns.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return columns.GetEnumerator();
        }
        /// <summary>
        /// Get column using given id
        /// </summary>
        /// <param name="id">The target column id</param>
        /// <returns>The column if found otherwise null.</returns>
        public ListViewColumn GetColumnByID(string id)
        {
            foreach (ListViewColumn column in columns)
            {
                if (column.ID == id) return column;
            }
            return null;
        }

        /// <summary>
        /// Sort the columns collection
        /// </summary>
        public void Sort()
        {
            columns.Sort();
        }
        /// <summary>
        /// Sort the columns collection using comparer
        /// </summary>
        /// <param name="comparer">The comparer to use in compare operation</param>
        public void Sort(IComparer<ListViewColumn> comparer)
        {
            columns.Sort(comparer);
        }
        /// <summary>
        /// Sort the columns collection using comparer
        /// </summary>
        /// <param name="index">The start index to start with</param>
        /// <param name="count">The count of columns</param>
        /// <param name="comparer">The comparer to use in compare operation</param>
        public void Sort(int index, int count, IComparer<ListViewColumn> comparer)
        {
            columns.Sort(index, count, comparer);
        }
    }
}
