using NthDimension.Rasterizer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms.Docking
{
    public class DockPanel : Widget
    {
        public event EventHandler<DockContentEventArgs> ActiveContentChanged;
        public event EventHandler<DockContentEventArgs> ContentAdded;
        public event EventHandler<DockContentEventArgs> ContentRemoved;

        private List<DockContent>                       _contents;
        private Dictionary<EDockArea, DockRegion>       _regions;

        private DockContent                             _activeContent;
        private bool                                    _switchingContent = false;

        #region Properties
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DockContent ActiveContent
        {
            get { return _activeContent; }
            set
            {
                // Don't let content visibility changes re-trigger event
                if (_switchingContent)
                    return;

                _switchingContent = true;

                _activeContent = value;

                ActiveGroup = _activeContent.DockGroup;
                ActiveRegion = ActiveGroup.DockRegion;

                foreach (var region in _regions.Values)
                    region.Invalidate();//.Redraw();

                if (ActiveContentChanged != null)
                    ActiveContentChanged(this, new DockContentEventArgs(_activeContent));

                _switchingContent = false;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DockRegion ActiveRegion { get; internal set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DockGroup ActiveGroup { get; internal set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DockContent ActiveDocument
        {
            get
            {
                return _regions[EDockArea.Document].ActiveDocument;
            }
        }

        //[Browsable(false)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //public DockContentDragFilter DockContentDragFilter { get; private set; }

        //[Browsable(false)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //public DockResizeFilter DockResizeFilter { get; private set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<DockSplitter> Splitters { get; private set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MouseButton MouseButtonState
        {
            get
            {
                //var buttonState = MouseButton; // TODO:: Get Mouse State
                //return buttonState;

                throw new NotImplementedException();

                return NthDimension.MouseButton.None;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Dictionary<EDockArea, DockRegion> Regions
        {
            get
            {
                return _regions;
            }
        }
        #endregion

        public DockPanel()
        {
            Splitters               = new List<DockSplitter>();

            throw new NotImplementedException();

            //DockContentDragFilter   = new DockContentDragFilter(this);
            //DockResizeFilter        = new DockResizeFilter(this);

            _regions                = new Dictionary<EDockArea, DockRegion>();
            _contents               = new List<DockContent>();

            BGColor                 = Colors.GreyBackground;

            CreateRegions();
        }



        public void AddContent(DockContent dockContent)
        {
            AddContent(dockContent, null);
        }

        public void AddContent(DockContent dockContent, DockGroup dockGroup)
        {
            if (_contents.Contains(dockContent))
                RemoveContent(dockContent);

            dockContent.DockPanel = this;
            _contents.Add(dockContent);

            if (dockGroup != null)
                dockContent.DockArea = dockGroup.DockArea;

            if (dockContent.DockArea == EDockArea.None)
                dockContent.DockArea = dockContent.DefaultDockArea;

            var region = _regions[dockContent.DockArea];
            region.AddContent(dockContent, dockGroup);

            if (ContentAdded != null)
                ContentAdded(this, new DockContentEventArgs(dockContent));

            //dockContent.Select();
            dockContent.Focus();
        }

        public void InsertContent(DockContent dockContent, DockGroup dockGroup, EDockInsertType insertType)
        {
            if (_contents.Contains(dockContent))
                RemoveContent(dockContent);

            dockContent.DockPanel = this;
            _contents.Add(dockContent);

            dockContent.DockArea = dockGroup.DockArea;

            var region = _regions[dockGroup.DockArea];
            region.InsertContent(dockContent, dockGroup, insertType);

            if (ContentAdded != null)
                ContentAdded(this, new DockContentEventArgs(dockContent));

            //dockContent.Select();
            dockContent.Focus();
        }

        public void RemoveContent(DockContent dockContent)
        {
            if (!_contents.Contains(dockContent))
                return;

            dockContent.DockPanel = null;
            _contents.Remove(dockContent);

            var region = _regions[dockContent.DockArea];
            region.RemoveContent(dockContent);

            if (ContentRemoved != null)
                ContentRemoved(this, new DockContentEventArgs(dockContent));
        }

        public bool ContainsContent(DockContent dockContent)
        {
            return _contents.Contains(dockContent);
        }

        public List<DockContent> GetDocuments()
        {
            return _regions[EDockArea.Document].GetContents();
        }

        private void CreateRegions()
        {
            var documentRegion = new DockRegion(this, EDockArea.Document);
            _regions.Add(EDockArea.Document, documentRegion);

            var leftRegion = new DockRegion(this, EDockArea.Left);
            _regions.Add(EDockArea.Left, leftRegion);

            var rightRegion = new DockRegion(this, EDockArea.Right);
            _regions.Add(EDockArea.Right, rightRegion);

            var bottomRegion = new DockRegion(this, EDockArea.Bottom);
            _regions.Add(EDockArea.Bottom, bottomRegion);

            // Add the regions in this order to force the bottom region to be positioned
            // between the left and right regions properly.
            Widgets.Add(documentRegion);
            Widgets.Add(bottomRegion);
            Widgets.Add(leftRegion);
            Widgets.Add(rightRegion);

            // Create tab index for intuitive tabbing order
            documentRegion.TabIndex = 0;
            rightRegion.TabIndex = 1;
            bottomRegion.TabIndex = 2;
            leftRegion.TabIndex = 3;
        }

        public void DragContent(DockContent content)
        {
            throw new NotImplementedException();
            //DockContentDragFilter.StartDrag(content);
        }

        #region Serialization
        public DockPanelState GetDockPanelState()
        {
            var state = new DockPanelState();

            state.Regions.Add(new DockRegionState(EDockArea.Document));
            state.Regions.Add(new DockRegionState(EDockArea.Left, _regions[EDockArea.Left].Size));
            state.Regions.Add(new DockRegionState(EDockArea.Right, _regions[EDockArea.Right].Size));
            state.Regions.Add(new DockRegionState(EDockArea.Bottom, _regions[EDockArea.Bottom].Size));

            var _groupStates = new Dictionary<DockGroup, DockGroupState>();

            var orderedContent = _contents.OrderBy(c => c.Order);
            foreach (var content in orderedContent)
            {
                foreach (var region in state.Regions)
                {
                    if (region.Area == content.DockArea)
                    {
                        DockGroupState groupState;

                        if (_groupStates.ContainsKey(content.DockGroup))
                        {
                            groupState = _groupStates[content.DockGroup];
                        }
                        else
                        {
                            groupState = new DockGroupState();
                            region.Groups.Add(groupState);
                            _groupStates.Add(content.DockGroup, groupState);
                        }

                        groupState.Contents.Add(content.SerializationKey);
                    }
                }
            }

            return state;
        }

        public void RestoreDockPanelState(DockPanelState state, Func<string, DockContent> getContentBySerializationKey)
        {
            foreach (var region in state.Regions)
            {
                switch (region.Area)
                {
                    case EDockArea.Left:
                        _regions[EDockArea.Left].Size = region.Size;
                        break;
                    case EDockArea.Right:
                        _regions[EDockArea.Right].Size = region.Size;
                        break;
                    case EDockArea.Bottom:
                        _regions[EDockArea.Bottom].Size = region.Size;
                        break;
                }

                foreach (var group in region.Groups)
                {
                    DockContent previousContent = null;

                    foreach (var contentKey in group.Contents)
                    {
                        var content = getContentBySerializationKey(contentKey);

                        if (content == null)
                            continue;

                        if (previousContent == null)
                            AddContent(content);
                        else
                            AddContent(content, previousContent.DockGroup);

                        previousContent = content;
                    }
                }
            }
        }
        #endregion Serialization
    }
}
