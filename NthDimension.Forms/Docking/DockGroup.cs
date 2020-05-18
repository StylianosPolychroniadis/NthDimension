using NthDimension.Forms.Events;
using NthDimension.Forms.Layout;
using NthDimension.Forms.Widgets;
using NthDimension.Rasterizer;
using NthDimension.Rasterizer.NanoVG;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms.Docking
{
    public class DockGroup : Panel
    {
        private List<DockContent>                       _contents = new List<DockContent>();
        private Dictionary<DockContent, DockTab>        _tabs = new Dictionary<DockContent, DockTab>();
        private DockTabArea                             _tabArea;
        private DockTab                                 _dragTab = null;

        #region Properties
        public DockPanel DockPanel { get; private set; }

        public DockRegion DockRegion { get; private set; }

        public EDockArea DockArea { get; private set; }

        public DockContent VisibleContent { get; private set; }

        public int Order { get; set; }

        public int ContentCount { get { return _contents.Count; } }
        #endregion Properties

        public DockGroup(DockPanel dockPanel, DockRegion dockRegion, int order)
        {
            //SetStyle(ControlStyles.OptimizedDoubleBuffer |
            //         ControlStyles.ResizeRedraw |
            //         ControlStyles.UserPaint, true);

            DockPanel = dockPanel;
            DockRegion = dockRegion;
            DockArea = dockRegion.DockArea;

            Order = order;

            _tabArea = new DockTabArea(DockArea);

            DockPanel.ActiveContentChanged += DockPanel_ActiveContentChanged;
        }

        #region Event Handlers
        protected override void OnSizeChanged()
        {
            base.OnSizeChanged();

            UpdateTabArea();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_dragTab != null)
            {
                var offsetX = e.Location.X + _tabArea.Offset;
                if (offsetX < _dragTab.ClientRectangle.Left)
                {
                    if (_dragTab.DockContent.Order > 0)
                    {
                        var otherTabs = _tabs.Values.Where(t => t.DockContent.Order == _dragTab.DockContent.Order - 1).ToList();
                        if (otherTabs.Count == 0)
                            return;

                        var otherTab = otherTabs.First();

                        if (otherTab == null)
                            return;

                        var oldIndex = _dragTab.DockContent.Order;
                        _dragTab.DockContent.Order = oldIndex - 1;
                        otherTab.DockContent.Order = oldIndex;

                        BuildTabs();
                        EnsureVisible();

                        _tabArea.RebuildMenu();

                        return;
                    }
                }
                else if (offsetX > _dragTab.ClientRectangle.Right)
                {
                    var maxOrder = _contents.Count;

                    if (_dragTab.DockContent.Order < maxOrder)
                    {
                        var otherTabs = _tabs.Values.Where(t => t.DockContent.Order == _dragTab.DockContent.Order + 1).ToList();
                        if (otherTabs.Count == 0)
                            return;

                        var otherTab = otherTabs.First();

                        if (otherTab == null)
                            return;

                        var oldIndex = _dragTab.DockContent.Order;
                        _dragTab.DockContent.Order = oldIndex + 1;
                        otherTab.DockContent.Order = oldIndex;

                        BuildTabs();
                        EnsureVisible();

                        _tabArea.RebuildMenu();

                        return;
                    }
                }

                return;
            }

            if (_tabArea.DropdownRectangle.Contains(e.Location))
            {
                _tabArea.DropdownHot = true;

                foreach (var tab in _tabs.Values)
                    tab.Hot = false;

                Invalidate();
                return;
            }

            _tabArea.DropdownHot = false;

            foreach (var tab in _tabs.Values)
            {
                var rect = RectangleToTabArea(tab.ClientRectangle);
                var hot = rect.Contains(e.Location);

                if (tab.Hot != hot)
                {
                    tab.Hot = hot;
                    Invalidate();
                }

                var closeRect = RectangleToTabArea(tab.CloseButtonRectangle);
                var closeHot = closeRect.Contains(e.Location);

                if (tab.CloseButtonHot != closeHot)
                {
                    tab.CloseButtonHot = closeHot;
                    Invalidate();
                }
            }
        }

        protected override void OnMouseDown(MouseDownEventArgs e)
        {
            base.OnMouseDown(e);

            if (_tabArea.DropdownRectangle.Contains(e.Location))
            {
                _tabArea.DropdownHot = true;
                return;
            }

            foreach (var tab in _tabs.Values)
            {
                var rect = RectangleToTabArea(tab.ClientRectangle);
                if (rect.Contains(e.Location))
                {
                    if (e.Button == MouseButton.Middle)
                    {
                        throw new NotImplementedException("Close");
                        //tab.DockContent.Close();
                        return;
                    }

                    var closeRect = RectangleToTabArea(tab.CloseButtonRectangle);
                    if (closeRect.Contains(e.Location))
                    {
                        _tabArea.ClickedCloseButton = tab;
                        return;
                    }
                    else
                    {
                        DockPanel.ActiveContent = tab.DockContent;
                        EnsureVisible();

                        _dragTab = tab;

                        return;
                    }
                }
            }

            if (VisibleContent != null)
                DockPanel.ActiveContent = VisibleContent;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            _dragTab = null;

            if (_tabArea.DropdownRectangle.Contains(e.Location))
            {
                if (_tabArea.DropdownHot)
                    _tabArea.ShowMenu(this, new Point(_tabArea.DropdownRectangle.Left, _tabArea.DropdownRectangle.Bottom - 2));

                return;
            }

            if (_tabArea.ClickedCloseButton == null)
                return;

            var closeRect = RectangleToTabArea(_tabArea.ClickedCloseButton.CloseButtonRectangle);
            if (closeRect.Contains(e.Location))
                throw new NotImplementedException("Close");
            //_tabArea.ClickedCloseButton.DockContent.Close();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            foreach (var tab in _tabs.Values)
                tab.Hot = false;

            Invalidate();
        }

        private void TabMenuItem_Select(object sender, EventArgs e)
        {
            var menuItem = sender as MenuStripItem;
            if (menuItem == null)
                return;

            var content = menuItem.Tag as DockContent;
            if (content == null)
                return;

            DockPanel.ActiveContent = content;
        }

        private void DockPanel_ActiveContentChanged(object sender, DockContentEventArgs e)
        {
            if (!_contents.Contains(e.Content))
                return;

            if (e.Content == VisibleContent)
            {
                VisibleContent.Focus();
                return;
            }

            VisibleContent = e.Content;

            foreach (var content in _contents)
                if (content == VisibleContent)
                    content.Show();
                else
                    content.Hide();

            VisibleContent.Focus();

            EnsureVisible();
            Invalidate();
        }

        private void DockContent_DockTextChanged(object sender, EventArgs e)
        {
            BuildTabs();
        }
        #endregion Event Handlers

        #region Drawing
        public void Redraw()
        {
            Invalidate();

            foreach (var content in _contents)
                content.Invalidate();
        }

        protected override void DoPaint(PaintEventArgs e)
        {
            throw new NotImplementedException();
            //var g = e.Graphics;

            //using (var b = new SolidBrush(Colors.GreyBackground))
            //{
            //    g.FillRectangle(b, new );
            //}

            //if (!_tabArea.Visible)
            //    return;

            //using (var b = new SolidBrush(Colors.MediumBackground))
            //{
            //    g.FillRectangle(b, _tabArea.ClientRectangle);
            //}

            //foreach (var tab in _tabs.Values)
            //{
            //    if (DockArea == EDockArea.Document)
            //        PaintDocumentTab(g, tab);
            //    else
            //        PaintToolWindowTab(g, tab);
            //}

            //if (DockArea == EDockArea.Document)
            //{
            //    // Color divider
            //    var isActiveGroup = DockPanel.ActiveGroup == this;
            //    var divColor = isActiveGroup ? Colors.BlueSelection : Colors.GreySelection;
            //    using (var b = new SolidBrush(divColor))
            //    {
            //        var divRect = new Rectangle(_tabArea.ClientRectangle.Left, _tabArea.ClientRectangle.Bottom - 2, _tabArea.ClientRectangle.Width, 2);
            //        g.FillRectangle(b, divRect);
            //    }

            //    // Content dropdown list
            //    var dropdownRect = new Rectangle(_tabArea.DropdownRectangle.Left, _tabArea.DropdownRectangle.Top, _tabArea.DropdownRectangle.Width, _tabArea.DropdownRectangle.Height - 2);

            //    using (var b = new SolidBrush(Colors.MediumBackground))
            //    {
            //        g.FillRectangle(b, dropdownRect);
            //    }

            //    using (var img = DockIcons.arrow)
            //    {
            //        g.DrawImageUnscaled(img, dropdownRect.Left + (dropdownRect.Width / 2) - (img.Width / 2), dropdownRect.Top + (dropdownRect.Height / 2) - (img.Height / 2) + 1);
            //    }
            //}
        }

        private void PaintDocumentTab(NVGcontext g, DockTab tab)
        {
            throw new NotImplementedException();
            //var tabRect = RectangleToTabArea(tab.ClientRectangle);

            //var isVisibleTab = VisibleContent == tab.DockContent;
            //var isActiveGroup = DockPanel.ActiveGroup == this;

            //var bgColor = isVisibleTab ? Colors.BlueSelection : Colors.DarkBackground;

            //if (!isActiveGroup)
            //    bgColor = isVisibleTab ? Colors.GreySelection : Colors.DarkBackground;

            //if (tab.Hot && !isVisibleTab)
            //    bgColor = Colors.MediumBackground;

            //using (var b = new SolidBrush(bgColor))
            //{
            //    g.FillRectangle(b, tabRect);
            //}

            //// Draw separators
            //if (tab.ShowSeparator)
            //{
            //    using (var p = new Pen(Colors.DarkBorder))
            //    {
            //        g.DrawLine(p, tabRect.Right - 1, tabRect.Top, tabRect.Right - 1, tabRect.Bottom);
            //    }
            //}

            //var xOffset = 0;

            //// Draw icon
            //if (tab.DockContent.Icon != null)
            //{
            //    g.DrawImageUnscaled(tab.DockContent.Icon, tabRect.Left + 5, tabRect.Top + 4);
            //    xOffset += tab.DockContent.Icon.Width + 2;
            //}

            //var tabTextFormat = new StringFormat
            //{
            //    Alignment = StringAlignment.Near,
            //    LineAlignment = StringAlignment.Center,
            //    FormatFlags = StringFormatFlags.NoWrap,
            //    Trimming = StringTrimming.EllipsisCharacter
            //};

            //// Draw text
            //var textColor = isVisibleTab ? Colors.LightText : Colors.DisabledText;
            //using (var b = new SolidBrush(textColor))
            //{
            //    var textRect = new Rectangle(tabRect.Left + 5 + xOffset, tabRect.Top, tabRect.Width - tab.CloseButtonRectangle.Width - 7 - 5 - xOffset, tabRect.Height);
            //    g.DrawString(tab.DockContent.DockText, Font, b, textRect, tabTextFormat);
            //}

            //// Close button
            //var img = tab.CloseButtonHot ? DockIcons.inactive_close_selected : DockIcons.inactive_close;

            //if (isVisibleTab)
            //{
            //    if (isActiveGroup)
            //        img = tab.CloseButtonHot ? DockIcons.close_selected : DockIcons.close;
            //    else
            //        img = tab.CloseButtonHot ? DockIcons.close_selected : DockIcons.active_inactive_close;
            //}

            //var closeRect = RectangleToTabArea(tab.CloseButtonRectangle);
            //g.DrawImageUnscaled(img, closeRect.Left, closeRect.Top);
        }

        private void PaintToolWindowTab(NVGcontext g, DockTab tab)
        {
            throw new NotImplementedException();
            //var tabRect = tab.ClientRectangle;

            //var isVisibleTab = VisibleContent == tab.DockContent;

            //var bgColor = isVisibleTab ? Colors.GreyBackground : Colors.DarkBackground;

            //if (tab.Hot && !isVisibleTab)
            //    bgColor = Colors.MediumBackground;

            //using (var b = new SolidBrush(bgColor))
            //{
            //    g.FillRectangle(b, tabRect);
            //}

            //// Draw separators
            //if (tab.ShowSeparator)
            //{
            //    using (var p = new Pen(Colors.DarkBorder))
            //    {
            //        g.DrawLine(p, tabRect.Right - 1, tabRect.Top, tabRect.Right - 1, tabRect.Bottom);
            //    }
            //}

            //var tabTextFormat = new StringFormat
            //{
            //    Alignment = StringAlignment.Near,
            //    LineAlignment = StringAlignment.Center,
            //    FormatFlags = StringFormatFlags.NoWrap,
            //    Trimming = StringTrimming.EllipsisCharacter
            //};

            //var textColor = isVisibleTab ? Colors.BlueHighlight : Colors.DisabledText;
            //using (var b = new SolidBrush(textColor))
            //{
            //    var textRect = new Rectangle(tabRect.Left + 5, tabRect.Top, tabRect.Width - 5, tabRect.Height);
            //    g.DrawString(tab.DockContent.DockText, Font, b, textRect, tabTextFormat);
            //}
        }

        //protected override void OnPaintBackground(PaintEventArgs e)
        //{
        //    // Absorb event
        //}
        #endregion Drawing

        public void AddContent(DockContent dockContent)
        {
            dockContent.DockGroup = this;
            dockContent.Dock = EDocking.Fill;

            dockContent.Order = 0;

            if (_contents.Count > 0)
            {
                var order = -1;
                foreach (var otherContent in _contents)
                {
                    if (otherContent.Order >= order)
                        order = otherContent.Order + 1;
                }

                dockContent.Order = order;
            }

            _contents.Add(dockContent);
            Widgets.Add(dockContent);

            dockContent.DockTextChanged += DockContent_DockTextChanged;

            _tabs.Add(dockContent, new DockTab(dockContent));

            if (VisibleContent == null)
            {
                //dockContent.Visible = true;
                dockContent.Show();
                VisibleContent = dockContent;
            }
            else
            {
                //dockContent.Visible = false;
                dockContent.Hide();
            }

            var menuItem = new MenuStripItem(dockContent.DockText);
            menuItem.Tag = dockContent;
            menuItem.MouseClickEvent += TabMenuItem_Select;
            throw new NotImplementedException("dockContent.Icon");
            //menuItem.Image = dockContent.Icon;
            _tabArea.AddMenuItem(menuItem);

            UpdateTabArea();
        }

        public void RemoveContent(DockContent dockContent)
        {
            dockContent.DockGroup = null;

            var order = dockContent.Order;

            _contents.Remove(dockContent);
            Widgets.Remove(dockContent);

            foreach (var otherContent in _contents)
            {
                if (otherContent.Order > order)
                    otherContent.Order--;
            }

            dockContent.DockTextChanged -= DockContent_DockTextChanged;

            if (_tabs.ContainsKey(dockContent))
                _tabs.Remove(dockContent);

            if (VisibleContent == dockContent)
            {
                VisibleContent = null;

                if (_contents.Count > 0)
                {
                    var newContent = _contents[0];
                    //newContent.Visible = true;
                    newContent.Show();
                    VisibleContent = newContent;
                }
            }

            var menuItem = _tabArea.GetMenuItem(dockContent);

            menuItem.MouseClickEvent -= TabMenuItem_Select;
            _tabArea.RemoveMenuItem(menuItem);

            UpdateTabArea();
        }

        public List<DockContent> GetContents()
        {
            return _contents.OrderBy(c => c.Order).ToList();
        }

        private void UpdateTabArea()
        {
            if (DockArea == EDockArea.Document)
                _tabArea.Visible = (_contents.Count > 0);
            else
                _tabArea.Visible = (_contents.Count > 1);

            var size = 0;

            switch (DockArea)
            {
                case EDockArea.Document:
                    size = _tabArea.Visible ? Consts.DocumentTabAreaSize : 0;
                    Padding = new Spacing(0, size, 0, 0);
                    _tabArea.ClientRectangle = new Rectangle(Padding.Left, 0, this.Width - Padding.Horizontal, size);
                    break;
                case EDockArea.Left:
                case EDockArea.Right:
                    size = _tabArea.Visible ? Consts.ToolWindowTabAreaSize : 0;
                    Padding = new Spacing(0, 0, 0, size);
                    _tabArea.ClientRectangle = new Rectangle(Padding.Left, this.Bottom - size, this.Width - Padding.Horizontal, size);
                    break;
                case EDockArea.Bottom:
                    size = _tabArea.Visible ? Consts.ToolWindowTabAreaSize : 0;
                    Padding = new Spacing(1, 0, 0, size);
                    _tabArea.ClientRectangle = new Rectangle(Padding.Left, this.Bottom - size, this.Width - Padding.Horizontal, size);
                    break;
            }

            if (DockArea == EDockArea.Document)
            {
                var dropdownSize = Consts.DocumentTabAreaSize;
                _tabArea.DropdownRectangle = new Rectangle(_tabArea.ClientRectangle.Right - dropdownSize, 0, dropdownSize, dropdownSize);
            }

            BuildTabs();

            EnsureVisible();
        }

        private void BuildTabs()
        {
            if (!_tabArea.Visible)
                return;

            SuspendLayout();

            var closeButtonSize = 7;//-> DockIcons.close.Width; -> 7x7

            // Calculate areas of all tabs
            var totalSize = 0;

            var orderedContent = _contents.OrderBy(c => c.Order);

            foreach (var content in orderedContent)
            {
                int width;

                var tab = _tabs[content];

                throw new NotImplementedException("DockGroup.BuildTabs() g = CreateGraphics");
                //using (var g = CreateGraphics())
                {
                    //width = tab.CalculateWidth(g, Font);
                }

                // Add additional width for document tab items
                if (DockArea == EDockArea.Document)
                {
                    width += 5;
                    width += closeButtonSize;

                    if (tab.DockContent.Icon != null)
                        width += tab.DockContent.Icon.Width + 5;
                }

                // Show separator on all tabs for now
                tab.ShowSeparator = true;
                width += 1;

                var y = DockArea == EDockArea.Document ? 0 : this.Height - Consts.ToolWindowTabAreaSize;
                var height = DockArea == EDockArea.Document ? Consts.DocumentTabAreaSize : Consts.ToolWindowTabAreaSize;

                var tabRect = new Rectangle(_tabArea.ClientRectangle.Left + totalSize, y, width, height);
                tab.ClientRectangle = tabRect;

                totalSize += width;
            }

            // Cap the size if too large for the tab area
            if (DockArea != EDockArea.Document)
            {
                if (totalSize > _tabArea.ClientRectangle.Width)
                {
                    var difference = totalSize - _tabArea.ClientRectangle.Width;

                    // No matter what, we want to slice off the 1 pixel separator from the final tab.
                    var lastTab = _tabs[orderedContent.Last()];
                    var tabRect = lastTab.ClientRectangle;
                    lastTab.ClientRectangle = new Rectangle(tabRect.Left, tabRect.Top, tabRect.Width - 1, tabRect.Height);
                    lastTab.ShowSeparator = false;

                    var differenceMadeUp = 1;

                    // Loop through and progressively resize the larger tabs until the total size fits within the tab area.
                    while (differenceMadeUp < difference)
                    {
                        var largest = _tabs.Values.OrderByDescending(tab => tab.ClientRectangle.Width)
                                                                     .First()
                                                                     .ClientRectangle.Width;

                        foreach (var content in orderedContent)
                        {
                            var tab = _tabs[content];

                            // Check if previous iteration of loop met the difference
                            if (differenceMadeUp >= difference)
                                break;

                            if (tab.ClientRectangle.Width >= largest)
                            {
                                var rect = tab.ClientRectangle;
                                tab.ClientRectangle = new Rectangle(rect.Left, rect.Top, rect.Width - 1, rect.Height);
                                differenceMadeUp += 1;
                            }
                        }
                    }

                    // After resizing the tabs reposition them accordingly.
                    var xOffset = 0;
                    foreach (var content in orderedContent)
                    {
                        var tab = _tabs[content];

                        var rect = tab.ClientRectangle;
                        tab.ClientRectangle = new Rectangle(_tabArea.ClientRectangle.Left + xOffset, rect.Top, rect.Width, rect.Height);

                        xOffset += rect.Width;
                    }
                }
            }

            // Build close button rectangles
            if (DockArea == EDockArea.Document)
            {
                foreach (var content in orderedContent)
                {
                    var tab = _tabs[content];
                    var closeRect = new Rectangle(tab.ClientRectangle.Right - 7 - closeButtonSize - 1,
                                                  tab.ClientRectangle.Top + (tab.ClientRectangle.Height / 2) - (closeButtonSize / 2) - 1,
                                                  closeButtonSize, closeButtonSize);
                    tab.CloseButtonRectangle = closeRect;
                }
            }

            // Update the tab area with the new total tab width
            totalSize = 0;
            foreach (var content in orderedContent)
            {
                var tab = _tabs[content];
                totalSize += tab.ClientRectangle.Width;
            }

            _tabArea.TotalTabSize = totalSize;

            ResumeLayout();

            Invalidate();
        }

        public void EnsureVisible()
        {
            if (DockArea != EDockArea.Document)
                return;

            if (VisibleContent == null)
                return;

            //var width = ClientRectangle.Width - Padding.Horizontal - _tabArea.DropdownRectangle.Width;
            var width = this.Width - Padding.Horizontal - _tabArea.DropdownRectangle.Width;
            var offsetArea = new Rectangle(Padding.Left, 0, width, 0);

            var tab = _tabs[VisibleContent];

            if (tab.ClientRectangle.IsEmpty)
                return;

            if (RectangleToTabArea(tab.ClientRectangle).Left < offsetArea.Left)
                _tabArea.Offset = tab.ClientRectangle.Left;
            else if (RectangleToTabArea(tab.ClientRectangle).Right > offsetArea.Right)
                _tabArea.Offset = tab.ClientRectangle.Right - width;

            if (_tabArea.TotalTabSize < offsetArea.Width)
                _tabArea.Offset = 0;

            if (_tabArea.TotalTabSize > offsetArea.Width)
            {
                var orderedContent = _contents.OrderBy(x => x.Order);
                var lastTab = _tabs[orderedContent.Last()];
                if (lastTab != null)
                {
                    if (RectangleToTabArea(lastTab.ClientRectangle).Right < offsetArea.Right)
                        _tabArea.Offset = lastTab.ClientRectangle.Right - width;
                }
            }

            Invalidate();
        }

        public void SetVisibleContent(DockContent content)
        {
            if (!_contents.Contains(content))
                return;

            if (VisibleContent != content)
            {
                VisibleContent = content;
                //content.Visible = true;
                content.Show();

                foreach (var otherContent in _contents)
                {
                    if (otherContent != content)
                        //otherContent.Visible = false;
                        otherContent.Hide();
                }

                Invalidate();
            }
        }

        private Point PointToTabArea(Point point)
        {
            return new Point(point.X - _tabArea.Offset, point.Y);
        }

        private Rectangle RectangleToTabArea(Rectangle rectangle)
        {
            return new Rectangle(PointToTabArea(rectangle.Location), rectangle.Size);
        }
    }
}
