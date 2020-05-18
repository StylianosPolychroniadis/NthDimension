using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions.Protection
{
    public class Protection_Guard : StartingActions
    {
        public Protection_Guard()
        {
            name = "Guard";
        }

        public override void GenerateActions()
        {
            NPC npc = new NPC();
            actions.Add(new GoTo(npc));
            actions.Add(new Defend(npc));
        }
    }
}
