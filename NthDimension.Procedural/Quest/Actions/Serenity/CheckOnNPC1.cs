using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions.Serenity
{
    public class Serenity_CheckOnNPC1 : StartingActions
    {
        public Serenity_CheckOnNPC1()
        {
            name = "CheckOnNPC1";
        }

        public override void GenerateActions()
        {
            NPC npc1 = new NPC();
            NPC npc2 = new NPC();
            actions.Add(new GoTo(npc1));
            actions.Add(new Listen(npc1));
            actions.Add(new GoTo(npc2));
            actions.Add(new Report(npc2));
        }
    }
}
