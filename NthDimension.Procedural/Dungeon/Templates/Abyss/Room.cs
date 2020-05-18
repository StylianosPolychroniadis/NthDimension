using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NthDimension.Procedural.Dungeon;
using NthDimension.Rendering.Scenegraph;

namespace NthDimension.Procedural.Dungeon.Templates.Abyss
{
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
                TileType = AbyssTemplate.RedSmallChecks
            });


            int numImp = new Range(0, 2).Random(rand);
            int numDemon = new Range(2, 4).Random(rand);
            int numBrute = new Range(1, 4).Random(rand);
            int numSkull = new Range(1, 3).Random(rand);

            var buf = rasterizer.Bitmap;
            var bounds = Bounds;
            while (numImp > 0 || numDemon > 0 || numBrute > 0 || numSkull > 0)
            {
                int x = rand.Next(bounds.X, bounds.MaxX);
                int y = rand.Next(bounds.Y, bounds.MaxY);
                if (buf[x, y].Object != null)
                    continue;

                switch (rand.Next(4))
                {
                    case 0:
                        if (numImp > 0)
                        {
                            buf[x, y].Object = new LevelObject
                            {
                                ObjectType = AbyssTemplate.AbyssImp
                            };
                            numImp--;
                        }
                        break;
                    case 1:
                        if (numDemon > 0)
                        {
                            buf[x, y].Object = new LevelObject
                            {
                                ObjectType = AbyssTemplate.AbyssDemon[rand.Next(AbyssTemplate.AbyssDemon.Length)]
                            };
                            numDemon--;
                        }
                        break;
                    case 2:
                        if (numBrute > 0)
                        {
                            buf[x, y].Object = new LevelObject
                            {
                                ObjectType = AbyssTemplate.AbyssBrute[rand.Next(AbyssTemplate.AbyssBrute.Length)]
                            };
                            numBrute--;
                        }
                        break;
                    case 3:
                        if (numSkull > 0)
                        {
                            buf[x, y].Object = new LevelObject
                            {
                                ObjectType = AbyssTemplate.AbyssBones
                            };
                            numSkull--;
                        }
                        break;
                }
            }
        }
        public override void Rasterize(SceneGame scene, Random rand)
        {
            throw new NotImplementedException();
        }
    }
}
