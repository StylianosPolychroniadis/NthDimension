using System.Drawing;


namespace NthStudio.Game.Dungeon.Worldgen
{
    class RoomInstanceFarm : RoomInstance
    {
        const int Margin = 2;
        Size _size;

        public RoomInstanceFarm()
        {
            _size = new Size(3 + Globals.Rand.Next(4), Globals.Rand.Next(4) + 3);
            _size.Width += Margin * 2;
            _size.Height += Margin * 2;
        }

        public override void DrawRoom(DungeonLevel level, Point roomOrigin)
        {
            level.SetFeature(roomOrigin.X + Margin, roomOrigin.Y + Margin,
                _size.Width - Margin * 2, _size.Height - Margin * 2, CaveFeature.FarmPlot);
        }

        public override Size RoomSize
        {
            get { return _size; }
        }

        public override bool ForceConnect { get { return false; } }

        protected override Point RoomFocusRelative
        {
            get { return new Point(0, 0); }
        }
    }
}
