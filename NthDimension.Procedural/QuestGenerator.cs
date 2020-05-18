using System;
using System.Collections.Generic;

using NthDimension.Procedural.Quest;
using NthDimension.Procedural.Quest.Actions;

namespace NthDimension.Procedural
{
    
    abstract public class QuestGenerator : ISimpleQuest
    {
        protected questType                 type;
        protected static Random             rnd1 = null;
        protected static Boolean            canGenerateSubquests = true;
        protected int                       amount_of_starting_actions;
        protected int                       indent = 0;
        protected uint                      actions_remained = 0;
        protected List<StartingActions>     startingActions;
        protected bool                      canGenerateSubquest = true;

        public void setSubquestGeneration(Boolean n)
        {
            this.canGenerateSubquest = n;
        }

        public bool getSubquestGeneration()
        {
            return this.canGenerateSubquest;
        }

        public QuestGenerator()
        {
            if (rnd1 == null)
                throw new Exception("Quest not initialized. Use SimpleQuest.Init() first.");
            amount_of_starting_actions = rnd1.Next(1, 3);
            startingActions = new List<StartingActions>();
        }

        public static void Init(Random r)
        {
            rnd1 = r;
            QuestAction.Init(rnd1);
            IQuestObject.Init(new Random(rnd1.Next()));
        }

        public static void SetSubquestGeneration(Boolean b)
        {
            canGenerateSubquests = b;
        }

        public static Boolean GetSubquestGeneration()
        {
            return canGenerateSubquests;
        }

        public void ChangeIndent(int new_value)
        {
            this.indent = new_value;
        }

        public void InitializeStartingStrategies()
        {
            for (int i = 0; i < amount_of_starting_actions; i++)
            {
                generateStrategy();
            }
        }

        questType GenerateRandomQuestType()
        {
            Array values = Enum.GetValues(typeof(questType));
            return (questType)values.GetValue(rnd1.Next(values.Length));
        }

        public void ChangeRandom(Random r)
        {
            Init(r);
        }

        public void changeAmountOfStartingActions(int new_value)
        {
            if (new_value <= 0)
            {
                throw new Exception("changeAmountOfStrategies<=0, should be >=1");
            }
            this.amount_of_starting_actions = new_value;
            startingActions = new List<StartingActions>();
        }
        public void DrawIndent(int indent)
        {
            for (int i = 0; i < indent; i++)
            {
                Console.Write(" ");
            }
        }


        public void InitializeObjects()
        {
            for (int i = 0; i < amount_of_starting_actions; i++)
            {
                startingActions[i].GenerateActions();
            }
        }
        public void DisplayQuest()
        {
            Console.WriteLine("Type:{0}", type);
            for (int i = 0; i < amount_of_starting_actions; i++)
            {
                DrawIndent(this.indent);
                Console.Write("{0})", i);
                startingActions[i].Write(indent + 2);
            }
        }

        public List<QuestObject> GetAllObjects()
        {
            List<QuestObject> objects = new List<QuestObject>();
            foreach (var startingAction in startingActions)
            {
                foreach (var action in startingAction.GetActions())
                {
                    QuestObject actionObj = action.GetObject();
                    if (actionObj != null && !objects.Contains(actionObj))
                        objects.Add(actionObj);
                    if (action is Exchange)
                    {
                        QuestObject actionNPC = action.GetNPC();
                        if (actionNPC != null && !objects.Contains(actionNPC))
                            objects.Add(actionNPC);
                    }
                    foreach (var subAction in action.GetAllSubActions())
                    {
                        QuestObject subActionObj = subAction.GetObject();
                        if (subActionObj != null && !objects.Contains(subActionObj))
                            objects.Add(subActionObj);
                        if (subAction is Exchange)
                        {
                            QuestObject subActionNPC = subAction.GetNPC();
                            if (subActionNPC != null && !objects.Contains(subActionNPC))
                                objects.Add(subActionNPC);
                        }
                    }
                }
            }

            return objects;
        }

        public void PrintAllObjects()
        {
            foreach (var obj in this.GetAllObjects())
                Console.WriteLine(obj.getName() + " x coord: " + obj.getXCoord() + " y coord:" + obj.getYCoord());
        }

    };
}
