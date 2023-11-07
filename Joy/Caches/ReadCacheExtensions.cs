namespace AvP.Joy.Caches;

public static class ReadCacheExtensions
{
    public static Func<T, TResult> Memoize<T, TResult>(
        this IReadCache<T, TResult> cache,
        Func<T, TResult> fn
    ) where T : notnull =>
        arg => cache.GetOrAdd(arg, () => fn(arg));

    public static Func<TValue> Prefetch<TValue>(
        this IReadCache<Voidlike, TValue> cache,
        Func<TValue> fn
    )
    {
        var result = F.Decorate(fn, cache.Memoize);
        var _ = result();
        return result;
    }
}
