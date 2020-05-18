using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Game.Dungeon
{
    /// <summary>
    /// Choosable objects implement this interface in order to return the relative frequencies with which they are chosen.
    /// </summary>
    public interface IChoosable
    {
        /// <summary>
        /// Gets the relative frequency with which this item is chosen
        /// </summary>
        int ChooseFrequency { get; }
    }

    /// <summary>
    /// Delegate to return the relative frequency, that an item is chosen.
    /// </summary>
    /// <typeparam name="T">Object type being chosen</typeparam>
    /// <param name="item">Item being considered</param>
    /// <returns>Returns an arbitrary positive integer which represents the relative frequency with which
    /// the specified item should be chosen</returns>
    public delegate int GetFrequencyDelegate<T>(T item);

    public static class Chooser
    {
        public static T ChooseOne<T>(IEnumerable<T> list) where T : IChoosable
        {
            GetFrequencyDelegate<T> getFrequency = delegate (T item)
            {
                return item.ChooseFrequency;
            };
            return ChooseOne(list, getFrequency);
        }

        public static T ChooseOne<T>(
            IEnumerable<T> list,
            GetFrequencyDelegate<T> getFrequency)
        {
            // Calculate the total weights of all objects
            int totalFrequency = 0;
            foreach (T item in list)
                totalFrequency += getFrequency(item);

            // Choose a value within the bounds of all the frequencies
            int value = Globals.Rand.Next(totalFrequency);

            // Find the item whose frequency bounds contain the random value
            foreach (T item in list)
            {
                int frequency = getFrequency(item);

                // Does our random value fall within this item's range?
                if (value < frequency)
                    return item;

                // This item has been eliminated; adjust the value accordingly and continue searching
                value -= frequency;
            }

            // This code should be logically impossible to reach, but the compiler doesn't know that
            return default(T);
        }
    }
}
