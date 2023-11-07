using System.Diagnostics.CodeAnalysis;

namespace AvP.Joy;

public static class DictionaryExtensions
{
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueGetter)
    {
        if (!dictionary.TryGetValue(key, out TValue? value))
        {
            value = valueGetter();
            dictionary.Add(key, value);
        }
        return value;
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static TValue? GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue? defaultValue = default)
    {
        if (null == dictionary) throw new ArgumentNullException(nameof(dictionary));

        TValue? value;
        return dictionary.TryGetValue(key, out value)
            ? value
            : defaultValue;
    }
}
