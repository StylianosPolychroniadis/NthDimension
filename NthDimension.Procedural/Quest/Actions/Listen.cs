using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions
{
    public class Listen : QuestAction
    {
        public Listen()
        {
            this.name = "Listen";
            this.obj = new NPC();
        }

        public Listen(QuestObject o)
        {
            this.name = "Listen";
            this.obj = o;
        }

        override public void DisplaySingleAction(int indent)
        {
            DrawIndent(indent);
            Console.WriteLine("{0} to {1}", this.name, this.obj.getName());
            writeSubActions(indent);
        }
    }
}
