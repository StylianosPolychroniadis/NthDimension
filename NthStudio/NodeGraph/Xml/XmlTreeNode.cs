using System;
using System.Collections.Generic;
using System.Xml;
namespace NthStudio.NodeGraph.Xml
{
	public class XmlTreeNode
	{
		public string m_nodeName;
		public string m_value;
		public List<XmlTreeNode> m_childNodes;
		public XmlTreeNode m_parentNode;
		public Dictionary<string, string> m_attributes;
		public bool m_bIsTextOnly;
		public XmlTreeNode(string p_nodeName, XmlTreeNode p_parent)
		{
			this.m_nodeName = p_nodeName;
			this.m_childNodes = new List<XmlTreeNode>();
			this.m_parentNode = p_parent;
			this.m_attributes = new Dictionary<string, string>();
			this.m_bIsTextOnly = false;
			this.m_value = null;
		}
		public XmlTreeNode(XmlReader p_XmlReader, XmlTreeNode p_parent)
		{
			this.m_nodeName = p_XmlReader.Name;
			this.m_parentNode = p_parent;
			if (p_XmlReader.NodeType == XmlNodeType.Text)
			{
				this.m_bIsTextOnly = true;
				this.m_value = p_XmlReader.Value;
			}
			else
			{
				this.m_bIsTextOnly = false;
				this.m_childNodes = new List<XmlTreeNode>();
				this.m_attributes = new Dictionary<string, string>();
				if (p_XmlReader.HasAttributes)
				{
					while (p_XmlReader.MoveToNextAttribute())
					{
						this.m_attributes.Add(p_XmlReader.Name, p_XmlReader.Value);
					}
				}
				p_XmlReader.MoveToElement();
				if (!p_XmlReader.IsEmptyElement)
				{
					if (p_XmlReader.HasValue)
					{
						p_XmlReader.Read();
						this.m_value = p_XmlReader.Value;
					}
					p_XmlReader.Read();
					if (p_XmlReader != null)
					{
						while (p_XmlReader.NodeType != XmlNodeType.EndElement || !(p_XmlReader.Name == this.m_nodeName))
						{
							XmlTreeNode xmlTreeNode = new XmlTreeNode(p_XmlReader, this);
							if (xmlTreeNode.m_bIsTextOnly)
							{
								this.m_value = xmlTreeNode.m_value;
							}
							else
							{
								this.m_childNodes.Add(xmlTreeNode);
							}
							p_XmlReader.Read();
						}
					}
				}
			}
		}
		public void AddParameter(string p_paramName, string p_paramValue)
		{
			if (!this.m_attributes.ContainsKey(p_paramName))
			{
				this.m_attributes.Add(p_paramName, p_paramValue);
				return;
			}
			throw new XmlTreeNodeParameterAlreadyExistsException();
		}
		public XmlTreeNode AddChild(string p_nodename)
		{
			XmlTreeNode xmlTreeNode = new XmlTreeNode(p_nodename, this);
			this.m_childNodes.Add(xmlTreeNode);
			return xmlTreeNode;
		}
		public void AddChild(XmlTreeNode p_node)
		{
			this.m_childNodes.Add(p_node);
			p_node.m_parentNode = this;
		}
		public bool IsRoot()
		{
			return this.m_parentNode == null;
		}
		public bool HasChild()
		{
			return this.m_childNodes.Count > 0;
		}
		public bool HasChild(string p_NodeName)
		{
			bool result;
			if (this.m_childNodes.Count > 0)
			{
				bool flag = false;
				foreach (XmlTreeNode current in this.m_childNodes)
				{
					if (current.m_nodeName == p_NodeName)
					{
						result = true;
						return result;
					}
				}
				result = flag;
			}
			else
			{
				result = false;
			}
			return result;
		}
		public List<XmlTreeNode> GetTheseChildren(string p_Name)
		{
			List<XmlTreeNode> list = new List<XmlTreeNode>();
			foreach (XmlTreeNode current in this.m_childNodes)
			{
				if (current.m_nodeName == p_Name)
				{
					list.Add(current);
				}
			}
			return list;
		}
		public XmlTreeNode GetFirstChild(string p_Name)
		{
			XmlTreeNode result;
			foreach (XmlTreeNode current in this.m_childNodes)
			{
				if (current.m_nodeName == p_Name)
				{
					result = current;
					return result;
				}
			}
			result = null;
			return result;
		}
		public void WriteNodeXml(XmlWriter p_xWriter)
		{
			p_xWriter.WriteStartElement(this.m_nodeName);
			foreach (KeyValuePair<string, string> current in this.m_attributes)
			{
				p_xWriter.WriteStartAttribute(current.Key);
				p_xWriter.WriteValue(current.Value);
				p_xWriter.WriteEndAttribute();
			}
			if (this.m_value != null)
			{
				p_xWriter.WriteValue(this.m_value + "\n");
			}
			foreach (XmlTreeNode current2 in this.m_childNodes)
			{
				current2.WriteNodeXml(p_xWriter);
			}
			p_xWriter.WriteEndElement();
		}
	}
}
