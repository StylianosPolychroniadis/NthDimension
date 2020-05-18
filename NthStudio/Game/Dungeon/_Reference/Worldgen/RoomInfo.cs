using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Game.Dungeon.Worldgen
{
    class RoomInfo
    {
        public Point Location;
        public DecorateStyle Style;
        public DecorateFlags Flags;

        public RoomInfo(Point location, DecorateStyle style, DecorateFlags flags)
        {
            Location = location;
            Style = style;
            Flags = flags;
        }
    }
}
