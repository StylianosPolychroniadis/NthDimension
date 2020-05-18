using System.Xml.Serialization;

namespace NthStudio.Plugins
{
    public class PluginInfo
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("Description")]
        public string Description { get; set; }

        [XmlAttribute("Type")]
        public string Type { get; set; }

        [XmlAttribute("AssemblyFile")]
        public string AssemblyFile { get; set; }

        [XmlAttribute("AssemblyVersion")]
        public string AssemblyVersion { get; set; }

        [XmlAttribute("AssemblyDate")]
        public string AssemblyDate { get; set; }

        [XmlAttribute("InstallPath")]
        public string InstallPath { get; set; }

        [XmlAttribute("Icon")]
        public string Icon { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
