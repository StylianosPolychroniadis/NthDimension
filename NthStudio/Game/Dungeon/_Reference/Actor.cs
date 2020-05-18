
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Game.Dungeon
{
    using NthDimension.Algebra;

    /// <summary>
    /// Actors are associated with anything that can be placed on the dungeon map which
    /// is not terrain.  Monsters, the player, objects, etc...
    /// </summary>
    internal class Actor
    {
        /// <summary>
        /// Stores the x,y coordinates of this actor
        /// </summary>
        Vector3                 _location;

        /// <summary>
        /// Store the default font char index used to display this actor
        /// </summary>
        byte                    _fontChar;

        /// <summary>
        /// Store the default color index used to display this actor
        /// </summary>
        byte                    _color;

        /// <summary>
        /// Store the default tile index used to display this actor
        /// </summary>
        int                     _tile;

        /// <summary>
        /// Flags for specifying various actor properties
        /// </summary>
        ActorFlag               _flags;

        public Actor(Vector3 location, byte fontChar, byte color, int tile, ActorFlag flags)
        {
            _location = location;
            _fontChar = fontChar;
            _color = color;
            _flags = flags;
            _tile = tile;
        }

        /// <summary>
        /// Gets or sets the location of this actor in dungeon coordinates.
        /// </summary>
        public virtual Vector3 Location
        {
            get { return _location; }
            set { _location = value; }
        }

        /// <summary>
        /// Gets the default color index used to display this actor on the dungeon map
        /// </summary>
        public byte Color { get { return _color; } }

        /// <summary>
        /// Gets the default tile index used to display this actor on the dungeon map
        /// </summary>
        public byte FontChar { get { return _fontChar; } }

        /// <summary>
        /// Gets the default tile index used to display this actor on the dungeon map
        /// </summary>
        public virtual int Tile { get { return _tile; } }

        /// <summary>
        /// Set the actor's location and bypass any associated changed events.  Used
        /// when loading from a saved game
        /// </summary>
        /// <param name="p">New location</param>
        protected void SetLocationDirect(Vector3 p)
        {
            _location = p;
        }
    }

    [Flags()]
    enum ActorFlag
    {
        None = 0,

        /// <summary>
        /// The tile used to display the actor changes randomly
        /// </summary>
        MorphTile = 1,

        /// <summary>
        /// The color used to display the actor changes randomly
        /// </summary>
        MorphColor = 2
    }
}
