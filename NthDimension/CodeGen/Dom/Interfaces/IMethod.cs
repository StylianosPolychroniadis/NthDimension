namespace NthDimension.CodeGen.Dom
{
    using System.Collections.Generic;

    public interface IMethodOrProperty : IMember
    {
        IList<IParameter> Parameters
        {
            get;
        }

        bool IsExtensionMethod
        {
            get;
        }
    }

    public interface IMethod : IMethodOrProperty
    {
        IList<ITypeParameter> TypeParameters
        {
            get;
        }

        bool IsConstructor
        {
            get;
        }
    }
}
