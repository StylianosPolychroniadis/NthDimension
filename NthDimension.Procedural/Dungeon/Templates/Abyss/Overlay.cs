using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NthDimension.Procedural.Dungeon;

namespace NthDimension.Procedural.Dungeon.Templates.Abyss
{
    internal class Overlay : LevelMapRender
    {
        static readonly LevelObject floor = new LevelObject
        {
            ObjectType = AbyssTemplate.PartialRedFloor
        };

        static readonly LevelObject broken = new LevelObject
        {
            ObjectType = AbyssTemplate.BrokenRedPillar
        };

        byte[,] GenerateHeightMap(int w, int h)
        {
            float[,] map = new float[w, h];
            int maxR = Math.Min(w, h);
            int r = Rand.Next(maxR * 1 / 3, maxR * 2 / 3);
            int r2 = r * r;

            for (int i = 0; i < 200; i++)
            {
                int cx = Rand.Next(w), cy = Rand.Next(h);
                float fact = (float)Rand.NextDouble() * 3 + 1;
                if (Rand.Next() % 2 == 0)
                    fact = 1 / fact;

                for (int x = 0; x < w; x++)
                    for (int y = 0; y < h; y++)
                    {
                        var z = r2 - ((x - cx) * (x - cx) / fact + (y - cy) * (y - cy) * fact);
                        if (z < 0)
                            continue;
                        map[x, y] += z / r2;
                    }
            }

            float max = 0;
            float min = float.MaxValue;
            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                {
                    if (map[x, y] > max)
                        max = map[x, y];
                    else if (map[x, y] < min)
                        min = map[x, y];
                }

            byte[,] norm = new byte[w, h];
            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                {
                    float normVal = (map[x, y] - min) / (max - min);
                    norm[x, y] = (byte)(normVal * normVal * byte.MaxValue);
                }
            return norm;
        }

        static int Lerp(int a, int b, float val)
        {
            return a + (int)((b - a) * val);
        }

        void RenderBackground()
        {
            const int Sample = 4;

            int w = Rasterizer.Width, h = Rasterizer.Height;
            var buf = Rasterizer.Bitmap;
            var hm = GenerateHeightMap(w / Sample + 2, h / Sample + 2);

            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                {
                    if (buf[x, y].TileType == AbyssTemplate.Lava ||
                        buf[x, y].TileType == AbyssTemplate.Space)
                        continue;

                    if (buf[x, y].Region == "Treasure")
                    {
                        buf[x, y].Region = null;
                        continue;
                    }

                    int dx = x / Sample, dy = y / Sample;
                    var hx1 = Lerp(hm[dx, dy], hm[dx + 1, dy], (x % Sample) / (float)Sample);
                    var hx2 = Lerp(hm[dx, dy + 1], hm[dx + 1, dy + 1], (x % Sample) / (float)Sample);
                    var hv = Lerp(hx1, hx2, (y % Sample) / (float)Sample);

                    if ((hv / 10) % 2 == 0)
                    {
                        buf[x, y].TileType = AbyssTemplate.Lava;
                        if (Rand.NextDouble() > 0.9 && buf[x, y].Object == null)
                            buf[x, y].Object = floor;
                    }
                }
        }

        void RenderSafeGround()
        {
            RoomRoot startRm = null;
            foreach (var room in Graph.Rooms)
                if (room is RoomRoot)
                {
                    startRm = (RoomRoot)room;
                    break;
                }

            if (startRm == null)
                return;

            var buf = Rasterizer.Bitmap;
            var pos = startRm.portalPos;
            for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                {
                    var tile = buf[pos.X + dx, pos.Y + dy];
                    if (tile.TileType == AbyssTemplate.Lava)
                    {
                        tile.TileType = AbyssTemplate.RedSmallChecks;
                        if (tile.Object != null && tile.Object.ObjectType != AbyssTemplate.CowardicePortal)
                            tile.Object = null;
                    }
                    buf[pos.X + dx, pos.Y + dy] = tile;
                }
        }

        void RenderLavaGround(Point a, Point b)
        {
            Rasterizer.DrawLine(a, b, (x, y) => {
                if (Rasterizer.Bitmap[x, y].TileType == AbyssTemplate.Lava)
                    return new LevelTile
                    {
                        TileType = AbyssTemplate.Lava,
                        Object = floor
                    };
                return Rasterizer.Bitmap[x, y];
            }, 1);
        }

        void RenderBossEdge(LevelRoom src, LevelRoom dst, Direction direction, int offset)
        {
            switch (direction)
            {
                case Direction.North:
                    RenderLavaGround(
                        new Point(offset, dst.Pos.Y + 35),
                        new Point(offset, src.Pos.Y));
                    break;

                case Direction.South:
                    RenderLavaGround(
                        new Point(offset, src.Pos.Y + src.Length),
                        new Point(offset, dst.Pos.Y + 10));
                    break;

                case Direction.West:
                    RenderLavaGround(
                        new Point(dst.Pos.X + 35, offset),
                        new Point(src.Pos.X, offset));
                    break;

                case Direction.East:
                    RenderLavaGround(
                        new Point(src.Pos.X + src.Width, offset),
                        new Point(dst.Pos.X + 10, offset));
                    break;

                default:
                    throw new ArgumentException();
            }
        }

        void RenderConnection()
        {
            foreach (var room in Graph.Rooms)
            {
                var xRange = new Range(room.Width * 1 / 4, room.Width * 3 / 4);
                var yRange = new Range(room.Length * 1 / 4, room.Length * 3 / 4);
                var pt = new Point(room.Pos.X + xRange.Random(Rand), room.Pos.Y + yRange.Random(Rand));

                foreach (var edge in room.Edges)
                {
                    var direction = edge.Linkage.Direction;
                    var randOffset = edge.Linkage.Offset + edge.Linkage.Offset % 3;

                    if (edge.RoomA != room)
                        direction = direction.Reverse();
                    else if (edge.RoomB is RoomBoss)
                    {
                        RenderBossEdge(edge.RoomA, edge.RoomB, direction, randOffset);
                    }
                    else if (edge.RoomA is RoomBoss)
                    {
                        RenderBossEdge(edge.RoomB, edge.RoomA, direction.Reverse(), randOffset);
                    }

                    if (room is RoomBoss)
                        continue;

                    Point pos;
                    switch (direction)
                    {
                        case Direction.North:
                            pos = new Point(randOffset, room.Pos.Y);
                            break;

                        case Direction.South:
                            pos = new Point(randOffset, room.Pos.Y + room.Length);
                            break;

                        case Direction.West:
                            pos = new Point(room.Pos.X, randOffset);
                            break;

                        case Direction.East:
                            pos = new Point(room.Pos.X + room.Width, randOffset);
                            break;

                        default:
                            throw new ArgumentException();
                    }
                    RenderLavaGround(pos, pt);
                }

                if (room is RoomRoot)
                    RenderLavaGround(((RoomRoot)room).portalPos, pt);
            }
        }

        void RenderPillars()
        {
            var buf = Rasterizer.Bitmap;
            int w = Rasterizer.Width, h = Rasterizer.Height;
            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                {
                    if (buf[x, y].Object != null && buf[x, y].Object.ObjectType == AbyssTemplate.RedPillar &&
                        Rand.NextDouble() > 0.7)
                        buf[x, y].Object = broken;
                }
        }

        void RenderWalls()
        {
            var wallA = new LevelTile
            {
                TileType = AbyssTemplate.Space,
                Object = new LevelObject
                {
                    ObjectType = AbyssTemplate.RedWall
                }
            };
            var wallB = new LevelTile
            {
                TileType = AbyssTemplate.Space,
                Object = new LevelObject
                {
                    ObjectType = AbyssTemplate.RedTorchWall
                }
            };

            var buf = Rasterizer.Bitmap;
            var tmp = (LevelTile[,])buf.Clone();
            int w = Rasterizer.Width, h = Rasterizer.Height;

            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                {
                    if (buf[x, y].TileType != AbyssTemplate.Space)
                        continue;

                    bool notWall = true;
                    if (x == 0 || y == 0 || x + 1 == w || y + 1 == h)
                        notWall = true;
                    else
                    {
                        for (int dx = -1; dx <= 1 && notWall; dx++)
                            for (int dy = -1; dy <= 1 && notWall; dy++)
                            {
                                if (tmp[x + dx, y + dy].TileType != AbyssTemplate.Space)
                                {
                                    notWall = false;
                                    break;
                                }
                            }
                    }
                    if (!notWall)
                        buf[x, y] = Rand.NextDouble() < 0.9 ? wallA : wallB;
                }
        }

        public override void Rasterize()
        {
            RenderBackground();
            RenderSafeGround();
            RenderConnection();
            RenderPillars();
            RenderWalls();
        }
    }
}
