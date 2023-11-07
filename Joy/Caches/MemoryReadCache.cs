
namespace AvP.Joy.Caches;

public class MemoryReadCache<TKey, TValue> : IReadCache<TKey, TValue> where TKey : notnull
{
    private readonly ReaderWriterLockSlim cacheLock = new(LockRecursionPolicy.SupportsRecursion);
    private readonly Dictionary<TKey, DatedValue> cache = new();
    private readonly IEvictionPolicy<TKey> evictionPolicy;
    private readonly IExpiryPolicy<TValue> expiryPolicy;
    private readonly Func<DateTimeOffset> nowProvider;

    public MemoryReadCache(
        IEvictionPolicy<TKey>? evictionPolicy = null,
        IExpiryPolicy<TValue>? expiryPolicy = null,
        Func<DateTimeOffset>? nowProvider = null
    )
    {
        this.evictionPolicy = evictionPolicy ?? new NoEvictionPolicy<TKey>();
        this.expiryPolicy = expiryPolicy ?? new NoExpiryPolicy<TValue>();
        this.nowProvider = nowProvider ?? (() => DateTimeOffset.UtcNow);
    }

    public static MemoryReadCache<TKey, TValue> WithFifoEviction(int maxCount = 1024) =>
        new MemoryReadCache<TKey, TValue>(
            evictionPolicy: new FifoEvictionPolicy<TKey>(maxCount)
        );

    public static MemoryReadCache<TKey, TValue> WithTtlExpiry(TimeSpan maxAge, Func<DateTimeOffset>? nowProvider = null) =>
        new MemoryReadCache<TKey, TValue>(
            expiryPolicy: new TtlExpiryPolicy<TValue>(maxAge),
            nowProvider: nowProvider
        );

    public TValue GetOrAdd(TKey key, Func<TValue> valueFn)
    {
        evictionPolicy.OnGetting(key);

        // Try with a read-only lock to maximize throughput.
        using (cacheLock.EnterReadLockDisposable())
            if (cache.TryGetValue(key, out var dated)
                && !expiryPolicy.IsExpired(dated.Value, dated.FetchedAt, nowProvider()))
                return dated.Value;

        // Release read lock before obtaining write lock
        // to avoid deadlock potential.
        using (cacheLock.EnterWriteLockDisposable())
        {
            // Check again for a current value,
            // since another thread could have fetched it
            // once we released our original read lock.
            if (cache.TryGetValue(key, out var dated)
                && !expiryPolicy.IsExpired(dated.Value, dated.FetchedAt, nowProvider()))
                return dated.Value;

            var value = valueFn();
            cache[key] = new(nowProvider(), value);

            if (evictionPolicy.ShouldEvictOnAdding(key, out TKey? keyToEvict))
                cache.Remove(keyToEvict);

            return value;
        }
    }

    private record DatedValue(DateTimeOffset FetchedAt, TValue Value);
}
