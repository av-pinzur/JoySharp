using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvP.Joy
{
    public static class EnumerableObjectExtensions
    {
        public static IEnumerable<TValue> InSingleton<TValue>(this TValue value)
        {
            yield return value;
        }

        public static IEnumerable<TValue> InSingletonIf<TValue>(this TValue value, bool predicate)
        {
            if (predicate) yield return value;
        }

        public static IEnumerable<TValue> InSingletonIf<TValue>(this TValue value, Func<TValue, bool> predicate)
        {
            if (predicate(value)) yield return value;
        }

        public static IEnumerable<TValue> InSingletonOrEmpty<TValue>(this TValue value) where TValue : class
        {
            return value.InSingletonIf(value != null);
        }

        public static IEnumerable<TValue> InSingletonOrEmpty<TValue>(this TValue? nullableValue) where TValue : struct
        {
            return (nullableValue ?? default(TValue)).InSingletonIf(nullableValue.HasValue);
        }

        public static IEnumerable<TValue> FollowedBy<TValue>(this TValue head, IEnumerable<TValue> tail)
        {
            yield return head;
            foreach (var o in tail) yield return o;
        }
    }
}