using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NthDimension.Forms.Events;
using NthDimension.Forms.Layout;

namespace NthDimension.Forms.Widgets
{
    public class MenuStrip : Widget
    {
        public MenuStrip()
        {
            Size = new Size(0, 22);
            Padding = new Spacing(3, 0, 3, 0);
            Dock = EDocking.Top;

            Widgets.WidgetAddedEvent += Widgets_WidgetAddedEvent;
            Widgets.WidgetRemoveEvent += Widgets_WidgetRemoveEvent;
        }

        /// <summary>
        /// Es el elemento seleccionado.
        /// </summary>
        internal MenuStripItem ActiveItem
        {
            get;
            set;
        }

        void Widgets_WidgetAddedEvent(Widget widgetAdded)
        {
            var tsmi = widgetAdded as MenuStripItem;

            if (tsmi != null)
            {
                tsmi.TextColor = Color.White;
                tsmi.ItemClickedEvent += WidgetAdded_ItemClickedEvent;
                tsmi.ContextMenuHideChangedEvent += tsmi_ContextMenuHideChangedEvent;
            }
            widgetAdded.BGColor = Color.FromArgb(66, 66, 66);
            widgetAdded.MouseEnterEvent += WidgetAdded_MouseEnterEvent;
            widgetAdded.MouseLeaveEvent += widgetAdded_MouseLeaveEvent;
        }

        void Widgets_WidgetRemoveEvent(Widget widgetRemoved)
        {
            var tsmi = widgetRemoved as MenuStripItem;

            if (tsmi != null)
            {
                tsmi.ItemClickedEvent -= WidgetAdded_ItemClickedEvent;
                tsmi.ContextMenuHideChangedEvent -= tsmi_ContextMenuHideChangedEvent;
            }
            widgetRemoved.MouseEnterEvent -= WidgetAdded_MouseEnterEvent;
            widgetRemoved.MouseLeaveEvent -= widgetAdded_MouseLeaveEvent;
        }
        // Antes de entrar en 'WidgetAdded_ItemClickedEvent()' 
        // el 'iMenuStripItem.contextMenu' es ocultado ya que pierde el foco,
        // y por lo tanto en 'tsmi_ContextMenuHideChangedEvent()'
        // AtiveItem pasa a valer nulo.
        // Despues al entrar en 'WidgetAdded_ItemClickedEvent()' y valiendo 
        // ActiveItem nulo, no es posible abrir el menú. Para evitar esto
        // se utiliza 'onContextHide'.
        bool onContextHide;

        void WidgetAdded_ItemClickedEvent(object sender, EventArgs ea)
        {
            var msi = sender as MenuStripItem;

            if (ActiveItem == null
               && msi != null
               && msi.MenuItems.Count > 0
               && !onContextHide)
            {
                ActiveItem = msi;
                DoShowMenu(ActiveItem);
            }
            onContextHide = false;
        }

        void tsmi_ContextMenuHideChangedEvent(object sender, EventArgs ea)
        {
            var msi = sender as MenuStripItem;

            if (msi != null && msi.IsHideContextMenu)
            {
                onContextHide = true;
                if (!ActiveItem.IsFocused)
                {
                    ActiveItem.DoLeave();
                }
                ActiveItem = null;
            }
        }

        void WidgetAdded_MouseEnterEvent(object sender, EventArgs e)
        {
            onContextHide = false;
            var msi = sender as MenuStripItem;

            if (ActiveItem != null)
            {
                if (sender != ActiveItem)
                {
                    ActiveItem.DoLeave();
                    DoHideMenu(ActiveItem);

                    ActiveItem = msi;
                    DoShowMenu(ActiveItem);
                }
            }

            if (msi != null)
            {
                msi.DoEnter();
            }
        }

        void widgetAdded_MouseLeaveEvent(object sender, EventArgs e)
        {
            var msi = sender as MenuStripItem;

            if (msi != null && !msi.IsFocusedContextMenu)
                msi.DoLeave();
        }

        void DoShowMenu(MenuStripItem msi)
        {
            Point wp = msi.LocalToWindow();
            var cl = new Point(wp.X, wp.Y + Height);

            msi.ShowMenu(cl);
        }

        void DoHideMenu(MenuStripItem msi)
        {
            if (ActiveItem != null)
                msi.DoLeave();
            msi.HideMenu();

            ActiveItem = null;
        }

        protected override void OnMouseDown(MouseDownEventArgs e)
        {
            base.OnMouseDown(e);

            Focus();
        }

        Size CalcSize()
        {
            int maxHeight = 0;

            foreach (Widget w in Widgets)
            {
                maxHeight = Math.Max(maxHeight, w.Size.Height);
            }
            return new Size(Size.Width, maxHeight);
        }

        protected override void OnBeforeParentLayout()
        {
            base.OnBeforeParentLayout();

            Dock = EDocking.Top;

            Size ns = CalcSize();

            if (ns.Height > 22)
                Size = ns;
        }

        protected override void DoPaint(PaintEventArgs e)
        {
            base.DoPaint(e);
        }
    }
}
