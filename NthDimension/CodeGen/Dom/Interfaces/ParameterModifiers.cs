namespace NthDimension.CodeGen.Dom
{
    using System;

    [Serializable]
    [Flags]
    public enum ParameterModifiers : byte
    {
        // Values must be the same as in NRefactory's ParamModifiers
        None = 0,
        In = 1,
        Out = 2,
        Ref = 4,
        Params = 8,
        Optional = 16
    }
}
