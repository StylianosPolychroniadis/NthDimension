using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.NodeGraph
{
	public class NodeGraphListData : NodeGraphData
	{
		private List<NodeGraphData> m_lData;
		public List<NodeGraphData> Data
		{
			get
			{
				return this.m_lData;
			}
		}
		public NodeGraphListData()
		{
			this.m_lData = new List<NodeGraphData>();
		}
		public void AddData(NodeGraphData p_Data)
		{
			this.m_lData.Add(p_Data);
		}
	}
}
