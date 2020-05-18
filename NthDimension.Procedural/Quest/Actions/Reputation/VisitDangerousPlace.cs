using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions.Reputation
{
    public class Reputation_VisitDangerousPlace : StartingActions
    {
        public Reputation_VisitDangerousPlace()
        {
            name = "VisitDangerousPlace";
        }

        public override void GenerateActions()
        {
            Item i1 = new Item();
            Item i2 = new Item();
            i1.setName("Place 1");
            i2.setName("Place 2");
            NPC npc = new NPC();
            actions.Add(new GoTo(i1));
            actions.Add(new GoTo(i2));
            actions.Add(new Report(npc));
        }
    };
}
