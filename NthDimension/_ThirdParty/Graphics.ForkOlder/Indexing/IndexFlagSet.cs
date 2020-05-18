using System.Collections;
using System.Collections.Generic;

namespace NthDimension.Graphics.Indexing
{
    /// <summary>
    /// This class provides a similar interface to BitArray, but can optionally
    /// use a HashSet (or perhaps some other DS) if the fraction of the index space 
    /// required is small
    /// </summary>
    public class IndexFlagSet : IEnumerable<int>
    {
        BitArray bits;
        HashSet<int> hash;
        int count;      // only tracked for bitset


        public IndexFlagSet(bool bForceSparse, int MaxIndex = -1)
        {
            if (bForceSparse)
            {
                hash = new HashSet<int>();
            }
            else
            {
                bits = new BitArray(MaxIndex);
            }
            count = 0;
        }

        public IndexFlagSet(int MaxIndex, int SubsetCountEst)
        {
            bool bSmall = MaxIndex < 128000;        // 16k in bits is a pretty small buffer?
            float fPercent = (float)SubsetCountEst / (float)MaxIndex;
            float fPercentThresh = 0.05f;

            if (bSmall || fPercent > fPercentThresh)
            {
                bits = new BitArray(MaxIndex);
            }
            else
                hash = new HashSet<int>();
            count = 0;
        }

        /// <summary>
        /// checks if value i is true
        /// </summary>
        public bool Contains(int i)
        {
            return this[i] == true;
        }

        /// <summary>
        /// sets value i to true
        /// </summary>
        public void Add(int i)
        {
            this[i] = true;
        }

        /// <summary>
        /// Returns number of true values in set
        /// </summary>
        public int Count
        {
            get
            {
                if (bits != null)
                    return count;
                else
                    return hash.Count;
            }
        }

        public bool this[int key]
        {
            get
            {
                return (bits != null) ? bits[key] : hash.Contains(key);
            }
            set
            {
                if (bits != null)
                {
                    if (bits[key] != value)
                    {
                        bits[key] = value;
                        if (value == false)
                            count--;
                        else
                            count++;
                    }
                }
                else
                {
                    if (value == true)
                        hash.Add(key);
                    else if (value == false && hash.Contains(key))
                        hash.Remove(key);
                }
            }
        }

        /// <summary>
        /// enumerate over indices w/ value = true
        /// </summary>
        public IEnumerator<int> GetEnumerator()
        {
            if (bits != null)
            {
                for (int i = 0; i < bits.Length; ++i)
                {
                    if (bits[i])
                        yield return i;
                }
            }
            else
            {
                foreach (int i in hash)
                    yield return i;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }



    }
}
