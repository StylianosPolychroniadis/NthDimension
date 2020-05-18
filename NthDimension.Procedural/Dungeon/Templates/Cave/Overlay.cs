using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NthDimension.Procedural.Dungeon;

namespace NthDimension.Procedural.Dungeon.Templates.Cave
{
    internal class Overlay : LevelMapRender
    {
        public override void Rasterize()
        {
            var wall = new LevelTile
            {
                TileType = CaveTemplate.Composite,
                Object = new LevelObject
                {
                    ObjectType = CaveTemplate.CaveWall
                }
            };
            var water = new LevelTile
            {
                TileType = CaveTemplate.ShallowWater
            };
            var space = new LevelTile
            {
                TileType = CaveTemplate.Space
            };

            int w = Rasterizer.Width, h = Rasterizer.Height;
            var buf = Rasterizer.Bitmap;
            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                {
                    if (buf[x, y].TileType != CaveTemplate.ShallowWater)
                        continue;

                    bool notWall = false;
                    if (x == 0 || y == 0 || x + 1 == w || y + 1 == h)
                        notWall = false;
                    else if (buf[x + 1, y].TileType == CaveTemplate.BrownLines ||
                             buf[x - 1, y].TileType == CaveTemplate.BrownLines ||
                             buf[x, y + 1].TileType == CaveTemplate.BrownLines ||
                             buf[x, y - 1].TileType == CaveTemplate.BrownLines)
                    {
                        notWall = true;
                    }
                    if (!notWall)
                        buf[x, y] = wall;
                }

            var tmp = (LevelTile[,])buf.Clone();
            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                {
                    if (buf[x, y].TileType != CaveTemplate.Composite)
                        continue;

                    bool nearWater = false;
                    if (x == 0 || y == 0 || x + 1 == w || y + 1 == h)
                        nearWater = false;
                    else if (tmp[x + 1, y].TileType == CaveTemplate.ShallowWater ||
                             tmp[x - 1, y].TileType == CaveTemplate.ShallowWater ||
                             tmp[x, y + 1].TileType == CaveTemplate.ShallowWater ||
                             tmp[x, y - 1].TileType == CaveTemplate.ShallowWater)
                    {
                        nearWater = true;
                    }
                    if (nearWater && Rand.NextDouble() > 0.4)
                        buf[x, y] = water;
                }

            tmp = (LevelTile[,])buf.Clone();
            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                {
                    if (buf[x, y].TileType != CaveTemplate.Composite)
                        continue;

                    bool allWall = false;
                    if (x == 0 || y == 0 || x + 1 == w || y + 1 == h)
                        allWall = true;
                    else
                    {
                        allWall = true;
                        for (int dx = -1; dx <= 1 && allWall; dx++)
                            for (int dy = -1; dy <= 1 && allWall; dy++)
                            {
                                if (tmp[x + dx, y + dy].TileType != CaveTemplate.Composite)
                                {
                                    allWall = false;
                                    break;
                                }
                            }
                    }
                    if (allWall)
                        buf[x, y] = space;
                }
        }
    }
}
