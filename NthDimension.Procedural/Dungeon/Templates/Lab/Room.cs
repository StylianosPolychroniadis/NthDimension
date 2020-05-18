using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Dungeon.Templates.Lab
{
    using NthDimension;
    using NthDimension.Procedural.Dungeon;
    internal class DungeonRoom : LevelRootRoom
    {
        [Flags]
        internal enum RoomFlags
        {
            Evil = 1,

            ConnectionMask = 6,
            Conn_Floor = 0,
            Conn_None = 2,
            Conn_Destructible = 4
        }

        struct RoomTemplate
        {
            public readonly Rect Bounds;
            public readonly Range NumBranches;
            public readonly RoomFlags Flags;
            public readonly Tuple<Direction, int>[] Connections;

            public RoomTemplate(Rect bounds, Range numBranches, RoomFlags flags, params Tuple<Direction, int>[] connections)
            {
                Bounds = bounds;
                Flags = flags;
                NumBranches = numBranches;
                Connections = connections;
            }
        }

        static Rect Rect(int x, int y, int w, int h)
        {
            return new Rect(x, y, x + w, y + h);
        }

        #region Templates

        static readonly RoomTemplate[] roomTemplates = {
            new RoomTemplate(Rect(24, 0, 46, 44),
                new Range(1, 4), RoomFlags.Conn_Floor,
                Tuple.Create(Direction.North, 11),
                Tuple.Create(Direction.South, 11),
                Tuple.Create(Direction.East, 10),
                Tuple.Create(Direction.West, 10)
                ),
            new RoomTemplate(Rect(50, 0, 36, 32),
                new Range(2, 2), RoomFlags.Conn_Destructible,
                Tuple.Create(Direction.East, 1),
                Tuple.Create(Direction.West, 7)
                ),
            new RoomTemplate(Rect(66, 0, 45, 32),
                new Range(1, 2), RoomFlags.Conn_Floor,
                Tuple.Create(Direction.North, 4),
                Tuple.Create(Direction.North, 17),
                Tuple.Create(Direction.South, 4),
                Tuple.Create(Direction.South, 17),
                Tuple.Create(Direction.East, 4),
                Tuple.Create(Direction.West, 4)
                ),
            new RoomTemplate(Rect(24, 24, 41, 40),
                new Range(1, 4), RoomFlags.Conn_Floor,
                Tuple.Create(Direction.North, 8),
                Tuple.Create(Direction.South, 9),
                Tuple.Create(Direction.East, 8),
                Tuple.Create(Direction.West, 9)
                ),
            new RoomTemplate(Rect(50, 12, 38, 47),
                new Range(1, 2), RoomFlags.Conn_Destructible,
                Tuple.Create(Direction.North, 7),
                Tuple.Create(Direction.South, 7)
                ),
            new RoomTemplate(Rect(68, 12, 42, 51),
                new Range(2, 3), RoomFlags.Conn_Floor,
                Tuple.Create(Direction.North, 4),
                Tuple.Create(Direction.South, 4),
                Tuple.Create(Direction.East, 13)
                ),
            new RoomTemplate(Rect(0, 50, 60, 42),
                new Range(1, 2), RoomFlags.Conn_Floor,
                Tuple.Create(Direction.East, 9),
                Tuple.Create(Direction.West, 9)
                ),
            new RoomTemplate(Rect(40, 44, 45, 45),
                new Range(2, 4), RoomFlags.Conn_Floor,
                Tuple.Create(Direction.North, 4),
                Tuple.Create(Direction.North, 17),
                Tuple.Create(Direction.South, 4),
                Tuple.Create(Direction.South, 17),
                Tuple.Create(Direction.East, 4),
                Tuple.Create(Direction.East, 17),
                Tuple.Create(Direction.West, 4),
                Tuple.Create(Direction.West, 17)
                ),
            new RoomTemplate(Rect(65, 43, 52, 53),
                new Range(1, 3), RoomFlags.Conn_Floor,
                Tuple.Create(Direction.South, 14),
                Tuple.Create(Direction.East, 6),
                Tuple.Create(Direction.West, 6)
                ),
            new RoomTemplate(Rect(0, 72, 44, 44),
                new Range(1, 2), RoomFlags.Conn_Floor,
                Tuple.Create(Direction.South, 13),
                Tuple.Create(Direction.West, 6)
                ),
            new RoomTemplate(Rect(24, 72, 52, 39),
                new Range(1, 3), RoomFlags.Conn_Floor,
                Tuple.Create(Direction.North, 2),
                Tuple.Create(Direction.South, 5),
                Tuple.Create(Direction.East, 14)
                ),
            new RoomTemplate(Rect(46, 69, 62, 70),
                new Range(2, 2), RoomFlags.Evil | RoomFlags.Conn_Destructible,
                Tuple.Create(Direction.North, 19),
                Tuple.Create(Direction.South, 19)
                ),
            new RoomTemplate(Rect(0, 128, 51, 51),
                new Range(2, 4), RoomFlags.Conn_Floor,
                Tuple.Create(Direction.North, 13),
                Tuple.Create(Direction.South, 13),
                Tuple.Create(Direction.East, 13),
                Tuple.Create(Direction.West, 13)
                ),
            new RoomTemplate(Rect(31, 119, 41, 52),
                new Range(1, 2), RoomFlags.Conn_Destructible,
                Tuple.Create(Direction.North, 15),
                Tuple.Create(Direction.East, 26)
                ),
            new RoomTemplate(Rect(52, 119, 55, 32),
                new Range(1, 2), RoomFlags.Conn_Floor,
                Tuple.Create(Direction.North, 4),
                Tuple.Create(Direction.North, 17),
                Tuple.Create(Direction.South, 4),
                Tuple.Create(Direction.South, 17),
                Tuple.Create(Direction.East, 4),
                Tuple.Create(Direction.West, 4)
                ),
            new RoomTemplate(Rect(77, 119, 50, 33),
                new Range(1, 2), RoomFlags.Conn_Destructible,
                Tuple.Create(Direction.East, 5),
                Tuple.Create(Direction.West, 5)
                ),
            new RoomTemplate(Rect(52, 132, 48, 40),
                new Range(1, 3), RoomFlags.Conn_Floor,
                Tuple.Create(Direction.North, 3),
                Tuple.Create(Direction.South, 3),
                Tuple.Create(Direction.East, 8)
                ),
            new RoomTemplate(Rect(0, 159, 48, 52),
                new Range(2, 2), RoomFlags.Evil | RoomFlags.Conn_Destructible,
                Tuple.Create(Direction.South, 4),
                Tuple.Create(Direction.West, 3)
                ),
            new RoomTemplate(Rect(32, 152, 52, 41),
                new Range(1, 2), RoomFlags.Conn_Floor,
                Tuple.Create(Direction.North, 14),
                Tuple.Create(Direction.South, 14)
                ),
            new RoomTemplate(Rect(30, 173, 45, 44),
                new Range(1, 2), RoomFlags.Conn_Floor,
                Tuple.Create(Direction.East, 10),
                Tuple.Create(Direction.West, 10)
                ),
            new RoomTemplate(Rect(65, 152, 41, 49),
                new Range(2, 2), RoomFlags.Evil | RoomFlags.Conn_Destructible,
                Tuple.Create(Direction.North, 8),
                Tuple.Create(Direction.South, 8)
                )
        };

        #endregion

        readonly int currentId;
        RoomTemplate currentTemplate;

        static readonly LevelObject destWall = new LevelObject
        {
            ObjectType = LabTemplate.DestructibleWall
        };

        public DungeonRoom(DungeonRoom prev, Random rand, bool noEvil)
        {
            var indexes = Enumerable.Range(0, roomTemplates.Length).ToList();
            rand.Shuffle(indexes);
            foreach (var index in indexes)
            {
                if (prev != null && index == prev.currentId)
                    continue;

                if ((roomTemplates[index].Flags & RoomFlags.Evil) != 0 && noEvil)
                    continue;

                if (prev != null)
                {
                    bool ok = false;
                    foreach (var conn in prev.ConnectionPoints)
                    {
                        var d = conn.Item1.Reverse();
                        if (roomTemplates[index].Connections.Any(targetConn => targetConn.Item1 == d))
                        {
                            ok = true;
                            break;
                        }
                    }
                    if (!ok)
                        continue;
                }

                currentId = index;
            }
            currentTemplate = roomTemplates[currentId];
        }

        public override LevelRoomType Type { get { return LevelRoomType.Normal; } }

        public override int Width { get { return currentTemplate.Bounds.MaxX - currentTemplate.Bounds.X; } }

        public override int Length { get { return currentTemplate.Bounds.MaxY - currentTemplate.Bounds.Y; } }

        public override Tuple<Direction, int>[] ConnectionPoints { get { return currentTemplate.Connections; } }

        public override Range NumBranches { get { return currentTemplate.NumBranches; } }

        public RoomFlags Flags { get { return currentTemplate.Flags; } }

        public override void Rasterize(BitmapRasterizer<LevelTile> rasterizer, Random rand)
        {
            rasterizer.Copy(LabTemplate.MapTemplate, currentTemplate.Bounds, Pos);

            if ((currentTemplate.Flags & RoomFlags.Evil) == 0)
                LabTemplate.CreateEnemies(rasterizer, Bounds, rand);

            var flags = currentTemplate.Flags & RoomFlags.ConnectionMask;
            LevelTile? tile = null;
            switch (flags)
            {
                case RoomFlags.Conn_Floor:
                    tile = new LevelTile
                    {
                        TileType = LabTemplate.LabFloor
                    };
                    break;

                case RoomFlags.Conn_Destructible:
                    tile = new LevelTile
                    {
                        TileType = LabTemplate.LabFloor,
                        Object = destWall
                    };
                    break;
                default:
                    return;
            }

            foreach (var edge in Edges)
            {
                var direction = edge.Linkage.Direction;
                if (edge.RoomA != this)
                    direction = direction.Reverse();

                Point a, b;
                switch (direction)
                {
                    case Direction.South:
                        a = new Point(edge.Linkage.Offset, Pos.Y + Length - 1);
                        b = new Point(a.X + 3, a.Y);
                        break;

                    case Direction.North:
                        a = new Point(edge.Linkage.Offset, Pos.Y);
                        b = new Point(a.X + 3, a.Y);
                        break;

                    case Direction.East:
                        a = new Point(Pos.X + Width - 1, edge.Linkage.Offset);
                        b = new Point(a.X, a.Y + 3);
                        break;

                    case Direction.West:
                        a = new Point(Pos.X, edge.Linkage.Offset);
                        b = new Point(a.X, a.Y + 3);
                        break;

                    default:
                        throw new ArgumentException();
                }
                if(null != a && null != b)
                    rasterizer.DrawLine(a, b, tile.Value);
            }

            LabTemplate.DrawSpiderWeb(rasterizer, Bounds, rand);
        }

        public override void Rasterize(Rendering.Scenegraph.SceneGame scene, Random rand)
        {
            throw new NotImplementedException();
        }
    }
}
