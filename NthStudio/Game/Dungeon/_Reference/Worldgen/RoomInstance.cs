using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Game.Dungeon.Worldgen
{
    abstract class RoomInstance
    {
        /// <summary>
        /// Gets the position of the focal point of the room (which may or or may not be the center), relative to the room's origin (0,0). 
        /// The focal point of the room is the point which is used for connecting tunnel, etc...
        /// </summary>
        protected abstract Point RoomFocusRelative { get; }

        /// <summary>
        /// Gets the size of the room instance, which includes outer walls
        /// </summary>
        public abstract Size RoomSize { get; }

        /// <summary>
        /// Draws the room onto the level at the specified 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="roomCenter"></param>
        public abstract void DrawRoom(
            DungeonLevel level,
            Point roomOrigin);

        /// <summary>
        /// Gets whether the room should always be connected to the other rooms in the level
        /// </summary>
        public virtual bool ForceConnect { get { return true; } }

        /// <summary>
        /// Gets the style in which this room should be decorated
        /// </summary>
        protected virtual DecorateStyle Style { get { return DecorateStyle.Undefined; } }

        /// <summary>
        /// Additional style flags
        /// </summary>
        protected virtual DecorateFlags StyleFlags { get { return DecorateFlags.None; } }

        /// <summary>
        /// Get any preferred locations for creating treasure / monsters, relative to the room's origin (0,0).
        /// </summary>
        public virtual IEnumerable<Point> GoodySpots { get { return null; } }

        public virtual void Register(List<RoomInfo> rooms, Point roomOrigin)
        {
            if (ForceConnect)
            {
                Point focus = roomOrigin;
                roomOrigin.Offset(this.RoomFocusRelative);
                rooms.Add(new RoomInfo(roomOrigin, this.Style, this.StyleFlags));
            }
        }
    }
}
