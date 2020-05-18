using NthDimension.Procedural.Quest.Actions.Reputation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Quests
{
    class ReputationQuest : QuestGenerator
    {
        public ReputationQuest()
        {
            this.type = questType.Reputation;
        }

        override public void generateStrategy()
        {
            int current_strategy = rnd1.Next(0, 100);
            if (current_strategy < 40)
                startingActions.Add(new Reputation_KillEnenmies());
            else
                if (current_strategy >= 40 && current_strategy < 70)
                startingActions.Add(new Reputation_ObtainRareItems());
            else
                startingActions.Add(new Reputation_VisitDangerousPlace());
        }
    }
}
