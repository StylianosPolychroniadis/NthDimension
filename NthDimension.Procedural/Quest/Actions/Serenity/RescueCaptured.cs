using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions.Serenity
{
    public class Serenity_RescueCaptured : StartingActions
    {
        public Serenity_RescueCaptured()
        {
            name = "RescueCaptured";
        }

        public override void GenerateActions()
        {
            NPC friendly = new NPC();
            NPC friendly2 = new NPC();
            NPC enemy = new NPC();
            actions.Add(new GoTo(friendly));
            actions.Add(new Damage(enemy));
            actions.Add(new Escort(friendly));
            actions.Add(new GoTo(friendly2));
            actions.Add(new Report(friendly2));
        }
    }
}
