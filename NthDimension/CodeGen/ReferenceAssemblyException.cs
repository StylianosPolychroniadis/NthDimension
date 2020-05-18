namespace NthDimension.CodeGen
{
    using System;
    public class ReferenceAssemblyException : Exception
    {
        public ReferenceAssemblyException(string message, Exception inner):base(message, inner) { }
        public ReferenceAssemblyException(string message):base(message) { }
    }
}
