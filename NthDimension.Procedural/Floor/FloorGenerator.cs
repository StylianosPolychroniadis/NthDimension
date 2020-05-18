using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using NthDimension.Procedural.Floor;

namespace NthDimension.Procedural
{
    public class FloorGenerator : IGenerator
    {
        public int Width {  get { return width; } }
        public int Length { get { return length; } }

        public AutoResetEvent                   WaitEvent            = new AutoResetEvent(true);
        public FloorSegment[,]                  FloorGrid;
        public Dictionary<int, int>             RoomSizes            = new Dictionary<int, int>();
        public List<CorridorGroup>              CorridorGroups;
        public Dictionary<int, Floor.Room>            Rooms                = new Dictionary<int, Floor.Room>();
        public int                              CurrentRoomId              = 0;
        public int                              SpanningRoomCount       = 0;
        public Queue<DirectionalSegment>        EligibleFloorSegments   = new Queue<DirectionalSegment>();

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
        private int length;
        private int maxRoomSize;
        private int blueprintSections;
        private int blueprintSectionLength;
        private int blueprintSectionWidth;
        private Random rnd = new Random();

        public FloorGenerator(int inwidth, int inlength, /*, TODO: int height,*/ int inMaxRooms, int inMaxRoomsPerGroup, int inMaxRoomSize,
            int inMaxLargeRooms, int inBluePrintSections, int inFootprintSectionLength, int inFootprintSectionWidth, bool inpause)
        {
            maxRooms = inMaxRooms;
            width = inwidth;
            length = inlength;
            maxRoomSize = inMaxRoomSize;
            maxRoomsPerGroup = inMaxRoomsPerGroup;
            FloorGrid = new FloorSegment[length, width];
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
            int startTop = (length / 3) * 2;
            int startLeft = (width / 3) * 2;
            roomCount = 0;
            EligibleFloorSegments.Enqueue(new DirectionalSegment(startTop, startLeft, Direction.North, currGroupID));
            while (EligibleFloorSegments.Count > 0)
            {
                int shuffleAmount = rnd.Next(EligibleFloorSegments.Count);
                for (int i = 0; i <= shuffleAmount; i++) EligibleFloorSegments.Enqueue(EligibleFloorSegments.Dequeue());
                DirectionalSegment currSegment = EligibleFloorSegments.Dequeue();
                generateRoom(currSegment, true, true, blueprintSectionLength, blueprintSectionWidth);
                if (roomCount >= inMaxFootprintSections) break;
            }
            roomCount = 0;
            currGroupRoomCount = 0;
            largeRoomCount = 0;
            currGroupID = 0;
            EligibleFloorSegments.Clear();
        }

        public void GenerateFloorPlan()
        {
            GenerateFloorFootprint(blueprintSections);
            int startTop = (length / 3) * 2;
            int startLeft = (width / 3) * 2;
            EligibleFloorSegments.Enqueue(new DirectionalSegment(startTop, startLeft, Direction.North, currGroupID));
            currMaxLargeRooms = rnd.Next(maxLargeRooms);
            currMaxRoomsPerGroup = rnd.Next(maxRoomsPerGroup) + 1;
            while (EligibleFloorSegments.Count > 0)
            {
                Queue<DirectionalSegment> selectedSegments = null;
                bool largeRoom = false;
                if (currGroupRoomCount <= currMaxRoomsPerGroup)
                {
                    selectedSegments = new Queue<DirectionalSegment>(EligibleFloorSegments.Where<DirectionalSegment>(d => d.GroupID == currGroupID));
                }
                if (selectedSegments == null || selectedSegments.Count == 0)
                {
                    selectedSegments = EligibleFloorSegments;
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
                DirectionalSegment currSegment = selectedSegments.Dequeue();
                EligibleFloorSegments = new Queue<DirectionalSegment>(EligibleFloorSegments.Where<DirectionalSegment>(d => d != currSegment));
                currSegment.GroupID = currGroupID;
                generateRoom(currSegment, largeRoom, false, blueprintSectionLength, blueprintSectionWidth);
                if (roomCount >= maxRooms) break;

            }

            //GroupByWall();
            removeIslandRooms();

            FindCorridors();
        }

        public bool IsInBounds(int Top, int Left)
        {
            return (Left >= 1 && Left < (width - 1)) && (Top >= 1 && Top < (length - 1));
        }

        public List<Direction> GetOutterWallDirections(int Top, int Left)
        {
            List<Direction> result = new List<Direction>();
            if (IsInBounds(Top - 1, Left)) if (FloorGrid[Top - 1, Left] == null) result.Add(Direction.North);
            if (IsInBounds(Top + 1, Left)) if (FloorGrid[Top + 1, Left] == null) result.Add(Direction.South);
            if (IsInBounds(Top, Left - 1)) if (FloorGrid[Top, Left - 1] == null) result.Add(Direction.West);
            if (IsInBounds(Top, Left + 1)) if (FloorGrid[Top, Left + 1] == null) result.Add(Direction.East);
            return result;
        }

        public bool isValidSegment(int Top, int Left, bool isfootprint)
        {
            bool inBounds = IsInBounds(Top, Left);
            bool notUsed = false;
            bool result = false;

            if (isfootprint)
            {
                notUsed = inBounds ? (FloorGrid[Top, Left] == null ? true : false) : false;
                result = inBounds && notUsed;
            }
            else
            {
                if (inBounds && FloorGrid[Top, Left] != null)
                {
                    notUsed = FloorGrid[Top, Left].GroupID == -1 ? true : false;
                }
                else
                {
                    notUsed = false;
                }
            }
            return inBounds && notUsed;
        }

        private bool isPartofRoom(int top, int left)
        {
            int roomNumber = IsInBounds(top, left) ? (FloorGrid[top, left] != null ? FloorGrid[top, left].RoomID : -1) : -1;
            int roomNumberNorth = IsInBounds(top - 1, left) ? (FloorGrid[top - 1, left] != null ? FloorGrid[top - 1, left].RoomID : -1) : -1;
            int roomNumberSouth = IsInBounds(top + 1, left) ? (FloorGrid[top + 1, left] != null ? FloorGrid[top + 1, left].RoomID : -1) : -1;
            int roomNumberEast = IsInBounds(top, left + 1) ? (FloorGrid[top, left + 1] != null ? FloorGrid[top, left + 1].RoomID : -1) : -1;
            int roomNumberWest = IsInBounds(top, left - 1) ? (FloorGrid[top, left - 1] != null ? FloorGrid[top, left - 1].RoomID : -1) : -1;
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

        private void groupByWall()
        {
            int northGroup = -2;
            int southGroup = -3;
            int westGroup = -4;
            int eastGroup = -5;

            for (int l = 0; l < width; l++)
            {
                for (int t = 0; t < length; t++)
                {
                    if (FloorGrid[t, l] != null && !isPartofRoom(t, l))
                    {
                        if (isValidSegment(t - 1, l, true)) // north of this segment is null
                        {
                            FloorGrid[t, l].GroupID = northGroup;
                        }
                        else if (isValidSegment(t, l - 1, true)) // west segment is null
                        {
                            FloorGrid[t, l].GroupID = westGroup;
                        }
                        else if (isValidSegment(t, l + 1, true)) // east segment is null
                        {
                            FloorGrid[t, l].GroupID = eastGroup;
                        }
                        else if (isValidSegment(t + 1, l, true)) // south segment is null
                        {
                            FloorGrid[t, l].GroupID = southGroup;
                        }
                    }
                }
            }
        }

        private void removeIslandRooms()
        {
            for (int left = 0; left < width; left++)
            {
                for (int top = 0; top < length; top++)
                {
                    int groupNumber = IsInBounds(top, left) ? (FloorGrid[top, left] != null ? FloorGrid[top, left].GroupID : -1) : -1;
                    FloorSegment segmentNorth = IsInBounds(top - 1, left) ? (FloorGrid[top - 1, left] != null ? FloorGrid[top - 1, left] : null) : null;
                    FloorSegment segmentSouth = IsInBounds(top + 1, left) ? (FloorGrid[top + 1, left] != null ? FloorGrid[top + 1, left] : null) : null;
                    FloorSegment segmentEast = IsInBounds(top, left + 1) ? (FloorGrid[top, left + 1] != null ? FloorGrid[top, left + 1] : null) : null;
                    FloorSegment segmentWest = IsInBounds(top, left - 1) ? (FloorGrid[top, left - 1] != null ? FloorGrid[top, left - 1] : null) : null;
                    if (groupNumber != -1)
                    {
                        if (segmentNorth != null && segmentSouth != null)
                        {
                            if ((groupNumber != segmentNorth.GroupID) && (groupNumber != segmentSouth.GroupID))
                            {
                                if (rnd.Next(2) == 0)
                                {
                                    FloorGrid[top, left].GroupID = segmentNorth.GroupID;
                                    FloorGrid[top, left].RoomID = segmentNorth.RoomID;
                                }
                                else
                                {
                                    FloorGrid[top, left].GroupID = segmentSouth.GroupID;
                                    FloorGrid[top, left].RoomID = segmentSouth.RoomID;
                                }
                            }
                        }
                        if (segmentEast != null && segmentWest != null)
                        {
                            if ((groupNumber != segmentEast.GroupID) && (groupNumber != segmentWest.GroupID))
                            {
                                if (rnd.Next(2) == 0)
                                {
                                    FloorGrid[top, left].GroupID = segmentEast.GroupID;
                                    FloorGrid[top, left].RoomID = segmentEast.RoomID;
                                }
                                else
                                {
                                    FloorGrid[top, left].GroupID = segmentWest.GroupID;
                                    FloorGrid[top, left].RoomID = segmentWest.RoomID;
                                }
                            }
                        }
                    }
                }
            }
        }

        public List<FloorSegment> GetAdjacentSegments(int top, int left)
        {
            FloorSegment segmentNorth = IsInBounds(top - 1, left) ? (FloorGrid[top - 1, left] != null ? FloorGrid[top - 1, left] : null) : null;
            FloorSegment segmentSouth = IsInBounds(top + 1, left) ? (FloorGrid[top + 1, left] != null ? FloorGrid[top + 1, left] : null) : null;
            FloorSegment segmentEast = IsInBounds(top, left + 1) ? (FloorGrid[top, left + 1] != null ? FloorGrid[top, left + 1] : null) : null;
            FloorSegment segmentWest = IsInBounds(top, left - 1) ? (FloorGrid[top, left - 1] != null ? FloorGrid[top, left - 1] : null) : null;
            List<FloorSegment> result = new List<FloorSegment>();
            if (segmentNorth != null) if (segmentNorth.GroupID != FloorGrid[top, left].GroupID) result.Add(segmentNorth);
            if (segmentSouth != null) if (segmentSouth.GroupID != FloorGrid[top, left].GroupID) result.Add(segmentSouth);
            if (segmentWest != null) if (segmentWest.GroupID != FloorGrid[top, left].GroupID) result.Add(segmentWest);
            if (segmentEast != null) if (segmentEast.GroupID != FloorGrid[top, left].GroupID) result.Add(segmentEast);
            return result;
        }

        private DirectionFlags getRoomEdges(int top, int left)
        {
            bool isNullNorth = IsInBounds(top - 1, left) ? (FloorGrid[top - 1, left] == null ? true : false) : false;
            bool isNullSouth = IsInBounds(top + 1, left) ? (FloorGrid[top + 1, left] == null ? true : false) : false;
            bool isNullWest = IsInBounds(top, left + 1) ? (FloorGrid[top, left + 1] == null ? true : false) : false;
            bool isNullEast = IsInBounds(top, left - 1) ? (FloorGrid[top, left - 1] == null ? true : false) : false;
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
                for (int top = 0; top < length; top++)
                {
                    FloorSegment currFloorSegment = IsInBounds(top, left) ? (FloorGrid[top, left] != null ? FloorGrid[top, left] : null) : null;
                    if (currFloorSegment != null)
                    {
                        currFloorSegment.Doors.Clear();
                        currFloorSegment.ConnectedCorridorIDs.Clear();
                    }
                }
            }
            CorridorGroups.Clear();
        }

        private List<Direction> getCorridorDirections(int top, int left)
        {
            List<Direction> result = new List<Direction>();
            FloorSegment currSegment = IsInBounds(top, left) ? (FloorGrid[top, left] != null ? FloorGrid[top, left] : null) : null;
            FloorSegment segmentNorth = IsInBounds(top - 1, left) ? (FloorGrid[top - 1, left] != null ? FloorGrid[top - 1, left] : null) : null;
            FloorSegment segmentSouth = IsInBounds(top + 1, left) ? (FloorGrid[top + 1, left] != null ? FloorGrid[top + 1, left] : null) : null;
            FloorSegment segmentEast = IsInBounds(top, left + 1) ? (FloorGrid[top, left + 1] != null ? FloorGrid[top, left + 1] : null) : null;
            FloorSegment segmentWest = IsInBounds(top, left - 1) ? (FloorGrid[top, left - 1] != null ? FloorGrid[top, left - 1] : null) : null;
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

        private bool areCorridorsConnected(corridorSegment corridor1, corridorSegment corridor2)
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
            Rooms.Clear();
            for (int left = 0; left < width; left++)
            {
                for (int top = 0; top < length; top++)
                {
                    FloorSegment currSegment = IsInBounds(top, left) ? (FloorGrid[top, left] != null ? FloorGrid[top, left] : null) : null;
                    if (currSegment != null)
                    {
                        if (Rooms.ContainsKey(currSegment.RoomID))
                        {
                            Rooms[currSegment.RoomID].RoomSegments.Add(new SegmentPos() { Left = left, Top = top });
                        }
                        else
                        {
                            Rooms.Add(currSegment.RoomID, new Floor.Room()
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
            Floor.Room currRoom = Rooms[roomID];
            foreach (SegmentPos currPos in currRoom.RoomSegments)
            {
                FloorSegment currSeg = FloorGrid[currPos.Top, currPos.Left];
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
                for (int top = 0; top < length; top++)
                {
                    FloorSegment currSegment = IsInBounds(top, left) ? (FloorGrid[top, left] != null ? FloorGrid[top, left] : null) : null;
                    if (currSegment != null)
                    {
                        currSegment.ConnectedCorridorIDs.Clear();
                        currSegment.AdjacentCorridorID = null;
                        List<Direction> corridorDirections = getCorridorDirections(top, left);
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
                                if (currCorridor.Any(cc => areCorridorsConnected(cc, currCorridorSegment)))
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
                    FloorGrid[currSegment.Segment1.Top, currSegment.Segment1.Left].AdjacentCorridorID = currGroup.Name;
                    // Segment2 of the corridor
                    FloorGrid[currSegment.Segment2.Top, currSegment.Segment2.Left].AdjacentCorridorID = currGroup.Name;
                }
            }

        }

        public void AssignDoorways()
        {
            int numEligibleRooms = assignDoorway();
            while (numEligibleRooms > 0)
            {
                numEligibleRooms = assignDoorway();
            }
        }

        private int assignDoorway()
        {
            // Determine which rooms are large.
            RoomSizes.Clear();
            for (int left = 0; left < width; left++)
            {
                for (int top = 0; top < length; top++)
                {
                    FloorSegment currFloorSegment = IsInBounds(top, left) ? (FloorGrid[top, left] != null ? FloorGrid[top, left] : null) : null;
                    if (currFloorSegment != null)
                    {
                        // determine the size of the room by counting the number of tiles with this room number
                        if (!RoomSizes.ContainsKey(currFloorSegment.RoomID))
                        {
                            RoomSizes.Add(currFloorSegment.RoomID, 1);
                        }
                        else
                        {
                            RoomSizes[currFloorSegment.RoomID]++;
                        }
                    }
                }
            }

            // eligibleDoors is a dictionary where int is the room number and eligibledoor is a structure to describe a potential door position
            Dictionary<int, List<EligibleDoor>> eligibleDoors = new Dictionary<int, List<EligibleDoor>>();

            // We are iterating though all floor segments, adding to the eligibleDoors dictionary.
            for (int left = 0; left < width; left++)
            {
                for (int top = 0; top < length; top++)
                {
                    FloorSegment currFloorSegment = IsInBounds(top, left) ? (FloorGrid[top, left] != null ? FloorGrid[top, left] : null) : null;
                    if (currFloorSegment != null)
                    {
                        eligibleDoors = getEligibleDoors(top, left, eligibleDoors);
                    }
                }
            }

            // Pick one room to recieve a door.  If no more doors are possible, return
            int[] possibleRooms = eligibleDoors.Keys.ToArray();
            if (possibleRooms.Length == 0) return 0;

            int selectedRoomID = possibleRooms[rnd.Next(possibleRooms.Length)];

            EligibleDoor[] possibleDoors = eligibleDoors[selectedRoomID].ToArray();
            EligibleDoor selectedDoor = possibleDoors[rnd.Next(possibleDoors.Length)];
            FloorSegment originSegment = FloorGrid[selectedDoor.Top, selectedDoor.Left];
            if (selectedDoor.RoomID == selectedRoomID) // if the selected door room id is the same as the selected room, this indicates a door pointing into a corridor
            {
                // Place the door
                FloorGrid[selectedDoor.Top, selectedDoor.Left].Doors.Add(selectedDoor.doorDirection);
                // mark each segment of the room with the newly connected corridor
                foreach (SegmentPos currPos in Rooms[selectedRoomID].RoomSegments)
                {
                    FloorGrid[currPos.Top, currPos.Left].ConnectedCorridorIDs.Add(originSegment.AdjacentCorridorID);
                }
                HashSet<string> originSegmentHallways = originSegment.ConnectedCorridorIDs;
                for (int left = 0; left < width; left++)
                {
                    for (int top = 0; top < length; top++)
                    {
                        FloorSegment currFloorSegment = IsInBounds(top, left) ? (FloorGrid[top, left] != null ? FloorGrid[top, left] : null) : null;
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
                        FloorGrid[selectedDoor.Top - 1, selectedDoor.Left].Doors.Add(Direction.South);
                        connectingSegment = FloorGrid[selectedDoor.Top - 1, selectedDoor.Left];
                        break;
                    case Direction.South:
                        FloorGrid[selectedDoor.Top + 1, selectedDoor.Left].Doors.Add(Direction.North);
                        connectingSegment = FloorGrid[selectedDoor.Top + 1, selectedDoor.Left];
                        break;
                    case Direction.East:
                        FloorGrid[selectedDoor.Top, selectedDoor.Left + 1].Doors.Add(Direction.West);
                        connectingSegment = FloorGrid[selectedDoor.Top, selectedDoor.Left + 1];
                        break;
                    case Direction.West:
                        FloorGrid[selectedDoor.Top, selectedDoor.Left - 1].Doors.Add(Direction.East);
                        connectingSegment = FloorGrid[selectedDoor.Top, selectedDoor.Left - 1];
                        break;
                }

                // Join the hallways.  wooooo  
                foreach (SegmentPos currOriginPos in Rooms[originSegment.RoomID].RoomSegments)
                {
                    FloorGrid[currOriginPos.Top, currOriginPos.Left].ConnectedCorridorIDs.UnionWith(connectingSegment.ConnectedCorridorIDs);
                }
                foreach (SegmentPos currConnectingPos in Rooms[connectingSegment.RoomID].RoomSegments)
                {
                    FloorGrid[currConnectingPos.Top, currConnectingPos.Left].ConnectedCorridorIDs.UnionWith(originSegment.ConnectedCorridorIDs);
                }
                HashSet<string> originSegmentHallways = originSegment.ConnectedCorridorIDs;
                HashSet<string> connectingSegmentHallways = connectingSegment.ConnectedCorridorIDs;
                for (int left = 0; left < width; left++)
                {
                    for (int top = 0; top < length; top++)
                    {
                        FloorSegment currFloorSegment = IsInBounds(top, left) ? (FloorGrid[top, left] != null ? FloorGrid[top, left] : null) : null;
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

        private bool isValidForNewDoor(SegmentPos toCheck, SegmentPos checkAgainst)
        {
            bool result = true;
            // The door to the target room is valid if the target room is connected to a hallway that the origin room
            // is not yet connected to;
            FloorSegment originSegment = FloorGrid[toCheck.Top, toCheck.Left];
            FloorSegment targetSegment = FloorGrid[checkAgainst.Top, checkAgainst.Left];
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

        private bool isValidCorridorForNewDoor(int inTop, int inLeft)
        {
            // A corridor door is valid if the corridor it leads to is one to which it is not already attached.
            bool result = true;
            string adjacentCorridor = FloorGrid[inTop, inLeft].AdjacentCorridorID;
            HashSet<string> connectedCorridors = FloorGrid[inTop, inLeft].ConnectedCorridorIDs;
            if (connectedCorridors.Contains(adjacentCorridor))
            {
                result = false;
            }
            return result;
        }

        private Dictionary<int, List<EligibleDoor>> getEligibleDoors(int top, int left, Dictionary<int, List<EligibleDoor>> eligibleDoors)
        {
            FloorSegment currSegment = IsInBounds(top, left) ? (FloorGrid[top, left] != null ? FloorGrid[top, left] : null) : null;
            FloorSegment segmentNorth = IsInBounds(top - 1, left) ? (FloorGrid[top - 1, left] != null ? FloorGrid[top - 1, left] : null) : null;
            FloorSegment segmentSouth = IsInBounds(top + 1, left) ? (FloorGrid[top + 1, left] != null ? FloorGrid[top + 1, left] : null) : null;
            FloorSegment segmentEast = IsInBounds(top, left + 1) ? (FloorGrid[top, left + 1] != null ? FloorGrid[top, left + 1] : null) : null;
            FloorSegment segmentWest = IsInBounds(top, left - 1) ? (FloorGrid[top, left - 1] != null ? FloorGrid[top, left - 1] : null) : null;

            List<EligibleDoor> hallwayDoors = new List<EligibleDoor>();
            List<EligibleDoor> roomDoors = new List<EligibleDoor>();

            // Check all the floor segments around this one to determine if it is valid to place a door there.

            if (segmentNorth != null)
                if (segmentNorth.RoomID != currSegment.RoomID) // if the room id's are not the same it's a different room
                    if (segmentNorth.GroupID != currSegment.GroupID) // if the group id's are not the same its a different group
                    {
                        if (isValidCorridorForNewDoor(top, left))
                            hallwayDoors.Add(new EligibleDoor() { Top = top, Left = left, RoomID = currSegment.RoomID, doorDirection = Direction.North });
                    }
                    else
                    {
                        if (isValidForNewDoor(new SegmentPos() { Top = top, Left = left }, new SegmentPos() { Top = top - 1, Left = left }))
                            roomDoors.Add(new EligibleDoor() { Top = top - 1, Left = left, RoomID = segmentNorth.RoomID, doorDirection = Direction.South });
                    }
            if (segmentSouth != null)
                if (segmentSouth.RoomID != currSegment.RoomID)
                    if (segmentSouth.GroupID != currSegment.GroupID)
                    {
                        if (isValidCorridorForNewDoor(top, left))
                            hallwayDoors.Add(new EligibleDoor() { Top = top, Left = left, RoomID = currSegment.RoomID, doorDirection = Direction.South });
                    }
                    else
                    {
                        if (isValidForNewDoor(new SegmentPos() { Top = top, Left = left }, new SegmentPos() { Top = top + 1, Left = left }))
                            roomDoors.Add(new EligibleDoor() { Top = top + 1, Left = left, RoomID = segmentSouth.RoomID, doorDirection = Direction.North });
                    }
            if (segmentEast != null)
                if (segmentEast.RoomID != currSegment.RoomID)
                    if (segmentEast.GroupID != currSegment.GroupID)
                    {
                        if (isValidCorridorForNewDoor(top, left))
                            hallwayDoors.Add(new EligibleDoor() { Top = top, Left = left, RoomID = currSegment.RoomID, doorDirection = Direction.East });
                    }
                    else
                    {
                        if (isValidForNewDoor(new SegmentPos() { Top = top, Left = left }, new SegmentPos() { Top = top, Left = left + 1 }))
                            roomDoors.Add(new EligibleDoor() { Top = top, Left = left + 1, RoomID = segmentEast.RoomID, doorDirection = Direction.West });
                    }
            if (segmentWest != null)
                if (segmentWest.RoomID != currSegment.RoomID)
                    if (segmentWest.GroupID != currSegment.GroupID)
                    {
                        if (isValidCorridorForNewDoor(top, left))
                            hallwayDoors.Add(new EligibleDoor() { Top = top, Left = left, RoomID = currSegment.RoomID, doorDirection = Direction.West });
                    }
                    else
                    {
                        if (isValidForNewDoor(new SegmentPos() { Top = top, Left = left }, new SegmentPos() { Top = top, Left = left - 1 }))
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
                for (int top = 0; top < length; top++)
                {
                    int groupNumber = IsInBounds(top, left) ? (FloorGrid[top, left] != null ? FloorGrid[top, left].GroupID : -1) : -1;
                    FloorSegment currFloorSegment = IsInBounds(top, left) ? (FloorGrid[top, left] != null ? FloorGrid[top, left] : null) : null;
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
            SpanningRoomCount = edgeGroups.Count;
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
                        for (int top = 0; top < length; top++)
                        {
                            FloorSegment currFloorSegment = IsInBounds(top, left) ? (FloorGrid[top, left] != null ? FloorGrid[top, left] : null) : null;
                            if (currFloorSegment != null)
                            {
                                if (currFloorSegment.RoomID == selected.Key.RoomID)
                                {
                                    FloorGrid[top, left].GroupID = selected.Value.First<FloorSegment>().GroupID;
                                }
                            }
                        }
                    }
                }
            }
            removeIslandRooms();
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

        private void generateRoom(DirectionalSegment startSegment, bool largeRoom, bool isFootprint, int inFootprintLength, int inFootprintWidth)
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
                DirectionalSegment ns = startSegment.IncrementForwards(i);
                // if the next segment is free, we can do the next part of the room
                if (isValidSegment(ns.Top, ns.Left, isFootprint))
                {
                    // We've reached the end of the room length wise, add an eligible segment if there's room
                    if (i == roomLength)
                    {
                        FloorGrid[ns.Top, ns.Left] =
                            new FloorSegment()
                            {
                                RoomID = CurrentRoomId,
                                GroupID = thisGroupID
                            };
                        roomSuccess = true;

                        DirectionalSegment newStartSegment = ns.IncrementForwards(1);
                        if (isValidSegment(newStartSegment.Top, newStartSegment.Left, isFootprint)) EligibleFloorSegments.Enqueue(newStartSegment);
                        if (i == 0) // the first segment potentially has an eligible section behind it.
                        {
                            newStartSegment = ns.IncrementForwards(-1);
                            if (isValidSegment(newStartSegment.Top, newStartSegment.Left, isFootprint)) EligibleFloorSegments.Enqueue(newStartSegment);
                        }

                    }
                    else // place the floor segment
                    {
                        FloorGrid[ns.Top, ns.Left] =
                            new FloorSegment()
                            {
                                RoomID = CurrentRoomId,
                                GroupID = thisGroupID
                            };
                        roomSuccess = true;
                    }

                    if (pause) WaitEvent.WaitOne();
                    // Add the left section of the room
                    DirectionalSegment firstLeftSegment = ns.IncrementLeft(1);
                    for (int l = 0; l <= roomLeftSize; l++)
                    {
                        // We've reached the max size for the left part of the room
                        DirectionalSegment nextLeftSegment = firstLeftSegment.IncrementForwards(l);
                        if (l == roomLeftSize)
                        {
                            if (isValidSegment(nextLeftSegment.Top, nextLeftSegment.Left, isFootprint)) EligibleFloorSegments.Enqueue(nextLeftSegment);
                        }
                        else // place the floor segment if there's room
                        {
                            if (isValidSegment(nextLeftSegment.Top, nextLeftSegment.Left, isFootprint))
                            {
                                FloorGrid[nextLeftSegment.Top, nextLeftSegment.Left] =
                                    new FloorSegment()
                                    {
                                        RoomID = CurrentRoomId,
                                        GroupID = thisGroupID
                                    };

                                // If we're also at maximum room length, we add the new eligible segments 
                                if (i == roomLength)
                                {
                                    DirectionalSegment nextForwardsStartSegment = nextLeftSegment.IncrementRight(1);
                                    if (isValidSegment(nextForwardsStartSegment.Top, nextForwardsStartSegment.Left, isFootprint)) EligibleFloorSegments.Enqueue(nextForwardsStartSegment);
                                }
                                if (i == 0)  // On first row, place eligible segments behind if room
                                {
                                    DirectionalSegment nextForwardsStartSegment = nextLeftSegment.IncrementLeft(1);
                                    if (isValidSegment(nextForwardsStartSegment.Top, nextForwardsStartSegment.Left, isFootprint)) EligibleFloorSegments.Enqueue(nextForwardsStartSegment);
                                }
                            }
                            else
                            {
                                break; // We've collided moving in the left direction. Abort placing left sections
                            }
                        }
                        if (pause) WaitEvent.WaitOne();
                    }
                    // Add the right section of the room
                    DirectionalSegment firstRightSegment = ns.IncrementRight(1);
                    for (int r = 0; r <= roomRightSize; r++)
                    {
                        DirectionalSegment nextRightSegment = firstRightSegment.IncrementForwards(r);
                        // We've reached the max size for the left part of the room
                        if (r == roomRightSize)
                        {
                            if (isValidSegment(nextRightSegment.Top, nextRightSegment.Left, isFootprint)) EligibleFloorSegments.Enqueue(nextRightSegment);
                        }
                        else // place the segment if there's room
                        {
                            if (isValidSegment(nextRightSegment.Top, nextRightSegment.Left, isFootprint))
                            {
                                FloorGrid[nextRightSegment.Top, nextRightSegment.Left] =
                                    new FloorSegment()
                                    {
                                        RoomID = CurrentRoomId,
                                        GroupID = thisGroupID
                                    };

                                // If we're also at maximum room length, we add the new eligible segments 
                                if (i == roomLength)
                                {
                                    DirectionalSegment nextForwardsStartSegment = nextRightSegment.IncrementLeft(1);
                                    if (isValidSegment(nextForwardsStartSegment.Top, nextForwardsStartSegment.Left, isFootprint)) EligibleFloorSegments.Enqueue(nextForwardsStartSegment);
                                }
                                if (i == 0)  // On first row, place eligible segments behind if room
                                {
                                    DirectionalSegment nextForwardsStartSegment = nextRightSegment.IncrementRight(1);
                                    if (isValidSegment(nextForwardsStartSegment.Top, nextForwardsStartSegment.Left, isFootprint)) EligibleFloorSegments.Enqueue(nextForwardsStartSegment);
                                }
                            }
                            else
                            {
                                break; // We've collided moving in the left direction. Abort placing left sections
                            }
                        }
                        if (pause) WaitEvent.WaitOne();
                    }
                }
                else break;
            }
            if (roomSuccess)
            {
                roomCount++;
                CurrentRoomId++;
                currGroupRoomCount++;
                if (largeRoom) largeRoomCount++;
                if (currGroupRoomCount > currMaxRoomsPerGroup)
                {
                    currGroupRoomCount = 0;
                    currGroupID++;
                }

                //Rendering.Utilities.ConsoleUtil.log(string.Format("+-> Room {0} Generated", floorGrid));
            }
        }
    }
}
