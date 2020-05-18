using System.Collections;
using System.Collections.Generic;

namespace NthDimension.Graphics.Indexing
{
    // An enumerator that enumerates over integers [start, start+count)
    // (useful when you need to do things like iterate over indices of an array rather than values)
    public class IndexRangeEnumerator : IEnumerable<int>
    {
        int Start = 0;
        int Count = 0;
        public IndexRangeEnumerator(int count) { Count = count; }
        public IndexRangeEnumerator(int start, int count) { Start = start; Count = count; }
        public IEnumerator<int> GetEnumerator()
        {
            for (int i = 0; i < Count; ++i)
                yield return Start + i;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
