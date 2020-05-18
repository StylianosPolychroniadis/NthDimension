using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Game.Dungeon
{
    public struct Dice
    {
        public static Dice Empty = new Dice(0, 0);

        [System.Xml.Serialization.XmlIgnore()]
        public int Sides;

        [System.Xml.Serialization.XmlIgnore()]
        public int Count;

        [System.Xml.Serialization.XmlIgnore()]
        public int Low;

        [System.Xml.Serialization.XmlIgnore()]
        public int High;

        [System.Xml.Serialization.XmlAttribute("desc")]
        public string Desc
        {
            get
            {
                if (Low == 0 && High == 0)
                {
                    if (Sides == 0)
                        return Count.ToString();
                    else
                        return string.Format("{0}d{1}", Count, Sides);
                }
                else
                    return string.Format("{0}-{1}", Low, High);
            }
            set
            {
                Dice setValue;
                if (TryParse(value, out setValue))
                {
                    Sides = setValue.Sides;
                    Count = setValue.Count;
                    Low = setValue.Low;
                    High = setValue.High;
                }
            }
        }

        public int GetAverage100()
        {
            if (Low == 0 && High == 0)
            {
                int die = Sides <= 0 ? 1 : Sides;
                int min = 100;
                int max = 100 * die;
                int dieAvg = (min + max) / 2;
                return dieAvg * Count;
            }
            else
            {
                return 50 * (Low + High);
            }
        }

        public Dice(int count, int sides)
        {
            Count = count;
            Sides = sides;
            Low = 0;
            High = 0;
        }

        public static int Throw(Dice d)
        {
            if (d.Low == 0 && d.High == 0)
            {
                if (d.Sides == 0)
                    return d.Count;
                else
                {
                    MT19937 rand = Globals.Rand;
                    int total = 0;
                    for (int i = 0; i < d.Count; i++)
                    {
                        total += d.Sides > 1 ? rand.Next(1, d.Sides) : d.Sides;
                    }
                    return total;
                }
            }
            else
                return Globals.RandRange(d.Low, d.High);
        }

        public static int Throw(int count, int minValue, int maxValue)
        {
            if (minValue == maxValue)
                return minValue;
            MT19937 rand = Globals.Rand;
            int diff = maxValue - minValue;
            if (diff < count)
                return rand.Next(minValue, maxValue);
            int maxPerRoll = diff / count;
            int totalValue = minValue;
            int totalRolled = 0;
            for (int i = 0; i < count - 1; i++)
            {
                totalValue += rand.Next(maxPerRoll + 1);
                totalRolled += maxPerRoll;
            }
            if (totalRolled < diff)
                totalValue += rand.Next(diff - totalRolled + 1);
            return totalValue;
        }

        public static bool TryParse(string s, out Dice d)
        {
            d = new Dice(0, 0);
            if (string.IsNullOrEmpty(s))
                return false;
            int splitIndex = s.IndexOf('d');
            if (splitIndex >= s.Length - 1)
                return false;
            else if (splitIndex < 0)
            {
                splitIndex = s.IndexOf('-');
                if (splitIndex >= s.Length - 1)
                    return false;
                else if (splitIndex <= 0)
                    return int.TryParse(s, out d.Count);
                else
                    return int.TryParse(s.Substring(0, splitIndex), out d.Low) &&
                        int.TryParse(s.Substring(splitIndex + 1), out d.High);
            }
            else
                return int.TryParse(s.Substring(0, splitIndex), out d.Count)
                    && int.TryParse(s.Substring(splitIndex + 1), out d.Sides);
        }

        public bool IsEmpty()
        {
            return Low == 0 && High == 0 && Count == 0 && Sides == 0;
        }
    }
}
