using System.Drawing;

namespace NthStudio.NodeGraph.Types
{
    public class NodeGraphDataTypeDefault : NodeGraphDataType
    {
        public NodeGraphDataTypeDefault()
        {
            this.m_LinkPen = new Pen(Color.FromArgb(120, 120, 120));
            this.m_LinkArrowBrush = new SolidBrush(Color.FromArgb(120, 120, 120));
            this.m_ConnectorOutlinePen = new Pen(Color.FromArgb(60, 60, 60));
            this.m_ConnectorFillBrush = new SolidBrush(Color.FromArgb(40, 40, 40));
            this.m_TypeName = "Generic";
        }
        public override string ToString()
        {
            return this.m_TypeName;
        }
    }
}
