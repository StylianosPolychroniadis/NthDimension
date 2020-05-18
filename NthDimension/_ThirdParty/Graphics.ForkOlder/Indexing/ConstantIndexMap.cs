namespace NthDimension.Graphics.Indexing
{
    // i = constant index map
    public class ConstantIndexMap : IIndexMap
    {
        public int Constant;

        public ConstantIndexMap(int c)
        {
            Constant = c;
        }

        public int this[int index]
        {
            get { return Constant; }
        }
    }
}
