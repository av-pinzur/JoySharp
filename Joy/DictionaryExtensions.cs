using System;
using System.Collections.Generic;

namespace AvP.Joy
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueGetter)
        {
            TValue value;
            if (!dictionary.TryGetValue(key, out value))
            {
                value = valueGetter();
                dictionary.Add(key, value);
            }
            return value;
        }
    }
}