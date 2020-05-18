using System.ComponentModel;
using System.Globalization;
using System.Drawing;

using NthStudio.NodeGraph;
using NthStudio.NodeGraph.Xml;
using NthStudio.NodeGraph.Types;

namespace NthStudio.Gui.NodeGraph.Data
{
    public class BoolData : NodeGraphData
    {
        private bool m_bValue;
        public bool Value
        {
            get
            {
                return this.m_bValue;
            }
            set
            {
                this.m_bValue = value;
            }
        }
        public BoolData(bool p_Value)
        {
            this.m_bValue = p_Value;
        }
    }

   




}
