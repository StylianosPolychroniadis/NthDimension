using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NthDimension.Procedural.FloorPlan
{
    public enum Direction { None, North, South, East, West };

    public struct DirectionFlags
    {
        public bool north;
        public bool south;
        public bool east;
        public bool west;
    };

    struct EligibleDoor
    {
        public int Top;
        public int Left;
        public int RoomID;
        public Direction doorDirection;
    }

    public struct SegmentPos
    {
        public int Top;
        public int Left;
    }

    public struct CorridorGroup
    {
        public string Name;
        public List<corridorSegment> Segments;
    }

    public struct corridorSegment
    {
        public SegmentPos Segment1;
        public SegmentPos Segment2;
    }

    class FloorSegment
    {
        public int RoomID;
        public int GroupID;
        public string AdjacentCorridorID;
        public HashSet<string> ConnectedCorridorIDs = new HashSet<string>();
        public List<Direction> Doors = new List<Direction>();
    }

    class Room
    {
        public HashSet<SegmentPos> RoomSegments = new HashSet<SegmentPos>();
    }

    class FloorGenerator
    {
        public AutoResetEvent waitEvent = new AutoResetEvent(true);
        public FloorSegment[,] floorGrid;
        public Dictionary<int, int> roomSizeDict = new Dictionary<int, int>();
        public List<CorridorGroup> CorridorGroups;
        public Dictionary<int, Room> RoomDict = new Dictionary<int, Room>();
        public int currRoomID = 0;
        public int spanningRoomCount = 0;
        public Queue<FloorplanDirectionalSegment> eligibleFloorSegments = new Queue<FloorplanDirectionalSegment>();

        public bool pause;
        private int maxRooms;
        private int roomCount;
        private int currGroupRoomCount = 0;
        private int maxRoomsPerGroup = 0;
        private int largeRoomCount = 0;
        private int currGroupID = 0;
        private int maxLargeRooms = 5;
        private int currMaxLargeRooms;
        private int currMaxRoomsPerGroup;
        private int width;
        private int height;
        private int maxRoomSize;
        private int blueprintSections;
        private int blueprintSectionLength;
        private int blueprintSectionWidth;
        private Random rnd = new Random();

        public FloorGenerator(int inwidth, int inheight, int inMaxRooms, int inMaxRoomsPerGroup, int inMaxRoomSize,
            int inMaxLargeRooms, int inBluePrintSections, int inFootprintSectionLength, int inFootprintSectionWidth, bool inpause)
        {
            maxRooms = inMaxRooms;
            width = inwidth;
            height = inheight;
            maxRoomSize = inMaxRoomSize;
            maxRoomsPerGroup = inMaxRoomsPerGroup;
            floorGrid = new FloorSegment[height, width];
            maxLargeRooms = inMaxLargeRooms;
            blueprintSections = inBluePrintSections;
            blueprintSectionLength = inFootprintSectionLength;
            blueprintSectionWidth = inFootprintSectionWidth;
            pause = inpause;
        }

        // Generates the footprint of the floor plan using GenerateRoom and setting the room number to -1
        // Once the footprint is generated, only segments of room number -1 are considered valid
        public void GenerateFloorFootprint(int inMaxFootprintSections)
        {
            int startTop = (height / 3) * 2;
            int startLeft = (width / 3) * 2;
            roomCount = 0;
            eligibleFloorSegments.Enqueue(new FloorplanDirectionalSegment(startTop, startLeft, Direction.North, currGroupID));
            while (eligibleFloorSegments.Count > 0)
            {
                int shuffleAmount = rnd.Next(eligibleFloorSegments.Count);
                for (int i = 0; i <= shuffleAmount; i++) eligibleFloorSegments.Enqueue(eligibleFloorSegments.Dequeue());
                FloorplanDirectionalSegment currSegment = eligibleFloorSegments.Dequeue();
                GenerateRoom(currSegment, true, true, blueprintSectionLength, blueprintSectionWidth);
                if (roomCount >= inMaxFootprintSections) break;
            }
            roomCount = 0;
            currGroupRoomCount = 0;
            largeRoomCount = 0;
            currGroupID = 0;
            eligibleFloorSegments.Clear();
        }

        public void GenerateFloorPlan()
        {
            GenerateFloorFootprint(blueprintSections);
            int startTop = (height / 3) * 2;
            int startLeft = (width / 3) * 2;
            eligibleFloorSegments.Enqueue(new FloorplanDirectionalSegment(startTop, startLeft, Direction.North, currGroupID));
            currMaxLargeRooms = rnd.Next(maxLargeRooms);
            currMaxRoomsPerGroup = rnd.Next(maxRoomsPerGroup) + 1;
            while (eligibleFloorSegments.Count > 0)
            {
                Queue<FloorplanDirectionalSegment> selectedSegments = null;
                bool largeRoom = false;
                if (currGroupRoomCount <= currMaxRoomsPerGroup)
                {
                    selectedSegments = new Queue<FloorplanDirectionalSegment>(eligibleFloorSegments.Where<FloorplanDirectionalSegment>(d => d.GroupID == currGroupID));
                }
                if (selectedSegments == null || selectedSegments.Count == 0)
                {
                    selectedSegments = eligibleFloorSegments;
                    largeRoomCount = 0;
                    currMaxLargeRooms = rnd.Next(maxLargeRooms);
                    currMaxRoomsPerGroup = rnd.Next(maxRoomsPerGroup) + 1;
                }
                else // We're still doing the same group
                {
                    if (largeRoomCount < currMaxLargeRooms) largeRoom = true;
                }
                int shuffleAmount = rnd.Next(selectedSegments.Count);
                for (int i = 0; i <= shuffleAmount; i++) selectedSegments.Enqueue(selectedSegments.Dequeue());
                FloorplanDirectionalSegment currSegment = selectedSegments.Dequeue();
                eligibleFloorSegments = new Queue<FloorplanDirectionalSegment>(eligibleFloorSegments.Where<FloorplanDirectionalSegment>(d => d != currSegment));
                currSegment.GroupID = currGroupID;
                GenerateRoom(currSegment, largeRoom, false, blueprintSectionLength, blueprintSectionWidth);
                if (roomCount >= maxRooms) break;

            }

            //GroupByWall();
            RemoveIslandRooms();

            FindCorridors();
        }

        public bool IsInBounds(int Top, int Left)
        {
            return (Left >= 1 && Left < (width - 1)) && (Top >= 1 && Top < (height - 1));
        }

        public List<Direction> GetOutterWallDirections(int Top, int Left)
        {
            List<Direction> result = new List<Direction>();
            if (IsInBounds(Top - 1, Left)) if (floorGrid[Top - 1, Left] == null) result.Add(Direction.North);
            if (IsInBounds(Top + 1, Left)) if (floorGrid[Top + 1, Left] == null) result.Add(Direction.South);
            if (IsInBounds(Top, Left - 1)) if (floorGrid[Top, Left - 1] == null) result.Add(Direction.West);
            if (IsInBounds(Top, Left + 1)) if (floorGrid[Top, Left + 1] == null) result.Add(Direction.East);
            return result;
        }

        public bool IsValidSegment(int Top, int Left, bool isfootprint)
        {
            bool inBounds = IsInBounds(Top, Left);
            bool notUsed = false;
            bool result = false;

            if (isfootprint)
            {
                notUsed = inBounds ? (floorGrid[Top, Left] == null ? true : false) : false;
                result = inBounds && notUsed;
            }
            else
            {
                if (inBounds && floorGrid[Top, Left] != null)
                {
                    notUsed = floorGrid[Top, Left].GroupID == -1 ? true : false;
                }
                else
                {
                    notUsed = false;
                }
            }
            return inBounds && notUsed;
        }

        private bool IsPartofRoom(int top, int left)
        {
            int roomNumber = IsInBounds(top, left) ? (floorGrid[top, left] != null ? floorGrid[top, left].RoomID : -1) : -1;
            int roomNumberNorth = IsInBounds(top - 1, left) ? (floorGrid[top - 1, left] != null ? floorGrid[top - 1, left].RoomID : -1) : -1;
            int roomNumberSouth = IsInBounds(top + 1, left) ? (floorGrid[top + 1, left] != null ? floorGrid[top + 1, left].RoomID : -1) : -1;
            int roomNumberEast = IsInBounds(top, left + 1) ? (floorGrid[top, left + 1] != null ? floorGrid[top, left + 1].RoomID : -1) : -1;
            int roomNumberWest = IsInBounds(top, left - 1) ? (floorGrid[top, left - 1] != null ? floorGrid[top, left - 1].RoomID : -1) : -1;
            if (roomNumber != -1)
            {
                return (roomNumber == roomNumberNorth) ||
                    (roomNumber == roomNumberSouth) ||
                    (roomNumber == roomNumberEast) ||
                    (roomNumber == roomNumberWest);
            }
            else
            {
                return false;
            }
        }

        private void GroupByWall()
        {
            int northGroup = -2;
            int southGroup = -3;
            int westGroup = -4;
            int eastGroup = -5;

            for (int l = 0; l < width; l++)
            {
                for (int t = 0; t < height; t++)
                {
                    if (floorGrid[t, l] != null && !IsPartofRoom(t, l))
                    {
                        if (IsValidSegment(t - 1, l, true)) // north of this segment is null
                        {
                            floorGrid[t, l].GroupID = northGroup;
                        }
                        else if (IsValidSegment(t, l - 1, true)) // west segment is null
                        {
                            floorGrid[t, l].GroupID = westGroup;
                        }
                        else if (IsValidSegment(t, l + 1, true)) // east segment is null
                        {
                            floorGrid[t, l].GroupID = eastGroup;
                        }
                        else if (IsValidSegment(t + 1, l, true)) // south segment is null
                        {
                            floorGrid[t, l].GroupID = southGroup;
                        }
                    }
                }
            }
        }

        private void RemoveIslandRooms()
        {
            for (int left = 0; left < width; left++)
            {
                for (int top = 0; top < height; top++)
                {
                    int groupNumber = IsInBounds(top, left) ? (floorGrid[top, left] != null ? floorGrid[top, left].GroupID : -1) : -1;
                    FloorSegment segmentNorth = IsInBounds(top - 1, left) ? (floorGrid[top - 1, left] != null ? floorGrid[top - 1, left] : null) : null;
                    FloorSegment segmentSouth = IsInBounds(top + 1, left) ? (floorGrid[top + 1, left] != null ? floorGrid[top + 1, left] : null) : null;
                    FloorSegment segmentEast = IsInBounds(top, left + 1) ? (floorGrid[top, left + 1] != null ? floorGrid[top, left + 1] : null) : null;
                    FloorSegment segmentWest = IsInBounds(top, left - 1) ? (floorGrid[top, left - 1] != null ? floorGrid[top, left - 1] : null) : null;
                    if (groupNumber != -1)
                    {
                        if (segmentNorth != null && segmentSouth != null)
                        {
                            if ((groupNumber != segmentNorth.GroupID) && (groupNumber != segmentSouth.GroupID))
                            {
                                if (rnd.Next(2) == 0)
                                {
                                    floorGrid[top, left].GroupID = segmentNorth.GroupID;
                                    floorGrid[top, left].RoomID = segmentNorth.RoomID;
                                }
                                else
                                {
                                    floorGrid[top, left].GroupID = segmentSouth.GroupID;
                                    floorGrid[top, left].RoomID = segmentSouth.RoomID;
                                }
                            }
                        }
                        if (segmentEast != null && segmentWest != null)
                        {
                            if ((groupNumber != segmentEast.GroupID) && (groupNumber != segmentWest.GroupID))
                            {
                                if (rnd.Next(2) == 0)
                                {
                                    floorGrid[top, left].GroupID = segmentEast.GroupID;
                                    floorGrid[top, left].RoomID = segmentEast.RoomID;
                                }
                                else
                                {
                                    floorGrid[top, left].GroupID = segmentWest.GroupID;
                                    floorGrid[top, left].RoomID = segmentWest.RoomID;
                                }
                            }
                        }
                    }
                }
            }
        }

        public List<FloorSegment> GetAdjacentSegments(int top, int left)
        {
            FloorSegment segmentNorth = IsInBounds(top - 1, left) ? (floorGrid[top - 1, left] != null ? floorGrid[top - 1, left] : null) : null;
            FloorSegment segmentSouth = IsInBounds(top + 1, left) ? (floorGrid[top + 1, left] != null ? floorGrid[top + 1, left] : null) : null;
            FloorSegment segmentEast = IsInBounds(top, left + 1) ? (floorGrid[top, left + 1] != null ? floorGrid[top, left + 1] : null) : null;
            FloorSegment segmentWest = IsInBounds(top, left - 1) ? (floorGrid[top, left - 1] != null ? floorGrid[top, left - 1] : null) : null;
            List<FloorSegment> result = new List<FloorSegment>();
            if (segmentNorth != null) if (segmentNorth.GroupID != floorGrid[top, left].GroupID) result.Add(segmentNorth);
            if (segmentSouth != null) if (segmentSouth.GroupID != floorGrid[top, left].GroupID) result.Add(segmentSouth);
            if (segmentWest != null) if (segmentWest.GroupID != floorGrid[top, left].GroupID) result.Add(segmentWest);
            if (segmentEast != null) if (segmentEast.GroupID != floorGrid[top, left].GroupID) result.Add(segmentEast);
            return result;
        }

        private DirectionFlags getRoomEdges(int top, int left)
        {
            bool isNullNorth = IsInBounds(top - 1, left) ? (floorGrid[top - 1, left] == null ? true : false) : false;
            bool isNullSouth = IsInBounds(top + 1, left) ? (floorGrid[top + 1, left] == null ? true : false) : false;
            bool isNullWest = IsInBounds(top, left + 1) ? (floorGrid[top, left + 1] == null ? true : false) : false;
            bool isNullEast = IsInBounds(top, left - 1) ? (floorGrid[top, left - 1] == null ? true : false) : false;
            return new DirectionFlags()
            {
                north = isNullNorth,
                south = isNullSouth,
                east = isNullEast,
                west = isNullWest
            };
        }

        public void ClearDoorways()
        {
            for (int left = 0; left < width; left++)
            {
                for (int top = 0; top < height; top++)
                {
                    FloorSegment currFloorSegment = IsInBounds(top, left) ? (floorGrid[top, left] != null ? floorGrid[top, left] : null) : null;
                    if (currFloorSegment != null)
                    {
                        currFloorSegment.Doors.Clear();
                        currFloorSegment.ConnectedCorridorIDs.Clear();
                    }
                }
            }
            CorridorGroups.Clear();
        }

        private List<Direction> GetCorridorDirections(int top, int left)
        {
            List<Direction> result = new List<Direction>();
            FloorSegment currSegment = IsInBounds(top, left) ? (floorGrid[top, left] != null ? floorGrid[top, left] : null) : null;
            FloorSegment segmentNorth = IsInBounds(top - 1, left) ? (floorGrid[top - 1, left] != null ? floorGrid[top - 1, left] : null) : null;
            FloorSegment segmentSouth = IsInBounds(top + 1, left) ? (floorGrid[top + 1, left] != null ? floorGrid[top + 1, left] : null) : null;
            FloorSegment segmentEast = IsInBounds(top, left + 1) ? (floorGrid[top, left + 1] != null ? floorGrid[top, left + 1] : null) : null;
            FloorSegment segmentWest = IsInBounds(top, left - 1) ? (floorGrid[top, left - 1] != null ? floorGrid[top, left - 1] : null) : null;
            if (currSegment != null)
            {
                if (segmentWest != null) if (currSegment.GroupID != segmentWest.GroupID) result.Add(Direction.West);
                if (segmentNorth != null) if (currSegment.GroupID != segmentNorth.GroupID) result.Add(Direction.North);
                if (segmentSouth != null) if (currSegment.GroupID != segmentSouth.GroupID) result.Add(Direction.South);
                if (segmentEast != null) if (currSegment.GroupID != segmentEast.GroupID) result.Add(Direction.East);
            }
            return result;
        }

        private bool segmentsAreEqual(SegmentPos segPos1, SegmentPos segPos2)
        {
            return (segPos1.Top == segPos2.Top) && (segPos1.Left == segPos2.Left);
        }

        private bool AreCorridorsConnected(corridorSegment corridor1, corridorSegment corridor2)
        {
            var validCorridorPositions = new List<SegmentPos>();
            SegmentPos basePos = corridor1.Segment1;
            if (corridor1.Segment1.Top - 1 == corridor1.Segment2.Top)
            {

                validCorridorPositions.Add(new SegmentPos() { Top = basePos.Top - 1, Left = basePos.Left - 1 });
                validCorridorPositions.Add(new SegmentPos() { Top = basePos.Top - 1, Left = basePos.Left });
                validCorridorPositions.Add(new SegmentPos() { Top = basePos.Top - 1, Left = basePos.Left + 1 });
                validCorridorPositions.Add(new SegmentPos() { Top = basePos.Top, Left = basePos.Left - 1 });
                validCorridorPositions.Add(new SegmentPos() { Top = basePos.Top, Left = basePos.Left });
                validCorridorPositions.Add(new SegmentPos() { Top = basePos.Top, Left = basePos.Left + 1 });
            }
            else if (corridor1.Segment1.Left == corridor1.Segment2.Left - 1)
            {
                validCorridorPositions.Add(new SegmentPos() { Top = basePos.Top - 1, Left = basePos.Left });
                validCorridorPositions.Add(new SegmentPos() { Top = basePos.Top - 1, Left = basePos.Left + 1 });
                validCorridorPositions.Add(new SegmentPos() { Top = basePos.Top, Left = basePos.Left });
                validCorridorPositions.Add(new SegmentPos() { Top = basePos.Top, Left = basePos.Left + 1 });
                validCorridorPositions.Add(new SegmentPos() { Top = basePos.Top + 1, Left = basePos.Left });
                validCorridorPositions.Add(new SegmentPos() { Top = basePos.Top + 1, Left = basePos.Left + 1 });
            }
            else
            {
                throw new Exception("Invalid corridor segment");
            }

            bool cor2seg1isValid = validCorridorPositions.Any(cp => segmentsAreEqual(cp, corridor2.Segment1));
            bool cor2seg2isValid = validCorridorPositions.Any(cp => segmentsAreEqual(cp, corridor2.Segment2));

            return cor2seg1isValid && cor2seg2isValid;
        }

        public void FindRooms()
        {
            RoomDict.Clear();
            for (int left = 0; left < width; left++)
            {
                for (int top = 0; top < height; top++)
                {
                    FloorSegment currSegment = IsInBounds(top, left) ? (floorGrid[top, left] != null ? floorGrid[top, left] : null) : null;
                    if (currSegment != null)
                    {
                        if (RoomDict.ContainsKey(currSegment.RoomID))
                        {
                            RoomDict[currSegment.RoomID].RoomSegments.Add(new SegmentPos() { Left = left, Top = top });
                        }
                        else
                        {
                            RoomDict.Add(currSegment.RoomID, new Room()
                            {
                                RoomSegments = new HashSet<SegmentPos>() { new SegmentPos() { Left = left, Top = top } }
                            });
                        }
                    }
                }
            }
        }

        public int RoomDoors(int roomID)
        {
            int result = 0;
            Room currRoom = RoomDict[roomID];
            foreach (SegmentPos currPos in currRoom.RoomSegments)
            {
                FloorSegment currSeg = floorGrid[currPos.Top, currPos.Left];
                result += currSeg.Doors.Count;
            }
            return result;
        }

        public void FindCorridors()
        {
            FindRooms();
            var corridorList = new List<List<corridorSegment>>();
            for (int left = 0; left < width; left++)
            {
                for (int top = 0; top < height; top++)
                {
                    FloorSegment currSegment = IsInBounds(top, left) ? (floorGrid[top, left] != null ? floorGrid[top, left] : null) : null;
                    if (currSegment != null)
                    {
                        currSegment.ConnectedCorridorIDs.Clear();
                        currSegment.AdjacentCorridorID = null;
                        List<Direction> corridorDirections = GetCorridorDirections(top, left);
                        List<corridorSegment> newCorridorSegments = new List<corridorSegment>();
                        foreach (Direction nextCorDir in corridorDirections)
                        {
                            // genetate the hallway segment
                            switch (nextCorDir)
                            {
                                case Direction.North:
                                    newCorridorSegments.Add(new corridorSegment()
                                    {
                                        Segment1 = new SegmentPos() { Top = top, Left = left },
                                        Segment2 = new SegmentPos() { Top = top - 1, Left = left }
                                    });
                                    break;
                                case Direction.South:
                                    newCorridorSegments.Add(new corridorSegment()
                                    {
                                        Segment1 = new SegmentPos() { Top = top + 1, Left = left },
                                        Segment2 = new SegmentPos() { Top = top, Left = left }
                                    });
                                    break;
                                case Direction.East:
                                    newCorridorSegments.Add(new corridorSegment()
                                    {
                                        Segment1 = new SegmentPos() { Top = top, Left = left },
                                        Segment2 = new SegmentPos() { Top = top, Left = left + 1 }
                                    });
                                    break;
                                case Direction.West:
                                    newCorridorSegments.Add(new corridorSegment()
                                    {
                                        Segment1 = new SegmentPos() { Top = top, Left = left - 1 },
                                        Segment2 = new SegmentPos() { Top = top, Left = left }
                                    });
                                    break;
                            }

                        }

                        // For each of the new corridor segments, check to see if they belong to an existing corridor.
                        // If it doesn't, start a new list
                        foreach (corridorSegment currCorridorSegment in newCorridorSegments)
                        {
                            var inList = new List<List<corridorSegment>>();
                            var outList = new List<List<corridorSegment>>();
                            foreach (List<corridorSegment> currCorridor in corridorList)
                            {
                                if (currCorridor.Any(cc => AreCorridorsConnected(cc, currCorridorSegment)))
                                {
                                    inList.Add(currCorridor);
                                }
                                else
                                {
                                    outList.Add(currCorridor);
                                }
                            }

                            var mergedList = new List<corridorSegment>();
                            mergedList.Add(currCorridorSegment);
                            if (inList.Count > 0)
                            {
                                foreach (List<corridorSegment> currInList in inList)
                                {
                                    mergedList.AddRange(currInList);
                                }
                            }

                            var newCorridorList = new List<List<corridorSegment>>();
                            newCorridorList.Add(mergedList);
                            newCorridorList.AddRange(outList);
                            corridorList = newCorridorList;
                        }
                    }
                }
            }
            CorridorGroups = new List<CorridorGroup>();

            // Define the corridor groups from the list of corridors.
            // A corridor group contains the definition of the corridor, and a list of rooms which belong to it
            int corridorGroupID = 1;
            foreach (List<corridorSegment> currCorridor in corridorList)
            {
                CorridorGroups.Add(new CorridorGroup()
                {
                    Name = corridorGroupID.ToString(),
                    Segments = currCorridor,
                });
                corridorGroupID++;
            }

            // Determine the starting groups in each corridorgroup
            foreach (CorridorGroup currGroup in CorridorGroups)
            {
                foreach (corridorSegment currSegment in currGroup.Segments)
                {
                    // Segment1 of the corridor
                    floorGrid[currSegment.Segment1.Top, currSegment.Segment1.Left].AdjacentCorridorID = currGroup.Name;
                    // Segment2 of the corridor
                    floorGrid[currSegment.Segment2.Top, currSegment.Segment2.Left].AdjacentCorridorID = currGroup.Name;
                }
            }

        }

        public void AssignDoorways()
        {
            int numEligibleRooms = AssignDoorway();
            while (numEligibleRooms > 0)
            {
                numEligibleRooms = AssignDoorway();
            }
        }

        public int AssignDoorway()
        {
            // Determine which rooms are large.
            roomSizeDict.Clear();
            for (int left = 0; left < width; left++)
            {
                for (int top = 0; top < height; top++)
                {
                    FloorSegment currFloorSegment = IsInBounds(top, left) ? (floorGrid[top, left] != null ? floorGrid[top, left] : null) : null;
                    if (currFloorSegment != null)
                    {
                        // determine the size of the room by counting the number of tiles with this room number
                        if (!roomSizeDict.ContainsKey(currFloorSegment.RoomID))
                        {
                            roomSizeDict.Add(currFloorSegment.RoomID, 1);
                        }
                        else
                        {
                            roomSizeDict[currFloorSegment.RoomID]++;
                        }
                    }
                }
            }

            // eligibleDoors is a dictionary where int is the room number and eligibledoor is a structure to describe a potential door position
            Dictionary<int, List<EligibleDoor>> eligibleDoors = new Dictionary<int, List<EligibleDoor>>();

            // We are iterating though all floor segments, adding to the eligibleDoors dictionary.
            for (int left = 0; left < width; left++)
            {
                for (int top = 0; top < height; top++)
                {
                    FloorSegment currFloorSegment = IsInBounds(top, left) ? (floorGrid[top, left] != null ? floorGrid[top, left] : null) : null;
                    if (currFloorSegment != null)
                    {
                        eligibleDoors = GetEligibleDoors(top, left, eligibleDoors);
                    }
                }
            }

            // Pick one room to recieve a door.  If no more doors are possible, return
            int[] possibleRooms = eligibleDoors.Keys.ToArray();
            if (possibleRooms.Length == 0) return 0;

            int selectedRoomID = possibleRooms[rnd.Next(possibleRooms.Length)];

            EligibleDoor[] possibleDoors = eligibleDoors[selectedRoomID].ToArray();
            EligibleDoor selectedDoor = possibleDoors[rnd.Next(possibleDoors.Length)];
            FloorSegment originSegment = floorGrid[selectedDoor.Top, selectedDoor.Left];
            if (selectedDoor.RoomID == selectedRoomID) // if the selected door room id is the same as the selected room, this indicates a door pointing into a corridor
            {
                // Place the door
                floorGrid[selectedDoor.Top, selectedDoor.Left].Doors.Add(selectedDoor.doorDirection);
                // mark each segment of the room with the newly connected corridor
                foreach (SegmentPos currPos in RoomDict[selectedRoomID].RoomSegments)
                {
                    floorGrid[currPos.Top, currPos.Left].ConnectedCorridorIDs.Add(originSegment.AdjacentCorridorID);
                }
                HashSet<string> originSegmentHallways = originSegment.ConnectedCorridorIDs;
                for (int left = 0; left < width; left++)
                {
                    for (int top = 0; top < height; top++)
                    {
                        FloorSegment currFloorSegment = IsInBounds(top, left) ? (floorGrid[top, left] != null ? floorGrid[top, left] : null) : null;
                        if (currFloorSegment != null)
                        {
                            foreach (string originSegmentHallwayID in originSegmentHallways)
                            {
                                if (currFloorSegment.ConnectedCorridorIDs.Contains(originSegmentHallwayID))
                                {
                                    if (currFloorSegment.ConnectedCorridorIDs.Count > originSegmentHallways.Count)
                                    {
                                        originSegmentHallways = currFloorSegment.ConnectedCorridorIDs;
                                    }
                                    currFloorSegment.ConnectedCorridorIDs.UnionWith(originSegmentHallways);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                // An adjoining room door.
                originSegment.Doors.Add(selectedDoor.doorDirection);
                FloorSegment connectingSegment = null;
                switch (selectedDoor.doorDirection)
                {
                    case Direction.North:
                        floorGrid[selectedDoor.Top - 1, selectedDoor.Left].Doors.Add(Direction.South);
                        connectingSegment = floorGrid[selectedDoor.Top - 1, selectedDoor.Left];
                        break;
                    case Direction.South:
                        floorGrid[selectedDoor.Top + 1, selectedDoor.Left].Doors.Add(Direction.North);
                        connectingSegment = floorGrid[selectedDoor.Top + 1, selectedDoor.Left];
                        break;
                    case Direction.East:
                        floorGrid[selectedDoor.Top, selectedDoor.Left + 1].Doors.Add(Direction.West);
                        connectingSegment = floorGrid[selectedDoor.Top, selectedDoor.Left + 1];
                        break;
                    case Direction.West:
                        floorGrid[selectedDoor.Top, selectedDoor.Left - 1].Doors.Add(Direction.East);
                        connectingSegment = floorGrid[selectedDoor.Top, selectedDoor.Left - 1];
                        break;
                }

                // Join the hallways.  wooooo  
                foreach (SegmentPos currOriginPos in RoomDict[originSegment.RoomID].RoomSegments)
                {
                    floorGrid[currOriginPos.Top, currOriginPos.Left].ConnectedCorridorIDs.UnionWith(connectingSegment.ConnectedCorridorIDs);
                }
                foreach (SegmentPos currConnectingPos in RoomDict[connectingSegment.RoomID].RoomSegments)
                {
                    floorGrid[currConnectingPos.Top, currConnectingPos.Left].ConnectedCorridorIDs.UnionWith(originSegment.ConnectedCorridorIDs);
                }
                HashSet<string> originSegmentHallways = originSegment.ConnectedCorridorIDs;
                HashSet<string> connectingSegmentHallways = connectingSegment.ConnectedCorridorIDs;
                for (int left = 0; left < width; left++)
                {
                    for (int top = 0; top < height; top++)
                    {
                        FloorSegment currFloorSegment = IsInBounds(top, left) ? (floorGrid[top, left] != null ? floorGrid[top, left] : null) : null;
                        if (currFloorSegment != null)
                        {
                            foreach (string originSegmentHallwayID in originSegmentHallways)
                            {
                                if (currFloorSegment.ConnectedCorridorIDs.Contains(originSegmentHallwayID))
                                {
                                    currFloorSegment.ConnectedCorridorIDs.UnionWith(connectingSegmentHallways);
                                }
                            }
                            foreach (string connectingSegmentHallwayID in connectingSegmentHallways)
                            {
                                if (currFloorSegment.ConnectedCorridorIDs.Contains(connectingSegmentHallwayID))
                                {
                                    currFloorSegment.ConnectedCorridorIDs.UnionWith(originSegmentHallways);
                                }
                            }
                        }
                    }
                }
            }
            return possibleRooms.Length - 1;
        }

        private bool IsValidForNewDoor(SegmentPos toCheck, SegmentPos checkAgainst)
        {
            bool result = true;
            // The door to the target room is valid if the target room is connected to a hallway that the origin room
            // is not yet connected to;
            FloorSegment originSegment = floorGrid[toCheck.Top, toCheck.Left];
            FloorSegment targetSegment = floorGrid[checkAgainst.Top, checkAgainst.Left];
            if (targetSegment.ConnectedCorridorIDs.Count > 0)
            {
                foreach (string currCorridorID in targetSegment.ConnectedCorridorIDs)
                {
                    if (originSegment.ConnectedCorridorIDs.Contains(currCorridorID))
                    {
                        result = false;
                        break;
                    }
                }
            }
            else
            {
                result = false;
            }
            return result;
        }

        private bool IsValidCorridorForNewDoor(int inTop, int inLeft)
        {
            // A corridor door is valid if the corridor it leads to is one to which it is not already attached.
            bool result = true;
            string adjacentCorridor = floorGrid[inTop, inLeft].AdjacentCorridorID;
            HashSet<string> connectedCorridors = floorGrid[inTop, inLeft].ConnectedCorridorIDs;
            if (connectedCorridors.Contains(adjacentCorridor))
            {
                result = false;
            }
            return result;
        }

        private Dictionary<int, List<EligibleDoor>> GetEligibleDoors(int top, int left, Dictionary<int, List<EligibleDoor>> eligibleDoors)
        {
            FloorSegment currSegment = IsInBounds(top, left) ? (floorGrid[top, left] != null ? floorGrid[top, left] : null) : null;
            FloorSegment segmentNorth = IsInBounds(top - 1, left) ? (floorGrid[top - 1, left] != null ? floorGrid[top - 1, left] : null) : null;
            FloorSegment segmentSouth = IsInBounds(top + 1, left) ? (floorGrid[top + 1, left] != null ? floorGrid[top + 1, left] : null) : null;
            FloorSegment segmentEast = IsInBounds(top, left + 1) ? (floorGrid[top, left + 1] != null ? floorGrid[top, left + 1] : null) : null;
            FloorSegment segmentWest = IsInBounds(top, left - 1) ? (floorGrid[top, left - 1] != null ? floorGrid[top, left - 1] : null) : null;

            List<EligibleDoor> hallwayDoors = new List<EligibleDoor>();
            List<EligibleDoor> roomDoors = new List<EligibleDoor>();

            // Check all the floor segments around this one to determine if it is valid to place a door there.

            if (segmentNorth != null)
                if (segmentNorth.RoomID != currSegment.RoomID) // if the room id's are not the same it's a different room
                    if (segmentNorth.GroupID != currSegment.GroupID) // if the group id's are not the same its a different group
                    {
                        if (IsValidCorridorForNewDoor(top, left))
                            hallwayDoors.Add(new EligibleDoor() { Top = top, Left = left, RoomID = currSegment.RoomID, doorDirection = Direction.North });
                    }
                    else
                    {
                        if (IsValidForNewDoor(new SegmentPos() { Top = top, Left = left }, new SegmentPos() { Top = top - 1, Left = left }))
                            roomDoors.Add(new EligibleDoor() { Top = top - 1, Left = left, RoomID = segmentNorth.RoomID, doorDirection = Direction.South });
                    }
            if (segmentSouth != null)
                if (segmentSouth.RoomID != currSegment.RoomID)
                    if (segmentSouth.GroupID != currSegment.GroupID)
                    {
                        if (IsValidCorridorForNewDoor(top, left))
                            hallwayDoors.Add(new EligibleDoor() { Top = top, Left = left, RoomID = currSegment.RoomID, doorDirection = Direction.South });
                    }
                    else
                    {
                        if (IsValidForNewDoor(new SegmentPos() { Top = top, Left = left }, new SegmentPos() { Top = top + 1, Left = left }))
                            roomDoors.Add(new EligibleDoor() { Top = top + 1, Left = left, RoomID = segmentSouth.RoomID, doorDirection = Direction.North });
                    }
            if (segmentEast != null)
                if (segmentEast.RoomID != currSegment.RoomID)
                    if (segmentEast.GroupID != currSegment.GroupID)
                    {
                        if (IsValidCorridorForNewDoor(top, left))
                            hallwayDoors.Add(new EligibleDoor() { Top = top, Left = left, RoomID = currSegment.RoomID, doorDirection = Direction.East });
                    }
                    else
                    {
                        if (IsValidForNewDoor(new SegmentPos() { Top = top, Left = left }, new SegmentPos() { Top = top, Left = left + 1 }))
                            roomDoors.Add(new EligibleDoor() { Top = top, Left = left + 1, RoomID = segmentEast.RoomID, doorDirection = Direction.West });
                    }
            if (segmentWest != null)
                if (segmentWest.RoomID != currSegment.RoomID)
                    if (segmentWest.GroupID != currSegment.GroupID)
                    {
                        if (IsValidCorridorForNewDoor(top, left))
                            hallwayDoors.Add(new EligibleDoor() { Top = top, Left = left, RoomID = currSegment.RoomID, doorDirection = Direction.West });
                    }
                    else
                    {
                        if (IsValidForNewDoor(new SegmentPos() { Top = top, Left = left }, new SegmentPos() { Top = top, Left = left - 1 }))
                            roomDoors.Add(new EligibleDoor() { Top = top, Left = left - 1, RoomID = segmentWest.RoomID, doorDirection = Direction.East });
                    }

            // Add all valid doors
            if (hallwayDoors.Count > 0)
            {
                if (!eligibleDoors.ContainsKey(currSegment.RoomID)) eligibleDoors.Add(currSegment.RoomID, new List<EligibleDoor>());
                hallwayDoors.ForEach(hd => eligibleDoors[currSegment.RoomID].Add(hd));
            }

            if (roomDoors.Count > 0)
            {
                if (!eligibleDoors.ContainsKey(currSegment.RoomID)) eligibleDoors.Add(currSegment.RoomID, new List<EligibleDoor>());
                roomDoors.ForEach(rd => eligibleDoors[currSegment.RoomID].Add(rd));
            }


            return eligibleDoors;
        }

        public void RegroupSpanningGroups()
        {
            // first find rooms which belong to a group that splits a floorplan in twain
            ClearDoorways();
            Dictionary<int, DirectionFlags> flagDict = new Dictionary<int, DirectionFlags>();
            Dictionary<FloorSegment, List<FloorSegment>> roomEdgeDict = new Dictionary<FloorSegment, List<FloorSegment>>();
            for (int left = 0; left < width; left++)
            {
                for (int top = 0; top < height; top++)
                {
                    int groupNumber = IsInBounds(top, left) ? (floorGrid[top, left] != null ? floorGrid[top, left].GroupID : -1) : -1;
                    FloorSegment currFloorSegment = IsInBounds(top, left) ? (floorGrid[top, left] != null ? floorGrid[top, left] : null) : null;
                    if (groupNumber != -1)
                    {
                        DirectionFlags edgeDirections = getRoomEdges(top, left);
                        if (edgeDirections.north || edgeDirections.south || edgeDirections.east || edgeDirections.west)
                        {
                            if (!flagDict.ContainsKey(groupNumber))
                            {
                                flagDict.Add(groupNumber, edgeDirections);
                            }
                            else
                            {
                                flagDict[groupNumber] = new DirectionFlags()
                                {
                                    north = flagDict[groupNumber].north || edgeDirections.north,
                                    south = flagDict[groupNumber].south || edgeDirections.south,
                                    east = flagDict[groupNumber].east || edgeDirections.east,
                                    west = flagDict[groupNumber].west || edgeDirections.west
                                };
                            }
                            // save the room number if it is adjacent to another group
                            List<FloorSegment> adjacentSegments = GetAdjacentSegments(top, left);
                            if (adjacentSegments.Count > 0)
                            {
                                if (!roomEdgeDict.ContainsKey(currFloorSegment)) roomEdgeDict.Add(currFloorSegment, new List<FloorSegment>());
                                foreach (FloorSegment currSeg in adjacentSegments) roomEdgeDict[currFloorSegment].Add(currSeg);
                            }
                        }
                    }
                }
            }
            // flagDict now contains a dictionary of groups which have a room that touches an edge.
            // we're interested in any groups which touch more than one edge.
            List<KeyValuePair<int, DirectionFlags>> edgeGroups = new List<KeyValuePair<int, DirectionFlags>>();
            foreach (KeyValuePair<int, DirectionFlags> currKV in flagDict)
            {
                if (spansFloorPlan(currKV.Value))
                {
                    edgeGroups.Add(currKV);
                }
            }
            if (edgeGroups.Count > 0)
            {
                Console.WriteLine(edgeGroups.Count.ToString());
                Console.WriteLine(roomEdgeDict.Keys.Count.ToString());
            }
            // Now, for each of the groups that touch opposite edges, we select one of the rooms which
            // is adjacent to another group and merge it
            spanningRoomCount = edgeGroups.Count;
            foreach (KeyValuePair<int, DirectionFlags> currEdgeGroup in edgeGroups)
            {
                // select a room from the group
                KeyValuePair<FloorSegment, List<FloorSegment>>[] roomsUnderConsideration =
                    roomEdgeDict.Where<KeyValuePair<FloorSegment, List<FloorSegment>>>(p => p.Key.GroupID == currEdgeGroup.Key).ToArray<KeyValuePair<FloorSegment, List<FloorSegment>>>();
                if (roomsUnderConsideration.Length > 0)
                {
                    KeyValuePair<FloorSegment, List<FloorSegment>> selected = roomsUnderConsideration[rnd.Next(roomsUnderConsideration.Length)];
                    for (int left = 0; left < width; left++)
                    {
                        for (int top = 0; top < height; top++)
                        {
                            FloorSegment currFloorSegment = IsInBounds(top, left) ? (floorGrid[top, left] != null ? floorGrid[top, left] : null) : null;
                            if (currFloorSegment != null)
                            {
                                if (currFloorSegment.RoomID == selected.Key.RoomID)
                                {
                                    floorGrid[top, left].GroupID = selected.Value.First<FloorSegment>().GroupID;
                                }
                            }
                        }
                    }
                }
            }
            RemoveIslandRooms();
            FindCorridors();
        }

        private bool spansFloorPlan(DirectionFlags inFlag)
        {
            int numExternalWallsTouched = 0;
            if (inFlag.north) numExternalWallsTouched++;
            if (inFlag.south) numExternalWallsTouched++;
            if (inFlag.east) numExternalWallsTouched++;
            if (inFlag.west) numExternalWallsTouched++;
            return numExternalWallsTouched > 1;
        }

        private int getTouchedEdges(DirectionFlags inFlag)
        {
            int touchedEdges = 0;
            if (inFlag.north) touchedEdges++;
            if (inFlag.south) touchedEdges++;
            if (inFlag.east) touchedEdges++;
            if (inFlag.west) touchedEdges++;
            return touchedEdges;
        }

        private void GenerateRoom(FloorplanDirectionalSegment startSegment, bool largeRoom, bool isFootprint, int inFootprintLength, int inFootprintWidth)
        {
            int roomLength = largeRoom ? rnd.Next(maxRoomSize) + 1 : 0;
            int roomLeftSize = largeRoom ? rnd.Next(maxRoomSize) : 0;
            int roomRightSize = largeRoom ? rnd.Next(maxRoomSize) : 0;
            int thisGroupID = isFootprint ? -1 : currGroupID;
            if (isFootprint)
            {
                roomLength = rnd.Next(inFootprintLength) + 3;
                roomLeftSize = rnd.Next(inFootprintWidth) + 2;
                roomRightSize = rnd.Next(inFootprintWidth) + 2;
            }
            bool roomSuccess = false;
            for (int i = 0; i <= roomLength; i++)
            {
                FloorplanDirectionalSegment ns = startSegment.IncrementForwards(i);
                // if the next segment is free, we can do the next part of the room
                if (IsValidSegment(ns.Top, ns.Left, isFootprint))
                {
                    // We've reached the end of the room length wise, add an eligible segment if there's room
                    if (i == roomLength)
                    {
                        floorGrid[ns.Top, ns.Left] =
                            new FloorSegment()
                            {
                                RoomID = currRoomID,
                                GroupID = thisGroupID
                            };
                        roomSuccess = true;

                        FloorplanDirectionalSegment newStartSegment = ns.IncrementForwards(1);
                        if (IsValidSegment(newStartSegment.Top, newStartSegment.Left, isFootprint)) eligibleFloorSegments.Enqueue(newStartSegment);
                        if (i == 0) // the first segment potentially has an eligible section behind it.
                        {
                            newStartSegment = ns.IncrementForwards(-1);
                            if (IsValidSegment(newStartSegment.Top, newStartSegment.Left, isFootprint)) eligibleFloorSegments.Enqueue(newStartSegment);
                        }

                    }
                    else // place the floor segment
                    {
                        floorGrid[ns.Top, ns.Left] =
                            new FloorSegment()
                            {
                                RoomID = currRoomID,
                                GroupID = thisGroupID
                            };
                        roomSuccess = true;
                    }

                    if (pause) waitEvent.WaitOne();
                    // Add the left section of the room
                    FloorplanDirectionalSegment firstLeftSegment = ns.IncrementLeft(1);
                    for (int l = 0; l <= roomLeftSize; l++)
                    {
                        // We've reached the max size for the left part of the room
                        FloorplanDirectionalSegment nextLeftSegment = firstLeftSegment.IncrementForwards(l);
                        if (l == roomLeftSize)
                        {
                            if (IsValidSegment(nextLeftSegment.Top, nextLeftSegment.Left, isFootprint)) eligibleFloorSegments.Enqueue(nextLeftSegment);
                        }
                        else // place the floor segment if there's room
                        {
                            if (IsValidSegment(nextLeftSegment.Top, nextLeftSegment.Left, isFootprint))
                            {
                                floorGrid[nextLeftSegment.Top, nextLeftSegment.Left] =
                                    new FloorSegment()
                                    {
                                        RoomID = currRoomID,
                                        GroupID = thisGroupID
                                    };

                                // If we're also at maximum room length, we add the new eligible segments 
                                if (i == roomLength)
                                {
                                    FloorplanDirectionalSegment nextForwardsStartSegment = nextLeftSegment.IncrementRight(1);
                                    if (IsValidSegment(nextForwardsStartSegment.Top, nextForwardsStartSegment.Left, isFootprint)) eligibleFloorSegments.Enqueue(nextForwardsStartSegment);
                                }
                                if (i == 0)  // On first row, place eligible segments behind if room
                                {
                                    FloorplanDirectionalSegment nextForwardsStartSegment = nextLeftSegment.IncrementLeft(1);
                                    if (IsValidSegment(nextForwardsStartSegment.Top, nextForwardsStartSegment.Left, isFootprint)) eligibleFloorSegments.Enqueue(nextForwardsStartSegment);
                                }
                            }
                            else
                            {
                                break; // We've collided moving in the left direction. Abort placing left sections
                            }
                        }
                        if (pause) waitEvent.WaitOne();
                    }
                    // Add the right section of the room
                    FloorplanDirectionalSegment firstRightSegment = ns.IncrementRight(1);
                    for (int r = 0; r <= roomRightSize; r++)
                    {
                        FloorplanDirectionalSegment nextRightSegment = firstRightSegment.IncrementForwards(r);
                        // We've reached the max size for the left part of the room
                        if (r == roomRightSize)
                        {
                            if (IsValidSegment(nextRightSegment.Top, nextRightSegment.Left, isFootprint)) eligibleFloorSegments.Enqueue(nextRightSegment);
                        }
                        else // place the segment if there's room
                        {
                            if (IsValidSegment(nextRightSegment.Top, nextRightSegment.Left, isFootprint))
                            {
                                floorGrid[nextRightSegment.Top, nextRightSegment.Left] =
                                    new FloorSegment()
                                    {
                                        RoomID = currRoomID,
                                        GroupID = thisGroupID
                                    };

                                // If we're also at maximum room length, we add the new eligible segments 
                                if (i == roomLength)
                                {
                                    FloorplanDirectionalSegment nextForwardsStartSegment = nextRightSegment.IncrementLeft(1);
                                    if (IsValidSegment(nextForwardsStartSegment.Top, nextForwardsStartSegment.Left, isFootprint)) eligibleFloorSegments.Enqueue(nextForwardsStartSegment);
                                }
                                if (i == 0)  // On first row, place eligible segments behind if room
                                {
                                    FloorplanDirectionalSegment nextForwardsStartSegment = nextRightSegment.IncrementRight(1);
                                    if (IsValidSegment(nextForwardsStartSegment.Top, nextForwardsStartSegment.Left, isFootprint)) eligibleFloorSegments.Enqueue(nextForwardsStartSegment);
                                }
                            }
                            else
                            {
                                break; // We've collided moving in the left direction. Abort placing left sections
                            }
                        }
                        if (pause) waitEvent.WaitOne();
                    }
                }
                else break;
            }
            if (roomSuccess)
            {
                roomCount++;
                currRoomID++;
                currGroupRoomCount++;
                if (largeRoom) largeRoomCount++;
                if (currGroupRoomCount > currMaxRoomsPerGroup)
                {
                    currGroupRoomCount = 0;
                    currGroupID++;
                }
            }
        }
    }
}
