﻿using System;
using System.Collections.Generic;

namespace AvP.Joy
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueGetter)
        {
            if (null == dictionary) throw new ArgumentNullException(nameof(dictionary));
            if (null == valueGetter) throw new ArgumentNullException(nameof(valueGetter));

            TValue value;
            if (!dictionary.TryGetValue(key, out value))
            {
                value = valueGetter();
                dictionary.Add(key, value);
            }
            return value;
        }

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue fallback = default(TValue))
        {
            if (null == dictionary) throw new ArgumentNullException(nameof(dictionary));

            TValue value;
            return dictionary.TryGetValue(key, out value)
                ? value
                : fallback;
        }
    }
}