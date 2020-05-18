using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms.Docking
{
    public class DockDropArea
    {
        #region Properties
        internal DockPanel DockPanel { get; private set; }

        internal Rectangle DropArea { get; private set; }

        internal Rectangle HighlightArea { get; private set; }

        internal DockRegion DockRegion { get; private set; }

        internal DockGroup DockGroup { get; private set; }

        internal EDockInsertType InsertType { get; private set; }
        #endregion Properties

        internal DockDropArea(DockPanel dockPanel, DockRegion region)
        {
            DockPanel = dockPanel;
            DockRegion = region;
            InsertType = EDockInsertType.None;

            BuildAreas();
        }

        internal DockDropArea(DockPanel dockPanel, DockGroup group, EDockInsertType insertType)
        {
            DockPanel = dockPanel;
            DockGroup = group;
            InsertType = insertType;

            BuildAreas();
        }

        internal void BuildAreas()
        {
            if (DockRegion != null)
                BuildRegionAreas();
            else if (DockGroup != null)
                BuildGroupAreas();
        }

        private void BuildRegionAreas()
        {
            switch (DockRegion.DockArea)
            {
                case EDockArea.Left:

                    var leftRect = new Rectangle
                    {
                        //X = DockPanel.PointToScreen(Point.Empty).X,
                        //Y = DockPanel.PointToScreen(Point.Empty).Y,
                        X = DockPanel.X,
                        Y = DockPanel.Y,
                        Width = 50,
                        Height = DockPanel.Height
                    };

                    DropArea = leftRect;
                    HighlightArea = leftRect;

                    break;

                case EDockArea.Right:

                    var rightRect = new Rectangle
                    {
                        //X = DockPanel.PointToScreen(Point.Empty).X + DockPanel.Width - 50,
                        //Y = DockPanel.PointToScreen(Point.Empty).Y,
                        X = DockPanel.X + DockPanel.Width - 50,
                        Y = DockPanel.Y,
                        Width = 50,
                        Height = DockPanel.Height
                    };

                    DropArea = rightRect;
                    HighlightArea = rightRect;

                    break;

                case EDockArea.Bottom:

                    //var x = DockPanel.PointToScreen(Point.Empty).X;
                    var x = DockPanel.X;
                    var width = DockPanel.Width;

                    if (DockPanel.Regions[EDockArea.Left].IsVisible)
                    {
                        x += DockPanel.Regions[EDockArea.Left].Width;
                        width -= DockPanel.Regions[EDockArea.Left].Width;
                    }

                    if (DockPanel.Regions[EDockArea.Right].IsVisible)
                    {
                        width -= DockPanel.Regions[EDockArea.Right].Width;
                    }

                    var bottomRect = new Rectangle
                    {
                        X = x,
                        //Y = DockPanel.PointToScreen(Point.Empty).Y + DockPanel.Height - 50,
                        Y = DockPanel.Y + DockPanel.Height - 50,
                        Width = width,
                        Height = 50
                    };

                    DropArea = bottomRect;
                    HighlightArea = bottomRect;

                    break;
            }
        }

        private void BuildGroupAreas()
        {
            switch (InsertType)
            {
                case EDockInsertType.None:
                    var dropRect = new Rectangle
                    {
                        //X = DockGroup.PointToScreen(Point.Empty).X,
                        //Y = DockGroup.PointToScreen(Point.Empty).Y,
                        X = DockGroup.X,
                        Y = DockGroup.Y,
                        Width = DockGroup.Width,
                        Height = DockGroup.Height
                    };

                    DropArea = dropRect;
                    HighlightArea = dropRect;

                    break;

                case EDockInsertType.Before:
                    var beforeDropWidth = DockGroup.Width;
                    var beforeDropHeight = DockGroup.Height;

                    switch (DockGroup.DockArea)
                    {
                        case EDockArea.Left:
                        case EDockArea.Right:
                            beforeDropHeight = DockGroup.Height / 4;
                            break;

                        case EDockArea.Bottom:
                            beforeDropWidth = DockGroup.Width / 4;
                            break;
                    }

                    var beforeDropRect = new Rectangle
                    {
                        //X = DockGroup.PointToScreen(Point.Empty).X,
                        //Y = DockGroup.PointToScreen(Point.Empty).Y,
                        X = DockGroup.X,
                        Y = DockGroup.Y,
                        Width = beforeDropWidth,
                        Height = beforeDropHeight
                    };

                    DropArea = beforeDropRect;
                    HighlightArea = beforeDropRect;

                    break;

                case EDockInsertType.After:
                    //var afterDropX = DockGroup.PointToScreen(Point.Empty).X;
                    //var afterDropY = DockGroup.PointToScreen(Point.Empty).Y;
                    var afterDropX = DockGroup.X;
                    var afterDropY = DockGroup.Y;
                    var afterDropWidth = DockGroup.Width;
                    var afterDropHeight = DockGroup.Height;

                    switch (DockGroup.DockArea)
                    {
                        case EDockArea.Left:
                        case EDockArea.Right:
                            afterDropHeight = DockGroup.Height / 4;
                            //afterDropY = DockGroup.PointToScreen(Point.Empty).Y + DockGroup.Height - afterDropHeight;
                            afterDropY = DockGroup.Y + DockGroup.Height - afterDropHeight;
                            break;

                        case EDockArea.Bottom:
                            afterDropWidth = DockGroup.Width / 4;
                            //afterDropX = DockGroup.PointToScreen(Point.Empty).X + DockGroup.Width - afterDropWidth;
                            afterDropX = DockGroup.X + DockGroup.Width - afterDropWidth;
                            break;
                    }

                    var afterDropRect = new Rectangle
                    {
                        X = afterDropX,
                        Y = afterDropY,
                        Width = afterDropWidth,
                        Height = afterDropHeight
                    };

                    DropArea = afterDropRect;
                    HighlightArea = afterDropRect;

                    break;
            }
        }

    }
}
