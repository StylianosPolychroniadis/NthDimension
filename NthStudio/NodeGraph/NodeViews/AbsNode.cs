
using NthStudio.Nodes;
using NthStudio.NodeGraph;
using NthStudio.NodeGraph.Xml;
using System;


namespace NthStudio.Nodes
{
	public class AbsNode : NodeGraphNode
	{
		public AbsNode(int p_X, int p_Y, NodeViewState p_View, bool p_CanBeSelected) : base(p_X, p_Y, p_View, p_CanBeSelected)
		{
			this.m_sName = "Abs(A)";
			this.m_Plugs.Add(new NodeGraphPlug("A", this, ConnectorType.FluidInletTop, 0, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Result", this, ConnectorType.FluidOutletBottom, 0, "Float", LinkVisualStyle.BezierCurve));
			base.Height = 45;
		}
		public AbsNode(XmlTreeNode p_TreeNode, NodeViewState p_View) : base(p_TreeNode, p_View)
		{
			this.m_sName = "Abs(A)";
			this.m_Plugs.Add(new NodeGraphPlug("A", this, ConnectorType.FluidInletTop, 0, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Result", this, ConnectorType.FluidOutletBottom, 0, "Float", LinkVisualStyle.BezierCurve));
			base.Height = 45;
		}
		public override NodeGraphData Process()
		{
			float value = (this.m_Plugs[0].Process() as Float).Value;
			return new Float(Math.Abs(value));
		}
	}
}
