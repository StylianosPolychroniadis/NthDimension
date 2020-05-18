namespace NthDimension.CodeGen.Dom
{
    public interface IEvent : IMember
    {
        IMethod AddMethod
        {
            get;
        }

        IMethod RemoveMethod
        {
            get;
        }

        IMethod RaiseMethod
        {
            get;
        }
    }
}
