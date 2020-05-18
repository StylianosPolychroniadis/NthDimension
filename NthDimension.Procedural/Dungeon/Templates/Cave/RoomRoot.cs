using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Dungeon.Templates.Cave
{
    using NthDimension.Procedural.Dungeon;
    internal class RoomRoot : LevelRoom
    {
        readonly int radius;

        public RoomRoot(int radius)
        {
            this.radius = radius;
        }

        public override LevelRoomType Type { get { return LevelRoomType.Start; } }

        public override int Width { get { return radius * 2 + 1; } }

        public override int Length { get { return radius * 2 + 1; } }

        public override void Rasterize(BitmapRasterizer<LevelTile> rasterizer, Random rand)
        {
            var tile = new LevelTile
            {
                TileType = CaveTemplate.LightSand
            };

            var cX = Pos.X + radius + 0.5;
            var cY = Pos.Y + radius + 0.5;
            var bounds = Bounds;
            var r2 = radius * radius;
            var buf = rasterizer.Bitmap;

            double pR = rand.NextDouble() * (radius - 2), pA = rand.NextDouble() * 2 * Math.PI;
            int pX = (int)(cX + Math.Cos(pR) * pR);
            int pY = (int)(cY + Math.Sin(pR) * pR);

            for (int x = bounds.X; x < bounds.MaxX; x++)
                for (int y = bounds.Y; y < bounds.MaxY; y++)
                {
                    if ((x - cX) * (x - cX) + (y - cY) * (y - cY) <= r2)
                    {
                        buf[x, y] = tile;
                        if (rand.NextDouble() > 0.95)
                        {
                            buf[x, y].Object = new LevelObject
                            {
                                ObjectType = CaveTemplate.PalmTree
                            };
                        }
                    }
                    if (x == pX && y == pY)
                    {
                        buf[x, y].Region = "Spawn";
                        buf[x, y].Object = new LevelObject
                        {
                            ObjectType = CaveTemplate.CowardicePortal
                        };
                    }
                }
        }

        public override void Rasterize(Rendering.Scenegraph.SceneGame scene, Random rand)
        {
            throw new NotImplementedException();
        }
    }
}
