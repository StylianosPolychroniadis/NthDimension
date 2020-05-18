using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Game.Dungeon.Worldgen
{
    class RoomInstanceNormal : RoomInstance
    {
        Size _size;
        Point _focus;
        bool _overlapped = false;
        Point _p1, _p2, _p3, _p4;

        public RoomInstanceNormal(Size size)
        {
            _size = size;

            // Pick a random focus point within the outer walls of the room
            _focus = new Point(
                Globals.Rand.Next(1, _size.Width - 2),
                Globals.Rand.Next(1, _size.Height - 2));

            ChooseStyle();
        }

        public RoomInstanceNormal(Point p1, Point p2, Point p3, Point p4)
        {
            _overlapped = true;

            Point topLeft = new Point(
                Math.Min(p1.X, p3.X),
                Math.Min(p1.Y, p3.Y));
            Point bottomRight = new Point(
                Math.Max(p2.X, p4.X),
                Math.Max(p2.Y, p4.Y));
            _size = new Size(bottomRight.X - topLeft.X + 3, bottomRight.Y - topLeft.Y + 3);
            _p1 = p1;
            _p2 = p2;
            _p3 = p3;
            _p4 = p4;

            Point offset = new Point(-(topLeft.X - 1), -(topLeft.Y - 1));
            _p1.Offset(offset);
            _p2.Offset(offset);
            _p3.Offset(offset);
            _p4.Offset(offset);

            if (Globals.Rand.Next(2) == 0)
                _focus = new Point(Globals.Rand.Next(_p1.X, _p2.X), Globals.Rand.Next(_p1.Y, _p2.Y));
            else
                _focus = new Point(Globals.Rand.Next(_p3.X, _p4.X), Globals.Rand.Next(_p3.Y, _p4.Y));

            ChooseStyle();
        }

        DecorateStyle _style;
        DecorateFlags _flags;
        void ChooseStyle()
        {
            _style = DecorateStyle.Default;
            _flags = base.StyleFlags;

            if (Globals.OneIn(10))
            {
                int rand = Globals.Rand.Next(3);
                if (rand == 0)
                    _style = DecorateStyle.Wooden;
                else if (rand == 1)
                    _style = DecorateStyle.Dungeon;
                else
                    _style = DecorateStyle.Town;

                //if (Globals.Rand.Next(2) == 0)
                _flags |= DecorateFlags.Spread;
            }

            if (Globals.Rand.Next(5) == 0)
                _flags |= DecorateFlags.AltDoors;
            if (Globals.Rand.Next(15) == 0)
            {
                _flags |= DecorateFlags.Bloody;
                _flags &= (~DecorateFlags.Spread);
            }
        }

        protected override DecorateStyle Style { get { return _style; } }
        protected override DecorateFlags StyleFlags { get { return _flags; } }

        public override Size RoomSize
        {
            get { return _size; }
        }

        protected override Point RoomFocusRelative
        {
            get { return _focus; }
        }

        public override void DrawRoom(
            DungeonLevel level,
            Point roomOrigin)
        {
            if (_overlapped)
                DrawRoomOverlapped(level, roomOrigin);
            else
                DrawRoomRect(level, roomOrigin);
        }
        void DrawRoomRect(DungeonLevel level, Point roomOrigin)
        {
            // Calculate floor space rectangle
            int left = roomOrigin.X + 1;
            int right = roomOrigin.X + _size.Width - 2;
            int top = roomOrigin.Y + 1;
            int bottom = roomOrigin.Y + _size.Height - 2;
            int width = right - left + 1;
            int height = bottom - top + 1;

            // Carve out the room's floor
            level.SetFeature(left, top, width, height,
                CaveFeature.CaveFloor);

            // Draw the outer wall surrounding the room
            GridUtil.DrawRectangle(level.CaveFeatures, left - 1, top - 1, width + 2, height + 2,
                CaveFeature.GraniteWallOuter);

            // Sometimes draw columns
            if ((_size.Width & 1) == 1 && (_size.Height & 1) == 1 && _size.Width * _size.Height > 35 && Globals.OneIn(5))
                DungeonGenerator.MakeColumns(roomOrigin.X, roomOrigin.Y, _size.Width, _size.Height, level);

            // Default to no light
            CaveFlag roomFlag = CaveFlag.IsRoom;

            // Shallow depths have high probability of light
            if (level.Depth <= Globals.Rand.Next(25))
                roomFlag |= CaveFlag.Glowing;

            // Set the "room" flag for the interior of the room
            level.SetCaveFlags(left - 1, top - 1, width + 2, height + 2, roomFlag, true);
        }

        void DrawRoomOverlapped(DungeonLevel level, Point roomOrigin)
        {
            // Default to no light
            CaveFlag roomFlag = CaveFlag.IsRoom;

            // Shallow depths have high probability of light
            if (level.Depth <= Globals.Rand.Next(25))
                roomFlag |= CaveFlag.Glowing;

            // Offset by room origin
            Point p1, p2, p3, p4;
            p1 = _p1;
            p2 = _p2;
            p3 = _p3;
            p4 = _p4;

            bool makeColumns = false;
            if ((p1.X & 1) == 1 && (p1.Y & 1) == 1 && (p2.X & 1) == 1 && (p2.Y & 1) == 1 &&
                (p3.X & 1) == 1 && (p3.Y & 1) == 1 && (p4.X & 1) == 1 && (p4.Y & 1) == 1)
                makeColumns = true;

            p1.Offset(roomOrigin);
            p2.Offset(roomOrigin);
            p3.Offset(roomOrigin);
            p4.Offset(roomOrigin);

            // Draw outer walls
            GridUtil.DrawRectangle(level.CaveFeatures, p1.X - 1, p1.Y - 1, p2.X - p1.X + 3, p2.Y - p1.Y + 3,
                CaveFeature.GraniteWallOuter);
            GridUtil.DrawRectangle(level.CaveFeatures, p3.X - 1, p3.Y - 1, p4.X - p3.X + 3, p4.Y - p3.Y + 3,
                CaveFeature.GraniteWallOuter);

            // Carve out the room's floor
            level.SetFeature(p1.X, p1.Y, p2.X - p1.X + 1, p2.Y - p1.Y + 1, CaveFeature.CaveFloor);
            level.SetFeature(p3.X, p3.Y, p4.X - p3.X + 1, p4.Y - p3.Y + 1, CaveFeature.CaveFloor);

            if (makeColumns && Globals.OneIn(2))
                DungeonGenerator.MakeColumns(roomOrigin.X, roomOrigin.Y, _size.Width, _size.Height, level);

            // Set the "room" flag for the interior of the room
            level.SetCaveFlags(p1.X - 1, p1.Y - 1, p2.X - p1.X + 3, p2.Y - p1.Y + 3, roomFlag, true);
            level.SetCaveFlags(p3.X - 1, p3.Y - 1, p4.X - p3.X + 3, p4.Y - p3.Y + 3, roomFlag, true);
        }
    }
}
