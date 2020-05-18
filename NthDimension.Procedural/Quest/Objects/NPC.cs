using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Objects
{
    public class NPC : QuestObject
    {
        public NPC()
        {
            generateNPCname();
            this.x = rnd1.Next(0, 100);
            this.y = rnd1.Next(0, 100);
        }

        protected void generateNPCname()
        {
            List<string> prefixes = new List<string> { "Elder", "Magician", "Warrior", "Ancient", "Brilliant", "Seeker", "Timeless", "Unbound", "Friendly", "Betrayed" };
            List<string> names = new List<string> { "Bond", "Mankrik", "Meto", "Merayl", "Perculia", "Alyssia", "Chargla", "Dalron", "Daakara", "Kilnara" };
            List<string> suffixes = new List<string> { "of the Sun", "of the Moon", "bloody", "the traitor", "the Warrior", "of Odyn", "the Mage", "the fierce", "of the Forest", "the Awesome" };
            this.name = generateName(prefixes, names, suffixes);
        }
    }
}
