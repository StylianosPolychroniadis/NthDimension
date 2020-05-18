using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions.Knowledge
{
    public class Knowledge_UseItemInTheField : StartingActions
    {
        public Knowledge_UseItemInTheField()
        {
            name = "UseItemInTheField";
        }

        public override void GenerateActions()
        {
            Item i = new Item();
            NPC npc = new NPC();
            NPC npc2 = new NPC();
            actions.Add(new Get(i));
            actions.Add(new GoTo(npc));
            actions.Add(new Use(i));
            actions.Add(new GoTo(npc2));
            actions.Add(new Give(i));
        }
    };
}
