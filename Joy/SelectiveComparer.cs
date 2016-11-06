using System;
using System.Collections.Generic;

namespace AvP.Joy
{
    public abstract class SelectiveComparer<T> : ComparerBase<T>
    {
        private enum SortDirection { Ascending = 1, Descending = -1 }

        private SelectiveComparer() { }

        public static SelectiveComparer<T> OrderBy<TComparand>(Func<T, TComparand> comparandSelector, IComparer<TComparand> comparer = null)
        {
            return new Impl<TComparand>(comparandSelector, comparer, SortDirection.Ascending);
        }

        public static SelectiveComparer<T> OrderByDescending<TComparand>(Func<T, TComparand> comparandSelector, IComparer<TComparand> comparer = null)
        {
            return new Impl<TComparand>(comparandSelector, comparer, SortDirection.Descending);
        }

        public SelectiveComparer<T> ThenBy<TComparand>(Func<T, TComparand> comparandSelector, IComparer<TComparand> comparer = null)
        {
            return new Chained<TComparand>(this, comparandSelector, comparer, SortDirection.Ascending);
        }

        public SelectiveComparer<T> ThenByDescending<TComparand>(Func<T, TComparand> comparandSelector, IComparer<TComparand> comparer = null)
        {
            return new Chained<TComparand>(this, comparandSelector, comparer, SortDirection.Descending);
        }

        private class Impl<TComparand> : SelectiveComparer<T>
        {
            private readonly Func<T, TComparand> comparandSelector;
            private readonly IComparer<TComparand> comparer;
            private readonly SortDirection direction;

            public Impl(Func<T, TComparand> comparandSelector, IComparer<TComparand> comparer, SortDirection direction)
            {
                if (comparandSelector == null) throw new ArgumentNullException(nameof(comparandSelector));

                this.comparandSelector = comparandSelector;
                this.comparer = comparer ?? Comparer<TComparand>.Default;
                this.direction = direction;
            }

            protected override int CompareImpl(T first, T second)
            {
                return comparer.Compare(comparandSelector(first), comparandSelector(second)) * (int)direction;
            }
        }

        private class Chained<TComparand> : Impl<TComparand>
        {
            private readonly SelectiveComparer<T> previousComparer;

            public Chained(SelectiveComparer<T> previousComparer, Func<T, TComparand> comparandSelector, IComparer<TComparand> comparer, SortDirection direction)
                : base(comparandSelector, comparer, direction)
            {
                if (previousComparer == null) throw new ArgumentNullException(nameof(previousComparer));
                this.previousComparer = previousComparer;
            }

            protected sealed override int CompareImpl(T x, T y)
            {
                return previousComparer.CompareOr(x, y, base.CompareImpl);
            }
        }
    }
}