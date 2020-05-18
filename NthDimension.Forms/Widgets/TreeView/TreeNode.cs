using NthDimension.Forms.Events;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using System.Text;

namespace NthDimension.Forms.Widgets
{
    public class TreeNode : Widget, ICloneable, ISerializable
    {
        #region Fields

        private TreeViewCanvas tree_view;
        internal TreeNode parent;

        private string text;
        private int image_index = -1;
        private int selected_image_index = -1;
        private ContextMenuStrip context_menu;
        private ContextMenuStrip context_menu_strip;
        private string image_key = String.Empty;
        private string selected_image_key = String.Empty;
        private int state_image_index = -1;
        private string state_image_key = String.Empty;
        private string tool_tip_text = String.Empty;
        internal TreeNodeCollection nodes;
        internal TreeViewAction check_reason = TreeViewAction.Unknown;

        internal int visible_order = 0;
        internal int width = -1;

        internal bool is_expanded = false;
        private bool check;
        internal OwnerDrawPropertyBag prop_bag;

        private object tag;

        internal IntPtr handle;

        private string name = string.Empty;

        #endregion   // Fields

        #region Internal Constructors

        internal TreeNode(TreeViewCanvas tree_view) : this()
        {
            this.tree_view = tree_view;
            is_expanded = true;
        }

        protected TreeNode(SerializationInfo serializationInfo, StreamingContext context) : this()
        {
            SerializationInfoEnumerator en;
            SerializationEntry e;
            int children;

            en = serializationInfo.GetEnumerator();
            children = 0;
            while (en.MoveNext())
            {
                e = en.Current;
                switch (e.Name)
                {
                    case "Text": Text = (string)e.Value; break;
                    case "PropBag": prop_bag = (OwnerDrawPropertyBag)e.Value; break;
                    case "ImageIndex": image_index = (int)e.Value; break;
                    case "SelectedImageIndex": selected_image_index = (int)e.Value; break;
                    case "Tag": tag = e.Value; break;
                    case "IsChecked": check = (bool)e.Value; break;
                    case "ChildCount": children = (int)e.Value; break;
                }
            }
            if (children > 0)
            {
                for (int i = 0; i < children; i++)
                {
                    var node = (TreeNode)serializationInfo.GetValue("children" + i, typeof(TreeNode));
                    Nodes.Add(node);
                }
            }
        }
        #endregion // Internal Constructors

        #region Public Constructors

        public TreeNode()
        {
            nodes = new TreeNodeCollection(this);
        }

        public TreeNode(string text) : this()
        {
            Text = text;
        }

        public TreeNode(string text, TreeNode[] children) : this(text)
        {
            Nodes.AddRange(children);
        }

        public TreeNode(string text, int imageIndex, int selectedImageIndex) : this(text)
        {
            this.image_index = imageIndex;
            this.selected_image_index = selectedImageIndex;
        }

        public TreeNode(string text, int imageIndex, int selectedImageIndex,
                            TreeNode[] children)
            : this(text, imageIndex, selectedImageIndex)
        {
            Nodes.AddRange(children);
        }

        #endregion // Public Constructors

        #region ICloneable Members
        public virtual object Clone()
        {
            var tn = (TreeNode)Activator.CreateInstance(GetType());
            tn.name = name;
            tn.text = text;
            tn.image_key = image_key;
            tn.image_index = image_index;
            tn.selected_image_index = selected_image_index;
            tn.selected_image_key = selected_image_key;
            tn.state_image_index = state_image_index;
            tn.state_image_key = state_image_key;
            tn.tag = tag;
            tn.check = check;
            tn.tool_tip_text = tool_tip_text;
            tn.context_menu = context_menu;
            tn.context_menu_strip = context_menu_strip;
            if (nodes != null)
            {
                foreach (TreeNode child in nodes)
                    tn.nodes.Add((TreeNode)child.Clone());
            }
            if (prop_bag != null)
                tn.prop_bag = OwnerDrawPropertyBag.Copy(prop_bag);
            return tn;
        }

        #endregion // ICloneable Members

        #region ISerializable Members

        void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
        {
            si.AddValue("Text", Text);
            si.AddValue("prop_bag", prop_bag, typeof(OwnerDrawPropertyBag));
            si.AddValue("ImageIndex", ImageIndex);
            si.AddValue("SelectedImageIndex", SelectedImageIndex);
            si.AddValue("Tag", Tag);
            si.AddValue("Checked", Checked);

            si.AddValue("NumberOfChildren", Nodes.Count);
            for (int i = 0; i < Nodes.Count; i++)
                si.AddValue("Child-" + i, Nodes[i], typeof(TreeNode));
        }

        protected virtual void Deserialize(SerializationInfo serializationInfo, StreamingContext context)
        {
            Text = serializationInfo.GetString("Text");
            prop_bag = (OwnerDrawPropertyBag)serializationInfo.GetValue("prop_bag", typeof(OwnerDrawPropertyBag));
            ImageIndex = serializationInfo.GetInt32("ImageIndex");
            SelectedImageIndex = serializationInfo.GetInt32("SelectedImageIndex");
            Tag = serializationInfo.GetValue("Tag", typeof(Object));
            Checked = serializationInfo.GetBoolean("Checked");

            int count = serializationInfo.GetInt32("NumberOfChildren");

            for (int i = 0; i < count; i++)
                Nodes.Add((TreeNode)serializationInfo.GetValue("Child-" + i, typeof(TreeNode)));
        }

        protected virtual void Serialize(SerializationInfo si, StreamingContext context)
        {
            si.AddValue("Text", Text);
            si.AddValue("prop_bag", prop_bag, typeof(OwnerDrawPropertyBag));
            si.AddValue("ImageIndex", ImageIndex);
            si.AddValue("SelectedImageIndex", SelectedImageIndex);
            si.AddValue("Tag", Tag);
            si.AddValue("Checked", Checked);

            si.AddValue("NumberOfChildren", Nodes.Count);
            for (int i = 0; i < Nodes.Count; i++)
                si.AddValue("Child-" + i, Nodes[i], typeof(TreeNode));
        }
        #endregion // ISerializable Members

        #region Public Instance Properties

        public Color BackColor
        {
            get
            {
                if (prop_bag != null)
                    return BGColor = prop_bag.BackColor;
                return Color.Empty;
            }
            set
            {
                if (prop_bag == null)
                    prop_bag = new OwnerDrawPropertyBag();
                prop_bag.BackColor  = BGColor = value;

                TreeViewCanvas tree_view = TreeView;
                if (tree_view != null)
                    tree_view.UpdateNode(this);
            }
        }

        public Rectangle Bounds
        {
            get
            {
                if (TreeView == null)
                    return Rectangle.Empty;

                int x = GetX();
                int y = GetY();

                if (width == -1)
                    width = TreeView.GetNodeWidth(this);

                var res = new Rectangle(x, y, width, TreeView.ActualItemHeight);
                return res;
            }
        }

        internal int GetY()
        {
            if (TreeView == null)
                return 0;
            return (visible_order - 1) * TreeView.ActualItemHeight - (TreeView.skipped_nodes * TreeView.ActualItemHeight);
        }

        internal int GetX()
        {
            if (TreeView == null)
                return 0;
            int indent_level = IndentLevel;
            int roots = (TreeView.ShowRootLines ? 1 : 0);
            int cb = (TreeView.CheckBoxes ? 19 : 0);
            if (TreeView.CheckBoxes && StateImage != null)
                cb = 19;
            int imgs = (TreeView.ImageList != null ? TreeView.ImageList.ImageSize.Width + 3 : 0);
            return ((indent_level + roots) * TreeView.Indent) + cb + imgs - TreeView.hbar_offset;
        }

        internal int GetLinesX()
        {
            int roots = (TreeView.ShowRootLines ? 1 : 0);
            return (IndentLevel + roots) * TreeView.Indent - TreeView.hbar_offset;
        }

        internal int GetImageX()
        {
            return GetLinesX() + (TreeView.CheckBoxes || StateImage != null ? 19 : 0);
        }

        // In theory we should be able to track this instead of computing
        // every single time we need it, however for now I am going to
        // do it this way to reduce bugs in my new bounds computing code
        internal int IndentLevel
        {
            get
            {
                TreeNode walk = this;
                int res = 0;
                while (walk.Parent != null)
                {
                    walk = walk.Parent;
                    res++;
                }

                return res;
            }
        }

        public bool Checked
        {
            get { return check; }
            set
            {
                if (check == value)
                    return;
                var args = new TreeViewCancelEventArgs(this, false, check_reason);
                if (TreeView != null)
                    TreeView.OnBeforeCheck(args);
                if (!args.Cancel)
                {
                    check = value;

                    // TreeViewCanvas can become null after OnAfterCheck, this the double null check
                    if (TreeView != null)
                        TreeView.OnAfterCheck(new TreeViewEventArgs(this, check_reason));
                    if (TreeView != null)
                        TreeView.UpdateNode(this);
                }
                check_reason = TreeViewAction.Unknown;
            }
        }

        public virtual ContextMenuStrip ContextMenu
        {
            get { return context_menu; }
            set { context_menu = value; }
        }

        public virtual ContextMenuStrip ContextMenuStrip
        {
            get { return context_menu_strip; }
            set { context_menu_strip = value; }
        }

        public TreeNode FirstNode
        {
            get
            {
                if (nodes.Count > 0)
                    return nodes[0];
                return null;
            }
        }

        public Color ForeColor
        {
            get
            {
                if (prop_bag != null)
                    return FGColor = prop_bag.ForeColor;
                if (TreeView != null)
                    return FGColor = TreeView.ForeColor;
                return FGColor = Color.Empty;
            }
            set
            {
                if (prop_bag == null)
                    prop_bag = new OwnerDrawPropertyBag();
                prop_bag.ForeColor = FGColor = value;

                TreeViewCanvas tree_view = TreeView;
                if (tree_view != null)
                    tree_view.UpdateNode(this);
            }
        }

        public string FullPath
        {
            get
            {
                if (TreeView == null)
                    throw new InvalidOperationException("No TreeViewCanvas associated");

                StringBuilder builder = new StringBuilder();
                BuildFullPath(builder);
                return builder.ToString();
            }
        }

        [DefaultValue(-1)]
        public int ImageIndex
        {
            get { return image_index; }
            set
            {
                if (image_index == value)
                    return;
                image_index = value;
                image_key = string.Empty;
                TreeViewCanvas tree = TreeView;
                if (tree != null)
                    tree.UpdateNode(this);
            }
        }

        [DefaultValue("")]
        public string ImageKey
        {
            get { return image_key; }
            set
            {
                if (image_key == value)
                    return;
                image_key = value;
                image_index = -1;

                TreeViewCanvas tree = TreeView;
                if (tree != null)
                    tree.UpdateNode(this);
            }
        }

        public bool IsEditing
        {
            get
            {
                TreeViewCanvas tv = TreeView;
                if (tv == null)
                    return false;
                return tv.edit_node == this;
            }
        }

        public bool IsExpanded
        {
            get
            {
                TreeViewCanvas tv = TreeView;

                if (tv != null)
                {
                    // This is ridiculous
                    bool found = false;
                    foreach (TreeNode walk in TreeView.Nodes)
                    {
                        if (walk.Nodes.Count > 0)
                            found = true;
                    }

                    if (!found)
                        return false;
                }

                return is_expanded;
            }
        }

        public bool IsSelected
        {
            get
            {
                if (TreeView == null)
                    return false;
                return TreeView.SelectedNode == this;
            }
        }

        public bool IsVisible
        {
            get
            {
                if (TreeView == null || !TreeView.VisibleAfterClipping)
                    return false;

                if (visible_order <= TreeView.skipped_nodes || visible_order - TreeView.skipped_nodes > TreeView.VisibleCount)
                    return false;

                return ArePreviousNodesExpanded;
            }
        }

        public TreeNode LastNode
        {
            get
            {
                return (nodes == null || nodes.Count == 0) ? null : nodes[nodes.Count - 1];
            }
        }

        public int Level
        {
            get { return IndentLevel; }
        }

        public string Name
        {
            get { return this.name; }
            set
            {
                // Value should never be null as per spec
                this.name = (value == null) ? string.Empty : value;
            }
        }

        public TreeNode NextNode
        {
            get
            {
                if (parent == null)
                    return null;
                int index = Index;
                if (parent.Nodes.Count > index + 1)
                    return parent.Nodes[index + 1];
                return null;
            }
        }

        public TreeNode NextVisibleNode
        {
            get
            {
                OpenTreeNodeEnumerator o = new OpenTreeNodeEnumerator(this);
                o.MoveNext(); // move to the node itself

                if (!o.MoveNext())
                    return null;
                TreeNode c = o.CurrentNode;
                if (!c.IsInClippingRect)
                    return null;
                return c;
            }
        }

        public NanoFont NodeFont
        {
            get
            {
                if (prop_bag != null)
                    return prop_bag.Font;
                if (TreeView != null)
                    return TreeView.Font;
                return null;
            }
            set
            {
                if (prop_bag == null)
                    prop_bag = new OwnerDrawPropertyBag();
                prop_bag.Font = value;
                Invalidate();
            }
        }

        public TreeNodeCollection Nodes
        {
            get
            {
                if (nodes == null)
                    nodes = new TreeNodeCollection(this);
                return nodes;
            }
        }

        public TreeNode Parent
        {
            get
            {
                TreeViewCanvas tree_view = TreeView;
                if (tree_view != null && tree_view.root_node == parent)
                    return null;
                return parent;
            }
        }

        public TreeNode PrevNode
        {
            get
            {
                if (parent == null)
                    return null;
                int index = Index;
                if (index <= 0 || index > parent.Nodes.Count)
                    return null;
                return parent.Nodes[index - 1];
            }
        }

        [Browsable(false)]
        public TreeNode PrevVisibleNode
        {
            get
            {
                OpenTreeNodeEnumerator o = new OpenTreeNodeEnumerator(this);
                o.MovePrevious(); // move to the node itself

                if (!o.MovePrevious())
                    return null;
                TreeNode c = o.CurrentNode;
                if (!c.IsInClippingRect)
                    return null;
                return c;
            }
        }

        [DefaultValue(-1)]
        public int SelectedImageIndex
        {
            get { return selected_image_index; }
            set { selected_image_index = value; }
        }

        [DefaultValue("")]
        public string SelectedImageKey
        {
            get { return selected_image_key; }
            set { selected_image_key = value; }
        }

        [DefaultValue(-1)]
        public int StateImageIndex
        {
            get { return state_image_index; }
            set
            {
                if (state_image_index != value)
                {
                    state_image_index = value;
                    state_image_key = string.Empty;
                    Invalidate();
                }
            }
        }

        [DefaultValue("")]
        public string StateImageKey
        {
            get { return state_image_key; }
            set
            {
                if (state_image_key != value)
                {
                    state_image_key = value;
                    state_image_index = -1;
                    Invalidate();
                }
            }
        }

        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        public string Text
        {
            get
            {
                if (text == null)
                    return String.Empty;
                return text;
            }
            set
            {
                if (text == value)
                    return;
                text = value;
                Invalidate();
                // UIA Framework Event: Text Changed
                TreeViewCanvas view = TreeView;
                //if (view != null)
                //	view.OnUIANodeTextChanged (new NeoTreeViewEventArgs (this));
            }
        }

        [DefaultValue("")]
        public string ToolTipText
        {
            get { return tool_tip_text; }
            set { tool_tip_text = value; }
        }

        public TreeViewCanvas TreeView
        {
            get
            {
                if (tree_view != null)
                    return tree_view;
                TreeNode walk = parent;
                while (walk != null)
                {
                    if (walk.TreeView != null)
                        break;
                    walk = walk.parent;
                }
                if (walk == null)
                    return null;
                return walk.TreeView;
            }
        }

        #endregion // Public Instance Properties

        #region Public Instance Methods

        public void BeginEdit()
        {
            TreeViewCanvas tv = TreeView;
            if (tv != null)
                tv.BeginEdit(this);
        }

        public void Collapse()
        {
            CollapseInternal(false);
        }

        public void Collapse(bool ignoreChildren)
        {
            if (ignoreChildren)
                Collapse();
            else
                CollapseRecursive(this);
        }

        public void EndEdit(bool cancel)
        {
            TreeViewCanvas tv = TreeView;
            if (!cancel && tv != null)
                tv.EndEdit(this);
            else if (cancel && tv != null)
                tv.CancelEdit(this);
        }

        public void Expand()
        {
            Expand(false);
        }

        public void ExpandAll()
        {
            ExpandRecursive(this);
            if (TreeView != null)
                TreeView.UpdateNode(TreeView.root_node);
        }

        public void EnsureVisible()
        {
            if (TreeView == null)
                return;

            if (this.Parent != null)
                ExpandParentRecursive(this.Parent);

            Rectangle bounds = Bounds;
            if (bounds.Y < 0)
            {
                TreeView.SetTop(this);
            }
            else if (bounds.Bottom > TreeView.ViewportRectangle.Bottom)
            {
                TreeView.SetBottom(this);
            }
        }

        public int GetNodeCount(bool includeSubTrees)
        {
            if (!includeSubTrees)
                return Nodes.Count;

            int count = 0;
            GetNodeCountRecursive(this, ref count);

            return count;
        }

        public void Remove()
        {
            if (parent == null)
                return;
            int index = Index;
            parent.Nodes.RemoveAt(index);
        }

        public void Toggle()
        {
            if (is_expanded)
                Collapse();
            else
                Expand();
        }

        public override String ToString()
        {
            return String.Concat("TreeNode: ", Text);
        }

        #endregion // Public Instance Methods

        #region Internal & Private Methods and Properties

        internal bool ArePreviousNodesExpanded
        {
            get
            {
                TreeNode parent = Parent;
                while (parent != null)
                {
                    if (!parent.is_expanded)
                        return false;
                    parent = parent.Parent;
                }

                return true;
            }
        }

        internal bool IsRoot
        {
            get
            {
                TreeViewCanvas tree_view = TreeView;
                if (tree_view == null)
                    return false;
                if (tree_view.root_node == this)
                    return true;
                return false;
            }
        }

        bool BuildFullPath(StringBuilder path)
        {
            if (parent == null)
                return false;

            if (parent.BuildFullPath(path))
                path.Append(TreeView.PathSeparator);

            path.Append(text);
            return true;
        }

        public int Index
        {
            get
            {
                if (parent == null)
                    return 0;
                return parent.Nodes.IndexOf(this);
            }
        }

        private void Expand(bool byInternal)
        {
            if (is_expanded || nodes.Count < 1)
            {
                is_expanded = true;
                return;
            }

            bool cancel = false;
            TreeViewCanvas tree_view = TreeView;
            if (tree_view != null)
            {
                TreeViewCancelEventArgs e = new TreeViewCancelEventArgs(this, false, TreeViewAction.Expand);
                tree_view.OnBeforeExpand(e);
                cancel = e.Cancel;
            }

            if (!cancel)
            {
                is_expanded = true;
                int count_to_next = CountToNext();

                if (tree_view != null)
                {
                    tree_view.OnAfterExpand(new TreeViewEventArgs(this));

                    tree_view.RecalculateVisibleOrder(this);
                    tree_view.UpdateScrollBars(false);

                    // ExpandBelow if we affect the visible area
                    if (visible_order < tree_view.skipped_nodes + tree_view.VisibleCount + 1 && ArePreviousNodesExpanded)
                        tree_view.ExpandBelow(this, count_to_next);
                }
            }
        }

        private void CollapseInternal(bool byInternal)
        {
            if (!is_expanded || nodes.Count < 1)
                return;

            if (IsRoot)
                return;

            bool cancel = false;
            TreeViewCanvas auxTree_view = TreeView;
            if (auxTree_view != null)
            {
                var e = new TreeViewCancelEventArgs(this, false, TreeViewAction.Collapse);
                auxTree_view.OnBeforeCollapse(e);
                cancel = e.Cancel;
            }

            if (!cancel)
            {
                int count_to_next = CountToNext();

                is_expanded = false;

                if (auxTree_view != null)
                {
                    auxTree_view.OnAfterCollapse(new TreeViewEventArgs(this));

                    bool hbar_visible = !auxTree_view.hbar.IsHide;
                    bool vbar_visible = !auxTree_view.vbar.IsHide;

                    auxTree_view.RecalculateVisibleOrder(this);
                    auxTree_view.UpdateScrollBars(false);

                    // CollapseBelow if we affect the visible area
                    if (visible_order < auxTree_view.skipped_nodes + auxTree_view.VisibleCount + 1
                        && ArePreviousNodesExpanded)
                        auxTree_view.CollapseBelow(this, count_to_next);
                    if (!byInternal && HasFocusInChildren())
                        auxTree_view.SelectedNode = this;

                    // If one or both of our scrollbars disappeared,
                    // invalidate everything
                    if ((hbar_visible & auxTree_view.hbar.IsHide)
                        || (vbar_visible & auxTree_view.vbar.IsHide))
                        auxTree_view.Invalidate();
                }
            }
        }

        private int CountToNext()
        {
            bool expanded = is_expanded;
            is_expanded = false;
            OpenTreeNodeEnumerator walk = new OpenTreeNodeEnumerator(this);

            TreeNode next = null;
            if (walk.MoveNext() && walk.MoveNext())
                next = walk.CurrentNode;

            is_expanded = expanded;
            walk.Reset();
            walk.MoveNext();

            int count = 0;
            while (walk.MoveNext() && walk.CurrentNode != next)
                count++;

            return count;
        }

        private bool HasFocusInChildren()
        {
            if (TreeView == null)
                return false;
            foreach (TreeNode node in nodes)
            {
                if (node == TreeView.SelectedNode)
                    return true;
                if (node.HasFocusInChildren())
                    return true;
            }
            return false;
        }

        private void ExpandRecursive(TreeNode node)
        {
            node.Expand(true);
            foreach (TreeNode child in node.Nodes)
                ExpandRecursive(child);
        }

        private void ExpandParentRecursive(TreeNode node)
        {
            node.Expand(true);
            if (node.Parent != null)
                ExpandParentRecursive(node.Parent);
        }

        internal void CollapseAll()
        {
            CollapseRecursive(this);
        }

        internal void CollapseAllUncheck()
        {
            CollapseUncheckRecursive(this);
        }

        private void CollapseRecursive(TreeNode node)
        {
            node.Collapse();
            foreach (TreeNode child in node.Nodes)
                CollapseRecursive(child);
        }

        private void CollapseUncheckRecursive(TreeNode node)
        {
            node.Collapse();
            node.Checked = false;
            foreach (TreeNode child in node.Nodes)
                CollapseUncheckRecursive(child);
        }

        internal void SetNodes(TreeNodeCollection nodes)
        {
            this.nodes = nodes;
        }

        private void GetNodeCountRecursive(TreeNode node, ref int count)
        {
            count += node.Nodes.Count;
            foreach (TreeNode child in node.Nodes)
                GetNodeCountRecursive(child, ref count);
        }

        internal bool NeedsWidth
        {
            get { return width == -1; }
        }

        internal void Invalidate()
        {
            // invalidate width first so Bounds retrieves
            // the updated value (we don't use it here however)
            width = -1;

            TreeViewCanvas tv = TreeView;
            if (tv == null)
                return;

            tv.UpdateNode(this);
        }

        internal void InvalidateWidth()
        {
            // bounds.Width = 0;
            width = -1;
        }

        internal void SetWidth(int width)
        {
            this.width = width;
        }

        internal void SetParent(TreeNode parent)
        {
            this.parent = parent;
        }

        private bool IsInClippingRect
        {
            get
            {
                if (TreeView == null)
                    return false;
                Rectangle bounds = Bounds;
                if (bounds.Y < 0 && bounds.Y > TreeView.ClientRect.Height)
                    return false;
                return true;
            }
        }

        internal Image StateImage
        {
            get
            {
                if (TreeView != null)
                {
                    if (TreeView.StateImageList == null)
                        return null;
                    if (state_image_index >= 0)
                        return TreeView.StateImageList.Images[state_image_index];
                    if (state_image_key != string.Empty)
                        return TreeView.StateImageList.Images[state_image_key];
                }

                return null;
            }
        }

        // Order of operation:
        // 1) Node.Image[Key|Index]
        // 2) TreeViewCanvas.Image[Key|Index]
        // 3) First image in TreeViewCanvas.ImageList
        internal int Image
        {
            get
            {
                if (TreeView == null || TreeView.ImageList == null)
                    return -1;

                if (IsSelected)
                {
                    if (selected_image_index >= 0)
                        return selected_image_index;
                    if (!string.IsNullOrEmpty(selected_image_key))
                        return TreeView.ImageList.Images.IndexOfKey(selected_image_key);
                    if (!string.IsNullOrEmpty(TreeView.SelectedImageKey))
                        return TreeView.ImageList.Images.IndexOfKey(TreeView.SelectedImageKey);
                    if (TreeView.SelectedImageIndex >= 0)
                        return TreeView.SelectedImageIndex;
                }
                else
                {
                    if (image_index >= 0)
                        return image_index;
                    if (!string.IsNullOrEmpty(image_key))
                        return TreeView.ImageList.Images.IndexOfKey(image_key);
                    if (!string.IsNullOrEmpty(TreeView.ImageKey))
                        return TreeView.ImageList.Images.IndexOfKey(TreeView.ImageKey);
                    if (TreeView.ImageIndex >= 0)
                        return TreeView.ImageIndex;
                }

                if (TreeView.ImageList.Images.Count > 0)
                    return 0;

                return -1;
            }
        }
        #endregion // Internal & Private Methods and Properties


        #region OwnerDrawPropertyBag

        #endregion OwnerDrawPropertyBag


    }


}
