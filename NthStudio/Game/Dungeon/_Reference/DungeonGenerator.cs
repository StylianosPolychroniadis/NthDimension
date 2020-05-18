using NthStudio.Game.Dungeon;
using NthStudio.Game.Dungeon.Worldgen;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Game.Dungeon
{
    class DungeonGenerator
    {
        #region struct Edge
        /// <summary>
        /// Describes a proposed connection between two rooms
        /// </summary>
        struct Edge
        {
            /// <summary>
            /// Smaller of the two room indices
            /// </summary>
            public int PMin;

            /// <summary>
            /// Larger of the two room indices
            /// </summary>
            public int PMax;

            /// <summary>
            /// The length squared of the edge between these two rooms
            /// </summary>
            public int LenSq;

            /// <summary>
            /// Creates a new edge
            /// </summary>
            /// <param name="p1">Room index 1</param>
            /// <param name="p2">Room index 2</param>
            /// <param name="lenSq">Length squared of the distance between the rooms</param>
            public Edge(int p1, int p2, int lenSq)
            {
                if (p1 < p2)
                {
                    PMin = p1;
                    PMax = p2;
                }
                else
                {
                    PMin = p2;
                    PMax = p1;
                }
                LenSq = lenSq;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is Edge))
                    return false;
                Edge other = (Edge)obj;
                return other.PMin == PMin && other.PMax == PMax;
            }
            /// <summary>
            /// This override is required by .NET because we have overridden .Equals
            /// </summary>
            public override int GetHashCode()
            {
                return PMin * 10000 + PMax;
            }
        }
        #endregion struct Edge

        #region Members
        /// <summary>
        /// Perlin noise function used to make tunnels more interesting
        /// </summary>
        Perlin                              _noise;
        /// <summary>
        /// Holds the level which we are currently creating
        /// </summary>
        DungeonLevel                        _level;
        /// <summary>
        /// Stores dungeon tiles
        /// </summary>
        FixedGrid<CaveFeature>              _caveFeatures;
        /// <summary>
        /// Stores flags for dungeon tiles
        /// </summary>
		FixedGrid<CaveFlag>                 _caveFlags;
        /// <summary>
        /// We track the list of points at which tunnels intersect.  Later we
        /// create doors at most of these locations after all the tunnels have
        /// been carved out.
        /// </summary>
        List<System.Drawing.Point>          _tunnelJunctions        = new List<System.Drawing.Point>();
        /// <summary>
        /// Locations to spawn extra goodies, as specified by fixed room definitions
        /// </summary>
        List<System.Drawing.Point>          _goodySpots             = new List<System.Drawing.Point>();
        
        /// <summary>
        /// Width of the dungeon measured in tiles
        /// </summary>
        const int                           DungeonSizeWidth        = 120;
        /// <summary>
        /// Height of the dungeon measured in tiles
        /// </summary>
		const int                           DungeonSizeHeight       = 80;

        const int                           TownWidth               = 64;
        const int                           TownHeight              = 64;

        const int                           BossDepth               = 20;
        const int                           CreateRoomCount         = 50;
        const int                           MinRoomBuffer           = 1;
        /// <summary>
        /// Chance of creating a door when a tunnel pierces into a room
        /// </summary>
        const int                           TunnelPierceDoorChance  = 100;
        #endregion Members

        #region Ctor
        /// <summary>
        /// Default constructor, don't know what we're going to create yet
        /// </summary>
        public DungeonGenerator()
        {
        }
        #endregion Ctor

        public DungeonLevel CreateTown()
        {
            // Determine size of the level
            System.Drawing.Size dungeonSize = new System.Drawing.Size(TownWidth, TownHeight);

            // Allocate storage for dungeon tiles.  Initially fill the grid with granite.
            _caveFeatures = new FixedGrid<CaveFeature>(dungeonSize.Width, dungeonSize.Height, CaveFeature.Grass);

            // Allocate storage for dungeon flags.
            _caveFlags = new FixedGrid<CaveFlag>(dungeonSize.Width, dungeonSize.Height, CaveFlag.None);

            // Create an empty level to start with
            _level = new DungeonLevel(_caveFeatures, _caveFlags, dungeonSize);

            // Assign depth
            _level.Depth = 0;

            // Get a local copy of the random number generator
            MT19937 rand = Globals.Rand;

            // Fill the town with grass
            GridUtil.FillRectangle(_caveFeatures, 0, 0, dungeonSize.Width, dungeonSize.Height, CaveFeature.Grass);
            GridUtil.FillRectangle(_caveFlags, 0, 0, dungeonSize.Width, dungeonSize.Height, CaveFlag.None);

            // Add some rooms
            List<RoomInstance> roomInstances = new List<RoomInstance>();
            roomInstances.Add(new RoomInstanceShop(CaveFeature.ShopArmor));
            roomInstances.Add(new RoomInstanceShop(CaveFeature.ShopGeneral));
            roomInstances.Add(new RoomInstanceShop(CaveFeature.ShopWeapons));
            roomInstances.Add(new RoomInstanceShop(CaveFeature.ShopPotions));
            roomInstances.Add(new RoomInstanceShop(CaveFeature.ShopMagic));
            roomInstances.Add(new RoomInstanceShop(CaveFeature.ShopBook));
            roomInstances.Add(new RoomInstanceFarm());
            roomInstances.Add(new RoomInstanceFarm());
            Lists.RandomizeOrder(roomInstances);
            roomInstances.Insert(2, new RoomInstanceDungeon());

            List<RoomInfo> shops = new List<RoomInfo>();
            CreateRoomsV3(shops, roomInstances, dungeonSize.Width, dungeonSize.Height);

            // Connect the shops with dirt paths
            ConnectRooms(shops, true);

            // Make dirt paths nicer - Step 1 - Fill dirt into shallow holes
            bool filled = true;
            while (filled)
            {
                filled = false;
                for (int x = 0; x < dungeonSize.Width; x++)
                    for (int y = 0; y < dungeonSize.Height; y++)
                    {
                        CaveFeature feature = _caveFeatures.GetValue(x, y);
                        if (feature != CaveFeature.Grass) continue;

                        // Count adjacent dirt tiles in cardinal directions
                        int adjCount = 0;
                        int adjStart = -1;
                        int adjEnd = -1;
                        for (int i = 0; i < 8; i += 2)
                        {
                            System.Drawing.Point p2 = new System.Drawing.Point(x + Direction.DirX[i], y + Direction.DirY[i]);
                            if (_caveFeatures.GetValue(p2.X, p2.Y) == CaveFeature.Dirt)
                            {
                                adjCount++;
                                if (adjStart == -1)
                                    adjStart = i;
                                adjEnd = i;
                            }
                        }
                        if (adjCount > 2 || (adjCount == 2 && Math.Abs(Direction.DirSubtract(adjStart, adjEnd)) != 2))
                        {
                            _level.SetFeature(x, y, CaveFeature.Dirt);
                            filled = true;
                            continue;
                        }

                        // Examine diagonals
                        bool bad = false;
                        if (adjCount == 0)
                        {
                            for (int i = 1; i < 8; i += 2)
                            {
                                System.Drawing.Point p2 = new System.Drawing.Point(x + Direction.DirX[i], y + Direction.DirY[i]);
                                if (_caveFeatures.GetValue(p2.X, p2.Y) == CaveFeature.Dirt)
                                    adjCount++;
                            }
                            if (adjCount > 1)
                                bad = true;
                        }
                        else if (adjCount == 1)
                        {
                            int i = ((adjStart + 3) & 7);
                            System.Drawing.Point p2 = new System.Drawing.Point(x + Direction.DirX[i], y + Direction.DirY[i]);
                            if (_caveFeatures.GetValue(p2.X, p2.Y) == CaveFeature.Dirt)
                                bad = true;
                            i = ((adjStart + 5) & 7);
                            p2 = new System.Drawing.Point(x + Direction.DirX[i], y + Direction.DirY[i]);
                            if (_caveFeatures.GetValue(p2.X, p2.Y) == CaveFeature.Dirt)
                                bad = true;
                        }
                        else if (adjCount == 2)
                        {
                            int badDir = (adjEnd + 3) & 7;
                            if (adjEnd == 6 && adjStart == 0)
                                badDir = 3;
                            System.Drawing.Point p2 = new System.Drawing.Point(x + Direction.DirX[badDir], y + Direction.DirY[badDir]);
                            if (_caveFeatures.GetValue(p2.X, p2.Y) == CaveFeature.Dirt)
                                bad = true;
                        }
                        if (bad)
                        {
                            _level.SetFeature(x, y, CaveFeature.Dirt);
                            filled = true;
                            continue;
                        }
                    }
            }

            // Make dirt paths nicer - Step 2 - place blend tiles
            for (int x = 0; x < dungeonSize.Width; x++)
                for (int y = 0; y < dungeonSize.Height; y++)
                {
                    CaveFeature feature = _caveFeatures.GetValue(x, y);
                    if (feature != CaveFeature.Grass) continue;

                    // Count adjacent dirt tiles in cardinal directions
                    int adjCount = 0;
                    int adjStart = -1;
                    int adjEnd = -1;
                    for (int i = 0; i < 8; i += 2)
                    {
                        System.Drawing.Point p2 = new System.Drawing.Point(x + Direction.DirX[i], y + Direction.DirY[i]);
                        if (_caveFeatures.GetValue(p2.X, p2.Y) == CaveFeature.Dirt)
                        {
                            adjCount++;
                            if (adjStart == -1)
                                adjStart = i;
                            adjEnd = i;
                        }
                    }
                    if (adjCount == 0)
                    {
                        for (int i = 1; i < 8; i += 2)
                        {
                            System.Drawing.Point p2 = new System.Drawing.Point(x + Direction.DirX[i], y + Direction.DirY[i]);
                            if (_caveFeatures.GetValue(p2.X, p2.Y) == CaveFeature.Dirt)
                            {
                                CaveFeature set;
                                if (i == 1)
                                    set = CaveFeature.GrassDirtBlend8;
                                else if (i == 3)
                                    set = CaveFeature.GrassDirtBlend9;
                                else if (i == 5)
                                    set = CaveFeature.GrassDirtBlend10;
                                else
                                    set = CaveFeature.GrassDirtBlend11;
                                _caveFeatures.SetValue(x, y, set);
                            }
                        }
                    }
                    else if (adjCount == 1)
                    {
                        CaveFeature set;
                        if (adjStart == 0)
                            set = CaveFeature.GrassDirtBlend4;
                        else if (adjStart == 2)
                            set = CaveFeature.GrassDirtBlend5;
                        else if (adjStart == 4)
                            set = CaveFeature.GrassDirtBlend6;
                        else
                            set = CaveFeature.GrassDirtBlend7;
                        _level.SetFeature(x, y, set);
                    }
                    else if (adjCount == 2)
                    {
                        CaveFeature set;
                        if (adjStart == 0)
                        {
                            if (adjEnd == 2)
                                set = CaveFeature.GrassDirtBlend0;
                            else
                                set = CaveFeature.GrassDirtBlend3;
                        }
                        else if (adjStart == 2)
                            set = CaveFeature.GrassDirtBlend1;
                        else
                            set = CaveFeature.GrassDirtBlend2;
                        _level.SetFeature(x, y, set);
                    }
                }

            // Perlin noise generator #1 determines locations of trees
            Perlin perlin1 = new Perlin(Globals.Rand.Next(10000), 4);

            // Perline noise generator #2 determines tree types
            Perlin perlin2 = new Perlin(Globals.Rand.Next(10000), 4);

            int density = 3;
            for (int x = 0; x < dungeonSize.Width; x++)
                for (int y = 0; y < dungeonSize.Height; y++)
                {
                    if (_caveFeatures.GetValue(x, y) != CaveFeature.Grass)
                        continue;
                    if (Globals.Rand.Next(density) != 0)
                        continue;
                    double xd = 0.2 * (double)x;
                    double yd = 0.2 * (double)y;
                    if (perlin1.PerlinNoise(xd, yd) > 0.1)
                    {
                        double p2 = perlin2.PerlinNoise(xd * 0.2, yd * 0.2);
                        if (p2 < -0.2)
                            _level.SetFeature(x, y, CaveFeature.Tree1);
                        else if (p2 > 0.2)
                            _level.SetFeature(x, y, CaveFeature.Tree2);
                        else
                            _level.SetFeature(x, y, CaveFeature.Tree3);
                    }
                }


            // Draw mountains surrounding the town
            for (int x = 0; x < dungeonSize.Width; x++)
            {
                _level.SetFeature(x, 0, CaveFeature.Mountain);
                _level.SetFeature(x, dungeonSize.Height - 1, CaveFeature.Mountain);
            }
            for (int y = 0; y < dungeonSize.Height; y++)
            {
                _level.SetFeature(0, y, CaveFeature.Mountain);
                _level.SetFeature(dungeonSize.Width - 1, y, CaveFeature.Mountain);
            }

            // Pick a location for the player to start at
            while (true)
            {
                int x = Globals.Rand.Next(_level.Size.Width);
                int y = Globals.Rand.Next(_level.Size.Height);
                System.Drawing.Point p = new System.Drawing.Point(x, y);
                if (_level.CaveFeatures.GetValue(x, y) == CaveFeature.Grass &&
                    !_level.EntityMap.ContainsKey(p))
                {
                    _level.StartLocation = p;
                    break;
                }
            }

            // Town is always remembered
            _level.SetCaveFlags(0, 0, dungeonSize.Width, dungeonSize.Height, CaveFlag.Glowing | CaveFlag.Marked, true);
            _level.CalculateUsed();
            return _level;
        }
        public DungeonLevel CreateLevel(int depth)
        {
            // Determine size of the level
            System.Drawing.Size dungeonSize = new System.Drawing.Size(DungeonSizeWidth, DungeonSizeHeight);

            // Allocate storage for dungeon tiles.  Initially fill the grid with granite.
            _caveFeatures = new FixedGrid<CaveFeature>(dungeonSize.Width, dungeonSize.Height, CaveFeature.GraniteWallExtra);

            // Allocate storage for dungeon flags.
            _caveFlags = new FixedGrid<CaveFlag>(dungeonSize.Width, dungeonSize.Height, CaveFlag.None);

            // Create an empty level to start with
            _level = new DungeonLevel(_caveFeatures, _caveFlags, dungeonSize);

            // Assign dungeon level depth
            _level.Depth = depth;

            // Level feeling for quest level
            if (depth == BossDepth)
                _level.Feeling = new string[] { "You feel a powerful evil.", "The final battle is near!" };

            // Get a local copy of the random number generator
            MT19937 rand = Globals.Rand;

            // Use perlin noise in order to randomize tunnels a bit
            _noise = new Perlin();

            // Fill the dungeon with normal granite
            GridUtil.FillRectangle(_caveFeatures, 0, 0, dungeonSize.Width, dungeonSize.Height, CaveFeature.GraniteWallExtra);
            GridUtil.FillRectangle(_caveFlags, 0, 0, dungeonSize.Width, dungeonSize.Height, CaveFlag.BlocksLOS);

            // Keep track of points inside rooms that have been created so far.  These
            // points are later used to connect the rooms via tunnels
            List<RoomInfo> rooms = new List<RoomInfo>();

            // Create some rooms in the dungeon
            CreateRoomsV3(rooms, dungeonSize.Width, dungeonSize.Height);

            // Draw permanent walls surrounding the dungeon.  We draw pemanent walls
            // before connecting rooms with tunnels in order to prevent the pathfinder from
            // trying to exit the dungeon area
            for (int x = 0; x < dungeonSize.Width; x++)
            {
                _level.SetFeature(x, 0, CaveFeature.PermanentWallExtra);
                _level.SetFeature(x, dungeonSize.Height - 1, CaveFeature.PermanentWallExtra);
            }
            for (int y = 0; y < dungeonSize.Height; y++)
            {
                _level.SetFeature(0, y, CaveFeature.PermanentWallExtra);
                _level.SetFeature(dungeonSize.Width - 1, y, CaveFeature.PermanentWallExtra);
            }

            // Connect the rooms with tunnels
            ConnectRooms(rooms, false);

            #region _TODO_

            //// Analyze connectivity, randomize doors
            //Analysis analysis = Analysis.Analyze(_level);

            //// Pick a location for the player to start at
            //Data.FeatureTile[] featureTiles = Globals.FeatureTiles.Entries;
            //int startid = analysis.Start.ID;
            //while (true)
            //{
            //    int x = Globals.RandSpread(analysis.Start.Location.X, 20);
            //    int y = Globals.RandSpread(analysis.Start.Location.Y, 20);

            //    System.Drawing.Point p = new System.Drawing.Point(x, y);
            //    CaveFeature onFeature = _level.CaveFeatures.GetValue(x, y);

            //    if (analysis.ZoneGrids.GetValue(x, y) == startid &&
            //        0 != (featureTiles[(int)onFeature].Flags & FeatureFlag.AllowMove) &&
            //        !_level.EntityMap.ContainsKey(p))
            //    {
            //        _level.StartLocation = p;
            //        break;
            //    }
            //}

            //// Pick a location for the staircase to the next level (or the final boss)
            //int destid = analysis.Dest.ID;
            //while (true)
            //{
            //    int x = Globals.RandSpread(analysis.Dest.Location.X, 20);
            //    int y = Globals.RandSpread(analysis.Dest.Location.Y, 20);

            //    System.Drawing.Point p = new System.Drawing.Point(x, y);
            //    CaveFeature onFeature = _level.CaveFeatures.GetValue(x, y);

            //    if (analysis.ZoneGrids.GetValue(x, y) == destid &&
            //        onFeature == CaveFeature.CaveFloor || onFeature == CaveFeature.CavernFloor)
            //    {
            //        if (depth == BossDepth)
            //            _level.PlaceOneMonster(p, Data.MonsterDef.ChooseMonster(9999, null, SpawnType.SpawnQuest),
            //                false, null);
            //        else
            //            _level.SetFeature(x, y, CaveFeature.StairsDown1);
            //        break;
            //    }
            //}

            //// Create monters and items at the goody spots
            //foreach (System.Drawing.Point p in _goodySpots)
            //{
            //    // Sometimes out-of-depth monsters
            //    int useDepth = depth + (Globals.OneIn(25) ? 5 : 0);

            //    // Place a monster which is not a growth
            //    if (depth != BossDepth)
            //        _level.PlaceEncounter(p, Data.EncounterDef.ChooseEncounter(useDepth, SpawnType.Growth));

            //    // Place an item
            //    PlaceItem(p, Item.CreateRandomItem(useDepth, null));
            //}

            //// Create a minimum of 13 encounters on each level
            //if (depth != BossDepth)
            //{
            //    for (int i = 0; i < 13; i++)
            //    {
            //        // Sometimes out-of-depth monsters
            //        int useDepth = depth + (Globals.OneIn(25) ? 5 : 0);

            //        System.Drawing.Point p = new System.Drawing.Point();
            //        do
            //        {
            //            p.X = rand.Next(dungeonSize.Width);
            //            p.Y = rand.Next(dungeonSize.Height);
            //        } while (_level.CaveFeatures.GetValue(p.X, p.Y) != CaveFeature.CaveFloor || _level.EntityMap.ContainsKey(p) ||
            //            _level.StartLocation == p);

            //        _level.PlaceEncounter(p, Data.EncounterDef.ChooseEncounter(useDepth));
            //    }
            //}

            //// Create at least one item in every secret room
            //foreach (Zone zone in analysis.Zones)
            //    if (zone.IsSecret)
            //    {
            //        // Sometimes out-of-depth items
            //        int useDepth = depth + (Globals.OneIn(25) ? 5 : 0);

            //        System.Drawing.Point p = new System.Drawing.Point();
            //        for (int tries = 0; tries < 100; tries++)
            //        {
            //            p.X = Globals.RandSpread(zone.Location.X, 10);
            //            p.Y = Globals.RandSpread(zone.Location.Y, 10);
            //            if (analysis.ZoneGrids.GetValue(p.X, p.Y) == zone.ID &&
            //                0 == (featureTiles[(int)_level.CaveFeatures.GetValue(p.X, p.Y)].Flags & FeatureFlag.NoObjects))
            //            {
            //                PlaceItem(p, Item.CreateRandomItem(useDepth, null));
            //                break;
            //            }
            //        }
            //    }

            //// Create a minimum of 10 objects on the floor
            //for (int i = 0; i < 10; i++)
            //{
            //    // Sometimes out-of-depth items
            //    int useDepth = depth + (Globals.OneIn(25) ? 5 : 0);

            //    System.Drawing.Point p = new System.Drawing.Point();
            //    do
            //    {
            //        p.X = rand.Next(dungeonSize.Width);
            //        p.Y = rand.Next(dungeonSize.Height);
            //    } while (0 != (featureTiles[(int)_level.CaveFeatures.GetValue(p.X, p.Y)].Flags & FeatureFlag.NoObjects));
            //    Item item = Item.CreateRandomItem(useDepth, null);
            //    PlaceItem(p, item);
            //}

            //// Create up to 10 objects carried by monsters on the level
            //{
            //    // Collect all monsters that can carry objects
            //    List<ActorMonster> monsters = new List<ActorMonster>();
            //    foreach (KeyValuePair<System.Drawing.Point, Actor> pair in _level.EntityMap)
            //    {
            //        if (pair.Value is ActorMonster)
            //        {
            //            ActorMonster monster = (ActorMonster)pair.Value;
            //            if (monster.CanCarry())
            //                monsters.Add(monster);
            //        }
            //    }

            //    // Sort the monsters by depth
            //    Comparison<ActorMonster> compare = delegate (ActorMonster m1, ActorMonster m2)
            //    {
            //        return m1.MonsterDef.Depth.CompareTo(m2.MonsterDef.Depth);
            //    };
            //    monsters.Sort(compare);

            //    // Create up to 10 objects
            //    for (int i = 0; i < 10; i++)
            //    {
            //        // No more monsters can carry objects
            //        if (monsters.Count == 0)
            //            break;

            //        // Sometimes out-of-depth items
            //        int useDepth = depth + (Globals.OneIn(25) ? 5 : 0);

            //        // Pick a monster
            //        int pick = Globals.BestOf(monsters.Count, 3);

            //        // Give an item to the monster
            //        monsters[pick].CarryItem(Item.CreateRandomItem(useDepth, null));

            //        // Monster can't carry any more
            //        if (!monsters[pick].CanCarry())
            //            monsters.RemoveAt(pick);
            //    }
            //}

            //// Decorate the level with varying tiles, etc...
            //DecorateDungeon(rooms, analysis);

            //// Spawn a portal back to town
            //if (depth != BossDepth)
            //{
            //    while (true)
            //    {
            //        int x = Globals.Rand.Next(_level.Size.Width);
            //        int y = Globals.Rand.Next(_level.Size.Height);

            //        System.Drawing.Point p = new System.Drawing.Point(x, y);
            //        CaveFeature onFeature = _level.CaveFeatures.GetValue(x, y);
            //        if (0 == (featureTiles[(int)onFeature].Flags & FeatureFlag.NoObjects))
            //        {
            //            GridAttribute attribute = new GridAttribute(GridAttributeDef.Portal, p, 0, 0);
            //            _level.GridAttributeMap.Add(p, attribute);
            //            break;
            //        }
            //    }
            //}

            //// Create some random traps on the floor
            //for (int createTrapCount = Dice.Throw(new Dice(2, 6)); createTrapCount > 0; createTrapCount--)
            //{
            //    while (true)
            //    {
            //        int x = Globals.Rand.Next(_level.Size.Width);
            //        int y = Globals.Rand.Next(_level.Size.Height);

            //        System.Drawing.Point p = new System.Drawing.Point(x, y);
            //        CaveFeature onFeature = _level.CaveFeatures.GetValue(x, y);
            //        if (0 == (featureTiles[(int)onFeature].Flags & FeatureFlag.NoObjects) &&
            //            !_level.GridAttributeMap.ContainsKey(p))
            //        {
            //            // Choose a random trap type
            //            GridAttributeDef trapDef = GridAttributeDef.FloorTraps[Globals.Rand.Next(GridAttributeDef.FloorTraps.Count)];

            //            // Trap doors not allowed on final level
            //            if (trapDef.Type == GridAttributeType.FloorTrapDoor && depth == BossDepth) continue;

            //            // Create a trap
            //            GridAttribute trapAttribute = new GridAttribute(trapDef, p, Globals.Rand.Next(100), 0);

            //            // Place it in the dungeon
            //            _level.GridAttributeMap.Add(p, trapAttribute);

            //            // sometimes create some junk nearby to alert players
            //            if (!Trap.MakeJunk(trapDef.Type)) break;

            //            // create junk
            //            CreateSkeletons(p, _level, 5);

            //            // finished with this attempt
            //            break;
            //        }
            //    }
            //}

            //// Create some fountains
            //for (int createFountainCount = 2; createFountainCount > 0; createFountainCount--)
            //{
            //    while (true)
            //    {
            //        int x = Globals.Rand.Next(_level.Size.Width);
            //        int y = Globals.Rand.Next(_level.Size.Height);

            //        System.Drawing.Point p = new System.Drawing.Point(x, y);
            //        CaveFeature onFeature = _level.CaveFeatures.GetValue(x, y);
            //        if (0 == (featureTiles[(int)onFeature].Flags & FeatureFlag.NoObjects) &&
            //            !_level.GridAttributeMap.ContainsKey(p) && !_level.ItemMap.ContainsKey(p))
            //        {
            //            _level.GridAttributeMap.Add(p, new GridAttribute(GridAttributeDef.FountainFull, p, 0, 0));
            //            break;
            //        }
            //    }
            //}

            //// Create some mushrooms for assassins to find
            //for (int mushroomCount = Dice.Throw(new Dice(4, 3)); mushroomCount > 0; mushroomCount--)
            //{
            //    while (true)
            //    {
            //        int x = Globals.Rand.Next(_level.Size.Width);
            //        int y = Globals.Rand.Next(_level.Size.Height);

            //        System.Drawing.Point p = new System.Drawing.Point(x, y);
            //        CaveFeature onFeature = _level.CaveFeatures.GetValue(x, y);
            //        if (0 == (featureTiles[(int)onFeature].Flags & FeatureFlag.NoObjects) &&
            //            !_level.GridAttributeMap.ContainsKey(p))
            //        {
            //            // get the mushroom definition
            //            GridAttributeDef mushroomDef = GridAttributeDef.Mushroom;

            //            // Create a mushroom patch
            //            GridAttribute mushroomAttribute = new GridAttribute(mushroomDef, p, Globals.Rand.Next(100), 0);

            //            // Place it in the dungeon
            //            _level.GridAttributeMap.Add(p, mushroomAttribute);

            //            // finished with this attempt
            //            break;
            //        }
            //    }
            //}

            #endregion

            // Calculate used space before finishing
            _level.CalculateUsed();
            //_level.Reveal(false);
            return _level;
        }

        /// <summary>
        /// Creates random rooms within the specified area
        /// </summary>
        /// <param name="rooms">Output list of points inside of rooms.  These points are later used
        /// to connect rooms with tunnels.</param>
        /// <param name="areaWidth">Width of the dungeon</param>
        /// <param name="areaHeight">Height of the dungeon</param>
        void CreateRoomsV3(List<RoomInfo> rooms, int areaWidth, int areaHeight)
        {
            CreateRoomsV3(rooms, null, areaWidth, areaHeight);
        }
        void CreateRoomsV3(List<RoomInfo> rooms, List<RoomInstance> unplaced, int areaWidth, int areaHeight)
        {
            // Start by creating a large list of random rooms.  These rooms
            // have not been placed yet.
            if (unplaced == null)
            {
                unplaced = new List<RoomInstance>();

                // HACK: Create a special kind of level for the boss fight
                if (_level.Depth == BossDepth)
                {
                    RoomTypeEnclosure enclosure = new RoomTypeEnclosure();
                    for (int i = 0; i < 5; i++)
                    {
                        RoomInstance instance;
                        if (enclosure.TryCreateInstance(areaWidth, areaHeight, out instance))
                            unplaced.Add(instance);
                    }
                    RoomTypeIntersection instersection = new RoomTypeIntersection();
                    for (int i = 0; i < 5; i++)
                    {
                        RoomInstance instance;
                        if (instersection.TryCreateInstance(areaWidth, areaHeight, out instance))
                            unplaced.Add(instance);
                    }
                    Lists.RandomizeOrder(unplaced);
                }
                else
                {
                    for (int i = 0; i < CreateRoomCount; i++)
                    {
                        RoomInstance instance = null;
                        if (RoomType.GetRandomRoomType().TryCreateInstance(areaWidth, areaHeight, out instance))
                            unplaced.Add(instance);
                    }
                }
            }

            // Track all the rooms that we've placed
            List<RoomInstance> placed = new List<RoomInstance>();

            // Track the upper-left corner of all the rooms that we've placed
            List<System.Drawing.Point> origins = new List<System.Drawing.Point>();

            // The first room is always placed in the center of the dungeon
            {
                // Place the first room in the very center of the dungeon
                System.Drawing.Point origin = new System.Drawing.Point(areaWidth / 2 - unplaced[0].RoomSize.Width / 2, areaHeight / 2 - unplaced[0].RoomSize.Height / 2);

                // Draw the room onto our dungeon tiles/flags
                unplaced[0].DrawRoom(_level, origin);
                AddGoodySpots(unplaced[0], origin);

                // Remember the focus point for this room.  This is the point that we'll connect with tunnels
                unplaced[0].Register(rooms, origin);

                // Remember the upper-left corner of this room
                origins.Add(origin);

                // This room has already been placed, so remove it from further consideration
                placed.Add(unplaced[0]);
                unplaced.RemoveAt(0);
            }

            // The next block of code tries to place the remaining rooms by "sliding" them towards the center.
            // This is sortof like Tetris, but from multiple directions

            // Keep a 1-dimensional array of the height of our pseudo-Tetris grid.  We
            // recalculate the array as we slide rooms from different directions, so this
            // array must be large enough to accomodate the dungeon's width or its height
            int[] slideDistances = new int[Math.Max(areaWidth, areaHeight)];

            // Initially we use all 4 directions for sliding rooms.  As the dungeon fills
            // up, the list of directions will shrink
            List<int> slideDirections = new List<int>();
            slideDirections.AddRange(new int[] { 0, 1, 2, 3 });

            // Track the slide directions that failed during the iteration so that we can
            //remove them from consideration.  
            List<int> failedDirections = new List<int>();

            // Continue to try sliding from different directions until we run out of directions
            // or we run out of rooms to place
            while (slideDirections.Count > 0 && unplaced.Count > 0)
            {
                // Use each slide direction in succession.  We don't want to unbalance the dungeon
                // by adding multiple rooms in a row from the same direction 
                foreach (int dir in slideDirections)
                {
                    // The next block of code calculate the heights for our pseudo-Tetris grid.

                    // Assume that the dungeon is empty until proven otherwise
                    Array.Clear(slideDistances, 0, slideDistances.Length);

                    // Track the unique pseudo-Tetris height values so we can try to slide rooms
                    // into these holes
                    List<int> uniqueDistances = new List<int>();

                    // The orientation of our pseudo-Tetris grid varies depending on the direction
                    // that we're sliding
                    int scanSize = (dir == 0 || dir == 2) ? areaHeight : areaWidth;

                    // Iterate over all the rooms that have already been placed and calculate
                    // their pseudo-Tetris height with respect to the slide direction
                    for (int i = 0; i < origins.Count; i++)
                    {
                        System.Drawing.Point origin = origins[i];
                        RoomInstance room = placed[i];

                        int distance = 0, setstart = 0, setend = 0;
                        switch (dir)
                        {
                            case 0:
                                // sliding from the right, so calculate distance from the left side
                                distance = room.RoomSize.Width + origin.X;
                                setstart = origin.Y;
                                setend = Math.Min(areaHeight - 1, origin.Y + room.RoomSize.Height - 1);
                                break;
                            case 1:
                                // sliding from above, so calculate distance from the bottom
                                distance = areaHeight - origin.Y;
                                setstart = origin.X;
                                setend = Math.Min(areaWidth - 1, origin.X + room.RoomSize.Width - 1);
                                break;
                            case 2:
                                // sliding from the left, so calculate distance from the right side
                                distance = areaWidth - origin.X;
                                setstart = origin.Y;
                                setend = Math.Min(areaHeight - 1, origin.Y + room.RoomSize.Height - 1);
                                break;
                            case 3:
                                // sliding from the bottom, so calculate distance from the top
                                distance = room.RoomSize.Height + origin.Y;
                                setstart = origin.X;
                                setend = Math.Min(areaWidth - 1, origin.X + room.RoomSize.Width - 1);
                                break;
                        }

                        // set distances
                        for (int j = setstart; j <= setend; j++)
                            slideDistances[j] = Math.Max(slideDistances[j], distance);

                        // remember unique distance values
                        if (!uniqueDistances.Contains(distance))
                            uniqueDistances.Add(distance);
                    }

                    // Process the unique distances in priority order
                    uniqueDistances.Sort();
                    bool slideSuccessful = false;
                    foreach (int distance in uniqueDistances)
                    {
                        // Try to find a tier of the pseudo-Tetris grid which matches this height value
                        for (int scan = 0; scan < slideDistances.Length; scan++)
                        {
                            if (slideDistances[scan] == distance)
                            {
                                // look ahead to see how far this tier stretches
                                int tiermax = scan;
                                int strictmax = scan;
                                bool strict = true;
                                for (int j = scan + 1; j < scanSize; j++)
                                {
                                    if (slideDistances[j] <= distance)
                                        tiermax = j;
                                    else
                                        break;
                                    if (strict && slideDistances[j] == distance)
                                        strictmax = j;
                                    else
                                        strict = false;
                                }

                                // look behind to see how far this tier stretches
                                int tiermin = scan;
                                int strictmin = scan;
                                strict = false;
                                for (int j = scan - 1; j >= 0; j--)
                                {
                                    if (slideDistances[j] <= distance)
                                        tiermin = j;
                                    else
                                        break;
                                    if (strict && slideDistances[j] == distance)
                                        strictmin = j;
                                    else
                                        strict = false;
                                }

                                // find a room which will fit in this tier
                                int roomIndex = -1;
                                int slideSize = -1;
                                for (int i = 0; i < unplaced.Count; i++)
                                {
                                    if (dir == 0 || dir == 2)
                                        slideSize = unplaced[i].RoomSize.Height;
                                    else
                                        slideSize = unplaced[i].RoomSize.Width;
                                    if (slideSize <= tiermax - tiermin - 1)
                                    {
                                        roomIndex = i;
                                        break;
                                    }
                                }

                                if (roomIndex >= 0)
                                {
                                    // determine where to place the room
                                    RoomInstance room = unplaced[roomIndex];
                                    int originmin = tiermin + 1;
                                    int originmax = tiermax - slideSize;

                                    originmin = Math.Max(originmin, strictmin - slideSize + 1);
                                    originmax = Math.Min(originmax, strictmax);
                                    if (originmax >= originmin)
                                    {
                                        int originval = Globals.Rand.Next(originmin, originmax);

                                        System.Drawing.Point origin = System.Drawing.Point.Empty;
                                        switch (dir)
                                        {
                                            case 0:
                                                origin = new System.Drawing.Point(distance + 1 + Globals.Rand.Next(4), originval);
                                                break;
                                            case 1:
                                                origin = new System.Drawing.Point(originval, areaHeight - distance - Globals.Rand.Next(4) - room.RoomSize.Height - 1);
                                                break;
                                            case 2:
                                                origin = new System.Drawing.Point(areaWidth - distance - Globals.Rand.Next(4) - room.RoomSize.Width - 1, originval);
                                                break;
                                            case 3:
                                                origin = new System.Drawing.Point(originval, distance + 1 + Globals.Rand.Next(4));
                                                break;
                                        }

                                        // Make sure that the determined room position is still inside the dungeon area
                                        if (origin.X > 1 && origin.Y > 1 &&
                                            origin.X + unplaced[roomIndex].RoomSize.Width < areaWidth - 1 &&
                                            origin.Y + unplaced[roomIndex].RoomSize.Height < areaHeight - 1)
                                        {
                                            // Finish adding the room
                                            origins.Add(origin);
                                            placed.Add(room);
                                            room.Register(rooms, origin);
                                            unplaced.RemoveAt(roomIndex);
                                            room.DrawRoom(_level, origin);
                                            AddGoodySpots(room, origin);
                                            slideSuccessful = true;
                                            break;
                                        }
                                    }
                                }

                                // jump ahead of this tier once we're done
                                scan = tiermax + 1;
                            }
                        }
                        if (slideSuccessful)
                            break;
                    }

                    // did we fail to place a room?
                    if (!slideSuccessful)
                        failedDirections.Add(dir);
                }

                foreach (int side in failedDirections)
                    slideDirections.Remove(side);
                failedDirections.Clear();
            }
        }
        void ConnectRooms(List<RoomInfo> rooms, bool dirtPaths)
        {
            // Create Delaunay triangulation of rooms in order to determine which rooms should be connected
            List<Triangulator.Geometry.Point> roomPoints = new List<Triangulator.Geometry.Point>();
            foreach (RoomInfo room in rooms)
                roomPoints.Add(new Triangulator.Geometry.Point(room.Location.X, room.Location.Y));
            List<Triangulator.Geometry.Triangle> triangles = Triangulator.Delauney.Triangulate(roomPoints);

            // Collect a list of unique edges from the triangulation
            List<Edge> edges = new List<Edge>();
            bool[] edgeFlags = new bool[rooms.Count * rooms.Count];
            int[] edgePoints = new int[3];
            foreach (Triangulator.Geometry.Triangle triangle in triangles)
            {
                edgePoints[0] = triangle.p1;
                edgePoints[1] = triangle.p2;
                edgePoints[2] = triangle.p3;
                for (int i = 0; i < 3; i++)
                {
                    int n = (i + 1) % 3;
                    int dx = rooms[edgePoints[i]].Location.X - rooms[edgePoints[n]].Location.X;
                    int dy = rooms[edgePoints[i]].Location.Y - rooms[edgePoints[n]].Location.Y;
                    Edge edge = new Edge(edgePoints[i], edgePoints[n], dx * dx + dy * dy);
                    int edgeId = edge.PMin * rooms.Count + edge.PMax;
                    if (!edgeFlags[edgeId])
                    {
                        edges.Add(edge);
                        edgeFlags[edgeId] = true;
                    }
                }
            }

            // Sort the triangle edges by length.  We need to consider the
            // shortest edges first when constructing a minimum spanning tree.
            Comparison<Edge> edgeComparison = delegate (Edge edge1, Edge edge2)
            {
                return edge1.LenSq - edge2.LenSq;
            };
            edges.Sort(edgeComparison);

            // Create minimum spanning tree
            List<Edge> roomEdges = new List<Edge>();
            bool[] roomFlags = new bool[rooms.Count];
            roomEdges.Add(edges[0]);
            roomFlags[edges[0].PMin] = true;
            roomFlags[edges[0].PMax] = true;
            int connected = 2;
            while (connected < rooms.Count)
            {
                for (int i = 1; i < edges.Count; i++)
                {
                    Edge edge = edges[i];
                    if ((roomFlags[edge.PMin] || roomFlags[edge.PMax]) &&
                        (roomFlags[edge.PMin] != roomFlags[edge.PMax]))
                    {
                        roomEdges.Add(edge);
                        roomFlags[edge.PMin] = true;
                        roomFlags[edge.PMax] = true;
                        connected++;
                        break;
                    }
                }
            }

            // Add approx. 50% of the remaining edges
            MT19937 rand = Globals.Rand;
            for (int i = 1; i < edges.Count; i++)
            {
                if ((dirtPaths || rand.Next(2) == 0) && !roomEdges.Contains(edges[i]))
                    roomEdges.Add(edges[i]);
            }

            // Make tunnels to connect the rooms
            List<System.Drawing.Point> failed = new List<System.Drawing.Point>();
            for (int i = 0; i < roomEdges.Count; i++)
            {
                System.Drawing.Point p1 = rooms[roomEdges[i].PMin].Location;
                System.Drawing.Point p2 = rooms[roomEdges[i].PMax].Location;
                if (dirtPaths)
                    MakeDirtPath(p1, p2);
                else
                    MakeSimpleTunnel(p1, p2);
            }
        }
        void AddGoodySpots(RoomInstance roomInstance, System.Drawing.Point roomOrigin)
        {
            IEnumerable<System.Drawing.Point> spots = roomInstance.GoodySpots;
            if (spots != null)
                foreach (System.Drawing.Point p in spots)
                    _goodySpots.Add(new System.Drawing.Point(roomOrigin.X + p.X, roomOrigin.Y + p.Y));
        }
        
        internal static void MakeColumns(int left, int top, int width, int height, DungeonLevel level)
        {
            FixedGrid<CaveFeature> features = level.CaveFeatures;

            for (int x = 2; x < width; x += 2)
            {
                for (int y = 2; y < height; y += 2)
                {
                    int cx = left + x;
                    int cy = top + y;
                    if (features.GetValue(cx, cy) == CaveFeature.CaveFloor)
                        level.SetFeature(cx, cy, CaveFeature.Column1);
                }
            }
        }
        bool MakeDirtPath(System.Drawing.Point a, System.Drawing.Point b)
        {
            // Get a local reference to the random number generator
            MT19937 rand = Globals.Rand;

            // Use A* algorithm for pathfinding... sortof
            AStar astar = new AStar(_level.Size.Width, _level.Size.Height);

            // This function is used by the A* path-finder.  It calculates the cost of creating
            // a path between two adjacent tiles.  We adjust the costs based on various
            // factors in order to get our tunnels shaped the way we want
            AStar.GetCostDelegate measureCost = delegate (System.Drawing.Point p1, System.Drawing.Point p2, System.Drawing.Point prev)
            {
                // don't allow diagonal movement
                if (Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y) != 1)
                    return double.NaN;

                // Avoid anything that's not grass, dirt, or a shop entrance
                bool goingIntoShop = false;
                CaveFeature feature = _caveFeatures.GetValue(p2.X, p2.Y);
                goingIntoShop = (feature >= CaveFeature.ShopGeneral && feature <= CaveFeature.ShopBook);
                if (feature != CaveFeature.Grass && feature != CaveFeature.Dirt && !goingIntoShop)
                    return double.NaN;

                // We need a path at least 3 tiles wide
                if (!goingIntoShop)
                {
                    CaveFeature otherFeature = _caveFeatures.GetValue(p2.X + p2.Y - p1.Y, p2.Y + p2.X - p1.X);
                    if (otherFeature != CaveFeature.Dirt && otherFeature != CaveFeature.Grass)
                        return double.NaN;
                    otherFeature = _caveFeatures.GetValue(p2.X + p1.Y - p2.Y, p2.Y + p1.X - p2.X);
                    if (otherFeature != CaveFeature.Dirt && otherFeature != CaveFeature.Grass)
                        return double.NaN;
                }

                // Cost of making a path through this square.  Assume 2 until proven otherwise
                int penalty = 2;

                // Additional cost based on turning.  We penalize turns because we prefer straight paths
                int turnPenalty = (p2.X == (2 * p1.X - prev.X) && p2.Y == (2 * p1.Y - prev.Y)) ? 0 : 10;

                // If this is the first then there's no penalty for turning
                if (prev == System.Drawing.Point.Empty)
                    turnPenalty = 0;

                // Are we about to intersect with an existing path?
                if (feature == CaveFeature.Dirt)
                {
                    // Lower the path cost because we like intersections.  Paths
                    // should merge for short distances when possible
                    penalty = 1;
                    turnPenalty = 0;
                }

                return penalty + turnPenalty;
            };

            // Find a path between the two rooms
            NodePath<System.Drawing.Point> path = astar.FindPath(a, b, measureCost);

            // Sorry, couldn't connect these rooms.  This can happen if one of the rooms is 
            // completely surrounded by permanent rock or if the rooms were placed
            // too close to each other or the edge of the dungeon.
            if (path == null)
                return false;

            // Draw the path
            foreach (System.Drawing.Point p in path.Path)
            {
                CaveFeature feature = _caveFeatures.GetValue(p.X, p.Y);
                if (feature == CaveFeature.Grass)
                    _level.SetFeature(p.X, p.Y, CaveFeature.Dirt);
            }

            return true;
        }
        bool MakeSimpleTunnel(System.Drawing.Point a, System.Drawing.Point b)
        {
            // Get a local reference to the random number generator
            MT19937 rand = Globals.Rand;

            // Use A* algorithm for pathfinding... sortof
            AStar astar = new AStar(_level.Size.Width, _level.Size.Height);

            // This function is used by the A* path-finder.  It calculates the cost of creating
            // a tunnel between two adjacent tiles.  We adjust the costs based on various
            // factors in order to get our tunnels shaped the way we want
            AStar.GetCostDelegate measureCost = delegate (System.Drawing.Point p1, System.Drawing.Point p2, System.Drawing.Point prev)
            {
                // don't allow diagonal movement
                if (Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y) != 1)
                    return double.NaN;

                // Avoid the edge of the dungeon
                CaveFeature feature = _caveFeatures.GetValue(p2.X, p2.Y);
                if (feature == CaveFeature.PermanentWallExtra)
                    return double.NaN;

                // Avoid the edge of vaults
                if (feature == CaveFeature.PermanentWallOuter)
                    return double.NaN;

                // Avoid walls marked as solid
                if (feature == CaveFeature.GraniteWallSolid)
                    return double.NaN;

                // Cost of tunneling through this square.  Assume 2 until proven otherwise
                int penalty = 2;

                // Additional cost based on turning.  We penalize turns because we prefer straight
                // sections of tunnel
                int turnPenalty = (p2.X == (2 * p1.X - prev.X) && p2.Y == (2 * p1.Y - prev.Y)) ? 0 : 10;

                // If this is the first then there's no penalty for turning
                if (prev == System.Drawing.Point.Empty)
                    turnPenalty = 0;

                // Use perlin noise to simulate random blobs of harder rock.  We penalize tunnels
                // through these sections in order to prevent the tunnels from being too straight
                if (_noise.PerlinNoise((double)p2.X * 0.25, (double)p2.Y * 0.25) >= 0.0
                    && feature != CaveFeature.CaveFloor)

                    // The penalty for tunneling through hard rock needs to be large relative
                    // to the penalty for turning, or the pathfinder won't care enough
                    penalty += 16;

                // Do not allow the path to turn if we're inside an outer wall
                if (turnPenalty > 0 && _caveFeatures.GetValue(p1.X, p1.Y) == CaveFeature.GraniteWallOuter)
                    return double.NaN;

                // Are we about to pierce the outer wall of a room?
                if (feature == CaveFeature.GraniteWallOuter)
                {
                    // Look one step ahead so we don't enter the room in bad spots such as corners
                    int tempx = 2 * p2.X - p1.X;
                    int tempy = 2 * p2.Y - p1.Y;
                    CaveFeature tempFeature = _caveFeatures.GetValue(tempx, tempy);

                    // Avoid walls which would indicate we pierced the room in a bad spot
                    if (tempFeature == CaveFeature.PermanentWallExtra ||
                        tempFeature == CaveFeature.PermanentWallOuter ||
                        tempFeature == CaveFeature.GraniteWallSolid ||
                        tempFeature == CaveFeature.GraniteWallOuter)
                        return double.NaN;
                }

                // Are we already inside a room?
                else if (0 != (_caveFlags.GetValue(p2.X, p2.Y) & CaveFlag.IsRoom))

                    // No penalty for turning inside a room
                    turnPenalty = 0;

                // Tunneling through regular granite
                else if (feature == CaveFeature.GraniteWallExtra)
                {
                    // nothing
                }

                // We're at a tunnel intersection of some sort
                else
                {
                    // Lower the path cost because we like intersections.  Tunnels
                    // should merge for short distances when possible
                    penalty = 1;
                    turnPenalty = 0;
                }

                return penalty + turnPenalty;
            };

            // Find a path between the two rooms
            NodePath<System.Drawing.Point> path = astar.FindPath(a, b, measureCost);

            // Sorry, couldn't connect these rooms.  This can happen if one of the rooms is 
            // completely surrounded by permanent rock or if the rooms were placed
            // too close to each other or the edge of the dungeon.
            if (path == null)
                return false;

            // Track every square that we tunnel through; don't perform
            // carving until later
            List<System.Drawing.Point> tunnelPoints = new List<System.Drawing.Point>();

            // Track all the positions where we pierce into a room.  We'll
            // want to create doors at some of these positions later
            List<System.Drawing.Point> piercePoints = new List<System.Drawing.Point>();

            // Keep track of when we create doors so we don't create 2 in a row
            bool justCreatedDoor = false;

            // Walk the path and finish creating the tunnel
            foreach (System.Drawing.Point p in path.Path)
            {
                CaveFeature feature = _caveFeatures.GetValue(p.X, p.Y);

                // Pierce the edge of a room
                if (feature == CaveFeature.GraniteWallOuter)
                {
                    // Remember the position at which we pierced into the room
                    piercePoints.Add(p);

                    // Set nearby walls to solid so that we don't re-enter the room
                    // too close to this position
                    for (int tempx2 = p.X - 1; tempx2 <= p.X + 1; tempx2++)
                        for (int tempy2 = p.Y - 1; tempy2 <= p.Y + 1; tempy2++)
                            if (_caveFeatures.GetValue(tempx2, tempy2) == CaveFeature.GraniteWallOuter)
                                _level.SetFeature(tempx2, tempy2, CaveFeature.GraniteWallSolid);

                }

                // Move quickly through rooms without erasing their contents
                else if (0 != (_caveFlags.GetValue(p.X, p.Y) & CaveFlag.IsRoom))
                {
                    // nothing
                }

                // Tunnel through regular old granite
                else if (feature == CaveFeature.GraniteWallExtra)
                {
                    // Remember to tunnel through here when we've reached our destination
                    tunnelPoints.Add(p);

                    // Clear the door flag
                    justCreatedDoor = false;
                }

                // This must be an intersection of some sort
                else
                {
                    // Remember tunnel intersections so we can create doors later
                    if (!justCreatedDoor)
                        _tunnelJunctions.Add(p);

                    // Don't create doors in adjacent grids
                    justCreatedDoor = true;
                }
            }

            // Carve out the tunel
            foreach (System.Drawing.Point tunnelPoint in tunnelPoints)

                // Create a floor at this location
                _level.SetFeature(tunnelPoint.X, tunnelPoint.Y, CaveFeature.CaveFloor);

            // Carve out entrances to rooms
            foreach (System.Drawing.Point piercePoint in piercePoints)
            {
                // Create a floor at this location
                _level.SetFeature(piercePoint.X, piercePoint.Y, CaveFeature.CaveFloor);

                // Randomly create doors
                if (rand.Next(100) < TunnelPierceDoorChance)
                {
                    _level.SetFeature(piercePoint.X, piercePoint.Y, CaveFeature.DoorClosed1);
                }
            }

            // Finished creating the tunel.  Doors at intersections are added later.
            return true;
        }

        public static bool IsBetweenTwoWalls(IGrid<CaveFeature> caveFeatures, int x, int y)
        {
            // Horizontal check
            if (caveFeatures.GetValue(x - 1, y) >= CaveFeature.WallStart &&
                caveFeatures.GetValue(x + 1, y) >= CaveFeature.WallStart)
                return true;

            // Vertical check
            if (caveFeatures.GetValue(x, y - 1) >= CaveFeature.WallStart &&
                caveFeatures.GetValue(x, y + 1) >= CaveFeature.WallStart)
                return true;

            return false;
        }
        
    }
}
