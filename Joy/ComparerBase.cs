using System.Collections.Generic;

namespace AvP.Joy
{
    public abstract class ComparerBase<T> : Comparer<T>
    {
        public override int Compare(T x, T y)
        {
            return ReferenceEquals(x, y) ? 0
                : x == null ? -1
                : y == null ? 1
                : CompareImpl(x, y);
        }

        protected abstract int CompareImpl(T x, T y);
    }
}