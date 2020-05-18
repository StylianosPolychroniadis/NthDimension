using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions.Protection
{
    public class Protection_Attack : StartingActions
    {
        public Protection_Attack()
        {
            name = "Attack";
        }

        public override void GenerateActions()
        {
            NPC friendly = new NPC();
            NPC enemy = new NPC();
            actions.Add(new GoTo(enemy));
            actions.Add(new Damage(enemy));
            actions.Add(new GoTo(friendly));
            actions.Add(new Report(friendly));
        }
    }
}
