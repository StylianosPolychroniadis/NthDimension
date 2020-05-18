using System.ComponentModel;
using System.Globalization;

using NthStudio.NodeGraph;
using NthStudio.NodeGraph.Xml;

namespace NthStudio.Gui.NodeGraph.Model
{
    public class BoolNode : NodeGraphNode
    {
        private bool m_bValue;
        [Category("Bool Const Node")]
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
        public BoolNode(XmlTreeNode p_TreeNode, NodeViewState p_View) : base(p_TreeNode, p_View)
        {
            this.m_sName = "Bool: NAN";
            this.m_Plugs.Add(new NodeGraphPlug("Value", this, enuOPERATORS.Inlet, 0, "Boolean", LinkVisualStyle.BezierCurve));
            this.m_Plugs.Add(new NodeGraphPlug("Value", this, enuOPERATORS.Outlet, 0, "Boolean", LinkVisualStyle.BezierCurve));
            base.Width = 80;
            base.Height = 45;
            this.Value = false;
        }
        public BoolNode(int p_X, int p_Y, NodeViewState p_View, bool p_CanBeSelected) : base(p_X, p_Y, p_View, p_CanBeSelected)
        {
            this.m_sName = "Bool: NAN";
            this.m_Plugs.Add(new NodeGraphPlug("Value", this, enuOPERATORS.Inlet, 0, "Boolean", LinkVisualStyle.BezierCurve));
            this.m_Plugs.Add(new NodeGraphPlug("Value", this, enuOPERATORS.Outlet, 0, "Boolean", LinkVisualStyle.BezierCurve));
            base.Width = 80;
            base.Height = 45;
            this.Value = false;
        }
        protected override string GetName()
        {
            return "Bool: " + this.m_bValue.ToString();
        }
        public override NodeGraphData Process()
        {
            return new Data.BoolData(this.m_bValue);
        }
        public override XmlTreeNode SerializeToXML(XmlTreeNode p_Parent)
        {
            XmlTreeNode xmlTreeNode = base.SerializeToXML(p_Parent);
            xmlTreeNode.AddParameter("Value", this.Value.ToString(CultureInfo.GetCultureInfo("en-us")));
            return xmlTreeNode;
        }
    }
}
