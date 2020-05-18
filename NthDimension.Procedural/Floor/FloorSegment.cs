using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Floor
{
    public class FloorSegment
    {
        public int                  RoomID;
        public int                  GroupID;
        public string               AdjacentCorridorID;
        public HashSet<string>      ConnectedCorridorIDs        = new HashSet<string>();
        public List<Direction>      Doors                       = new List<Direction>();
    }
}
