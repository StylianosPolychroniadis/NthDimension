using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Dungeon
{
    public class LevelGraph
    {
        public LevelMapTemplate Template { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public LevelRoom[] Rooms { get; private set; }

        internal LevelGraph(LevelMapTemplate template, LevelRoom[] rooms)
        {
            Template = template;

            int dx = int.MaxValue, dy = int.MaxValue;
            int mx = int.MinValue, my = int.MinValue;

            for (int i = 0; i < rooms.Length; i++)
            {
                var bounds = rooms[i].Bounds;

                if (bounds.X < dx)
                    dx = bounds.X;
                if (bounds.Y < dy)
                    dy = bounds.Y;

                if (bounds.MaxX > mx)
                    mx = bounds.MaxX;
                if (bounds.MaxY > my)
                    my = bounds.MaxY;
            }

            const int Pad = 4;

            Width = mx - dx + Pad * 2;
            Height = my - dy + Pad * 2;

            for (int i = 0; i < rooms.Length; i++)
            {
                var room = rooms[i];
                var pos = room.Pos;
                room.Pos = new Point(pos.X - dx + Pad, pos.Y - dy + Pad);

                foreach (var edge in room.Edges)
                {
                    if (edge.RoomA != room)
                        continue;
                    if (edge.Linkage.Direction == Direction.South || edge.Linkage.Direction == Direction.North)
                        edge.Linkage = new Link(edge.Linkage.Direction, edge.Linkage.Offset - dx + Pad);
                    else if (edge.Linkage.Direction == Direction.East || edge.Linkage.Direction == Direction.West)
                        edge.Linkage = new Link(edge.Linkage.Direction, edge.Linkage.Offset - dy + Pad);
                }
            }
            Rooms = rooms;
        }
    }
}
