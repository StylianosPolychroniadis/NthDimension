using NthDimension.Algebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Dungeon.Templates.Lab
{
    using NthDimension.Procedural.Dungeon;
    public class LabTemplate : LevelMapTemplate
    {
        internal static readonly LevelTileType LabFloor                 = new LevelTileType(0x00d3,             "Lab Floor");
        internal static readonly LevelTileType Space                    = new LevelTileType(0x00fe,             "Space");

        internal static readonly LevelObjectType LabWall                = new LevelObjectType(0x188c,           "Lab Wall");
        internal static readonly LevelObjectType DestructibleWall       = new LevelObjectType(0x18c3,           "Lab Destructible Wall");
        internal static readonly LevelObjectType Web                    = new LevelObjectType(0x0732,           "Spider Web");

        internal static readonly LevelObjectType[] Big                  = {
                                                                            new LevelObjectType(0x0981,         "Escaped Experiment"),
                                                                            new LevelObjectType(0x0982,         "Enforcer Bot 3000"),
                                                                            new LevelObjectType(0x0983,         "Crusher Abomination")
                                                                          };

        internal static readonly LevelObjectType[] Small                = {
                                                                            new LevelObjectType(0x0979,         "Mini Bot"),
                                                                            new LevelObjectType(0x0980,         "Rampage Cyborg")
                                                                        };

        static readonly LevelObject web = new LevelObject
        {
            ObjectType = Web
        };

        internal static readonly LevelTile[,] MapTemplate;

        static LabTemplate()
        {
            MapTemplate = ReadTemplate(typeof(LabTemplate));
        }

        public override int MaxDepth { get { return 20; } }

        NormDist targetDepth;
        public override NormDist TargetDepth { get { return targetDepth; } }

        public override NormDist SpecialRmCount { get { return null; } }

        public override NormDist SpecialRmDepthDist { get { return null; } }

        //public override Range RoomSeparation { get { return new Range(6, 8); } }
        public override Range RoomSeparation { get { return new Range(2, 4); } }

        public override int CorridorWidth { get { return 4; } }

        public override Range NumRoomRate { get { return new Range(2, 3); } }

        bool generatedEvilRoom;

        public override void Initialize()
        {
            targetDepth             = new NormDist(3, 10, 7, 15, Rand.Next());
            generatedEvilRoom       = false;
        }

        public override LevelRoom CreateStart(int depth)
        {
            return new RoomRoot();
        }

        public override LevelRoom CreateTarget(int depth, LevelRoom prev)
        {
            return new RoomBoss();
        }

        public override LevelRoom CreateSpecial(int depth, LevelRoom prev)
        {
            throw new InvalidOperationException();
        }

        public override LevelRoom CreateNormal(int depth, LevelRoom prev)
        {
            var rm = new DungeonRoom(prev as DungeonRoom, Rand, generatedEvilRoom);
            if ((rm.Flags & DungeonRoom.RoomFlags.Evil) != 0)
                generatedEvilRoom = true;
            return rm;
        }

        public override LevelMapCorridor CreateCorridor()
        {
            return new Corridor();
        }

        public override LevelMapRender CreateOverlay()
        {
            return new Overlay();
        }

        internal static void DrawSpiderWeb(BitmapRasterizer<LevelTile> rasterizer, Rect bounds, Random rand)
        {
            int w = rasterizer.Width, h = rasterizer.Height;
            var buf = rasterizer.Bitmap;

            for (int x = bounds.X; x < bounds.MaxX; x++)
                for (int y = bounds.Y; y < bounds.MaxY; y++)
                {
                    if (buf[x, y].TileType == Space || buf[x, y].Object != null)
                        continue;

                    if (rand.NextDouble() > 0.99)
                        buf[x, y].Object = web;
                }
        }

        internal static void CreateEnemies(BitmapRasterizer<LevelTile> rasterizer, Rect bounds, Random rand)
        {
            int numBig = new Range(0, 3).Random(rand);
            int numSmall = new Range(4, 10).Random(rand);

            var buf = rasterizer.Bitmap;
            while (numBig > 0 || numSmall > 0)
            {
                int x = rand.Next(bounds.X, bounds.MaxX);
                int y = rand.Next(bounds.Y, bounds.MaxY);
                if (buf[x, y].TileType == Space || buf[x, y].Object != null)
                    continue;

                switch (rand.Next(2))
                {
                    case 0:
                        if (numBig > 0)
                        {
                            buf[x, y].Object = new LevelObject
                            {
                                ObjectType = Big[rand.Next(Big.Length)]
                            };
                            numBig--;
                        }
                        break;
                    case 1:
                        if (numSmall > 0)
                        {
                            buf[x, y].Object = new LevelObject
                            {
                                ObjectType = Small[rand.Next(Small.Length)]
                            };
                            numSmall--;
                        }
                        break;
                }
            }
        }
    }
}
