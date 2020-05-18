using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Dungeon
{
    public abstract class LevelRootRoom : LevelRoom
    {
        public abstract Tuple<Direction, int>[] ConnectionPoints { get; }

        public override Range NumBranches { get { return new Range(1, ConnectionPoints.Length); } }
    }
}
