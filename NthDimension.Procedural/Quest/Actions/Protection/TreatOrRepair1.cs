using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions.Protection
{
    public class Protection_TreatOrRepair1 : StartingActions
    {
        public Protection_TreatOrRepair1()
        {
            name = "TreatOrRepair1";
        }

        public override void GenerateActions()
        {
            NPC npc1 = new NPC();
            Item i = new Item();
            actions.Add(new Get(i));
            actions.Add(new GoTo(npc1));
            actions.Add(new Use(i));
        }
    }
}
