
using NthStudio.NodeGraph;
using NthStudio.NodeGraph.Xml;

namespace NthStudio.Gui.Nodegraph.Model
{ 
	public class SwitchFloatNode : NodeGraphNode
	{
		public SwitchFloatNode(int p_X, int p_Y, NodeViewState p_View, bool p_CanBeSelected) : base(p_X, p_Y, p_View, p_CanBeSelected)
		{
			this.m_sName = "Float switch (switch?A:B)";
			this.m_Plugs.Add(new NodeGraphPlug("A (true)", this, ConnectorType.FluidInletTop, 0, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("B (false)", this, ConnectorType.FluidInletTop, 1, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Switch", this, ConnectorType.FluidInletTop, 2, "Boolean", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Result", this, ConnectorType.FluidOutletBottom, 0, "Float", LinkVisualStyle.BezierCurve));
			base.Height = 80;
		}
		public SwitchFloatNode(XmlTreeNode p_TreeNode, NodeViewState p_View) : base(p_TreeNode, p_View)
		{
			this.m_sName = "Float switch (switch?A:B)";
			this.m_Plugs.Add(new NodeGraphPlug("A (true)", this, ConnectorType.FluidInletTop, 0, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("B (false)", this, ConnectorType.FluidInletTop, 1, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Switch", this, ConnectorType.FluidInletTop, 2, "Boolean", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Result", this, ConnectorType.FluidOutletBottom, 0, "Float", LinkVisualStyle.BezierCurve));
			base.Height = 80;
		}
		public override NodeGraphData Process()
		{
			float value = (this.m_Plugs[0].Process() as Float).Value;
			float value2 = (this.m_Plugs[1].Process() as Float).Value;
			bool value3 = (this.m_Plugs[2].Process() as Bool).Value;
			NodeGraphData result;
			if (value3)
			{
				result = new Float(value);
			}
			else
			{
				result = new Float(value2);
			}
			return result;
		}
	}
}
