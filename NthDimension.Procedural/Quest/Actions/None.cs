using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions
{
    public class None : QuestAction
    {
        public None()
        {
            this.name = "Nothing";
        }

        override public void DisplaySingleAction(int indent)
        {
            /*
            DrawIndent(indent);
            Console.WriteLine("{0}", this.name);
            writeSubActions(indent);
            */
        }
    }
}
