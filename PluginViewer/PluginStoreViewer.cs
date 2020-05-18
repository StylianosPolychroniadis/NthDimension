using NthDimension.Forms.Widgets;
using NthStudio.IoC;
using NthStudio.Plugins;
using System;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Xml;
using NthDimension.Forms.Events;
using NthDimension.Forms;

namespace PluginViewer
{
    [Plugin("PluginStore Viewer",
     "Displays the current plugin store in a tree view",
     "",
     "",
     "1.0")]
    public partial class PluginStoreViewer : BasePluginControl
    {
        private System.ComponentModel.IContainer components = null;

        const string PluginStoreFile = "./Plugins/PluginStore.xml";
        const string PluginStorePath = "./Plugins";

        private PluginStoreTreeView treeView;

        private bool m_init;

        public PluginStoreViewer()
        {
            
        }

        private void DisplayPluginStore()
        {
            XmlDocument store = new XmlDocument();
            store.Load(PluginStoreFile);

            treeView.Nodes.Clear();
            ConvertXmlNodeToTreeNode(store.SelectSingleNode("//PluginStore/Plugins"), treeView.Nodes);
        }


        private bool CheckForPluginStore()
        {
            string exe = System.Reflection.Assembly.GetExecutingAssembly().Location;
            Directory.SetCurrentDirectory(Path.GetDirectoryName(exe));

            if (!File.Exists(PluginStoreFile))
            {
                AddRootNode("No plugin store file found");
                return false;
            }
            return true;
        }

        private void AddRootNode(string text)
        {
            treeView.Nodes.Add(text);
        }

        private void ConvertXmlNodeToTreeNode(XmlNode xmlNode, TreeNodeCollection treeNodes)
        {
            if (xmlNode.Name.StartsWith("xml"))
                return;

            TreeNode newTreeNode = new TreeNode(xmlNode.Name);
            treeNodes.Add(newTreeNode);
            newTreeNode.Bounds.Inflate(1000, 0);

            switch (xmlNode.NodeType)
            {
                case XmlNodeType.ProcessingInstruction:
                case XmlNodeType.XmlDeclaration:
                case XmlNodeType.Text:
                case XmlNodeType.CDATA:
                case XmlNodeType.Comment:
                    break;
                case XmlNodeType.Element:
                    newTreeNode.Text = xmlNode.Name;
                    if (xmlNode.Attributes != null && xmlNode.Attributes["Name"] != null)
                    {
                        newTreeNode.Tag = new PluginElementData()
                        {
                            Name = xmlNode.Attributes["Name"].Value,
                            IconPath = xmlNode.Attributes["Icon"].Value
                        };
                    }
                    break;
                case XmlNodeType.Attribute:
                    newTreeNode.Tag = new AttributeData() { Name = xmlNode.Name, Value = xmlNode.Value };
                    break;
            }

            if (xmlNode.Attributes != null)
            {
                foreach (XmlAttribute attribute in xmlNode.Attributes)
                {
                    ConvertXmlNodeToTreeNode(attribute, newTreeNode.Nodes);
                }
            }

            if (xmlNode.NodeType != XmlNodeType.Attribute)
                foreach (XmlNode childNode in xmlNode.ChildNodes)
                {
                    ConvertXmlNodeToTreeNode(childNode, newTreeNode.Nodes);
                }
        }

        #region Designer.cs
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.treeView = new PluginStoreTreeView();
            SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.BGColor = System.Drawing.Color.Silver;
            //this.treeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView.Dock = NthDimension.Forms.EDocking.Fill;
            //this.treeView.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.treeView.Location = new System.Drawing.Point(0, 25);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(551, 251);
            this.treeView.TabIndex = 0;
            // 
            // PluginStoreViewer
            // 
            //this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BGColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Widgets.Add(this.treeView);
            this.Name = "PluginStoreViewer";
            this.Size = new System.Drawing.Size(551, 276);
            
            ResumeLayout();

        }

        #endregion
        #endregion

        protected override void DoPaint(PaintEventArgs e)
        {
            if(!m_init)
            {
                InitializeComponent();
                if (CheckForPluginStore())
                {
                    DisplayPluginStore();
                }
            }

            base.DoPaint(e);
        }
    }

    public class AttributeData
    {
        public string Name;
        public string Value;
    }

    public class PluginElementData
    {
        public string Name;
        public string IconPath;
    }

    /// <summary>
    /// A tree view customized to render plugin metadata
    /// from the plugin store
    /// </summary>
    public class PluginStoreTreeView : TreeView
    {
        Pen blackPen;
        ListDictionary IconCache = new ListDictionary();
        SolidBrush grayBrush = new SolidBrush(Color.DarkGray);

        public PluginStoreTreeView()
            : base()
        {
            //this.DrawMode = TreeViewDrawMode.OwnerDrawText;
            blackPen = new Pen(Color.Black);
        }

        /// <summary>
        /// Implements a cached bitmap library
        /// </summary>
        /// <param name="iconPath"></param>
        /// <returns></returns>
        public Bitmap GetIcon(string iconPath)
        {
            if (null == IconCache[iconPath])
            {
                if (File.Exists("." + iconPath))
                    IconCache[iconPath] = new Icon("." + iconPath).ToBitmap();
                else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + iconPath))
                {
                    IconCache[iconPath] = new Icon(AppDomain.CurrentDomain.BaseDirectory + iconPath).ToBitmap();
                }
                else
                    IconCache[iconPath] = string.Empty;
            }
            if (IconCache[iconPath] is Bitmap)
                return (Bitmap)IconCache[iconPath];
            return null;
        }


        protected override void DoPaint(PaintEventArgs e)
        {
            //// We need to work with customised nodes (ie Tag has been set) only
            //// and that too, only visible ones
            //if (e.Node.IsVisible && null != e.Node.Tag)
            //{
            //    // Do it only if visible custom rendering
            //    // only if the tree node is visible
            //    if (e.Node.Tag is AttributeData)
            //    {
            //        e.DrawDefault = false;
            //        AttributeData data = (AttributeData)e.Node.Tag;

            //        // Atttribute name
            //        e.Graphics.DrawString(data.Name, Font, new NanoSolidBrush(Color.DarkGray), e.Bounds.X, e.Bounds.Y);
            //        SizeF nameSize = NanoFont.MeasureText(data.Name, Font);  //e.Graphics.MeasureString(data.Name, Font);

            //        // Value
            //        e.Graphics.DrawString(data.Value, Font, new NanoSolidBrush(Color.Black), e.Bounds.X + nameSize.Width + 5, e.Bounds.Y);
            //    }
            //    else if (e.Node.Tag is PluginElementData)
            //    {
            //        e.DrawDefault = false;
            //        PluginElementData data = (PluginElementData)e.Node.Tag;

            //        // Icon
            //        Bitmap icon;
            //        if ((icon = GetIcon(data.IconPath)) != null)
            //        {
            //            e.Graphics.DrawImage(icon, e.Bounds.X, e.Bounds.Y, 16, 16);
            //        }

            //        // Name
            //        e.Graphics.DrawString(data.Name, Font, new NanoSolidBrush(Color.Black), e.Bounds.X + 20, e.Bounds.Y);
            //    }

            //}
            //else
            //{
            //    // Better let the control do the dirty work instead
            //    e.DrawDefault = true;
            //}

            base.DoPaint(e);
        }

       
    }
}
