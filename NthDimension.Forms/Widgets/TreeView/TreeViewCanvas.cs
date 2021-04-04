using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Collections;

using NthDimension.Forms.Delegates;
using NthDimension.Forms.Events;
using NthDimension.Forms.Themes;
using NthDimension.Forms.Layout;
using NthDimension.Forms.Configuration;

namespace NthDimension.Forms.Widgets
{
    public class TreeViewCanvas : Widget
    {
        #region Fields
        private string path_separator = "\\";
        private int item_height = -1;
        internal bool sorted;
        internal TreeNode root_node;
        internal bool nodes_added;
        private TreeNodeCollection nodes;

        private TreeViewAction selection_action;
        internal TreeNode selected_node;
        private TreeNode pre_selected_node;
        private TreeNode focused_node;
        internal TreeNode highlighted_node;
        private Rectangle mouse_rect;
        private bool select_mmove;

        private ImageList image_list;
        private int image_index = -1;
        private int selected_image_index = -1;

        private string image_key;
        private bool is_hovering;
        private TreeNode mouse_click_node;
        private bool right_to_left_layout;
        private string selected_image_key;
        private bool show_node_tool_tips;
        private ImageList state_image_list;
        private TreeNode tooltip_currently_showing;
        private ToolTip tooltip_window;
        private bool full_row_select;
        private bool hot_tracking;
        private int indent = 19;

        private NodeLabelEditEventArgs edit_args;
        private LabelEditTextBox edit_text_box;
        internal TreeNode edit_node;

        private bool checkboxes;
        private bool label_edit;
        private bool scrollable = true;
        private bool show_lines = true;
        private bool show_root_lines = true;
        private bool show_plus_minus = true;
        private bool hide_selection = true;

        private int max_visible_order = -1;
        internal ScrollBarV vbar;
        internal ScrollBarH hbar;
        private bool vbar_bounds_set;
        private bool hbar_bounds_set;
        internal int skipped_nodes;
        internal int hbar_offset;

        private int update_stack;
        private bool update_needed;

        private NanoPen dash;
        private Color line_color;
        private StringFormat string_format;

        private int drag_begin_x = -1;
        private int drag_begin_y = -1;
        private long handle_count = 1;

        private TreeViewDrawMode draw_mode;

        IComparer tree_view_node_sorter;
        #endregion  // Fields

        #region Public Constructors
        public TreeViewCanvas(ScrollBarH hbar, ScrollBarV vbar)
        {
            this.vbar = vbar;
            this.hbar = hbar;

            //vbar = new ScrollBarV();
            //vbar.Size = new Size(SystemInformation.VerticalScrollBarWidth, 0);
            //hbar = new ScrollBarH();
            //hbar.Size = new Size(0, SystemInformation.HorizontalScrollBarHeight);

            //InternalBorderStyle = BorderStyle.Fixed3D;
            //base.background_color = ThemeEngine.Current.ColorWindow;
            //base.foreground_color = ThemeEngine.Current.ColorWindowText;
            draw_mode = TreeViewDrawMode.Normal;
            BackColor = ThemeEngine.BackColor2;

            root_node = new TreeNode(this);
            root_node.Text = "ROOT NODE";
            nodes = new TreeNodeCollection(root_node);
            root_node.SetNodes(nodes);

            MouseDownEvent += new MouseDownHandler(MouseDownHandler);
            MouseUpEvent += new MouseUpHandler(MouseUpHandler);
            MouseMoveEvent += new MouseMoveHandler(MouseMoveHandler);
            SizeChangedEvent += new SizeChangedHandler(SizeChangedHandler);
            //FontChanged += new EventHandler(FontChangedHandler);
            LostFocusEvent += new EventHandler(LostFocusHandler);
            GotFocusEvent += new EventHandler(GotFocusHandler);
            //MouseWheel += new MouseWheelHandler(MouseWheelHandler);
            HideChangedEvent += new HideChangedHandler(VisibleChangedHandler);

            //SetStyle(ControlStyles.UserPaint | ControlStyles.StandardClick | ControlStyles.UseTextForAccessibility, false);

            string_format = new StringFormat();
            string_format.LineAlignment = StringAlignment.Center;
            string_format.Alignment = StringAlignment.Center;

            vbar.IsHide = true;
            hbar.IsHide = true;
            vbar.ValueChanged += new EventHandler(ScrollBarVValueChanged);
            hbar.ValueChanged += new EventHandler(ScrollBarHValueChanged);

            /*SuspendLayout();
			Widgets.Add(vbar);
			Widgets.Add(hbar);
			ResumeLayout();*/
        }
        #endregion // Public Constructors

        #region Public Instance Properties

        public Color BackColor
        {
            get { return BGColor; }
            set
            {
                BGColor = value;

                hbar.BGColor = value;
                vbar.BGColor = value;
                CreateDashPen();
                Invalidate();
            }
        }

        /*
				[Browsable(false)]
				[EditorBrowsable(EditorBrowsableState.Never)]
				public override Image BackgroundImage
				{
					get { return base.BackgroundImage; }
					set { base.BackgroundImage = value; }
				}
		 */
        [DefaultValue(BorderStyle.Fixed3D)]
        public BorderStyle BorderStyle
        {
            get; // { return InternalBorderStyle; }
            set; // { InternalBorderStyle  = value; }
        }

        public string ShowMessageCentered
        {
            get;
            set;
        }

        public bool CheckBoxes
        {
            get { return checkboxes; }
            set
            {
                if (value == checkboxes)
                    return;
                checkboxes = value;

                // Match a "bug" in the MS implementation where disabling checkboxes
                // collapses the entire tree, but enabling them does not affect the
                // state of the tree.
                if (!checkboxes)
                    root_node.CollapseAllUncheck();

                Invalidate();

                // UIA Framework Event: CheckBoxes Changed
                //OnUIACheckBoxesChanged(EventArgs.Empty);
            }
        }

        public Color ForeColor
        {
            get { return base.FGColor; }
            set { base.FGColor = value; }
        }

        public bool FullRowSelect
        {
            get { return full_row_select; }
            set
            {
                if (value == full_row_select)
                    return;
                full_row_select = value;
                Invalidate();
            }
        }
        [DefaultValue(true)]
        public bool HideSelection
        {
            get { return hide_selection; }
            set
            {
                if (hide_selection == value)
                    return;
                hide_selection = value;
                Invalidate();
            }
        }

        [DefaultValue(false)]
        public bool HotTracking
        {
            get { return hot_tracking; }
            set { hot_tracking = value; }
        }

        [DefaultValue(-1)]
        public int ImageIndex
        {
            get { return image_index; }
            set
            {
                if (value < -1)
                {
                    throw new ArgumentException("'" + value + "' is not a valid value for 'value'. " +
                                                "'value' must be greater than or equal to 0.");
                }
                if (image_index == value)
                    return;
                image_index = value;
                Invalidate();
            }
        }
        public ImageList ImageList
        {
            get { return image_list; }
            set
            {
                image_list = value;
                Invalidate();
            }
        }

        public int Indent
        {
            get { return indent; }
            set
            {
                if (indent == value)
                    return;
                if (value > 32000)
                {
                    throw new ArgumentException("'" + value + "' is not a valid value for 'Indent'. " +
                                                "'Indent' must be less than or equal to 32000");
                }
                if (value < 0)
                {
                    throw new ArgumentException("'" + value + "' is not a valid value for 'Indent'. " +
                                                "'Indent' must be greater than or equal to 0.");
                }
                indent = value;
                Invalidate();
            }
        }

        public int ItemHeight
        {
            get
            {
                if (item_height == -1)
                    return FontHeight + 3;
                return item_height;
            }
            set
            {
                if (value == item_height)
                    return;
                item_height = value;
                Invalidate();
            }
        }

        internal int ActualItemHeight
        {
            get
            {
                int res = ItemHeight;
                if (ImageList != null && ImageList.ImageSize.Height > res)
                    res = ImageList.ImageSize.Height;
                return res;
            }
        }

        public bool LabelEdit
        {
            get { return label_edit; }
            set
            {
                label_edit = value;

                // UIA Framework Event: LabelEdit Changed
                //OnUIALabelEditChanged(EventArgs.Empty);
            }
        }

        public TreeNodeCollection Nodes
        {
            get { return nodes; }
        }

        public new Spacing Padding
        {
            get { return base.Padding; }
            set { base.Padding = value; }
        }

        [DefaultValue("\\")]
        public string PathSeparator
        {
            get { return path_separator; }
            set { path_separator = value; }
        }

        public virtual bool RightToLeftLayout
        {
            get { return right_to_left_layout; }
            set
            {
                if (right_to_left_layout != value)
                {
                    right_to_left_layout = value;
                    OnRightToLeftLayoutChanged(EventArgs.Empty);
                }
            }
        }

        [DefaultValue(true)]
        public bool Scrollable
        {
            get { return scrollable; }
            set
            {
                if (scrollable == value)
                    return;
                scrollable = value;
                UpdateScrollBars(false);
            }
        }

        [DefaultValue(-1)]
        public int SelectedImageIndex
        {
            get { return selected_image_index; }
            set
            {
                if (value < -1)
                {
                    throw new ArgumentException("'" + value + "' is not a valid value for 'value'. " +
                                                "'value' must be greater than or equal to 0.");
                }
                UpdateNode(SelectedNode);
            }
        }

        public TreeNode SelectedNode
        {
            get
            {
                //if (!IsHandleCreated)
                //	return pre_selected_node;
                return selected_node;
            }
            set
            {
                /*if (!IsHandleCreated)
				{
					pre_selected_node = value;
					return;
				}*/

                if (selected_node == value)
                {
                    selection_action = TreeViewAction.Unknown;
                    return;
                }

                if (value != null)
                {
                    var e = new TreeViewCancelEventArgs(value, false, selection_action);
                    OnBeforeSelect(e);

                    if (e.Cancel)
                        return;
                }

                Rectangle invalid = Rectangle.Empty;

                if (selected_node != null)
                {
                    invalid = Bloat(selected_node.Bounds);
                }
                if (focused_node != null)
                {
                    invalid = Rectangle.Union(invalid,
                                              Bloat(focused_node.Bounds));
                }

                if (value != null)
                    invalid = Rectangle.Union(invalid, Bloat(value.Bounds));

                highlighted_node = value;
                selected_node = value;
                focused_node = value;

                if (full_row_select || draw_mode != TreeViewDrawMode.Normal)
                {
                    invalid.X = 0;
                    invalid.Width = ViewportRectangle.Width;
                }

                if (invalid != Rectangle.Empty)
                    Invalidate(invalid);

                // We ensure its visible after we update because
                // scrolling is used for insure visible
                if (selected_node != null)
                    selected_node.EnsureVisible();

                if (value != null)
                {
                    OnAfterSelect(new TreeViewEventArgs(value, TreeViewAction.Unknown));
                }
                selection_action = TreeViewAction.Unknown;
            }
        }

        private Rectangle Bloat(Rectangle rect)
        {
            rect.Y--;
            rect.X--;
            rect.Height += 2;
            rect.Width += 2;
            return rect;
        }

        [DefaultValue(true)]
        public bool ShowLines
        {
            get { return show_lines; }
            set
            {
                if (show_lines == value)
                    return;
                show_lines = value;
                Invalidate();
            }
        }

        public bool ShowNodeToolTips
        {
            get { return show_node_tool_tips; }
            set { show_node_tool_tips = value; }
        }

        [DefaultValue(true)]
        public bool ShowPlusMinus
        {
            get { return show_plus_minus; }
            set
            {
                if (show_plus_minus == value)
                    return;
                show_plus_minus = value;
                Invalidate();
            }
        }

        [DefaultValue(true)]
        public bool ShowRootLines
        {
            get { return show_root_lines; }
            set
            {
                if (show_root_lines == value)
                    return;
                show_root_lines = value;
                Invalidate();
            }
        }

        public bool Sorted
        {
            get { return sorted; }
            set
            {
                if (sorted == value)
                    return;
                sorted = value;
                //LAMESPEC: The documentation says that setting this to true should sort alphabetically if TreeViewNodeSorter is set.
                // There seems to be a bug in the Microsoft implementation.
                if (sorted && tree_view_node_sorter == null)
                {
                    Sort(null);
                }
            }
        }

        public ImageList StateImageList
        {
            get { return state_image_list; }
            set
            {
                state_image_list = value;
                Invalidate();
            }
        }
        /*
				public override string Text
				{
					get { return base.Text; }
					set { base.Text = value; }
				}
		 */
        public TreeNode TopNode
        {
            get
            {
                if (root_node.FirstNode == null)
                    return null;
                var one = new OpenTreeNodeEnumerator(root_node.FirstNode);
                one.MoveNext();
                for (int i = 0; i < skipped_nodes; i++)
                    one.MoveNext();
                return one.CurrentNode;
            }
            set
            {
                SetTop(value);
            }
        }

        public IComparer TreeViewNodeSorter
        {
            get
            {
                return tree_view_node_sorter;
            }
            set
            {
                tree_view_node_sorter = value;
                if (tree_view_node_sorter != null)
                {
                    Sort();
                    //LAMESPEC: The documentation says that setting this should set Sorted to false.
                    // There seems to be a bug in the Microsoft implementation.
                    sorted = true;
                }
            }
        }

        public int VisibleCount
        {
            get
            {
                return ViewportRectangle.Height / ActualItemHeight;
            }
        }
        /*
				/// According to MSDN this property has no effect on the treeview
				protected override bool DoubleBuffered
				{
					get { return base.DoubleBuffered; }
					set { base.DoubleBuffered = value; }
				}
		 */
        [DefaultValue("TextColor [Black]")]
        public Color LineColor
        {
            get
            {
                if (line_color == Color.Empty)
                {
                    Color res = Color.LightGray; // ControlPaint.Dark(BackColor);
                                                 //if (res == BackColor)
                                                 //	res = ControlPaint.Light(BackColor);
                    return res;
                }
                return line_color;
            }
            set
            {
                line_color = value;
                if (show_lines)
                {
                    CreateDashPen();
                    Invalidate();
                }
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
                image_index = -1;
                image_key = value;
                Invalidate();
            }
        }

        [DefaultValue("")]
        public string SelectedImageKey
        {
            get { return selected_image_key; }
            set
            {
                if (selected_image_key == value)
                    return;
                selected_image_index = -1;
                selected_image_key = value;
                UpdateNode(SelectedNode);
            }
        }

        /*public override ImageLayout BackgroundImageLayout
		{
			get { return base.BackgroundImageLayout; }
			set { base.BackgroundImageLayout = value; }
		}*/

        [DefaultValue(TreeViewDrawMode.Normal)]
        public TreeViewDrawMode DrawMode
        {
            get { return draw_mode; }
            set { draw_mode = value; }
        }
        #endregion // Public Instance Properties

        #region UIA Framework Properties
        internal ScrollBarBase UIAScrollBarH
        {
            get { return hbar; }
        }

        internal ScrollBarBase UIAScrollBarV
        {
            get { return vbar; }
        }
        #endregion // UIA Framework Properties

        #region Protected Instance Properties

        protected Size DefaultSize
        {
            get { return new Size(121, 97); }
        }

        #endregion // Protected Instance Properties

        #region Public Instance Methods
        public void BeginUpdate()
        {
            update_stack++;
        }

        public void EndUpdate()
        {
            if (update_stack > 1)
            {
                update_stack--;
            }
            else
            {
                update_stack = 0;
                if (update_needed)
                {
                    RecalculateVisibleOrder(root_node);
                    UpdateScrollBars(false);
                    Invalidate(ViewportRectangle);
                    update_needed = false;
                }
            }
        }

        public void Sort()
        {
            Sort(tree_view_node_sorter);
        }

        void Sort(IComparer sorter)
        {
            sorted = true;
            Nodes.Sort(sorter);
            RecalculateVisibleOrder(root_node);
            UpdateScrollBars(false);
            Invalidate();
        }

        void SetVScrollValue(int value)
        {
            if (value > vbar.Maximum)
                value = vbar.Maximum;
            else if (value < vbar.Minimum)
                value = vbar.Minimum;

            vbar.Value = value;
        }

        public void ExpandAll()
        {
            BeginUpdate();
            root_node.ExpandAll();

            EndUpdate();

            //
            // Everything below this is basically an emulation of a strange bug on MS
            // where they don't always scroll to the last node on ExpandAll
            //
            //if (!IsHandleCreated)
            //	return;

            bool found = false;
            foreach (TreeNode child in Nodes)
            {
                if (child.Nodes.Count > 0)
                    found = true;
            }

            if (!found)
                return;

            if (/*IsHandleCreated &&*/ !vbar.IsHide) //.VisibleInternal)
            {
                SetVScrollValue(vbar.Maximum - VisibleCount + 1);
            }
            else
            {
                RecalculateVisibleOrder(root_node);
                UpdateScrollBars(true);

                // Only move the top node if we now have a scrollbar
                if (!vbar.IsHide) //.VisibleInternal)
                {
                    SetTop(Nodes[Nodes.Count - 1]);
                    SelectedNode = Nodes[Nodes.Count - 1];
                }
            }
        }

        public void CollapseAll()
        {
            BeginUpdate();
            root_node.CollapseAll();
            EndUpdate();

            if (!vbar.IsHide) //.VisibleInternal)
                SetVScrollValue(vbar.Maximum - VisibleCount + 1);
        }

        public TreeNode GetNodeAt(Point pt)
        {
            return GetNodeAt(pt.Y);
        }

        public TreeNode GetNodeAt(int x, int y)
        {
            return GetNodeAt(y);
        }

        private TreeNode GetNodeAtUseX(int x, int y)
        {
            TreeNode node = GetNodeAt(y);
            if (node == null || !(IsTextArea(node, x) || full_row_select))
                return null;
            return node;

        }

        public int GetNodeCount(bool includeSubTrees)
        {
            return root_node.GetNodeCount(includeSubTrees);
        }

        public TreeViewHitTestInfo HitTest(Point pt)
        {
            return HitTest(pt.X, pt.Y);
        }

        public TreeViewHitTestInfo HitTest(int x, int y)
        {
            TreeNode n = GetNodeAt(y);

            if (n == null)
                return new TreeViewHitTestInfo(null, TreeViewHitTestLocations.None);

            if (IsTextArea(n, x))
                return new TreeViewHitTestInfo(n, TreeViewHitTestLocations.Label);
            else if (IsPlusMinusArea(n, x))
                return new TreeViewHitTestInfo(n, TreeViewHitTestLocations.PlusMinus);
            else if ((checkboxes || n.StateImage != null) && IsCheckboxArea(n, x))
                return new TreeViewHitTestInfo(n, TreeViewHitTestLocations.StateImage);
            else if (x > n.Bounds.Right)
                return new TreeViewHitTestInfo(n, TreeViewHitTestLocations.RightOfLabel);
            else if (IsImage(n, x))
                return new TreeViewHitTestInfo(n, TreeViewHitTestLocations.Image);
            else
                return new TreeViewHitTestInfo(null, TreeViewHitTestLocations.Indent);
        }

        public override string ToString()
        {
            int count = Nodes.Count;
            if (count <= 0)
                return String.Concat(base.ToString(), ", Nodes.Count: 0");
            return String.Concat(base.ToString(), ", Nodes.Count: ", count, ", Nodes[0]: ", Nodes[0]);

        }
        #endregion // Public Instance Methods

        #region Protected Instance Methods
        /*
				protected override void CreateHandle()
				{
					base.CreateHandle();
					RecalculateVisibleOrder(root_node);
					UpdateScrollBars(false);

					if (pre_selected_node != null)
						SelectedNode = pre_selected_node;
				}
		 */

        protected OwnerDrawPropertyBag GetItemRenderStyles(TreeNode node, int state)
        {
            return node.prop_bag;
        }

        protected bool IsInputKey(Keys keyData)
        {
            if ((keyData & Keys.Alt) == 0)
            {
                switch (keyData & Keys.KeyCode)
                {
                    case Keys.Left:
                    case Keys.Up:
                    case Keys.Right:
                    case Keys.Down:
                        return true;
                    case Keys.Enter:
                    case Keys.Escape:
                    case Keys.Prior:
                    case Keys.Next:
                    case Keys.End:
                    case Keys.Home:
                        if (edit_node != null)
                            return true;

                        break;
                }
            }
            return false; //base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            OpenTreeNodeEnumerator ne;

            switch (e.KeyData & Keys.KeyCode)
            {
                case Keys.Add:
                    if (selected_node != null && selected_node.IsExpanded)
                        selected_node.Expand();
                    break;
                case Keys.Subtract:
                    if (selected_node != null && selected_node.IsExpanded)
                        selected_node.Collapse();
                    break;
                case Keys.Left:
                    if (selected_node != null)
                    {
                        if (selected_node.IsExpanded && selected_node.Nodes.Count > 0)
                            selected_node.Collapse();
                        else
                        {
                            TreeNode parent = selected_node.Parent;
                            if (parent != null)
                            {
                                selection_action = TreeViewAction.ByKeyboard;
                                SelectedNode = parent;
                            }
                        }
                    }
                    break;
                case Keys.Right:
                    if (selected_node != null)
                    {
                        if (!selected_node.IsExpanded)
                            selected_node.Expand();
                        else
                        {
                            TreeNode child = selected_node.FirstNode;
                            if (child != null)
                                SelectedNode = child;
                        }
                    }
                    break;
                case Keys.Up:
                    if (selected_node != null)
                    {
                        ne = new OpenTreeNodeEnumerator(selected_node);
                        if (ne.MovePrevious() && ne.MovePrevious())
                        {
                            selection_action = TreeViewAction.ByKeyboard;
                            SelectedNode = ne.CurrentNode;
                        }
                    }
                    break;
                case Keys.Down:
                    if (selected_node != null)
                    {
                        ne = new OpenTreeNodeEnumerator(selected_node);
                        if (ne.MoveNext() && ne.MoveNext())
                        {
                            selection_action = TreeViewAction.ByKeyboard;
                            SelectedNode = ne.CurrentNode;
                        }
                    }
                    break;
                case Keys.Home:
                    if (root_node.Nodes.Count > 0)
                    {
                        ne = new OpenTreeNodeEnumerator(root_node.Nodes[0]);
                        if (ne.MoveNext())
                        {
                            selection_action = TreeViewAction.ByKeyboard;
                            SelectedNode = ne.CurrentNode;
                        }
                    }
                    break;
                case Keys.End:
                    if (root_node.Nodes.Count > 0)
                    {
                        ne = new OpenTreeNodeEnumerator(root_node.Nodes[0]);
                        while (ne.MoveNext())
                        { }
                        selection_action = TreeViewAction.ByKeyboard;
                        SelectedNode = ne.CurrentNode;
                    }
                    break;
                case Keys.PageDown:
                    if (selected_node != null)
                    {
                        ne = new OpenTreeNodeEnumerator(selected_node);
                        int move = VisibleCount;
                        for (int i = 0; i < move && ne.MoveNext(); i++)
                        {

                        }
                        selection_action = TreeViewAction.ByKeyboard;
                        SelectedNode = ne.CurrentNode;
                    }
                    break;
                case Keys.PageUp:
                    if (selected_node != null)
                    {
                        ne = new OpenTreeNodeEnumerator(selected_node);
                        int move = VisibleCount;
                        for (int i = 0; i < move && ne.MovePrevious(); i++)
                        { }
                        selection_action = TreeViewAction.ByKeyboard;
                        SelectedNode = ne.CurrentNode;
                    }
                    break;
                case Keys.Multiply:
                    if (selected_node != null)
                        selected_node.ExpandAll();
                    break;
            }
            base.OnKeyDown(e);

            if (!e.Handled && checkboxes &&
                selected_node != null &&
                (e.KeyData & Keys.KeyCode) == Keys.Space)
            {
                selected_node.check_reason = TreeViewAction.ByKeyboard;
                selected_node.Checked = !selected_node.Checked;
                e.Handled = true;
            }
        }

        protected override void OnKeyPress(KeyPressedEventArgs e)
        {
            base.OnKeyPress(e);
            if (e.KeyChar == ' ')
                e.Handled = true;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if ((e.KeyData & Keys.KeyCode) == Keys.Space)
                e.Handled = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            is_hovering = true;

            TreeNode tn = GetNodeAt(e.Location);

            if (tn != null)
                MouseEnteredItem(tn);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            is_hovering = false;

            if (tooltip_currently_showing != null)
                MouseLeftItem(tooltip_currently_showing);
        }

        protected virtual void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
        {
            if (NodeMouseClick != null)
                NodeMouseClick(this, e);
        }

        protected virtual void OnNodeMouseDoubleClick(TreeNodeMouseClickEventArgs e)
        {
            if (NodeMouseDoubleClick != null)
                NodeMouseDoubleClick(this, e);
        }

        protected virtual void OnNodeMouseHover(TreeNodeMouseHoverEventArgs e)
        {
            if (NodeMouseHover != null)
                NodeMouseHover(this, e);
        }

        protected virtual void OnItemDrag(ItemDragEventArgs e)
        {
            //if (ItemDragEvent != null)
            //	ItemDragEvent(this, e);
        }

        protected virtual void OnDrawNode(DrawTreeNodeEventArgs e)
        {
            if (DrawNode != null)
                DrawNode(this, e);
        }

        protected virtual void OnRightToLeftLayoutChanged(EventArgs e)
        {
            if (RightToLeftLayoutChanged != null)
                RightToLeftLayoutChanged(this, e);
        }

        protected internal virtual void OnAfterCheck(TreeViewEventArgs e)
        {
            if (AfterCheck != null)
                AfterCheck(this, e);
        }

        protected internal virtual void OnAfterCollapse(TreeViewEventArgs e)
        {
            if (AfterCollapse != null)
                AfterCollapse(this, e);
        }

        protected internal virtual void OnAfterExpand(TreeViewEventArgs e)
        {
            if (AfterExpand != null)
                AfterExpand(this, e);
        }

        protected virtual void OnAfterLabelEdit(NodeLabelEditEventArgs e)
        {
            if (AfterLabelEdit != null)
                AfterLabelEdit(this, e);
        }

        protected virtual void OnAfterSelect(TreeViewEventArgs e)
        {
            if (AfterSelect != null)
                AfterSelect(this, e);
        }

        protected internal virtual void OnBeforeCheck(TreeViewCancelEventArgs e)
        {
            if (BeforeCheck != null)
                BeforeCheck(this, e);
        }

        protected internal virtual void OnBeforeCollapse(TreeViewCancelEventArgs e)
        {
            if (BeforeCollapse != null)
                BeforeCollapse(this, e);
        }

        protected internal virtual void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            if (BeforeExpand != null)
                BeforeExpand(this, e);
        }

        protected virtual void OnBeforeLabelEdit(NodeLabelEditEventArgs e)
        {
            if (BeforeLabelEdit != null)
                BeforeLabelEdit(this, e);
        }

        protected virtual void OnBeforeSelect(TreeViewCancelEventArgs e)
        {
            if (BeforeSelect != null)
                BeforeSelect(this, e);
        }
        /*
				const int WM_LBUTTONDBLCLK = 515;
				const int WM_CONTEXTMENU = 123;

				protected override void WndProc(ref Message m)
				{
					switch (m.Msg)
					{
						case WM_LBUTTONDBLCLK:
							int val = m.LParam.ToInt32();
							DoubleClickHandler(null, new
								MouseEventArgs(EMouseButtons.Left,
										2, val & 0xffff,
										(val >> 16) & 0xffff, 0));
							break;
						case WM_CONTEXTMENU:
							if (WmContextMenu(ref m))
								return;

							break;
					}
					base.WndProc(ref m);
				}
		 */
        #endregion // Protected Instance Methods

        #region	Internal & Private Methods and Properties

        internal bool ScaleChildrenInternal
        {
            get { return false; }
        }

        internal IntPtr CreateNodeHandle()
        {
            return (IntPtr)handle_count++;
        }

        // According to MSDN docs, for these to be raised,
        // the click must occur over a TreeNode
        internal void HandleClick(int clicks, MouseEventArgs me)
        {
            if (GetNodeAt(me.Location) != null)
            {
                if ((clicks > 1)/* && GetStyle(ControlStyles.StandardDoubleClick)*/)
                {
                    //OnDoubleClick(me);
                    //OnMouseDoubleClick(me);
                }
                else
                {
                    //OnClick(me);
                    OnMouseClick(me);
                }
            }
        }

        internal bool IsInputCharInternal(char charCode)
        {
            return true;
        }

        internal TreeNode NodeFromHandle(IntPtr handle)
        {
            // This method is called rarely, so instead of maintaining a table
            // we just walk the tree nodes to find the matching handle
            return NodeFromHandleRecursive(root_node, handle);
        }

        private TreeNode NodeFromHandleRecursive(TreeNode node, IntPtr handle)
        {
            if (node.handle == handle)
                return node;
            foreach (TreeNode child in node.Nodes)
            {
                TreeNode match = NodeFromHandleRecursive(child, handle);
                if (match != null)
                    return match;
            }
            return null;
        }

        internal Rectangle ViewportRectangle
        {
            get
            {
                Rectangle res = ClientRect;

                /*if (vbar != null && !vbar.IsHide)
					res.Width -= vbar.Width;
				if (hbar != null && !hbar.IsHide)
					res.Height -= hbar.Height;*/
                return res;
            }
        }

        private TreeNode GetNodeAt(int y)
        {
            if (nodes.Count <= 0)
                return null;

            var o = new OpenTreeNodeEnumerator(TopNode);
            int move = y / ActualItemHeight;
            for (int i = -1; i < move; i++)
            {
                if (!o.MoveNext())
                    return null;
            }

            return o.CurrentNode;
        }

        private bool IsTextArea(TreeNode node, int x)
        {
            return node != null && node.Bounds.Left <= x && node.Bounds.Right >= x;
        }

        private bool IsSelectableArea(TreeNode node, int x)
        {
            if (node == null)
                return false;
            int l = node.Bounds.Left;
            if (ImageList != null)
                l -= ImageList.ImageSize.Width;
            return l <= x && node.Bounds.Right >= x;

        }

        private bool IsPlusMinusArea(TreeNode node, int x)
        {
            if (node.Nodes.Count == 0 || (node.parent == root_node && !show_root_lines))
                return false;

            int l = node.Bounds.Left + 5;

            if (show_root_lines || node.Parent != null)
                l -= indent;
            if (ImageList != null)
                l -= ImageList.ImageSize.Width + 3;
            if (checkboxes)
                l -= 19;
            // StateImage is basically a custom checkbox
            else if (node.StateImage != null)
                l -= 19;
            return (x > l && x < l + 10);
        }

        private bool IsCheckboxArea(TreeNode node, int x)
        {
            int l = CheckBoxLeft(node);
            return (x > l && x < l + 10);
        }

        private bool IsImage(TreeNode node, int x)
        {
            if (ImageList == null)
                return false;

            int l = node.Bounds.Left;

            l -= ImageList.ImageSize.Width + 5;

            if (x >= l && x <= (l + ImageList.ImageSize.Width + 5))
                return true;

            return false;
        }

        private int CheckBoxLeft(TreeNode node)
        {
            int l = node.Bounds.Left + 5;

            if (show_root_lines || node.Parent != null)
                l -= indent;

            if (!show_root_lines && node.Parent == null)
                l -= indent;

            if (ImageList != null)
                l -= ImageList.ImageSize.Width + 3;

            return l;
        }

        internal void RecalculateVisibleOrder(TreeNode start)
        {
            if (update_stack > 0)
                return;

            int order;
            if (start == null)
            {
                start = root_node;
                order = 0;
            }
            else
                order = start.visible_order;

            var walk = new OpenTreeNodeEnumerator(start);
            while (walk.MoveNext())
            {
                walk.CurrentNode.visible_order = order;
                order++;
            }

            max_visible_order = order;
        }

        internal void SetTop(TreeNode node)
        {
            int order = 0;
            if (node != null)
                order = System.Math.Max(0, node.visible_order - 1);

            if (!vbar.IsHide)
            {
                skipped_nodes = order;
                return;
            }

            SetVScrollValue(System.Math.Min(order, vbar.Maximum - VisibleCount + 1));
        }

        internal void SetBottom(TreeNode node)
        {
            if (!vbar.IsHide)
                return;

            var walk = new OpenTreeNodeEnumerator(node);

            int bottom = ViewportRectangle.Bottom;
            int offset = 0;
            while (walk.MovePrevious())
            {
                if (walk.CurrentNode.Bounds.Bottom <= bottom)
                    break;
                offset++;
            }

            int nv = vbar.Value + offset;
            if (vbar.Value + offset < vbar.Maximum)
            {
                SetVScrollValue(nv);
            }
            else
            {
#if DEBUG
                Console.Error.WriteLine("setting bottom to value greater then maximum ({0}, {1})",
                                        nv, vbar.Maximum);
#endif
            }

        }

        internal void UpdateBelow(TreeNode node)
        {
            if (update_stack > 0)
            {
                update_needed = true;
                return;
            }

            if (node == root_node)
            {
                Invalidate(ViewportRectangle);
                return;
            }

            // We need to update the current node so the plus/minus block gets update too
            int top = System.Math.Max(node.Bounds.Top - 1, 0);
            var invalid = new Rectangle(0, top,
                                        Width, Height - top);
            Invalidate(invalid);
        }

        internal void UpdateNode(TreeNode node)
        {
            if (node == null)
                return;

            if (update_stack > 0)
            {
                update_needed = true;
                return;
            }

            if (node == root_node)
            {
                Invalidate();
                return;
            }

            var invalid = new Rectangle(0, node.Bounds.Top - 1, Width,
                                           node.Bounds.Height + 1);
            Invalidate(invalid);
        }

        internal void UpdateNodePlusMinus(TreeNode node)
        {
            if (update_stack > 0)
            {
                update_needed = true;
                return;
            }

            int l = node.Bounds.Left + 5;

            if (show_root_lines || node.Parent != null)
                l -= indent;
            if (ImageList != null)
                l -= ImageList.ImageSize.Width + 3;
            if (checkboxes)
                l -= 19;

            Invalidate(new Rectangle(l, node.Bounds.Top, 8, node.Bounds.Height));
        }

        protected override void DoPaint(PaintEventArgs e)
        {
            base.DoPaint(e);

            if (!string.IsNullOrEmpty(ShowMessageCentered))
            {
                var f = new NanoFont(NanoFont.DefaultRegular, 14f);
                e.GC.DrawString(ShowMessageCentered, f, new NanoSolidBrush(FGColor), 10, Height / 2);
                return;
            }

            Draw(e.ClipRect, e.GC);
        }

        internal void CreateDashPen()
        {
            dash = new NanoPen(LineColor, 1);
            //dash.DashStyle = DashStyle.Dot;
        }

        private void Draw(Rectangle clip, GContext dc)
        {
            var bc = new SolidBrush(ThemeEngine.BackColor2);
            dc.FillRectangle(bc, clip);

            if (dash == null)
                CreateDashPen();

            Rectangle viewport = ViewportRectangle;
            Rectangle original_clip = clip;
            if (clip.Bottom > viewport.Bottom)
                clip.Height = viewport.Bottom - clip.Top;

            var walk = new OpenTreeNodeEnumerator(TopNode);
            while (walk.MoveNext())
            {
                TreeNode current = walk.CurrentNode;

                // Haven't gotten to visible nodes yet
                if (current.GetY() + ActualItemHeight < clip.Top)
                    continue;

                // Past the visible nodes
                if (current.GetY() > clip.Bottom)
                    break;

                DrawTreeNode(current, dc, clip);
            }
        }

        private void DrawNodeState(TreeNode node, GContext dc, int x, int y)
        {
            if (node.Checked)
            {
                if (StateImageList.Images[1] != null)
                    dc.DrawImage(StateImageList.Images[1], new Rectangle(x, y, 16, 16));
            }
            else
            {
                if (StateImageList.Images[0] != null)
                    dc.DrawImage(StateImageList.Images[0], new Rectangle(x, y, 16, 16));
            }
        }

        private void DrawNodeCheckBox(TreeNode node, GContext dc, int x, int middle)
        {
            var pen = new NanoPen(Color.Black);
            pen.Width = 2;
            dc.DrawRectangle(pen, x + 2, middle - 4, 11, 11);

            if (node.Checked)
            {
                var check_pen = new NanoPen(Color.Black);
                check_pen.Width = 1.5f;

                const int check_size = 5;
                const int lineWidth = 3;

                var rect = new Rectangle(x + 4, middle - 3, check_size, check_size);

                const int i = 1;
                //for (int i = 0; i < lineWidth; i++)
                {
                    dc.DrawLine(check_pen, rect.Left + 1, rect.Top + lineWidth + i, rect.Left + 3, rect.Top + 5 + i);
                    dc.DrawLine(check_pen, rect.Left + 3, rect.Top + 5 + i, rect.Left + 7, rect.Top + 1 + i);
                }
            }
        }

        private void DrawNodeLines(TreeNode node, GContext dc, Rectangle clip, NanoPen pDash, int x, int y, int middle)
        {
            int ladjust = 9;
            int radjust = 0;

            if (node.nodes.Count > 0 && show_plus_minus)
                ladjust = 13;
            if (checkboxes)
                radjust = 3;

            if (show_root_lines || node.Parent != null)
                dc.DrawLine(pDash, x - indent + ladjust, middle, x + radjust, middle);

            if (node.PrevNode != null || node.Parent != null)
            {
                ladjust = 9;
                dc.DrawLine(pDash, x - indent + ladjust, node.Bounds.Top,
                            x - indent + ladjust, middle - (show_plus_minus && node.Nodes.Count > 0 ? 4 : 0));
            }

            if (node.NextNode != null)
            {
                ladjust = 9;
                dc.DrawLine(pDash, x - indent + ladjust, middle + (show_plus_minus && node.Nodes.Count > 0 ? 4 : 0),
                            x - indent + ladjust, node.Bounds.Bottom);

            }

            ladjust = 0;
            if (show_plus_minus)
                ladjust = 9;
            TreeNode parent = node.Parent;
            while (parent != null)
            {
                if (parent.NextNode != null)
                {
                    int px = parent.GetLinesX() - indent + ladjust;
                    dc.DrawLine(pDash, px, node.Bounds.Top, px, node.Bounds.Bottom);
                }
                parent = parent.Parent;
            }
        }

        private void DrawNodeImage(TreeNode node, GContext dc, Rectangle clip, int x, int y)
        {
            if (!RectsIntersect(clip, x, y, ImageList.ImageSize.Width, ImageList.ImageSize.Height))
                return;

            int use_index = node.Image;

            if (use_index > -1 && use_index < ImageList.Images.Count)
                ImageList.Draw(dc, x, y, ImageList.ImageSize.Width, ImageList.ImageSize.Height, use_index);
        }

        private void LabelEditFinished(object sender, EventArgs e)
        {
            EndEdit(edit_node);
        }

        internal void BeginEdit(TreeNode node)
        {
            if (edit_node != null)
                EndEdit(edit_node);

            if (edit_text_box == null)
            {
                /*edit_text_box = new NeoLabelEditTextBox();
				edit_text_box.BorderStyle = BorderStyle.FixedSingle;
				edit_text_box.Visible = false;
				edit_text_box.EditingCancelled += new EventHandler(LabelEditCancelled);
				edit_text_box.EditingFinished += new EventHandler(LabelEditFinished);
				edit_text_box.TextChanged += new EventHandler(LabelTextChanged);
				Controls.Add(edit_text_box);*/
            }

            node.EnsureVisible();

            edit_text_box.Bounds = node.Bounds;
            edit_text_box.Text = node.Text;
            edit_text_box.IsHide = false;
            edit_text_box.Focus();
            //edit_text_box.SelectAll();

            edit_args = new NodeLabelEditEventArgs(node);
            OnBeforeLabelEdit(edit_args);

            edit_node = node;

            if (edit_args.CancelEdit)
            {
                edit_node = null;
                EndEdit(node);
            }
        }

        private void LabelEditCancelled(object sender, EventArgs e)
        {
            edit_args.SetLabel(null);
            EndEdit(edit_node);
        }

        private void LabelTextChanged(object sender, EventArgs e)
        {
            //int width = TextRenderer.MeasureText(edit_text_box.Text, edit_text_box.Font).Width + 4;
            int width = (int)System.Math.Ceiling(WHUD.LibContext.MeasureTextWidth(edit_text_box.Text, edit_text_box.Font)) + 4;
            edit_text_box.Size = new Size(width, edit_text_box.Size.Height); // .Width = width;

            if (edit_args != null)
                edit_args.SetLabel(edit_text_box.Text);
        }

        internal void EndEdit(TreeNode node)
        {
            if (edit_text_box != null && edit_text_box.VisibleAfterClipping)
            {
                edit_text_box.IsHide = true;
                Focus();
            }

            //
            // If we get a call to BeginEdit from any AfterLabelEdit handler,
            // the focus seems to always remain in the TreeViewCanvas. This call seems
            // to synchronize the focus events - I don't like it but it works
            //
            //Application.DoEvents();

            if (edit_node != null && edit_node == node)
            {
                edit_node = null;

                var e = new NodeLabelEditEventArgs(edit_args.Node, edit_args.Label);

                OnAfterLabelEdit(e);

                if (e.CancelEdit)
                    return;

                if (e.Label != null)
                    e.Node.Text = e.Label;
            }

            // EndEdit ends editing even if not called on the editing node
            edit_node = null;
            UpdateNode(node);
        }

        internal void CancelEdit(TreeNode node)
        {
            edit_args.SetLabel(null);

            if (edit_text_box != null && edit_text_box.VisibleAfterClipping)
            {
                edit_text_box.IsHide = true;
                Focus();
            }

            edit_node = null;
            UpdateNode(node);
        }

        internal int GetNodeWidth(TreeNode node)
        {
            NanoFont font = node.NodeFont;
            if (node.NodeFont == null)
                font = Font;

            return (int)WHUD.LibContext.MeasureTextWidth(node.Text, font) + 3;
        }

        private void DrawSelectionAndFocus(TreeNode node, GContext dc, Rectangle r)
        {
            if (IsFocused && focused_node == node && !full_row_select)
            {
                //ControlPaint.DrawFocusRectangle(dc, r, ForeColor, BackColor);
            }
            if (draw_mode != TreeViewDrawMode.Normal)
                return;

            r.Inflate(-1, -1);

            if (node == highlighted_node)
            {
                Color back_color;

                if (IsFocused)
                {
                    // Use the node's BackColor if is not empty, and is not actually the selected one (yet)
                    back_color = (node != selected_node && node.BackColor != Color.Empty)
                        ? node.BackColor : ThemeEngine.ColorHighlight;
                }
                else
                {
                    // TextColor del fondo del nodo seleccionado cuando el TreeView no tiene el foco.
                    back_color = ThemeEngine.ColorControl; // Color.FromArgb(57, 60, 68); //TextColor.DimGray;
                }
                dc.FillRectangle(new SolidBrush(back_color), r);

            }
            else if (!hide_selection && node == highlighted_node)
            {
                dc.FillRectangle(new NanoSolidBrush(SystemColors.Control), r);
            }
            else
            {
                // If selected_node is not the current highlighted one, use the color of the TreeViewCanvas
                Color back_color = (node == selected_node) ? BackColor : node.BackColor;
                dc.FillRectangle(new SolidBrush(back_color), r);
            }
        }

        private void DrawStaticNode(TreeNode node, GContext dc)
        {
            if (!full_row_select || show_lines)
                DrawSelectionAndFocus(node, dc, node.Bounds);

            NanoFont font = node.NodeFont;
            if (node.NodeFont == null)
                font = Font;
            Color text_color = (IsFocused && node == highlighted_node ?
                                ThemeEngine.ColorHighlightText : node.ForeColor);
            if (text_color.IsEmpty)
                text_color = ForeColor;

            int tAscender = (int)System.Math.Ceiling(font.Ascender);
            dc.DrawString(node.Text, font,
                          new NanoSolidBrush(text_color),
                          node.Bounds.Location.X,
                          node.Bounds.Location.Y + tAscender);
        }

        private void DrawTreeNode(TreeNode node, GContext dc, Rectangle clip)
        {
            int child_count = node.nodes.Count;
            int y = node.GetY();
            int middle = y + (ActualItemHeight / 2);

            if (full_row_select && !show_lines)
            {
                var r = new Rectangle(1, y, ViewportRectangle.Width - 2, ActualItemHeight);
                DrawSelectionAndFocus(node, dc, r);
            }

            if (draw_mode == TreeViewDrawMode.Normal
                || draw_mode == TreeViewDrawMode.OwnerDrawText)
            {
                if ((show_root_lines || node.Parent != null) && show_plus_minus && child_count > 0)
                {
                    bool fillBackColor = !(full_row_select && !show_lines);
                    ThemeEngine.TreeViewDrawNodePlusMinus(this, node, dc,
                                                          node.GetLinesX() - Indent + 5,
                                                          middle, fillBackColor);
                }

                if (checkboxes && state_image_list == null)
                    DrawNodeCheckBox(node, dc, CheckBoxLeft(node) - 3, middle);

                if (checkboxes && state_image_list != null)
                    DrawNodeState(node, dc, CheckBoxLeft(node) - 3, y);

                if (!checkboxes && node.StateImage != null)
                    dc.DrawImage(node.StateImage, new Rectangle(CheckBoxLeft(node) - 3, y, 16, 16));

                if (show_lines)
                    DrawNodeLines(node, dc, clip, dash, node.GetLinesX(), y, middle);

                if (ImageList != null)
                    DrawNodeImage(node, dc, clip, node.GetImageX(), y);
            }

            if (draw_mode != TreeViewDrawMode.Normal)
            {
                dc.FillRectangle(new NanoSolidBrush(Color.White), node.Bounds);
                TreeNodeStates tree_node_state = TreeNodeStates.Default; ;
                if (node.IsSelected)
                    tree_node_state = TreeNodeStates.Selected;
                if (node.Checked)
                    tree_node_state |= TreeNodeStates.Checked;
                if (node == focused_node)
                    tree_node_state |= TreeNodeStates.Focused;
                Rectangle node_bounds = node.Bounds;
                if (draw_mode == TreeViewDrawMode.OwnerDrawText)
                {
                    node_bounds.X += 3;
                    node_bounds.Y += 1;
                }
                else
                {
                    node_bounds.X = 0;
                    node_bounds.Width = Width;
                }

                var e = new DrawTreeNodeEventArgs(dc, node, node_bounds, tree_node_state);

                OnDrawNode(e);
                if (!e.DrawDefault)
                    return;
            }

            if (!node.IsEditing)
                DrawStaticNode(node, dc);
        }

        void BarSetValues(ScrollBarBase sb, int max, int large)
        {
            sb.Maximum = max;
            sb.LargeChange = large;
        }

        internal void UpdateScrollBars(bool force)
        {
            if (!force
                && (update_stack > 0 || IsHide)
                || (ClientRect.Width <= 0 || ClientRect.Height <= 0))
                return;

            bool vert = false;
            bool horz = false;
            int height = 0;
            int width = -1;

            int auxItem_height = ActualItemHeight;
            if (scrollable)
            {
                var walk = new OpenTreeNodeEnumerator(root_node);

                while (walk.MoveNext())
                {
                    int r = walk.CurrentNode.Bounds.Right;
                    if (r > width)
                        width = r;

                    height += auxItem_height;
                }

                height -= auxItem_height; // root_node adjustment
                width += hbar_offset;

                if (height > ClientRect.Height)
                {
                    vert = true;

                    if (width > ClientRect.Width) // - SystemInformation.VerticalScrollBarWidth)
                        horz = true;
                }
                else if (width > ClientRect.Width)
                {
                    horz = true;
                }

                if (!vert && horz && height > ClientRect.Height) // - SystemInformation.HorizontalScrollBarHeight)
                    vert = true;
            }

            if (vert)
            {
                int visible_height = ClientRect.Height; // horz ? ClientRect.Height - hbar.Height : ClientRect.Height;
                BarSetValues(vbar, System.Math.Max(0, max_visible_order - 2), visible_height / ActualItemHeight);
                /*
				vbar.Maximum = max_visible_order;
				vbar.LargeChange = ClientRectangle.Height / ItemHeight;
				 */

                if (!vbar_bounds_set)
                {
                    //vbar.Bounds = new Rectangle(ClientRect.Width - vbar.Width, 0, vbar.Width,
                    //                            ClientRect.Height -
                    //                            (horz ? SystemInformation.VerticalScrollBarWidth : 0));
                    vbar_bounds_set = true;

                    // We need to recalc the hbar if the vbar is now visible
                    hbar_bounds_set = false;
                }


                vbar.IsHide = false;
                if (skipped_nodes > 0)
                {
                    int skip = System.Math.Min(skipped_nodes, vbar.Maximum - VisibleCount + 1);
                    skipped_nodes = 0;
                    //BarSetValues(vbar, SafeValueSet(skip);
                    skipped_nodes = skip;
                }
            }
            else
            {
                skipped_nodes = 0;
                RecalculateVisibleOrder(root_node);
                vbar.IsHide = true;
                SetVScrollValue(0);
                vbar_bounds_set = false;
            }

            if (horz)
            {
                BarSetValues(hbar, width + 1, ClientRect.Width /*- (vert ? SystemInformation.VerticalScrollBarWidth : 0)*/);
                /*
				hbar.LargeChange = ClientRectangle.Width;
				hbar.Maximum = width + 1;
				 */

                if (!hbar_bounds_set)
                {
                    //hbar.Bounds = new Rectangle(0, ClientRect.Height - hbar.Height,
                    //                            ClientRect.Width - (vert ? SystemInformation.VerticalScrollBarWidth : 0),
                    //                            hbar.Height);
                    hbar_bounds_set = true;
                }
                hbar.IsHide = false;
            }
            else
            {
                hbar_offset = 0;
                hbar.IsHide = true;
                hbar_bounds_set = false;
            }
        }

        private void SizeChangedHandler(object sender, EventArgs e)
        {
            //if (IsHandleCreated)
            {
                if (max_visible_order == -1)
                    RecalculateVisibleOrder(root_node);
                UpdateScrollBars(true);
            }

            if (!vbar.IsHide)
            {
                //vbar.Bounds = new Rectangle(ClientRect.Width - vbar.Width, 0, vbar.Width,
                //                            ClientRect.Height - (!hbar.IsHide ? hbar.Height : 0));
            }

            if (!hbar.IsHide)
            {
                //hbar.Bounds = new Rectangle(0, ClientRect.Height - hbar.Height,
                //                            ClientRect.Width - (!vbar.IsHide ? vbar.Width : 0), hbar.Height);
            }
        }

        private void ScrollBarVValueChanged(object sender, EventArgs e)
        {
            EndEdit(edit_node);

            SetVScrollPos(vbar.Value, null);
        }

        private void SetVScrollPos(int pos, TreeNode new_top)
        {
            if (vbar.IsHide)
                return;

            if (pos < 0)
                pos = 0;

            if (skipped_nodes == pos)
                return;

            int diff = skipped_nodes - pos;
            skipped_nodes = pos;

            //if (!IsHandleCreated)
            //	return;

            int y_move = diff * ActualItemHeight;
            //XplatUI.ScrollWindow(Handle, ViewportRectangle, 0, y_move, false);
        }

        private void ScrollBarHValueChanged(object sender, EventArgs e)
        {
            EndEdit(edit_node);

            int old_offset = hbar_offset;
            hbar_offset = hbar.Value;

            if (hbar_offset < 0)
            {
                hbar_offset = 0;
            }

            //XplatUI.ScrollWindow(Handle, ViewportRectangle, old_offset - hbar_offset, 0, false);
        }

        /// <summary>
        /// Expandir abajo
        /// </summary>
        /// <param name="node"></param>
        /// <param name="count_to_next"></param>
        internal void ExpandBelow(TreeNode node, int count_to_next)
        {
            if (update_stack > 0)
            {
                update_needed = true;
                return;
            }

            // If node Bottom is less than 0, the node is above and not visible,
            // and we need to scroll the entire viewport
            int node_bottom = node.Bounds.Bottom >= 0 ? node.Bounds.Bottom : 0;
            var below = new Rectangle(0, node_bottom, ViewportRectangle.Width,
                                      ViewportRectangle.Height - node_bottom);

            int amount = count_to_next * ActualItemHeight;

            //if (amount > 0)
            //	XplatUI.ScrollWindow(Handle, below, 0, amount, false);

            if (show_plus_minus)
            {
                //Invalidate(new Rectangle(0, node.GetY(), Width, ActualItemHeight));
                // Miki
                int ny = node.GetY();
                int h = ViewportRectangle.Height - amount + ActualItemHeight;
                Invalidate(new Rectangle(0, ny, Width, amount + ActualItemHeight + h));
            }
        }

        /// <summary>
        /// Contraer abajo
        /// </summary>
        /// <param name="node"></param>
        /// <param name="count_to_next"></param>
        internal void CollapseBelow(TreeNode node, int count_to_next)
        {
            if (update_stack > 0)
            {
                update_needed = true;
                return;
            }

            var below = new Rectangle(0, node.Bounds.Bottom, ViewportRectangle.Width,
                                      ViewportRectangle.Height - node.Bounds.Bottom);

            int amount = count_to_next * ActualItemHeight;

            //if (amount > 0)
            //	XplatUI.ScrollWindow(Handle, below, 0, -amount, false);

            if (show_plus_minus)
            {
                //Invalidate(new Rectangle(0, node.GetY(), Width, ActualItemHeight));
                // Miki
                Invalidate(below);
            }
        }

        public override void OnMouseWheel(MouseEventArgs e)
        {
            if (vbar == null || vbar.IsHide)
            {
                return;
            }

            if (e.DeltaWheel <= 0)
            {
                SetVScrollValue(System.Math.Min(vbar.Value + MouseConfig.MouseWheelScrollLines, vbar.Maximum - VisibleCount + 1));
            }
            else
            {
                SetVScrollValue(System.Math.Max(0, vbar.Value - MouseConfig.MouseWheelScrollLines));
            }
        }

        private void VisibleChangedHandler(object sender, EventArgs e)
        {
            if (!IsHide)
            {
                UpdateScrollBars(false);
            }
        }

        private void FontChangedHandler(object sender, EventArgs e)
        {
            //if (IsHandleCreated)
            {
                TreeNode top = TopNode;
                InvalidateNodeWidthRecursive(root_node);

                SetTop(top);
            }
        }

        private void InvalidateNodeWidthRecursive(TreeNode node)
        {
            node.InvalidateWidth();
            foreach (TreeNode child in node.Nodes)
            {
                InvalidateNodeWidthRecursive(child);
            }
        }

        private void GotFocusHandler(object sender, EventArgs e)
        {
            if (selected_node == null)
            {
                if (pre_selected_node != null)
                {
                    SelectedNode = pre_selected_node;
                    return;
                }

                SelectedNode = TopNode;

            }
            else if (selected_node != null)
                UpdateNode(selected_node);
        }

        private void LostFocusHandler(object sender, EventArgs e)
        {
            UpdateNode(SelectedNode);
        }

        private void MouseDownHandler(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButton.Right)
            {
                Focus();
                return;
            }

            TreeNode node = GetNodeAt(e.Y);
            if (node == null)
                return;

            mouse_click_node = node;
            Focus();

            if (show_plus_minus && IsPlusMinusArea(node, e.X) && e.Button == MouseButton.Left)
            {
                node.Toggle();
                UpdateScrollBars(true);
                Invalidate();
                return;
            }
            else if (checkboxes && IsCheckboxArea(node, e.X) && e.Button == MouseButton.Left)
            {
                node.check_reason = TreeViewAction.ByMouse;
                node.Checked = !node.Checked;
                UpdateNode(node);
                return;
            }
            else if (IsSelectableArea(node, e.X) || full_row_select)
            {
                TreeNode old_highlighted = highlighted_node;
                highlighted_node = node;
                if (label_edit && e.Clicks == 1 && highlighted_node == old_highlighted && e.Button == MouseButton.Left)
                {
                    BeginEdit(node);
                }
                else if (highlighted_node != focused_node)
                {
                    Size ds = MouseConfig.DragSize;
                    mouse_rect.X = e.X - ds.Width;
                    mouse_rect.Y = e.Y - ds.Height;
                    mouse_rect.Width = ds.Width * 2;
                    mouse_rect.Height = ds.Height * 2;

                    select_mmove = true;
                }

                Invalidate(highlighted_node.Bounds);
                if (old_highlighted != null)
                    Invalidate(Bloat(old_highlighted.Bounds));

                drag_begin_x = e.X;
                drag_begin_y = e.Y;
            }
        }

        private void MouseUpHandler(object sender, MouseEventArgs e)
        {
            TreeNode node = GetNodeAt(e.Y);

            if (node != null && node == mouse_click_node)
            {
                if (e.Clicks == 2)
                    OnNodeMouseDoubleClick(new TreeNodeMouseClickEventArgs(node, e.Button, e.Clicks, e.X, e.Y));
                else
                    OnNodeMouseClick(new TreeNodeMouseClickEventArgs(node, e.Button, e.Clicks, e.X, e.Y));
            }

            mouse_click_node = null;

            drag_begin_x = -1;
            drag_begin_y = -1;

            if (!select_mmove)
                return;

            select_mmove = false;

            if (e.Button == MouseButton.Right && selected_node != null)
            {
                Invalidate(highlighted_node.Bounds);
                highlighted_node = selected_node;
                Invalidate(selected_node.Bounds);
                return;
            }

            var ce = new TreeViewCancelEventArgs(highlighted_node, false, TreeViewAction.ByMouse);
            OnBeforeSelect(ce);

            Rectangle invalid;
            if (!ce.Cancel)
            {
                TreeNode prev_focused_node = focused_node;
                TreeNode prev_highlighted_node = highlighted_node;

                selected_node = highlighted_node;
                focused_node = highlighted_node;
                OnAfterSelect(new TreeViewEventArgs(selected_node, TreeViewAction.ByMouse));

                if (prev_highlighted_node != null)
                {
                    if (prev_focused_node != null)
                    {
                        invalid = Rectangle.Union(Bloat(prev_focused_node.Bounds),
                                                  Bloat(prev_highlighted_node.Bounds));
                    }
                    else
                    {
                        invalid = Bloat(prev_highlighted_node.Bounds);
                    }

                    invalid.X = 0;
                    invalid.Width = ViewportRectangle.Width;

                    Invalidate(invalid);
                }

            }
            else
            {
                if (highlighted_node != null)
                    Invalidate(highlighted_node.Bounds);

                highlighted_node = focused_node;
                selected_node = focused_node;
                if (selected_node != null)
                    Invalidate(selected_node.Bounds);
            }
        }

        private void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            // XXX - This should use HitTest and only fire when we are over
            // the important parts of a node, not things like gridlines or
            // whitespace
            TreeNode tn = GetNodeAt(e.Location);

            if (tn != tooltip_currently_showing)
                MouseLeftItem(tooltip_currently_showing);

            if (tn != null && tn != tooltip_currently_showing)
                MouseEnteredItem(tn);

            if (e.Button == MouseButton.Left || e.Button == MouseButton.Right)
            {
                if (drag_begin_x != -1 && drag_begin_y != -1)
                {
                    double rise = System.Math.Pow(drag_begin_x - e.X, 2);
                    double run = System.Math.Pow(drag_begin_y - e.Y, 2);
                    double move = System.Math.Sqrt(rise + run);
                    if (move > 3)
                    {
                        TreeNode drag = GetNodeAtUseX(e.X, e.Y);

                        if (drag != null)
                        {
                            OnItemDrag(new ItemDragEventArgs(e.Button, drag));
                        }
                        drag_begin_x = -1;
                        drag_begin_y = -1;
                    }
                }

            }

            // If there is enough movement before the mouse comes up,
            // selection is reverted back to the originally selected node
            if (!select_mmove || mouse_rect.Contains(e.X, e.Y))
                return;

            Invalidate(highlighted_node.Bounds);
            if (selected_node != null)
                Invalidate(selected_node.Bounds);
            if (focused_node != null)
                Invalidate(focused_node.Bounds);

            highlighted_node = selected_node;
            focused_node = selected_node;

            select_mmove = false;
        }

        private void DoubleClickHandler(object sender, MouseEventArgs e)
        {
            TreeNode node = GetNodeAtUseX(e.X, e.Y);
            if (node != null && node.Nodes.Count > 0)
            {
                node.Toggle();
            }
        }

        private bool RectsIntersect(Rectangle r, int left, int top, int width, int height)
        {
            return !((r.Left > left + width) || (r.Right < left) ||
                     (r.Top > top + height) || (r.Bottom < top));
        }
        /*
				// Return true if message was handled, false to send it to base
				private bool WmContextMenu(ref Message m)
				{
					Point pt;
					NeoTreeNode tn;

					int L = (int)m.LParam.ToInt32() & 0xffff;

					int H = (int)(m.LParam.ToInt32() & 0xffff0000);

					//pt = new Point(LowOrder((int)m.LParam.ToInt32()), HighOrder((int)m.LParam.ToInt32()));
					pt = new Point(L, H);

					// This means it's a keyboard menu request
					if (pt.X == -1 || pt.Y == -1)
					{
						tn = SelectedNode;

						if (tn == null)
							return false;

						pt = new Point(tn.Bounds.Left, tn.Bounds.Top + (tn.Bounds.Height / 2));
					}
					else
					{
						pt = PointToClient(pt);

						tn = GetNodeAt(pt);

						if (tn == null)
							return false;
					}

					// At this point, we have a valid TreeNode
					if (tn.ContextMenu != null)
					{
						tn.ContextMenu.Show(this, pt);
						return true;
					}
					else if (tn.ContextMenuStrip != null)
					{
						tn.ContextMenuStrip.Show(this, pt);
						return true;
					}

					// The node we found did not have its own menu, let the parent try to display its menu
					return false;
				}
		 */

        #region Stuff for ToolTips

        private void MouseEnteredItem(TreeNode item)
        {
            tooltip_currently_showing = item;

            if (!is_hovering)
                return;

            //if (ShowNodeToolTips && !string.IsNullOrEmpty(tooltip_currently_showing.ToolTipText))
            //	ToolTipWindow.Present(this, tooltip_currently_showing.ToolTipText);

            OnNodeMouseHover(new TreeNodeMouseHoverEventArgs(tooltip_currently_showing));
        }

        private void MouseLeftItem(TreeNode item)
        {
            ToolTipWindow.Hide();
            tooltip_currently_showing = null;
        }

        private ToolTip ToolTipWindow
        {
            get
            {
                if (tooltip_window == null)
                    tooltip_window = new ToolTip();

                return tooltip_window;
            }
        }
        #endregion

        #endregion // Internal & Private Methods and Properties

        #region Events

        /*public event ItemDragEventHandler ItemDrag
		{
			add { Events.AddHandler(ItemDragEvent, value); }
			remove { Events.RemoveHandler(ItemDragEvent, value); }
		}*/

#pragma warning disable CS0067

        public event TreeViewEventHandler AfterCheck;

        public event TreeViewEventHandler AfterCollapse;

        public event TreeViewEventHandler AfterExpand;

        public event NodeLabelEditEventHandler AfterLabelEdit;

        public event TreeViewEventHandler AfterSelect;

        public event TreeViewCancelEventHandler BeforeCheck;

        public event TreeViewCancelEventHandler BeforeCollapse;

        public event TreeViewCancelEventHandler BeforeExpand;

        public event NodeLabelEditEventHandler BeforeLabelEdit;

        public event TreeViewCancelEventHandler BeforeSelect;

        public event DrawTreeNodeEventHandler DrawNode;

        public event TreeNodeMouseClickEventHandler NodeMouseClick;

        public event TreeNodeMouseClickEventHandler NodeMouseDoubleClick;

        public event TreeNodeMouseHoverEventHandler NodeMouseHover;

        public event EventHandler RightToLeftLayoutChanged;

        public event EventHandler BackgroundImageChanged;

        public event EventHandler BackgroundImageLayoutChanged;

        public event EventHandler PaddingChanged;

        public event PaintEventHandler Paint;

        public event EventHandler TextChanged;

        #endregion Events
    }

    [Serializable]
    public class OwnerDrawPropertyBag : MarshalByRefObject, ISerializable
    {

        private Color fore_color;
        private Color back_color;
        private NanoFont font;

        internal OwnerDrawPropertyBag()
        {
            fore_color = back_color = Color.Empty;
        }

        private OwnerDrawPropertyBag(Color fore_color, Color back_color, NanoFont font)
        {
            this.fore_color = fore_color;
            this.back_color = back_color;
            this.font = font;
        }

        protected OwnerDrawPropertyBag(SerializationInfo info, StreamingContext context)
        {
            SerializationInfoEnumerator en;
            SerializationEntry e;

            en = info.GetEnumerator();

            while (en.MoveNext())
            {
                e = en.Current;
                switch (e.Name)
                {
                    case "Font": font = (NanoFont)e.Value; break;
                    case "ForeColor": fore_color = (Color)e.Value; break;
                    case "BackColor": back_color = (Color)e.Value; break;
                }
            }
        }


        public Color ForeColor
        {
            get { return fore_color; }
            set { fore_color = value; }
        }

        public Color BackColor
        {
            get { return back_color; }
            set { back_color = value; }
        }

        public NanoFont Font
        {
            get { return font; }
            set { font = value; }
        }

        public virtual bool IsEmpty()
        {
            return (font == null && fore_color.IsEmpty && back_color.IsEmpty);
        }

        void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
        {
            si.AddValue("BackColor", BackColor);
            si.AddValue("ForeColor", ForeColor);
            si.AddValue("Font", Font);
        }

        public static OwnerDrawPropertyBag Copy(OwnerDrawPropertyBag value)
        {
            return new OwnerDrawPropertyBag(value.ForeColor, value.BackColor, value.Font);
        }
    }
}
