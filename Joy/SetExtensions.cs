namespace AvP.Joy;

public static class SetExtensions
{
    public static bool Excludes<TValue>(this ISet<TValue> set, TValue item)
    {
        return !set.Contains(item);
    }
}
