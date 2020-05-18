using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions
{
    public class Use : QuestAction
    {
        public Use()
        {
            this.name = "Use";
            this.obj = new Item();
        }
        public Use(QuestObject o)
        {
            this.name = "Use";
            this.obj = o;
        }
    };
}
