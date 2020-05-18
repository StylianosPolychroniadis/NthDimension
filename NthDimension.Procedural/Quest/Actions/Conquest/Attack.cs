using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions.Conquest
{
    public class Conquest_Attack : StartingActions
    {
        public Conquest_Attack()
        {
            name = "Attack";
        }

        public override void GenerateActions()
        {
            NPC npc = new NPC();
            actions.Add(new GoTo(npc));
            actions.Add(new Damage(npc));
        }
    }
}
