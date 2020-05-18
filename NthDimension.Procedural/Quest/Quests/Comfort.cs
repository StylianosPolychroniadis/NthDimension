using NthDimension.Procedural.Quest.Actions.Comfort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Quests
{
    class ComfortQuest : QuestGenerator
    {
        public ComfortQuest()
        {
            this.type = questType.Comfort;
        }

        override public void generateStrategy()
        {
            int current_strategy = rnd1.Next(0, 100);
            if (current_strategy < 50)
                startingActions.Add(new Comfort_KillPests());
            else
                startingActions.Add(new Comfort_ObtainLuxuries());

        }
    }
}
