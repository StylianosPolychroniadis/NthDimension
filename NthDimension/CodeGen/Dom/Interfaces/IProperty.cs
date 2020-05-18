namespace NthDimension.CodeGen.Dom
{
    public interface IProperty : IMethodOrProperty
    {
        DomRegion GetterRegion
        {
            get;
        }

        DomRegion SetterRegion
        {
            get;
        }

        bool CanGet
        {
            get;
        }

        bool CanSet
        {
            get;
        }

        bool IsIndexer
        {
            get;
        }
    }
}
