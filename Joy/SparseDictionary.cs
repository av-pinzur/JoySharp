using System.Collections;

namespace AvP.Joy;

public class ReadOnlyCollectionFacade<T> : ICollection<T>
{
    private readonly IEnumerable<T> items;

    public ReadOnlyCollectionFacade(IEnumerable<T> items)
    {
        this.items = items;
    }

    public int Count { get { return items.Count(); } }
    public bool IsReadOnly { get { return true; } }

    public void CopyTo(T[] array, int arrayIndex)
        => items.ToList().CopyTo(array, arrayIndex);

    public IEnumerator<T> GetEnumerator()
        => items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    #region Invalid Interface Operations

    private const string READ_ONLY = "The current collection is read-only.";

    void ICollection<T>.Add(T item)
    {
        throw new InvalidOperationException(READ_ONLY);
    }

    void ICollection<T>.Clear()
    {
        throw new InvalidOperationException(READ_ONLY);
    }

    bool ICollection<T>.Contains(T item)
    {
        throw new InvalidOperationException(READ_ONLY);
    }

    bool ICollection<T>.Remove(T item)
    {
        throw new InvalidOperationException(READ_ONLY);
    }

    #endregion
}

/// <summary>Implementation of <see cref="IDictionary{TKey, TValue}"/> that always appears to have a value set for every possible key..</summary>
public class SparseDictionaryWrapper<TKey, TValue> : IDictionary<TKey, TValue>
{
    private readonly IDictionary<TKey, TValue> subject;
    private readonly IEnumerable<TKey> keys;
    private readonly TValue defaultValue;

    public SparseDictionaryWrapper(IDictionary<TKey, TValue> subject, IEnumerable<TKey> keys, TValue defaultValue)
    {
        if (null == subject) throw new ArgumentNullException(nameof(subject));
        if (null == keys) throw new ArgumentNullException(nameof(keys));

        this.subject = subject;
        this.keys = keys;
        this.defaultValue = defaultValue;
    }

    public bool IsReadOnly => subject.IsReadOnly;
    public int Count => keys.Count();

    public TValue this[TKey key]
    {
        get
        {
            TValue? result;
            return subject.TryGetValue(key, out result) ? result : defaultValue;
        }
        set => subject[key] = value;
    }

    public ICollection<TKey> Keys =>
        new ReadOnlyCollectionFacade<TKey>(keys);

    public ICollection<TValue> Values =>
        new ReadOnlyCollectionFacade<TValue>(keys.Select(k => this[k]));

    public bool ContainsKey(TKey key)
        => true;

    public bool TryGetValue(TKey key, out TValue value)
    {
        value = this[key];
        return true;
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        => Equals(this[item.Key], item.Value);

    private IEnumerable<KeyValuePair<TKey, TValue>> AllPairs
        => keys.Select(k => new KeyValuePair<TKey, TValue>(k, this[k]));

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        => AllPairs.ToList().CopyTo(array, arrayIndex);

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        => AllPairs.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #region Invalid Interface Operations

    private const string IMMUTABLE_KEYSET = "The current dictionary has an immutable key set.";

    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
    {
        throw new InvalidOperationException(IMMUTABLE_KEYSET);
    }

    void ICollection<KeyValuePair<TKey, TValue>>.Clear()
    {
        throw new InvalidOperationException(IMMUTABLE_KEYSET);
    }

    void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
    {
        throw new InvalidOperationException(IMMUTABLE_KEYSET);
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
    {
        throw new InvalidOperationException(IMMUTABLE_KEYSET);
    }

    bool IDictionary<TKey, TValue>.Remove(TKey key)
    {
        throw new InvalidOperationException(IMMUTABLE_KEYSET);
    }

    #endregion
}
