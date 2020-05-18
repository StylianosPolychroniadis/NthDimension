using NthStudio.NodeGraph;
using NthStudio.NodeGraph.Xml;
using System.Drawing;
 
namespace NthStudio.Gui.NodeGraph.Model
{


    public class MaterialNode : NodeGraphNode
    {


        

        public MaterialNode(int p_X, int p_Y, NodeViewState p_View, bool p_CanBeSelected)
            : base(p_X, p_Y, p_View, p_CanBeSelected)
        {
            this.m_sName = "Material";

            base.Width = 100;
            base.Height = 50;

            this.m_Plugs.Add(new NodeGraphPlug("", this, enuOPERATORS.Inlet, 0, "Diffuse", LinkVisualStyle.BezierCurve));
            this.m_Plugs.Add(new NodeGraphPlug("", this, enuOPERATORS.Inlet, 0, "Normal", LinkVisualStyle.BezierCurve));
            this.m_Plugs.Add(new NodeGraphPlug("", this, enuOPERATORS.Inlet, 0, "Specular", LinkVisualStyle.BezierCurve));
            this.m_Plugs.Add(new NodeGraphPlug("", this, enuOPERATORS.Inlet, 0, "Emissive", LinkVisualStyle.BezierCurve));
        }
        public MaterialNode(XmlTreeNode p_Input, NodeViewState p_View)
            : base(p_Input, p_View)
        {
            this.m_sName = "Material";

            base.Width = 100;
            base.Height = 50;

            this.m_Plugs.Add(new NodeGraphPlug("", this, enuOPERATORS.Inlet, 0, "Diffuse", LinkVisualStyle.BezierCurve));
            this.m_Plugs.Add(new NodeGraphPlug("", this, enuOPERATORS.Inlet, 0, "Normal", LinkVisualStyle.BezierCurve));
            this.m_Plugs.Add(new NodeGraphPlug("", this, enuOPERATORS.Inlet, 0, "Specular", LinkVisualStyle.BezierCurve));
            this.m_Plugs.Add(new NodeGraphPlug("", this, enuOPERATORS.Inlet, 0, "Emissive", LinkVisualStyle.BezierCurve));
        }
    }
}
