using System.Collections.Generic;

namespace NthStudio.NodeGraph
{
    public abstract class NodeGraphData
    {
    }

    public class NodeGraphDataInvalid : NodeGraphData
    {
        private List<string> m_lErrorMessages;
        private List<NodeGraphNode> m_lInvalidNodes;
        public List<NodeGraphNode> InvalidNodes
        {
            get
            {
                return this.m_lInvalidNodes;
            }
        }
        public List<string> ErrorMessages
        {
            get
            {
                return this.m_lErrorMessages;
            }
        }
        public NodeGraphDataInvalid()
        {
            this.m_lErrorMessages = new List<string>();
            this.m_lInvalidNodes = new List<NodeGraphNode>();
        }
        public NodeGraphDataInvalid(NodeGraphNode p_InvalidNode, string p_ErrorMessage)
        {
            this.m_lErrorMessages = new List<string>();
            this.m_lInvalidNodes = new List<NodeGraphNode>();
            this.AddInvalidNode(p_InvalidNode, p_ErrorMessage);
        }
        public void AddInvalidNode(NodeGraphNode p_InvalidNode, string p_ErrorMessage)
        {
            if (!this.m_lInvalidNodes.Contains(p_InvalidNode))
            {
                this.m_lInvalidNodes.Add(p_InvalidNode);
            }
            this.m_lErrorMessages.Add(p_InvalidNode.Name + ":" + p_ErrorMessage);
        }
        public void Merge(NodeGraphDataInvalid p_Data)
        {
            for (int i = 0; i < p_Data.InvalidNodes.Count; i++)
            {
                this.m_lInvalidNodes.Add(p_Data.InvalidNodes[i]);
                this.m_lErrorMessages.Add(p_Data.ErrorMessages[i]);
            }
        }
    }

}
