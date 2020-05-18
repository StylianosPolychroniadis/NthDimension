using System.Collections.Generic;

namespace NthDimension.Graphics.Indexing
{
    // Add true/false operator[] to integer HashSet
    public class IndexHashSet : HashSet<int>
    {
        public bool this[int key]
        {
            get
            {
                return Contains(key);
            }
            set
            {
                if (value == true)
                    Add(key);
                else if (value == false && Contains(key))
                    Remove(key);
            }
        }
    }
}
