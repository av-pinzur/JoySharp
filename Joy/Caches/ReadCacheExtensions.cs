namespace AvP.Joy.Caches;

public static class ReadCacheExtensions
{
    public static Func<T, TResult> Memoize<T, TResult>(
        this IReadCache<T, TResult> cache,
        Func<T, TResult> fn
    ) where T : notnull =>
        arg => cache.GetOrAdd(arg, () => fn(arg));
}
