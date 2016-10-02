using System.Linq;

namespace AvP.Joy
{
    public static class ObjectExtensions
    {
        public static bool IsAmong<TValue>(this TValue value, params TValue[] set)
            => set.Contains(value);

        public static string ToStringOrDefault<TValue>(this Maybe<TValue> value, string defaultValue)
            => value.HasValue ? value.Value.ToString() : defaultValue;

        public static string ToStringOrDefault<TValue>(this TValue value, string defaultValue) where TValue : class
            => value == null ? defaultValue : value.ToString();

        public static string ToStringOrDefault<TValue>(this TValue? value, string defaultValue) where TValue : struct
            => value == null ? defaultValue : value.ToString();

        public static int GetHashCodeNullable(this object obj)
            => null == obj ? 0 : obj.GetHashCode();

        /*
        public static bool? EqualsNullCheck(this object objA, object objB)
            => null == objA ? null == objB : null == objB ? false : default(bool?);
        */
    }
}