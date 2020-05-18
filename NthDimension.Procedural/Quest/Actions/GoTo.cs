using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions
{
    public class GoTo : QuestAction
    {
        int x;
        int y;
        public GoTo()
        {
            this.name = "GoTo";
            generateStartingAction();
            generateX();
            generateY();
        }

        public GoTo(QuestObject o)
        {
            this.name = "GoTo";
            this.obj = o;
            generateStartingAction();
            generateX();
            generateY();
        }

        void generateStartingAction()
        {
            int next_action = rnd1.Next(0, 3);
            if (next_action == 0)
            {
                this.subActions.Add(new None());
            }
            else
            {
                if (next_action == 1)
                    this.subActions.Add(new Learn(obj));
                else
                    this.subActions.Add(new Explore(obj));
            }
        }

        void generateX()
        {
            this.x = rnd1.Next(0, 100);
        }
        void generateY()
        {
            this.y = rnd1.Next(0, 100);
        }

        override public void DisplaySingleAction(int indent)
        {
            DrawIndent(indent);
            Console.WriteLine("{0} {1}:{2}", this.name, this.x, this.y);
            writeSubActions(indent);
        }
    };
}
