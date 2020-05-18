using NthDimension.Forms.Layout;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;


namespace NthDimension.Forms.Docking
{
    public class DockRegion : Panel
    {
        private List<DockGroup> _groups;

        //private Form                    _parentForm;
        private DockSplitter        _splitter;

        #region Properties
        public DockPanel DockPanel { get; private set; }

        public EDockArea DockArea { get; private set; }

        public DockContent ActiveDocument
        {
            get
            {
                if (DockArea != EDockArea.Document || _groups.Count == 0)
                    return null;

                return _groups[0].VisibleContent;
            }
        }

        public List<DockGroup> Groups
        {
            get
            {
                return _groups.ToList();
            }
        }
        #endregion Properties

        public DockRegion(DockPanel dockPanel, EDockArea dockArea)
        {
            _groups = new List<DockGroup>();

            DockPanel = dockPanel;
            DockArea = dockArea;

            BuildProperties();
        }


        internal void AddContent(DockContent dockContent)
        {
            AddContent(dockContent, null);
        }

        internal void AddContent(DockContent dockContent, DockGroup dockGroup)
        {
            // If no existing group is specified then create a new one
            if (dockGroup == null)
            {
                // If this is the document region, then default to first group if it exists
                if (DockArea == EDockArea.Document && _groups.Count > 0)
                    dockGroup = _groups[0];
                else
                    dockGroup = CreateGroup();
            }

            dockContent.DockRegion = this;
            dockGroup.AddContent(dockContent);

            if (!IsVisible)
            {
                //Visible = true;
                Show();
                CreateSplitter();
            }

            PositionGroups();
        }

        internal void InsertContent(DockContent dockContent, DockGroup dockGroup, EDockInsertType insertType)
        {
            var order = dockGroup.Order;

            if (insertType == EDockInsertType.After)
                order++;

            var newGroup = InsertGroup(order);

            dockContent.DockRegion = this;
            newGroup.AddContent(dockContent);

            if (!IsVisible)
            {
                //Visible = true;
                Show();
                CreateSplitter();
            }

            PositionGroups();
        }

        internal void RemoveContent(DockContent dockContent)
        {
            dockContent.DockRegion = null;

            var group = dockContent.DockGroup;
            group.RemoveContent(dockContent);

            dockContent.DockArea = EDockArea.None;

            // If that was the final content in the group then remove the group
            if (group.ContentCount == 0)
                RemoveGroup(group);

            // If we just removed the final group, and this isn't the document region, then hide
            if (_groups.Count == 0 && DockArea != EDockArea.Document)
            {
                //Visible = false;
                Hide();
                RemoveSplitter();
            }

            PositionGroups();
        }

        public List<DockContent> GetContents()
        {
            var result = new List<DockContent>();

            foreach (var group in _groups)
                result.AddRange(group.GetContents());

            return result;
        }

        private DockGroup CreateGroup()
        {
            var order = 0;

            if (_groups.Count >= 1)
            {
                order = -1;
                foreach (var group in _groups)
                {
                    if (group.Order >= order)
                        order = group.Order + 1;
                }
            }

            var newGroup = new DockGroup(DockPanel, this, order);
            _groups.Add(newGroup);
            Widgets.Add(newGroup);

            return newGroup;
        }

        private DockGroup InsertGroup(int order)
        {
            foreach (var group in _groups)
            {
                if (group.Order >= order)
                    group.Order++;
            }

            var newGroup = new DockGroup(DockPanel, this, order);
            _groups.Add(newGroup);
            Widgets.Add(newGroup);

            return newGroup;
        }

        private void RemoveGroup(DockGroup group)
        {
            var lastOrder = group.Order;

            _groups.Remove(group);
            Widgets.Remove(group);

            foreach (var otherGroup in _groups)
            {
                if (otherGroup.Order > lastOrder)
                    otherGroup.Order--;
            }
        }

        private void PositionGroups()
        {
            EDocking dockStyle;

            switch (DockArea)
            {
                default:
                case EDockArea.Document:
                    dockStyle = EDocking.Fill;
                    break;
                case EDockArea.Left:
                case EDockArea.Right:
                    dockStyle = EDocking.Top;
                    break;
                case EDockArea.Bottom:
                    dockStyle = EDocking.Left;
                    break;
            }

            if (_groups.Count == 1)
            {
                _groups[0].Dock = EDocking.Fill;
                return;
            }

            if (_groups.Count > 1)
            {
                var lastGroup = _groups.OrderByDescending(g => g.Order).First();

                foreach (var group in _groups.OrderByDescending(g => g.Order))
                {
                    throw new System.NotImplementedException();
                    //group.SendToBack();

                    if (group.Order == lastGroup.Order)
                        group.Dock = EDocking.Fill;
                    else
                        group.Dock = dockStyle;
                }

                SizeGroups();
            }
        }

        private void SizeGroups()
        {
            if (_groups.Count <= 1)
                return;

            var size = new Size(0, 0);

            switch (DockArea)
            {
                default:
                case EDockArea.Document:
                    return;
                case EDockArea.Left:
                case EDockArea.Right:
                    size = new Size(Width, Height / _groups.Count);
                    break;
                case EDockArea.Bottom:
                    size = new Size(Width / _groups.Count, Height);
                    break;
            }

            foreach (var group in _groups)
                group.Size = size;
        }

        private void BuildProperties()
        {
            MinimumSize = new Size(50, 50);

            switch (DockArea)
            {
                default:
                case EDockArea.Document:
                    Dock = EDocking.Fill;
                    Padding = new Spacing(0, 1, 0, 0);
                    break;
                case EDockArea.Left:
                    Dock = EDocking.Left;
                    Padding = new Spacing(0, 0, 1, 0);
                    //IsVisible = false;
                    Hide();
                    break;
                case EDockArea.Right:
                    Dock = EDocking.Right;
                    Padding = new Spacing(1, 0, 0, 0);
                    //Visible = false;
                    Hide();
                    break;
                case EDockArea.Bottom:
                    Dock = EDocking.Bottom;
                    Padding = new Spacing(0, 0, 0, 0);
                    //Visible = false;
                    Hide();
                    break;
            }
        }

        private void CreateSplitter()
        {
            if (_splitter != null && DockPanel.Splitters.Contains(_splitter))
                DockPanel.Splitters.Remove(_splitter);

            switch (DockArea)
            {
                case EDockArea.Left:
                    _splitter = new DockSplitter(DockPanel, this, EDockSplitterType.Right);
                    break;
                case EDockArea.Right:
                    _splitter = new DockSplitter(DockPanel, this, EDockSplitterType.Left);
                    break;
                case EDockArea.Bottom:
                    _splitter = new DockSplitter(DockPanel, this, EDockSplitterType.Top);
                    break;
                default:
                    return;
            }

            DockPanel.Splitters.Add(_splitter);
        }

        private void RemoveSplitter()
        {
            if (DockPanel.Splitters.Contains(_splitter))
                DockPanel.Splitters.Remove(_splitter);
        }

    }
}
