using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Game.Dungeon.Worldgen
{
    abstract class RoomType : IChoosable
    {
        static List<RoomType> AllRoomTypes;

        static RoomType()
        {
            // Collect a list of all the room types
            AllRoomTypes = new List<RoomType>();
            AllRoomTypes.Add(new RoomTypeNormal());
            AllRoomTypes.Add(new RoomTypeFromFile());
            AllRoomTypes.Add(new RoomTypeIntersection());
            AllRoomTypes.Add(new RoomTypeCavern());
            AllRoomTypes.Add(new RoomTypeEnclosure());
        }

        /// <summary>
        /// Defines the room frequency of normal, boring rooms.  This frequency is used as a baseline for
        /// other room types to define their own frequencies against.
        /// </summary>
        protected const int NormalRoomFrequency = 100;

        /// <summary>
        /// Gets the relative frequency of this room type.  Higher numbers are more frequent.
        /// </summary>
        public abstract int ChooseFrequency { get; }

        /// <summary>
        /// Attempts to create an instance of this RoomType with the specified maximum dimensions
        /// </summary>
        /// <param name="width">Maximum width of the room, including the outer walls</param>
        /// <param name="height">Maximum height of the room, including the outer walls</param>
        /// <param name="instance">Room instance than was created</param>
        /// <returns>Returns true if an instance could be created</returns>
        public abstract bool TryCreateInstance(int maxWidth, int maxHeight, out RoomInstance instance);

        /// <summary>
        /// Chooses a random room type
        /// </summary>
        /// <returns>Random room type</returns>
        public static RoomType GetRandomRoomType()
        {
            return Chooser.ChooseOne(AllRoomTypes);
        }
    }
}
