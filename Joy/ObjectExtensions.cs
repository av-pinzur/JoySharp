using System.Linq;

namespace AvP.Joy
{
    public static class ObjectExtensions
    {
        public static bool IsAmong<TValue>(this TValue value, params TValue[] set)
        {
            return set.Contains(value);
        }

        public static string ToStringOrDefault<TValue>(this Maybe<TValue> value, string defaultValue)
        {
            return value.HasValue ? value.Value.ToString() : defaultValue;
        }

        public static string ToStringOrDefault<TValue>(this TValue value, string defaultValue) where TValue : class
        {
            return value == null ? defaultValue : value.ToString();
        }

        public static string ToStringOrDefault<TValue>(this TValue? value, string defaultValue) where TValue : struct
        {
            return value == null ? defaultValue : value.ToString();
        }
    }
}