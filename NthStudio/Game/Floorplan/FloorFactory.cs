

namespace NthStudio.Game.Procedural
{
    using NthDimension.Procedural;
    using NthDimension.Procedural.Room;

    public class FloorFactory
    {
        private FloorGenerator floorGen;
        /////<summary>Instance of furniture layout algorithm solver</summary>
        //private Furnisher furnisher;

        /////<summary>Placement information for fittings</summary>
        //private FittingPlacement[] fittingPlacements;

        public FloorFactory()
        {
            floorGen = new FloorGenerator(30,          // Width
                                       30,          // Length
                                       800,         // Max Room Count
                                       15,          // Max Room Count per Group
                                       3,           // Max Room Size
                                       15,          // Max Large Rooms
                                       4,           // Blueprint Sections (Room Types)
                                       4,           // Footprint length (?)
                                       4,           // Footprint width (?)
                                       false);      // Paused (?)

            floorGen.GenerateFloorPlan();
            floorGen.RegroupSpanningGroups();
            floorGen.AssignDoorways();

            int roomCount       = floorGen.RoomSizes.Count;
            int corridorCount   = floorGen.CorridorGroups.Count;

            NthDimension.Rendering.Utilities.ConsoleUtil.log(string.Format("(+) Floor rooms [{0}] corridors [{1}]", roomCount, corridorCount));

            int idx             = 0;

            System.Text.StringBuilder sb = new System.Text.StringBuilder(string.Empty, floorGen.Width * floorGen.Length);

            //for(int x = 0; x < floorGen.Width; x++)
            //    for(int y = 0; y < floorGen.Length; y++)
            //    {
            //        //if(floorGen.FloorGrid[x,y].)
            //    }

            foreach (var room in floorGen.Rooms)
            {
                NthDimension.Rendering.Utilities.ConsoleUtil.log(string.Format("Room {0} \tRoom Segments {1} Type {2} Index {3}",
                                                                    room.Key,
                                                                    room.Value.RoomSegments.Count,
                                                                    room.Value.GetType().ToString(),
                                                                    idx++));
            }



            foreach (var tile in floorGen.FloorGrid)
            {
                               
                //foreach(var door in tile.Doors)
                //{

                //}

                //throw new System.NotImplementedException();
                if(null != tile)
                    NthDimension.Rendering.Utilities.ConsoleUtil.log(string.Format("Id {3} \tGroup {0} Room {1} Corridor {2}", 
                                                                    tile.GroupID, 
                                                                    tile.RoomID, 
                                                                    tile.AdjacentCorridorID,
                                                                    idx++));
            }
        }

        public FloorFactory(string furnisherXmlFile = "FittingDatabase.xml") : this()
        {
            //// Load fitting semantics from XML file
            //furnisher = new Furnisher(furnisherXmlFile);
        }

        /*
                private void DrawDoors(int tileSize, int hallwaySize)
        {
            // Create a path for drawing the hallways
            Path doorsPath = new Path();
            doorsPath.Stroke = Brushes.Black;
            doorsPath.StrokeThickness = 1;
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = Colors.Black;
            doorsPath.Fill = mySolidColorBrush;

            // Use a composite geometry for adding the doors to
            CombinedGeometry doorsGeometryGroup = new CombinedGeometry();
            doorsGeometryGroup.GeometryCombineMode = GeometryCombineMode.Union;
            doorsGeometryGroup.Geometry1 = null;
            doorsGeometryGroup.Geometry2 = null;

            for (int top = 0; top < fpHeight; top++)
            {
                for (int left = 0; left < fpWidth; left++)
                {
                    if (fp.floorGrid[top, left] != null)
                    {
                        FloorSegment thisSegment = fp.floorGrid[top, left];
                        double doorX = 0;
                        double doorY = 0;
                        double doorWidth = 0;
                        double doorHeight = 0;
                        // position the doors differently depending on weather or not there's a hallway.  Math, bitches.
                        foreach (Direction nextDoorDir in thisSegment.Doors)
                        {
                            switch (nextDoorDir)
                            {
                                case Direction.North:
                                    FloorSegment northSegment = fp.IsInBounds(top - 1, left) ? fp.floorGrid[top - 1, left] : null;
                                    doorX = (left * tileSize) + (tileSize / 2) - (hallwaySize / 2);
                                    doorY = northSegment != null ? northSegment.GroupID != thisSegment.GroupID ? (top * tileSize) + (hallwaySize / 4) : (top * tileSize) - (hallwaySize / 4) : (top * tileSize) - (hallwaySize / 4);
                                    doorWidth = hallwaySize;
                                    doorHeight = hallwaySize / 2;
                                    break;
                                case Direction.South:
                                    FloorSegment southSegment = fp.IsInBounds(top + 1, left) ? fp.floorGrid[top + 1, left] : null;
                                    doorX = (left * tileSize) + (tileSize / 2) - (hallwaySize / 2);
                                    doorY = southSegment != null ? southSegment.GroupID != thisSegment.GroupID ? ((top * tileSize) + tileSize) - ((hallwaySize / 4) * 3) : ((top * tileSize) + tileSize) - (hallwaySize / 4) : ((top * tileSize) + tileSize) - (hallwaySize / 4);
                                    doorWidth = hallwaySize;
                                    doorHeight = hallwaySize / 2;
                                    break;
                                case Direction.East:
                                    FloorSegment eastSegment = fp.IsInBounds(top, left + 1) ? fp.floorGrid[top, left + 1] : null;
                                    doorX = eastSegment != null ? eastSegment.GroupID != thisSegment.GroupID ? ((left * tileSize) + tileSize) - ((hallwaySize / 4) * 3) : ((left * tileSize) + tileSize) - (hallwaySize / 4) : ((left * tileSize) + tileSize) - (hallwaySize / 4);
                                    doorY = (top * tileSize) + (tileSize / 2) - (hallwaySize / 2);
                                    doorWidth = hallwaySize / 2;
                                    doorHeight = hallwaySize;
                                    break;
                                case Direction.West:
                                    FloorSegment westSegment = fp.IsInBounds(top, left - 1) ? fp.floorGrid[top, left - 1] : null;
                                    doorX = westSegment != null ? westSegment.GroupID != thisSegment.GroupID ? (left * tileSize) + (hallwaySize / 4) : (left * tileSize) - (hallwaySize / 4) : (left * tileSize) - (hallwaySize / 4);
                                    doorY = (top * tileSize) + (tileSize / 2) - (hallwaySize / 2);
                                    doorWidth = hallwaySize / 2;
                                    doorHeight = hallwaySize;
                                    break;
                            }
                            RectangleGeometry newDoorGeometry = new RectangleGeometry(new Rect(doorX, doorY, doorWidth, doorHeight));
                            doorsGeometryGroup = new CombinedGeometry()
                            {
                                Geometry1 = doorsGeometryGroup,
                                Geometry2 = newDoorGeometry
                            };
                        }
                    }
                }
            }

            doorsPath.Data = doorsGeometryGroup;
            RenderOptions.SetEdgeMode(doorsPath, EdgeMode.Aliased);
            MainCanvas.Children.Add(doorsPath);
        }

        private void DrawHallways(int tileSize, int hallwaySize)
        {
            // Create a path for drawing the hallways
            Path hallwayPath = new Path();
            hallwayPath.Stroke = Brushes.Black;
            hallwayPath.StrokeThickness = 1;
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = Colors.White;
            hallwayPath.Fill = mySolidColorBrush;

            // Use a composite geometry for adding the hallways to
            CombinedGeometry hallwayGeometryGroup = new CombinedGeometry();
            hallwayGeometryGroup.GeometryCombineMode = GeometryCombineMode.Union;
            hallwayGeometryGroup.Geometry1 = null;
            hallwayGeometryGroup.Geometry2 = null;

            for (int top = 0; top < fpHeight; top++)
            {
                for (int left = 0; left < fpWidth; left++)
                {
                    if (fp.floorGrid[top, left] != null)
                    {
                        FloorSegment thisSegment = fp.floorGrid[top, left];

                        // If the segment above is in bounds and from a different group, we draw a hallway.
                        // Otherwise, if it's a different room of the same group, we draw a wall.

                        FloorSegment northSegment = fp.IsInBounds(top - 1, left) ? fp.floorGrid[top - 1, left] : null;
                        if (northSegment != null)
                        {
                            if (northSegment.GroupID != thisSegment.GroupID)
                            {
                                RectangleGeometry newHallwayGeometry = new RectangleGeometry(
                                    new Rect(
                                        (left * tileSize) - (hallwaySize / 2),
                                        (top * tileSize) - (hallwaySize / 2),
                                        tileSize + hallwaySize,
                                        hallwaySize));
                                hallwayGeometryGroup = new CombinedGeometry()
                                {
                                    Geometry1 = hallwayGeometryGroup,
                                    Geometry2 = newHallwayGeometry
                                };

                            }
                        }

                        FloorSegment southSegment = fp.IsInBounds(top + 1, left) ? fp.floorGrid[top + 1, left] : null;
                        if (southSegment != null)
                        {
                            if (southSegment.GroupID != thisSegment.GroupID)
                            {
                                RectangleGeometry newHallwayGeometry = new RectangleGeometry(
                                    new Rect(
                                        (left * tileSize) - (hallwaySize / 2),
                                        ((top * tileSize) + tileSize) - (hallwaySize / 2),
                                        tileSize + hallwaySize,
                                        hallwaySize));
                                hallwayGeometryGroup = new CombinedGeometry()
                                {
                                    Geometry1 = hallwayGeometryGroup,
                                    Geometry2 = newHallwayGeometry
                                };
                            }
                        }

                        FloorSegment eastSegment = fp.IsInBounds(top, left + 1) ? fp.floorGrid[top, left + 1] : null;
                        if (eastSegment != null)
                        {
                            if (eastSegment.GroupID != thisSegment.GroupID)
                            {
                                RectangleGeometry newHallwayGeometry = new RectangleGeometry(
                                    new Rect(
                                        ((left * tileSize) + tileSize) - (hallwaySize / 2),
                                        (top * tileSize) - (hallwaySize / 2),
                                        hallwaySize,
                                        tileSize + hallwaySize));
                                hallwayGeometryGroup = new CombinedGeometry()
                                {
                                    Geometry1 = hallwayGeometryGroup,
                                    Geometry2 = newHallwayGeometry
                                };
                            }
                        }

                        FloorSegment westSegment = fp.IsInBounds(top, left - 1) ? fp.floorGrid[top, left - 1] : null;
                        if (westSegment != null)
                        {
                            if (westSegment.GroupID != thisSegment.GroupID)
                            {
                                RectangleGeometry newHallwayGeometry = new RectangleGeometry(
                                    new Rect(
                                        (left * tileSize) - (hallwaySize / 2),
                                        (top * tileSize) - (hallwaySize / 2),
                                        hallwaySize,
                                        tileSize + hallwaySize));
                                hallwayGeometryGroup = new CombinedGeometry()
                                {
                                    Geometry1 = hallwayGeometryGroup,
                                    Geometry2 = newHallwayGeometry
                                };
                            }
                        }
                    }
                }
            }
            hallwayPath.Data = hallwayGeometryGroup;
            RenderOptions.SetEdgeMode(hallwayPath, EdgeMode.Aliased);
            MainCanvas.Children.Add(hallwayPath);

            // Draw the remaining eligible segments
            if (fp.pause)
            {
                foreach (DirectionalSegment ds in fp.eligibleFloorSegments)
                {
                    Rectangle newRect = new Rectangle()
                    {
                        Width = tileSize,
                        Height = tileSize,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1
                    };
                    MainCanvas.Children.Add(newRect);
                    Canvas.SetLeft(newRect, ds.Left * tileSize);
                    Canvas.SetTop(newRect, ds.Top * tileSize);
                }
            }
            // Signal the algorithm to continue
            fp.waitEvent.Set();
            labelCountRoom.Content = fp.currRoomID.ToString();
        }

        private void DrawData(int tileSize)
        {
            // Draw the floorplan
            MainCanvas.Children.Clear();
            for (int top = 0; top < fpHeight; top++)
            {
                for (int left = 0; left < fpWidth; left++)
                {
                    if (fp.floorGrid[top, left] != null)
                    {
                        FloorSegment thisSegment = fp.floorGrid[top, left];
                        if (!colorDict.ContainsKey(thisSegment.GroupID))
                        {
                            colorDict.Add(thisSegment.GroupID, PickBrush());
                        }
                        Rectangle newRect = new Rectangle()
                        {
                            Width = tileSize,
                            Height = tileSize,
                            Fill = colorDict[thisSegment.GroupID]
                        };
                        RenderOptions.SetEdgeMode(newRect, EdgeMode.Aliased);

                        // Draw the lines between squares if they belong to different rooms
                        FloorSegment northSegment = fp.IsInBounds(top - 1, left) ? fp.floorGrid[top - 1, left] : null;
                        bool drawNorthLine = northSegment == null ? true : (northSegment.RoomID != thisSegment.RoomID) ? true : false;
                        if (drawNorthLine)
                        {
                            Line newLine = new Line()
                            {
                                Stroke = Brushes.Black,
                                StrokeThickness = 2,
                                X1 = left * tileSize,
                                Y1 = top * tileSize,
                                X2 = (left * tileSize) + tileSize,
                                Y2 = top * tileSize
                            };
                            MainCanvas.Children.Add(newLine);
                        }

                        FloorSegment westSegment = fp.IsInBounds(top, left - 1) ? fp.floorGrid[top, left - 1] : null;
                        bool drawWestLine = westSegment == null ? true : (westSegment.RoomID != thisSegment.RoomID) ? true : false;
                        if (drawWestLine)
                        {
                            Line newLine = new Line()
                            {
                                Stroke = Brushes.Black,
                                StrokeThickness = 2,
                                X1 = left * tileSize,
                                Y1 = top * tileSize,
                                X2 = left * tileSize,
                                Y2 = (top * tileSize) + tileSize
                            };
                            MainCanvas.Children.Add(newLine);
                        }

                        FloorSegment eastSegment = fp.IsInBounds(top, left + 1) ? fp.floorGrid[top, left + 1] : null;
                        bool drawEastLine = eastSegment == null ? true : (eastSegment.RoomID != thisSegment.RoomID) ? true : false;
                        if (drawEastLine)
                        {
                            Line newLine = new Line()
                            {
                                Stroke = Brushes.Black,
                                StrokeThickness = 2,
                                X1 = (left * tileSize) + tileSize,
                                Y1 = top * tileSize,
                                X2 = (left * tileSize) + tileSize,
                                Y2 = (top * tileSize) + tileSize
                            };
                            MainCanvas.Children.Add(newLine);
                        }

                        FloorSegment southSegment = fp.IsInBounds(top + 1, left) ? fp.floorGrid[top + 1, left] : null;
                        bool drawSouthLine = southSegment == null ? true : (southSegment.RoomID != thisSegment.RoomID) ? true : false;
                        if (drawSouthLine)
                        {
                            Line newLine = new Line()
                            {
                                Stroke = Brushes.Black,
                                StrokeThickness = 2,
                                X1 = left * tileSize,
                                Y1 = (top * tileSize) + tileSize,
                                X2 = (left * tileSize) + tileSize,
                                Y2 = (top * tileSize) + tileSize
                            };
                            MainCanvas.Children.Add(newLine);
                        }
                        MainCanvas.Children.Add(newRect);
                        Canvas.SetLeft(newRect, left * tileSize);
                        Canvas.SetTop(newRect, top * tileSize);

                        if (drawRoomNumbers)
                        {
                            TextBlock tbRoomNum = new TextBlock();
                            tbRoomNum.Inlines.Add(new Run(fp.floorGrid[top, left].RoomID.ToString()));
                            RenderOptions.SetEdgeMode(tbRoomNum, EdgeMode.Aliased);
                            MainCanvas.Children.Add(tbRoomNum);
                            Canvas.SetLeft(tbRoomNum, left * tileSize + 6);
                            Canvas.SetTop(tbRoomNum, top * tileSize + 6);
                        }
                        else if (drawGroupNumbers)
                        {
                            TextBlock tbGroupNum = new TextBlock();
                            tbGroupNum.Inlines.Add(new Run(fp.floorGrid[top, left].GroupID.ToString()));
                            RenderOptions.SetEdgeMode(tbGroupNum, EdgeMode.Aliased);
                            MainCanvas.Children.Add(tbGroupNum);
                            Canvas.SetLeft(tbGroupNum, left * tileSize + 6);
                            Canvas.SetTop(tbGroupNum, top * tileSize + 6);
                        }
                        else if (drawCorridorGroups)
                        {
                            TextBlock tbGroupNum = new TextBlock();
                            HashSet<string> groupValues = fp.floorGrid[top, left].ConnectedCorridorIDs;
                            if(groupValues.Count > 0)
                            {
                                string groupCSV = String.Join(",", groupValues.Select(x => x.ToString()).ToArray());
                                tbGroupNum.Inlines.Add(new Run(groupCSV));
                                RenderOptions.SetEdgeMode(tbGroupNum, EdgeMode.Aliased);
                                MainCanvas.Children.Add(tbGroupNum);
                                Canvas.SetLeft(tbGroupNum, left * tileSize + 6);
                                Canvas.SetTop(tbGroupNum, top * tileSize + 6);
                            }
                        }
                    }
                }
            }

            // Draw the remaining eligible segments
            if (fp.pause)
            {
                foreach (DirectionalSegment ds in fp.eligibleFloorSegments)
                {
                    Rectangle newRect = new Rectangle()
                    {
                        Width = tileSize,
                        Height = tileSize,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1
                    };
                    MainCanvas.Children.Add(newRect);
                    Canvas.SetLeft(newRect, ds.Left * tileSize);
                    Canvas.SetTop(newRect, ds.Top * tileSize);
                }
            }
            // Signal the algorithm to continue
            fp.waitEvent.Set();
            labelCountRoom.Content = fp.currRoomID.ToString();
        } 
          
        */
    }
}
