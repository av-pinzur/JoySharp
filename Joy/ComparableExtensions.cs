using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvP.Joy
{
    public static class ComparableExtensions
    {
        public static T MinVs<T>(this T first, params T[] others) where T : IComparable<T>
        {
            return others.Aggregate(first, Comparer<T>.Default.Min);
        }

        public static T MaxVs<T>(this T first, params T[] others) where T : IComparable<T>
        {
            return others.Aggregate(first, Comparer<T>.Default.Max);
        }
    }
}
