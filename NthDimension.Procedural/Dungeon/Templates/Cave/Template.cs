using NthDimension.Algebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Dungeon.Templates.Cave
{
    using NthDimension.Procedural.Dungeon;
    public class CaveTemplate : LevelMapTemplate
    {
        internal static readonly LevelTileType LightSand = new LevelTileType(0x00bd, "Light Sand");
        internal static readonly LevelTileType BrownLines = new LevelTileType(0x000c, "Brown Lines");
        internal static readonly LevelTileType ShallowWater = new LevelTileType(0x0073, "Shallow Water");
        internal static readonly LevelTileType Composite = new LevelTileType(0x00fd, "Composite");
        internal static readonly LevelTileType Space = new LevelTileType(0x00fe, "Space");

        internal static readonly LevelObjectType CaveWall = new LevelObjectType(0x01ce, "Cave Wall");
        internal static readonly LevelObjectType PalmTree = new LevelObjectType(0x018e, "Palm Tree");
        internal static readonly LevelObjectType CowardicePortal = new LevelObjectType(0x0703, "Portal of Cowardice");

        internal static readonly LevelObjectType PirateKing = new LevelObjectType(0x0927, "Dreadstump the Pirate King");

        internal static readonly LevelObjectType[] Boss = {
            new LevelObjectType(0x683, "Pirate Lieutenant"),
            new LevelObjectType(0x684, "Pirate Commander"),
            new LevelObjectType(0x685, "Pirate Captain"),
            new LevelObjectType(0x686, "Pirate Admiral")
        };

        internal static readonly LevelObjectType[] Minion = {
            new LevelObjectType(0x687, "Cave Pirate Brawler"),
            new LevelObjectType(0x688, "Cave Pirate Sailor"),
            new LevelObjectType(0x689, "Cave Pirate Veteran")
        };

        internal static readonly LevelObjectType[] Pet = {
            new LevelObjectType(0x68a, "Cave Pirate Moll"),
            new LevelObjectType(0x68b, "Cave Pirate Parrot"),
            new LevelObjectType(0x68c, "Cave Pirate Macaw"),
            new LevelObjectType(0x68d, "Cave Pirate Monkey"),
            new LevelObjectType(0x68e, "Cave Pirate Hunchback"),
            new LevelObjectType(0x68f, "Cave Pirate Cabin Boy")
        };

        public override int MaxDepth { get { return 10; } }

        NormDist targetDepth;
        public override NormDist TargetDepth { get { return targetDepth; } }

        public override NormDist SpecialRmCount { get { return null; } }

        public override NormDist SpecialRmDepthDist { get { return null; } }

        public override Range RoomSeparation { get { return new Range(3, 7); } }

        public override int CorridorWidth { get { return 2; } }

        public override void Initialize()
        {
            targetDepth = new NormDist(1, 5.5f, 4, 7, Rand.Next());
        }

        public override LevelRoom CreateStart(int depth)
        {
            return new RoomRoot(40);
        }

        public override LevelRoom CreateTarget(int depth, LevelRoom prev)
        {
            return new RoomBoss(80);
        }

        public override LevelRoom CreateSpecial(int depth, LevelRoom prev)
        {
            throw new InvalidOperationException();
        }

        public override LevelRoom CreateNormal(int depth, LevelRoom prev)
        {
            return new Room(Rand.Next(25, 85), Rand.Next(25, 85));
        }

        public override LevelMapCorridor CreateCorridor()
        {
            return new Corridor();
        }

        public override LevelMapRender CreateBackground()
        {
            return new Background();
        }

        public override LevelMapRender CreateOverlay()
        {
            return new Overlay();
        }
    }
}
