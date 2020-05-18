using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Game.Dungeon.Worldgen
{
    class RoomInstanceIntersection : RoomInstance
    {
        public RoomInstanceIntersection()
        {
        }

        public override Size RoomSize
        {
            get { return new Size(RoomTypeIntersection.RoomSize, RoomTypeIntersection.RoomSize); }
        }

        protected override Point RoomFocusRelative
        {
            get { return new Point(RoomTypeIntersection.RoomSize / 2, RoomTypeIntersection.RoomSize / 2); }
        }

        public override void DrawRoom(DungeonLevel level, Point roomOrigin)
        {
            // do nothing
        }
    }
}
