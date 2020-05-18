using NthDimension.Procedural.Quest.Actions.Knowledge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Quests
{
    class KnowledgeQuest : QuestGenerator
    {
        public KnowledgeQuest()
        {
            this.type = questType.Knowledge;
        }

        override public void generateStrategy()
        {
            int current_strategy = rnd1.Next(0, 100);
            if (current_strategy < 30)
                startingActions.Add(new Knowledge_DeliverItemForStudy());
            else
            if (current_strategy >= 30 && current_strategy < 60)
                startingActions.Add(new Knowledge_InterviewNPC());
            else
                if (current_strategy >= 60 && current_strategy < 90)
                startingActions.Add(new Knowledge_UseItemInTheField());
            else
                startingActions.Add(new Knowledge_Spy());
        }
    }
}
