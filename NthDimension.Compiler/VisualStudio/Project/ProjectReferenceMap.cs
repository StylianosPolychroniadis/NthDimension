namespace NthStudio.Compiler.VisualStudio.Project
{
    using System.Collections.Generic;


    public class ProjectReferenceMap
    {
        // this has a different signature than the one below
        protected bool Equals(ProjectReferenceMap other)
        {
            return string.Equals(Name, other.Name) && string.Equals(File, other.File) && string.Equals(Assembly, other.Assembly);
        }

        //public override bool Equals(object obj)   
        //{
        //    return this.Equals(obj);
        //}

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (File != null ? File.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Assembly != null ? Assembly.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(ProjectReferenceMap left, ProjectReferenceMap right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ProjectReferenceMap left, ProjectReferenceMap right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return string.Format("{0} ({1}) {2}/{3}", Assembly, File, UsedBy.Count, Uses.Count);
        }

        public string Name { get; set; }
        public string File { get; set; }
        public string Assembly { get; set; }
        public List<ProjectReferenceMap> UsedBy { get; set; }
        public List<ReferenceEntry> Uses { get; set; }


    }
}
