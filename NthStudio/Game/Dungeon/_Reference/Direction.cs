using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Game.Dungeon
{
    internal static class Direction
    {
        public const int DirCount = 8;
        public static int[] DirX = new int[] { 1, 1, 0, -1, -1, -1, 0, 1 };
        public static int[] DirY = new int[] { 0, -1, -1, -1, 0, 1, 1, 1 };

        public const int DirRight = 0;
        public const int DirUpRight = 1;
        public const int DirUp = 2;
        public const int DirUpLeft = 3;
        public const int DirLeft = 4;
        public const int DirDownLeft = 5;
        public const int DirDown = 6;
        public const int DirDownRight = 7;

        public static System.Drawing.Point GetNextCirclePoint(System.Drawing.Point p, System.Drawing.Point center)
        {
            int radius = Globals.Distance(p, center);
            int dir;
            if (p.Y >= center.Y)
            {
                if (p.X >= center.X)
                    dir = DirUp;
                else
                    dir = DirRight;
            }
            else
            {
                if (p.X >= center.X)
                    dir = DirLeft;
                else
                    dir = DirDown;
            }

            System.Drawing.Point target = new System.Drawing.Point(p.X + DirX[dir], p.Y + DirY[dir]);
            if (Globals.Distance(target, center) == radius)
                return target;

            dir = (dir + 6) & 7;
            target = new System.Drawing.Point(p.X + DirX[dir], p.Y + DirY[dir]);
            if (Globals.Distance(target, center) == radius)
                return target;

            dir = (dir + 1) & 7;
            target = new System.Drawing.Point(p.X + DirX[dir], p.Y + DirY[dir]);
            return target;
        }

        public static System.Drawing.Point GetPrevCirclePoint(System.Drawing.Point p, System.Drawing.Point center)
        {
            int radius = Globals.Distance(p, center);
            int dir;
            if (p.Y >= center.Y)
            {
                if (p.X >= center.X)
                    dir = DirDown;
                else
                    dir = DirLeft;
            }
            else
            {
                if (p.X >= center.X)
                    dir = DirRight;
                else
                    dir = DirUp;
            }

            System.Drawing.Point target = new System.Drawing.Point(p.X + DirX[dir], p.Y + DirY[dir]);
            if (Globals.Distance(target, center) == radius)
                return target;

            dir = (dir + 6) & 7;
            target = new System.Drawing.Point(p.X + DirX[dir], p.Y + DirY[dir]);
            if (Globals.Distance(target, center) == radius)
                return target;

            dir = (dir + 1) & 7;
            target = new System.Drawing.Point(p.X + DirX[dir], p.Y + DirY[dir]);
            return target;
        }

        /// <summary>
        /// Returns the difference between two directions, in the range of -4 to +4
        /// </summary>
        /// <param name="d1">0 to 7</param>
        /// <param name="d2">0 to 7</param>
        /// <returns></returns>
        public static int DirSubtract(int d1, int d2)
        {
            int diff = (d2 - d1) & 7;
            int n = diff / 8;
            diff -= (n * 8);
            if (diff < -4)
                diff += 8;
            else if (diff > 4)
                diff -= 8;
            return diff;
        }
    }
}
