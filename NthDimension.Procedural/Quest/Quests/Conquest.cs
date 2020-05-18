using NthDimension.Procedural.Quest.Actions.Conquest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Quests
{
    class ConquestQuest : QuestGenerator
    {
        public ConquestQuest()
        {
            this.type = questType.Conquest;
        }

        override public void generateStrategy()
        {
            int current_strategy = rnd1.Next(0, 100);
            if (current_strategy < 50)
                startingActions.Add(new Conquest_Attack());
            else
                startingActions.Add(new Conquest_Steal());

        }
    }
}
