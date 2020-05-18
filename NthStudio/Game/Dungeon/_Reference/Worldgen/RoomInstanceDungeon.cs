using System.Drawing;

namespace NthStudio.Game.Dungeon.Worldgen
{
    class RoomInstanceDungeon : RoomInstance
    {
        Size _size;
        Point _focus;

        public RoomInstanceDungeon()
        {
            _size = new Size(17, 17);
            _focus = new Point(9, 9);
        }

        public override Size RoomSize
        {
            get { return _size; }
        }

        protected override Point RoomFocusRelative
        {
            get { return _focus; }
        }

        public override void DrawRoom(DungeonLevel level, Point roomOrigin)
        {
            level.SetFeature(roomOrigin.X + 7, roomOrigin.Y + 3, 5, 13, CaveFeature.Wall3);
            level.SetFeature(roomOrigin.X + 3, roomOrigin.Y + 7, 13, 5, CaveFeature.Wall3);

            level.SetFeature(roomOrigin.X + 7, roomOrigin.Y + 3, 5, 13, CaveFeature.Wall3);
            level.SetFeature(roomOrigin.X + 3, roomOrigin.Y + 7, 13, 5, CaveFeature.Wall3);

            level.SetFeature(roomOrigin.X + 8, roomOrigin.Y + 4, 3, 11, CaveFeature.CaveFloor);
            level.SetFeature(roomOrigin.X + 4, roomOrigin.Y + 8, 11, 3, CaveFeature.CaveFloor);
            level.SetFeature(roomOrigin.X + 8, roomOrigin.Y + 8, 3, 3, CaveFeature.Wall3);

            level.SetFeature(roomOrigin.X + 9, roomOrigin.Y + 3, CaveFeature.DoorGateClosed3);
            level.SetFeature(roomOrigin.X + 3, roomOrigin.Y + 9, CaveFeature.DoorGateClosed3);
            level.SetFeature(roomOrigin.X + 15, roomOrigin.Y + 9, CaveFeature.DoorGateClosed3);
            level.SetFeature(roomOrigin.X + 9, roomOrigin.Y + 15, CaveFeature.DoorGateClosed3);

            level.SetFeature(roomOrigin.X + 7, roomOrigin.Y + 5, CaveFeature.WindowBars3);
            level.SetFeature(roomOrigin.X + 11, roomOrigin.Y + 5, CaveFeature.WindowBars3);
            level.SetFeature(roomOrigin.X + 5, roomOrigin.Y + 7, CaveFeature.WindowBars3);
            level.SetFeature(roomOrigin.X + 5, roomOrigin.Y + 11, CaveFeature.WindowBars3);
            level.SetFeature(roomOrigin.X + 13, roomOrigin.Y + 7, CaveFeature.WindowBars3);
            level.SetFeature(roomOrigin.X + 13, roomOrigin.Y + 11, CaveFeature.WindowBars3);
            level.SetFeature(roomOrigin.X + 7, roomOrigin.Y + 13, CaveFeature.WindowBars3);
            level.SetFeature(roomOrigin.X + 11, roomOrigin.Y + 13, CaveFeature.WindowBars3);

            level.SetFeature(roomOrigin.X + 9, roomOrigin.Y + 8, CaveFeature.DoorGateClosed3);
            level.SetFeature(roomOrigin.X + 9, roomOrigin.Y + 10, CaveFeature.DoorGateClosed3);
            level.SetFeature(roomOrigin.X + 8, roomOrigin.Y + 9, CaveFeature.DoorGateClosed3);
            level.SetFeature(roomOrigin.X + 10, roomOrigin.Y + 9, CaveFeature.DoorGateClosed3);
            level.SetFeature(roomOrigin.X + 9, roomOrigin.Y + 9, CaveFeature.StairsDown3);

        }
    }
}
