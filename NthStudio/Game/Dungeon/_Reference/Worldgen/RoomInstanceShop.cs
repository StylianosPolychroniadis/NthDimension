using System.Drawing;

namespace NthStudio.Game.Dungeon.Worldgen
{
    class RoomInstanceShop : RoomInstance
    {
        const int Margin = 2;
        Size _size;
        Point _focus;
        CaveFeature _shop;

        public RoomInstanceShop(CaveFeature shop)
        {
            _shop = shop;
            _size = new Size(3 + Globals.Rand.Next(6), 3 + Globals.Rand.Next(3));
            if (_size.Width == 3)
                _focus = new Point(1, _size.Height - 1);
            else
                _focus = new Point(1 + Globals.Rand.Next(_size.Width - 2), _size.Height - 1);

            // Need grassy margins
            _size.Width += Margin * 2;
            _size.Height += Margin * 2;

            _focus.X += Margin;
            _focus.Y += Margin;
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
            level.SetFeature(roomOrigin.X + Margin, roomOrigin.Y + Margin,
                _size.Width - Margin * 2, _size.Height - Margin * 2,
                CaveFeature.Wall4);
            level.SetFeature(roomOrigin.X + _focus.X, roomOrigin.Y + _focus.Y, _shop);
        }
    }
}
