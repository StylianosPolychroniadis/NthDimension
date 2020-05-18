using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Objects
{
    public class WallsOrGates : QuestObject
    {
        public WallsOrGates()
        {
            generateWallName();
            this.x = rnd1.Next(0, 100);
            this.y = rnd1.Next(0, 100);
        }
        protected void generateWallName()
        {
            List<string> prefixes = new List<string> { "Old", "Broken", "Damaged", "Attacked", "Requiring repair" };
            List<string> names = new List<string> { "Gates", "Walls" };
            List<string> suffixes = new List<string>();
            this.name = generateName(prefixes, names, suffixes);
        }
    }
}
