using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Game.Dungeon.Worldgen
{
    class RoomInstanceEnclosure : RoomInstance
    {
        RoomInstance _normal1, _normal2;
        Size _totalSize;
        Point _room1Position, _room2Position;

        public RoomInstanceEnclosure(RoomInstance normal1, RoomInstance normal2,
            Point room1Position, Point room2Position, Size totalSize)
        {
            _totalSize = totalSize;
            _normal1 = normal1;
            _normal2 = normal2;
            _room1Position = room1Position;
            _room2Position = room2Position;
        }

        public override void DrawRoom(DungeonLevel level, Point roomOrigin)
        {
            // Draw the hallway surrounding them
            {
                int top1, left1, right1, bottom1;
                GetOuterHallBorders(_room1Position, _normal1.RoomSize,
                    out left1, out top1, out right1, out bottom1);
                left1 += roomOrigin.X;
                right1 += roomOrigin.X;
                top1 += roomOrigin.Y;
                bottom1 += roomOrigin.Y;

                int top2, left2, right2, bottom2;
                GetOuterHallBorders(_room2Position, _normal2.RoomSize,
                    out left2, out top2, out right2, out bottom2);
                left2 += roomOrigin.X;
                right2 += roomOrigin.X;
                top2 += roomOrigin.Y;
                bottom2 += roomOrigin.Y;

                // set flags
                CaveFlag roomFlag = CaveFlag.IsRoom;
                if (level.Depth <= Globals.Rand.Next(25))
                    roomFlag |= CaveFlag.Glowing;

                level.SetCaveFlags(left1, top1, right1 - left1 + 1, bottom1 - top1 + 1, roomFlag, true);
                level.SetCaveFlags(left2, top2, right2 - left2 + 1, bottom2 - top2 + 1, roomFlag, true);

//// draw outer wall
//Utility.GridUtil.DrawRectangle(level.CaveFeatures, left1, top1, right1 - left1 + 1, bottom1 - top1 + 1, CaveFeature.GraniteWallOuter);
//Utility.GridUtil.DrawRectangle(level.CaveFeatures, left2, top2, right2 - left2 + 1, bottom2 - top2 + 1, CaveFeature.GraniteWallOuter);

                // draw the inner hallway
                top1 += 1;
                top2 += 1;
                left1 += 1;
                left2 += 1;
                right1 -= 1;
                right2 -= 1;
                bottom1 -= 1;
                bottom2 -= 1;
                level.SetFeature(left1, top1, right1 - left1 + 1, bottom1 - top1 + 1, CaveFeature.CaveFloor);
                level.SetFeature(left2, top2, right2 - left2 + 1, bottom2 - top2 + 1, CaveFeature.CaveFloor);

                // draw columns
                DungeonGenerator.MakeColumns(left1 - 1, top1 - 1, right1 - left1 + 3, bottom1 - top1 + 3, level);
                DungeonGenerator.MakeColumns(left2 - 1, top2 - 1, right2 - left2 + 3, bottom2 - top2 + 3, level);

                // get ready to draw inner wall
                top1 += 3;
                top2 += 3;
                left1 += 3;
                left2 += 3;
                right1 -= 3;
                right2 -= 3;
                bottom1 -= 3;
                bottom2 -= 3;

                // Draw inner wall
                level.SetFeature(left1, top1, right1 - left1 + 1, bottom1 - top1 + 1, CaveFeature.GraniteWallOuter);
                level.SetFeature(left2, top2, right2 - left2 + 1, bottom2 - top2 + 1, CaveFeature.GraniteWallOuter);

                // draw inner space
                top1 += 1;
                top2 += 1;
                left1 += 1;
                left2 += 1;
                right1 -= 1;
                right2 -= 1;
                bottom1 -= 1;
                bottom2 -= 1;
                level.SetCaveFlags(left1, top1, right1 - left1 + 1, bottom1 - top1 + 1, roomFlag, false);
                level.SetCaveFlags(left2, top2, right2 - left2 + 1, bottom2 - top2 + 1, roomFlag, false);
                level.SetFeature(left1, top1, right1 - left1 + 1, bottom1 - top1 + 1, CaveFeature.GraniteWallExtra);
                level.SetFeature(left2, top2, right2 - left2 + 1, bottom2 - top2 + 1, CaveFeature.GraniteWallExtra);
            }

            // Draw the enclosed rooms
            {
                Point origin1 = _room1Position;
                Point origin2 = _room2Position;
                origin1.Offset(roomOrigin);
                origin2.Offset(roomOrigin);
                _normal1.DrawRoom(level, origin1);
                _normal2.DrawRoom(level, origin2);
            }
        }

        public static void GetOuterHallBorders(Point roomPosition, Size roomSize,
            out int left, out int top, out int right, out int bottom)
        {
            top = roomPosition.Y;
            left = roomPosition.X;
            right = roomPosition.X + roomSize.Width - 1;
            bottom = roomPosition.Y + roomSize.Height - 1;

            ExpandBorders(ref top, ref left, ref right, ref bottom);
        }

        private static void ExpandBorders(ref int top, ref int left, ref int right, ref int bottom)
        {
            top -= 6;
            left -= 6;
            right += 6;
            bottom += 6;
            if (0 != (top & 1))
                top--;
            if (0 != (left & 1))
                left--;
            if (0 != (right & 1))
                right++;
            if (0 != (bottom & 1))
                bottom++;
        }

        protected override Point RoomFocusRelative
        {
            get { throw new NotImplementedException(); }
        }

        public override Size RoomSize
        {
            get { return _totalSize; }
        }

        public override void Register(List<RoomInfo> rooms, Point roomOrigin)
        {
            Point origin1 = _room1Position;
            Point origin2 = _room2Position;
            origin1.Offset(roomOrigin);
            origin2.Offset(roomOrigin);
            _normal1.Register(rooms, origin1);
            _normal2.Register(rooms, origin2);
        }
    }
}
