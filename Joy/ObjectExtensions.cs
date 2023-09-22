using System.Diagnostics.CodeAnalysis;

namespace AvP.Joy
{
    public static class ObjectExtensions
    {
        public static bool IsAmong<TValue>(this TValue value, params TValue[] set)
            => set.Contains(value);

        public static bool IsAmong<TValue>(this TValue value, IEnumerable<TValue> set)
            => set.Contains(value);

        public static bool IsAmong<TValue>(this TValue value, ICollection<TValue> set)
            => set.Contains(value);

        [return: NotNullIfNotNull("defaultValue")]
        public static string? ToStringOrDefault<TValue>(this Maybe<TValue> value, string? defaultValue = default)
            => value.HasValue ? value.Value.ToStringOrDefault(defaultValue) : defaultValue;

        [return: NotNullIfNotNull("defaultValue")]
        public static string? ToStringOrDefault<TValue>(this TValue? value, string? defaultValue = default)
            => value?.ToString() ?? defaultValue;

        public static int GetHashCodeNullable(this object? obj)
            => obj?.GetHashCode() ?? 0;

        /*
        public static bool? EqualsNullCheck(this object objA, object objB)
            => null == objA ? null == objB : null == objB ? false : default(bool?);
        */
    }
}