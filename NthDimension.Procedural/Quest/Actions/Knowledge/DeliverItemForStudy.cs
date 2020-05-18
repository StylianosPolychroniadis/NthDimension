using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions.Knowledge
{
    public class Knowledge_DeliverItemForStudy : StartingActions
    {
        public Knowledge_DeliverItemForStudy()
        {
            name = "DeliverItemForStudy";
        }

        public override void GenerateActions()
        {
            Item i = new Item();
            NPC npc = new NPC();
            actions.Add(new Get(i));
            actions.Add(new GoTo(npc));
            actions.Add(new Give(i));
        }
    }
}
