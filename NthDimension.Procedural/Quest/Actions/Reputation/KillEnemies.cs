using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions.Reputation
{
    public class Reputation_KillEnenmies : StartingActions
    {
        public Reputation_KillEnenmies()
        {
            name = "KillEnenmies";
        }
        public override void GenerateActions()
        {
            NPC enemy = new NPC();
            NPC friend = new NPC();
            actions.Add(new GoTo(enemy));
            actions.Add(new Kill(enemy));
            actions.Add(new GoTo(friend));
            actions.Add(new Report(friend));
        }
    };
}
