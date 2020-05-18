using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions.Comfort
{
    public class Comfort_ObtainLuxuries : StartingActions
    {
        public Comfort_ObtainLuxuries()
        {
            name = "ObtainLuxuries";
        }

        public override void GenerateActions()
        {
            Item i = new Item();
            NPC npc = new NPC();
            actions.Add(new Get(i));
            actions.Add(new GoTo(npc));
            actions.Add(new Give(i));
        }
    };
}
