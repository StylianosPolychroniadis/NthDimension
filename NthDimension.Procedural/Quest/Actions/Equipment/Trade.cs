using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions.Equipment
{
    public class Equipment_Trade : StartingActions
    {
        public Equipment_Trade()
        {
            name = "Trade";
        }

        public override void GenerateActions()
        {
            Item i = new Item();
            NPC npc = new NPC();
            actions.Add(new GoTo(npc));
            actions.Add(new Exchange(i, npc));
        }
    }
}
