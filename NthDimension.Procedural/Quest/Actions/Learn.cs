using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions
{
    public class Learn : QuestAction
    {
        public Learn()
        {
            this.name = "Learn information about";
            generateStartingAction();
        }

        public Learn(QuestObject o)
        {
            this.name = "Learn information about";
            this.obj = o;
            generateStartingAction();
        }

        void generateStartingAction()
        {
            int next_action = rnd1.Next(0, 3);
            if (next_action == 0)
            {
                this.subActions.Add(new None());
            }
            else
            {
                if (next_action == 1)
                {
                    NPC npc = new NPC();
                    this.subActions.Add(new GoTo(npc));
                    this.subActions.Add(new Listen(npc));
                    if (QuestGenerator.GetSubquestGeneration())
                    {
                        subQuest sq = new subQuest();
                        sq.quest.InitializeStartingStrategies();
                        sq.quest.InitializeObjects();
                        subActions.Add(sq);
                    }
                    this.subActions.Add(new Listen(npc));
                }
                else
                {
                    if (next_action == 2)
                    {
                        Item i = new Item();
                        this.subActions.Add(new GoTo(i));
                        this.subActions.Add(new Get(i));
                        this.subActions.Add(new Read(i));
                    }
                    else
                    {
                        if (obj == null)
                        {
                            Item obj = new Item();
                        }
                        NPC npc = new NPC();
                        this.subActions.Add(new Get(obj));
                        if (QuestGenerator.GetSubquestGeneration())
                        {
                            subQuest sq = new subQuest();
                            sq.quest.InitializeStartingStrategies();
                            sq.quest.InitializeObjects();
                            subActions.Add(sq);
                        }
                        this.subActions.Add(new Give(obj));
                        this.subActions.Add(new Listen(npc));
                    }
                }
            }
        }
    }
}
