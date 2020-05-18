using System.Drawing;
using NthStudio.NodeGraph;


namespace NthStudio.Gui.NodeGraph.Type
{
    public class NodeGraphDataTypeBool : NodeGraphDataType
    {
        public NodeGraphDataTypeBool()
        {
            this.m_LinkPen = new Pen(Color.FromArgb(216, 0, 255));
            this.m_LinkArrowBrush = new SolidBrush(Color.FromArgb(216, 0, 255));
            this.m_ConnectorOutlinePen = new Pen(Color.FromArgb(216, 0, 255));
            this.m_ConnectorFillBrush = new SolidBrush(Color.FromArgb(125, 0, 147));
            this.m_TypeName = "Boolean";
        }
        public override string ToString()
        {
            return this.m_TypeName;
        }
    }
}
