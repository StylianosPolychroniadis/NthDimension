using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Game.Dungeon.Worldgen
{
    class RoomTypeFromFile : RoomType
    {
        const int TryCreateRoomCount = 10;

        public RoomTypeFromFile()
        {
        }

        public override int ChooseFrequency
        {
            get { return NormalRoomFrequency * 7 / 100; }
        }

        void Rotate90(ref int xmx, ref int xmy, ref int ymx, ref int ymy)
        {
            int oldymx = ymx;
            int oldymy = ymy;
            ymx = xmx;
            ymy = xmy;
            xmx = oldymx * -1;
            xmy = oldymy * -1;
        }

        public override bool TryCreateInstance(int maxWidth, int maxHeight, out RoomInstance instance)
        {
            instance = null;

            // Get all the room definitions
            List<Data.RoomDef> defs = Globals.RoomDefinitions;

            // Try up to 10 times to create a room that fits in the provided area
            for (int i = 0; i < TryCreateRoomCount; i++)
            {
                // Choose a random room definition
                Data.RoomDef def = Chooser.ChooseOne(defs);

                // Choose a random orientation
                int face = Globals.Rand.Next(4);

                int xmx = 1, xmy = 0, ymx = 0, ymy = 1;
                for (int j = 0; j < face; j++)
                    Rotate90(ref xmx, ref xmy, ref ymx, ref ymy);

                // Determine the size of the rotated room
                int width = def.Size.Width;
                int height = def.Size.Height;
                if (xmy != 0)
                {
                    int temp = width;
                    width = height;
                    height = temp;
                }

                // If the room doesn't fit then try 1 more rotatation
                if (width > maxWidth || height > maxHeight)
                {
                    Rotate90(ref xmx, ref xmy, ref ymx, ref ymy);
                    int temp = width;
                    width = height;
                    height = temp;
                }

                // Do we still fit?
                if (width > maxWidth || height > maxHeight)
                    continue;

                // Return the room instance
                instance = new RoomInstanceFromFile(def, new System.Drawing.Size(width, height), xmx, xmy, ymx, ymy);
                return true;
            }

            // Failed to create a room
            return false;
        }
    }
}
