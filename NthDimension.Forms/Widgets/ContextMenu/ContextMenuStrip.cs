using NthDimension.Forms.Delegates;
using NthDimension.Forms.Events;
using NthDimension.Forms.Layout;
using NthDimension.Forms.Themes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms.Widgets
{
    public class ContextMenuStrip : Overlay
    {
        public event ItemClickedHandler MenuItemClickEvent;

        const int iconAreaWidth = 25;

        public ContextMenuStrip()
        {
            Size = new Size(10, 10);

            NotifyForMouseMove = true;

            SelectColor = Color.FromArgb(135, 12, 62);
            BGColor = Color.FromArgb(66, 66, 66);
            ShowBoundsLines = true;
            BoundsOutlineColor = Color.Black;
            Padding = new Spacing(iconAreaWidth, 0, 0, 0);

            Widgets.WidgetAddedEvent += Widgets_WidgetAddedEvent;
            Widgets.WidgetRemoveEvent += Widgets_WidgetRemoveEvent;
        }

        public Color SelectColor
        {
            get;
            set;
        }

        /// <summary>
        /// Es el item que ha desplegado un menú.
        /// </summary>
        internal MenuStripItem ActiveItem
        {
            get;
            set;
        }

        public MenuStripItem ItemMouseHovered
        {
            get;
            private set;
        }

        #region Widget Added Events

        void Widgets_WidgetAddedEvent(Widget widgetAdded)
        {
            var tsmi = widgetAdded as MenuStripItem;

            if (tsmi != null)
            {
                tsmi.ItemClickedEvent += WidgetAdded_ItemClickedEvent;
                tsmi.TextColor = Color.White;
            }
            widgetAdded.BGColor = Color.FromArgb(66, 66, 66);
            widgetAdded.LostFocusEvent += WidgetAdded_LostFocusEvent;
        }

        void Widgets_WidgetRemoveEvent(Widget widgetRemoved)
        {
            var tsmi = widgetRemoved as MenuStripItem;

            if (tsmi != null)
            {
                tsmi.ItemClickedEvent -= WidgetAdded_ItemClickedEvent;
            }
            widgetRemoved.LostFocusEvent -= WidgetAdded_LostFocusEvent;
        }

        void WidgetAdded_ItemClickedEvent(object sender, EventArgs ea)
        {
            var msi = sender as MenuStripItem;

            if (msi != null
                && ActiveItem == null
                && msi.MenuItems.Count > 0)
            {
                ActiveItem = msi;
            }
            else
            {
                ActiveItem = null;
            }

            if (MenuItemClickEvent != null)
                MenuItemClickEvent(sender, ea);
        }

        void WidgetAdded_LostFocusEvent(object sender, EventArgs e)
        {
            var mi = sender as MenuStripItem;

            if (mi != null && ActiveItem != null)
                mi.DoLeave();

            ActiveItem = null;
        }
        #endregion Widget Added Events

        bool raiseItemClicked;

        protected override void OnMouseDown(MouseDownEventArgs e)
        {
            // Cuando el ratón es pulsado dentro del área que comprende 'iconAreaWidth'
            // "Padding = new Spacing(iconAreaWidth, 0, 0, 0)"
            if (ItemMouseHovered != null
                && ItemMouseHovered.MenuItems.Count > 0
                || !ItemMouseHovered.IsItemEnable)
            {
                raiseItemClicked = false;
                e.FocusedLostFocusOnMouseDown = false;
            }
            else
                raiseItemClicked = true;

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (raiseItemClicked)
            {
                raiseItemClicked = false;
                ItemMouseHovered.RaiseItemClickedEvent(EventArgs.Empty);
            }
        }

        internal void ShowLink(WHUD winHUD)
        {
            RegisterOverlay(winHUD);
            Show();
        }

        public override void Hide()
        {
            base.Hide();

            if (ActiveItem != null)
                // Los 'ContextMenuStrip' se ocultan en cadena, como fichas de dominó,
                // desde el principal al último en mostrarse.
                ActiveItem.HideMenu();

            ActiveItem = null;
            rectEnter = Rectangle.Empty;
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            Hide();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (ActiveItem == null)
                rectEnter = Rectangle.Empty;
        }

        void MouseMoveMenuItem_MouseEnter(Widget w)
        {
            var mi = w as MenuStripItem;

            if (ActiveItem != null)
            {
                ActiveItem.HideMenu();
            }

            if (mi != null
                && mi.MenuItems.Count > 0
                && mi.IsItemEnable)
            {
                ActiveItem = mi;
                Point wp = ActiveItem.LocalToWindow();
                Point cp = LocalToWindow();
                var lp = new Point(cp.X + Width, wp.Y);
                ActiveItem.ShowMenu(lp);
            }
            else
                ActiveItem = null;
        }

        Rectangle rectEnter;

        void DoMouseMove(Point location)
        {
            foreach (Widget w in Widgets)
            {
                rectEnter = new Rectangle(0, w.Y, Width, w.Height);

                if (rectEnter.Contains(location))
                {
                    ItemMouseHovered = w as MenuStripItem;

                    if (ItemMouseHovered != null)
                    {
                        if (ItemMouseHovered != ActiveItem)
                        {
                            MouseMoveMenuItem_MouseEnter(ItemMouseHovered);
                        }
                    }
                    else
                        rectEnter = Rectangle.Empty;

                    break;
                }
            }

            // Esto ocurre cuando el puntero del ratón pasa por encima de un'SeparatorMenuItem' u otro
            // 'Widget'. Sobre cualquier item que no sea un 'MenuStripItem'.
            if (ItemMouseHovered == null)
            {
                if (ActiveItem != null)
                {
                    ActiveItem.HideMenu();
                    ActiveItem = null;
                }
            }

            Repaint();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            DoMouseMove(e.Location);
        }

        protected override void DoPaint(PaintEventArgs e)
        {
            base.DoPaint(e);

            GContext gc = e.GC;

            gc.FillRectangle(new NanoSolidBrush(ThemeEngine.BackColor2), //Color.FromArgb(72, 75, 85)),
                             0, 0, iconAreaWidth, Height);

            if (!rectEnter.IsEmpty)
            {
                gc.FillRectangle(new NanoSolidBrush(SelectColor),
                                 rectEnter.X + 1, rectEnter.Y, rectEnter.Width - 2, rectEnter.Height);
            }
        }

        public Size CalculateSize()
        {
            var result = new Size();

            if (Widgets.Count > 0)
            {
                int maxWidth = 10;
                int maxHeight = 0;
                int sizeWidth = 0;
                int sizeHeight = 0;
                int maxShortCutWidth = 0;

                foreach (Widget w in Widgets)
                {
                    w.Dock = EDocking.Top;

                    var mi = w as MenuStripItem;
                    if (mi != null)
                    {
                        Size ss = WHUD.LibContext.MeasureText(mi.ShortCut, mi.Font);
                        if (ss.Width > 0)
                            ss.Width += mi.separation;

                        maxShortCutWidth = System.Math.Max(maxShortCutWidth, ss.Width);
                        var miSize = mi.CalculateSize();
                        // 'miSize.Width, ss.Width y miSize.Height' solo varian
                        // si el texto cambia.
                        sizeWidth = miSize.Width - ss.Width;
                        sizeHeight = miSize.Height;
                    }
                    else
                    {
                        // 'w.Width' puede variar al ajustar el 'layout' de 'w'
                        // por esto no se puede utilizar aquí
                        //sizeWidth = w.Width;
                        sizeHeight = w.Height;
                    }

                    maxWidth = System.Math.Max(maxWidth, sizeWidth);
                    maxHeight += sizeHeight;
                }
                result = new Size(iconAreaWidth + maxWidth
                                  + (maxShortCutWidth > 0 ? maxShortCutWidth : 20), maxHeight);
            }

            return result;
        }

        protected override void OnBeforeParentLayout()
        {
            base.OnBeforeParentLayout();

            Size = CalculateSize();
        }
    }
}
