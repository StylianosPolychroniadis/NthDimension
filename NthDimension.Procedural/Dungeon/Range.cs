using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Dungeon
{
    public struct Range
    {
        public static readonly Range Zero = new Range(0, 0);

        public readonly int Begin;
        public readonly int End;

        public Range(int begin, int end)
        {
            if (end < begin)
                end = begin;
            Begin = begin;
            End = end;
        }

        public int Random(Random rand)
        {
            return rand.Next(Begin, End + 1);
        }

        public bool IsEmpty { get { return Begin == End; } }

        public Range Intersection(Range range)
        {
            return new Range(Math.Max(Begin, range.Begin), Math.Min(End, range.End));
        }

        public override string ToString()
        {
            return string.Format("[{0}, {1}]", Begin, End);
        }
    }
}
