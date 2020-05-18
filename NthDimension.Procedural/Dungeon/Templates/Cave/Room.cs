using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Dungeon.Templates.Cave
{
    using NthDimension.Procedural.Dungeon;
    internal class Room : LevelRoom
    {
        readonly int w;
        readonly int h;

        public Room(int w, int h)
        {
            this.w = w;
            this.h = h;
        }

        public override LevelRoomType Type { get { return LevelRoomType.Normal; } }

        public override int Width { get { return w; } }

        public override int Length { get { return h; } }

        public override void Rasterize(BitmapRasterizer<LevelTile> rasterizer, Random rand)
        {
            rasterizer.FillRect(Bounds, new LevelTile
            {
                TileType = CaveTemplate.BrownLines
            });

            int numBoss = new Range(0, 1).Random(rand);
            int numMinion = new Range(3, 5).Random(rand);
            int numPet = new Range(0, 2).Random(rand);

            var buf = rasterizer.Bitmap;
            var bounds = Bounds;
            while (numBoss > 0 || numMinion > 0 || numPet > 0)
            {
                int x = rand.Next(bounds.X, bounds.MaxX);
                int y = rand.Next(bounds.Y, bounds.MaxY);
                if (buf[x, y].Object != null)
                    continue;

                switch (rand.Next(3))
                {
                    case 0:
                        if (numBoss > 0)
                        {
                            buf[x, y].Object = new LevelObject
                            {
                                ObjectType = CaveTemplate.Boss[rand.Next(CaveTemplate.Boss.Length)]
                            };
                            numBoss--;
                        }
                        break;
                    case 1:
                        if (numMinion > 0)
                        {
                            buf[x, y].Object = new LevelObject
                            {
                                ObjectType = CaveTemplate.Minion[rand.Next(CaveTemplate.Minion.Length)]
                            };
                            numMinion--;
                        }
                        break;
                    case 2:
                        if (numPet > 0)
                        {
                            buf[x, y].Object = new LevelObject
                            {
                                ObjectType = CaveTemplate.Pet[rand.Next(CaveTemplate.Pet.Length)]
                            };
                            numPet--;
                        }
                        break;
                }
            }
        }

        public override void Rasterize(Rendering.Scenegraph.SceneGame scene, Random rand)
        {
            throw new NotImplementedException();
        }
    }
}
