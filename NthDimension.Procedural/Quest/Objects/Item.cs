using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Objects
{
    public class Item : QuestObject
    {

        public Item()
        {
            GenerateItemName();
            this.x = rnd1.Next(0, 100);
            this.y = rnd1.Next(0, 100);
        }
        protected void GenerateItemName()
        {
            List<string> prefixes = new List<string> { "Magic", "Lost in time", "Stolen", "Sparkling", "Glooming", "Important", "Poisonous", "Powerfull", "Lost", "Mighty" };
            List<string> names = new List<string> { "Inscripted Stone", "Book", "Tome", "Diamond", "Note", "Sword", "Dagger", "Message", "Trap", "Spear" };
            List<string> suffixes = new List<string> { "of the Snake", "of the Wolf", "of the Dead", "of arcane magic", "from the Master", "from the One", "of the Invoker", "of the Bandit", "from the Friend", "from the Enemy" };
            this.name = generateName(prefixes, names, suffixes);
        }
    }
}
