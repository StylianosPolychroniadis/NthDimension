using NthDimension.Collections;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Game.Dungeon.Worldgen
{
    class RoomTypeCavern : RoomType
    {
        const int MaxDigCount = 400;
        const int MinDigCount = 100;

        public RoomTypeCavern()
        {
        }

        public override int ChooseFrequency { get { return NormalRoomFrequency / 20; } }

        public override bool TryCreateInstance(int maxWidth, int maxHeight, out RoomInstance instance)
        {
            // Get local reference to the random number generator
            MT19937 rand = Globals.Rand;

            // Use perlin noise for digging out a random cavern
            Perlin perlin = new Perlin(rand.Next(10000), 4);

            // Set output room instance
            instance = null;

            // Choose a random number of tiles to dig
            int digCount = rand.Next(MaxDigCount - MinDigCount) + MinDigCount;

            // We create a natural-looking cavern by using perlin noise to decide which
            // direction to dig in.  Noise values are stored in a priority queue
            ExpandableGrid<byte> mask = new ExpandableGrid<byte>(0);
            PriorityQueue<double, Point> digQueue = new PriorityQueue<double, Point>(true);
            int minx, miny, maxx, maxy;
            minx = miny = maxx = maxy = 0;

            // Start digging at the room origin
            digQueue.Add(0.0, new Point(0, 0));

            // The room origin has already been dug out
            mask.SetValue(0, 0, 3);

            // Dig out the specified number of tiles
            for (int i = 0; i < digCount; i++)
            {
                // Get the next tile in the dig queue
                KeyValuePair<double, Point> pair = digQueue.Pop();

                // This tile is now marked as being dug out
                mask.SetValue(pair.Value.X, pair.Value.Y, 3);

                // Keep track of the minimum and maximum extent of the cavern
                minx = Math.Min(minx, pair.Value.X);
                miny = Math.Min(miny, pair.Value.Y);
                maxx = Math.Max(maxx, pair.Value.X);
                maxy = Math.Max(maxy, pair.Value.Y);

                // Examine neighboring tiles in all 8 directions
                for (int j = 0; j < 8; j++)
                {
                    int x2 = pair.Value.X + Direction.DirX[j];
                    int y2 = pair.Value.Y + Direction.DirY[j];

                    // Only look at tiles which have not been dug out yet or queued
                    if (mask.GetValue(x2, y2) <= 1)
                    {
                        // Only queue tiles in cardinal directions - results are better this way
                        if (0 == (j & 1))
                        {
                            digQueue.Add(perlin.PerlinNoise(x2 * 0.1, y2 * 0.1), new Point(x2, y2));
                            mask.SetValue(x2, y2, 2);
                        }

                        // Mark diagonals so we can create walls later
                        else
                            mask.SetValue(x2, y2, 1);
                    }
                }
            }
            int roomWidth = maxx - minx + 3;
            int roomHeight = maxy - miny + 3;
            if (roomWidth > maxWidth ||
                roomHeight > maxHeight)
                return false;
            instance = new RoomInstanceCavern(mask, minx, miny, new Size(roomWidth, roomHeight));
            return true;
        }
    }
}
