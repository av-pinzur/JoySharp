using System;
using System.Collections.Generic;

namespace AvP.Joy
{
    public class DelegatingComparer<T> : IComparer<T>
    {
        private readonly Comparison<T> comparison;

        public DelegatingComparer(Comparison<T> comparison)
        {
            if (comparison == null) throw new ArgumentNullException("comparison");
            this.comparison = comparison;
        }

        public int Compare(T x, T y) { return comparison(x, y); }
    }
}