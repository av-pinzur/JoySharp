namespace AvP.Joy;

public static class TupleExtensions
{
    public static Tuple<T1, T2> Append<T1, T2>(this Tuple<T1> source, T2 obj)
        => Tuple.Create(source.Item1, obj);

    public static Tuple<T1, T2, T3> Append<T1, T2, T3>(this Tuple<T1, T2> source, T3 obj)
        => Tuple.Create(source.Item1, source.Item2, obj);

    public static Tuple<T1, T2, T3, T4> Append<T1, T2, T3, T4>(this Tuple<T1, T2, T3> source, T4 obj)
        => Tuple.Create(source.Item1, source.Item2, source.Item3, obj);

    public static Tuple<T1, T2, T3, T4, T5> Append<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4> source, T5 obj)
        => Tuple.Create(source.Item1, source.Item2, source.Item3, source.Item4, obj);

    public static Tuple<T1, T2, T3, T4, T5, T6> Append<T1, T2, T3, T4, T5, T6>(this Tuple<T1, T2, T3, T4, T5> source, T6 obj)
        => Tuple.Create(source.Item1, source.Item2, source.Item3, source.Item4, source.Item5, obj);

    public static Tuple<T1, T2, T3, T4, T5, T6, T7> Append<T1, T2, T3, T4, T5, T6, T7>(this Tuple<T1, T2, T3, T4, T5, T6> source, T7 obj)
        => Tuple.Create(source.Item1, source.Item2, source.Item3, source.Item4, source.Item5, source.Item6, obj);

    public static Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>> Append<T1, T2, T3, T4, T5, T6, T7, T8>(this Tuple<T1, T2, T3, T4, T5, T6, T7> source, T8 obj)
        => Tuple.Create(source.Item1, source.Item2, source.Item3, source.Item4, source.Item5, source.Item6, source.Item7, obj);
}
