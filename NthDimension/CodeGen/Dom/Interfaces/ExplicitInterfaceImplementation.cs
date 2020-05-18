namespace NthDimension.CodeGen.Dom
{
    using System;

    public sealed class ExplicitInterfaceImplementation : IEquatable<ExplicitInterfaceImplementation>
    {
        readonly IReturnType interfaceReference;
        readonly string memberName;

        public ExplicitInterfaceImplementation(IReturnType interfaceReference, string memberName)
        {
            this.interfaceReference = interfaceReference;
            this.memberName = memberName;
        }

        public IReturnType InterfaceReference
        {
            get { return interfaceReference; }
        }

        public string MemberName
        {
            get { return memberName; }
        }

        public ExplicitInterfaceImplementation Clone()
        {
            return this; // object is immutable, no Clone() required
        }

        public override int GetHashCode()
        {
            return interfaceReference.GetHashCode() ^ memberName.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ExplicitInterfaceImplementation);
        }

        public bool Equals(ExplicitInterfaceImplementation other)
        {
            if (other == null)
                return false;
            else
                return this.interfaceReference == other.interfaceReference && this.memberName == other.memberName;
        }
    }
}
