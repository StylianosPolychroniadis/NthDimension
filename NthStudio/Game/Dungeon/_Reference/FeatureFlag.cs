using System;

namespace NthStudio.Game.Dungeon
{
    [Flags()]
    public enum FeatureFlag
    {
        None = 0,

        /// <summary>
        /// Objects cannot be dropped on this tile
        /// </summary>
		NoObjects = 1,

        /// <summary>
        /// This tile blocks line of sight
        /// </summary>
		BlockLOS = 2,

        /// <summary>
        /// The player (and monsters) are allowed to move onto this tile
        /// </summary>
		AllowMove = 4,

        /// <summary>
        /// Any player can easily open this door/gate/whatever
        /// </summary>
		PlayerOpen = 8,

        /// <summary>
        /// Monsters can open this door/gate/whatever
        /// </summary>
		MonsterOpen = 16,

        /// <summary>
        /// Look command automatically stops here
        /// </summary>
        Interesting = 32,

        /// <summary>
        /// Allow player and monsters to shoot through this tile
        /// </summary>
        AllowShot = 64,
    }
}
