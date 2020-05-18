
using NthDimension.Forms.Events;
using NthStudio.NodeGraph;
using NthStudio.NodeGraph.Xml;

namespace NthStudio.Nodes
{
	public class ESISystemNode : NodeGraphNode
	{
		private static uint _esisystemCount = 0u;
		public ESISystemNode(int p_X, int p_Y, NodeViewState p_View, bool p_CanBeSelected) : base(p_X, p_Y, p_View, p_CanBeSelected)
		{
			ESISystemNode._esisystemCount += 1u;
			this.m_sName = "ESI System";
			base.Height = 400;
			base.Width = 350;
			this.m_Plugs.Add(new NodeGraphPlug("Gross Power", this, ConnectorType.ReadInputConnector, 0, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("House Load", this, ConnectorType.ReadInputConnector, 1, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Circ Water or Makeup Flow", this, ConnectorType.ReadInputConnector, 2, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Sum of Pump Power", this, ConnectorType.ReadInputConnector, 3, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug(" Condensate Pump A", this, ConnectorType.ReadInputConnector, 4, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug(" Condensate Pump B", this, ConnectorType.ReadInputConnector, 5, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug(" Condensate Pump C", this, ConnectorType.ReadInputConnector, 6, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Circulatory Water Temp", this, ConnectorType.ReadInputConnector, 7, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Radiation & Convection Loss", this, ConnectorType.ReadInputConnector, 8, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Bottom Ash/Fuel Flow Ratio", this, ConnectorType.ReadInputConnector, 9, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Carbon/Bottom Ash Weight Ratio", this, ConnectorType.ReadInputConnector, 10, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Specific Heat of Bottom Ash", this, ConnectorType.ReadInputConnector, 11, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Dry Bottom Ash Temp", this, ConnectorType.ReadInputConnector, 12, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Dust/Fuel Flow Weight Ratio", this, ConnectorType.ReadInputConnector, 13, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Fly Ash/Fuel Flow Ratio", this, ConnectorType.ReadInputConnector, 14, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Speciic Heat of Flue Dust", this, ConnectorType.ReadInputConnector, 15, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Gas Temp at Flue Dust Coll. Point", this, ConnectorType.ReadInputConnector, 16, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Rejects/Fuel Flow", this, ConnectorType.ReadInputConnector, 17, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Rejects Heating Value", this, ConnectorType.ReadInputConnector, 18, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Carbon/Fly Ash Weight Ratio", this, ConnectorType.ReadInputConnector, 19, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Fuel Temp", this, ConnectorType.ReadInputConnector, 20, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Gross Power", this, ConnectorType.WriteOutputConnector, 0, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("House Load", this, ConnectorType.WriteOutputConnector, 1, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Circ Water or Makeup Flow", this, ConnectorType.WriteOutputConnector, 2, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Sum of Pump Power", this, ConnectorType.WriteOutputConnector, 3, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug(" Condensate Pump A", this, ConnectorType.WriteOutputConnector, 4, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug(" Condensate Pump B", this, ConnectorType.WriteOutputConnector, 5, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug(" Condensate Pump C", this, ConnectorType.WriteOutputConnector, 6, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Circulatory Water Temp", this, ConnectorType.WriteOutputConnector, 7, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Radiation & Convection Loss", this, ConnectorType.WriteOutputConnector, 8, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Bottom Ash/Fuel Flow Ratio", this, ConnectorType.WriteOutputConnector, 9, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Carbon/Bottom Ash Weight Ratio", this, ConnectorType.WriteOutputConnector, 10, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Specific Heat of Bottom Ash", this, ConnectorType.WriteOutputConnector, 11, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Dry Bottom Ash Temp", this, ConnectorType.WriteOutputConnector, 12, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Dust/Fuel Flow Weight Ratio", this, ConnectorType.WriteOutputConnector, 13, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Fly Ash/Fuel Flow Ratio", this, ConnectorType.WriteOutputConnector, 14, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Speciic Heat of Flue Dust", this, ConnectorType.WriteOutputConnector, 15, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Gas Temp at Flue Dust Coll. Point", this, ConnectorType.WriteOutputConnector, 16, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Rejects/Fuel Flow", this, ConnectorType.WriteOutputConnector, 17, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Rejects Heating Value", this, ConnectorType.WriteOutputConnector, 18, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Carbon/Fly Ash Weight Ratio", this, ConnectorType.WriteOutputConnector, 19, "Float", LinkVisualStyle.BezierCurve));
			this.m_Plugs.Add(new NodeGraphPlug("Fuel Temp", this, ConnectorType.WriteOutputConnector, 20, "Float", LinkVisualStyle.BezierCurve));
		}
		public ESISystemNode(XmlTreeNode p_Input, NodeViewState p_View) : base(p_Input, p_View)
		{
		}
		public override void Draw(PaintEventArgs e, bool useCustomPoints, bool showHeader)
		{
			base.Draw(e, useCustomPoints, true);
		}
	}
}
