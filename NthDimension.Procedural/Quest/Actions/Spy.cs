using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions
{
    public class Spy : QuestAction
    {
        public Spy()
        {
            this.name = "Spy";
            this.obj = new NPC();
        }

        public Spy(QuestObject o)
        {
            this.name = "Spy";
            this.obj = o;
        }

        override public void DisplaySingleAction(int indent)
        {
            DrawIndent(indent);
            Console.WriteLine("{0} on {1}", this.name, this.obj.getName());
            writeSubActions(indent);
        }
    };
}
