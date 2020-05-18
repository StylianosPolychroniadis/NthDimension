using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Game.Dungeon.Worldgen
{
    class RoomTypeIntersection : RoomType
    {
        public const int RoomSize = 8;

        public RoomTypeIntersection()
        {
        }

        public override int ChooseFrequency { get { return NormalRoomFrequency / 5; } }
        //public override int ChooseFrequency { get { return NormalRoomFrequency *5; } }

        public override bool TryCreateInstance(int maxWidth, int maxHeight, out RoomInstance instance)
        {
            instance = null;
            if (maxWidth < RoomSize ||
                maxHeight < RoomSize)
                return false;
            instance = new RoomInstanceIntersection();
            return true;
        }
    }
}
