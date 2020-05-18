namespace NthDimension.CodeGen.Dom
{
    public sealed class FoldingRegion
    {
        string name;
        DomRegion region;

        public string Name
        {
            get
            {
                return name;
            }
        }

        public DomRegion Region
        {
            get
            {
                return region;
            }
        }

        public FoldingRegion(string name, DomRegion region)
        {
            this.name = name;
            this.region = region;
        }
    }
}
