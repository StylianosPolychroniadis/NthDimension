using System;


namespace NthDimension.Algebra
{
    // Struct influenced by GradientSpace g3
    public struct Index2i : IComparable<Index2i>, IEquatable<Index2i>
    {
        public int a;
        public int b;

        public Index2i(int z) { a = b = z; }
        public Index2i(int ii, int jj) { a = ii; b = jj; }
        public Index2i(int[] i2) { a = i2[0]; b = i2[1]; }
        public Index2i(Index2i copy) { a = copy.a; b = copy.b; }

        static public readonly Index2i Zero = new Index2i(0, 0);
        static public readonly Index2i One = new Index2i(1, 1);
        static public readonly Index2i Max = new Index2i(int.MaxValue, int.MaxValue);
        static public readonly Index2i Min = new Index2i(int.MinValue, int.MinValue);


        public int this[int key]
        {
            get { return (key == 0) ? a : b; }
            set { if (key == 0) a = value; else b = value; }
        }

        public int[] array
        {
            get { return new int[] { a, b }; }
        }


        public int LengthSquared
        {
            get { return a * a + b * b; }
        }
        public int Length
        {
            get { return (int)System.Math.Sqrt(LengthSquared); }
        }


        public void Set(Index2i o)
        {
            a = o[0]; b = o[1];
        }
        public void Set(int ii, int jj)
        {
            a = ii; b = jj;
        }


        public static Index2i operator -(Index2i v)
        {
            return new Index2i(-v.a, -v.b);
        }

        public static Index2i operator *(int f, Index2i v)
        {
            return new Index2i(f * v.a, f * v.b);
        }
        public static Index2i operator *(Index2i v, int f)
        {
            return new Index2i(f * v.a, f * v.b);
        }
        public static Index2i operator /(Index2i v, int f)
        {
            return new Index2i(v.a / f, v.b / f);
        }


        public static Index2i operator *(Index2i a, Index2i b)
        {
            return new Index2i(a.a * b.a, a.b * b.b);
        }
        public static Index2i operator /(Index2i a, Index2i b)
        {
            return new Index2i(a.a / b.a, a.b / b.b);
        }


        public static Index2i operator +(Index2i v0, Index2i v1)
        {
            return new Index2i(v0.a + v1.a, v0.b + v1.b);
        }
        public static Index2i operator +(Index2i v0, int f)
        {
            return new Index2i(v0.a + f, v0.b + f);
        }

        public static Index2i operator -(Index2i v0, Index2i v1)
        {
            return new Index2i(v0.a - v1.a, v0.b - v1.b);
        }
        public static Index2i operator -(Index2i v0, int f)
        {
            return new Index2i(v0.a - f, v0.b - f);
        }


        public static bool operator ==(Index2i a, Index2i b)
        {
            return (a.a == b.a && a.b == b.b);
        }
        public static bool operator !=(Index2i a, Index2i b)
        {
            return (a.a != b.a || a.b != b.b);
        }
        public override bool Equals(object obj)
        {
            return this == (Index2i)obj;
        }
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = (int)2166136261;
                // Suitable nullity checks etc, of course :)
                hash = (hash * 16777619) ^ a.GetHashCode();
                hash = (hash * 16777619) ^ b.GetHashCode();
                return hash;
            }
        }
        public int CompareTo(Index2i other)
        {
            if (a != other.a)
                return a < other.a ? -1 : 1;
            else if (b != other.b)
                return b < other.b ? -1 : 1;
            return 0;
        }
        public bool Equals(Index2i other)
        {
            return (a == other.a && b == other.b);
        }


        public override string ToString()
        {
            return string.Format("[{0},{1}]", a, b);
        }

    }
}
