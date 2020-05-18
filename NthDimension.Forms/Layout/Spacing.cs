using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms.Layout
{
    /// <summary>
    /// Represents a spacing.
    /// </summary>
    public struct Spacing : IEquatable<Spacing>
    {
        public int Top;
        public int Bottom;
        public int Left;
        public int Right;

        public int Horizontal
        {
            get { return Left + Right; }
        }
        public int Vertical
        {
            get { return Top + Bottom; }
        }

        #region Help, GetHashCode for mutable fields

        private static int counter;
        private readonly int hashCode;

        #endregion Help, GetHashCode for mutable fields

        // common values
        public static Spacing Zero = new Spacing(0, 0, 0, 0);
        public static Spacing One = new Spacing(1, 1, 1, 1);
        public static Spacing Two = new Spacing(2, 2, 2, 2);
        public static Spacing Three = new Spacing(3, 3, 3, 3);
        public static Spacing Four = new Spacing(4, 4, 4, 4);
        public static Spacing Five = new Spacing(5, 5, 5, 5);
        public static Spacing Six = new Spacing(6, 6, 6, 6);
        public static Spacing Seven = new Spacing(7, 7, 7, 7);
        public static Spacing Eight = new Spacing(8, 8, 8, 8);
        public static Spacing Nine = new Spacing(9, 9, 9, 9);
        public static Spacing Ten = new Spacing(10, 10, 10, 10);

        public Spacing(int left, int top, int right, int bottom)
        {
            this.hashCode = counter++;
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
        }

        public bool Equals(Spacing other)
        {
            return other.Top == Top && other.Bottom == Bottom && other.Left == Left && other.Right == Right;
        }

        public static bool operator ==(Spacing lhs, Spacing rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Spacing lhs, Spacing rhs)
        {
            return !lhs.Equals(rhs);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (obj.GetType() != typeof(Spacing))
                return false;

            return Equals((Spacing)obj);
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }
    }
}
