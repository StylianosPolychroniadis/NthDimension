using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions
{
    public class Exchange : QuestAction
    {
        QuestObject item;
        QuestObject npc;
        public Exchange()
        {
            this.name = "Exchange";
            this.obj = new Item();
        }
        public Exchange(QuestObject new_item, QuestObject new_NPC)
        {
            this.name = "Exchange";
            this.item = new_item;
            this.npc = new_NPC;
        }

        override public void DisplaySingleAction(int indent)
        {
            DrawIndent(indent);
            Console.WriteLine("{0} {1} with {2}", this.name, this.item.getName(), this.npc.getName());
            writeSubActions(indent);
        }

        override public QuestObject GetNPC()
        {
            return npc;
        }

    }
}
