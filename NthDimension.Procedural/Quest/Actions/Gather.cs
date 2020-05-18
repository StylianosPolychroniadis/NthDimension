using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions
{
    public class Gather : QuestAction
    {
        public Gather()
        {
            this.name = "Gather/PickUp";
            this.obj = new Item();
        }

        public Gather(QuestObject o)
        {
            this.name = "Gather/PickUp";
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
