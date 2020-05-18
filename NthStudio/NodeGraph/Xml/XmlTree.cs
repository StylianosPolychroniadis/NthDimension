using System;
using System.IO;
using System.Xml;
namespace NthStudio.NodeGraph.Xml
{
	public class XmlTree
	{
		public XmlTreeNode m_rootNode;
		public XmlTree(string p_RootName)
		{
			this.m_rootNode = new XmlTreeNode(p_RootName, null);
		}
		public void SaveXML(string p_filename)
		{
			XmlWriter xmlWriter = XmlWriter.Create(p_filename, new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "     "
			});
			this.m_rootNode.WriteNodeXml(xmlWriter);
			xmlWriter.Flush();
			xmlWriter.Close();
		}
		public void SaveXML(Stream p_Stream)
		{
			XmlWriter xmlWriter = XmlWriter.Create(p_Stream, new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "     "
			});
			this.m_rootNode.WriteNodeXml(xmlWriter);
			xmlWriter.Flush();
			xmlWriter.Close();
		}
		public void LoadXML(string p_filename)
		{
			try
			{
				XmlReader xmlReader = XmlReader.Create(p_filename, new XmlReaderSettings
				{
					IgnoreWhitespace = true,
					IgnoreComments = true
				});
				if (!xmlReader.IsEmptyElement)
				{
					xmlReader.Read();
					if (xmlReader.NodeType == XmlNodeType.XmlDeclaration)
					{
						xmlReader.Read();
					}
					this.m_rootNode = new XmlTreeNode(xmlReader, null);
				}
				this.m_rootNode.GetType();
				xmlReader.Close();
			}
			catch (Exception ex)
			{
				ex.GetHashCode();
			}
		}
		public void LoadXML(Stream p_Stream)
		{
			XmlReader xmlReader = XmlReader.Create(p_Stream, new XmlReaderSettings
			{
				IgnoreWhitespace = true,
				IgnoreComments = true
			});
			if (!xmlReader.IsEmptyElement)
			{
				xmlReader.Read();
				if (xmlReader.NodeType == XmlNodeType.XmlDeclaration)
				{
					xmlReader.Read();
				}
				this.m_rootNode = new XmlTreeNode(xmlReader, null);
			}
			this.m_rootNode.GetType();
			xmlReader.Close();
		}
		public static XmlTree FromFile(string p_FileName)
		{
			XmlTree xmlTree = new XmlTree("Temporary");
			xmlTree.LoadXML(p_FileName);
			return xmlTree;
		}
	}
}
