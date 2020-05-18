using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions
{
    public class Report : QuestAction
    {
        public Report()
        {
            this.name = "Report";
            this.obj = new NPC();
        }
        public Report(QuestObject o)
        {
            this.name = "Report";
            this.obj = o;
        }

        override public void DisplaySingleAction(int indent)
        {
            DrawIndent(indent);
            Console.WriteLine("{0} to {1}", this.name, this.obj.getName());
            writeSubActions(indent);
        }
    };
}
