namespace AvP.Joy.Caches;

public interface IReadCache<TKey, TValue> where TKey : notnull
{
    TValue GetOrAdd(TKey key, Func<TValue> valueFn);
}