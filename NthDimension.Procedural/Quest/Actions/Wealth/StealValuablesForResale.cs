using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions.Wealth
{
    public class Wealth_StealValuablesForResale : StartingActions
    {
        public Wealth_StealValuablesForResale()
        {
            this.name = "StealValuablesForResale";
        }
        override public void GenerateActions()
        {
            Item i = new Item();
            NPC npc = new NPC();
            actions.Add(new GoTo(i));
            actions.Add(new Steal(i));
            actions.Add(new GoTo(npc));
            actions.Add(new Exchange(i, npc));
        }
    }
}
