using NthStudio.Game.Dungeon;
using NthStudio.Game.Dungeon.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Game.Dungeon
{
    internal static class Globals
    {
        static Globals()
        {
            Rand = new MT19937();
            Rand.init_by_array(new ulong[] { (ulong)Environment.TickCount });
        }

        #region Random
        public static MT19937 Rand = null;

        public static bool OneIn(int number)
        {
            return Rand.Next(number) == 0;
        }

        public static int RandSpread(int x, int dist)
        {
            if (dist == 0)
                return x;
            else
                return Globals.Rand.Next(dist * 2 + 1) + x - dist;
        }

        public static int RandRange(int low, int high)
        {
            int delta = high - low;
            if (delta == 0)
                return low;
            return Rand.Next(delta + 1) + low;
        }

        public static int Rand1(int max)
        {
            if (max <= 1)
                return 1;
            else
                return Rand.Next(max) + 1;
        }

        public static int RandNormal(int mean, int stand)
        {
            // Paranoia
            if (stand < 1) return mean;

            // Get normal distribution table
            ushort[] randTable = RandomNormal.RandNormalTable;

            // Roll for probability
            ushort temp = (ushort)Globals.Rand.Next(32768);

            // Binary search
            int low = 0;
            int high = 256;
            while (low < high)
            {
                int mid = (low + high) >> 1;
                if (randTable[mid] < temp)
                    low = mid + 1;
                else
                    high = mid;
            }

            // Convert index into an offset
            int offset = stand * (int)low / 64;

            if (Globals.Rand.Next(100) < 50)
                return mean - offset;
            else
                return mean + offset;
        }

        

        #endregion

        /// <summary>
        /// Distance approximation.  Close to the real distance when the distance
        /// in X or Y dwarfs the distance in the other direction.
        /// </summary>
        public static int Distance(System.Drawing.Point p1, System.Drawing.Point p2)
        {
            int dx = p1.X > p2.X ? p1.X - p2.X : p2.X - p1.X;
            int dy = p1.Y > p2.Y ? p1.Y - p2.Y : p2.Y - p1.Y;
            return dy > dx ? (dy + (dx >> 1)) : (dx + (dy >> 1));
        }

        #region Color Definitions
        static ColorDefs _colorDefinitions = null;
        public static ColorDefs ColorDefinitions
        {
            get
            {
                if (_colorDefinitions == null)
                {
                    _colorDefinitions = ColorDefs.LoadFromFile(
                        System.IO.Path.Combine(NthDimension.Utilities.DirectoryUtil.Documents, "color.txt"));
                }
                return _colorDefinitions;
            }
        }
        #endregion Color Definitions

        #region Feature Definitions
        static FeatureTiles _featureTiles = null;
        public static FeatureTiles FeatureTiles
        {
            get
            {
                if (_featureTiles == null)
                {
                    _featureTiles = FeatureTiles.LoadFromFile(
                        System.IO.Path.Combine(NthDimension.Utilities.DirectoryUtil.Documents, "feature.txt"));
                }
                return _featureTiles;
            }
        }
        #endregion Feature Definitions

        #region Tile Definitions
        static TileDefs _tileDefs = null;
        public static TileDefs TileDefinitions
        {
            get
            {
                if (_tileDefs == null)
                    _tileDefs = TileDefs.LoadFromFile(System.IO.Path.Combine(NthDimension.Utilities.DirectoryUtil.Documents, "tiles.txt"));
                return _tileDefs;
            }
        }
        #endregion

        #region Room Definitions
        static List<RoomDef> _rooms = null;
        public static List<RoomDef> RoomDefinitions
        {
            get
            {
                if (_rooms == null)
                    _rooms = RoomDef.LoadFromFile(System.IO.Path.Combine(NthDimension.Utilities.DirectoryUtil.Documents, "room.txt"));
                return _rooms;
            }
        }
        #endregion Room Definitions


    }
}
