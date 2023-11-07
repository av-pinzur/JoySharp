namespace AvP.Joy;

public static class EqualityComparerExtensions
{
    public static int GetHashCodeNullable<T>(this IEqualityComparer<T> equalityComparer, T? obj)
        => null == obj ? 0 : equalityComparer.GetHashCode(obj);
}
