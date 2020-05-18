namespace NthDimension.CodeGen.Dom.Refactoring
{
    using System;

    public class CodeGeneratorOptions
    {
        public bool BracesOnSameLine = true;
        public bool EmptyLinesBetweenMembers = true;
        string indentString = "\t";

        public string IndentString
        {
            get { return indentString; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }
                indentString = value;
            }
        }
    }
}
