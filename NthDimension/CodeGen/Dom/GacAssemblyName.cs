namespace NthDimension.CodeGen.Dom
{
    using System;

    public class GacAssemblyName : IEquatable<GacAssemblyName>
    {
        readonly string fullName;
        readonly string[] info;

        public GacAssemblyName(string fullName)
        {
            if (fullName == null)
                throw new ArgumentNullException("fullName");
            this.fullName = fullName;
            info = fullName.Split(',');
        }

        public string Name
        {
            get
            {
                return info[0];
            }
        }

        public string Version
        {
            get
            {
                return (info.Length > 1) ? info[1].Substring(info[1].LastIndexOf('=') + 1) : null;
            }
        }

        public string PublicKey
        {
            get
            {
                return (info.Length > 3) ? info[3].Substring(info[3].LastIndexOf('=') + 1) : null;
            }
        }

        public string FullName
        {
            get { return fullName; }
        }



        public override string ToString()
        {
            return fullName;
        }

        public bool Equals(GacAssemblyName other)
        {
            if (other == null)
                return false;
            else
                return fullName == other.fullName;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as GacAssemblyName);
        }

        public override int GetHashCode()
        {
            return fullName.GetHashCode();
        }
    }
}
