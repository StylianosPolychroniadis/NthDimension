using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions.Equipment
{
    public class Equipment_Deliver : StartingActions
    {
        public Equipment_Deliver()
        {
            name = "Deliver";
        }

        public override void GenerateActions()
        {
            NPC npc = new NPC();
            Item i = new Item();
            actions.Add(new Get(i));
            actions.Add(new GoTo(npc));
            actions.Add(new Give(i));
        }
    }
}
