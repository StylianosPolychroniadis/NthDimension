namespace NthDimension.CodeGen.Dom
{
    using System;

    public interface IAttribute : IComparable
    {
        AttributeTarget AttributeTarget
        {
            get;
        }

        string Name
        {
            get;
        }
    }

    public enum AttributeTarget
    {
        None,
        Assembly,
        Field,
        Event,
        Method,
        Module,
        Param,
        Property,
        Return,
        Type
    }

    public struct AttributeArgument
    {
        public readonly IReturnType Type;
        public readonly object Value;

        public AttributeArgument(IReturnType type, object value)
        {
            this.Type = type;
            this.Value = value;
        }
    }
}
