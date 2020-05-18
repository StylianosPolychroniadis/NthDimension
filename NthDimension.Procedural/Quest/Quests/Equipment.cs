using NthDimension.Procedural.Quest.Actions.Equipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Quests
{
    class EquipmentQuest : QuestGenerator
    {
        public EquipmentQuest()
        {
            this.type = questType.Equipment;
        }

        override public void generateStrategy()
        {
            int current_strategy = rnd1.Next(0, 100);
            if (current_strategy < 25)
                startingActions.Add(new Equipment_Assemble());
            else
                if (current_strategy < 50)
                startingActions.Add(new Equipment_Deliver());
            else
                    if (current_strategy < 75)
                startingActions.Add(new Equipment_Steal());
            else
                startingActions.Add(new Equipment_Trade());


        }
    }
}
