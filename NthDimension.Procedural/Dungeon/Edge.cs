using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Dungeon
{
    public class Edge
    {
        Edge()
        {
        }

        public LevelRoom RoomA { get; private set; }
        public LevelRoom RoomB { get; private set; }
        public Link Linkage { get; set; }

        public static void Link(LevelRoom a, LevelRoom b, Link link)
        {
#if DEBUG
            //Debug.Assert(a != b);
#endif
            var edge = new Edge
            {
                RoomA = a,
                RoomB = b,
                Linkage = link
            };
            a.Edges.Add(edge);
            b.Edges.Add(edge);
        }

        public static void UnLink(LevelRoom a, LevelRoom b)
        {
            Edge edge = null;
            foreach (var ed in a.Edges)
                if (ed.RoomA == b || ed.RoomB == b)
                {
                    edge = ed;
                    break;
                }
            if (edge == null)
                throw new ArgumentException();
            a.Edges.Remove(edge);
            b.Edges.Remove(edge);
        }
    }
}
