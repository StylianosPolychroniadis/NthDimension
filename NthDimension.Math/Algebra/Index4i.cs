using System;


namespace NthDimension.Algebra
{
    // Struct influenced by GradientSpace g3
    public struct Index4i
    {
        public int a;
        public int b;
        public int c;
        public int d;

        public Index4i(int z) { a = b = c = d = z; }
        public Index4i(int aa, int bb, int cc, int dd) { a = aa; b = bb; c = cc; d = dd; }
        public Index4i(int[] i2) { a = i2[0]; b = i2[1]; c = i2[2]; d = i2[3]; }
        public Index4i(Index4i copy) { a = copy.a; b = copy.b; c = copy.b; d = copy.d; }

        static public readonly Index4i Zero = new Index4i(0, 0, 0, 0);
        static public readonly Index4i One = new Index4i(1, 1, 1, 1);
        static public readonly Index4i Max = new Index4i(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue);


        public int this[int key]
        {
            get { return (key == 0) ? a : (key == 1) ? b : (key == 2) ? c : d; }
            set { if (key == 0) a = value; else if (key == 1) b = value; else if (key == 2) c = value; else d = value; }
        }

        public int[] array
        {
            get { return new int[4] { a, b, c, d }; }
        }


        public int LengthSquared
        {
            get { return a * a + b * b + c * c + d * d; }
        }
        public int Length
        {
            get { return (int)System.Math.Sqrt(LengthSquared); }
        }


        public void Set(Index4i o)
        {
            a = o[0]; b = o[1]; c = o[2]; d = o[3];
        }
        public void Set(int aa, int bb, int cc, int dd)
        {
            a = aa; b = bb; c = cc; d = dd;
        }


        public bool Contains(int val)
        {
            return a == val || b == val || c == val || d == val;
        }

        public void Sort()
        {
            int tmp;   // if we use 2 temp ints, we can swap in a different order where some test pairs
                       // could be done simultaneously, but no idea if compiler would optimize that anyway...
            if (d < c) { tmp = d; d = c; c = tmp; }
            if (c < b) { tmp = c; c = b; b = tmp; }
            if (b < a) { tmp = b; b = a; a = tmp; }   // now a is smallest value
            if (b > c) { tmp = c; c = b; b = tmp; }
            if (c > d) { tmp = d; d = c; c = tmp; }   // now d is largest value
            if (b > c) { tmp = c; c = b; b = tmp; }   // bow b,c are sorted
        }


        public static Index4i operator -(Index4i v)
        {
            return new Index4i(-v.a, -v.b, -v.c, -v.d);
        }

        public static Index4i operator *(int f, Index4i v)
        {
            return new Index4i(f * v.a, f * v.b, f * v.c, f * v.d);
        }
        public static Index4i operator *(Index4i v, int f)
        {
            return new Index4i(f * v.a, f * v.b, f * v.c, f * v.d);
        }
        public static Index4i operator /(Index4i v, int f)
        {
            return new Index4i(v.a / f, v.b / f, v.c / f, v.d / f);
        }


        public static Index4i operator *(Index4i a, Index4i b)
        {
            return new Index4i(a.a * b.a, a.b * b.b, a.c * b.c, a.d * b.d);
        }
        public static Index4i operator /(Index4i a, Index4i b)
        {
            return new Index4i(a.a / b.a, a.b / b.b, a.c / b.c, a.d / b.d);
        }


        public static Index4i operator +(Index4i v0, Index4i v1)
        {
            return new Index4i(v0.a + v1.a, v0.b + v1.b, v0.c + v1.c, v0.d + v1.d);
        }
        public static Index4i operator +(Index4i v0, int f)
        {
            return new Index4i(v0.a + f, v0.b + f, v0.c + f, v0.d + f);
        }

        public static Index4i operator -(Index4i v0, Index4i v1)
        {
            return new Index4i(v0.a - v1.a, v0.b - v1.b, v0.c - v1.c, v0.d - v1.d);
        }
        public static Index4i operator -(Index4i v0, int f)
        {
            return new Index4i(v0.a - f, v0.b - f, v0.c - f, v0.d - f);
        }


        public static bool operator ==(Index4i a, Index4i b)
        {
            return (a.a == b.a && a.b == b.b && a.c == b.c && a.d == b.d);
        }
        public static bool operator !=(Index4i a, Index4i b)
        {
            return (a.a != b.a || a.b != b.b || a.c != b.c || a.d != b.d);
        }
        public override bool Equals(object obj)
        {
            return this == (Index4i)obj;
        }
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = (int)2166136261;
                // Suitable nullity checks etc, of course :)
                hash = (hash * 16777619) ^ a.GetHashCode();
                hash = (hash * 16777619) ^ b.GetHashCode();
                hash = (hash * 16777619) ^ c.GetHashCode();
                hash = (hash * 16777619) ^ d.GetHashCode();
                return hash;
            }
        }
        public int CompareTo(Index4i other)
        {
            if (a != other.a)
                return a < other.a ? -1 : 1;
            else if (b != other.b)
                return b < other.b ? -1 : 1;
            else if (c != other.c)
                return c < other.c ? -1 : 1;
            else if (d != other.d)
                return d < other.d ? -1 : 1;
            return 0;
        }
        public bool Equals(Index4i other)
        {
            return (a == other.a && b == other.b && c == other.c && d == other.d);
        }



        public override string ToString()
        {
            return string.Format("[{0},{1},{2},{3}]", a, b, c, d);
        }

    }
}
