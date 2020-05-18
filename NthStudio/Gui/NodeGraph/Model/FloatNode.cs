using System.ComponentModel;
using System.Globalization;

using NthStudio.NodeGraph;
using NthStudio.NodeGraph.Xml;

namespace NthStudio.Gui.NodeGraph.Model
{
    public class FloatNode : NodeGraphNode
    {
        private float m_fValue;
        [Category("Float Const Node")]
        public float Value
        {
            get
            {
                return this.m_fValue;
            }
            set
            {
                this.m_fValue = value;
            }
        }
        public FloatNode(XmlTreeNode p_TreeNode, NodeViewState p_View) : base(p_TreeNode, p_View)
        {
            this.m_sName = "Float: NAN";
            this.m_Plugs.Add(new NodeGraphPlug("Value", this, enuOPERATORS.Outlet, 0, "Float", LinkVisualStyle.BezierCurve));
            base.Width = 80;
            base.Height = 45;
            this.Value = 0f;
        }
        public FloatNode(int p_X, int p_Y, NodeViewState p_View, bool p_CanBeSelected) : base(p_X, p_Y, p_View, p_CanBeSelected)
        {
            this.m_sName = "Float: NAN";
            this.m_Plugs.Add(new NodeGraphPlug("Value", this, enuOPERATORS.Outlet, 0, "Float", LinkVisualStyle.BezierCurve));
            base.Width = 80;
            base.Height = 45;
            this.Value = 0f;
        }
        protected override string GetName()
        {
            return "Float: " + this.m_fValue.ToString();
        }
        public override NodeGraphData Process()
        {
            return new Data.Float(this.m_fValue);
        }
        public override XmlTreeNode SerializeToXML(XmlTreeNode p_Parent)
        {
            XmlTreeNode xmlTreeNode = base.SerializeToXML(p_Parent);
            xmlTreeNode.AddParameter("Value", this.Value.ToString(CultureInfo.GetCultureInfo("en-us")));
            return xmlTreeNode;
        }
    }
}
