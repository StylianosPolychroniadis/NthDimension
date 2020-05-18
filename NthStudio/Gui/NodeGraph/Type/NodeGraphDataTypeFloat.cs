
using System.Drawing;
using NthStudio.NodeGraph;

namespace NthStudio.Gui.NodeGraph.Type
{
    public class NodeGraphDataTypeFloat : NodeGraphDataType
    {
        public NodeGraphDataTypeFloat()
        {
            this.m_LinkPen = new Pen(Color.FromArgb(255, 172, 54));
            this.m_LinkArrowBrush = new SolidBrush(Color.FromArgb(255, 172, 54));
            this.m_ConnectorOutlinePen = new Pen(Color.FromArgb(255, 172, 54));
            this.m_ConnectorFillBrush = new SolidBrush(Color.FromArgb(248, 146, 0));
            this.m_TypeName = "Float";
        }
        public override string ToString()
        {
            return this.m_TypeName;
        }
    }
}
