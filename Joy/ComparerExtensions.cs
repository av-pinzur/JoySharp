using System;
using System.Collections.Generic;

namespace AvP.Joy
{
    public static class ComparerExtensions
    {
        public static T Min<T>(this IComparer<T> comparer, T first, T second)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            return comparer.Compare(first, second) <= 0 ? first : second;
        }

        public static TSource MinBy<T, TSource>(this IComparer<T> comparer, Func<TSource, T> selector, TSource first, TSource second)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            return comparer.Compare(selector(first), selector(second)) <= 0 ? first : second;
        }

        public static T Max<T>(this IComparer<T> comparer, T first, T second)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            return comparer.Compare(first, second) >= 0 ? first : second;
        }

        public static TSource MaxBy<T, TSource>(this IComparer<T> comparer, Func<TSource, T> selector, TSource first, TSource second)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            return comparer.Compare(selector(first), selector(second)) >= 0 ? first : second;
        }

        public static int CompareOr<T>(this IComparer<T> comparer, T x, T y, IComparer<T> fallbackComparer)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            if (fallbackComparer == null) throw new ArgumentNullException(nameof(fallbackComparer));

            return comparer.CompareOr(x, y, fallbackComparer.Compare);
        }

        public static int CompareOr<T>(this IComparer<T> comparer, T x, T y, Comparison<T> fallbackComparison)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            if (fallbackComparison == null) throw new ArgumentNullException(nameof(fallbackComparison));

            return comparer.CompareOr(x, y, () => fallbackComparison(x, y));
        }

        public static int CompareOr<T>(this IComparer<T> comparer, T x, T y, Func<int> fallbackResult)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            if (fallbackResult == null) throw new ArgumentNullException(nameof(fallbackResult));

            var firstResult = comparer.Compare(x, y);
            return firstResult != 0 ? firstResult : fallbackResult();
        }

        public static int CompareOr<T>(this IComparer<T> comparer, T x, T y, int fallbackResult)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            return comparer.CompareOr(x, y, () => fallbackResult);
        }

        public static IComparer<T> AsComparer<T>(this Comparison<T> comparison)
        {
            return new DelegatingComparer<T>(comparison);
        }

        public static IComparer<T> AsComparer<T>(this Func<T, T, int> comparison)
        {
            return new DelegatingComparer<T>((x, y) => comparison(x, y));
        }

        public static IComparer<T> Reverse<T>(this IComparer<T> comparer)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            return new DelegatingComparer<T>((x, y) => -comparer.Compare(x, y));
        }
    }
}