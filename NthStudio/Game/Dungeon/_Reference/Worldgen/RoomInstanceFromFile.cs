using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Game.Dungeon.Worldgen
{
    class RoomInstanceFromFile : RoomInstance
    {
        int _xmx, _xmy, _ymx, _ymy;
        Size _transformedSize;
        Data.RoomDef _def;
        Point _focus;

        void Transform(ref int x, ref int y)
        {
            int nx = x * _xmx + y * _xmy;
            int ny = x * _ymx + y * _ymy;
            if (_xmx < 0 || _xmy < 0)
                nx += _transformedSize.Width - 1;
            if (_ymx < 0 || _ymy < 0)
                ny += _transformedSize.Height - 1;
            x = nx;
            y = ny;
        }

        public RoomInstanceFromFile(Data.RoomDef def, Size transformedSize,
            int xmx, int xmy, int ymx, int ymy)
        {
            _def = def;
            _transformedSize = transformedSize;
            _xmx = xmx;
            _xmy = xmy;
            _ymx = ymx;
            _ymy = ymy;

            // Choose a random focus point
            Size size = _def.Size;
            MT19937 rand = Globals.Rand;
            while (true)
            {
                int x = rand.Next(size.Width);
                int y = rand.Next(size.Height);
                if (def.Tiles[y * size.Width + x] != CaveFeature.CaveFloor)
                    continue;
                Transform(ref x, ref y);
                _focus = new Point(x, y);
                break;
            }
        }

        protected override Point RoomFocusRelative
        {
            get { return _focus; }
        }

        public override Size RoomSize
        {
            get { return _transformedSize; }
        }

        public override IEnumerable<Point> GoodySpots
        {
            get
            {
                if (_def.GoodySpots != null)
                {
                    List<Point> spots = new List<Point>();
                    foreach (Point p in _def.GoodySpots)
                    {
                        int x = p.X;
                        int y = p.Y;
                        Transform(ref x, ref y);
                        spots.Add(new Point(x, y));
                    }
                    return spots;
                }
                else
                    return null;
            }
        }

        public override void DrawRoom(DungeonLevel level, Point roomOrigin)
        {
            // Default to no light
            CaveFlag roomFlag = CaveFlag.IsRoom;

            // Shallow depths have high probability of light
            if (level.Depth <= Globals.Rand.Next(25))
                roomFlag |= CaveFlag.Glowing;

            Size defsize = _def.Size;
            for (int dx = 0; dx < defsize.Width; dx++)
                for (int dy = 0; dy < defsize.Height; dy++)
                {
                    CaveFeature feature = _def.Tiles[dy * defsize.Width + dx];
                    if (feature == CaveFeature.None)
                        continue;

                    int x = dx;
                    int y = dy;
                    Transform(ref x, ref y);
                    level.SetFeature(x + roomOrigin.X, y + roomOrigin.Y, feature);
                    level.SetCaveFlags(x + roomOrigin.X, y + roomOrigin.Y, roomFlag, true);
                }
        }
    }
}
