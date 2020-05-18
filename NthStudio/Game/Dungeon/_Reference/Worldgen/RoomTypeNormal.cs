using System;
using System.Drawing;

namespace NthStudio.Game.Dungeon.Worldgen
{
    /// <summary>
    /// Plain, boring rectangular room
    /// </summary>
    class RoomTypeNormal : RoomType
    {
        /// <summary>
        /// Maximum size of the room, including outer walls
        /// </summary>
        const int RoomSizeMax = 12;

        /// <summary>
        /// Minimum size of the room, including outer walls
        /// </summary>
        const int RoomSizeMin = 5;

        public RoomTypeNormal()
        {
        }

        public override int ChooseFrequency { get { return NormalRoomFrequency; } }

        public override bool TryCreateInstance(int maxWidth, int maxHeight, out RoomInstance instance)
        {
            instance = null;
            MT19937 rand = Globals.Rand;

            // Occasionally try to create an overlapped room
            if (Globals.Rand.Next(3) == 0 && maxWidth >= RoomSizeMin + 2 && maxHeight >= RoomSizeMin + 2)
            {
                maxWidth = Math.Min(maxWidth, RoomSizeMax);
                maxHeight = Math.Min(maxHeight, RoomSizeMax);
                Point center = new Point(maxWidth >> 1, maxHeight >> 1);
                Point p1 = new Point(
                    center.X - Globals.Rand.Next(1, center.X - 1),
                    center.Y - Globals.Rand.Next(1, center.Y - 1));
                Point p2 = new Point(
                    center.X + Globals.Rand.Next(1, maxWidth - 2 - center.X),
                    center.Y + Globals.Rand.Next(1, maxHeight - 2 - center.Y));
                Point p3 = new Point(
                    center.X - Globals.Rand.Next(1, center.X - 1),
                    center.Y - Globals.Rand.Next(1, center.Y - 1));
                Point p4 = new Point(
                    center.X + Globals.Rand.Next(1, maxWidth - 2 - center.X),
                    center.Y + Globals.Rand.Next(1, maxHeight - 2 - center.Y));

                instance = new RoomInstanceNormal(p1, p2, p3, p4);
                return true;
            }

            // Ensure that we have the minimum size needed to create the room
            if (maxWidth < RoomSizeMin || maxHeight < RoomSizeMin)
                return false;

            // Pick a random size for the room
            int width, height;
            maxWidth = Math.Min(RoomSizeMax, maxWidth);
            maxHeight = Math.Min(RoomSizeMax, maxHeight);
            if (Globals.Rand.Next(3) > 0)
                maxWidth = Math.Min(maxWidth, RoomSizeMax / 2);
            if (Globals.Rand.Next(3) > 0)
                maxHeight = Math.Min(maxHeight, RoomSizeMax / 2);
            width = Dice.Throw(2, RoomSizeMin, maxWidth);
            height = Dice.Throw(2, RoomSizeMin, maxHeight);

            // Create the room instance
            instance = new RoomInstanceNormal(new Size(width, height));
            return true;
        }
    }
}
