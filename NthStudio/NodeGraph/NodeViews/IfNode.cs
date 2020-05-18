
using NthStudio.NodeGraph;
using NthStudio.NodeGraph.Xml;
using System.ComponentModel;
using System.Globalization;

namespace NthStudio.Nodes
{
	public class IfNode : NodeGraphNode
	{
		public enum IfNodeBehavior
		{
			ErrorOnMissingInput,
			ReturnDefaultValue
		}
		private float m_fDefaultValue;
		private IfNode.IfNodeBehavior m_eBehavior;
		[Category("If Node")]
		public float DefaultValue
		{
			get
			{
				return this.m_fDefaultValue;
			}
			set
			{
				this.m_fDefaultValue = value;
			}
		}
		[Category("If Node")]
		public IfNode.IfNodeBehavior Behavior
		{
			get
			{
				return this.m_eBehavior;
			}
			set
			{
				this.m_eBehavior = value;
			}
		}
		public IfNode(int p_X, int p_Y, NodeViewState p_View, bool p_CanBeSelected) : base(p_X, p_Y, p_View, p_CanBeSelected)
		{
			this.m_sName = "If condition";
			this.m_Plugs.Add(new NodeGraphPlug("A", this, ConnectorType.FluidInlet, 0, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("B", this, ConnectorType.FluidInlet, 1, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("A>B", this, ConnectorType.FluidInlet, 2, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("A=B", this, ConnectorType.FluidInlet, 3, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("A<B", this, ConnectorType.FluidInlet, 4, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Value", this, ConnectorType._GasOutlet, 0, "Float", LinkVisualStyle.BezierCurve));
			base.Height = 110;
			this.m_eBehavior = IfNode.IfNodeBehavior.ErrorOnMissingInput;
			this.m_fDefaultValue = 0f;
		}
		public IfNode(XmlTreeNode p_TreeNode, NodeViewState p_View) : base(p_TreeNode, p_View)
		{
			this.m_sName = "If condition";
			this.m_Plugs.Add(new NodeGraphPlug("A", this, ConnectorType.FluidInlet, 0, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("B", this, ConnectorType.FluidInlet, 1, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("A>B", this, ConnectorType.FluidInlet, 2, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("A=B", this, ConnectorType.FluidInlet, 3, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("A<B", this, ConnectorType.FluidInlet, 4, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Value", this, ConnectorType._GasOutlet, 0, "Float", LinkVisualStyle.BezierCurve));
			base.Height = 110;
			if (p_TreeNode.m_attributes["Behavior"] == "ErrorOnMissingInput")
			{
				this.m_eBehavior = IfNode.IfNodeBehavior.ErrorOnMissingInput;
			}
			else
			{
				this.m_eBehavior = IfNode.IfNodeBehavior.ReturnDefaultValue;
			}
			this.m_fDefaultValue = float.Parse(p_TreeNode.m_attributes["DefaultValue"], CultureInfo.GetCultureInfo("en-us"));
		}
		public override XmlTreeNode SerializeToXML(XmlTreeNode p_Parent)
		{
			XmlTreeNode xmlTreeNode = base.SerializeToXML(p_Parent);
			xmlTreeNode.AddParameter("Behavior", this.m_eBehavior.ToString());
			xmlTreeNode.AddParameter("DefaultValue", this.m_fDefaultValue.ToString(CultureInfo.GetCultureInfo("en-us")));
			return xmlTreeNode;
		}
		public override NodeGraphData Process()
		{
			NodeGraphDataInvalid nodeGraphInvalidData = new NodeGraphDataInvalid();
			bool flag = false;
			float num = 0f;
			float num2 = 0f;
			float p_Value = 0f;
			NodeGraphData nodeGraphData = this.m_Plugs[0].Process();
			NodeGraphData nodeGraphData2 = this.m_Plugs[1].Process();
			if (nodeGraphData is NodeGraphDataInvalid)
			{
				nodeGraphInvalidData.Merge(nodeGraphData as NodeGraphDataInvalid);
				flag = true;
			}
			if (nodeGraphData2 is NodeGraphDataInvalid)
			{
				nodeGraphInvalidData.Merge(nodeGraphData2 as NodeGraphDataInvalid);
				flag = true;
			}
			if (nodeGraphData is Float)
			{
				num = (nodeGraphData as Float).Value;
			}
			else
			{
				nodeGraphInvalidData.AddInvalidNode(this, "A Input is not NodeGraphFloatData");
				flag = true;
			}
			if (nodeGraphData2 is Float)
			{
				num2 = (nodeGraphData2 as Float).Value;
			}
			else
			{
				nodeGraphInvalidData.AddInvalidNode(this, "B Input is not NodeGraphFloatData");
				flag = true;
			}
			if (!flag)
			{
				if (num == num2)
				{
					NodeGraphData nodeGraphData3 = this.m_Plugs[3].Process();
					if (nodeGraphData3 is NodeGraphDataInvalid)
					{
						nodeGraphInvalidData.Merge(nodeGraphData3 as NodeGraphDataInvalid);
						flag = true;
					}
					else if (nodeGraphData3 is Float)
					{
						p_Value = (nodeGraphData3 as Float).Value;
					}
					else
					{
						nodeGraphInvalidData.AddInvalidNode(this, "A==B Input is not NodeGraphFloatData");
						flag = true;
					}
				}
				else if (num < num2)
				{
					NodeGraphData nodeGraphData3 = this.m_Plugs[4].Process();
					if (nodeGraphData3 is NodeGraphDataInvalid)
					{
						nodeGraphInvalidData.Merge(nodeGraphData3 as NodeGraphDataInvalid);
						flag = true;
					}
					else if (nodeGraphData3 is Float)
					{
						p_Value = (nodeGraphData3 as Float).Value;
					}
					else
					{
						nodeGraphInvalidData.AddInvalidNode(this, "A<B Input is not NodeGraphFloatData");
						flag = true;
					}
				}
				else
				{
					NodeGraphData nodeGraphData3 = this.m_Plugs[2].Process();
					if (nodeGraphData3 is NodeGraphDataInvalid)
					{
						nodeGraphInvalidData.Merge(nodeGraphData3 as NodeGraphDataInvalid);
						flag = true;
					}
					else if (nodeGraphData3 is Float)
					{
						p_Value = (nodeGraphData3 as Float).Value;
					}
					else
					{
						nodeGraphInvalidData.AddInvalidNode(this, "A>B Input is not NodeGraphFloatData");
						flag = true;
					}
				}
			}
			NodeGraphData result;
			if (flag)
			{
				if (this.m_eBehavior == IfNode.IfNodeBehavior.ErrorOnMissingInput)
				{
					result = nodeGraphInvalidData;
				}
				else
				{
					result = new Float(this.m_fDefaultValue);
				}
			}
			else
			{
				result = new Float(p_Value);
			}
			return result;
		}
	}
}
