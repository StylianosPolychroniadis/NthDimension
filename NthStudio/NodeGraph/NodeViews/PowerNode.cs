
using NthStudio.NodeGraph;
using NthStudio.NodeGraph.Xml;
using System;
namespace NthStudio.Nodes
{
	public class PowerNode : NodeGraphNode
	{
		public PowerNode(int p_X, int p_Y, NodeViewState p_View, bool p_CanBeSelected) : base(p_X, p_Y, p_View, p_CanBeSelected)
		{
			this.m_sName = "A^B";
			this.m_Plugs.Add(new NodeGraphPlug("A", this, ConnectorType.FluidInlet, 0, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("B", this, ConnectorType.FluidInlet, 1, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Result (A^B)", this, ConnectorType._GasOutlet, 0, "Float", LinkVisualStyle.BezierCurve));
			base.Height = 64;
		}
		public PowerNode(XmlTreeNode p_TreeNode, NodeViewState p_View) : base(p_TreeNode, p_View)
		{
			this.m_sName = "A^B";
			this.m_Plugs.Add(new NodeGraphPlug("A", this, ConnectorType.FluidInlet, 0, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("B", this, ConnectorType.FluidInlet, 1, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Result (A^B)", this, ConnectorType._GasOutlet, 0, "Float", LinkVisualStyle.BezierCurve));
			base.Height = 64;
		}
		public override NodeGraphData Process()
		{
			float value = (this.m_Plugs[0].Process() as Float).Value;
			float value2 = (this.m_Plugs[1].Process() as Float).Value;
			
            
            return new Float((float)Math.Pow((double)value, (double)value2));
		}
	}
}
