using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions.Comfort
{
    public class Comfort_KillPests : StartingActions
    {
        public Comfort_KillPests()
        {
            name = "KillPests";
        }

        public override void GenerateActions()
        {
            NPC npc1 = new NPC();
            NPC npc2 = new NPC();
            actions.Add(new GoTo(npc1));
            actions.Add(new Damage(npc1));
            actions.Add(new GoTo(npc2));
            actions.Add(new Report(npc2));
        }
    };
}
