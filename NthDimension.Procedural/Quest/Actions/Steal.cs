using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions
{
    public class Steal : QuestAction
    {
        public Steal()
        {
            this.name = "Steal";
            this.obj = new Item();
            generateStartingAction();
        }
        public Steal(QuestObject obj)
        {
            this.name = "Steal";
            this.obj = obj;
            generateStartingAction();
        }

        void generateStartingAction()
        {
            int current_action = rnd1.Next();
            if (current_action == 0)
            {
                subActions.Add(new GoTo(obj));
                subActions.Add(new Stealth());
                subActions.Add(new Take(obj));
            }
            else
            {
                NPC npc = new NPC();
                npc.setCoords(obj);
                subActions.Add(new GoTo(npc));
                subActions.Add(new Kill(npc));
                subActions.Add(new Take(obj));
            }
        }

        override public void DisplaySingleAction(int indent)
        {
            DrawIndent(indent);
            Console.WriteLine("{0} {1}", this.name, obj.getName());
            writeSubActions(indent);
        }
    };
}
