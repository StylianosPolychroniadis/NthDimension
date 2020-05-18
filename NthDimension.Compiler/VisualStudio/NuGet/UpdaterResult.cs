using System.Collections.Generic;
using System.Xml.Linq;

namespace NthStudio.Compiler.VisualStudio.NuGet
{
    using NthStudio.Compiler.Tools;
    public class UpdaterResult : NotifyObject
    {
        private IList<XDocument> _documents;
        private IList<PackageReference> _references;
        private Dictionary<string, IList<PackageReference>> _groupedReferences;

        public IList<XDocument> Documents
        {
            get { return _documents; }
            set { Update(ref _documents, value); }
        }

        public IList<PackageReference> References
        {
            get { return _references; }
            set { Update(ref _references, value); }
        }

        public Dictionary<string, IList<PackageReference>> GroupedReferences
        {
            get { return _groupedReferences; }
            set { Update(ref _groupedReferences, value); }
        }

        public void Reset()
        {
            if(null != Documents) Documents.Clear();
            if(null != References) References.Clear();
            if(null != GroupedReferences) GroupedReferences.Clear();            
        }
    }
}
