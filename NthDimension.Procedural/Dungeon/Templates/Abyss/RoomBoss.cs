using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NthDimension.Procedural.Dungeon;

namespace NthDimension.Procedural.Dungeon.Templates.Abyss
{
    internal class RoomBoss : LevelRoom
    {
        public override LevelRoomType Type { get { return LevelRoomType.Target; } }

        public override int Width { get { return 42; } }

        public override int Length { get { return 42; } }

        public override void Rasterize(BitmapRasterizer<LevelTile> rasterizer, Random rand)
        {
            var buf = rasterizer.Bitmap;
            var bounds = Bounds;

            rasterizer.Copy(AbyssTemplate.MapTemplate, new Rect(10, 10, 52, 52), Pos, tile => tile.TileType.Name == "Space");

            int numCorrupt = new Range(2, 10).Random(rand);
            while (numCorrupt > 0)
            {
                int x = rand.Next(bounds.X, bounds.MaxX);
                int y = rand.Next(bounds.Y, bounds.MaxY);

                if (buf[x, y].Object == null)
                    continue;
                if (buf[x, y].Object.ObjectType != AbyssTemplate.PartialRedFloor)
                    continue;

                buf[x, y].Object = null;
                numCorrupt--;
            }

            int numImp = new Range(1, 2).Random(rand);
            int numDemon = new Range(1, 3).Random(rand);
            int numBrute = new Range(1, 3).Random(rand);

            while (numImp > 0 || numDemon > 0 || numBrute > 0)
            {
                int x = rand.Next(bounds.X, bounds.MaxX);
                int y = rand.Next(bounds.Y, bounds.MaxY);

                if (buf[x, y].Object != null || buf[x, y].TileType == AbyssTemplate.Space)
                    continue;

                switch (rand.Next(3))
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
                }
            }
        }

        public override void Rasterize(Rendering.Scenegraph.SceneGame scene, Random rand)
        {
            throw new NotImplementedException();
        }
    }
}
