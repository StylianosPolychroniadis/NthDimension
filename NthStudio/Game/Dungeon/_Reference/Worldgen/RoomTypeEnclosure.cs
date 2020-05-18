using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Game.Dungeon.Worldgen
{
    /// <summary>
    /// Two rooms enclosed by a large hallway
    /// </summary>
    class RoomTypeEnclosure : RoomType
    {
        public RoomTypeEnclosure()
        {
        }

        public override int ChooseFrequency
        {
            get { return 1 + NormalRoomFrequency / 50; }
        }

        public override bool TryCreateInstance(int maxWidth, int maxHeight, out RoomInstance instance)
        {
            instance = null;

            // Create two normal rooms which will be enclosed
            RoomTypeNormal normalType = new RoomTypeNormal();
            RoomInstance normal1, normal2;
            if (!normalType.TryCreateInstance(999, 999, out normal1))
                return false;
            if (!normalType.TryCreateInstance(999, 999, out normal2))
                return false;

            // Determine how the second room should be placed relative to the first
            Size size1 = normal1.RoomSize;
            Size size2 = normal2.RoomSize;
            Point room1Position = new Point(0, 0);
            Point room2Position;
            {
                int minx, miny, maxx, maxy;

                // On the right side
                if (Globals.OneIn(2))
                {
                    minx = maxx = size1.Width + 1;
                    miny = 0 - size2.Height - 1;
                    maxy = size1.Height + 1;
                }

                // On top
                else
                {
                    miny = maxy = 0 - size2.Height - 1;
                    minx = 0 - size2.Width - 1;
                    maxx = size1.Width + 1;
                }

                room2Position = new Point(
                    Globals.RandRange(minx, maxx),
                    Globals.RandRange(miny, maxy));
            }

            // Move rooms so that upper-left corner is at 0,0
            Point offset = new Point(
                    -Math.Min(room1Position.X, room2Position.X),
                    -Math.Min(room1Position.Y, room2Position.Y));
            room1Position.Offset(offset);
            room2Position.Offset(offset);

            // Offset room origins again to account for hallway
            offset = new Point(6, 6);
            room1Position.Offset(offset);
            room2Position.Offset(offset);

            // Determine the total enclosure size
            int top1, left1, right1, bottom1;
            RoomInstanceEnclosure.GetOuterHallBorders(room1Position, size1,
                out left1, out top1, out right1, out bottom1);
            int top2, left2, right2, bottom2;
            RoomInstanceEnclosure.GetOuterHallBorders(room2Position, size2,
                out left2, out top2, out right2, out bottom2);
            int left = Math.Min(left1, left2);
            int top = Math.Min(top1, top2);
            int right = Math.Max(right1, right2);
            int bottom = Math.Max(bottom1, bottom2);
            Size totalSize = new Size(right - left + 1, bottom - top + 1);

            // Enforce size constraints
            if (totalSize.Width > maxWidth ||
                totalSize.Height > maxHeight)
                return false;

            instance = new RoomInstanceEnclosure(normal1, normal2, room1Position, room2Position, totalSize);
            return true;
        }
    }
}
