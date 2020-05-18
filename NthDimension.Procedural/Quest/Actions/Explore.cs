using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions
{
    public class Explore : QuestAction
    {
        public Explore()
        {
            this.name = "Explore/Check/Find about";
        }

        public Explore(QuestObject o)
        {
            this.name = "Explore/Check/Find about";
            this.obj = o;
        }

        override public void DisplaySingleAction(int indent)
        {
            DrawIndent(indent);
            if (this.obj != null)
                Console.WriteLine("{0} {1}", this.name, this.obj.getName());
            else
                Console.WriteLine("{0}", this.name);


            writeSubActions(indent);
        }
    }
}
