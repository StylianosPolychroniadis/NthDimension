using NthStudio.NodeGraph.Types;
using NthStudio.NodeGraph.Xml;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;

namespace NthStudio.NodeGraph
{
    public class NodeViewState
    {
        private List<NodeGraphNode> m_NodeCollection;
        private List<NodeGraphNode> m_SelectedItems;
        private List<NodeGraphLink> m_Links;
        private Dictionary<string, NodeGraphDataType> m_KnownDataTypes;
        private NodeGraphPanel m_oPanel;

        #region Ctor
        public NodeViewState(NodeGraphPanel p_Panel)
        {
            this.m_NodeCollection = new List<NodeGraphNode>();
            this.m_SelectedItems = new List<NodeGraphNode>();
            this.m_Links = new List<NodeGraphLink>();
            this.m_KnownDataTypes = new Dictionary<string, NodeGraphDataType>();
            this.RegisterDataType(new NodeGraphDataTypeDefault());
            this.ViewX = 0;
            this.ViewY = 0;
            this.ViewZoom = 1f;
            this.CurrentViewZoom = 1f;
            this.m_oPanel = p_Panel;
        }
        public NodeViewState(XmlTreeNode p_XmlTreeNode, NodeGraphPanel p_Panel)
        {
            CultureInfo provider = new CultureInfo("en-us");

            if (null == m_KnownDataTypes)
                m_KnownDataTypes = new Dictionary<string, NodeGraphDataType>();

            this.registerAllTypes();

            this.m_oPanel = p_Panel;
            this.ViewX = int.Parse(p_XmlTreeNode.m_attributes["ViewX"]);
            this.ViewY = int.Parse(p_XmlTreeNode.m_attributes["ViewY"]);
            this.ViewZoom = float.Parse(p_XmlTreeNode.m_attributes["ViewZoom"], provider);

            this.m_NodeCollection = new List<NodeGraphNode>();

            XmlTreeNode firstChild = p_XmlTreeNode.GetFirstChild("NodeGraphNodeCollection");


            foreach (XmlTreeNode current in firstChild.m_childNodes)
            {
                this.m_NodeCollection.Add(NodeGraphNode.DeserializeFromXML(current, this));
            }

            this.m_Links = new List<NodeGraphLink>();

            XmlTreeNode firstChild2 = p_XmlTreeNode.GetFirstChild("NodeGraphLinkCollection");

            foreach (XmlTreeNode current2 in firstChild2.m_childNodes)
            {
                this.m_Links.Add(NodeGraphLink.DeserializeFromXML(current2, this));
            }

            this.m_SelectedItems = new List<NodeGraphNode>();
        }

        /// <summary>
        /// TODO:: Investigate if function can be removed
        /// </summary>
        public void registerAllTypes()
        {
            this.RegisterDataType(new Gui.NodeGraph.Type.NodeGraphDataTypeBool());
            this.RegisterDataType(new Gui.NodeGraph.Type.NodeGraphDataTypeFloat());
            //this.RegisterDataType(new NodeGraphDataTypeMaterial());
            //this.RegisterDataType(new NodeGraphDataTypeFluid());
            //this.RegisterDataType(new NodeGraphDataTypeGas());
            //this.RegisterDataType(new NodeGraphDataTypeAir());
            //this.RegisterDataType(new NodeGraphDataTypeSpray());
        }
        #endregion

        public int ViewX;
        public int ViewY;
        public float ViewZoom;
        public float CurrentViewZoom;

        public List<NodeGraphNode> NodeCollection
        {
            get
            {
                return this.m_NodeCollection;
            }
            set
            {
                this.m_NodeCollection = value;
            }
        }
        public List<NodeGraphNode> SelectedItems
        {
            get
            {
                return this.m_SelectedItems;
            }
            set
            {
                this.m_SelectedItems = value;
            }
        }
        public SelectedLine SelectedLine { get; set; }
        public List<NodeGraphLink> Links
        {
            get
            {
                return this.m_Links;
            }
            set
            {
                this.m_Links = value;
            }
        }
        public NodeGraphPanel ParentPanel
        {
            get
            {
                return this.m_oPanel;
            }
            set
            {
                this.m_oPanel = value;
            }
        }
        public Dictionary<string, NodeGraphDataType> KnownDataTypes
        {
            get
            {
                return this.m_KnownDataTypes;
            }
        }

        public int GetNodeIndex(NodeGraphNode p_Node)
        {
            int result;
            for (int i = 0; i < this.m_NodeCollection.Count; i++)
            {
                if (this.m_NodeCollection[i] == p_Node)
                {
                    result = i;
                    return result;
                }
            }
            result = -1;
            return result;
        }
        public int GetSelectionNodeIndex(NodeGraphNode p_Node)
        {
            int result;
            for (int i = 0; i < this.m_SelectedItems.Count; i++)
            {
                if (this.m_SelectedItems[i] == p_Node)
                {
                    result = i;
                    return result;
                }
            }
            result = -1;
            return result;
        }
        public void RegisterDataType(NodeGraphDataType p_DataType)
        {
            this.m_KnownDataTypes.Add(p_DataType.ToString(), p_DataType);
        }
        public void UnRegisterAllDataTypes()
        {
            this.m_KnownDataTypes.Clear();
        }
        public XmlTreeNode SerializeToXML(XmlTreeNode p_Parent)
        {
            XmlTreeNode xmlTreeNode = new XmlTreeNode(SerializationUtils.GetFullTypeName(this), p_Parent);
            xmlTreeNode.AddParameter("ViewX", this.ViewX.ToString());
            xmlTreeNode.AddParameter("ViewY", this.ViewY.ToString());
            xmlTreeNode.AddParameter("ViewZoom", this.ViewZoom.ToString(CultureInfo.GetCultureInfo("en-us")));
            XmlTreeNode xmlTreeNode2 = xmlTreeNode.AddChild("NodeGraphNodeCollection");
            foreach (NodeGraphNode current in this.NodeCollection)
            {
                xmlTreeNode2.AddChild(current.SerializeToXML(xmlTreeNode2));
            }
            XmlTreeNode xmlTreeNode3 = xmlTreeNode.AddChild("NodeGraphLinkCollection");
            foreach (NodeGraphLink current2 in this.Links)
            {
                xmlTreeNode3.AddChild(current2.SerializeToXML(xmlTreeNode3));
            }
            return xmlTreeNode;
        }
        public void CopySelectionToClipboard()
        {
            XmlTree xmlTree = new XmlTree("NodeGraphCopy");
            XmlTreeNode xmlTreeNode = xmlTree.m_rootNode.AddChild("Nodes");
            XmlTreeNode xmlTreeNode2 = xmlTree.m_rootNode.AddChild("Links");
            foreach (NodeGraphNode current in this.m_SelectedItems)
            {
                xmlTreeNode.AddChild(current.SerializeToXML(xmlTree.m_rootNode));
            }
            foreach (NodeGraphLink current2 in this.m_Links)
            {
                if (this.m_SelectedItems.Contains(current2.Input.Parent) && this.m_SelectedItems.Contains(current2.Output.Parent))
                {
                    XmlTreeNode xmlTreeNode3 = new XmlTreeNode("ToBeRelinked", xmlTreeNode2);
                    xmlTreeNode3.AddParameter("InputNodeId", this.GetSelectionNodeIndex(current2.Input.Parent).ToString());
                    xmlTreeNode3.AddParameter("InputNodeConnectorIdx", current2.Input.Parent.GetConnectorIndex(current2.Input).ToString());
                    xmlTreeNode3.AddParameter("OutputNodeId", this.GetSelectionNodeIndex(current2.Output.Parent).ToString());
                    xmlTreeNode3.AddParameter("OutputNodeConnectorIdx", current2.Output.Parent.GetConnectorIndex(current2.Output).ToString());
                    xmlTreeNode2.AddChild(xmlTreeNode3);
                }
            }

#if WINFORMS
            Clipboard.Clear();
#endif 
            string text = Path.GetTempPath() + "NodeGraphClipboard.xml";
            xmlTree.SaveXML(text);

#if WINFORMS
            Clipboard.SetFileDropList(new StringCollection
            {
                text
            });
#endif
        }
        public void PasteSelectionFromClipBoard()
        {
#if WINFORMS
            if (Clipboard.ContainsFileDropList())
            {
                if (Clipboard.GetFileDropList().Contains(Path.GetTempPath() + "NodeGraphClipboard.xml"))
                {
                    XmlTree xmlTree = XmlTree.FromFile(Path.GetTempPath() + "NodeGraphClipboard.xml");
                    XmlTreeNode rootNode = xmlTree.m_rootNode;
                    XmlTreeNode firstChild = rootNode.GetFirstChild("Nodes");
                    XmlTreeNode firstChild2 = rootNode.GetFirstChild("Links");
                    int count = this.m_NodeCollection.Count;
                    foreach (XmlTreeNode current in firstChild.m_childNodes)
                    {
                        NodeGraphNode nodeGraphNode = NodeGraphNode.DeserializeFromXML(current, this);
                        nodeGraphNode.X += 10;
                        nodeGraphNode.Y += 10;
                        nodeGraphNode.UpdateHitRectangle();
                        this.NodeCollection.Add(nodeGraphNode);
                    }
                    foreach (XmlTreeNode current2 in firstChild2.m_childNodes)
                    {
                        int num = int.Parse(current2.m_attributes["InputNodeId"]);
                        int index = int.Parse(current2.m_attributes["InputNodeConnectorIdx"]);
                        int num2 = int.Parse(current2.m_attributes["OutputNodeId"]);
                        int index2 = int.Parse(current2.m_attributes["OutputNodeConnectorIdx"]);
                        this.m_Links.Add(new NodeGraphLink(this.m_NodeCollection[count + num].Plugs[index], this.m_NodeCollection[count + num2].Plugs[index2], this.m_NodeCollection[count + num].Plugs[index].DataType));
                    }
                }
                this.ParentPanel.Refresh();
            }
#endif
        }
    }
}
