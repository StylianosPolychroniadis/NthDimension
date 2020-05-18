using System;
using System.Drawing;

namespace NthDimension.Procedural.Dungeon
{
    public struct Rect
    {
        public static readonly Rect Empty = default(Rect);

        public readonly int MaxX;

        public readonly int MaxY;

        public readonly int X;

        public readonly int Y;

        public bool IsEmpty
        {
            get
            {
                return this.X == this.MaxX || this.Y == this.MaxY;
            }
        }

     
        public Rect(int x, int y, int maxX, int maxY)
        {
            this.X = x;
            this.Y = y;
            this.MaxX = ((maxX < x) ? x : maxX);
            this.MaxY = ((maxY < y) ? y : maxY);
        }

        public Rect(double x, double y, double maxX, double maxY)
        {
            this = new Rect(Math.Round((double)x), Math.Round((double)x), Math.Round((double)maxX), Math.Round((double)maxY));
        }

        public bool Contains(Point pt)
        {
            return this.Contains(pt.X, pt.Y);
        }

        public bool Contains(double x, double y)
        {
            return x >= (double)this.X && x < (double)this.MaxX && y >= (double)this.Y && y < (double)this.MaxY;
        }

        public bool Contains(int x, int y)
        {
            return x >= this.X && x < this.MaxX && y >= this.Y && y < this.MaxY;
        }

        public Rect Intersection(Rect rect)
        {
            return new Rect(Math.Max(this.X, rect.X), Math.Max(this.Y, rect.Y), Math.Min(this.MaxX, rect.MaxX), Math.Min(this.MaxY, rect.MaxY));
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2}, {3})", new object[]
            {
                this.X,
                this.MaxX,
                this.Y,
                this.MaxY
            });
        }
    }
}
