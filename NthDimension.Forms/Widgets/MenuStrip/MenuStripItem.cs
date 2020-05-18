using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

using NthDimension.Forms.Delegates;
using NthDimension.Forms.Events;
using NthDimension.Forms.Layout;

namespace NthDimension.Forms.Widgets
{
    /// <summary>
	/// Description of MenuStripItem.
	/// </summary>
	public class MenuStripItem : Widget
    {
        public event ItemClickedHandler ItemClickedEvent;
        public event ContextMenuHideChangedHandler ContextMenuHideChangedEvent;

        readonly ContextMenuStrip contextMenu;

        readonly Label iLabel;
        Label iShortCut;
        ArrowMenuItem iArrowMenuItem;

        public readonly int separation = 20;

        bool menuContext;

        public MenuStripItem(string text, bool menuContext = false, string shortCutText = null)
        {
            NotifyForMouseDown = true;
            NotifyForMouseUp = true;

            this.menuContext = menuContext;

            MenuItems = new ItemList();

            if (menuContext)
                PaintBackGround = false;
            iLabel = new Label(text);
            iLabel.TextAlign = ETextAlignment.Left | ETextAlignment.CenterV;
            iLabel.Margin = new Spacing(6, 3, 6, 3);
            iLabel.Font = Font = new NanoFont(NanoFont.DefaultRegular, 9f);
            iLabel.Dock = EDocking.Fill;
            //iLabel.ShowBoundsLines = true;
            //iLabel.ShowMarginLines = true;

            Widgets.Add(iLabel);

            Dock = menuContext ? EDocking.Top : EDocking.Left;

            if (shortCutText == null)
                ShortCut = string.Empty;
            else
            {
                ShortCut = shortCutText;

                iShortCut = new Label(shortCutText);
                iShortCut.Font = Font;
                iShortCut.TextAlign = ETextAlignment.Left | ETextAlignment.CenterV;
                iShortCut.Margin = new Spacing(separation, 3, 6, 3);
                iShortCut.Dock = EDocking.Right;
                iShortCut.Size = new Size(iShortCut.AutoSize.Width, 0);
                iShortCut.MouseDownEvent += IShortCut_MouseDownEvent;

                Widgets.Add(iShortCut);
            }

            contextMenu = new ContextMenuStrip();
            contextMenu.ShowBoundsLines = true;
            contextMenu.BoundsOutlineColor = Color.Black;
            contextMenu.HideChangedEvent += (sender, ea) => {
                if (ContextMenuHideChangedEvent != null)
                    ContextMenuHideChangedEvent(this, EventArgs.Empty);
            };

            MenuItems.MenuItemAddedEvent += MenuItems_MenuItemAddedEvent;
            MenuItems.MenuItemRemoveEvent += MenuItems_MenuItemRemoveEvent;
            iLabel.MouseDownEvent += ILabel_MouseDownEvent;

            SetColor(Color.White, true);
        }

        public bool IsHideContextMenu
        {
            get
            {
                return contextMenu.IsHide;
            }
        }

        public bool IsFocusedContextMenu
        {
            get
            {
                return contextMenu.IsFocused;
            }
        }

        void SetColor(Color c, bool isItemEnable)
        {
            if (isItemEnable)
            {
                iLabel.FGColor = c;
                if (iShortCut != null)
                    iShortCut.FGColor = c;
                if (iArrowMenuItem != null)
                    iArrowMenuItem.FGColor = c;
            }
            else
            {
                iLabel.FGColor = Color.Gray;
                if (iShortCut != null)
                    iShortCut.FGColor = Color.Gray;
                if (iArrowMenuItem != null)
                    iArrowMenuItem.FGColor = Color.Gray;
            }
        }

        bool IsItemEnable_ = true;
        public bool IsItemEnable
        {
            get { return IsItemEnable_; }
            set
            {
                if (value == IsItemEnable_)
                    return;

                IsItemEnable_ = value;
                SetColor(TextColor, value);
                Repaint();
            }
        }

        Widget Icon_;
        public Widget Icon
        {
            get
            {
                return Icon_;
            }
            set
            {
                if (value == Icon_)
                    return;
                Icon_ = value;
            }
        }

        public string Text
        {
            get { return iLabel.Text; }
        }

        Color TextColor_ = Color.White;
        public Color TextColor
        {
            get
            {
                return TextColor_;
            }
            set
            {
                if (value == TextColor_)
                    return;
                TextColor_ = value;
                Repaint();
            }
        }

        public string ShortCut
        {
            get;
            private set;
        }

        public ItemList MenuItems
        {
            get;
            private set;
        }

        internal void DoEnter()
        {
            BGColor = Color.FromArgb(135, 12, 62);
            Repaint();
        }

        internal void DoLeave()
        {
            BGColor = Color.FromArgb(66, 66, 66);
            Repaint();
        }

        void IShortCut_MouseDownEvent(Widget sender, MouseDownEventArgs mea)
        {
            if (MenuItems.Count > 0 || !IsItemEnable)
                mea.FocusedLostFocusOnMouseDown = false;
        }

        void ILabel_MouseDownEvent(Widget sender, MouseDownEventArgs mea)
        {
            if (MenuItems.Count > 0 || !IsItemEnable)
                mea.FocusedLostFocusOnMouseDown = false;
        }

        internal void RaiseItemClickedEvent(EventArgs e)
        {
            Focus();

            if (ItemClickedEvent != null)
                ItemClickedEvent(this, e);
        }

        void MenuItems_MenuItemAddedEvent(Widget itemAdded)
        {
            contextMenu.Widgets.Add(itemAdded);

            if (iArrowMenuItem == null && menuContext)
            {
                if (iShortCut != null)
                {
                    Widgets.Remove(iShortCut);
                    iShortCut = null;
                    ShortCut = string.Empty;
                }
                iArrowMenuItem = new ArrowMenuItem(this);
                iArrowMenuItem.Dock = EDocking.Right;
                iArrowMenuItem.Size = new Size(iLabel.AutoSize.Height, 0);
                //iArrowMenuItem.ShowBoundsLines = true;

                if (IsItemEnable)
                    iArrowMenuItem.FGColor = TextColor;
                else
                    iArrowMenuItem.FGColor = Color.Gray;


                if (string.Compare(iLabel.Text, "File2", StringComparison.InvariantCulture) == 0)
                {
                    string s = Text;
                }

                iArrowMenuItem.PaintBackGround = false;
                Widgets.Insert(0, iArrowMenuItem);
            }
        }

        void MenuItems_MenuItemRemoveEvent(Widget itemRemoved)
        {
            contextMenu.Widgets.Remove(itemRemoved);

            if (contextMenu.Widgets.Count <= 0 && iArrowMenuItem != null)
                Widgets.Remove(iArrowMenuItem);
        }

        public Size CalculateSize()
        {
            int iLw = iLabel.Margin.Left + iLabel.AutoSize.Width + iLabel.Margin.Right;
            int iLh = iLabel.Margin.Top + iLabel.AutoSize.Height + iLabel.Margin.Bottom;
            int iExtraWidth = 0;

            if (iShortCut != null)
            {
                iExtraWidth = iShortCut.Margin.Left + iShortCut.AutoSize.Width + iShortCut.Margin.Right;
            }
            else if (iArrowMenuItem != null)
            {
                iExtraWidth = iLh;
            }
            Size = new Size(iLw + iExtraWidth, iLh);

            return Size;
        }

        protected override void OnBeforeParentLayout()
        {
            base.OnBeforeParentLayout();

            Size = CalculateSize();
        }

        #region Logic

        bool raiseItemClicked;

        void DoMouseDown()
        {
            if (IsItemEnable)
            {
                if (MenuItems.Count > 0
                    && !menuContext)
                {
                    Focus();
                }
                raiseItemClicked = true;
            }
            else
                raiseItemClicked = false;
        }

        protected override void OnMouseDown(MouseDownEventArgs e)
        {
            e.FocusedLostFocusOnMouseDown = false;
            base.OnMouseDown(e);

            DoMouseDown();
        }

        // RaiseItemClickedEvent() debe lanzarse en 'OnMouseUp()', si se lanza
        // en 'OnMouseDown()' puede ocurrir que se establezca 'IsItemEnable' a false
        // y entonces no se ejecutaría el código 'mea.FocusedLostFocusOnMouseDown = false'
        // dentro de 'IShortCut_MouseDownEvent()' ó 'ILabel_MouseDownEvent()', 
        // el 'ContextMenuStrip' asociado a este 'MenuStripItem' no se ocultaría.
        // Además, lo lógico es que los eventos 'Click' se disparen al soltar el pulsador del ratòn.

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (raiseItemClicked)
            {
                raiseItemClicked = false;

                if (ItemClickedEvent != null)
                    ItemClickedEvent(this, e);
            }
        }

        protected override void OnNotifyMouseDown(MouseEventArgs e)
        {
            base.OnNotifyMouseDown(e);

            DoMouseDown();
        }

        internal void ShowMenu(Point p)
        {
            Size = CalculateSize();

            if (MenuItems.Count > 0)
            {
                contextMenu.Location = p;

                var pc = Parent as MenuStrip;
                if (pc != null)
                    contextMenu.Show(WindowHUD);
                else
                    contextMenu.ShowLink(WindowHUD);
            }
            DoEnter();
        }

        internal void HideMenu()
        {
            contextMenu.Hide();
        }
        #endregion Logic

        #region InnerClass-ArrowMenuItem

        public class ArrowMenuItem : Widget
        {
            MenuStripItem cmi;

            public ArrowMenuItem(MenuStripItem cmi)
            {
                this.cmi = cmi;
            }

            protected override void OnMouseDown(MouseDownEventArgs e)
            {
                base.OnMouseDown(e);

                if (cmi.MenuItems.Count > 0)
                    e.FocusedLostFocusOnMouseDown = false;
            }

            protected override void DoPaint(PaintEventArgs e)
            {
                base.DoPaint(e);

                GContext gc = e.GC;

                int wDiv3 = ClientRect.Width / 3;
                int hDiv3 = ClientRect.Height / 3;
                int hDiv2 = ClientRect.Height / 2;

                gc.FillPolygon(new NanoSolidBrush(FGColor), new Point[]{
                                   new Point(ClientRect.Left + wDiv3, hDiv3),
                                   new Point(ClientRect.Left + wDiv3, ClientRect.Bottom - hDiv3),
                                   new Point(ClientRect.Right - wDiv3-1, hDiv2)});
            }
        }
        #endregion InnerClass-ArrowMenuItem
    }

    public class ItemList : IList<Widget>
    {
        public event NeoMenuItemAddHandler MenuItemAddedEvent;
        public event NeoMenuItemRemoveHandler MenuItemRemoveEvent;

        readonly List<Widget> items = new List<Widget>();
        //Widget owner;

        public ItemList() //Widget owner)
        {
            //this.owner = owner;
        }

        #region IList implementation

        public int IndexOf(Widget item)
        {
            return items.IndexOf(item);
        }

        public void Insert(int index, Widget item)
        {
            if (items.Contains(item) == false)
            {
                items.Insert(index, item);
                //item.SetParent(owner);
                if (MenuItemAddedEvent != null)
                    MenuItemAddedEvent(item);
            }
        }

        public void RemoveAt(int index)
        {
            Widget wr = items[index];
            items.RemoveAt(index);
            //wr.SetParent(null);
            if (MenuItemRemoveEvent != null)
                MenuItemRemoveEvent(wr);
        }

        public Widget this[int index]
        {
            get
            {
                return items[index];
            }
            set
            {
                if (items.Contains(value) == false)
                {
                    //menuItems[index].SetParent(null);
                    items[index] = value;
                    //value.SetParent(owner);
                    if (MenuItemAddedEvent != null)
                        MenuItemAddedEvent(value);
                }
            }
        }

        #endregion

        #region ICollection<MenuItem> implementation

        public void Add(Widget item)
        {
            if (((Widget)item).Parent != null)
                throw new Exception("Widget ya añadido a otro Widget.");
            //if (item is WHUD)
            //	throw new Exception("El Widget 'WHUD' no puede añadirse a otro Widget.");

            if (items.Contains(item) == false)
            {
                items.Add(item);
                //item.SetParent(owner);
                if (MenuItemAddedEvent != null)
                    MenuItemAddedEvent(item);
            }
        }

        public void Clear()
        {
            //foreach (Widget w in menuItems)
            //	w.SetParent(null);
            items.Clear();
        }

        public bool Contains(Widget item)
        {
            return items.Contains(item);
        }

        public void CopyTo(Widget[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        public bool Remove(Widget item)
        {
            bool r = items.Remove(item);
            if (r && MenuItemRemoveEvent != null)
            {
                //item.SetParent(null);
                MenuItemRemoveEvent(item);
            }
            return r;
        }

        public int Count
        {
            get
            {
                return items.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region IEnumerable<Widget> implementation

        public IEnumerator<Widget> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        #endregion

        #region IEnumerable implementation

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }

        #endregion

        #region Public methods

        #endregion Public method
    }
}
