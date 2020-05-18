using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Game.Dungeon.Worldgen
{
    class RoomInstanceCavern : RoomInstance
    {
        Size _size;
        int _minx, _miny;
        ExpandableGrid<byte> _mask;
        public RoomInstanceCavern(ExpandableGrid<byte> mask, int minx, int miny, Size size)
        {
            _mask = mask;
            _minx = minx;
            _miny = miny;
            _size = size;
        }

        protected override Point RoomFocusRelative
        {
            get { return new Point(1 - _minx, 1 - _miny); }
        }

        public override Size RoomSize
        {
            get { return _size; }
        }

        public override void DrawRoom(DungeonLevel level, Point roomOrigin)
        {
            // Default to no light
            CaveFlag roomFlag = CaveFlag.IsRoom;

            // Shallow depths have high probability of light
            if (level.Depth <= Globals.Rand.Next(10))
                roomFlag |= CaveFlag.Glowing;

            DoesLocationMatchDelegate doesMatchFloor = delegate (int x, int y)
            {
                return _mask.GetValue(x, y) == 3;
            };
            DoesLocationMatchDelegate doesMatchRoom = delegate (int x, int y)
            {
                int maskValue = _mask.GetValue(x, y);
                return maskValue > 0;
            };
            Point paintOffset = new Point(roomOrigin.X + 1 - _minx, roomOrigin.Y + 1 - _miny);

            PaintLocationDelegate paintFeature = delegate (int x, int y)
            {
                level.SetFeature(x + paintOffset.X, y + paintOffset.Y, CaveFeature.CaveFloor);
                _mask.SetValue(x, y, 2);
            };
            PaintLocationDelegate paintFlag = delegate (int x, int y)
            {
                int px = x + paintOffset.X;
                int py = y + paintOffset.Y;
                level.SetCaveFlags(px, py, roomFlag, true);
                if (level.CaveFeatures.GetValue(px, py) == CaveFeature.GraniteWallExtra &&
                    _mask.GetValue(x, y) == 2)
                {
                    if (DungeonGenerator.IsBetweenTwoWalls(level.CaveFeatures, px, py))
                        level.SetFeature(x + paintOffset.X, y + paintOffset.Y, CaveFeature.GraniteWallOuter);
                    else
                        level.SetFeature(x + paintOffset.X, y + paintOffset.Y, CaveFeature.GraniteWallSolid);
                }
                else if (_mask.GetValue(x, y) == 1)
                    level.SetFeature(x + paintOffset.X, y + paintOffset.Y, CaveFeature.GraniteWallOuter);
                _mask.SetValue(x, y, 0);
            };

            FloodFill.Fill(0, 0, doesMatchFloor, paintFeature);
            FloodFill.Fill(0, 0, doesMatchRoom, paintFlag);
        }

        protected override DecorateStyle Style
        {
            get { return DecorateStyle.Cavern; }
        }
    }
}
