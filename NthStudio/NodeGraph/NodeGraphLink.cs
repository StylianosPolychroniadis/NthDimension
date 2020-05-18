using NthStudio.NodeGraph.Xml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.NodeGraph
{
    public enum LinkVisualStyle
    {
        Direct,
        RectangleHorizontal,
        RectangleVertical,
        BezierCurve
    }

    public class NodeGraphLinkEventArgs : EventArgs
    {
        public List<NodeGraphLink> Links;
        public NodeGraphLinkEventArgs(List<NodeGraphLink> p_AffectedLinks)
        {
            this.Links = p_AffectedLinks;
        }
    }

    public class NodeGraphLink
    {
        private NodeGraphPlug _mInputPlug;
        private NodeGraphPlug _mOutputPlug;
        private NodeGraphDataType m_NodeGraphDataType;
        public NodeGraphPlug Input
        {
            get
            {
                return this._mInputPlug;
            }
        }
        public NodeGraphPlug Output
        {
            get
            {
                return this._mOutputPlug;
            }
        }
        public NodeGraphDataType NodeGraphDataType
        {
            get
            {
                return this.m_NodeGraphDataType;
            }
        }

        public System.Drawing.Point p1Offset = new Point(0, 0);
        public System.Drawing.Point p2Offset = new Point(0, 0);

        public System.Drawing.Point p1Collision = new Point();
        public System.Drawing.Point p2Collision = new Point();

        // TODO:: Add Points Here and Render to Allow User to modify them 

        public NodeGraphLink(NodeGraphPlug p_Input, NodeGraphPlug p_Output, NodeGraphDataType p_DataType)
        {
            this._mInputPlug = p_Input;
            this._mOutputPlug = p_Output;
            this.m_NodeGraphDataType = p_DataType;
        }
        public NodeGraphLink(XmlTreeNode p_TreeNode, NodeViewState p_View)
        {
            int index = int.Parse(p_TreeNode.m_attributes["InputNodeId"]);
            int index2 = int.Parse(p_TreeNode.m_attributes["OutputNodeId"]);
            int index3 = int.Parse(p_TreeNode.m_attributes["InputNodeConnectorIdx"]);
            int index4 = int.Parse(p_TreeNode.m_attributes["OutputNodeConnectorIdx"]);

            this._mInputPlug = p_View.NodeCollection[index].Plugs[index3];
            this._mOutputPlug = p_View.NodeCollection[index2].Plugs[index4];
            this.m_NodeGraphDataType = p_View.NodeCollection[index].Plugs[index3].DataType;
        }

        public static NodeGraphLink DeserializeFromXML(XmlTreeNode p_ObjectXml, NodeViewState p_View)
        {
            string nodeName = p_ObjectXml.m_nodeName;
            object[] args = new object[]
            {
                p_ObjectXml,
                p_View
            };
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            object obj = executingAssembly.CreateInstance(nodeName, false, BindingFlags.CreateInstance, null, args, CultureInfo.GetCultureInfo("en-us"), null);

            if (p_ObjectXml.m_attributes.ContainsKey("P1X") && p_ObjectXml.m_attributes.ContainsKey("P1Y"))
                ((NodeGraphLink)obj).p1Offset = new Point(int.Parse(p_ObjectXml.m_attributes["P1X"]), int.Parse(p_ObjectXml.m_attributes["P1Y"]));

            if (p_ObjectXml.m_attributes.ContainsKey("P2X") && p_ObjectXml.m_attributes.ContainsKey("P2Y"))
                ((NodeGraphLink)obj).p2Offset = new Point(int.Parse(p_ObjectXml.m_attributes["P2X"]), int.Parse(p_ObjectXml.m_attributes["P2Y"]));

            return obj as NodeGraphLink;
        }
        public XmlTreeNode SerializeToXML(XmlTreeNode p_XmlParentTreeNode)
        {
            XmlTreeNode xmlTreeNode = new XmlTreeNode(SerializationUtils.GetFullTypeName(this), p_XmlParentTreeNode);
            NodeViewState parentView = this.Input.Parent.ParentView;
            NodeGraphNode parent = this.Input.Parent;
            NodeGraphNode parent2 = this.Output.Parent;
            xmlTreeNode.AddParameter("InputNodeId", parentView.GetNodeIndex(parent).ToString());
            xmlTreeNode.AddParameter("OutputNodeId", parentView.GetNodeIndex(parent2).ToString());
            xmlTreeNode.AddParameter("InputNodeConnectorIdx", parent.GetConnectorIndex(this.Input).ToString());
            xmlTreeNode.AddParameter("OutputNodeConnectorIdx", parent2.GetConnectorIndex(this.Output).ToString());
            xmlTreeNode.AddParameter("P1X", p1Offset.X.ToString());
            xmlTreeNode.AddParameter("P1Y", p1Offset.Y.ToString());
            xmlTreeNode.AddParameter("P2X", p2Offset.X.ToString());
            xmlTreeNode.AddParameter("P2Y", p2Offset.Y.ToString());

            return xmlTreeNode;
        }
    }
}
