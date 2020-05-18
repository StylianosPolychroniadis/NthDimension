using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions
{
    public class Capture : QuestAction
    {
        public Capture()
        {
            this.name = "Capture";
            this.obj = new NPC();
        }
        public Capture(QuestObject o)
        {
            this.name = "Capture";
            this.obj = o;
        }
        override public void DisplaySingleAction(int indent)
        {
            DrawIndent(indent);
            Console.WriteLine("{0} {1}", this.name, this.obj.getName());
            writeSubActions(indent);
        }
    }
}
