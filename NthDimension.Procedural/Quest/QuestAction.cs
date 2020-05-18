using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest
{
    public abstract class QuestAction : IQuestAction
    {
        protected static Random rnd1;
        protected string name;
        protected QuestObject obj;
        protected List<QuestAction> subActions = new List<QuestAction>();
        override public void DisplaySingleAction(int indent)
        {
            DrawIndent(indent);
            if (obj == null)
                Console.WriteLine("{0}", this.name);
            else
                Console.WriteLine("{0} {1}", this.name, this.obj.getName());
            writeSubActions(indent);
        }
        public static void Init(Random rnd_ref)
        {
            rnd1 = rnd_ref;
        }
        public void DrawIndent(int indent)
        {
            for (int i = 0; i < indent; i++)
            {
                Console.Write(" ");
            }
        }
        public void DrawIndentWithDashes(int indent)
        {
            for (int i = 0; i < indent; i++)
            {
                Console.Write("-");
            }
        }
        protected void writeSubActions(int indent)
        {
            indent = indent + 2;
            if (this.subActions.Count != 0)
                this.subActions.ForEach(delegate (QuestAction a)
                {
                    a.DisplaySingleAction(indent);
                });
        }
        public QuestObject GetObject()
        {
            return this.obj;
        }

        override public QuestObject GetNPC()
        {
            return null;
        }

        public List<QuestAction> GetAllSubActions()
        {
            return subActions;
        }
    };
}
