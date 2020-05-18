using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.FloorPlan
{

    class FloorplanDirectionalSegment
    {
        public int Left;
        public int Top;
        public Direction Direction;
        public int GroupID;

        public FloorplanDirectionalSegment(int inTop, int inLeft, Direction inDirection, int inGroupID)
        {
            Left = inLeft;
            Top = inTop;
            Direction = inDirection;
            GroupID = inGroupID;
        }

        public FloorplanDirectionalSegment IncrementForwards(int amount)
        {
            FloorplanDirectionalSegment result = new FloorplanDirectionalSegment(Top, Left, Direction, GroupID);
            switch (Direction)
            {
                case Direction.North:
                    result.Top -= amount;
                    break;
                case Direction.East:
                    result.Left += amount;
                    break;
                case Direction.South:
                    result.Top += amount;
                    break;
                case Direction.West:
                    result.Left -= amount;
                    break;
            }
            return result;
        }

        public FloorplanDirectionalSegment IncrementLeft(int amount)
        {
            FloorplanDirectionalSegment result = new FloorplanDirectionalSegment(Top, Left, Direction.None, GroupID);
            switch (Direction)
            {
                case Direction.North:
                    result.Direction = Direction.West;
                    result.Left -= amount;
                    break;
                case Direction.East:
                    result.Direction = Direction.North;
                    result.Top -= amount;
                    break;
                case Direction.South:
                    result.Direction = Direction.East;
                    result.Left += amount;
                    break;
                case Direction.West:
                    result.Direction = Direction.South;
                    result.Top += amount;
                    break;
            }
            return result;
        }

        public FloorplanDirectionalSegment IncrementRight(int amount)
        {
            FloorplanDirectionalSegment result = new FloorplanDirectionalSegment(Top, Left, Direction.None, GroupID);
            switch (Direction)
            {
                case Direction.North:
                    result.Direction = Direction.East;
                    result.Left += amount;
                    break;
                case Direction.East:
                    result.Direction = Direction.South;
                    result.Top += amount;
                    break;
                case Direction.South:
                    result.Direction = Direction.West;
                    result.Left -= amount;
                    break;
                case Direction.West:
                    result.Direction = Direction.North;
                    result.Top--;
                    break;
            }
            return result;
        }
    }
}
