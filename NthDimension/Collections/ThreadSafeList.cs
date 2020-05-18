using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace NthDimension.Collections
{
    public class ThreadSafeList<T> : IList<T>
    {
        readonly Lazy<ReaderWriterLockSlim> _locker = new Lazy<ReaderWriterLockSlim>(() => new ReaderWriterLockSlim());
        readonly List<T> _innerList = new List<T>();

        ReaderWriterLockSlim Locker
        {
            get { return _locker.Value; }
        }

        public int IndexOf(T item)
        {
            this.Locker.EnterReadLock();
            try
            {
                return _innerList.IndexOf(item);
            }
            finally
            {
                this.Locker.ExitReadLock();
            }
        }

        public void Insert(int index, T item)
        {
            this.Locker.EnterWriteLock();
            try
            {
                _innerList.Insert(index, item);
            }
            finally
            {
                this.Locker.ExitWriteLock();
            }
        }

        public void RemoveAt(int index)
        {
            this.Locker.EnterWriteLock();
            try
            {
                _innerList.RemoveAt(index);
            }
            finally
            {
                this.Locker.ExitWriteLock();
            }
        }

        public T this[int index]
        {
            get
            {
                this.Locker.EnterReadLock();
                try
                {
                    return _innerList[index];
                }
                finally
                {
                    this.Locker.ExitReadLock();
                }
            }
            set
            {
                this.Locker.EnterWriteLock();
                try
                {
                    _innerList[index] = value;
                }
                finally
                {
                    this.Locker.ExitWriteLock();
                }
            }
        }

        public void Add(T item)
        {
            this.Locker.EnterWriteLock();
            try
            {
                _innerList.Add(item);
            }
            finally
            {
                this.Locker.ExitWriteLock();
            }
        }

        public void AddRange(IEnumerable<T> collection)//(ICollection collection)
        {
            Locker.EnterWriteLock();
            try
            {
                _innerList.AddRange(collection);
            }
            finally
            {
                Locker.ExitWriteLock();
            }
        }

        public void RemoveRange(int index, int count)
        {
            Locker.EnterWriteLock();
            try
            {
                _innerList.RemoveRange(index, count);
            }
            finally
            {
                Locker.ExitWriteLock();
            }
        }

        public void Clear()
        {
            this.Locker.EnterWriteLock();
            try
            {
                _innerList.Clear();
            }
            finally
            {
                this.Locker.ExitWriteLock();
            }
        }

        public bool Contains(T item)
        {
            this.Locker.EnterReadLock();
            try
            {
                return _innerList.Contains(item);
            }
            finally
            {
                this.Locker.ExitReadLock();
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.Locker.EnterWriteLock();
            try
            {
                _innerList.CopyTo(array, arrayIndex);
            }
            finally
            {
                this.Locker.ExitWriteLock();
            }
        }

        public int Count
        {
            get
            {
                this.Locker.EnterReadLock();
                try
                {
                    return _innerList.Count;
                }
                finally
                {
                    this.Locker.ExitReadLock();
                }
            }
        }

        public bool IsReadOnly
        {
            get
            {
                this.Locker.EnterReadLock();
                try
                {
                    return ((ICollection<T>)_innerList).IsReadOnly;
                }
                finally
                {
                    this.Locker.ExitReadLock();
                }
            }
        }

        public bool Remove(T item)
        {
            this.Locker.EnterWriteLock();
            try
            {
                return _innerList.Remove(item);
            }
            finally
            {
                this.Locker.ExitWriteLock();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            this.Locker.EnterReadLock();
            try
            {
                foreach (var item in _innerList)
                    yield return item;
            }
            finally
            {
                this.Locker.ExitReadLock();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            this.Locker.EnterReadLock();
            try
            {
                foreach (var item in _innerList)
                    yield return item;
            }
            finally
            {
                this.Locker.ExitReadLock();
            }
        }
    }
}
