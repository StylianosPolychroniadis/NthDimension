using NthDimension.Algebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NthDimension.Procedural.Dungeon;

namespace NthDimension.Procedural.Dungeon.Templates.Abyss
{
    public class AbyssTemplate : LevelMapTemplate
    {
        internal static readonly LevelTileType RedSmallChecks                   = new LevelTileType(0x003c,         "Red Small Checks");
        internal static readonly LevelTileType Lava                             = new LevelTileType(0x0070,         "Lava");
        internal static readonly LevelTileType Space                            = new LevelTileType(0x00fe,         "Space");

        internal static readonly LevelObjectType RedWall                        = new LevelObjectType(0x0150,       "Red Wall");
        internal static readonly LevelObjectType RedTorchWall                   = new LevelObjectType(0x0151,       "Red Torch Wall");
        internal static readonly LevelObjectType PartialRedFloor                = new LevelObjectType(0x0153,       "Partial Red Floor");
        internal static readonly LevelObjectType RedPillar                      = new LevelObjectType(0x017e,       "Red Pillar");
        internal static readonly LevelObjectType BrokenRedPillar                = new LevelObjectType(0x0183,       "Broken Red Pillar");
        internal static readonly LevelObjectType CowardicePortal                = new LevelObjectType(0x0703,       "Portal of Cowardice");

        internal static readonly LevelObjectType AbyssImp                       = new LevelObjectType(0x66d,        "Imp of the Abyss");

        internal static readonly LevelObjectType[] AbyssDemon                   = {
                                                                                      new LevelObjectType(0x66e,    "Demon of the Abyss"),
                                                                                      new LevelObjectType(0x66f,    "Demon Warrior of the Abyss"),
                                                                                      new LevelObjectType(0x670,    "Demon Mage of the Abyss")
                                                                                  };

        internal static readonly LevelObjectType[] AbyssBrute                   = {
                                                                                      new LevelObjectType(0x671,    "Brute of the Abyss"),
                                                                                      new LevelObjectType(0x672,    "Brute Warrior of the Abyss")
                                                                                  };

        internal static readonly LevelObjectType AbyssBones                     = new LevelObjectType(0x01fa,       "Abyss Bones");

        internal static readonly LevelTile[,] MapTemplate;

        static AbyssTemplate()
        {
            MapTemplate = ReadTemplate(typeof(AbyssTemplate));
        }

        public override int MaxDepth { get { return 50; } }

        NormDist targetDepth;
        public override NormDist TargetDepth { get { return targetDepth; } }

        NormDist specialRmCount;
        public override NormDist SpecialRmCount { get { return specialRmCount; } }

        NormDist specialRmDepthDist;
        public override NormDist SpecialRmDepthDist { get { return specialRmDepthDist; } }

        public override Range RoomSeparation { get { return new Range(1, 2); } }

        public override int CorridorWidth { get { return 3; } }

        public override void Initialize()
        {
            targetDepth = new NormDist(3, 20, 15, 35, Rand.Next());
            specialRmCount = new NormDist(1.5f, 0.5f, 0, 5, Rand.Next());
            specialRmDepthDist = new NormDist(5, 20, 10, 35, Rand.Next());
        }

        public override LevelRoom CreateStart(int depth)
        {
            return new RoomRoot(16);
        }

        public override LevelRoom CreateTarget(int depth, LevelRoom prev)
        {
            return new RoomBoss();
        }

        public override LevelRoom CreateSpecial(int depth, LevelRoom prev)
        {
            return new TreasureRoom();
        }

        public override LevelRoom CreateNormal(int depth, LevelRoom prev)
        {
            return new Room((int)new NormDist(3, 20, 15, 35, Rand.Next()).NextValue(), 
                            (int)new NormDist(3, 20, 15, 35, Rand.Next()).NextValue() );
        }

        public override LevelMapCorridor CreateCorridor()
        {
            return new Corridor();
        }

        public override LevelMapRender CreateOverlay()
        {
            return new Overlay();
        }
    }
}
