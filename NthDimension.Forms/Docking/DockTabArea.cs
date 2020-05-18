using NthDimension.Forms.Widgets;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms.Docking
{
    public class DockTabArea
    {
        private Dictionary<DockContent, DockTab> _tabs = new Dictionary<DockContent, DockTab>();

        private List<MenuStripItem> _menuItems = new List<MenuStripItem>();
        private ContextMenuStrip _tabMenu = new ContextMenuStrip();

        #region Properties
        public EDockArea DockArea { get; private set; }

        public Rectangle ClientRectangle { get; set; }

        public Rectangle DropdownRectangle { get; set; }

        public bool DropdownHot { get; set; }

        public int Offset { get; set; }

        public int TotalTabSize { get; set; }

        public bool Visible { get; set; }

        public DockTab ClickedCloseButton { get; set; }
        #endregion

        public DockTabArea(EDockArea dockArea)
        {
            DockArea = dockArea;
        }

        public void ShowMenu(Widget control, Point location)
        {
            _tabMenu.Location = location;
            _tabMenu.Show();
        }

        public void AddMenuItem(MenuStripItem menuItem)
        {
            _menuItems.Add(menuItem);
            RebuildMenu();
        }

        public void RemoveMenuItem(MenuStripItem menuItem)
        {
            _menuItems.Remove(menuItem);
            RebuildMenu();
        }

        public MenuStripItem GetMenuItem(DockContent content)
        {
            MenuStripItem menuItem = null;
            foreach (MenuStripItem item in _menuItems)
            {
                var menuContent = item.Tag as DockContent;
                if (menuContent == null)
                    continue;

                if (menuContent == content)
                    menuItem = item;
            }

            return menuItem;
        }

        public void RebuildMenu()
        {
            _tabMenu.Widgets.Clear();

            var orderedItems = new List<MenuStripItem>();

            var index = 0;
            for (var i = 0; i < _menuItems.Count; i++)
            {
                foreach (var item in _menuItems)
                {
                    var content = (DockContent)item.Tag;
                    if (content.Order == index)
                        orderedItems.Add(item);
                }
                index++;
            }

            foreach (var item in orderedItems)
                _tabMenu.Widgets.Add(item);
        }
    }
}
