using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions.Serenity
{
    public class Serenity_Revenge : StartingActions
    {
        public Serenity_Revenge()
        {
            name = "Revenge";
        }

        public override void GenerateActions()
        {
            NPC npc = new NPC();
            actions.Add(new GoTo(npc));
            actions.Add(new Damage(npc));
        }
    }
}
