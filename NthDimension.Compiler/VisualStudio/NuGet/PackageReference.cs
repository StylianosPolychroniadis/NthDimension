#define USE_NotifyObject

using System.Xml.Linq;

namespace NthStudio.Compiler.VisualStudio.NuGet
{
#if USE_NotifyObject
    using NthStudio.Compiler.Tools;
    public class PackageReference : NotifyObject
#else
    public class PackageReference
#endif
    {

#if USE_NotifyObject
#else
#endif
        private string              _name;
        private string              _version;
        private string              _fileName;
        private XDocument           _document;
        private XElement            _reference;
        private XAttribute          _versionAttribute;

        public string Name
        {
            get { return _name; }
#if USE_NotifyObject
            set { Update(ref _name, value); }
#else
            set { _name = value; }
#endif

        }

        public string Version
        {
            get { return _version; }
#if USE_NotifyObject
            set { Update(ref _version, value); }
#else
            set { _version = value; }
#endif

        }

        public string FileName
        {
            get { return _fileName; }
#if USE_NotifyObject
            set { Update(ref _fileName, value); }
#else
            set { _fileName = value; }
#endif

        }

        public XDocument Document
        {
            get { return _document; }
#if USE_NotifyObject
            set { Update(ref _document, value); }
#else
            set { _document = value; }
#endif

        }

        public XElement Reference
        {
            get { return _reference; }
#if USE_NotifyObject
            set { Update(ref _reference, value); }
#else
            set { _reference = value; }
#endif

        }

        public XAttribute VersionAttribute
        {
            get { return _versionAttribute; }
#if USE_NotifyObject
            set { Update(ref _versionAttribute, value); }
#else
            set { _versionAttribute = value; }
#endif

        }
    }
}
