using System;
using System.Collections.Generic;
using System.Drawing;
using NthDimension.Forms.Events;

namespace NthDimension.Forms.Widgets
{
    public class ListView : Widget
    {
        #region events
        /// <summary>
        /// Rised when the control need to draw a column
        /// </summary>
        //[Description("Rised when the control need to draw a column. The column information will be sent along with this event args"), Category("ManagedListView")]
        public event EventHandler<ListViewColumnDrawArgs> DrawColumn;
        /// <summary>
        /// Rised when the control need to draw an item. The item information will be sent along with this event args. Please note that this event rised only with Thumbnails View Mode.
        /// </summary>
        //[Description("Rised when the control need to draw an item. The item information will be sent along with this event args. Please note that this event rised only with Thumbnails View Mode."), Category("ManagedListView")]
        public event EventHandler<ListViewItemDrawArgs> DrawItem;
        /// <summary>
        /// Rised when the control need to draw a sub item
        /// </summary>
        //[Description("Rised when the control need to draw a sub item. The sub item information will be sent along with this event args. NOTE: rised only if the sub item draw mode property equal None."), Category("ManagedListView")]
        public event EventHandler<ListViewSubItemDrawArgs> DrawSubItem;
        /// <summary>
        /// Rised when the mouse is over a sub item
        /// </summary>
        //[Description("Rised when the mouse get over a sub item."), Category("ManagedListView")]
        public event EventHandler<ListViewMouseOverSubItemArgs> MouseOverSubItem;
        /// <summary>
        /// Rised when the item selection changed
        /// </summary>
        //[Description("Rised when the user select/unselect items."), Category("ManagedListView")]
        public event EventHandler SelectedIndexChanged;
        /// <summary>
        /// Rised when the user clicks a column
        /// </summary>
        //[Description("Rised when the user click on column."), Category("ManagedListView")]
        public event EventHandler<ListViewColumnClickArgs> ColumnClicked;
        /// <summary>
        /// Rised when the user pressed the return key
        /// </summary>
        //[Description("Rised when the user pressed the return key."), Category("ManagedListView")]
        public event EventHandler EnterPressed;
        /// <summary>
        /// Rised when the user double click on item
        /// </summary>
        //[Description("Rised when the user double click on item"), Category("ManagedListView")]
        public event EventHandler<ListViewItemDoubleClickArgs> ItemDoubleClick;
        /// <summary>
        /// Rised when the control needs to shwitch to the columns context menu
        /// </summary>
        //[Description("Rised when the control needs to shwitch to the columns context menu"), Category("ManagedListView")]
        public event EventHandler SwitchToColumnsContextMenu;
        /// <summary>
        /// Rised when the control needs to shwitch to the normal context menu
        /// </summary>
        //[Description("Rised when the control needs to shwitch to the normal context menu"), Category("ManagedListView")]
        public event EventHandler SwitchToNormalContextMenu;
        /// <summary>
        /// Rised when the user finished resizing a column
        /// </summary>
        //[Description("Rised when the user finished resizing a column"), Category("ManagedListView")]
        public event EventHandler AfterColumnResize;
        /// <summary>
        /// Rised when the user draged item(s)
        /// </summary>
        //[Description("Rised when the user draged item(s)"), Category("ManagedListView")]
        public event EventHandler ItemsDrag;
        /// <summary>
        /// Rised when the user changed the view mode
        /// </summary>
        //[Description("Rised when the user changed the view mode"), Category("ManagedListView")]
        public event EventHandler ViewModeChanged;

        #endregion

        private EListViewViewMode viewMode = EListViewViewMode.Details;
        private int wheelScrollSpeed = 20;

        public ListView()
        {
            InitializeComponent();
            FGColor = Color.WhiteSmoke;
            BGColor = Color.FromArgb(28, 30, 31);

            hScrollBar1.BGColor = vScrollBar1.BGColor = Color.FromArgb(51, 51, 51);
            hScrollBar1.ThumbColor = vScrollBar1.FGColor = Color.FromArgb(77, 77, 77);
            hScrollBar1.Size = new Size(0, 13);
            vScrollBar1.Size = new Size(0, 13);

            OverItemColor = Color.FromArgb(57, 60, 68);
            ColumnsColor = Color.FromArgb(77, 77, 77);
            ColumnsTextColor = Color.WhiteSmoke;
            LinesColor = Color.LightGray;
            ColumnsSubLineColor = Color.Black;
            ListViewRectColor = OverItemColor;
            SelectedItemColor = Color.FromArgb(135, 12, 62);

            wheelScrollSpeed = ListViewPanel1.GetItemHeight();

            ListViewPanel1.items.ItemAddedEvent += Value_ItemAdded;
        }

        private ScrollBarH hScrollBar1;
        private ListViewPanel ListViewPanel1;
        private ScrollBarV vScrollBar1;

        private void InitializeComponent()
        {
            vScrollBar1 = new ScrollBarV();
            hScrollBar1 = new ScrollBarH();
            ListViewPanel1 = new ListViewPanel(this);
            SuspendLayout();
            // 
            // vScrollBar1
            // 
            this.vScrollBar1.Dock = EDocking.Right;
            this.vScrollBar1.Location = new Point(477, 0);
            this.vScrollBar1.Name = "vScrollBar1";
            this.vScrollBar1.Size = new Size(17, 316);
            this.vScrollBar1.Scroll += vScrollBar1_Scroll;
            this.vScrollBar1.KeyDownEvent += hScrollBar1_KeyDown;
            // 
            // hScrollBar1
            // 
            this.hScrollBar1.Dock = EDocking.Bottom;
            this.hScrollBar1.Location = new Point(0, 316);
            this.hScrollBar1.Name = "hScrollBar1";
            this.hScrollBar1.Size = new Size(494, 17);
            this.hScrollBar1.SmallChange = 0;
            this.hScrollBar1.Scroll += hScrollBar1_Scroll;
            this.hScrollBar1.KeyDownEvent += hScrollBar1_KeyDown;
            // 
            // ListViewPanel1
            // 
            this.ListViewPanel1.AllowColumnsReorder = true;
            this.ListViewPanel1.ChangeColumnSortModeWhenClick = true;
            this.ListViewPanel1.DrawHighlight = true;
            this.ListViewPanel1.viewMode = EListViewViewMode.Details;

            this.ListViewPanel1.Dock = EDocking.Fill;
            this.ListViewPanel1.Location = new Point(0, 0);
            this.ListViewPanel1.Name = "ListViewPanel1";
            this.ListViewPanel1.Size = new Size(477, 316);
            this.ListViewPanel1.Name = "ListViewPanel1";
            this.ListViewPanel1.RefreshValues += ListViewPanel1_RefreshValues;
            this.ListViewPanel1.ClearScrolls += ListViewPanel1_ClearScrolls;
            this.ListViewPanel1.AdvanceVScrollRequest += ListViewPanel1_AdvanceVScrollRequest;
            this.ListViewPanel1.ReverseVScrollRequest += ListViewPanel1_ReverseVScrollRequest;
            this.ListViewPanel1.RefreshScrollBars += ListViewPanel1_RefreshScrollBars;
            this.ListViewPanel1.SelectedIndexChanged += ListViewPanel1_SelectedIndexChanged;
            this.ListViewPanel1.DrawColumn += ListViewPanel1_DrawColumn;
            this.ListViewPanel1.DrawItem += ListViewPanel1_DrawItem;
            this.ListViewPanel1.DrawSubItem += ListViewPanel1_DrawSubItem;
            this.ListViewPanel1.MouseOverSubItem += ListViewPanel1_MouseOverSubItem;
            this.ListViewPanel1.ColumnClicked += ListViewPanel1_ColumnClicked;
            this.ListViewPanel1.ItemDoubleClick += ListViewPanel1_ItemDoubleClick;
            this.ListViewPanel1.EnterPressedOverItem += ListViewPanel1_EnterPressedOverItem;
            this.ListViewPanel1.SwitchToColumnsContextMenu += ListViewPanel1_SwitchToColumnsContextMenu;
            this.ListViewPanel1.SwitchToNormalContextMenu += ListViewPanel1_SwitchToNormalContextMenu;
            this.ListViewPanel1.AfterColumnResize += ListViewPanel1_AfterColumnResize;
            this.ListViewPanel1.ItemsDrag += ListViewPanel1_ItemsDrag;
            this.ListViewPanel1.ScrollToSelectedItemRequest += ListViewPanel1_ScrollToSelectedItemRequest;
            //this.ListViewPanel1.DragDrop += new System.Windows.Forms.DragEventHandler(this.ManagedListViewPanel1_DragDrop);
            //this.ListViewPanel1.DragEnter += new System.Windows.Forms.DragEventHandler(this.ManagedListViewPanel1_DragEnter);
            //this.ListViewPanel1.DragOver += new System.Windows.Forms.DragEventHandler(this.ManagedListViewPanel1_DragOver);
            //this.ListViewPanel1.DragLeave += new System.EventHandler(this.ManagedListViewPanel1_DragLeave);
            this.ListViewPanel1.KeyDownEvent += ListViewPanel1_KeyDown;
            this.ListViewPanel1.MouseDoubleClickEvent += ListViewPanel1_MouseDoubleClick;
            this.ListViewPanel1.MouseMoveEvent += ListViewPanel1_MouseMove;
            // 
            // ManagedListView
            // 
            Widgets.Add(this.vScrollBar1);
            Widgets.Add(this.hScrollBar1);
            Widgets.Add(this.ListViewPanel1);
            Name = "ManagedListView";
            Size = new Size(494, 333);
            this.PaintEvent += ListView_Paint;
            this.KeyDownEvent += ListViewPanel1_KeyDown;
            MouseEnterEvent += ListView_MouseEnter;
            this.MouseLeaveEvent += ListView_MouseLeave;
            this.SizeChangedEvent += ListView_Resize;

            ResumeLayout();
        }


        #region properties

        public Color OverItemColor
        {
            get;
            set;
        }

        public Color ListViewRectColor
        {
            get;
            set;
        }

        public Color ColumnsColor
        {
            get;
            set;
        }

        public Color ColumnsTextColor
        {
            get;
            set;
        }

        public Color ColumnsSubLineColor
        {
            get;
            set;
        }

        public Color LinesColor
        {
            get;
            set;
        }

        public Color SelectedItemColor
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set the viewmode.
        /// </summary>
        //[Description("The list view mode"), Category("ManagedListView")]
        public EListViewViewMode ViewMode
        {
            get { return viewMode; }
            set
            {
                viewMode = value;
                ListViewPanel1.viewMode = value;
                ListViewPanel1.HscrollOffset = hScrollBar1.Value = 0;
                ListViewPanel1.VscrollOffset = vScrollBar1.Value = 0;
                ListViewPanel1_RefreshScrollBars(this, null);
                ListViewPanel1.Invalidate();

                if (ViewModeChanged != null)
                    ViewModeChanged(this, new EventArgs());

                if (value == EListViewViewMode.Thumbnails)
                    if (SwitchToNormalContextMenu != null)
                        SwitchToNormalContextMenu(this, new EventArgs());
            }
        }

        /// <summary>
        /// Get or set the column collection.
        /// </summary>
        //[Description("The columns collection"), Category("ManagedListView")]
        public ListViewColumnsCollection Columns
        {
            get { return ListViewPanel1.columns; }
            set { ListViewPanel1.columns = value; ListViewPanel1.Invalidate(); }
        }

        /// <summary>
        /// Get or set the items collection.
        /// </summary>
        //[Description("The items collection"), Category("ManagedListView")]
        public ListViewItemsCollection Items
        {
            get { return ListViewPanel1.items; }
            set
            {
                ListViewPanel1.items = value;
                ListViewPanel1.Invalidate();
                value.ItemAddedEvent += Value_ItemAdded;
            }
        }

        private void Value_ItemAdded(object sender, ListViewItemParameter e)
        {
            e.Item.TextColor = FGColor;
        }

        /// <summary>
        /// Get or set if selected items can be draged and droped
        /// </summary>
        //[Description("If enabled, the selected items can be draged and droped"), Category("ManagedListView")]
        public bool AllowItemsDragAndDrop
        { get { return ListViewPanel1.AllowItemsDragAndDrop; } set { ListViewPanel1.AllowItemsDragAndDrop = value; } }

        /// <summary>
        /// Allow columns reorder ? after a column reordered, the index of that column within the columns collection get changed
        /// </summary>
        //[Description("Allow columns reorder ? after a column reordered, the index of that column within the columns collection get changed"), Category("ManagedListView")]
        public bool AllowColumnsReorder
        { get { return ListViewPanel1.AllowColumnsReorder; } set { ListViewPanel1.AllowColumnsReorder = value; } }

        /// <summary>
        /// If enabled, the sort mode of a column get changed when the user clicks that column
        /// </summary>
        //[Description("If enabled, the sort mode of a column get changed when the user clicks that column"), Category("ManagedListView")]
        public bool ChangeColumnSortModeWhenClick
        { get { return ListViewPanel1.ChangeColumnSortModeWhenClick; } set { ListViewPanel1.ChangeColumnSortModeWhenClick = value; } }

        /// <summary>
        /// The thunmbnail height. Work only for thunmbnails view mode.
        /// </summary>
        //[Description("The thunmbnail height. Work only for thumbnails view mode."), Category("ManagedListView")]
        public int ThunmbnailsHeight
        { get { return ListViewPanel1.ThumbnailsHeight; } set { ListViewPanel1.ThumbnailsHeight = value; ListViewPanel1.Invalidate(); } }

        /// <summary>
        /// The thunmbnail width. Work only for thumbnails view mode.
        /// </summary>
        //[Description("The thunmbnail width. Work only for thumbnails view mode."), Category("ManagedListView")]
        public int ThunmbnailsWidth
        { get { return ListViewPanel1.ThumbnailsWidth; } set { ListViewPanel1.ThumbnailsWidth = value; ListViewPanel1.Invalidate(); } }

        /// <summary>
        /// The speed of the scroll when using mouse wheel. Default value is 20.
        /// </summary>
        //[Description("The speed of the scroll when using mouse wheel. Default value is 20."), Category("ManagedListView")]
        public int WheelScrollSpeed
        { get { return wheelScrollSpeed; } set { wheelScrollSpeed = value; } }

        /// <summary>
        /// If enabled, the item get highlighted when the mouse over it
        /// </summary>
        //[Description("If enabled, the item get highlighted when the mouse over it"), Category("ManagedListView")]
        public bool DrawHighlight
        { get { return ListViewPanel1.DrawHighlight; } set { ListViewPanel1.DrawHighlight = value; } }

        /// <summary>
        /// The images list that will be used for draw
        /// </summary>
        //[Description("The images list that will be used for draw"), Category("ManagedListView")]
        public ImageList ImagesList
        { get { return ListViewPanel1.ImagesList; } set { ListViewPanel1.ImagesList = value; } }

        /// <summary>
        /// Get the selected items collection
        /// </summary>
        //[Browsable(false)]
        public List<ListViewItem> SelectedItems
        {
            get
            {
                return ListViewPanel1.SelectedItems;
            }
        }

        /// <summary>
        /// Get or set the font of this control
        /// </summary>
        public override NanoFont Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
                ListViewPanel1.Font = value;
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Scroll view port into item
        /// </summary>
        /// <param name="itemIndex">The item index</param>
        public void ScrollToItem(int itemIndex)
        {
            try
            {
                ListView_Resize(this, null);
                vScrollBar1.Value = ListViewPanel1.GetVscrollValueForItem(itemIndex);
                ListViewPanel1.VscrollOffset = ListViewPanel1.GetVscrollValueForItem(itemIndex);
            }
            catch { }
        }
        #endregion Methods

        private void ListView_MouseEnter(object sender, EventArgs e)
        {
            ListView_Resize(sender, e);
        }

        private void ListView_Resize(object sender, EventArgs e)
        {
            if (ListViewPanel1.viewMode == EListViewViewMode.Details)
            {
                int size = ListViewPanel1.CalculateColumnsSize();
                if (size < this.Width)
                {
                    hScrollBar1.Maximum = 1;
                    ListViewPanel1.HscrollOffset = hScrollBar1.Value = 0;
                    ListViewPanel1.Invalidate();
                    hScrollBar1.IsHide = true;
                }
                else
                {
                    hScrollBar1.Maximum = size - ListViewPanel1.Width + 20;
                    hScrollBar1.IsHide = false;
                }

                size = ListViewPanel1.CalculateItemsSize();
                if (size < this.Height - 18)
                {
                    vScrollBar1.Maximum = 1;
                    ListViewPanel1.VscrollOffset = vScrollBar1.Value = 0;
                    ListViewPanel1.Invalidate();
                    vScrollBar1.IsHide = true;
                }
                else
                {
                    vScrollBar1.Maximum = size - ListViewPanel1.Height + 40;
                    vScrollBar1.IsHide = false;
                }
            }
            else
            {
                int size = ListViewPanel1.CalculateSizeOfItemsAsThumbnails().Height;
                if (size < this.Height - 18)
                {
                    vScrollBar1.Maximum = 1;
                    ListViewPanel1.VscrollOffset = vScrollBar1.Value = 0;
                    ListViewPanel1.Invalidate();
                    vScrollBar1.IsHide = true;
                }
                else
                {
                    vScrollBar1.Maximum = size - ListViewPanel1.Height + 40;
                    vScrollBar1.IsHide = false;
                }

                size = ListViewPanel1.CalculateSizeOfItemsAsThumbnails().Width;
                if (size < this.Width)
                {
                    hScrollBar1.Maximum = 1;
                    ListViewPanel1.HscrollOffset = hScrollBar1.Value = 0;
                    ListViewPanel1.Invalidate();
                    hScrollBar1.IsHide = true;
                }
                else
                {
                    hScrollBar1.Maximum = size - ListViewPanel1.Width + 20;
                    hScrollBar1.IsHide = false;
                }
            }
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            ListViewPanel1.VscrollOffset = vScrollBar1.Value;
            ListViewPanel1.Invalidate();
        }

        private void hScrollBar1_KeyDown(object sender, KeyEventArgs e)
        {
            ListViewPanel1.OnKeyDownRised(e);
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            ListViewPanel1.HscrollOffset = hScrollBar1.Value;
            ListViewPanel1.Invalidate();
        }

        private void ListViewPanel1_RefreshValues(object sender, EventArgs e)
        {
            ListView_Resize(sender, e);
        }

        private void ListViewPanel1_ClearScrolls(object sender, EventArgs e)
        {
            hScrollBar1.Maximum = 1;
            ListViewPanel1.HscrollOffset = hScrollBar1.Value = 0;
            hScrollBar1.Enabled = false;

            vScrollBar1.Maximum = 1;
            ListViewPanel1.VscrollOffset = vScrollBar1.Value = 0;
            vScrollBar1.Enabled = false;

            ListViewPanel1.Invalidate();
        }

        private void ListViewPanel1_AdvanceVScrollRequest(object sender, EventArgs e)
        {
            try
            {
                vScrollBar1.Value += wheelScrollSpeed;
                ListViewPanel1.VscrollOffset += wheelScrollSpeed;
            }
            catch { }
        }

        private void ListViewPanel1_ReverseVScrollRequest(object sender, EventArgs e)
        {
            try
            {
                vScrollBar1.Value -= wheelScrollSpeed;
                ListViewPanel1.VscrollOffset -= wheelScrollSpeed;
            }
            catch { }
        }

        private void ListViewPanel1_RefreshScrollBars(object sender, EventArgs e)
        {
            ListView_Resize(sender, e);
        }

        private void ListViewPanel1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedIndexChanged != null)
                SelectedIndexChanged(this, new EventArgs());
        }

        private void ListViewPanel1_DrawColumn(object sender, ListViewColumnDrawArgs e)
        {
            if (DrawColumn != null)
                DrawColumn(this, e);
        }

        private void ListViewPanel1_DrawItem(object sender, ListViewItemDrawArgs e)
        {
            if (DrawItem != null)
                DrawItem(this, e);
        }

        private void ListViewPanel1_DrawSubItem(object sender, ListViewSubItemDrawArgs e)
        {
            if (DrawSubItem != null)
                DrawSubItem(this, e);
        }

        private void ListViewPanel1_MouseOverSubItem(object sender, ListViewMouseOverSubItemArgs e)
        {
            if (MouseOverSubItem != null)
                MouseOverSubItem(this, e);
        }

        private void ListViewPanel1_ColumnClicked(object sender, ListViewColumnClickArgs e)
        {
            if (ColumnClicked != null)
                ColumnClicked(this, e);
        }

        private void ListViewPanel1_ItemDoubleClick(object sender, ListViewItemDoubleClickArgs e)
        {
            if (ItemDoubleClick != null)
                ItemDoubleClick(this, e);
        }
        private void ListViewPanel1_EnterPressedOverItem(object sender, EventArgs e)
        {
            if (EnterPressed != null)
                EnterPressed(this, new EventArgs());
        }

        private void ListViewPanel1_SwitchToColumnsContextMenu(object sender, EventArgs e)
        {
            if (SwitchToColumnsContextMenu != null)
                SwitchToColumnsContextMenu(this, new EventArgs());
        }

        private void ListViewPanel1_SwitchToNormalContextMenu(object sender, EventArgs e)
        {
            if (SwitchToNormalContextMenu != null)
                SwitchToNormalContextMenu(this, new EventArgs());
        }

        private void ListViewPanel1_AfterColumnResize(object sender, EventArgs e)
        {
            if (AfterColumnResize != null)
                AfterColumnResize(this, new EventArgs());
        }

        private void ListViewPanel1_ItemsDrag(object sender, EventArgs e)
        {
            if (ItemsDrag != null)
                ItemsDrag(this, new EventArgs());
        }

        private void ListViewPanel1_ScrollToSelectedItemRequest(object sender, ListViewItemSelectArgs e)
        {
            ScrollToItem(e.ItemIndex);
        }

        private void ListViewPanel1_KeyDown(object sender, KeyEventArgs e)
        {
            ListViewPanel1.OnKeyDownRised(e);
        }

        private void ListViewPanel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OnMouseDoubleClick(e);
        }

        private void ListViewPanel1_MouseMove(object sender, MouseEventArgs e)
        {
            OnMouseMove(e);
        }

        private void ListView_MouseLeave(object sender, EventArgs e)
        {
            ListViewPanel1.OnMouseLeaveRise();
        }

        private void ListView_Paint(object sender, PaintEventArgs e)
        {
            ListViewPanel1.Invalidate();
            ListView_Resize(sender, null);
        }
    }
}
