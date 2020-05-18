namespace NthDimension.CodeGen.Dom
{
    public interface IField : IMember
    {
        /// <summary>Gets if this field is a local variable that has been converted into a field.</summary>
        bool IsLocalVariable { get; }

        /// <summary>Gets if this field is a parameter that has been converted into a field.</summary>
        bool IsParameter { get; }
    }
}
