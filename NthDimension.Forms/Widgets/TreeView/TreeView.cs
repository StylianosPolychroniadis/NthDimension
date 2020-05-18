using NthDimension.Forms.Delegates;
using NthDimension.Forms.Events;
using NthDimension.Forms.Layout;
using NthDimension.Forms.Themes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms.Widgets
{
    #region Enumerators
    public enum TreeViewAction
    {
        Unknown,
        ByKeyboard,
        ByMouse,
        Collapse,
        Expand
    }

    [Flags]
    public enum TreeNodeStates
    {
        Selected = 1,
        Grayed = 2,
        Checked = 8,
        Focused = 16,
        Default = 32,
        Hot = 64,
        Marked = 128,
        Indeterminate = 256,
        ShowKeyboardCues = 512
    }

    public enum TreeViewDrawMode
    {
        Normal = 0,
        OwnerDrawText = 1,
        OwnerDrawAll = 2
    }

    [Flags]
    public enum TreeViewHitTestLocations
    {
        None = 1,
        Image = 2,
        Label = 4,
        Indent = 8,
        PlusMinus = 16,
        RightOfLabel = 32,
        StateImage = 64,
        AboveClientArea = 256,
        BelowClientArea = 512,
        RightOfClientArea = 1024,
        LeftOfClientArea = 2048
    }
    public enum BorderStyle
    {
        None,
        FixedSingle,
        Fixed3D
    }
    #endregion

    public delegate void SelectionChanged();

   
    public class TreeView : Widget
    {
        public event SelectionChanged OnSelectionChanged;
        public event TreeNodeMouseClickEventHandler NodeMouseClick;
        public event TreeNodeMouseClickEventHandler NodeMouseDoubleClick;
        public event TreeNodeMouseHoverEventHandler NodeMouseHover;

        private TreeNode lastNode;

        readonly TreeViewCanvas iTreeView;
        readonly ScrollBarV iVBar;
        readonly ScrollBarH iHBar;

        public TreeView()
        {
            iHBar = new ScrollBarH { Dock = EDocking.Bottom };
            iVBar = new ScrollBarV { Dock = EDocking.Right };
            iTreeView = new TreeViewCanvas(iHBar, iVBar) { Dock = EDocking.Fill };

            #region iTreeView Events Dispatching
            iTreeView.AfterSelect += delegate
            {
                SelectedNode = iTreeView.SelectedNode;
            };

            iTreeView.NodeMouseClick += delegate (object sender, TreeNodeMouseClickEventArgs e)
            {
                e.Node.OnMouseClick(e);
                if (null != NodeMouseClick)
                    this.NodeMouseClick(sender, e);
            };

            iTreeView.NodeMouseDoubleClick += delegate (object sender, TreeNodeMouseClickEventArgs e)
            {
                e.Node.OnMouseDoubleClick(e);
                if (null != NodeMouseDoubleClick)
                    this.NodeMouseDoubleClick(sender, e);               
            };

            iTreeView.NodeMouseHover += delegate (object sender, TreeNodeMouseHoverEventArgs e)
            {
                if (null != NodeMouseHover)
                    this.NodeMouseHover(sender, e);

                //e.Node.OnMouseEnter(e);
            };
            #endregion iTreeView Events Dispatching

            Widgets.Add(iVBar);
            Widgets.Add(iHBar);
            Widgets.Add(iTreeView);

            PaintBackGround = false;
            iHBar.BGColor = ThemeEngine.BackColor2; // = Color.FromArgb(50, 72, 99);
            iHBar.ThumbColor = FGColor; //TextColor.FromArgb(77, 77, 77);
            iVBar.BGColor = ThemeEngine.BackColor2;
            iVBar.ThumbColor = FGColor; //TextColor.FromArgb(77, 77, 77);
        }

        public TreeViewCanvas Canvas
        {
            get { return iTreeView; }
        }

        public new Color BGColor
        {
            get
            {
                return base.BGColor;
            }
            set
            {
                base.BGColor = value;
                iTreeView.BGColor = value;
            }
        }
        public new Color FGColor
        {
            get { return base.FGColor; }
            set
            {
                base.FGColor = value;
                iTreeView.FGColor = value;
                iHBar.ThumbColor = value;
                iVBar.ThumbColor = value;
            }
        }

        public TreeNodeCollection Nodes
        {
            get { return iTreeView.Nodes; }
        }

        public bool FullRowSelect
        {
            get { return iTreeView.FullRowSelect; }
            set { iTreeView.FullRowSelect = value; }
        }

        public bool CheckBoxes
        {
            get { return iTreeView.CheckBoxes; }
            set { iTreeView.CheckBoxes = value; }
        }

        [DefaultValue(true)]
        public bool ShowLines
        {
            get { return iTreeView.ShowLines; }
            set { iTreeView.ShowLines = value; }
        }

        public TreeNode SelectedNode
        {
            get { return iTreeView.SelectedNode; }
            set
            {
                iTreeView.SelectedNode = value;

                if (lastNode != iTreeView.SelectedNode)
                {
                    lastNode = iTreeView.SelectedNode;

                    if (null != OnSelectionChanged)
                        this.OnSelectionChanged();
                }
            }
        }

        protected override void DoPaint(PaintEventArgs e)
        {
            base.DoPaint(e);

            if (!iHBar.IsHide && !iVBar.IsHide)
            {
                iVBar.Margin = new Spacing(0, 0, 0, iHBar.Height);

                var corner = new Rectangle(Width - iVBar.Width,
                                           Height - iHBar.Height,
                                           iVBar.Width, iHBar.Height);
                e.GC.FillRectangle(new SolidBrush(BGColor), corner);
                //e.GC.DrawRectangle(new NanoPen(TextColor.YellowGreen), corner);
                e.GC.FillRectangle(new NanoSolidBrush(iTreeView.BackColor), corner);
            }
            else
            {
                iVBar.Margin = Spacing.Zero;
            }
        }
    }

    public class TreeViewHitTestInfo
    {

        private TreeNode node;
        private TreeViewHitTestLocations location;

        public TreeViewHitTestInfo(TreeNode hitNode, TreeViewHitTestLocations hitLocation)
        {
            this.node = hitNode;
            this.location = hitLocation;
        }

        public TreeNode Node
        {
            get { return node; }
        }

        public TreeViewHitTestLocations Location
        {
            get { return location; }
        }
    }
}
