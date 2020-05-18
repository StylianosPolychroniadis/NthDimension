using System.Collections.Generic;
using System.Linq;

namespace NthDimension.Procedural.Quest
{
    public abstract class QuestObject : IQuestObject
    {
        protected string name;
        protected int x;
        protected int y;
        public QuestObject()
        {
            this.x = rnd1.Next(0, 100);
            this.y = rnd1.Next(0, 100);
            this.name = "ObjName";
        }
        public QuestObject(string name)
        {
            this.name = name;
            generatePosition();
        }
        public void setName(string new_name)
        {
            this.name = new_name;
        }


        void generatePosition()
        {
            this.x = rnd1.Next(0, 100);
            this.y = rnd1.Next(0, 100);
        }
        override public string getName()
        {
            return this.name;
        }
        public void setCoords(int new_x, int new_y)
        {
            this.x = new_x;
            this.y = new_y;
        }
        public void setCoords(QuestObject o)
        {
            this.x = o.x;
            this.y = o.y;
        }
        public int getXCoord()
        {
            return this.x;
        }
        public int getYCoord()
        {
            return this.y;
        }
        protected string generateName(List<string> prefixes, List<string> names, List<string> suffixes)
        {
            int action = rnd1.Next(0, 10);
            if (action < 2)
            {
                return names[rnd1.Next(0, names.Count())];
            }
            else
            {
                if (action < 5)
                {
                    return prefixes[rnd1.Next(0, prefixes.Count())] + " " + names[rnd1.Next(0, names.Count())];
                }
                else
                {
                    if (action < 8)
                    {
                        return names[rnd1.Next(0, names.Count())] + " " + suffixes[rnd1.Next(0, suffixes.Count())];
                    }
                    else
                    {
                        return prefixes[rnd1.Next(0, prefixes.Count())] + " " + names[rnd1.Next(0, names.Count())] + " " + suffixes[rnd1.Next(0, suffixes.Count())];
                    }
                }
            }
        }
        protected string generateFragmentNames(string name)
        {
            List<string> prefixes = new List<string>
            {
                "Parts of",
                "Fragments of",
                "Shards of",
                "Elements of"
            };
            return (prefixes[rnd1.Next(0, prefixes.Count)] + " " + name);
        }
    }
}
