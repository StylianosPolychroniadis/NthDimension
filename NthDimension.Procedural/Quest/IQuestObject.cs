using System;

namespace NthDimension.Procedural.Quest
{
    abstract public class IQuestObject
    {
        protected static Random rnd1;
        public static void Init(Random random)
        {
            rnd1 = random;
        }

        public abstract string getName();

    }
}
