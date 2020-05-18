using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest
{
    public abstract class StartingActions
    {
        public void Write(int indent)
        {
            DisplayActions(indent);
        }


        public abstract void GenerateActions();
        protected string name;
        protected List<QuestAction> actions = new List<QuestAction>();

        protected void DisplayActions(int indent)
        {
            Console.WriteLine("Type:{0}", name);
            actions.ForEach(delegate (QuestAction a)
            {
                a.DisplaySingleAction(indent);
            });
        }

        public List<QuestAction> GetActions()
        {
            return actions;
        }
    };
}
