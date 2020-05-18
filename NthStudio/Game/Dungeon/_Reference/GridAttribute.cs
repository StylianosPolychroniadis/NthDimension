using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NthStudio.Game.Dungeon
{
    public enum GridAttributeType
    {
        DoorHidden,
        DoorLocked,
        DoorElectrified,
        Portal,
        Misc,
        Mushroom,
        FountainFull,
        FountainEmpty,

        TrapStart,
        FloorTrapStart,
        FloorTrapPit,
        FloorTrapDart,
        FloorTrapTeleport,
        FloorTrapNoise,
        FloorTrapUndead,
        FloorTrapDoor,
        FloorTrapScorch,
        FloorTrapEnd,
        DoorTrapStart,
        DoorTrapNeedle,
        DoorTrapShock,
        DoorTrapExplode,
        DoorTrapEnd,
        TrapEnd,
    }

    /// <summary>
    /// XML-Serializable class
    /// </summary>
    [Serializable()]
    public class GridAttribute
    {
        bool                    _marked;
        int                     _skill1, _skill2;
        Point                   _location;
        string                  _name;
        GridAttributeDef        _def;

        public GridAttribute()
        {
        }

        internal GridAttribute(GridAttributeDef def, Point location, int skill1, int skill2)
        {
            _def = def;
            _name = _def.Name;
            _location = location;
            _skill1 = skill1;
            _skill2 = skill2;
        }

        [XmlAttribute("name")]
        public string Name { get { return _name; } set { _name = value; } }

        /// <summary>
        /// Gets or sets whether the attribute has been detected by the player.  This property does
        /// not apply to all attributes
        /// </summary>
        [XmlAttribute("marked")]
        public bool Marked { get { return _marked; } set { _marked = value; } }

        /// <summary>
        /// Gets or sets the skill level associated with this attribute.  This property does not
        /// apply to all attributes
        /// </summary>
        [XmlAttribute("skill1")]
        public int Skill1 { get { return _skill1; } set { _skill1 = value; } }

        /// <summary>
        /// Gets or sets the skill level associated with this attribute.  This property does not
        /// apply to all attributes
        /// </summary>
        [XmlAttribute("skill2")]
        public int Skill2 { get { return _skill2; } set { _skill2 = value; } }

        [XmlAttribute("x")]
        public int X { get { return _location.X; } set { _location.X = value; } }

        [XmlAttribute("y")]
        public int Y { get { return _location.Y; } set { _location.Y = value; } }

        [XmlIgnore()]
        public Point Location { get { return _location; } set { _location = value; } }

        [XmlIgnore()]
        internal GridAttributeDef GridAttributeDef { get { return _def; } }

        public void FindDef()
        {
            if (string.IsNullOrEmpty(_name))
                throw new ApplicationException("Unable to load GridAttribute because the name is undefined.");
            _def = GridAttributeDef.GetDef(_name);
            if (_def == null)
                throw new ApplicationException("Unable to load GridAttribute because the name was unrecognized: " + _name);
        }
    }

    internal class GridAttributeDef
    {
        string                  _name;
        int                     _tileIndex  = -1;
        GridAttributeType       _type;
        bool                    _visible    = false;
        string                  _desc;

        public bool Visible { get { return _visible; } }

        private GridAttributeDef(string name, GridAttributeType type)
            : this(name, null, null, type, false)
        {
        }
        private GridAttributeDef(string name, string tileName, GridAttributeType type, bool visible)
            : this(name, tileName, null, type, visible)
        {
        }

        private GridAttributeDef(string name, string tileName, string description, GridAttributeType type, bool visible)
        {
            _name = name;
            _visible = visible;
            _desc = description;
            if (!string.IsNullOrEmpty(tileName))
            {
                throw new NotImplementedException("Globals.TileDefinitions.TileNames.TryGetValue(tileName, out _tileIndex))");
                //if (!Globals.TileDefinitions.TileNames.TryGetValue(tileName, out _tileIndex))
                //    throw new ApplicationException("Failed to initialize GridAttribute definitions because the tile name was unrecognized: " + tileName);
            }
            _type = type;
        }

        public string Name { get { return _name; } }
        public string Description { get { return _desc; } }
        public int TileIndex { get { return _tileIndex; } }
        public GridAttributeType Type { get { return _type; } }

        static Dictionary<string, GridAttributeDef> Entries;
        public static GridAttributeDef DoorHidden;
        public static GridAttributeDef DoorLocked;
        public static GridAttributeDef Portal;
        public static GridAttributeDef Rubble;
        public static GridAttributeDef DoorElectrified;
        public static List<GridAttributeDef> FloorTraps;
        public static List<GridAttributeDef> DoorTraps;
        public static List<GridAttributeDef> JunkDecorations;
        public static GridAttributeDef Mushroom;
        public static GridAttributeDef FountainFull;
        public static GridAttributeDef FountainEmpty;

        static GridAttributeDef()
        {
            Entries = new Dictionary<string, GridAttributeDef>();

            DoorHidden = new GridAttributeDef("DoorHidden", GridAttributeType.DoorHidden);
            Entries.Add(DoorHidden.Name, DoorHidden);

            DoorLocked = new GridAttributeDef("DoorLocked", GridAttributeType.DoorLocked);
            Entries.Add(DoorLocked.Name, DoorLocked);

            Portal = new GridAttributeDef("Portal", "Portal", GridAttributeType.Portal, true);
            Entries.Add(Portal.Name, Portal);

            Rubble = new GridAttributeDef("Rubble", "Rubble", "a pile of rubble", GridAttributeType.Misc, true);
            Entries.Add(Rubble.Name, Rubble);

            DoorElectrified = new GridAttributeDef("DoorElectrified", GridAttributeType.DoorElectrified);
            Entries.Add(DoorElectrified.Name, DoorElectrified);

            Mushroom = new GridAttributeDef("Mushroom", GridAttributeType.Mushroom);
            Entries.Add(Mushroom.Name, Mushroom);

            FountainFull = new GridAttributeDef("FountainFull", "FountainFull", "a fountain", GridAttributeType.FountainFull, true);
            Entries.Add(FountainFull.Name, FountainFull);

            FountainEmpty = new GridAttributeDef("FountainEmpty", "FountainEmpty", "an empty fountain", GridAttributeType.FountainEmpty, true);
            Entries.Add(FountainEmpty.Name, FountainEmpty);

            // Junk decorations
            {
                GridAttributeDef junk;
                JunkDecorations = new List<GridAttributeDef>();

                junk = new GridAttributeDef("JunkSkull", "Junk Skull", "a broken skull", GridAttributeType.Misc, true);
                JunkDecorations.Add(junk);

                junk = new GridAttributeDef("JunkBrokenBone", "Junk Broken Bone", "a broken bone", GridAttributeType.Misc, true);
                JunkDecorations.Add(junk);

                junk = new GridAttributeDef("JunkLargeSkeleton", "Junk Large Skeleton", "a large skeleton", GridAttributeType.Misc, true);
                JunkDecorations.Add(junk);

                junk = new GridAttributeDef("JunkMediumSkeleton1", "Junk Medium Skeleton 1", "a human skeleton", GridAttributeType.Misc, true);
                JunkDecorations.Add(junk);

                junk = new GridAttributeDef("JunkMediumSkeleton2", "Junk Medium Skeleton 2", "a human skeleton", GridAttributeType.Misc, true);
                JunkDecorations.Add(junk);

                junk = new GridAttributeDef("JunkSmallSkeleton", "Junk Small Skeleton", "a small skeleton", GridAttributeType.Misc, true);
                JunkDecorations.Add(junk);

                junk = new GridAttributeDef("JunkCanineSkeleton", "Junk Canine Skeleton", "a canine skeleton", GridAttributeType.Misc, true);
                JunkDecorations.Add(junk);

                foreach (GridAttributeDef def in JunkDecorations)
                    Entries.Add(def.Name, def);
            }

            // Traps on the floor
            {
                GridAttributeDef trap;
                FloorTraps = new List<GridAttributeDef>();

                trap = new GridAttributeDef("FloorTrapPit", "Trap Pit", "a pit", GridAttributeType.FloorTrapPit, false);
                FloorTraps.Add(trap);

                trap = new GridAttributeDef("FloorTrapDart", "Trap Dart", "a poison dart trap", GridAttributeType.FloorTrapDart, false);
                FloorTraps.Add(trap);

                trap = new GridAttributeDef("FloorTrapTeleport", "Trap Rune Teleport", "a strange rune", GridAttributeType.FloorTrapTeleport, false);
                FloorTraps.Add(trap);

                trap = new GridAttributeDef("FloorTrapNoise", "Trap Rune Noise", "a strange rune", GridAttributeType.FloorTrapNoise, false);
                FloorTraps.Add(trap);

                trap = new GridAttributeDef("FloorTrapUndead", "Trap Rune Summon", "a strange rune", GridAttributeType.FloorTrapUndead, false);
                FloorTraps.Add(trap);

                trap = new GridAttributeDef("FloorTrapDoor", "Trap Door", "a trap door", GridAttributeType.FloorTrapDoor, false);
                FloorTraps.Add(trap);

                trap = new GridAttributeDef("FloorTrapScorch", "Trap Scorch", "a scorch mark", GridAttributeType.FloorTrapScorch, false);
                FloorTraps.Add(trap);

                foreach (GridAttributeDef def in FloorTraps)
                    Entries.Add(def.Name, def);
            }

            // Traps on doors
            {
                GridAttributeDef trap;
                DoorTraps = new List<GridAttributeDef>();

                trap = new GridAttributeDef("DoorTrapNeedle", "Exclamation", "a trapped door", GridAttributeType.DoorTrapNeedle, false);
                DoorTraps.Add(trap);

                trap = new GridAttributeDef("DoorTrapShock", "Exclamation", "a trapped door", GridAttributeType.DoorTrapShock, false);
                DoorTraps.Add(trap);

                trap = new GridAttributeDef("DoorTrapExplode", "Exclamation", "a trapped door", GridAttributeType.DoorTrapExplode, false);
                DoorTraps.Add(trap);

                foreach (GridAttributeDef def in DoorTraps)
                    Entries.Add(def.Name, def);
            }
        }

        public static GridAttributeDef GetDef(string name)
        {
            GridAttributeDef def = null;
            Entries.TryGetValue(name, out def);
            return def;
        }
    }
}
