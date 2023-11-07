namespace AvP.Joy.Caches;

public class TtlReadCache<TKey, TValue> : MemoryReadCache<TKey, TValue> where TKey : notnull
{
    public TtlReadCache(TimeSpan maxAge, Func<DateTimeOffset>? nowProvider = null)
        : base(expiryPolicy: new TtlExpiryPolicy<TValue>(maxAge), nowProvider: nowProvider) { }
}
