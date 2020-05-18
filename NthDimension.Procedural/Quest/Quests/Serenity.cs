using NthDimension.Procedural.Quest.Actions.Serenity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Quests
{
    class SerenityQuest : QuestGenerator
    {
        public SerenityQuest()
        {
            this.type = questType.Serenity;
        }

        override public void generateStrategy()
        {
            int current_strategy = rnd1.Next(0, 100);
            if (current_strategy < 11)
                startingActions.Add(new Serenity_Revenge());
            else
            {
                if (current_strategy < 28)
                    startingActions.Add(new Serenity_Capture1());
                else
                    if (current_strategy < 35)
                    startingActions.Add(new Serenity_Capture2());
                else
                        if (current_strategy < 45)
                    startingActions.Add(new Serenity_CheckOnNPC1());
                else
                            if (current_strategy < 58)
                    startingActions.Add(new Serenity_CheckOnNPC2());
                else
                                if (current_strategy < 75)
                    startingActions.Add(new Serenity_RecoverLost());
                else
                    startingActions.Add(new Serenity_RescueCaptured());


            }
        }
    }
}
