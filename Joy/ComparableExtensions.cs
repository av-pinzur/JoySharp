using System;
using System.Collections.Generic;
using System.Linq;

namespace AvP.Joy
{
    public static class ComparableExtensions
    {
        public static T MinVs<T>(this T first, params T[] others) where T : IComparable<T>
            => others.Aggregate(first, Comparer<T>.Default.Min);

        public static T MaxVs<T>(this T first, params T[] others) where T : IComparable<T>
            => others.Aggregate(first, Comparer<T>.Default.Max);

        public static int CompareToNullable<T>(this T x, T y) where T : IComparable<T>
            => x == null 
                ? y == null 
                    ? 0 
                    : -1 
                : y == null 
                    ? 1 
                    : x.CompareTo(y);
    }
}