using System;
using System.Collections.Generic;

namespace NthDimension.Collections
{
    /// <summary>
    /// Generic priority queue.  The Key value acts as the priority.  As items are added to the queue,
    /// they are sorted according to their priority.  A priority queue is faster than a SortedDictionary,
    /// but does not offer random access.
    /// </summary>
    public class PriorityQueue<TKey, TValue> where TKey : IComparable
    {
        List<KeyValuePair<TKey, TValue>> _heap = new List<KeyValuePair<TKey, TValue>>();
        int _heapCount = 0;
        int _compareMult = 1;

        /// <summary>
        /// Creates an empty priority queue
        /// </summary>
        /// <param name="ascending">If true then the Pop() and Peek() functions will always return
        /// the item with the lowest Key.  Otherwise Pop() and Peek() return the item
        /// with the highest Key</param>
        public PriorityQueue(bool ascending)
        {
            if (!ascending)
                _compareMult = -1;
        }

        public void Clear()
        {
            _heapCount = 0;
            _heap.Clear();
        }

        /// <summary>
        /// Adds an item to the priority queue.
        /// </summary>
        /// <param name="key">Key to use as the priority value</param>
        /// <param name="value"></param>
        public void Add(TKey key, TValue value)
        {
            if (_heap.Count == _heapCount)
                _heap.Add(new KeyValuePair<TKey, TValue>(key, value));
            else
                _heap[_heapCount] = new KeyValuePair<TKey, TValue>(key, value);
            _heapCount++;

            HeapBubbleUp(_heapCount - 1);
        }

        /// <summary>
        /// Gets the item at the front of the queue, but does not remove it
        /// </summary>
        public KeyValuePair<TKey, TValue> Peek()
        {
            return _heap[0];
        }

        /// <summary>
        /// Gets the item at the front of the queue and removes it from the queue
        /// </summary>
        public KeyValuePair<TKey, TValue> Pop()
        {
            KeyValuePair<TKey, TValue> result = _heap[0];
            _heapCount--;
            if (_heapCount > 0)
            {
                _heap[0] = _heap[_heapCount];

                int i = 0;
                while ((i << 1) + 1 < _heapCount)
                {
                    int j = (i << 1) + 1;
                    if (j + 1 < _heapCount && _heap[j + 1].Key.CompareTo(_heap[j].Key) * _compareMult < 0)
                        j++;
                    if (_heap[i].Key.CompareTo(_heap[j].Key) * _compareMult <= 0)
                        break;
                    KeyValuePair<TKey, TValue> temp = _heap[j];
                    _heap[j] = _heap[i];
                    _heap[i] = temp;
                    i = j;
                }
            }
            return result;
        }

        void HeapBubbleUp(int i)
        {
            while (i > 0)
            {
                int j = ((i + 1) >> 1) - 1;
                if (_heap[j].Key.CompareTo(_heap[i].Key) * _compareMult <= 0)
                    break;
                KeyValuePair<TKey, TValue> temp = _heap[j];
                _heap[j] = _heap[i];
                _heap[i] = temp;
                i = j;
            }
        }

        /// <summary>
        /// Gets the number of items remaining in the queue
        /// </summary>
        public int Count { get { return _heapCount; } }
    }
}
