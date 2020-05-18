using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NthDimension.CodeGen;
using NthDimension.Forms;
using NthDimension.Forms.Events;
using NthDimension.Forms.Widgets;

namespace NthStudio.Gui.Widgets.ObjectExplorer
{
    public class ProjectExplorer : Widget
    {
        ////List<ExplorerItem> explorerList = new List<ExplorerItem>();
        ////private void ReBuildObjectExplorer(string text)
        ////{
        ////    try
        ////    {
        ////        List<ExplorerItem> list = new List<ExplorerItem>();
        ////        int lastClassIndex = -1;
        ////        //find classes, methods and properties
        ////        Regex regex = new Regex(@"^(?<range>[\w\s]+\b(class|struct|enum|interface)\s+[\w<>,\s]+)|^\s*(public|private|internal|protected)[^\n]+(\n?\s*{|;)?", RegexOptions.Multiline);
        ////        foreach (Match r in regex.Matches(text))
        ////            try
        ////            {
        ////                string s = r.Value;
        ////                int i = s.IndexOfAny(new char[] { '=', '{', ';' });
        ////                if (i >= 0)
        ////                    s = s.Substring(0, i);
        ////                s = s.Trim();

        ////                var item = new ExplorerItem() { title = s, position = r.Index };
        ////                if (Regex.IsMatch(item.title, @"\b(class|struct|enum|interface)\b"))
        ////                {
        ////                    item.title = item.title.Substring(item.title.LastIndexOf(' ')).Trim();
        ////                    item.type = ExplorerItemType.Class;
        ////                    list.Sort(lastClassIndex + 1, list.Count - (lastClassIndex + 1), new ExplorerItemComparer());
        ////                    lastClassIndex = list.Count;
        ////                }
        ////                else
        ////                    if (item.title.Contains(" event "))
        ////                {
        ////                    int ii = item.title.LastIndexOf(' ');
        ////                    item.title = item.title.Substring(ii).Trim();
        ////                    item.type = ExplorerItemType.Event;
        ////                }
        ////                else
        ////                        if (item.title.Contains("("))
        ////                {
        ////                    var parts = item.title.Split('(');
        ////                    item.title = parts[0].Substring(parts[0].LastIndexOf(' ')).Trim() + "(" + parts[1];
        ////                    item.type = ExplorerItemType.Method;
        ////                }
        ////                else
        ////                            if (item.title.EndsWith("]"))
        ////                {
        ////                    var parts = item.title.Split('[');
        ////                    if (parts.Length < 2) continue;
        ////                    item.title = parts[0].Substring(parts[0].LastIndexOf(' ')).Trim() + "[" + parts[1];
        ////                    item.type = ExplorerItemType.Method;
        ////                }
        ////                else
        ////                {
        ////                    int ii = item.title.LastIndexOf(' ');
        ////                    item.title = item.title.Substring(ii).Trim();
        ////                    item.type = ExplorerItemType.Property;
        ////                }
        ////                list.Add(item);
        ////            }
        ////            catch {; }

        ////        list.Sort(lastClassIndex + 1, list.Count - (lastClassIndex + 1), new ExplorerItemComparer());

        ////        //BeginInvoke(
        ////            //new Action(() =>
        ////            //{
        ////                explorerList = list;
        ////                //.RowCount = explorerList.Count;
        ////                //dgvObjectExplorer.Invalidate();
        ////           //})
        ////        //);
        ////    }
        ////    catch {; }
        ////}


        public event EventHandler<ExplorerClickEventArgs>       FileClick;
        public event EventHandler<ExplorerLabelEditEventArgs>   FileNameChanged;
        public event EventHandler                               NewItemAdd;
        public event EventHandler                               FileItemDeleted;
        public event EventHandler                               AddRefrenceItem;
        public event EventHandler                               AddWebRefrenceItem;

        private bool                                            m_init = false;

        private ScriptLanguage                                  _scriptLanguage        = ScriptLanguage.CSharp;

        private TreeView                                        treeView;
        private TreeNode                                        rootNode;
        private TreeNode                                        referencesNode;

        private ContextMenuStrip                                cMenuProject;
        private ContextMenuStrip                                cMenuFile;
        private ContextMenuStrip                                cMenuFolder;

        private IToolStripItem tProjectAdd;

        private const string                                    rootNodeText           = "Project";
        private const string                                    referencesNodeText     = "References";



        public ProjectExplorer(bool lines = true, bool fullRow = true)
        {
            treeView                = new TreeView();
            treeView.Size           = new System.Drawing.Size(200, 350);
            treeView.Dock           = EDocking.Fill;
            treeView.ShowLines      = lines;
            treeView.FullRowSelect  = fullRow;
            treeView.FGColor        = System.Drawing.Color.WhiteSmoke;
            this.Widgets.Add(treeView);        
        }

        private void initializeTree()
        {
            treeView.Nodes.Clear();
            rootNode = new TreeNode(rootNodeText);
            rootNode.Expand();
            rootNode.Tag = NodeType.Project;

            referencesNode = new TreeNode(referencesNodeText);
            referencesNode.Tag = NodeType.References;

            rootNode.Nodes.Add(referencesNode);

            updateNode(rootNode);

            treeView.Nodes.AddRange(new TreeNode[]
            {
                rootNode
            });
        }
        private void TreeView_KeyUpEvent(object sender, NthDimension.Forms.Events.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                TreeNode node = treeView.SelectedNode;
                if (node != null)
                    StartLabelEdit(node);
            }
        }

        private void TreeView_NodeMouseClick(object sender, NthDimension.Forms.Events.TreeNodeMouseClickEventArgs e)
        {
            TreeNode node = e.Node;
            treeView.SelectedNode = node;

            ContextMenuStrip c = null;
            if (e.Button == NthDimension.MouseButton.Right && node != null)
            {
                NodeType t = (NodeType)node.Tag;
                switch (t)
                {
                    case NodeType.Project:
                        ////tProjectAdd.Visible = true;
                        ////tProjectBuild.Visible = true;
                        ////tProjectAdd.Visible = true;
                        ////tProjectClean.Visible = true;
                        ////tProjectProp.Visible = true;
                        ////tProjectSep1.Visible = true;
                        ////tProjectSep2.Visible = true;
                        ////tProjectSep3.Visible = true;
                        c = cMenuProject;
                        break;
                    case NodeType.References:
                        //tProjectAdd.Visible = false;
                        //tProjectBuild.Visible = false;
                        //tProjectAdd.Visible = false;
                        //tProjectClean.Visible = false;
                        //tProjectProp.Visible = false;
                        //tProjectSep1.Visible = false;
                        //tProjectSep2.Visible = false;
                        //tProjectSep3.Visible = false;
                        c = cMenuProject;
                        break;
                    case NodeType.Folder:
                        c = cMenuFolder;
                        break;
                    case NodeType.File:
                        //tFileCopy.Visible = true;
                        //tFileCut.Visible = true;
                        //tFileOpen.Visible = true;
                        //tFilePaste.Visible = true;
                        //tFileRename.Visible = true;
                        //tFileSep1.Visible = true;
                        //tFileSep2.Visible = true;
                        c = cMenuFile;
                        break;
                    case NodeType.Reference:
                        //tFileCopy.Visible = false;
                        //tFileCut.Visible = false;
                        //tFileOpen.Visible = false;
                        //tFilePaste.Visible = false;
                        //tFileRename.Visible = false;
                        //tFileSep1.Visible = false;
                        //tFileSep2.Visible = false;
                        c = cMenuFile;
                        break;
                }

                if (c != null)
                {
                    c.Show(this.WindowHUD/*, e.Location*/);
                }
            }
            updateNode(node);
        }

        private void TreeView_NodeMouseDoubleClick(object sender, NthDimension.Forms.Events.TreeNodeMouseClickEventArgs e)
        {
            NodeType t = (NodeType)e.Node.Tag;
            if (t == NodeType.File)
                OnFileClick(new ExplorerClickEventArgs(e.Node.Text));
            updateNodeImage(e.Node);
        }

        protected virtual void OnFileClick(ExplorerClickEventArgs e)
        {
            if (FileClick != null)
                FileClick(this, e);
        }

        private void updateNode(TreeNode node)
        {
            updateNodeImage(node);
            foreach (TreeNode n in treeView.Nodes)
                updateNodeImage(n);
        }

        private void updateNodeImage(TreeNode node)
        {
            NodeType t = (NodeType)node.Tag;
            switch (t)
            {
                case NodeType.Project:
                    node.ImageKey = (_scriptLanguage == ScriptLanguage.CSharp ? "CSharpProject.ico" : "VbProject.ico");
                    break;
                case NodeType.References:
                case NodeType.WebRererenceFolder:
                    node.ImageKey = (node.IsExpanded ? "ReferencesOpen.ico" : "ReferencesClosed.ico");
                    break;
                case NodeType.Folder:
                    node.ImageKey = (node.IsExpanded ? "OpenFolder.ico" : "ClosedFolder.ico");
                    break;
                case NodeType.File:
                    node.ImageKey = (_scriptLanguage == ScriptLanguage.CSharp ? "CSharpFile.ico" : "VbFile.ico");
                    node.Text = System.IO.Path.GetFileNameWithoutExtension(node.Text) + (_scriptLanguage == ScriptLanguage.CSharp ? ".cs" : ".vb");
                    break;
                case NodeType.Reference:
                    node.ImageKey = "Reference.ico";
                    break;
            }
            node.SelectedImageKey = node.ImageKey;
        }

        public void StartLabelEdit(TreeNode node)
        {
            NodeType t = (NodeType)node.Tag;
            if (t == NodeType.File || t == NodeType.Folder)
            {
                //treeView.LabelEdit = true;
                node.BeginEdit();
            }
        }

        private void InitializeComponent()
        {
            this.initializeTree();

            treeView.NodeMouseClick             += TreeView_NodeMouseClick;
            treeView.NodeMouseDoubleClick       += TreeView_NodeMouseDoubleClick;
            treeView.KeyUpEvent                 += TreeView_KeyUpEvent;

            m_init                              = true;
        }

        protected override void DoPaint(PaintEventArgs e)
        {
            if (!m_init)
                this.InitializeComponent();

            base.DoPaint(e);
        }

    }

    public class ExplorerClickEventArgs : EventArgs
    {
        public string FileName = "";
        public ExplorerClickEventArgs(string fileName)
        {
            FileName = fileName;
        }
    }

    public class ExplorerLabelEditEventArgs : EventArgs
    {
        public string OldName = "";
        public string NewName = "";
        public bool Cancel = false;
        public ExplorerLabelEditEventArgs(string newName, string oldName)
        {
            OldName = oldName;
            NewName = newName;
            Cancel = false;
        }
    }

    public enum NodeType
    {
        Project = 1,
        File,
        Folder,
        References,
        Reference,
        WebRererenceFolder
    }
}
