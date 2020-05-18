using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions
{
    public class Repair : QuestAction
    {
        public Repair()
        {
            this.name = "Repair or create";
            this.obj = new WallsOrGates();
        }
        public Repair(QuestObject o)
        {
            this.name = "Repair or create";
            this.obj = o;
        }
    };
}
