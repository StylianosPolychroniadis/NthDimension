using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Collections
{
    public static class DictionaryExtensions
    {
        public static TValue GetValueOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> creationFunc)
        {
            TValue tValue;
            if (!dictionary.TryGetValue(key, out tValue))
            {
                tValue = creationFunc(key);
                dictionary.Add(key, tValue);
            }
            return tValue;
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            return dictionary.GetValueOrDefault(key, (TKey _) => defaultValue);
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> defaultFunc)
        {
            TValue result;
            if (!dictionary.TryGetValue(key, out result))
            {
                return defaultFunc(key);
            }
            return result;
        }
    }
}
