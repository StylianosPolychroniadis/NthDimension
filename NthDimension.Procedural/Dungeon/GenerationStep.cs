using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Dungeon
{
    public enum GenerationStep
    {
        Initialize = 0,

        TargetGeneration = 1,
        SpecialGeneration = 2,
        BranchGeneration = 3,

        Finish = 4
    }
}
