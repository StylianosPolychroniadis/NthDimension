using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions.Serenity
{
    public class Serenity_CheckOnNPC2 : StartingActions
    {
        public Serenity_CheckOnNPC2()
        {
            name = "CheckOnNPC2";
        }

        public override void GenerateActions()
        {
            NPC npc1 = new NPC();
            NPC npc2 = new NPC();
            Item i = new Item();
            actions.Add(new GoTo(npc1));
            actions.Add(new Take(i));
            actions.Add(new GoTo(npc2));
            actions.Add(new Give(i));
        }
    }
}
