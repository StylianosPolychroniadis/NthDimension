using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions.Conquest
{
    public class Conquest_Steal : StartingActions
    {
        public Conquest_Steal()
        {
            name = "Steal";
        }

        public override void GenerateActions()
        {
            NPC enemy = new NPC();
            Item i = new Item();
            i.setCoords(enemy);
            NPC npc = new NPC();
            actions.Add(new GoTo(enemy));
            actions.Add(new Steal(i));
            actions.Add(new GoTo(npc));
            actions.Add(new Give(i));
        }
    }
}
