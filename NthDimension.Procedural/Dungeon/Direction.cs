using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Dungeon
{
    public enum Direction
    {
        South       = 0,
        East        = 1,
        North       = 2,
        West        = 3
    }

    public static class DirectionExtensions
    {
        public static Direction Reverse(this Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return Direction.South;
                case Direction.South:
                    return Direction.North;
                case Direction.East:
                    return Direction.West;
                case Direction.West:
                    return Direction.East;
            }
            throw new ArgumentException();
        }
    }
}
