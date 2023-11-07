using System.Diagnostics.CodeAnalysis;

namespace AvP.Joy.Sequences;

public static class Sequence
{
    #region AsSequence, ViaSequence, AsEnumerable, ToList

    public static ISequence<char> AsSequence(this string source)
    {
        IDisposable disposer;
        return source.AsSequence(out disposer);
    }

    public static ISequence<TSource> AsSequence<TSource>(this IEnumerable<TSource> source, out IDisposable disposer)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        var e = source.GetEnumerator();
        try
        {
            disposer = e;
            return e.AsSequence();
        }
        catch
        {
            if (e != null) e.Dispose();
            throw;
        }
    }

    public static ISequence<TSource> AsSequence<TSource>(this IEnumerator<TSource> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        return (source.MoveNext())
            ? new LazySequence<TSource>(source.Current, () => source.AsSequence())
             : Sequence.Empty<TSource>();
    }

    public static IEnumerable<TResult> ViaSequence<TSource, TResult>(this IEnumerable<TSource> source, Func<ISequence<TSource>, ISequence<TResult>> sequenceOperation)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (sequenceOperation == null) throw new ArgumentNullException(nameof(sequenceOperation));

        IDisposable disposer;
        return sequenceOperation(source.AsSequence(out disposer)).AsEnumerable(disposer);
    }

    public static IEnumerable<TSource> AsEnumerable<TSource>(this ISequence<TSource> source, IDisposable? disposer = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return AsEnumerableImpl(source, disposer);
    }

    private static IEnumerable<TSource> AsEnumerableImpl<TSource>(ISequence<TSource> source, IDisposable? disposer)
    {
        using (disposer)
        {
            var o = source;
            while (o.Any)
            {
                yield return o.Head;
                o = o.GetTail();
            };
        }
    }

    public static List<TSource> ToList<TSource>(this ISequence<TSource> source)
    {
        return source.AsEnumerable().ToList();
    }

    #endregion
    #region Empty, Singleton, Over, Repeat, Range, Generate

    public static ISequence<TResult> Empty<TResult>()
    {
        return new LinkedSequence<TResult>();
    }

    public static ISequence<TResult> Singleton<TResult>(TResult value)
    {
        return new LinkedSequence<TResult>(value, Empty<TResult>());
    }

    public static ISequence<TResult> Over<TResult>(TResult first, TResult second, params TResult[] others)
    {
        if (others == null) throw new ArgumentNullException(nameof(others));

        return new LinkedSequence<TResult>(first,
            new LinkedSequence<TResult>(second,
                F<ISequence<TResult>>.YEval(0, self => i => i < others.Length
                    ? new LinkedSequence<TResult>(others[i], self(i + 1))
                    : Empty<TResult>())));
    }

    public static ISequence<TResult> Repeat<TResult>(TResult element, int count)
    {
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "Parameter must be zero or greater.");
        return count == 0 ? Empty<TResult>() : new LazySequence<TResult>(element, () => Repeat(element, count - 1));
    }

    public static ISequence<int> Range(int start, int count)
    {
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "Parameter must be zero or greater.");
        return count == 0 ? Empty<int>() : new LazySequence<int>(start, () => Range(start + 1, count - 1));
    }

    public static ISequence<int> Generate(int seed, int step)
    {
        if (step == 0) throw new ArgumentException("Parameter must not equal 0.", nameof(step));
        return Generate(seed, i => i + step);
    }

    public static ISequence<TResult> Generate<TResult>(TResult seed, Func<TResult, TResult> step)
    {
        if (step == null) throw new ArgumentNullException(nameof(step));
        return Generate(seed, o => Maybe.Some(step(o)));
    }

    public static ISequence<TResult> Generate<TResult>(TResult seed, Func<TResult, Maybe<TResult>> step)
    {
        if (step == null) throw new ArgumentNullException(nameof(step));
        return F<ISequence<TResult>>.YEval(
            Maybe.Some(seed),
            self => o => !o.HasValue ? Empty<TResult>()
                : new LazySequence<TResult>(o.Value, () => self(step(o.Value))));
    }

    #endregion
    #region OfType, Cast, ToStrings

    public static ISequence<TResult> OfType<TResult>(this ISequence<object> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return source.Where(o => o is TResult).Cast<TResult>();
    }

    public static ISequence<TResult> Cast<TResult>(this ISequence<object> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return source.Select(o => (TResult)o);
    }

    public static ISequence<string> ToStrings<TSource>(this ISequence<TSource> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return source.Select(o => o?.ToString() ?? string.Empty);
    }

    #endregion
    #region Any, None, All, Count

    public static bool Any<TSource>(this ISequence<TSource> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return source.Any;
    }

    public static bool Any<TSource>(this ISequence<TSource> source, Func<TSource, bool> predicate)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return source.Where(predicate).Any();
    }

    public static bool None<TSource>(this ISequence<TSource> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return !source.Any();
    }

    public static bool None<TSource>(this ISequence<TSource> source, Func<TSource, bool> predicate)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return !source.Any(predicate);
    }

    public static bool All<TSource>(this ISequence<TSource> source, Func<TSource, bool> predicate)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        return !source.Any(o => !predicate(o));
    }

    public static int Count<TSource>(this ISequence<TSource> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return F<int>.Loop(source, 0,
            loop => (s, count) => s.None() ? loop.Complete(count) : loop.Recur(s.GetTail(), count + 1));
    }

    public static int Count<TSource>(this ISequence<TSource> source, Func<TSource, bool> predicate)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        return source.Where(predicate).Count();
    }

    #endregion
    #region HeadOrDefault, HeadMaybe, OrEmpty, DefaultIfEmpty

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static TSource? HeadOrDefault<TSource>(this ISequence<TSource> source, TSource? defaultValue = default(TSource))
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return source.Any ? source.Head : defaultValue;
    }

    public static Maybe<TSource> HeadMaybe<TSource>(this ISequence<TSource> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return source.Any ? source.Head : Maybe<TSource>.None;
    }

    public static ISequence<TSource> OrEmpty<TSource>(this ISequence<TSource> source)
    {
        return source ?? Empty<TSource>();
    }

    public static ISequence<TSource?> DefaultIfEmpty<TSource>(this ISequence<TSource> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return source.Any ? source : Singleton<TSource?>(default);
    }

    public static ISequence<TSource> DefaultIfEmpty<TSource>(this ISequence<TSource> source, TSource defaultValue)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return source.Any ? source : Singleton<TSource>(defaultValue);
    }

    #endregion
    #region Single, SingleOrDefault, First, FirstOrDefault, Nth, NthOrDefault, Last, LastOrDefault

    public static TSource Single<TSource>(this ISequence<TSource> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (source.None()) throw new InvalidOperationException("Sequence is empty.");
        if (source.GetTail().Any()) throw new InvalidOperationException("Sequence has more than one element.");
        return source.Head;
    }

    public static TSource Single<TSource>(this ISequence<TSource> source, Func<TSource, bool> predicate)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        return source.Where(predicate).Single();
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static TSource? SingleOrDefault<TSource>(this ISequence<TSource> source, TSource? defaultValue = default(TSource))
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return source.None() ? defaultValue : source.Single();
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static TSource? SingleOrDefault<TSource>(this ISequence<TSource> source, Func<TSource, bool> predicate, TSource? defaultValue = default(TSource))
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        return source.Where(predicate).SingleOrDefault(defaultValue);
    }

    public static TSource First<TSource>(this ISequence<TSource> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return FirstImpl(source, "Sequence is empty.");
    }

    private static TSource FirstImpl<TSource>(ISequence<TSource> source, string emptyExcMsg)
    {
        if (source.None()) throw new InvalidOperationException(emptyExcMsg);
        return source.Head;
    }

    public static TSource First<TSource>(this ISequence<TSource> source, Func<TSource, bool> predicate)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        return FirstImpl(source.SkipWhile(o => !predicate(o)), "Sequence contains no elements meeting the specified criteria.");
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static TSource? FirstOrDefault<TSource>(this ISequence<TSource> source, TSource? defaultValue = default(TSource))
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return source.None() ? defaultValue : source.First();
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static TSource? FirstOrDefault<TSource>(this ISequence<TSource> source, Func<TSource, bool> predicate, TSource? defaultValue = default(TSource))
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        return source.SkipWhile(o => !predicate(o)).FirstOrDefault(defaultValue);
    }

    public static TSource Nth<TSource>(this ISequence<TSource> source, int zeroBasedIndex)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return FirstImpl(source.Skip(zeroBasedIndex), "Sequence contains too few elements.");
    }

    public static TSource Nth<TSource>(this ISequence<TSource> source, int zeroBasedIndex, Func<TSource, bool> predicate)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        return FirstImpl(source.Where(predicate).Skip(zeroBasedIndex), "Sequence contains too few elements meeting the specified critieria.");
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static TSource? NthOrDefault<TSource>(this ISequence<TSource> source, int zeroBasedIndex, TSource? defaultValue = default(TSource))
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return source.Skip(zeroBasedIndex).FirstOrDefault(defaultValue);
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static TSource? NthOrDefault<TSource>(this ISequence<TSource> source, int zeroBasedIndex, Func<TSource, bool> predicate, TSource? defaultValue = default(TSource))
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        return source.Where(predicate).NthOrDefault(zeroBasedIndex, defaultValue);
    }

    public static TSource Last<TSource>(this ISequence<TSource> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (source.None()) throw new InvalidOperationException("Sequence is empty.");
        return F<TSource>.Loop(source,
            loop => s => F.Let(s.GetTail(), tail => tail.None() ? loop.Complete(s.Head) : loop.Recur(tail)));
    }

    public static TSource Last<TSource>(this ISequence<TSource> source, Func<TSource, bool> predicate)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (source.None()) throw new InvalidOperationException("Sequence is empty.");
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        return source.Where(predicate).Last();
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static TSource? LastOrDefault<TSource>(this ISequence<TSource> source, TSource? defaultValue = default(TSource))
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return source.None() ? defaultValue : source.Last();
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static TSource? LastOrDefault<TSource>(this ISequence<TSource> source, Func<TSource, bool> predicate, TSource? defaultValue = default(TSource))
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        return source.Where(predicate).LastOrDefault(defaultValue);
    }

    #endregion
    #region Min, MinBy, Max, MaxBy, Aggregate

    public static TResult Min<TSource, TResult>(this ISequence<TSource> source, Func<TSource, TResult> resultSelector)
    {
        return source.Select(resultSelector).Min();
    }

    public static TSource Min<TSource>(this ISequence<TSource> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (source.None()) throw new InvalidOperationException("Sequence is empty.");

        return source.MinBy(Comparer<TSource>.Default);
    }

    public static TSource MinBy<TSource, TComparand>(this ISequence<TSource> source, Func<TSource, TComparand> comparandSelector)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (source.None()) throw new InvalidOperationException("Sequence is empty.");
        if (comparandSelector == null) throw new ArgumentNullException(nameof(comparandSelector));

        return source.MinBy(SelectiveComparer<TSource>.OrderBy(comparandSelector));
    }

    public static TSource MinBy<TSource>(this ISequence<TSource> source, IComparer<TSource> comparer)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (source.None()) throw new InvalidOperationException("Sequence is empty.");
        if (comparer == null) throw new ArgumentNullException(nameof(comparer));

        return source.Aggregate(comparer.Min);
    }

    public static TResult Max<TSource, TResult>(this ISequence<TSource> source, Func<TSource, TResult> resultSelector)
    {
        return source.Select(resultSelector).Max();
    }

    public static TSource Max<TSource>(this ISequence<TSource> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (source.None()) throw new InvalidOperationException("Sequence is empty.");

        return source.MaxBy(Comparer<TSource>.Default);
    }

    public static TSource MaxBy<TSource, TComparand>(this ISequence<TSource> source, Func<TSource, TComparand> comparandSelector)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (source.None()) throw new InvalidOperationException("Sequence is empty.");
        if (comparandSelector == null) throw new ArgumentNullException(nameof(comparandSelector));

        return source.MaxBy(SelectiveComparer<TSource>.OrderBy(comparandSelector));
    }

    public static TSource MaxBy<TSource>(this ISequence<TSource> source, IComparer<TSource> comparer)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (source.None()) throw new InvalidOperationException("Sequence is empty.");
        if (comparer == null) throw new ArgumentNullException(nameof(comparer));

        return source.Aggregate(comparer.Max);
    }

    public static TSource Aggregate<TSource>(this ISequence<TSource> source, Func<TSource, TSource, TSource> accumulator)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (source.None()) throw new InvalidOperationException("Sequence is empty.");

        return source.GetTail().Aggregate(source.Head, accumulator);
    }

    public static TAccumulate Aggregate<TSource, TAccumulate>(this ISequence<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (accumulator == null) throw new ArgumentNullException(nameof(accumulator));

        return F<TAccumulate>.Loop(source, seed,
            loop => (current, accumulated) =>
                !current.Any ? loop.Complete(accumulated) : loop.Recur(current.GetTail(), accumulator(accumulated, current.Head)));
    }

    public static TResult Aggregate<TSource, TAccumulate, TResult>(this ISequence<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator, Func<TAccumulate, TResult> resultSelector)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (accumulator == null) throw new ArgumentNullException(nameof(accumulator));
        if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));

        return resultSelector(source.Aggregate(seed, accumulator));
    }

    #endregion
    #region Select, Where

    public static ISequence<TResult> Select<TSource, TResult>(this ISequence<TSource> source, Func<TSource, TResult> resultSelector)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));

        return source.None() ? Empty<TResult>()
            : new LazySequence<TResult>(resultSelector(source.Head), () => source.GetTail().Select(resultSelector));
    }

    public static ISequence<TResult> Select<TSource, TResult>(this ISequence<TSource> source, Func<TSource, int, TResult> resultSelector)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));

        return F<ISequence<TResult>>.YEval(source, 0,
            self => (s, index) => s.None() ? Empty<TResult>()
                : new LazySequence<TResult>(resultSelector(s.Head, index), () => self(s.GetTail(), index + 1)));
    }

    public static ISequence<TSource> Where<TSource>(this ISequence<TSource> source, Func<TSource, bool> predicate)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        return new WrappedSequence<TSource>(source.SkipWhile(o => !predicate(o)), tail => tail.Where(predicate));
    }

    public static ISequence<TSource> Where<TSource>(this ISequence<TSource> source, Func<TSource, int, bool> predicate)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        return ApplyWithIndex(Where, source, predicate);
    }

    #endregion
    #region Index, Unindex

    public static ISequence<Indexed<TSource>> Index<TSource>(this ISequence<TSource> source)
    {
        return source.Select((o, i) => new Indexed<TSource>(i, o));
    }

    public static ISequence<TSource> Unindex<TSource>(this ISequence<Indexed<TSource>> source)
    {
        return source.Select(t => t.Value);
    }

    private static ISequence<TResult> ApplyWithIndex<TSource, TFuncResult, TResult>(
        Func<ISequence<Indexed<TSource>>, Func<Indexed<TSource>, TFuncResult>, ISequence<Indexed<TResult>>> operation,
        ISequence<TSource> source,
        Func<TSource, int, TFuncResult> function)
    {
        return Unindex(ApplyWithIndexFlat(operation, source, function));
    }

    private static TResult ApplyWithIndexFlat<TSource, TFuncResult, TResult>(
        Func<ISequence<Indexed<TSource>>, Func<Indexed<TSource>, TFuncResult>, TResult> operation,
        ISequence<TSource> source,
        Func<TSource, int, TFuncResult> function)
    {
        return operation(Index(source), t => function(t.Value, t.Index));
    }

    #endregion
    #region SelectMany, Concat

    public static ISequence<TResult> SelectMany<TSource, TResult>(
        this ISequence<TSource> source,
        Func<TSource, ISequence<TResult>> selector)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (selector == null) throw new ArgumentNullException(nameof(selector));

        return source.SelectMany(selector, (s, c) => c);
    }

    public static ISequence<TResult> SelectMany<TSource, TResult>(
        this ISequence<TSource> source,
        Func<TSource, int, ISequence<TResult>> selector)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (selector == null) throw new ArgumentNullException(nameof(selector));

        return ApplyWithIndexFlat(SelectMany, source, selector);
    }

    public static ISequence<TResult> SelectMany<TSource, TCollection, TResult>(
        this ISequence<TSource> source,
        Func<TSource, ISequence<TCollection>> collectionSelector,
        Func<TSource, TCollection, TResult> resultSelector)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (collectionSelector == null) throw new ArgumentNullException(nameof(collectionSelector));
        if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));

        return F<ISequence<TResult>>.YEval(source,
            self1 => s => s.None() ? Empty<TResult>()
                : F<ISequence<TResult>>.YEval(collectionSelector(s.Head),
                    self2 => c => c.None() ? self1(s.GetTail())
                        : new LazySequence<TResult>(resultSelector(s.Head, c.Head), () => self2(c.GetTail()))));
    }

    public static ISequence<TResult> SelectMany<TSource, TCollection, TResult>(
        this ISequence<TSource> source,
        Func<TSource, int, ISequence<TCollection>> collectionSelector,
        Func<TSource, TCollection, TResult> resultSelector)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (collectionSelector == null) throw new ArgumentNullException(nameof(collectionSelector));
        if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));

        Func<Indexed<TSource>, TCollection, TResult> _resultSelector = (s, c) => resultSelector(s.Value, c);
        return ApplyWithIndexFlat(
            (_source, _collectionSelector) => SelectMany(_source, _collectionSelector, _resultSelector),
            source,
            collectionSelector);
    }

    public static ISequence<TSource> Concat<TSource>(this ISequence<TSource> first, ISequence<TSource> second)
    {
        if (first == null) throw new ArgumentNullException(nameof(first));
        if (second == null) throw new ArgumentNullException(nameof(second));

        return ConcatSequence<TSource>.Create(first, second);
    }

    #endregion
    #region Take, TakeWhile, Skip, SkipWhile

    public static ISequence<TSource> Take<TSource>(this ISequence<TSource> source, int count)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "Parameter must be zero or greater.");

        return count > 0
            ? new WrappedSequence<TSource>(source, tail => tail.Take(count - 1))
            : Empty<TSource>();
    }

    public static ISequence<TSource> TakeWhile<TSource>(this ISequence<TSource> source, Func<TSource, bool> predicate)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        return source.Any && predicate(source.Head)
            ? new WrappedSequence<TSource>(source, tail => tail.TakeWhile(predicate))
            : Empty<TSource>();
    }

    public static ISequence<TSource> TakeWhile<TSource>(this ISequence<TSource> source, Func<TSource, int, bool> predicate)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        return ApplyWithIndex(TakeWhile, source, predicate);
    }

    public static ISequence<TSource> Skip<TSource>(this ISequence<TSource> source, int count)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "Parameter must be zero or greater.");

        return F<ISequence<TSource>>.Loop(source, count,
            loop => (s, c) => s.None() || c == 0 ? loop.Complete(s) : loop.Recur(s.GetTail(), c - 1));
    }

    public static ISequence<TSource> SkipWhile<TSource>(this ISequence<TSource> source, Func<TSource, bool> predicate)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        return F<ISequence<TSource>>.Loop(source,
            loop => (s) => s.None() || !predicate(s.Head) ? loop.Complete(s) : loop.Recur(s.GetTail()));
    }

    public static ISequence<TSource> SkipWhile<TSource>(this ISequence<TSource> source, Func<TSource, int, bool> predicate)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        return ApplyWithIndex(SkipWhile, source, predicate);
    }

    #endregion
    #region Zip, ZipAll

    public static ISequence<TResult> Zip<TFirst, TSecond, TResult>(this ISequence<TFirst> first, ISequence<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
    {
        if (first == null) throw new ArgumentNullException(nameof(first));
        if (second == null) throw new ArgumentNullException(nameof(second));
        if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));

        return first.None() || second.None() ? Empty<TResult>()
            : new LazySequence<TResult>(
                resultSelector(first.Head, second.Head),
                () => first.GetTail().Zip(second.GetTail(), resultSelector));
    }

    public static ISequence<TResult> Zip<TSource, TResult>(this ISequence<ISequence<TSource>> sources, Func<ISequence<TSource>, TResult> resultSelector)
    {
        if (sources == null) throw new ArgumentNullException(nameof(sources));
        if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));

        return sources.Any(source => source.None()) ? Empty<TResult>()
            : new LazySequence<TResult>(
                resultSelector(sources.Select(source => source.Head)),
                () => sources.Select(source => source.GetTail()).Zip(resultSelector));
    }

    public static ISequence<TResult> ZipAll<TFirst, TSecond, TResult>(this ISequence<TFirst> first, ISequence<TSecond> second, Func<Maybe<TFirst>, Maybe<TSecond>, TResult> resultSelector)
    {
        if (first == null) throw new ArgumentNullException(nameof(first));
        if (second == null) throw new ArgumentNullException(nameof(second));
        if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));

        return first.None() && second.None() ? Empty<TResult>()
            : new LazySequence<TResult>(
                resultSelector(first.HeadMaybe(), second.HeadMaybe()),
                () => first.GetTail().ZipAll(second.GetTail(), resultSelector));
    }

    public static ISequence<TResult> ZipAll<TSource, TResult>(this ISequence<ISequence<TSource>> sources, Func<ISequence<Maybe<TSource>>, TResult> resultSelector)
    {
        if (sources == null) throw new ArgumentNullException(nameof(sources));
        if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));

        return sources.None(source => source.Any) ? Empty<TResult>()
            : new LazySequence<TResult>(
                resultSelector(sources.Select(HeadMaybe)),
                () => sources.Select(source => source.GetTail()).ZipAll(resultSelector));
    }

    #endregion
    #region Slide

    public static ISequence<ISequence<TSource>> Slide<TSource>(this ISequence<TSource> source, int windowSize)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (windowSize < 1) throw new ArgumentOutOfRangeException(nameof(windowSize), "Parameter value must not be less than 1.");

        return source.None() ? Empty<ISequence<TSource>>()
            : new LazySequence<ISequence<TSource>>(source.Take(windowSize), () => source.GetTail().Slide(windowSize));
    }

    public static ISequence<ISequence<TSource>> Slide<TSource>(this ISequence<TSource> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        return source.None() ? Empty<ISequence<TSource>>()
            : new LazySequence<ISequence<TSource>>(source, () => source.GetTail().Slide());
    }

    #endregion
}
