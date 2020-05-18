using System;
using System.Drawing;
using System.Collections.Generic;

using NthDimension.Algebra;
using NthStudio.Game.Dungeon;
using NthStudio.Game.Dungeon.Data;

namespace NthStudio.Game.Dungeon
{
    
    internal class DungeonLevel
    {
        #region Events
        public class PointEventArgs : EventArgs
        {
            Point _p;
            public PointEventArgs(Point p)
            {
                _p = p;
            }
            public Point Location { get { return _p; } }
        }

        public delegate void PointEventHandler(object sender, PointEventArgs e);
        /// <summary>
		/// Event fired when a location is being updated and needs to be redrawn.  This event is fired
		/// when monsters move, etc...
		/// </summary>
		public event PointEventHandler UpdatingPoint;
        #endregion Events

        #region Properties
        /// <summary>
        /// Gets the level features
        /// </summary>
        public FixedGrid<CaveFeature>       CaveFeatures            { get { return _features; } }
        /// <summary>
        /// Gets the flags for each tile
        /// </summary>
		public FixedGrid<CaveFlag>          CaveFlags               { get { return _flags; } }
        /// <summary>
        /// Gets or sets the depth of the current level
        /// </summary>
        public int                          Depth                   { get { return _depth; } set { _depth = value; } }
        public string[]                     Feeling                 { get { return _feeling; } set { _feeling = value; } }
        public Dictionary<Point, 
                         GridAttribute>     GridAttributeMap        { get { return _gridAttributeMap; } }
        /// <summary>
		/// Gets the item map, which provides for quick lookup of items based on dungeon position.
		/// </summary>
		public Dictionary<Point, 
                        List<ActorItem>>    ItemMap { get { return _itemMap; } }
        /// <summary>
        /// Gets the entity map, which provides for quick lookup of entities (monsters, the player)
        /// based on dungeon position.
        /// </summary>
        public Dictionary<Point, Actor>     EntityMap               { get { return _entityMap; } }
        /// <summary>
		/// Gets the size of the dungeon area
		/// </summary>
		public Size                         Size                    { get { return _size; } }
        public Point                        StartLocation           { get { return _startLocation; } set { _startLocation = value; } }
        #endregion Properties

        #region Members
        /// <summary>
        /// Depth of the current dungeon level
        /// </summary>
        int                                 _depth;
        /// <summary>
        /// Level feeling(s)
        /// </summary>
        string[]                            _feeling;
        /// <summary>
        /// Player start location
        /// </summary>
        Point                               _startLocation;                         // TODO:: Convert to Vector3
        /// <summary>
		/// Size of the dungeon area
		/// </summary>
		Size                                _size;
        /// <summary>
        /// Rectangle representing the "used" area of the map
        /// </summary>
        Rectangle                           _usedRect;
        /// <summary>
        /// Grid of features representing the tiles of the dungeon
        /// </summary>
        FixedGrid<CaveFeature>              _features;
        /// <summary>
        /// Grid of flags holding various information about each tile of the dungeon
        /// </summary>
        FixedGrid<CaveFlag>                 _flags;
        /// <summary>
        /// Dictionary for quick lookup of entities (monsters, the player) based on 
        /// dungeon position.
        /// </summary>
        Dictionary<Point, Actor>            _entityMap              = new Dictionary<Point, Actor>();        
        /// <summary>
        /// Dictionary for quick lookup of grid attributes based on dungeon position
        /// </summary>
        Dictionary<Point, GridAttribute>    _gridAttributeMap = new Dictionary<Point, GridAttribute>();

        /// <summary>
		/// Dictionary for quick lookup of items on the floor based on dungeon position
		/// </summary>
		Dictionary<Point, List<ActorItem>> _itemMap = new Dictionary<Point, List<ActorItem>>();


        //// Gameplay
        //// --------
        ///// <summary>
        ///// List of all monsters on the level
        ///// </summary>
        //List<ActorMonster>                  _monsters             = new List<ActorMonster>();                 // TODO:: ActorMonster
        ///// <summary>
        ///// Dictionary for quick lookup of items on the floor based on dungeon position
        ///// </summary>
        //Dictionary<Point, List<ActorItem>>  _itemMap              = new Dictionary<Point, List<ActorItem>>(); // TODO ActorItem
        //List<Encounter>                     _encounters           = new List<Encounter>();                    // TODO Encounter

        #endregion

        #region Ctor
        public DungeonLevel(FixedGrid<CaveFeature> features, FixedGrid<CaveFlag> flags, Size size)
        {
            _features = features;
            _flags = flags;
            _size = size;

            // Initialize store space for flows
            _flowCost = new FixedGrid<byte>(_size.Width, _size.Height, 0);
            _flowWhen = new FixedGrid<byte>(_size.Width, _size.Height, 0);
        }

        public DungeonLevel()
        {
        }
        #endregion

        #region Methods - Flow
        FixedGrid<byte> _flowWhen;
        FixedGrid<byte> _flowCost;
        Queue<Point> _flowQueue = new Queue<Point>();
        byte _flowSave = 0;

        public IGrid<byte> FlowCost { get { return _flowCost; } }
        public IGrid<byte> FlowWhen { get { return _flowWhen; } }
        #endregion

        #region Methods - Modify Terrain
        public void CalculateUsed()
        {
            int xmin = _size.Width;
            int xmax = 0;
            int ymin = _size.Height;
            int ymax = 0;
            FeatureTile[] tiles = Globals.FeatureTiles.Entries;

            for (int x = 0; x < _size.Width; x++)
                for (int y = 0; y < _size.Height; y++)
                {
                    CaveFeature feature = _features.GetValue(x, y);
                    if (0 != (tiles[(int)feature].Flags & (FeatureFlag.AllowMove | FeatureFlag.PlayerOpen | FeatureFlag.MonsterOpen)))
                    {
                        xmin = Math.Min(xmin, x);
                        xmax = Math.Max(xmax, x);
                        ymin = Math.Min(ymin, y);
                        ymax = Math.Max(ymax, y);
                    }
                }
            xmin--;
            ymin--;
            xmax++;
            ymax++;
            _usedRect = new Rectangle(xmin, ymin, xmax - xmin + 1, ymax - ymin + 1);
        }

        /// <summary>
        /// Sets the feature at the specified coordinates.  Also updates any relevant cave flags
        /// for the specified tile.
        /// </summary>
        public void SetFeature(int x, int y, CaveFeature feature)
        {
            // Change the feature
            _features.SetValue(x, y, feature);

            // Block line-of-sight for walls, doors, etc...
            if (0 != (Globals.FeatureTiles.Entries[(int)feature].Flags & FeatureFlag.BlockLOS))
                _flags.SetValue(x, y, _flags.GetValue(x, y) | CaveFlag.BlocksLOS);
            else
                _flags.SetValue(x, y, _flags.GetValue(x, y) & (~CaveFlag.BlocksLOS));

            // TODO: notice changes
            if (UpdatingPoint != null)
                UpdatingPoint(this, new PointEventArgs(new Point(x, y)));
        }
        /// <summary>
        /// Fills the specified rectangle with a feature.  Also updates any relevant cave flags
        /// for the affected tiles.
        /// </summary>
        public void SetFeature(int left, int top, int width, int height, CaveFeature feature)
        {
            for (int y = top; y < top + height; y++)
                for (int x = left; x < left + width; x++)
                    SetFeature(x, y, feature);

            // TODO: We might need to notice changes if this
            // code is ever called after level generation finishes
        }

        /// <summary>
        /// Sets or clears one or more flags for the specified rectangle.
        /// </summary>
        /// <param name="left">Left side of the rectangle</param>
        /// <param name="top">Top of the rectangle</param>
        /// <param name="width">Width of the rectangle</param>
        /// <param name="height">Height of the rectangle</param>
        /// <param name="flags">Flags to set or clear</param>
        /// <param name="set">If true then the flags will be set.  Otherwise, the specified flags will be cleared.</param>
        public void SetCaveFlags(int left, int top, int width, int height,
            CaveFlag flags, bool set)
        {
            if (set)
            {
                for (int x = left; x < left + width; x++)
                    for (int y = top; y < top + height; y++)
                        _flags.SetValue(x, y, _flags.GetValue(x, y) | flags);
            }
            else
            {
                CaveFlag notFlags = ~flags;
                for (int x = left; x < left + width; x++)
                    for (int y = top; y < top + height; y++)
                        _flags.SetValue(x, y, _flags.GetValue(x, y) & notFlags);
            }
        }

        /// <summary>
        /// Sets or clears one or more flags for the specified point.
        /// </summary>
        public void SetCaveFlags(int x, int y, CaveFlag flags, bool set)
        {
            if (set)
                _flags.SetValue(x, y, _flags.GetValue(x, y) | flags);
            else
            {
                CaveFlag notFlags = ~flags;
                _flags.SetValue(x, y, _flags.GetValue(x, y) & notFlags);
            }
        }

        #endregion Methods - Modify Terrain
    }
}
