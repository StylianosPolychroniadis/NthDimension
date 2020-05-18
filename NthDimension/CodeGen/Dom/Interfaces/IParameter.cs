namespace NthDimension.CodeGen.Dom
{
    using System;
    using System.Collections.Generic;

    public interface IParameter : IComparable
    {
        string Name
        {
            get;
        }

        IReturnType ReturnType
        {
            get;
            set;
        }

        IList<IAttribute> Attributes
        {
            get;
        }

        ParameterModifiers Modifiers
        {
            get;
        }

        DomRegion Region
        {
            get;
        }

        string Documentation
        {
            get;
        }

        bool IsOut
        {
            get;
        }

        bool IsRef
        {
            get;
        }

        bool IsParams
        {
            get;
        }

        bool IsOptional
        {
            get;
        }
    }
}
