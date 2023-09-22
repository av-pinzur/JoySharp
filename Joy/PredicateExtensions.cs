namespace AvP.Joy
{
    public static class PredicateExtensions
    {
        public static Predicate<T> Not<T>(this Predicate<T> predicate)
            => value => !predicate(value);

        public static Predicate<T> And<T>(this Predicate<T> predicate, Predicate<T> other)
            => value => predicate(value) && other(value);

        public static Predicate<T> Or<T>(this Predicate<T> predicate, Predicate<T> other)
            => value => predicate(value) || other(value);

        public static Predicate<T> Xor<T>(this Predicate<T> predicate, Predicate<T> other)
            => value => predicate(value) ^ other(value);
    }
}
