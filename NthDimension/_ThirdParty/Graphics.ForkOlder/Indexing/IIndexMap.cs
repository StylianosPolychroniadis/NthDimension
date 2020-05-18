using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Graphics.Indexing
{
    // basic interface that allows mapping an index to another index
    public interface IIndexMap
    {
        int this[int index] { get; }
    }
}
