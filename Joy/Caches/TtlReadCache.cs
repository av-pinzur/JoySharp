namespace AvP.Joy.Caches;

public class TtlReadCache<TKey, TValue> : IReadCache<TKey, TValue> where TKey : notnull
{
    private readonly ReaderWriterLockSlim cacheLock = new(LockRecursionPolicy.SupportsRecursion);
    private readonly Dictionary<TKey, DatedValue> cache = new();
    private readonly TimeSpan maxAge;
    private readonly Func<DateTimeOffset> nowProvider;

    public TtlReadCache(TimeSpan maxAge, Func<DateTimeOffset>? nowProvider = null)
    {
        if (maxAge < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(maxAge));

        this.maxAge = maxAge;
        this.nowProvider = nowProvider ?? (() => DateTimeOffset.UtcNow);
    }

    public TValue GetOrAdd(TKey key, Func<TValue> valueFn)
    {
        using (cacheLock.EnterReadLockDisposable())
            if (cache.TryGetValue(key, out DatedValue? dated) && !IsExpired(dated))
                return dated.Value;

        using (cacheLock.EnterWriteLockDisposable())
        {
            if (cache.TryGetValue(key, out DatedValue? dated) && !IsExpired(dated))
                return dated.Value;

            var value = valueFn();
            cache[key] = new(nowProvider(), value);
            return value;
        }
    }

    private bool IsExpired(DatedValue dated) =>
        F.Let(
            nowProvider() - dated.FetchedAt,
            age => age > maxAge
        );

    private record DatedValue(DateTimeOffset FetchedAt, TValue Value);
}
