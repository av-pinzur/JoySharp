using AvP.Joy.Enumerables;

namespace AvP.Joy;

public static class EnumerableComparer
{
    public static IComparer<IEnumerable<T>> ByElementsOrdered<T>() where T : IComparable<T>
        => new Ordered<T>(Comparer<T>.Default);

    public static IComparer<IEnumerable<T>> ByElementsOrdered<T>(IComparer<T> comparer)
        => new Ordered<T>(comparer);

    public static IComparer<IEnumerable<T>> ByElementsUnordered<T>() where T : IComparable<T>
        => new Unordered<T>(Comparer<T>.Default);

    public static IComparer<IEnumerable<T>> ByElementsUnordered<T>(IComparer<T> comparer)
        => new Unordered<T>(comparer);

    private sealed class Ordered<T> : ComparerBase<IEnumerable<T>>
    {
        private IComparer<T> elementComparer;

        public Ordered(IComparer<T> elementComparer)
        {
            this.elementComparer = elementComparer;
        }

        protected override int CompareImpl(IEnumerable<T> x, IEnumerable<T> y)
        {
            return x.CompareByElementsOrdered(y, elementComparer);
        }
    }

    private sealed class Unordered<T> : ComparerBase<IEnumerable<T>>
    {
        private IComparer<T> elementComparer;

        public Unordered(IComparer<T> elementComparer)
        {
            this.elementComparer = elementComparer;
        }

        protected override int CompareImpl(IEnumerable<T> x, IEnumerable<T> y)
        {
            return x.CompareByElementsUnordered(y, elementComparer);
        }
    }
}
