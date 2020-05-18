using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Graphics.Indexing
{
    // dense or sparse index map
    public class IndexMap : IIndexMap
    {
        // this is returned if sparse map doesn't contain value
        public readonly int InvalidIndex = int.MinValue;


        int[] dense_map;
        Dictionary<int, int> sparse_map;
        int MaxIndex;

        public IndexMap(bool bForceSparse, int MaxIndex = -1)
        {
            if (bForceSparse)
            {
                sparse_map = new Dictionary<int, int>();
            }
            else
            {
                dense_map = new int[MaxIndex];
            }
            this.MaxIndex = MaxIndex;
            SetToInvalid();
        }

        public IndexMap(int[] use_dense_map, int MaxIndex = -1)
        {
            dense_map = use_dense_map;
            this.MaxIndex = MaxIndex;
        }


        public IndexMap(int MaxIndex, int SubsetCountEst)
        {
            bool bSmall = MaxIndex < 32000;        // if buffer is less than 128k, just use dense map
            float fPercent = (float)SubsetCountEst / (float)MaxIndex;
            float fPercentThresh = 0.1f;

            if (bSmall || fPercent > fPercentThresh)
            {
                dense_map = new int[MaxIndex];
            }
            else
            {
                sparse_map = new Dictionary<int, int>();
            }
            this.MaxIndex = MaxIndex;
            SetToInvalid();
        }


        // no effect on sparse map
        public void SetToInvalid()
        {
            if (dense_map != null)
            {
                for (int i = 0; i < dense_map.Length; ++i)
                    dense_map[i] = InvalidIndex;
            }
        }


        // dense variant: returns true unless you have set index to InvalidIndex (eg via SetToInvalid)
        // sparse variant: returns true if index is in map
        // either: returns false if index is out-of-bounds
        public bool Contains(int index)
        {
            if (MaxIndex > 0 && index >= MaxIndex)
                return false;
            if (dense_map != null)
                return dense_map[index] != InvalidIndex;
            else
                return sparse_map.ContainsKey(index);
        }



        public int this[int index]
        {
            get
            {
                if (dense_map != null)
                    return dense_map[index];
                else
                {
                    int to;
                    if (sparse_map.TryGetValue(index, out to))
                        return to;
                    return InvalidIndex;
                }
            }
            set
            {
                if (dense_map != null)
                {
                    dense_map[index] = value;
                }
                else
                {
                    sparse_map[index] = value;
                }
            }
        }

    }
}
