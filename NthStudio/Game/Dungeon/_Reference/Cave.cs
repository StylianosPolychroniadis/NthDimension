namespace NthStudio.Game.Dungeon
{
    using System;

    #region Features
    public enum CaveFeature
    {
        Undefined = -1,
        None = 0,

        CaveFloor,
        CavernFloor,
        Grass,
        Dirt,
        FarmPlot,
        GrassDirtBlend0,
        GrassDirtBlend1,
        GrassDirtBlend2,
        GrassDirtBlend3,
        GrassDirtBlend4,
        GrassDirtBlend5,
        GrassDirtBlend6,
        GrassDirtBlend7,
        GrassDirtBlend8,
        GrassDirtBlend9,
        GrassDirtBlend10,
        GrassDirtBlend11,

        Tree1,
        Tree2,
        Tree3,

        ShopStart,
        ShopGeneral,
        ShopArmor,
        ShopWeapons,
        ShopPotions,
        ShopMagic,
        ShopBook,
        ShopEnd,

        DoorOpenStart,
        DoorOpen1,
        DoorStoneOpen1,
        DoorOpen2,
        DoorOpen3,
        DoorGateOpen3,
        DoorOpen4,
        DoorStoneOpen4,
        DoorSecretOpen1,
        DoorSecretOpen2,
        DoorSecretOpen3,
        DoorSecretOpen4,
        DoorCavernOpen,
        DoorOpenEnd,

        DoorBrokenStart,
        DoorBroken1,
        DoorStoneBroken1,
        DoorBroken2,
        DoorBroken3,
        DoorGateBroken3,
        DoorBroken4,
        DoorStoneBroken4,
        DoorSecretBroken1,
        DoorSecretBroken2,
        DoorSecretBroken3,
        DoorSecretBroken4,
        DoorCavernBroken,
        DoorBrokenEnd,

        DoorClosedStart,
        DoorClosed1,
        DoorStoneClosed1,
        DoorClosed2,
        DoorClosed3,
        DoorGateClosed3,
        DoorClosed4,
        DoorStoneClosed4,
        DoorSecretClosed1,
        DoorSecretClosed2,
        DoorSecretClosed3,
        DoorSecretClosed4,
        DoorCavernClosed,
        DoorClosedEnd,

        DoorHiddenStart,
        DoorSecretHidden1,
        DoorSecretHidden2,
        DoorSecretHidden3,
        DoorSecretHidden4,
        DoorCavernHidden,
        DoorHiddenEnd,

        Rubble,

        StairsUp1,
        StairsUp2,
        StairsUp3,
        StairsUp4,
        StairsDown1,
        StairsDown2,
        StairsDown3,
        StairsDown4,

        /// <summary>
        /// All entries from this point on must be walls!
        /// </summary>
        WallStart,

        Mountain,

        /// <summary>
        /// "Extra" granite walls which exist outside of any rooms
        /// </summary>
        GraniteWallExtra,

        /// <summary>
        /// Granite walls which make up the outside of a room
        /// </summary>
        GraniteWallOuter,

        /// <summary>
        /// Granite walls which exist in the interior of a room
        /// </summary>
        GraniteWallInner,

        /// <summary>
        /// Granite walls which should not be tunneled through during
        /// level generation
        /// </summary>
        GraniteWallSolid,

        CavernWall,
        Column1,
        Column3,

        WallBlood1,
        WallMoss1,
        WallImage1,
        WallFountain1,
        WallColumns1,
        Window1,
        WindowBars1,

        Wall2,
        WallBlood2,
        WallMoss2,
        WallImage2,
        WallFountain2,
        WallColumns2,
        Window2,
        WindowBars2,

        Wall3,
        WallBlood3,
        WallMoss3,
        WallImage3,
        WallFountain3,
        WallColumns3,
        Window3,
        WindowBars3,

        Wall4,
        WallBlood4,
        WallMoss4,
        WallImage4,
        WallFountain4,
        WallColumns4,
        Window4,
        WindowBars4,

        MagmaWall,
        QuartzWall,

        // Permanent walls go at the end always
        PermanentWallExtra,
        PermanentWallOuter,
        PermanentWallInner
    }
    #endregion

    #region Flags
    [Flags()]
    enum CaveFlag
    {
        None = 0,

        /// <summary>
        /// Memorized feature
        /// </summary>
        Marked = 1,

        /// <summary>
        /// Self-illuminating
        /// </summary>
        Glowing = 2,

        /// <summary>
        /// Part of a vault; invalid destination for teleportation
        /// </summary>
        IsVault = 4,

        /// <summary>
        /// Part of a room; affects darkness, illumination spells
        /// </summary>
        IsRoom = 8,

        Seen = 0x10,
        View = 0x20,

        /// <summary>
        /// Any function which sets this flag must clear it before exiting
        /// </summary>
        Temp = 0x40,

        BlocksLOS = 0x80,

        // WARNING: flags beyond this value will not be serialized
        Temp2 = 0x100
    }
    #endregion
}
