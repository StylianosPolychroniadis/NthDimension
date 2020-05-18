using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions
{
    public class Damage : QuestAction
    {
        public Damage()
        {
            this.name = "Damage";
        }

        public Damage(QuestObject o)
        {
            this.name = "Damage";
            this.obj = o;
        }
    }
}
