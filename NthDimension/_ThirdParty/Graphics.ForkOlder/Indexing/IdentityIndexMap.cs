
namespace NthDimension.Graphics.Indexing
{
    // i = i index map
    public class IdentityIndexMap : IIndexMap
    {
        public int this[int index]
        {
            get { return index; }
        }
    }
}
