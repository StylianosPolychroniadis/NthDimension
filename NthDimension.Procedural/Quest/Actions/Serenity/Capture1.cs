using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions.Serenity
{
    public class Serenity_Capture1 : StartingActions
    {
        public Serenity_Capture1()
        {
            name = "Capture1";
        }

        public override void GenerateActions()
        {
            Item i = new Item();
            NPC enemy = new NPC();
            NPC friendly = new NPC();
            actions.Add(new Get(i));
            actions.Add(new GoTo(enemy));
            actions.Add(new Use(i));
            actions.Add(new GoTo(friendly));
            actions.Add(new Give(enemy));
        }
    }
}
