using System.Collections.Generic;
using System.Xml.Serialization;

namespace NthStudio.Plugins
{
    [XmlRoot("PluginStore")]
    public class PluginStore
    {
        public List<PluginInfo> Plugins { get; set; }

        public PluginStore() { Plugins = new List<PluginInfo>(); }
    }
   
}
