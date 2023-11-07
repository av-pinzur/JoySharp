namespace AvP.Joy;

public static class Maybe
{
    public static Maybe<T> Some<T>(T value)
        => Maybe<T>.Some(value);

    public static Maybe<T> If<T>(bool condition, Func<T> valueGetter)
        => Maybe<T>.If(condition, valueGetter);

    public static Maybe<T> IfNonNull<T>(T value)
        => Maybe<T>.IfNonNull(value);

    public static int CompareTo<T>(this Maybe<T> x, Maybe<T> y) where T : IComparable<T>
        => CompareBy(System.Collections.Generic.Comparer<T>.Default).Compare(x, y);

    public static IEqualityComparer<Maybe<T>> EquateBy<T>(IEqualityComparer<T> valueEqualityComparer)
        => new EqualityComparer<T>(valueEqualityComparer);

    public static IComparer<Maybe<T>> CompareBy<T>(IComparer<T> valueComparer)
        => new Comparer<T>(valueComparer);

    private struct EqualityComparer<T> : IEqualityComparer<Maybe<T>>
    {
        private IEqualityComparer<T> valueEqualityComparer;

        public EqualityComparer(IEqualityComparer<T> valueEqualityComparer)
        {
            this.valueEqualityComparer = valueEqualityComparer;
        }

        public bool Equals(Maybe<T> x, Maybe<T> y)
            => x.HasValue == y.HasValue
                && valueEqualityComparer.Equals(x.Value, y.Value);

        public int GetHashCode(Maybe<T> obj)
            => !obj.HasValue ? 0
                : valueEqualityComparer.GetHashCodeNullable(obj.Value);
    }

    private struct Comparer<T> : IComparer<Maybe<T>>
    {
        private IComparer<T> valueComparer;

        public Comparer(IComparer<T> valueComparer)
        {
            this.valueComparer = valueComparer;
        }

        public int Compare(Maybe<T> x, Maybe<T> y)
            => x.HasValue
                ? y.HasValue
                    ? valueComparer.Compare(x.Value, y.Value)
                    : 1
                : y.HasValue
                    ? -1
                    : 0;
    }
}
