using System;
using System.Drawing;
using System.ComponentModel;

using NthDimension.Forms;
using NthDimension.Forms.Widgets;
using NthDimension.Forms.Events;
using NthDimension.Forms.Layout;
using NthDimension;

namespace NthStudio.Gui.Widgets.TabStrip
{
    /*
     * Original code: https://www.codeproject.com/Articles/13902/TabStrips-A-TabControl-in-the-Visual-Studio-w
     */

    public class TabStrip : Widget, IDisposable
    {
        #region Events

        public event TabStripItemClosingHandler TabStripItemClosing;
        public event TabStripItemChangedHandler TabStripItemSelectionChanged;
        public event HandledEventHandler MenuItemsLoading;
        public event EventHandler MenuItemsLoaded;
        public event EventHandler TabStripItemClosed;

        #endregion

        #region Constants

        private const int DEF_HEADER_HEIGHT = 19;
        private const int DEF_GLYPH_WIDTH = 40;

        private int DEF_START_POS = 10;

        #endregion

        #region Static Fields

        internal static int PreferredWidth = 350;
        internal static int PreferredHeight = 200;

        #endregion

        #region Fields

        private Rectangle                   stripButtonRect = Rectangle.Empty;
        private TabStripItem                selectedItem = null;
        private ContextMenuStrip            menu = null;
        private TabStripMenuGlyph           menuGlyph = null;
        private TabStripCloseButton         closeButton = null;
        private TabStripItemCollection items;
        private StringFormat sf = null;
        private static NanoFont defaultFont = new NanoFont(NanoFont.DefaultRegular, 10f);

        private bool alwaysShowClose = true;
        private bool isIniting = false;
        private bool alwaysShowMenuGlyph = true;

        TabStripItem lastTabFocus = null;
        //ToolTip tooltip;
        bool ToolTipVisible = false;

        #endregion

        #region Ctor

        public TabStrip()
        {
            BGColor = Color.FromArgb(48, 48, 48);
            FGColor = Color.White;

            /*tooltip = new ToolTip();
			tooltip.ReshowDelay = 500;
			tooltip.InitialDelay = 500;
			tooltip.Popup += (object sender, PopupEventArgs e) =>
			{
				Point mp = PointToClient(MousePosition);
				TabStripItem item = GetTabItemByPoint(mp);
				if (item == null)
					e.Cancel = true;
			};*/

            BeginInit();

            /*SetStyle(ControlStyles.ContainerControl, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.Selectable, true);
			*/
            items = new TabStripItemCollection();
            items.CollectionChanged += new CollectionChangeEventHandler(OnCollectionChanged);
            base.Size = new Size(350, 200);

            menu = new ContextMenuStrip();
            //menu.Renderer = ToolStripRenderer;
            //menu.VisibleChanged += new EventHandler(OnMenuVisibleChanged);
            menu.MenuItemClickEvent += OnMenuItemClicked;

            menuGlyph = new TabStripMenuGlyph();
            closeButton = new TabStripCloseButton();
            Font = defaultFont;
            sf = new StringFormat();

            EndInit();

            UpdateLayout();
        }

        #endregion

        #region Props

        [DefaultValue(null)]
        public TabStripItem SelectedItem
        {
            get { return selectedItem; }
            set
            {
                if (selectedItem == value)
                {
                    return;
                }

                if (value == null && Items.Count > 0)
                {
                    TabStripItem itm = Items[0];
                    if (itm.IsVisible)
                    {
                        selectedItem = itm;
                        selectedItem.Selected = true;
                        selectedItem.Dock = EDocking.Fill;
                    }
                }
                else
                {
                    selectedItem = value;
                }

                foreach (TabStripItem itm in Items)
                {
                    if (itm == selectedItem)
                    {
                        SelectItem(itm);
                        itm.Dock = EDocking.Fill;
                        itm.Show();
                    }
                    else
                    {
                        UnSelectItem(itm);
                        itm.Hide();
                    }
                }

                SelectItem(selectedItem);
                Invalidate();

                if (!selectedItem.IsDrawn)
                {
                    Items.MoveTo(0, selectedItem);
                    Invalidate();
                }

                selectedItem.Focus();

                OnTabStripItemChanged(
                    new TabStripItemChangedEventArgs(selectedItem, TabStripItemChangeTypes.SelectionChanged));
            }
        }

        [DefaultValue(true)]
        public bool AlwaysShowMenuGlyph
        {
            get { return alwaysShowMenuGlyph; }
            set
            {
                if (alwaysShowMenuGlyph == value)
                    return;

                alwaysShowMenuGlyph = value;
                Invalidate();
            }
        }

        [DefaultValue(true)]
        public bool AlwaysShowClose
        {
            get { return alwaysShowClose; }
            set
            {
                if (alwaysShowClose == value)
                    return;

                alwaysShowClose = value;
                Invalidate();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TabStripItemCollection Items
        {
            get { return items; }
        }

        [DefaultValue(typeof(Size), "350,200")]
        public new Size Size
        {
            get { return base.Size; }
            set
            {
                if (base.Size == value)
                    return;

                base.Size = value;
                UpdateLayout();
            }
        }

        /// <summary>
        /// DesignerSerializationVisibility
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new WidgetList Widgets
        {
            get { return base.Widgets; }
        }

        #endregion

        #region ShouldSerialize

        public bool ShouldSerializeFont()
        {
            return Font != null && !Font.Equals(defaultFont);
        }

        public bool ShouldSerializeSelectedItem()
        {
            return true;
        }

        public bool ShouldSerializeItems()
        {
            return items.Count > 0;
        }

        public void ResetFont()
        {
            Font = defaultFont;
        }

        #endregion

        #region ISupportInitialize Members

        public void BeginInit()
        {
            isIniting = true;
        }

        public void EndInit()
        {
            isIniting = false;
        }
        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// Returns hit test results
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public HitTestResult HitTest(Point pt)
        {
            if (closeButton.Bounds.Contains(pt))
                return HitTestResult.CloseButton;

            if (menuGlyph.Bounds.Contains(pt))
                return HitTestResult.MenuGlyph;

            if (GetTabItemByPoint(pt) != null)
                return HitTestResult.TabItem;

            //No other result is available.
            return HitTestResult.None;
        }

        /// <summary>
        /// Add a <see cref="TabStripItem"/> to this control without selecting it.
        /// </summary>
        /// <param name="tabItem"></param>
        public void AddTab(TabStripItem tabItem)
        {
            AddTab(tabItem, false);
        }

        /// <summary>
        /// Add a <see cref="TabStripItem"/> to this control.
        /// User can make the currently selected item or not.
        /// </summary>
        /// <param name="tabItem"></param>
        /// <param name = "autoSelect"></param>
        public void AddTab(TabStripItem tabItem, bool autoSelect)
        {
            tabItem.Dock = EDocking.Fill;
            Items.Add(tabItem);

            if ((autoSelect && tabItem.IsVisible) || (tabItem.IsVisible && Items.DrawnCount < 1))
            {
                SelectedItem = tabItem;
                SelectItem(tabItem);
            }
        }

        /// <summary>
        /// Remove a <see cref="TabStripItem"/> from this control.
        /// </summary>
        /// <param name="tabItem"></param>
        public void RemoveTab(TabStripItem tabItem)
        {
            int tabIndex = Items.IndexOf(tabItem);

            if (tabIndex >= 0)
            {
                UnSelectItem(tabItem);
                Items.Remove(tabItem);
            }

            if (Items.Count > 0)
            {
                //if (RightToLeft == RightToLeft.No)
                {
                    if (Items[tabIndex - 1] != null)
                    {
                        SelectedItem = Items[tabIndex - 1];
                    }
                    else
                    {
                        SelectedItem = Items.FirstVisible;
                    }
                }
                /*else
				{
					if (Items[tabIndex + 1] != null)
					{
						SelectedItem = Items[tabIndex + 1];
					}
					else
					{
						SelectedItem = Items.LastVisible;
					}
				}*/
            }
        }

        /// <summary>
        /// Get a <see cref="TabStripItem"/> at provided point.
        /// If no item was found, returns null value.
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public TabStripItem GetTabItemByPoint(Point pt)
        {
            TabStripItem item = null;
            bool found = false;

            for (int i = 0; i < Items.Count; i++)
            {
                TabStripItem current = Items[i];

                if (current.StripRect.Contains(pt) && current.IsVisible && current.IsDrawn)
                {
                    item = current;
                    found = true;
                }

                if (found)
                    break;
            }

            return item;
        }

        /// <summary>
        /// Display items menu
        /// </summary>
        public virtual void ShowMenu()
        {
            if (menu.Widgets.Count > 0)
            {
                //if (RightToLeft == RightToLeft.No)
                {
                    Size s = menu.CalculateSize();
                    Point mp = LocalToWindow();
                    menu.Location = new Point(mp.X + menuGlyph.Bounds.Left - (s.Width - DEF_GLYPH_WIDTH),
                                              mp.Y + menuGlyph.Bounds.Bottom);
                    Widget w = FocusedWidget;
                    menu.Show(WindowHUD);
                }
                /*else
				{
					//menu.Show(this, new Point(menuGlyph.Bounds.Right, menuGlyph.Bounds.Bottom));
				}*/
            }
        }

        #endregion Public

        #region Internal

        internal void UnDrawAll()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].IsDrawn = false;
            }
        }

        internal void SelectItem(TabStripItem tabItem)
        {
            tabItem.Dock = EDocking.Fill;
            tabItem.Show();
            tabItem.Selected = true;
        }

        internal void UnSelectItem(TabStripItem tabItem)
        {
            tabItem.Hide();
            tabItem.Selected = false;
        }

        #endregion Internal


        #region Protected

        /// <summary>
        /// Fires <see cref="TabStripItemClosing"/> event.
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnTabStripItemClosing(TabStripItemClosingEventArgs e)
        {
            if (TabStripItemClosing != null)
                TabStripItemClosing(e);
        }

        /// <summary>
        /// Fires <see cref="TabStripItemClosed"/> event.
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnTabStripItemClosed(EventArgs e)
        {
            if (TabStripItemClosed != null)
                TabStripItemClosed(this, e);
        }

        /// <summary>
        /// Fires <see cref="MenuItemsLoading"/> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMenuItemsLoading(HandledEventArgs e)
        {
            if (MenuItemsLoading != null)
                MenuItemsLoading(this, e);
        }

        /// <summary>
        /// Fires <see cref="MenuItemsLoaded"/> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMenuItemsLoaded(EventArgs e)
        {
            if (MenuItemsLoaded != null)
                MenuItemsLoaded(this, e);
        }

        /// <summary>
        /// Fires <see cref="TabStripItemSelectionChanged"/> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTabStripItemChanged(TabStripItemChangedEventArgs e)
        {
            if (TabStripItemSelectionChanged != null)
                TabStripItemSelectionChanged(e);
        }

        /// <summary>
        /// Loads menu items based on <see cref="TabStripItem"/>s currently added
        /// to this control.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMenuItemsLoad(EventArgs e)
        {
            //menu.RightToLeft = RightToLeft;
            menu.Widgets.Clear();

            for (int i = 0; i < Items.Count; i++)
            {
                TabStripItem item = Items[i];
                if (!item.IsVisible)
                    continue;

                MenuStripItem tItem = new MenuStripItem(item.Title, true);
                tItem.Tag = item;
                //tItem.Image = item.Image;
                menu.Widgets.Add(tItem);
            }

            OnMenuItemsLoaded(EventArgs.Empty);
        }

        #endregion Protected

        #region Overrides

        protected override void DoPaint(PaintEventArgs e)
        {
            SetDefaultSelected();
            Rectangle borderRc = ClientRect;
            borderRc.Width--;
            borderRc.Height--;

            //if (RightToLeft == RightToLeft.No)
            {
                DEF_START_POS = 10;
            }
            /*else
			{
				DEF_START_POS = stripButtonRect.Right;
			}*/

            e.GC.DrawRectangle(new NanoPen(SystemColors.ControlDark), borderRc);
            //e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            #region Draw Pages

            for (int i = 0; i < Items.Count; i++)
            {
                TabStripItem currentItem = Items[i];
                if (!currentItem.IsVisible)
                    continue;

                OnCalcTabPage(e.GC, currentItem);
                currentItem.IsDrawn = false;

                if (!AllowDraw(currentItem))
                    continue;

                OnDrawTabPage(e.GC, currentItem);
            }

            #endregion

            #region Draw UnderPage Line

            Color cul = Color.FromArgb(71, 71, 71);

            //if (RightToLeft == RightToLeft.No)
            {
                if (Items.DrawnCount == 0 || Items.VisibleCount == 0)
                {
                    e.GC.DrawLine(new NanoPen(cul),
                                  new Point(0, DEF_HEADER_HEIGHT),
                                  new Point(ClientRect.Width, DEF_HEADER_HEIGHT));
                }
                else if (SelectedItem != null && SelectedItem.IsDrawn)
                {
                    Point end = new Point((int)SelectedItem.StripRect.Left - 9, DEF_HEADER_HEIGHT);
                    e.GC.DrawLine(new NanoPen(cul),
                                  new Point(0, DEF_HEADER_HEIGHT), end);
                    end.X += (int)SelectedItem.StripRect.Width + 10;
                    e.GC.DrawLine(new NanoPen(cul),
                                  end,
                                  new Point(ClientRect.Width, DEF_HEADER_HEIGHT));
                }
            }
            /*else
			{
				if (Items.DrawnCount == 0 || Items.VisibleCount == 0)
				{
					e.GC.DrawLine(new NanoPen(SystemColors.ControlDark),
								  new Point(0, DEF_HEADER_HEIGHT),
								  new Point(ClientRect.Width, DEF_HEADER_HEIGHT));
				}
				else if (SelectedItem != null && SelectedItem.IsDrawn)
				{
					Point end = new Point((int)SelectedItem.StripRect.Left, DEF_HEADER_HEIGHT);
					e.GC.DrawLine(new NanoPen(SystemColors.ControlDark),
								  new Point(0, DEF_HEADER_HEIGHT), end);
					end.X += (int)SelectedItem.StripRect.Width + 20;
					e.GC.DrawLine(new NanoPen(SystemColors.ControlDark),
								  end,
								  new Point(ClientRect.Width, DEF_HEADER_HEIGHT));
				}
			}*/

            #endregion

            #region Draw Menu and Close Glyphs

            if (Items.Count > 0)
            {
                if (AlwaysShowMenuGlyph || Items.DrawnCount > Items.VisibleCount)
                    menuGlyph.DrawGlyph(e.GC, this);

                if (AlwaysShowClose || (SelectedItem != null && SelectedItem.CanClose))
                    closeButton.DrawCross(e.GC);
            }
            #endregion
        }

        protected override void OnMouseDown(MouseDownEventArgs e)
        {
            if (e.Button != MouseButton.Left)
                return;

            HitTestResult result = HitTest(e.Location);
            if (result == HitTestResult.MenuGlyph)
            {
                var args = new HandledEventArgs(false);
                OnMenuItemsLoading(args);

                if (!args.Handled)
                    OnMenuItemsLoad(EventArgs.Empty);

                ShowMenu();
                e.FocusedLostFocusOnMouseDown = false;
            }
            else if (result == HitTestResult.CloseButton && menu.IsHide)
            {
                if (SelectedItem != null)
                {
                    var args = new TabStripItemClosingEventArgs(SelectedItem);
                    OnTabStripItemClosing(args);
                    if (!args.Cancel && SelectedItem.CanClose)
                    {
                        RemoveTab(SelectedItem);
                        OnTabStripItemClosed(EventArgs.Empty);
                    }
                }
            }
            else if (result == HitTestResult.TabItem)
            {
                TabStripItem item = GetTabItemByPoint(e.Location);
                if (item != null)
                    SelectedItem = item;
            }

            Invalidate();

            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (menuGlyph.Bounds.Contains(e.Location))
            {
                menuGlyph.IsMouseOver = menu.IsHide;
                Invalidate(menuGlyph.Bounds);
            }
            else
            {
                menuGlyph.IsMouseOver = false;
                Invalidate(menuGlyph.Bounds);
            }

            if (closeButton.Bounds.Contains(e.Location))
            {
                closeButton.IsMouseOver = menu.IsHide;
                Invalidate(closeButton.Bounds);
            }
            else
            {
                closeButton.IsMouseOver = false;
                Invalidate(closeButton.Bounds);
            }

            #region ToolTip management

            TabStripItem item = GetTabItemByPoint(e.Location);

            if (item != null && item != lastTabFocus)
            {
                if (ToolTipVisible == false)
                {
                    lastTabFocus = item;
                    OnMouseTabEnter(item, e.Location);
                }
                else
                {
                    //if (Environment.OSVersion.Platform != PlatformID.Unix)
                    //	tooltip.RemoveAll();
                    OnMouseTabLeave(lastTabFocus);
                    lastTabFocus = null;
                }
            }
            if (item == null && lastTabFocus != null)
            {
                //if (Environment.OSVersion.Platform != PlatformID.Unix)
                //	tooltip.RemoveAll();
                OnMouseTabLeave(lastTabFocus);
                lastTabFocus = null;
            }

            #endregion ToolTip management
        }

        void OnMouseTabEnter(TabStripItem item, Point pos)
        {
            Cursors.Cursor = Cursors.Hand;

            if (!String.IsNullOrEmpty(item.TabPageToolTipText))
            {
                //tooltip.SetToolTip(this, item.TabPageToolTipText);
                ToolTipVisible = true;
            }
        }

        void OnMouseTabLeave(TabStripItem item)
        {
            //tooltip.Hide(this);
            ToolTipVisible = false;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            #region ToolTip management

            //if (Environment.OSVersion.Platform != PlatformID.Unix)
            //	tooltip.RemoveAll();
            //tooltip.Hide(this);
            lastTabFocus = null;
            ToolTipVisible = false;

            #endregion ToolTip management

            menuGlyph.IsMouseOver = false;
            Invalidate(menuGlyph.Bounds);

            closeButton.IsMouseOver = false;
            Invalidate(closeButton.Bounds);
        }

        #endregion Overrides

        #region Privates

        private bool AllowDraw(TabStripItem item)
        {
            bool result = true;

            //if (RightToLeft == RightToLeft.No)
            {
                if (item.StripRect.Right >= stripButtonRect.Width)
                    result = false;
            }
            /*else
			{
				if (item.StripRect.Left <= stripButtonRect.Left)
					return false;
			}*/

            return result;
        }

        private void SetDefaultSelected()
        {
            if (selectedItem == null && Items.Count > 0)
                SelectedItem = Items[0];

            for (int i = 0; i < Items.Count; i++)
            {
                TabStripItem itm = Items[i];
                itm.Dock = EDocking.Fill;
            }
        }

        private void OnMenuItemClicked(object sender, EventArgs e)
        {
            Widget tsi = sender as Widget;

            if (sender != null)
            {
                TabStripItem clickedItem = (TabStripItem)tsi.Tag;
                SelectedItem = clickedItem;
            }
        }

        private void OnCalcTabPage(GContext gc, TabStripItem currentItem)
        {
            NanoFont currentFont = Font;
            if (currentItem == SelectedItem)
                currentFont = NanoFont.DefaultRegularBold;

            //SizeF textSize = g.MeasureString(currentItem.Title, currentFont, new SizeF(200, 10), sf);
            Size textSize = WHUD.LibContext.MeasureText(currentItem.Title, currentFont);
            textSize.Width += 20;

            //if (RightToLeft == RightToLeft.No)
            {
                RectangleF buttonRect = new RectangleF(DEF_START_POS, 3, textSize.Width, 17);
                currentItem.StripRect = buttonRect;
                DEF_START_POS += (int)textSize.Width;
            }
            /*else
			{
				RectangleF buttonRect = new RectangleF(DEF_START_POS - textSize.Width + 1, 3, textSize.Width - 1, 17);
				currentItem.StripRect = buttonRect;
				DEF_START_POS -= (int)textSize.Width;
			}*/
        }

        private void OnDrawTabPage(GContext gc, TabStripItem currentItem)
        {
            bool isFirstTab = Items.IndexOf(currentItem) == 0;
            NanoFont currentFont = Font;
            int yoff = 0;
            Color ttc = FGColor;

            if (currentItem == SelectedItem)
                currentFont = NanoFont.DefaultRegularBold;

            //SizeF textSize = g.MeasureString(currentItem.Title, currentFont, new SizeF(200, 10), sf);
            Size textSize = WHUD.LibContext.MeasureText(currentItem.Title, currentFont);
            textSize.Width += 20;
            RectangleF buttonRect = currentItem.StripRect;

      //buttonRect.Y += currentItem.Height;

            //GraphicsPath path = new GraphicsPath();
            //LinearGradientBrush brush;
            Color brushColor;

            #region Draw Not Right-To-Left Tab

            //if (RightToLeft == RightToLeft.No)
            {
                /* int mtop = 3;
				
				if (currentItem == SelectedItem || isFirstTab)
				{
					path.AddLine(buttonRect.Left - 10, buttonRect.Bottom - 1,
								 buttonRect.Left + (buttonRect.Height / 2) - 4, mtop + 4);
				}
				else
				{
					path.AddLine(buttonRect.Left, buttonRect.Bottom - 1, buttonRect.Left,
								 buttonRect.Bottom - (buttonRect.Height / 2) - 2);
					path.AddLine(buttonRect.Left, buttonRect.Bottom - (buttonRect.Height / 2) - 3,
								 buttonRect.Left + (buttonRect.Height / 2) - 4, mtop + 3);
				}

				path.AddLine(buttonRect.Left + (buttonRect.Height / 2) + 2, mtop, buttonRect.Right - 3, mtop);
				path.AddLine(buttonRect.Right, mtop + 2, buttonRect.Right, buttonRect.Bottom - 1);
				path.AddLine(buttonRect.Right - 4, buttonRect.Bottom - 1, buttonRect.Left, buttonRect.Bottom - 1);
				path.CloseFigure();*/

                if (currentItem == SelectedItem)
                {
                    //brush = new LinearGradientBrush(buttonRect, SystemColors.ControlLightLight, SystemColors.Window, LinearGradientMode.Vertical);
                    brushColor = Color.FromArgb(82, 82, 82); // SystemColors.Window;
                    yoff = 4;
                }
                else
                {
                    //brush = new LinearGradientBrush(buttonRect, SystemColors.ControlLightLight, SystemColors.Control, LinearGradientMode.Vertical);
                    brushColor = Color.FromArgb(71, 71, 71); // SystemColors.Control;
                    yoff = 2;
                    buttonRect.Y += yoff;
                    buttonRect.Height -= yoff;
                    ttc = Color.LightGray;
                }

                //gc.DrawRectangle(new NanoPen(Color.Red, 1), new Rectangle((int)buttonRect.X,
                //    (int)buttonRect.Y, (int)buttonRect.Width , (int)buttonRect.Height));

                //gc.FillPath(brush, path);
                //gc.DrawPath(SystemPens.ControlDark, path);
                gc.FillRectangle(new NanoSolidBrush(brushColor),
                                 (int)buttonRect.X,
                                 (int)buttonRect.Y,
                                 (int)buttonRect.Width,
                                 (int)buttonRect.Height);

                if (currentItem == SelectedItem)
                {
                    gc.DrawLine(new NanoPen(brushColor),
                                (int)buttonRect.Left - 9,
                                (int)buttonRect.Height + 2,
                                (int)(buttonRect.Left + buttonRect.Width - 1),
                                (int)buttonRect.Height + 2);
                }

                PointF textLoc = new PointF(buttonRect.Left + buttonRect.Height - 4, buttonRect.Top + (buttonRect.Height / 2) - (textSize.Height / 2) - 3);
                RectangleF textRect = buttonRect;
                textRect.Location = textLoc;
                textRect.Width = buttonRect.Width - (textRect.Left - buttonRect.Left) - 4;
                textRect.Height = textSize.Height + currentFont.Height / 2;

                //if (currentItem == SelectedItem)
                {
                    int tAscender = (int)Math.Ceiling(Font.Ascender);
                    gc.DrawString(currentItem.Title, currentFont, new NanoSolidBrush(ttc),
                                  textRect.X, textRect.Y + tAscender + yoff);
                }
                /*else
				{
					gc.DrawString(currentItem.Title, currentFont, new SolidBrush(ForeColor), textRect, sf);
				}*/
            }

            #endregion


            #region Draw Right-To-Left Tab

            /*if (RightToLeft == RightToLeft.Yes)
			{
				if (currentItem == SelectedItem || isFirstTab)
				{
					path.AddLine(buttonRect.Right + 10, buttonRect.Bottom - 1,
								 buttonRect.Right - (buttonRect.Height / 2) + 4, mtop + 4);
				}
				else
				{
					path.AddLine(buttonRect.Right, buttonRect.Bottom - 1, buttonRect.Right,
								 buttonRect.Bottom - (buttonRect.Height / 2) - 2);
					path.AddLine(buttonRect.Right, buttonRect.Bottom - (buttonRect.Height / 2) - 3,
								 buttonRect.Right - (buttonRect.Height / 2) + 4, mtop + 3);
				}

				path.AddLine(buttonRect.Right - (buttonRect.Height / 2) - 2, mtop, buttonRect.Left + 3, mtop);
				path.AddLine(buttonRect.Left, mtop + 2, buttonRect.Left, buttonRect.Bottom - 1);
				path.AddLine(buttonRect.Left + 4, buttonRect.Bottom - 1, buttonRect.Right, buttonRect.Bottom - 1);
				path.CloseFigure();

				if (currentItem == SelectedItem)
				{
					brush =
						new LinearGradientBrush(buttonRect, SystemColors.ControlLightLight, SystemColors.Window,
												LinearGradientMode.Vertical);
				}
				else
				{
					brush =
						new LinearGradientBrush(buttonRect, SystemColors.ControlLightLight, SystemColors.Control,
												LinearGradientMode.Vertical);
				}

				g.FillPath(brush, path);
				g.DrawPath(SystemPens.ControlDark, path);

				if (currentItem == SelectedItem)
				{
					g.DrawLine(new Pen(brush), buttonRect.Right + 9, buttonRect.Height + 2,
							   buttonRect.Right - buttonRect.Width + 1, buttonRect.Height + 2);
				}

				PointF textLoc = new PointF(buttonRect.Left + 2, buttonRect.Top + (buttonRect.Height / 2) - (textSize.Height / 2) - 2);
				RectangleF textRect = buttonRect;
				textRect.Location = textLoc;
				textRect.Width = buttonRect.Width - (textRect.Left - buttonRect.Left) - 10;
				textRect.Height = textSize.Height + currentFont.Size / 2;

				if (currentItem == SelectedItem)
				{
					textRect.Y -= 1;
					g.DrawString(currentItem.Title, currentFont, new SolidBrush(ForeColor), textRect, sf);
				}
				else
				{
					g.DrawString(currentItem.Title, currentFont, new SolidBrush(ForeColor), textRect, sf);
				}

				//g.FillRectangle(Brushes.Red, textRect);
			}*/

            #endregion

            currentItem.IsDrawn = true;
        }

        private void UpdateLayout()
        {
            //if (RightToLeft == RightToLeft.No)
            {
                sf.Trimming = StringTrimming.EllipsisCharacter;
                sf.FormatFlags |= StringFormatFlags.NoWrap;
                sf.FormatFlags &= StringFormatFlags.DirectionRightToLeft;

                stripButtonRect = new Rectangle(0, 0, ClientRect.Width - DEF_GLYPH_WIDTH - 2, 10);
                menuGlyph.Bounds = new Rectangle(ClientRect.Width - DEF_GLYPH_WIDTH, 2, 16, 16);
                closeButton.Bounds = new Rectangle(ClientRect.Width - 20, 2, 16, 16);
            }
            /*else
			{
				sf.Trimming = StringTrimming.EllipsisCharacter;
				sf.FormatFlags |= StringFormatFlags.NoWrap;
				sf.FormatFlags |= StringFormatFlags.DirectionRightToLeft;

				stripButtonRect = new Rectangle(DEF_GLYPH_WIDTH + 2, 0, ClientRect.Width - DEF_GLYPH_WIDTH - 15, 10);
				closeButton.Bounds = new Rectangle(4, 2, 16, 16); //ClientSize.Width - DEF_GLYPH_WIDTH, 2, 16, 16);
				menuGlyph.Bounds = new Rectangle(20 + 4, 2, 16, 16); //this.ClientSize.Width - 20, 2, 16, 16);
			}*/

            /*DockPadding.Top = DEF_HEADER_HEIGHT + 1;
			DockPadding.Bottom = 1;
			DockPadding.Right = 1;
			DockPadding.Left = 1;*/

            Padding = new Spacing(1, DEF_HEADER_HEIGHT + 1, 1, 1);
        }

        private void OnCollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            TabStripItem itm = (TabStripItem)e.Element;

            if (e.Action == CollectionChangeAction.Add)
            {
                Widgets.Add(itm);
                OnTabStripItemChanged(new TabStripItemChangedEventArgs(itm, TabStripItemChangeTypes.Added));
            }
            else if (e.Action == CollectionChangeAction.Remove)
            {
                Widgets.Remove(itm);
                OnTabStripItemChanged(new TabStripItemChangedEventArgs(itm, TabStripItemChangeTypes.Removed));
            }
            else
            {
                OnTabStripItemChanged(new TabStripItemChangedEventArgs(itm, TabStripItemChangeTypes.Changed));
            }

            UpdateLayout();
            Invalidate();
        }

        #endregion Privates

        #endregion Methods

        protected override void OnLayout()
        {
            base.OnLayout();

            if (isIniting)
                return;

            UpdateLayout();
            Invalidate();
        }

        #region IDisposable

        ///<summary>
        ///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///</summary>
        ///<filterpriority>2</filterpriority>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                items.CollectionChanged -= new CollectionChangeEventHandler(OnCollectionChanged);
                //menu.ItemClicked -= new ToolStripItemClickedEventHandler(OnMenuItemClicked);
                //menu.VisibleChanged -= new EventHandler(OnMenuVisibleChanged);

                /*foreach (TabStripItem item in items)
				{
					if (item != null && !item.IsDisposed)
						item.Dispose();
				}

				if (menu != null && !menu.IsDisposed)
					menu.Dispose();*/

                if (sf != null)
                    sf.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion

    }
}
