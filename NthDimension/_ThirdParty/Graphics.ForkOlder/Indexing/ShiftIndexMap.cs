namespace NthDimension.Graphics.Indexing
{
    // i = i + constant index map
    public class ShiftIndexMap : IIndexMap
    {
        public int Shift;

        public ShiftIndexMap(int n)
        {
            Shift = n;
        }

        public int this[int index]
        {
            get { return index + Shift; }
        }
    }
}
