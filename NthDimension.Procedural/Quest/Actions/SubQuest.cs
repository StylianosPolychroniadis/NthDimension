using NthDimension.Procedural.Quest.Quests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions
{
    public class subQuest : QuestAction
    {
        public QuestGenerator quest;
        int quest_indent = 0;
        public subQuest()
        {
            this.name = "subQuest";
            int action = rnd1.Next(0, Enum.GetNames(typeof(questType)).Length);
            if (action == 0)
                quest = new KnowledgeQuest();
            if (action == 1)
                quest = new ComfortQuest();
            if (action == 2)
                quest = new ReputationQuest();
            if (action == 3)
                quest = new SerenityQuest();
            if (action == 4)
                quest = new ProtectionQuest();
            if (action == 5)
                quest = new ConquestQuest();
            if (action == 6)
                quest = new WealthQuest();
            if (action == 7)
                quest = new EquipmentQuest();
        }
        override public void DisplaySingleAction(int indent)
        {
            indent = indent + quest_indent;
            DrawIndentWithDashes(indent);
            Console.Write("{0} started, ", this.name);
            writeSubActions(indent);
            quest.InitializeStartingStrategies();
            quest.ChangeIndent(indent);
            quest.DisplayQuest();
            DrawIndentWithDashes(indent);
            Console.WriteLine("{0} end", this.name);
        }
    }
}
