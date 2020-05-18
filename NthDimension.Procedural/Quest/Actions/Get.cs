using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions
{
    public class Get : QuestAction
    {
        public Get()
        {
            this.name = "Get";
            this.obj = new Item();
            generateStartingAction();
        }

        public Get(QuestObject item_to_get)
        {
            this.name = "Get";
            this.obj = item_to_get;
            generateStartingAction();
        }

        void generateStartingAction()
        {
            int current_action = rnd1.Next(0, 4);
            if (current_action == 0)
            {
                subActions.Add(new None());
            }
            else
            {
                if (current_action == 1)
                {
                    subActions.Add(new Steal(obj));
                }
                else
                {
                    if (current_action == 2)
                    {
                        subActions.Add(new GoTo(obj));
                        subActions.Add(new Gather(obj));
                    }
                    else
                    {
                        subActions.Add(new GoTo(obj));
                        subActions.Add(new Get(obj));
                        NPC npc = new NPC();
                        subActions.Add(new GoTo(npc));
                        if (QuestGenerator.GetSubquestGeneration())
                        {
                            subQuest sq = new subQuest();
                            sq.quest.InitializeStartingStrategies();
                            sq.quest.InitializeObjects();
                            subActions.Add(sq);
                        }
                        subActions.Add(new Exchange(obj, npc));
                    }
                }
            }
        }

        override public void DisplaySingleAction(int indent)
        {
            DrawIndent(indent);
            Console.WriteLine("{0} {1}", this.name, obj.getName());
            writeSubActions(indent);
        }
    };
}
