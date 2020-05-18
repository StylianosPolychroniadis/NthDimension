using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions
{
    public class Read : QuestAction
    {

        public Read()
        {
            this.name = "Read";
            this.obj = new Item();
        }

        public Read(QuestObject o)
        {
            this.name = "Read";
            this.obj = o;
        }
    }
}
