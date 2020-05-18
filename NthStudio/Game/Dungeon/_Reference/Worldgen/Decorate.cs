using System;
using System.Collections.Generic;

namespace NthStudio.Game.Dungeon.Worldgen
{
    enum DecorateStyle
    {
        // No decoration style specified
        Undefined = 0,

        // Boring dungeon
        Default,

        // Expansive cavern
        Cavern,

        // Wooden walls and stuff
        Wooden,

        // Dark stone walls
        Dungeon,

        // Light brick walls
        Town,
    }

    [Flags()]
    enum DecorateFlags
    {
        None = 0,
        Spread = 1,
        Bloody = 2,
        AltDoors = 4,
    }

    class Decorate
    {
        static Dictionary<CaveFeature, CaveFeature>[] maps;
        static Decorate()
        {
            DecorateStyle[] allStyles = (DecorateStyle[])Enum.GetValues(typeof(DecorateStyle));
            maps = new Dictionary<CaveFeature, CaveFeature>[allStyles.Length];
            CaveFeature[] allFeatures = (CaveFeature[])Enum.GetValues(typeof(CaveFeature));
            for (int i = 0; i < allStyles.Length; i++)
            {
                DecorateStyle style = allStyles[i];
                maps[i] = new Dictionary<CaveFeature, CaveFeature>();
                foreach (CaveFeature feature in allFeatures)
                {
                    CaveFeature mappedFeature = GetStyleFeature(feature, style);
                    if (mappedFeature != feature)
                        maps[i][feature] = mappedFeature;
                }
            }
        }

        public static CaveFeature LookupStyleFeature(CaveFeature feature, DecorateStyle style)
        {
            CaveFeature mapped;
            if (maps[(int)style].TryGetValue(feature, out mapped))
                return mapped;
            else
                return feature;
        }

        static CaveFeature GetStyleFeature(CaveFeature feature, DecorateStyle style)
        {
            switch (style)
            {
                case DecorateStyle.Default:
                    return feature;
                case DecorateStyle.Cavern:
                    switch (feature)
                    {
                        case CaveFeature.CaveFloor:
                            return CaveFeature.CavernFloor;
                        case CaveFeature.DoorSecretHidden1:
                            return CaveFeature.DoorCavernHidden;
                        case CaveFeature.GraniteWallExtra:
                        case CaveFeature.GraniteWallInner:
                        case CaveFeature.GraniteWallOuter:
                        case CaveFeature.GraniteWallSolid:
                        case CaveFeature.WallMoss1:
                            return CaveFeature.CavernWall;
                        default:
                            return feature;
                    }
                case DecorateStyle.Wooden:
                    switch (feature)
                    {
                        case CaveFeature.DoorBroken1:
                            return CaveFeature.DoorBroken2;
                        case CaveFeature.DoorClosed1:
                            return CaveFeature.DoorClosed2;
                        case CaveFeature.DoorOpen1:
                            return CaveFeature.DoorOpen2;
                        case CaveFeature.DoorSecretHidden1:
                            return CaveFeature.DoorSecretHidden2;
                        case CaveFeature.DoorStoneBroken1:
                            return CaveFeature.DoorBroken2;
                        case CaveFeature.DoorStoneClosed1:
                            return CaveFeature.DoorClosed2;
                        case CaveFeature.DoorStoneOpen1:
                            return CaveFeature.DoorOpen2;
                        case CaveFeature.GraniteWallExtra:
                        case CaveFeature.GraniteWallInner:
                        case CaveFeature.GraniteWallOuter:
                        case CaveFeature.GraniteWallSolid:
                            return CaveFeature.Wall2;
                        case CaveFeature.WallBlood1:
                            return CaveFeature.WallBlood2;
                        case CaveFeature.WallColumns1:
                            return CaveFeature.WallColumns2;
                        case CaveFeature.WallFountain1:
                            return CaveFeature.WallFountain2;
                        case CaveFeature.WallImage1:
                            return CaveFeature.WallImage2;
                        case CaveFeature.WallMoss1:
                            return CaveFeature.WallMoss2;
                        case CaveFeature.Window1:
                            return CaveFeature.Window2;
                        case CaveFeature.WindowBars1:
                            return CaveFeature.WindowBars2;
                        case CaveFeature.StairsDown1:
                            return CaveFeature.StairsDown2;
                        default:
                            return feature;
                    }
                case DecorateStyle.Dungeon:
                    switch (feature)
                    {
                        case CaveFeature.DoorBroken1:
                            return CaveFeature.DoorBroken3;
                        case CaveFeature.DoorClosed1:
                            return CaveFeature.DoorClosed3;
                        case CaveFeature.DoorOpen1:
                            return CaveFeature.DoorOpen3;
                        case CaveFeature.DoorSecretHidden1:
                            return CaveFeature.DoorSecretHidden3;
                        case CaveFeature.DoorStoneBroken1:
                            return CaveFeature.DoorGateBroken3;
                        case CaveFeature.DoorStoneClosed1:
                            return CaveFeature.DoorGateClosed3;
                        case CaveFeature.DoorStoneOpen1:
                            return CaveFeature.DoorGateOpen3;
                        case CaveFeature.GraniteWallExtra:
                        case CaveFeature.GraniteWallInner:
                        case CaveFeature.GraniteWallOuter:
                        case CaveFeature.GraniteWallSolid:
                            return CaveFeature.Wall3;
                        case CaveFeature.WallBlood1:
                            return CaveFeature.WallBlood3;
                        case CaveFeature.WallColumns1:
                            return CaveFeature.WallColumns3;
                        case CaveFeature.WallFountain1:
                            return CaveFeature.WallFountain3;
                        case CaveFeature.WallImage1:
                            return CaveFeature.WallImage3;
                        case CaveFeature.WallMoss1:
                            return CaveFeature.WallMoss3;
                        case CaveFeature.Window1:
                            return CaveFeature.Window3;
                        case CaveFeature.WindowBars1:
                            return CaveFeature.WindowBars3;
                        case CaveFeature.StairsDown1:
                            return CaveFeature.StairsDown3;
                        default:
                            return feature;
                    }

                case DecorateStyle.Town:
                    switch (feature)
                    {
                        case CaveFeature.DoorBroken1:
                            return CaveFeature.DoorBroken4;
                        case CaveFeature.DoorClosed1:
                            return CaveFeature.DoorClosed4;
                        case CaveFeature.DoorOpen1:
                            return CaveFeature.DoorOpen4;
                        case CaveFeature.DoorSecretHidden1:
                            return CaveFeature.DoorSecretHidden4;
                        case CaveFeature.DoorStoneBroken1:
                            return CaveFeature.DoorStoneBroken4;
                        case CaveFeature.DoorStoneClosed1:
                            return CaveFeature.DoorStoneClosed4;
                        case CaveFeature.DoorStoneOpen1:
                            return CaveFeature.DoorStoneOpen4;
                        case CaveFeature.GraniteWallExtra:
                        case CaveFeature.GraniteWallInner:
                        case CaveFeature.GraniteWallOuter:
                        case CaveFeature.GraniteWallSolid:
                            return CaveFeature.Wall4;
                        case CaveFeature.WallBlood1:
                            return CaveFeature.WallBlood4;
                        case CaveFeature.WallColumns1:
                            return CaveFeature.WallColumns4;
                        case CaveFeature.WallFountain1:
                            return CaveFeature.WallFountain4;
                        case CaveFeature.WallImage1:
                            return CaveFeature.WallImage4;
                        case CaveFeature.WallMoss1:
                            return CaveFeature.WallMoss4;
                        case CaveFeature.Window1:
                            return CaveFeature.Window4;
                        case CaveFeature.WindowBars1:
                            return CaveFeature.WindowBars4;
                        case CaveFeature.StairsDown1:
                            return CaveFeature.StairsDown4;
                        case CaveFeature.Column1:
                            return CaveFeature.Column3;
                        default:
                            return feature;
                    }
                default:
                    return feature;
            }
        }
    }
}
