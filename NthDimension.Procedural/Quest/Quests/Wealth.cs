using NthDimension.Procedural.Quest.Actions.Wealth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Quests
{
    class WealthQuest : QuestGenerator
    {
        public WealthQuest()
        {
            this.type = questType.Wealth;
        }


        override public void generateStrategy()
        {
            int current_strategy = rnd1.Next(0, 100);
            if (current_strategy < 45)
                startingActions.Add(new Wealth_GatherRawMaterials());
            else
            if (current_strategy >= 45 && current_strategy < 90)
                startingActions.Add(new Wealth_StealValuablesForResale());
            else
                startingActions.Add(new Wealth_MakeValuablesForResale());
        }
    }
}
