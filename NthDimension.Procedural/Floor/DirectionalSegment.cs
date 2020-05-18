using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Floor
{
    public class DirectionalSegment
    {
        public int Left;
        public int Top;
        public Direction Direction;
        public int GroupID;

        public DirectionalSegment(int inTop, int inLeft, Direction inDirection, int inGroupID)
        {
            Left = inLeft;
            Top = inTop;
            Direction = inDirection;
            GroupID = inGroupID;
        }

        public DirectionalSegment IncrementForwards(int amount)
        {
            DirectionalSegment result = new DirectionalSegment(Top, Left, Direction, GroupID);
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

        public DirectionalSegment IncrementLeft(int amount)
        {
            DirectionalSegment result = new DirectionalSegment(Top, Left, Direction.None, GroupID);
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

        public DirectionalSegment IncrementRight(int amount)
        {
            DirectionalSegment result = new DirectionalSegment(Top, Left, Direction.None, GroupID);
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
