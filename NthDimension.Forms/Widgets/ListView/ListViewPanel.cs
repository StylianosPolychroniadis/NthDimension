using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NthDimension.Forms.Events;

namespace NthDimension.Forms.Widgets
{
    public class ListViewPanel : Widget
    {
        ListView iListView;

        /// <summary>
        /// The Advanced ListView panel.
        /// </summary>
        public ListViewPanel(ListView pListView)
        {
            iListView = pListView;

            //ControlStyles flag = ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint;
            //this.SetStyle(flag, true);
            //this.AllowDrop = true;

            items.ItemAddedEvent += items_ItemsCollectionChanged;
            items.ItemRemovedEvent += items_ItemsCollectionChanged;
            columns.ColumnAddedEvent += columns_ColumnsCollectionChanged;
            columns.ColumnRemovedEvent += columns_ColumnsCollectionChanged;
            _StringFormat = new StringFormat();
            columns.CollectionClear += new EventHandler(columns_CollectionClear);
            items.CollectionClearEvent += new EventHandler(columns_CollectionClear);
            _StringFormat = new StringFormat(StringFormatFlags.NoWrap);
            _StringFormat.Trimming = StringTrimming.EllipsisCharacter;
        }

        private StringFormat _StringFormat;
        private Point DownPoint = new Point();
        private Point UpPoint = new Point();
        private Point DownPointAsViewPort = new Point();
        private Point CurrentMousePosition = new Point();
        /// <summary>
        /// The view mode
        /// </summary>
        public EListViewViewMode viewMode = EListViewViewMode.Details;
        /// <summary>
        /// The columns collection
        /// </summary>
        public ListViewColumnsCollection columns = new ListViewColumnsCollection();
        /// <summary>
        /// The items collection
        /// </summary>
        public ListViewItemsCollection items = new ListViewItemsCollection();
        /// <summary>
        /// The images list
        /// </summary>
        public ImageList ImagesList = new ImageList();
        /// <summary>
        /// The selected items collection
        /// </summary>
        public List<ListViewItem> SelectedItems
        {
            get
            {
                List<ListViewItem> selectedItems = new List<ListViewItem>();
                foreach (ListViewItem item in items)
                {
                    if (item.Selected)
                    {
                        selectedItems.Add(item);
                    }
                }
                return selectedItems;
            }
        }
        /// <summary>
        /// The horisontal scroll offset
        /// </summary>
        public int HscrollOffset = 0;
        /// <summary>
        /// The vertical scroll offset value
        /// </summary>
        public int VscrollOffset = 0;
        /// <summary>
        /// Rised when values refresh required.
        /// </summary>
        public event EventHandler RefreshValues;
        /// <summary>
        /// Rised when the control requst a scroll values rest
        /// </summary>
        public event EventHandler ClearScrolls;
        /// <summary>
        /// Indecate whether the user can drag and drop items
        /// </summary>
        public bool AllowItemsDragAndDrop = true;
        /// <summary>
        /// Indecate whether the user can reorder the columns.
        /// </summary>
        public bool AllowColumnsReorder = true;
        /// <summary>
        /// Indecate whether the sort mode value of a column get changed when the user click that column.
        /// </summary>
        public bool ChangeColumnSortModeWhenClick = true;
        /// <summary>
        /// The thumbnail height value
        /// </summary>
        public int ThumbnailsHeight = 36;
        /// <summary>
        /// The thumbnails width value
        /// </summary>
        public int ThumbnailsWidth = 36;
        /// <summary>
        /// The item text height
        /// </summary>
        public int itemTextHeight = 28;
        /// <summary>
        /// Indecate whether to draw item highlight when the mouse over that item.
        /// </summary>
        public bool DrawHighlight = true;
        /// <summary>
        /// The item height
        /// </summary>
        public int itemHeight = 15;
        private EListViewMoveType moveType = EListViewMoveType.None;
        private int selectedColumnIndex = -1;
        private bool highlightSelectedColumn = false;
        private bool highlightItemAsOver = false;
        private int overItemSelectedIndex = 0;
        private int selectedItemIndex = 0;
        private int OldoverItemSelectedIndex = 0;
        private int LatestOverItemSelectedIndex = 0;
        private int originalcolumnWidth = 0;
        private int downX = 0;
        private bool isMouseDown = false;
        private int SelectRectanglex;
        private int SelectRectangley;
        private int SelectRectanglex1;
        private int SelectRectangley1;
        private bool DrawSelectRectangle;
        private bool isSecectingItems = false;
        private bool isMovingColumn = false;
        private int currentColumnMovedIndex = 0;
        private int columnHeight = 24;
        private int columnh = 8;
        private int itemh = 6;
        private int highlightSensitive = 6;
        private int spaceBetweenItemsThunmbailsView = 5;

        #region events
        /// <summary>
        /// Rised when the control requests an advance for vertical scroll value
        /// </summary>
        public event EventHandler AdvanceVScrollRequest;
        /// <summary>
        /// Rised when the control requests a reverse for vertical scroll value
        /// </summary>
        public event EventHandler ReverseVScrollRequest;
        /// <summary>
        /// Rised when the control requests a refresh for scroll bars
        /// </summary>
        public event EventHandler RefreshScrollBars;
        /// <summary>
        /// Rised when selected items value changed
        /// </summary>
        public event EventHandler SelectedIndexChanged;
        /// <summary>
        /// Rised when the control needs to draw column
        /// </summary>
        public event EventHandler<ListViewColumnDrawArgs> DrawColumn;
        /// <summary>
        /// Rised when the control needs to draw item
        /// </summary>
        public event EventHandler<ListViewItemDrawArgs> DrawItem;
        /// <summary>
        /// Rised when the control needs to draw subitem
        /// </summary>
        public event EventHandler<ListViewSubItemDrawArgs> DrawSubItem;
        /// <summary>
        /// Rised when the mouse cursor over a subiem.
        /// </summary>
        public event EventHandler<ListViewMouseOverSubItemArgs> MouseOverSubItem;
        /// <summary>
        /// Rised when a column get clicked
        /// </summary>
        public event EventHandler<ListViewColumnClickArgs> ColumnClicked;
        /// <summary>
        /// Rised when an item double click occures
        /// </summary>
        public event EventHandler<ListViewItemDoubleClickArgs> ItemDoubleClick;
        /// <summary>
        /// Rised when the user presses enter after selecting one item.
        /// </summary>
        public event EventHandler EnterPressedOverItem;
        /// <summary>
        /// Rised when the control requests to show the context menu strip
        /// </summary>
        public event EventHandler<MouseEventArgs> ShowContextMenuStrip;
        /// <summary>
        /// Rised when the control requests to switch into the columns context menu strip
        /// </summary>
        public event EventHandler SwitchToColumnsContextMenu;
        /// <summary>
        /// Rised when the control requests to switch into the normal context menu strip
        /// </summary>
        public event EventHandler SwitchToNormalContextMenu;
        /// <summary>
        /// Rised when a column get resized
        /// </summary>
        public event EventHandler AfterColumnResize;
        /// <summary>
        /// Rised when an item get draged
        /// </summary>
        public event EventHandler ItemsDrag;
        /// <summary>
        /// Rised when the control requests to scroll into given item
        /// </summary>
        public event EventHandler<ListViewItemSelectArgs> ScrollToSelectedItemRequest;
        #endregion
        /// <summary>
        /// Get or set the font
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
                if (RefreshScrollBars != null)
                    RefreshScrollBars(this, null);
                Invalidate();
            }
        }
        private enum EListViewMoveType
        {
            None, ColumnVLine, Column
        }
        /// <summary>
        /// Get item index at cursor point
        /// </summary>
        /// <returns>The item index if found otherwise -1</returns>
        public int GetItemIndexAtCursorPoint()
        {
            return GetItemIndexAtPoint(CurrentMousePosition);
        }
        /// <summary>
        /// Get item index at point
        /// </summary>
        /// <param name="location">The location within the viewport</param>
        /// <returns>The item index if found otherwise -1</returns>
        public int GetItemIndexAtPoint(Point location)
        {
            int index = -1;
            if (viewMode == EListViewViewMode.Details)
            {
                int size = CalculateItemsSize();
                int y = location.Y;
                y -= columnHeight;
                if (y > 0 && y < size)
                {
                    index = (VscrollOffset + y) / itemHeight;
                }
            }
            else
            {
                //thunmbnails view select item
                int offset = VscrollOffset % (spaceBetweenItemsThunmbailsView + ThumbnailsHeight + itemTextHeight);
                int vLines = this.Height / (spaceBetweenItemsThunmbailsView + ThumbnailsHeight + itemTextHeight);
                int hLines = this.Width / (spaceBetweenItemsThunmbailsView + ThumbnailsWidth);
                int passedRows = VscrollOffset / (spaceBetweenItemsThunmbailsView + ThumbnailsHeight + itemTextHeight);
                int itemIndex = passedRows * hLines;

                int mouseVlines = (location.Y + offset) / (spaceBetweenItemsThunmbailsView + ThumbnailsHeight + itemTextHeight);
                int mouseHlines = location.X / (spaceBetweenItemsThunmbailsView + ThumbnailsWidth);

                int indexAsMouse = (mouseVlines * hLines) + mouseHlines;
                if (indexAsMouse + itemIndex < items.Count)
                {
                    if (location.X < hLines * (spaceBetweenItemsThunmbailsView + ThumbnailsWidth))
                    {
                        index = indexAsMouse + itemIndex;
                    }
                }
            }
            return index;
        }
        /// <summary>
        /// Calculate all columns width
        /// </summary>
        /// <returns>The columns width (all columns)</returns>
        public int CalculateColumnsSize()
        {
            int size = 0;
            foreach (ListViewColumn column in columns)
            {
                size += column.Width;
            }
            return size;
        }
        /// <summary>
        /// Calculate all items size (height). Works with Details view mode only
        /// </summary>
        /// <returns>The height of all items</returns>
        public int CalculateItemsSize()
        {
            Size CharSize = WHUD.LibContext.MeasureText("TEST", this.Font);
            itemHeight = CharSize.Height + itemh;
            return itemHeight * items.Count;
        }
        /// <summary>
        /// Get the height of one item. Works with Details view mode only
        /// </summary>
        /// <returns></returns>
        public int GetItemHeight()
        {
            Size CharSize = WHUD.LibContext.MeasureText("TEST", this.Font);
            return CharSize.Height + itemh;
        }
        /// <summary>
        /// Get vertical scroll value for item
        /// </summary>
        /// <param name="itemIndex">The item index</param>
        /// <returns>The vertical scroll value</returns>
        public int GetVscrollValueForItem(int itemIndex)
        {
            if (viewMode == EListViewViewMode.Details)
            {
                return itemIndex * itemHeight;
            }
            else
            {
                int hLines = this.Width / (spaceBetweenItemsThunmbailsView + ThumbnailsWidth);
                // used too many math calculation to get this lol
                int val = (itemIndex * (spaceBetweenItemsThunmbailsView + ThumbnailsHeight + itemTextHeight)) / hLines;
                val -= val % (spaceBetweenItemsThunmbailsView + ThumbnailsHeight + itemTextHeight);
                return val;
            }
        }
        /// <summary>
        /// Calculate all items size (height). Works with Thumbnails view mode only
        /// </summary>
        /// <returns></returns>
        public Size CalculateSizeOfItemsAsThumbnails()
        {
            if (items.Count == 0)
                return Size.Empty;
            int w = 0;
            int h = 0;
            int vLines = this.Height / (spaceBetweenItemsThunmbailsView + ThumbnailsHeight + itemTextHeight);
            if (vLines == 0) vLines = 1;
            int hLines = this.Width / (spaceBetweenItemsThunmbailsView + ThumbnailsWidth);
            if (hLines == 0) vLines = 1;

            double itemRows = Math.Ceiling((double)items.Count / hLines);
            h = (int)(itemRows * (spaceBetweenItemsThunmbailsView + ThumbnailsHeight + itemTextHeight));

            if ((spaceBetweenItemsThunmbailsView + ThumbnailsWidth) > this.Width)
                w = (18 + ThumbnailsWidth);

            return new Size(w, h);
        }
        /// <summary>
        /// Rise the key own event
        /// </summary>
        /// <param name="e">The key event arguments</param>
        public void OnKeyDownRised(KeyEventArgs e)
        {
            if (items.Count > 0)
            {
                if (e.KeyCode == Keys.PageUp)
                {
                    // select none
                    foreach (ListViewItem item in items)
                        item.Selected = false;
                    // select first one
                    items[0].Selected = true;
                    // scroll
                    if (ScrollToSelectedItemRequest != null)
                        ScrollToSelectedItemRequest(this, new ListViewItemSelectArgs(0));
                    return;
                }
                else if (e.KeyCode == Keys.PageDown)
                {
                    // select none
                    foreach (ListViewItem item in items)
                        item.Selected = false;
                    // select first one
                    items[items.Count - 1].Selected = true;
                    // scroll
                    if (ScrollToSelectedItemRequest != null)
                        ScrollToSelectedItemRequest(this, new ListViewItemSelectArgs(items.Count - 1));
                    return;
                }
            }
            if (viewMode == EListViewViewMode.Details)
            {
                #region single selection
                if (SelectedItems.Count == 1)
                {
                    int index = items.IndexOf(SelectedItems[0]);
                    if (e.KeyCode == Keys.Down)
                    {
                        if (index + 1 < items.Count)
                        {
                            items[index].Selected = false;
                            items[index + 1].Selected = true;

                            //see if we need to scroll
                            int lines = (this.Height / itemHeight) - 2;
                            int maxLineIndex = (VscrollOffset / itemHeight) + lines;

                            if (index + 1 > maxLineIndex)
                                if (AdvanceVScrollRequest != null)
                                    AdvanceVScrollRequest(this, null);

                            if (SelectedIndexChanged != null)
                                SelectedIndexChanged(this, new EventArgs());
                        }
                    }
                    else if (e.KeyCode == Keys.Up)
                    {
                        if (index - 1 >= 0)
                        {
                            items[index].Selected = false;
                            items[index - 1].Selected = true;

                            int lowLineIndex = (VscrollOffset / itemHeight) + 1;

                            if (index - 1 < lowLineIndex)
                                if (ReverseVScrollRequest != null)
                                    ReverseVScrollRequest(this, null);

                            if (SelectedIndexChanged != null)
                                SelectedIndexChanged(this, new EventArgs());
                        }
                    }
                    else if (e.KeyCode == Keys.Return)
                    {
                        if (EnterPressedOverItem != null)
                            EnterPressedOverItem(this, new EventArgs());
                    }
                    else//char ?
                    {
                        KeysConverter conv = new KeysConverter();
                        for (int i = index + 1; i < items.Count; i++)
                        {
                            if (items[i].GetSubItemByID(columns[0].ID).Text.Length > 0)
                            {
                                if (items[i].GetSubItemByID(columns[0].ID).Text.Substring(0, 1) == conv.ConvertToString(e.KeyCode))
                                {
                                    items[index].Selected = false;
                                    items[i].Selected = true;
                                    if (ScrollToSelectedItemRequest != null)
                                        ScrollToSelectedItemRequest(this, new ListViewItemSelectArgs(i));
                                    this.Invalidate();
                                    break;
                                }
                            }
                        }
                    }
                }
                #endregion
                #region multi selection
                else if (SelectedItems.Count > 1)
                {
                    int index = items.IndexOf(SelectedItems[0]);
                    KeysConverter conv = new KeysConverter();
                    for (int i = index + 1; i < items.Count; i++)
                    {
                        if (items[i].GetSubItemByID(columns[0].ID).Text.Length > 0)
                        {
                            if (items[i].GetSubItemByID(columns[0].ID).Text.Substring(0, 1) == conv.ConvertToString(e.KeyCode))
                            {
                                items[index].Selected = false;
                                items[i].Selected = true;
                                if (ScrollToSelectedItemRequest != null)
                                    ScrollToSelectedItemRequest(this, new ListViewItemSelectArgs(i));
                                break;
                            }
                        }
                    }
                }
                #endregion
                #region No selection
                else
                {
                    KeysConverter conv = new KeysConverter();
                    for (int i = 0; i < items.Count; i++)
                    {
                        if (items[i].GetSubItemByID(columns[0].ID).Text.Length > 0)
                        {
                            if (items[i].GetSubItemByID(columns[0].ID).Text.Substring(0, 1) == conv.ConvertToString(e.KeyCode))
                            {
                                items[i].Selected = true;
                                if (ScrollToSelectedItemRequest != null)
                                    ScrollToSelectedItemRequest(this, new ListViewItemSelectArgs(i));
                                break;
                            }
                        }
                    }
                }
                #endregion
            }
            else// Thumbnails
            {
                #region single selection
                if (SelectedItems.Count == 1)
                {
                    int index = items.IndexOf(SelectedItems[0]);
                    // in this mode we got 4 directions so calculations may be more complicated
                    if (e.KeyCode == Keys.Right)
                    {
                        if (index + 1 < items.Count)
                        {
                            // advance selection
                            items[index].Selected = false;
                            items[index + 1].Selected = true;

                            // see if the new selected item is under the viewport
                            int vscroll = GetVscrollValueForItem(index + 1);
                            if (this.Height - vscroll < (ThumbnailsHeight + itemTextHeight))
                            {
                                if (ScrollToSelectedItemRequest != null)
                                    ScrollToSelectedItemRequest(this, new ListViewItemSelectArgs(index + 1));
                            }
                            if (SelectedIndexChanged != null)
                                SelectedIndexChanged(this, new EventArgs());
                        }
                    }
                    else if (e.KeyCode == Keys.Left)
                    {
                        if (index - 1 >= 0)
                        {
                            items[index].Selected = false;
                            items[index - 1].Selected = true;

                            int vscroll = GetVscrollValueForItem(index - 1);

                            if (vscroll < VscrollOffset)
                                if (ScrollToSelectedItemRequest != null)
                                    ScrollToSelectedItemRequest(this, new ListViewItemSelectArgs(index - 1));

                            if (SelectedIndexChanged != null)
                                SelectedIndexChanged(this, new EventArgs());
                        }
                    }
                    else if (e.KeyCode == Keys.Down)
                    {
                        // find out the index of the item below the selected one
                        int vLines = this.Height / (spaceBetweenItemsThunmbailsView + ThumbnailsHeight + itemTextHeight);
                        int hLines = this.Width / (spaceBetweenItemsThunmbailsView + ThumbnailsWidth);
                        int passedRows = VscrollOffset / (spaceBetweenItemsThunmbailsView + ThumbnailsHeight + itemTextHeight);
                        int itemIndexOfFirstItemInViewPort = passedRows * hLines;
                        int addit = index - itemIndexOfFirstItemInViewPort;
                        int newIndex = itemIndexOfFirstItemInViewPort + hLines + addit;
                        // now let's see if we can select this one
                        if (newIndex < items.Count)
                        {
                            // advance selection
                            items[index].Selected = false;
                            items[newIndex].Selected = true;

                            // see if the new selected item is under the viewport
                            int vscroll = GetVscrollValueForItem(newIndex);
                            if (this.Height - vscroll < (ThumbnailsHeight + itemTextHeight))
                            {
                                if (ScrollToSelectedItemRequest != null)
                                    ScrollToSelectedItemRequest(this, new ListViewItemSelectArgs(newIndex));
                            }
                            if (SelectedIndexChanged != null)
                                SelectedIndexChanged(this, new EventArgs());
                        }
                    }
                    else if (e.KeyCode == Keys.Up)
                    {
                        // find out the index of the item above the selected one
                        int vLines = this.Height / (spaceBetweenItemsThunmbailsView + ThumbnailsHeight + itemTextHeight);
                        int hLines = this.Width / (spaceBetweenItemsThunmbailsView + ThumbnailsWidth);
                        int passedRows = VscrollOffset / (spaceBetweenItemsThunmbailsView + ThumbnailsHeight + itemTextHeight);
                        int itemIndexOfFirstItemInViewPort = passedRows * hLines;
                        int addit = index - itemIndexOfFirstItemInViewPort;
                        int newIndex = itemIndexOfFirstItemInViewPort - hLines + addit;
                        // now let's see if we can select this one
                        if (newIndex >= 0)
                        {
                            // advance selection
                            items[index].Selected = false;
                            items[newIndex].Selected = true;

                            // see if the new selected item is under the viewport
                            int vscroll = GetVscrollValueForItem(newIndex);
                            if (vscroll < VscrollOffset)
                            {
                                if (ScrollToSelectedItemRequest != null)
                                    ScrollToSelectedItemRequest(this, new ListViewItemSelectArgs(newIndex));
                            }
                            if (SelectedIndexChanged != null)
                                SelectedIndexChanged(this, new EventArgs());
                        }
                    }
                    else if (e.KeyCode == Keys.Return)
                    {
                        if (EnterPressedOverItem != null)
                            EnterPressedOverItem(this, new EventArgs());
                    }
                    else//char ?
                    {
                        KeysConverter conv = new KeysConverter();
                        for (int i = index + 1; i < items.Count; i++)
                        {
                            if (items[i].GetSubItemByID(columns[0].ID).Text.Length > 0)
                            {
                                if (items[i].GetSubItemByID(columns[0].ID).Text.Substring(0, 1) == conv.ConvertToString(e.KeyCode))
                                {
                                    items[index].Selected = false;
                                    items[i].Selected = true;
                                    if (ScrollToSelectedItemRequest != null)
                                        ScrollToSelectedItemRequest(this, new ListViewItemSelectArgs(i));
                                    break;
                                }
                            }
                        }
                    }
                }
                #endregion
                #region multi selection
                else if (SelectedItems.Count > 1)
                {
                    int index = items.IndexOf(SelectedItems[0]);
                    KeysConverter conv = new KeysConverter();
                    for (int i = index + 1; i < items.Count; i++)
                    {
                        if (items[i].GetSubItemByID(columns[0].ID).Text.Length > 0)
                        {
                            if (items[i].GetSubItemByID(columns[0].ID).Text.Substring(0, 1) == conv.ConvertToString(e.KeyCode))
                            {
                                items[index].Selected = false;
                                items[i].Selected = true;
                                if (ScrollToSelectedItemRequest != null)
                                    ScrollToSelectedItemRequest(this, new ListViewItemSelectArgs(i));
                                break;
                            }
                        }
                    }
                }
                #endregion
                #region No selection
                else
                {
                    KeysConverter conv = new KeysConverter();
                    for (int i = 0; i < items.Count; i++)
                    {
                        if (items[i].GetSubItemByID(columns[0].ID).Text.Length > 0)
                        {
                            if (items[i].GetSubItemByID(columns[0].ID).Text.Substring(0, 1) == conv.ConvertToString(e.KeyCode))
                            {
                                items[i].Selected = true;
                                if (ScrollToSelectedItemRequest != null)
                                    ScrollToSelectedItemRequest(this, new ListViewItemSelectArgs(i));
                                break;
                            }
                        }
                    }
                }
                #endregion
            }
            this.Invalidate();
        }
        /// <summary>
        /// Rise the refresh scroll bars event
        /// </summary>
        public void OnRefreshScrollBars()
        {
            if (RefreshScrollBars != null)
                RefreshScrollBars(this, null);
        }
        /// <summary>
        /// Rise the mouse leave event
        /// </summary>
        public void OnMouseLeaveRise()
        {
            OnMouseLeave(new EventArgs());
        }

        /// <summary>
        /// Rise the paint event
        /// </summary>
        /// <param name="pe"><see cref="PaintEventArgs"/></param>
        protected override void DoPaint(PaintEventArgs pe)
        {
            base.DoPaint(pe);

            if (viewMode == EListViewViewMode.Details)
                DrawDetailsView(pe);
            else
                DrawThumbailsView(pe);

            //select rectangle
            if (DrawSelectRectangle)
                pe.GC.DrawRectangle(new NanoPen(Color.Yellow),
                                    SelectRectanglex, SelectRectangley, SelectRectanglex1 - SelectRectanglex, SelectRectangley1 - SelectRectangley);
        }

        /// <summary>
        /// Rise the mouse down event
        /// </summary>
        /// <param name="e"><see cref="MouseEventArgs"/></param>
        protected override void OnMouseDown(MouseDownEventArgs e)
        {
            base.OnMouseDown(e);
            DownPoint = e.Location;
            DownPointAsViewPort = new Point(e.X + HscrollOffset, e.Y + VscrollOffset);
            isMouseDown = true;
            if (viewMode == EListViewViewMode.Details)
            {
                if (e.Button == MouseButton.Left)
                {
                    if (e.Y > columnHeight)
                    {
                        if (e.X > CalculateColumnsSize() - HscrollOffset)
                        {
                            DrawSelectRectangle = true;
                        }
                        else if (e.Y > CalculateItemsSize() - VscrollOffset)
                        {
                            DrawSelectRectangle = true;
                        }
                        else
                            DrawSelectRectangle = false;
                    }
                    else
                        DrawSelectRectangle = false;
                }
                else
                    DrawSelectRectangle = false;
            }
            else
            {
                if (e.Button == MouseButton.Left)
                {
                    if (!highlightItemAsOver && overItemSelectedIndex < 0)
                    {
                        DrawSelectRectangle = true;
                    }
                    else
                    {
                        DrawSelectRectangle = false;
                    }
                }
            }

            Invalidate();
        }
        /// <summary>
        /// Rise the mouse up event
        /// </summary>
        /// <param name="e"><see cref="MouseEventArgs"/></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            UpPoint = e.Location;
            base.OnMouseUp(e);
            #region resort column

            if (e.Button == MouseButton.Left)
            {
                if (moveType == EListViewMoveType.ColumnVLine && selectedColumnIndex >= 0)
                {
                    if (AfterColumnResize != null)
                        AfterColumnResize(this, new EventArgs());
                }
                if (moveType == EListViewMoveType.Column && selectedColumnIndex >= 0 && isMovingColumn)
                {
                    //get index
                    int cX = 0;
                    int x = 0;
                    int i = 0;
                    foreach (ListViewColumn column in columns)
                    {
                        cX += column.Width;
                        if (cX >= HscrollOffset)
                        {
                            if (e.X >= (x - HscrollOffset) && e.X <= (cX - HscrollOffset) + 3)
                            {
                                selectedColumnIndex = i;
                            }
                        }
                        i++;
                        x += column.Width;
                        if (x - HscrollOffset > this.Width)
                            break;
                    }
                    ListViewColumn currentColumn = columns[currentColumnMovedIndex];
                    columns.Remove(columns[currentColumnMovedIndex]);
                    columns.Insert(selectedColumnIndex, currentColumn);
                }
            }
            #endregion
            isMovingColumn = false;
            DrawSelectRectangle = false;
            SelectRectanglex = 0;
            SelectRectangley = 0;
            SelectRectanglex1 = 0;
            SelectRectangley1 = 0;
            isMouseDown = false;
            Invalidate();
        }
        /// <summary>
        /// Rise the mouse move event
        /// </summary>
        /// <param name="e"><see cref="MouseEventArgs"/></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Cursors.Cursor = Cursors.Default;
            CurrentMousePosition = e.Location;

            if (viewMode == EListViewViewMode.Details)
            {
                if (e.Button == MouseButton.Left)
                {
                    if (moveType == EListViewMoveType.ColumnVLine && selectedColumnIndex >= 0)
                    {
                        int shift = e.X - downX;
                        Cursors.Cursor = Cursors.HSplit;
                        columns[selectedColumnIndex].Width = originalcolumnWidth + shift;
                        Invalidate();
                        if (RefreshValues != null)
                            RefreshValues(this, null);
                        return;
                    }
                }
                #region moving column vertical line
                if (e.Y <= columnHeight)
                {
                    if (SwitchToColumnsContextMenu != null)
                        SwitchToColumnsContextMenu(this, new EventArgs());
                    highlightSelectedColumn = true;
                    if (e.Button == MouseButton.Left)
                    {
                        if (moveType == EListViewMoveType.ColumnVLine && selectedColumnIndex >= 0)
                        {
                            int shift = e.X - downX;
                            Cursors.Cursor = Cursors.HSplit;
                            columns[selectedColumnIndex].Width = originalcolumnWidth + shift;
                            Invalidate();
                            if (RefreshValues != null)
                                RefreshValues(this, null);
                        }
                        if (AllowColumnsReorder)
                        {
                            if (moveType == EListViewMoveType.Column && selectedColumnIndex >= 0)
                            {
                                currentColumnMovedIndex = selectedColumnIndex;
                                if (e.X > DownPoint.X + 3 || e.X < DownPoint.X - 3)
                                    isMovingColumn = true;
                            }
                        }
                    }
                    else
                    {
                        moveType = EListViewMoveType.None;
                        int cX = 0;
                        int x = 0;
                        int i = 0;
                        foreach (ListViewColumn column in columns)
                        {
                            cX += column.Width;
                            if (cX >= HscrollOffset)
                            {
                                if (e.X >= (x - HscrollOffset) && e.X <= (cX - HscrollOffset) + 3)
                                {
                                    selectedColumnIndex = i;
                                    moveType = EListViewMoveType.Column;
                                }
                                //vertical line select ?
                                int min = cX - HscrollOffset - 3;
                                int max = cX - HscrollOffset + 3;
                                if (e.X >= min && e.X <= max)
                                {
                                    downX = e.X;
                                    originalcolumnWidth = column.Width;
                                    moveType = EListViewMoveType.ColumnVLine;
                                    Cursors.Cursor = Cursors.HSplit;
                                    break;
                                }
                            }

                            i++;
                            x += column.Width;

                            if (x - HscrollOffset > this.Width)
                                break;
                        }
                    }
                }
                else
                {
                    if (SwitchToNormalContextMenu != null)
                        SwitchToNormalContextMenu(this, new EventArgs());
                    if (e.Button == MouseButton.Left)
                    {
                        if (moveType == EListViewMoveType.ColumnVLine && selectedColumnIndex >= 0)
                        {
                            int shift = e.X - downX;
                            Cursors.Cursor = Cursors.HSplit;
                            columns[selectedColumnIndex].Width = originalcolumnWidth + shift;
                            Invalidate();
                            if (RefreshValues != null)
                                RefreshValues(this, null);
                        }
                        if (AllowColumnsReorder)
                        {
                            if (moveType == EListViewMoveType.Column && selectedColumnIndex >= 0)
                            {
                                currentColumnMovedIndex = selectedColumnIndex;
                                if (e.X > DownPoint.X + 3 || e.X < DownPoint.X - 3)
                                    isMovingColumn = true;
                            }
                        }
                    }
                    else
                    {
                        //clear
                        moveType = EListViewMoveType.None;
                        selectedColumnIndex = -1;
                        highlightSelectedColumn = false;
                        isMovingColumn = false;
                    }
                }
                #endregion
                #region item select
                if (e.Y > columnHeight)
                {
                    if (e.Button == MouseButton.Left)
                    {
                        if (DrawSelectRectangle)
                        {
                            //draw select rectangle
                            SelectRectanglex = DownPointAsViewPort.X - HscrollOffset;
                            SelectRectangley = DownPointAsViewPort.Y - VscrollOffset;
                            SelectRectanglex1 = e.X;
                            SelectRectangley1 = e.Y;
                            if (SelectRectanglex1 < SelectRectanglex)
                            {
                                SelectRectanglex = e.X;
                                SelectRectanglex1 = DownPointAsViewPort.X - HscrollOffset;
                            }
                            if (SelectRectangley1 < SelectRectangley)
                            {
                                SelectRectangley = e.Y;
                                SelectRectangley1 = DownPointAsViewPort.Y - VscrollOffset;
                            }
                            if (e.Y > this.Height)
                            {
                                for (int y = 0; y < 10; y++)
                                    if (AdvanceVScrollRequest != null)
                                        AdvanceVScrollRequest(this, null);
                            }

                            //select the items
                            if (ModifierKeys != Keys.Control)
                            {
                                foreach (ListViewItem item in items)
                                    item.Selected = false;
                            }
                            if (SelectRectanglex + HscrollOffset < CalculateColumnsSize())
                            {
                                Size CharSize = WHUD.LibContext.MeasureText("TEST", this.Font);

                                isSecectingItems = true;
                                bool itemSelected = false;
                                for (int i = VscrollOffset + SelectRectangley; i < VscrollOffset + SelectRectangley1; i++)
                                {
                                    int itemIndex = (i - columnHeight) / itemHeight;
                                    if (itemIndex < items.Count)
                                    {
                                        items[itemIndex].Selected = true; itemSelected = true;
                                    }
                                }
                                if (itemSelected)
                                    if (SelectedIndexChanged != null)
                                        SelectedIndexChanged(this, new EventArgs());
                            }
                        }
                        //drag and drop
                        if (AllowItemsDragAndDrop)
                        {
                            if (e.X > DownPoint.X + 3 || e.X < DownPoint.X - 3 || e.Y > DownPoint.Y + 3 | e.Y < DownPoint.Y - 3)
                            {
                                if (highlightItemAsOver)
                                {
                                    if (overItemSelectedIndex >= 0)
                                    {
                                        if (items[overItemSelectedIndex].Selected)
                                        {
                                            if (ItemsDrag != null)
                                                ItemsDrag(this, new EventArgs());
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (e.X < CalculateColumnsSize() - HscrollOffset)
                        {
                            Size CharSize = WHUD.LibContext.MeasureText("TEST", this.Font);
                            int itemIndex = (e.Y + (VscrollOffset - columnHeight) - (itemHeight / highlightSensitive)) / itemHeight;
                            if (itemIndex < items.Count)
                            {
                                highlightItemAsOver = true;
                                overItemSelectedIndex = itemIndex;
                                if (OldoverItemSelectedIndex != itemIndex)
                                {
                                    if (OldoverItemSelectedIndex >= 0 && OldoverItemSelectedIndex < items.Count)
                                        items[OldoverItemSelectedIndex].OnMouseLeave();
                                }
                                OldoverItemSelectedIndex = itemIndex;
                                //rise the event

                                int cX = 0;
                                int x = 0;
                                int i = 0;
                                foreach (ListViewColumn column in columns)
                                {
                                    cX += column.Width;
                                    if (cX >= HscrollOffset)
                                    {
                                        if (cX > e.X + HscrollOffset)
                                        {
                                            if (i < items[itemIndex].SubItems.Count)
                                            {
                                                ListViewSubItem sitem = items[itemIndex].GetSubItemByID(column.ID);
                                                if (sitem != null)
                                                {
                                                    sitem.OnMouseOver(new Point(e.X - (x - HscrollOffset), 0), CharSize);
                                                    if (sitem.GetType() == typeof(ListViewSubItemRating))
                                                    {
                                                        ((ListViewSubItemRating)sitem).DrawOverImage = true;
                                                    }
                                                }

                                                if (MouseOverSubItem != null)
                                                {
                                                    MouseOverSubItem(this, new ListViewMouseOverSubItemArgs(overItemSelectedIndex,
                                                                                                            columns[i].ID, e.X - x - HscrollOffset));
                                                }
                                            }
                                            break;
                                        }
                                    }
                                    x += column.Width;
                                    i++;
                                    if (x - HscrollOffset > this.Width)
                                        break;
                                }


                            }
                            else
                            {
                                highlightItemAsOver = false;
                                if (overItemSelectedIndex < items.Count && overItemSelectedIndex >= 0)
                                    items[OldoverItemSelectedIndex].OnMouseLeave();
                                OldoverItemSelectedIndex = overItemSelectedIndex = -1;
                            }
                        }
                        else
                        {
                            highlightItemAsOver = false;
                            if (overItemSelectedIndex < items.Count && overItemSelectedIndex >= 0)
                                items[OldoverItemSelectedIndex].OnMouseLeave();
                            OldoverItemSelectedIndex = overItemSelectedIndex = -1;
                        }
                    }
                }
                if (DrawSelectRectangle)
                {
                    if (e.Y < columnHeight)
                    {
                        for (int y = 0; y < 10; y++)
                            if (ReverseVScrollRequest != null)
                                ReverseVScrollRequest(this, null);
                    }
                }
                #endregion
            }
            else
            {
                if (e.Button == MouseButton.Left)
                {
                    if (DrawSelectRectangle)
                    {
                        //draw select rectangle
                        SelectRectanglex = DownPointAsViewPort.X - HscrollOffset;
                        SelectRectangley = DownPointAsViewPort.Y - VscrollOffset;
                        SelectRectanglex1 = e.X;
                        SelectRectangley1 = e.Y;
                        if (SelectRectanglex1 < SelectRectanglex)
                        {
                            SelectRectanglex = e.X;
                            SelectRectanglex1 = DownPointAsViewPort.X - HscrollOffset;
                        }
                        if (SelectRectangley1 < SelectRectangley)
                        {
                            SelectRectangley = e.Y;
                            SelectRectangley1 = DownPointAsViewPort.Y - VscrollOffset;
                        }
                        if (e.Y > this.Height)
                        {
                            for (int y = 0; y < 10; y++)
                                if (AdvanceVScrollRequest != null)
                                    AdvanceVScrollRequest(this, null);
                        }
                        if (e.Y < 0)
                        {
                            for (int y = 0; y < 10; y++)
                                if (ReverseVScrollRequest != null)
                                    ReverseVScrollRequest(this, null);
                        }

                        //select the items
                        if (ModifierKeys != Keys.Control)
                        {
                            foreach (ListViewItem item in items)
                                item.Selected = false;
                        }
                        bool itemSelected = false;
                        isSecectingItems = true;
                        int offset = VscrollOffset % (spaceBetweenItemsThunmbailsView + ThumbnailsHeight + itemTextHeight);
                        int vLines = this.Height / (spaceBetweenItemsThunmbailsView + ThumbnailsHeight + itemTextHeight);
                        int hLines = this.Width / (spaceBetweenItemsThunmbailsView + ThumbnailsWidth);
                        int passedRows = VscrollOffset / (spaceBetweenItemsThunmbailsView + ThumbnailsHeight + itemTextHeight);
                        int itemIndex = passedRows * hLines;
                        for (int i = VscrollOffset + SelectRectangley; i < VscrollOffset + SelectRectangley1; i++)
                        {
                            for (int j = HscrollOffset + SelectRectanglex; j < HscrollOffset + SelectRectanglex1; j++)
                            {
                                int recVlines = (i + offset) / (spaceBetweenItemsThunmbailsView + ThumbnailsHeight + itemTextHeight);
                                int recHlines = j / (spaceBetweenItemsThunmbailsView + ThumbnailsWidth);
                                if (recHlines < hLines)
                                {
                                    int indexAsrec = (recVlines * hLines) + recHlines;
                                    if (indexAsrec < items.Count)
                                    { items[indexAsrec].Selected = true; itemSelected = true; }
                                }
                                j += (spaceBetweenItemsThunmbailsView + ThumbnailsWidth);
                            }
                            i += (spaceBetweenItemsThunmbailsView + ThumbnailsHeight + itemTextHeight);
                        }
                        if (itemSelected)
                            if (SelectedIndexChanged != null)
                                SelectedIndexChanged(this, new EventArgs());
                    }
                    //drag and drop
                    if (AllowItemsDragAndDrop)
                    {
                        if (e.X > DownPoint.X + 3 || e.X < DownPoint.X - 3 || e.Y > DownPoint.Y + 3 | e.Y < DownPoint.Y - 3)
                        {
                            if (highlightItemAsOver)
                            {
                                if (overItemSelectedIndex >= 0)
                                {
                                    if (items[overItemSelectedIndex].Selected)
                                    {
                                        if (ItemsDrag != null)
                                            ItemsDrag(this, new EventArgs());
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    //thunmbnails view select item
                    int offset = VscrollOffset % (spaceBetweenItemsThunmbailsView + ThumbnailsHeight + itemTextHeight);
                    int vLines = this.Height / (spaceBetweenItemsThunmbailsView + ThumbnailsHeight + itemTextHeight);
                    int hLines = this.Width / (spaceBetweenItemsThunmbailsView + ThumbnailsWidth);
                    int passedRows = VscrollOffset / (spaceBetweenItemsThunmbailsView + ThumbnailsHeight + itemTextHeight);
                    int itemIndex = passedRows * hLines;

                    int mouseVlines = (e.Y + offset) / (spaceBetweenItemsThunmbailsView + ThumbnailsHeight + itemTextHeight);
                    int mouseHlines = e.X / (spaceBetweenItemsThunmbailsView + ThumbnailsWidth);

                    int indexAsMouse = (mouseVlines * hLines) + mouseHlines;
                    if (indexAsMouse + itemIndex < items.Count)
                    {
                        if (e.X < hLines * (spaceBetweenItemsThunmbailsView + ThumbnailsWidth))
                        {
                            highlightItemAsOver = true;
                            overItemSelectedIndex = indexAsMouse + itemIndex;
                        }
                        else
                        {
                            highlightItemAsOver = false;
                            OldoverItemSelectedIndex = overItemSelectedIndex = -1;
                        }
                    }
                    else
                    {
                        highlightItemAsOver = false;
                        OldoverItemSelectedIndex = overItemSelectedIndex = -1;
                    }
                }
            }
            Invalidate();
        }

        /// <summary>
        /// Rise the mouse click event
        /// </summary>
        /// <param name="e"><see cref="MouseEventArgs"/></param>
        public override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (viewMode == EListViewViewMode.Details)
            {
                #region item select
                if (e.Y > columnHeight)
                {
                    if ((e.X < CalculateColumnsSize() - HscrollOffset))
                    {
                        if (e.Button == MouseButton.Left && !isSecectingItems && overItemSelectedIndex >= 0)
                        {
                            ListViewItem lvi = items[overItemSelectedIndex];
                            if (lvi == null)
                                return;

                            bool currentItemStatus = lvi.Selected;
                            bool isShiftSelection = false;
                            if (ModifierKeys == Keys.Shift)
                            {
                                isShiftSelection = true;
                                if (LatestOverItemSelectedIndex == -1)
                                    isShiftSelection = false;
                            }
                            else if (ModifierKeys != Keys.Control)
                            {
                                LatestOverItemSelectedIndex = -1;
                                foreach (ListViewItem item in items)
                                    item.Selected = false;
                            }

                            if (highlightItemAsOver)
                            {
                                if (overItemSelectedIndex >= 0)
                                {
                                    if (!isShiftSelection)
                                    {
                                        items[overItemSelectedIndex].Selected = true;
                                        LatestOverItemSelectedIndex = overItemSelectedIndex;
                                        if (SelectedIndexChanged != null && !currentItemStatus)
                                            SelectedIndexChanged(this, new EventArgs());

                                        int cX = 0;
                                        int x = 0;
                                        int i = 0;
                                        foreach (ListViewColumn column in columns)
                                        {
                                            cX += column.Width;
                                            if (cX >= HscrollOffset)
                                            {
                                                if (cX > e.X)
                                                {
                                                    ListViewSubItem sub = items[overItemSelectedIndex].GetSubItemByID(column.ID);
                                                    if (sub != null)
                                                    {
                                                        sub.OnMouseClick(new Point(e.X - x - HscrollOffset, 0),
                                                                         WHUD.LibContext.MeasureText("TEST", this.Font), overItemSelectedIndex);
                                                    }
                                                    break;
                                                }
                                            }
                                            x += column.Width;
                                            i++;
                                            if (x - HscrollOffset > this.Width)
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        foreach (ListViewItem item in items)
                                            item.Selected = false;
                                        if (overItemSelectedIndex > LatestOverItemSelectedIndex)
                                        {
                                            for (int j = LatestOverItemSelectedIndex; j < overItemSelectedIndex + 1; j++)
                                            {
                                                items[j].Selected = true;
                                            }
                                            if (SelectedIndexChanged != null)
                                                SelectedIndexChanged(this, new EventArgs());
                                        }
                                        else if (overItemSelectedIndex < LatestOverItemSelectedIndex)
                                        {
                                            for (int j = overItemSelectedIndex; j < LatestOverItemSelectedIndex + 1; j++)
                                            {
                                                items[j].Selected = true;
                                            }
                                            if (SelectedIndexChanged != null)
                                                SelectedIndexChanged(this, new EventArgs());
                                        }
                                    }
                                }

                            }
                        }
                        if (e.Button == MouseButton.Left && !isSecectingItems && overItemSelectedIndex == -1)
                        {
                            LatestOverItemSelectedIndex = -1;
                            foreach (ListViewItem item in items)
                                item.Selected = false;
                        }
                    }
                    else
                    {
                        if (!isSecectingItems)
                            foreach (ListViewItem item in items)
                                item.Selected = false;
                    }
                }
                #endregion
                #region Column click
                if (e.Y < columnHeight)
                {
                    if (e.Button == MouseButton.Left)
                    {
                        if (moveType == EListViewMoveType.Column && selectedColumnIndex >= 0 && !isMovingColumn)
                        {
                            if (ChangeColumnSortModeWhenClick)
                            {
                                switch (columns[selectedColumnIndex].SortMode)
                                {
                                    case EListViewSortMode.None: columns[selectedColumnIndex].SortMode = EListViewSortMode.AtoZ; break;
                                    case EListViewSortMode.AtoZ: columns[selectedColumnIndex].SortMode = EListViewSortMode.ZtoA; break;
                                    case EListViewSortMode.ZtoA: columns[selectedColumnIndex].SortMode = EListViewSortMode.None; break;
                                }
                            }
                            if (ColumnClicked != null)
                                ColumnClicked(this, new ListViewColumnClickArgs(columns[selectedColumnIndex].ID));
                            Invalidate();
                        }
                    }
                }
                #endregion
            }
            else
            {
                if (e.Button == MouseButton.Left && !isSecectingItems && overItemSelectedIndex >= 0)
                {
                    bool currentItemStatus = items[overItemSelectedIndex].Selected;
                    bool isShiftSelection = false;
                    if (ModifierKeys == Keys.Shift)
                    {
                        isShiftSelection = true;
                        if (LatestOverItemSelectedIndex == -1)
                            isShiftSelection = false;
                    }
                    else if (ModifierKeys != Keys.Control)
                    {
                        LatestOverItemSelectedIndex = -1;
                        foreach (ListViewItem item in items)
                            item.Selected = false;
                    }

                    if (highlightItemAsOver)
                    {
                        if (overItemSelectedIndex >= 0)
                        {
                            if (!isShiftSelection)
                            {
                                items[overItemSelectedIndex].Selected = true;
                                LatestOverItemSelectedIndex = overItemSelectedIndex;
                                if (SelectedIndexChanged != null && !currentItemStatus)
                                    SelectedIndexChanged(this, new EventArgs());
                            }
                            else
                            {
                                foreach (ListViewItem item in items)
                                    item.Selected = false;
                                if (overItemSelectedIndex > LatestOverItemSelectedIndex)
                                {
                                    for (int j = LatestOverItemSelectedIndex; j < overItemSelectedIndex + 1; j++)
                                    {
                                        items[j].Selected = true;
                                    }
                                    if (SelectedIndexChanged != null)
                                        SelectedIndexChanged(this, new EventArgs());
                                }
                                else if (overItemSelectedIndex < LatestOverItemSelectedIndex)
                                {
                                    for (int j = overItemSelectedIndex; j < LatestOverItemSelectedIndex + 1; j++)
                                    {
                                        items[j].Selected = true;
                                    }
                                    if (SelectedIndexChanged != null)
                                        SelectedIndexChanged(this, new EventArgs());
                                }
                            }
                        }

                    }
                }
                if (e.Button == MouseButton.Left && !isSecectingItems && overItemSelectedIndex == -1)
                {
                    LatestOverItemSelectedIndex = -1;
                    foreach (ListViewItem item in items)
                        item.Selected = false;
                }
            }
            if (!IsFocused)
            {
                base.Focus();
            }
            isSecectingItems = false;
        }
        /// <summary>
        /// Rise the mouse wheel event
        /// </summary>
        /// <param name="e"><see cref="MouseEventArgs"/></param>
        public override void OnMouseWheel(MouseEventArgs e)
        {
            if (e.DeltaWheel > 0)
            {
                if (ReverseVScrollRequest != null)
                    ReverseVScrollRequest(this, null);
            }
            if (e.DeltaWheel < 0)
            {
                if (AdvanceVScrollRequest != null)
                    AdvanceVScrollRequest(this, null);
            }
            base.OnMouseWheel(e);
            Invalidate();
        }

        /// <summary>
        /// Rise the mouse double click event
        /// </summary>
        /// <param name="e"><see cref="MouseEventArgs"/></param>
        public override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (e.Y > columnHeight)
            {
                if (ItemDoubleClick != null)
                    ItemDoubleClick(this, new ListViewItemDoubleClickArgs(overItemSelectedIndex));
            }
        }
        /// <summary>
        /// Rise the mouse leave event
        /// </summary>
        /// <param name="e"><see cref="EventArgs"/></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (highlightItemAsOver)
            {
                highlightItemAsOver = false;
                if (overItemSelectedIndex < items.Count && overItemSelectedIndex >= 0)
                    if (items[OldoverItemSelectedIndex] != null)
                        items[OldoverItemSelectedIndex].OnMouseLeave();
                OldoverItemSelectedIndex = overItemSelectedIndex = -1;
                Invalidate();
            }
        }

        void DrawString(GContext gc, string str, NanoFont font, Color c, Rectangle rect)
        {
            int tAscender = (int)Math.Ceiling(Font.Ascender);
            var nsb = new NanoSolidBrush(c);
            gc.DrawString(str, font, nsb, rect.Y + tAscender, rect);
        }

        private void DrawDetailsView(PaintEventArgs pe)
        {
            int cX = 0;
            int x = 0;
            int i = 0;
            //get default size of word
            Size CharSize = WHUD.LibContext.MeasureText("TEST", this.Font);
            columnHeight = CharSize.Height + columnh;
            int columnTextOffset = columnh / 2;
            itemHeight = CharSize.Height + itemh;
            int itemTextOffset = itemh / 2;
            int lines = (this.Height / itemHeight) + 2;

            int offset = VscrollOffset % itemHeight;
            int columnsWidth = CalculateColumnsSize();

            pe.GC.FillRectangle(new NanoSolidBrush(iListView.BGColor), columnsWidth - HscrollOffset, 0,
                                ClientRect.Width - (columnsWidth - HscrollOffset), Height);

            if (columns.Count > 0)
                pe.GC.DrawLine(new NanoPen(iListView.ColumnsSubLineColor), new Point(0, columnHeight), new Point(this.Width, columnHeight));

            foreach (ListViewColumn column in columns)
            {
                cX += column.Width;
                if (cX >= HscrollOffset)
                {
                    int lineIndex = VscrollOffset / itemHeight;
                    //draw sub items releated to this column
                    for (int j = 0; j < lines; j++)
                    {
                        try
                        {
                            //clear
                            pe.GC.FillRectangle(new SolidBrush(iListView.BGColor),
                                                new Rectangle(x - HscrollOffset + 1, (j * itemHeight) - offset + columnHeight + 1,
                                                              column.Width - 1, itemHeight));

                            if (lineIndex < items.Count)
                            {
                                if (items[lineIndex].IsSpecialItem)
                                {
                                    pe.GC.FillRectangle(new NanoSolidBrush(Color.YellowGreen),
                                                        new Rectangle(x - HscrollOffset, (j * itemHeight) - offset + columnHeight + 1,
                                                                      column.Width - 1, itemHeight));
                                }
                                if (items[lineIndex].Selected)
                                {
                                    pe.GC.FillRectangle(new NanoSolidBrush(iListView.SelectedItemColor),
                                                        new Rectangle(x - HscrollOffset, (j * itemHeight) - offset + columnHeight + 1,
                                                                      column.Width - 1, itemHeight));
                                }
                                else
                                {
                                    if (highlightItemAsOver)
                                    {
                                        if (lineIndex == overItemSelectedIndex)
                                        {
                                            if (DrawHighlight)
                                                pe.GC.FillRectangle(new NanoSolidBrush(iListView.OverItemColor),
                                                                    new Rectangle(x - HscrollOffset, (j * itemHeight) - offset + columnHeight + 1,
                                                                                  column.Width - 1, itemHeight));
                                        }
                                    }
                                }

                                ListViewItem lvi = items[lineIndex];
                                ListViewSubItem subitem = lvi.GetSubItemByID(column.ID);
                                Color drawColor = subitem.TextColor = lvi.TextColor;
                                NanoFont drawFont = subitem.CustomFontEnabled ? subitem.CustomFont : this.Font;
                                if (subitem != null)
                                {
                                    if (subitem.GetType() == typeof(ListViewSubItemRating))
                                    {
                                        ((ListViewSubItemRating)subitem).OnRefreshRating(lineIndex, itemHeight);
                                        //Image img = Properties.Resources.noneRating;
                                        if (!((ListViewSubItemRating)subitem).DrawOverImage)
                                        {
                                            switch (((ListViewSubItemRating)subitem).Rating)
                                            {
                                                //case 1: img = Properties.Resources.star_1; break;
                                                //case 2: img = Properties.Resources.star_2; break;
                                                //case 3: img = Properties.Resources.star_3; break;
                                                //case 4: img = Properties.Resources.star_4; break;
                                                //case 5: img = Properties.Resources.star_5; break;
                                            }
                                        }
                                        else
                                        {
                                            switch (((ListViewSubItemRating)subitem).OverRating)
                                            {
                                                //case 1: img = Properties.Resources.star_1; break;
                                                //case 2: img = Properties.Resources.star_2; break;
                                                //case 3: img = Properties.Resources.star_3; break;
                                                //case 4: img = Properties.Resources.star_4; break;
                                                //case 5: img = Properties.Resources.star_5; break;
                                            }
                                        }
                                        //pe.GC.DrawImage(img, new Rectangle(x - HscrollOffset + 2, (j * itemHeight) - offset + columnHeight + 1,
                                        //                                   itemHeight * 4, itemHeight - 1));

                                        ((ListViewSubItemRating)subitem).DrawOverImage = false;
                                    }
                                    else
                                    {
                                        switch (subitem.DrawMode)
                                        {
                                            case EListViewItemDrawMode.Text:
                                                /*pe.GC.DrawString(subitem.Text, drawFont, new NanoSolidBrush(drawColor),
                                                                 new Rectangle(x - HscrollOffset + 2,
                                                                               (j * itemHeight) - offset + columnHeight + itemTextOffset,
                                                                               column.Width, CharSize.Height), _StringFormat);*/
                                                string str = Ellipsis.Compact(subitem.Text, drawFont, column.Width - 4, EllipsisFormat.Middle);
                                                DrawString(pe.GC, str, drawFont, drawColor, new Rectangle(x - HscrollOffset + 4,
                                                                                                          (j * itemHeight) - offset + columnHeight + itemTextOffset,
                                                                                                          column.Width - 4, CharSize.Height));

                                                break;
                                            case EListViewItemDrawMode.Image:
                                                //if (subitem.ImageIndex < ImagesList.Images.Count)
                                                {
                                                    //pe.GC.DrawImage(ImagesList.Images[subitem.ImageIndex],
                                                    //                      new Rectangle(x - HscrollOffset + 2, (j * itemHeight) - offset + columnHeight + 1,
                                                    //                                    itemHeight - 1, itemHeight - 1));
                                                }
                                                break;
                                            case EListViewItemDrawMode.TextAndImage:
                                                int plus = 2;
                                                //if (subitem.ImageIndex < ImagesList.Images.Count)
                                                {
                                                    //pe.Graphics.DrawImage(ImagesList.Images[subitem.ImageIndex],
                                                    //                      new Rectangle(x - HscrollOffset + 2, (j * itemHeight) - offset + columnHeight + 1,
                                                    //                                    itemHeight - 1, itemHeight - 1));
                                                    plus += itemHeight;
                                                }
                                                /*pe.GC.DrawString(subitem.Text, drawFont, new SolidBrush(drawColor),
                                                                 new Rectangle(x - HscrollOffset + 2 + plus,
                                                                               (j * itemHeight) - offset + columnHeight + itemTextOffset,
                                                                               column.Width - plus, CharSize.Height), _StringFormat);*/
                                                DrawString(pe.GC, subitem.Text, drawFont, drawColor, new Rectangle(x - HscrollOffset + 2 + plus,
                                                                                                                   (j * itemHeight) - offset + columnHeight + itemTextOffset,
                                                                                                                   column.Width - plus, CharSize.Height));
                                                break;
                                            case EListViewItemDrawMode.UserDraw:    //rise the event
                                                if (DrawSubItem != null)
                                                {
                                                    var args = new ListViewSubItemDrawArgs(column.ID, lineIndex);
                                                    DrawSubItem(this, args);
                                                    int p = 2;
                                                    if (args.ImageToDraw != null)
                                                    {
                                                        //pe.Graphics.DrawImage(args.ImageToDraw,
                                                        //                      new Rectangle(x - HscrollOffset + 2, (j * itemHeight) - offset + columnHeight + 1
                                                        //                                    , itemHeight - 1, itemHeight - 1));
                                                        p += itemHeight;
                                                    }
                                                    DrawString(pe.GC, args.TextToDraw, drawFont, drawColor, new Rectangle(x - HscrollOffset + 2 + p,
                                                                                                                          (j * itemHeight) - offset + columnHeight + itemTextOffset,
                                                                                                                          column.Width - p, CharSize.Height));
                                                }
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message + "\n\n" + ex.ToString());
                            //MessageBox.Show(ex.Message+"\n\n"+ex.ToString());
                        }
                        lineIndex++;
                    }

                    //draw the column rectangle, draw the column after the item to hide the offset
                    Color Hcolor = iListView.ColumnsColor;
                    if (highlightSelectedColumn && selectedColumnIndex == i)
                    {
                        if (!isMouseDown)
                            Hcolor = Color.Black;
                        else
                        {
                            if (moveType != EListViewMoveType.ColumnVLine)
                                Hcolor = Color.DarkSlateGray;
                            else
                                Hcolor = iListView.SelectedItemColor;
                        }
                    }
                    //DRAW COLUMN
                    pe.GC.FillRectangle(new NanoSolidBrush(Hcolor),
                                        new Rectangle(x - HscrollOffset + 1, 1, column.Width, columnHeight));
                    //draw the column line
                    pe.GC.DrawLine(new NanoPen(iListView.LinesColor), new Point(cX - HscrollOffset, 1),
                                   new Point(cX - HscrollOffset, this.Height - 1));
                    //draw the column text
                    DrawString(pe.GC, column.HeaderText, this.Font, iListView.ColumnsTextColor, new Rectangle(x - HscrollOffset + 2, columnTextOffset, column.Width, columnHeight));
                    //rise the event
                    if (DrawColumn != null)
                        DrawColumn(this, new ListViewColumnDrawArgs(column.ID, pe.GC,
                                                                    new Rectangle(x - HscrollOffset, 2, column.Width, columnHeight)));
                    //draw sort traingle
                    switch (column.SortMode)
                    {
                        case EListViewSortMode.AtoZ:
                            //pe.GC.DrawImage(Properties.Resources.SortAlpha.ToBitmap(),
                            //                new Rectangle(x - HscrollOffset + column.Width - 14, 2, 12, 16));
                            break;
                        case EListViewSortMode.ZtoA:
                            //pe.GC.DrawImage(Properties.Resources.SortZ.ToBitmap(),
                            //                new Rectangle(x - HscrollOffset + column.Width - 14, 2, 12, 16));
                            break;
                    }
                }
                x += column.Width;
                i++;
                if (x - HscrollOffset > this.Width)
                    break;
            }

            if (isMovingColumn)
            {
                pe.GC.FillRectangle(new NanoSolidBrush(iListView.ColumnsSubLineColor),
                                    new Rectangle(CurrentMousePosition.X, 1, columns[selectedColumnIndex].Width, columnHeight));
                //draw the column line
                pe.GC.DrawLine(new NanoPen(iListView.ColumnsTextColor), new Point(cX - HscrollOffset, 1),
                               new Point(cX - HscrollOffset, this.Height - 1));
                //draw the column text
                DrawString(pe.GC, columns[selectedColumnIndex].HeaderText, this.Font,
                           iListView.ColumnsTextColor, new Rectangle(CurrentMousePosition.X, 2, columns[selectedColumnIndex].Width, columnHeight));
            }

            pe.GC.DrawRectangle(new NanoPen(iListView.ListViewRectColor), 0, 0, Width, Height);
        }

        private void DrawThumbailsView(PaintEventArgs pe)
        {
            Size CharSize = WHUD.LibContext.MeasureText("TEST", this.Font);
            itemTextHeight = CharSize.Height * 2;

            int vLines = this.Height / (spaceBetweenItemsThunmbailsView + ThumbnailsHeight + itemTextHeight);
            int hLines = this.Width / (spaceBetweenItemsThunmbailsView + ThumbnailsWidth);
            if (hLines == 0) hLines = 1;
            int passedRows = VscrollOffset / (spaceBetweenItemsThunmbailsView + ThumbnailsHeight + itemTextHeight);
            int itemIndex = passedRows * hLines;
            if (itemIndex >= items.Count)
                return;
            int y = 2;
            for (int i = 0; i < vLines + 2; i++)
            {
                int x = spaceBetweenItemsThunmbailsView;
                for (int j = 0; j < hLines; j++)
                {
                    int offset = VscrollOffset % (spaceBetweenItemsThunmbailsView + ThumbnailsHeight + itemTextHeight);
                    if (highlightItemAsOver)
                    {
                        if (itemIndex == overItemSelectedIndex)
                        {
                            pe.GC.FillRectangle(new NanoSolidBrush(Color.LightGray),
                                                new Rectangle(x - 2, y - offset - 2, ThumbnailsWidth + 4, ThumbnailsHeight + itemTextHeight + 4));
                        }
                    }
                    if (items[itemIndex].Selected)
                        pe.GC.FillRectangle(new NanoSolidBrush(Color.LightSkyBlue),
                                            new Rectangle(x - 2, y - offset - 2, ThumbnailsWidth + 4, ThumbnailsHeight + itemTextHeight + 4));

                    StringFormat format = new StringFormat();
                    format.Alignment = StringAlignment.Center;
                    //format.LineAlignment = StringAlignment.Center;
                    format.Trimming = StringTrimming.EllipsisCharacter;
                    string textToDraw = "";
                    Image imageToDraw = null;

                    switch (items[itemIndex].DrawMode)
                    {
                        case EListViewItemDrawMode.Text:
                        case EListViewItemDrawMode.Image:
                        case EListViewItemDrawMode.TextAndImage:
                            //if (items[itemIndex].ImageIndex < ImagesList.Images.Count)
                            {
                                //imageToDraw = ImagesList.Images[items[itemIndex].ImageIndex];
                            }
                            textToDraw = items[itemIndex].Text;
                            break;

                        case EListViewItemDrawMode.UserDraw:
                            ListViewItemDrawArgs args = new ListViewItemDrawArgs(itemIndex);
                            if (DrawItem != null)
                                DrawItem(this, args);
                            imageToDraw = args.ImageToDraw;
                            textToDraw = args.TextToDraw;
                            break;
                    }
                    // Draw image
                    if (imageToDraw != null)
                    {
                        Size siz = CalculateStretchImageValues(imageToDraw.Width, imageToDraw.Height);
                        int imgX = x + (ThumbnailsWidth / 2) - (siz.Width / 2);
                        int imgY = (y - offset) + (ThumbnailsHeight / 2) - (siz.Height / 2);
                        //pe.Graphics.DrawImage(imageToDraw, new Rectangle(imgX, imgY, siz.Width, siz.Height));
                    }
                    // Draw text
                    DrawString(pe.GC, textToDraw, this.Font, Color.Black,
                               new Rectangle(x, y + ThumbnailsHeight + 1 - offset, ThumbnailsWidth, itemTextHeight));
                    // advance
                    x += ThumbnailsWidth + spaceBetweenItemsThunmbailsView;
                    itemIndex++;
                    if (itemIndex == items.Count)
                        break;
                }
                y += ThumbnailsHeight + itemTextHeight + spaceBetweenItemsThunmbailsView;
                if (itemIndex == items.Count)
                    break;
            }
        }
        private Size CalculateStretchImageValues(int imgW, int imgH)
        {
            float pRatio = (float)ThumbnailsWidth / ThumbnailsHeight;
            float imRatio = (float)imgW / imgH;
            int viewImageWidth = 0;
            int viewImageHeight = 0;

            if (ThumbnailsWidth >= imgW && ThumbnailsHeight >= imgH)
            {
                viewImageWidth = imgW;
                viewImageHeight = imgH;
            }
            else if (ThumbnailsWidth > imgW && ThumbnailsHeight < imgH)
            {
                viewImageHeight = ThumbnailsHeight;
                viewImageWidth = (int)(ThumbnailsHeight * imRatio);
            }
            else if (ThumbnailsWidth < imgW && ThumbnailsHeight > imgH)
            {
                viewImageWidth = ThumbnailsWidth;
                viewImageHeight = (int)(ThumbnailsWidth / imRatio);
            }
            else if (ThumbnailsWidth < imgW && ThumbnailsHeight < imgH)
            {
                if (ThumbnailsWidth >= ThumbnailsHeight)
                {
                    //width image
                    if (imgW >= imgH && imRatio >= pRatio)
                    {
                        viewImageWidth = ThumbnailsWidth;
                        viewImageHeight = (int)(ThumbnailsWidth / imRatio);
                    }
                    else
                    {
                        viewImageHeight = ThumbnailsHeight;
                        viewImageWidth = (int)(ThumbnailsHeight * imRatio);
                    }
                }
                else
                {
                    if (imgW < imgH && imRatio < pRatio)
                    {
                        viewImageHeight = ThumbnailsHeight;
                        viewImageWidth = (int)(ThumbnailsHeight * imRatio);
                    }
                    else
                    {
                        viewImageWidth = ThumbnailsWidth;
                        viewImageHeight = (int)(ThumbnailsWidth / imRatio);
                    }
                }
            }

            return new Size(viewImageWidth, viewImageHeight);
        }

        private void items_ItemsCollectionChanged(object sender, ListViewItemParameter e)
        {
            OnRefreshScrollBars();
            Invalidate();
        }

        private void columns_ColumnsCollectionChanged(object sender, ListViewColumnParameter e)
        {
            OnRefreshScrollBars();
            Invalidate();
        }

        private void columns_CollectionClear(object sender, EventArgs e)
        {
            if (ClearScrolls != null)
                ClearScrolls(this, null);

            highlightSelectedColumn = false;
            highlightItemAsOver = false;
            overItemSelectedIndex = -1;
            OldoverItemSelectedIndex = -1;
            LatestOverItemSelectedIndex = -1;
            HscrollOffset = 0;
            VscrollOffset = 0;

            OnRefreshScrollBars();

            Invalidate();
        }
    }
}
