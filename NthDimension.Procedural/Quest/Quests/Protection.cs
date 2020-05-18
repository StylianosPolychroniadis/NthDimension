using NthDimension.Procedural.Quest.Actions.Protection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Quests
{
    class ProtectionQuest : QuestGenerator
    {
        public ProtectionQuest()
        {
            this.type = questType.Protection;
        }

        override public void generateStrategy()
        {
            int current_strategy = rnd1.Next(0, 100);
            if (current_strategy < 11)
                startingActions.Add(new Protection_Attack());
            else
            {
                if (current_strategy < 28)
                    startingActions.Add(new Protection_TreatOrRepair1());
                else
                    if (current_strategy < 35)
                    startingActions.Add(new Protection_TreatOrRepair2());
                else
                        if (current_strategy < 45)
                    startingActions.Add(new Protection_CreateDiversion1());
                else
                            if (current_strategy < 58)
                    startingActions.Add(new Protection_CreateDiversion2());
                else
                                if (current_strategy < 75)
                    startingActions.Add(new Protection_AssembleFortification());
                else
                    startingActions.Add(new Protection_Guard());
            }
        }
    }
}
