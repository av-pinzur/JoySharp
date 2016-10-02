using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AvP.Joy.Sequences;

namespace AvP.Joy.Enumerables
{
    public static class EnumerableExtensions
    {
        #region Generate

        public static IEnumerable<int> Generate(int seed, int step)
        {
            if (step == 0) throw new ArgumentException("Parameter must not equal 0.", "step");
            return Generate(seed, i => i + step);
        }

        public static IEnumerable<T> Generate<T>(T seed, Func<T, T> step)
        {
            if (step == null) throw new ArgumentNullException("step");
            return Generate(seed, o => Maybe.Some(step(o)));
        }

        public static IEnumerable<T> Generate<T>(T seed, Func<T, Maybe<T>> step)
        {
            var o = Maybe.Some(seed);
            while (o.HasValue)
            {
                yield return o.Value;
                o = step(o.Value);
            }
        }

        #endregion
        #region Slide

        public static IEnumerable<IReadOnlyList<TSource>> Slide<TSource>(this IEnumerable<TSource> source, int windowSize)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (windowSize < 1) throw new ArgumentOutOfRangeException("windowSize", "Parameter value must not be less than 1.");

            IDisposable disposer;
            var sequence = source.AsSequence(out disposer);
            return sequence.Slide(windowSize).AsEnumerable(disposer).Select(Sequence.ToList);

            // Fully functional; speed better (31 secs / 1 million).
            //IDisposable disposer;
            //var sequence = source.AsSequence(out disposer);
            //return F<ISequence<List<TSource>>>.YEval(
            //        sequence.Take(windowSize), sequence.Skip(windowSize),
            //        self => (frame, remainder) => 
            //            frame.None() ? Sequence.Empty<List<TSource>>() 
            //                : new LazySequence<List<TSource>>(
            //                    frame.ToList(), 
            //                    () => self(frame.Skip(1).Concat(remainder.Take(1)), remainder.Skip(1)) ) )
            //    .AsEnumerable(disposer);

            // Works great ... reeeeeeally slowly.
            //var pair = source.ToEnumerableHead();
            //var disposable = pair.Item1;
            //var head = pair.Item2;
            //return EnumerableHead.Repeat(head, windowSize)
            //    .Select((e, i) => e.Skip(i))
            //    .ZipAll(e => e.Where(o => o.HasValue).Select(o => o.Value).ToList())
            //    .AsEnumerable(disposable);

            // Fast, but state-full.
            //using (var e = source.GetEnumerator())
            //{
            //    var firstFrame = e.Take(windowSize).ToList();
            //    return firstFrame.None() 
            //        ? Enumerable.Empty<IReadOnlyList<TSource>>() 
            //        : Generate(
            //            firstFrame, 
            //            lastResult => e.MoveNext()
            //                ? lastResult.Skip(1).Concat(e.Current).ToList()
            //                : lastResult.Count > 1
            //                    ? lastResult.Skip(1).ToList()
            //                    : Maybe<List<TSource>>.None);
            //}

            // Works like a dream ... slowly.
            //return Fork(source, windowSize)
            //    .Select((e, i) => e.AsEnumerable().Skip(i))
            //    .ZipAll(e => e.Where(o => o.HasValue).Select(o => o.Value).ToList());
        }

        #endregion
        #region Zip, ZipAll

        public static IEnumerable<TResult> ZipAll<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<Maybe<TFirst>, Maybe<TSecond>, TResult> resultSelector)
        {
            if (null == first) throw new ArgumentNullException(nameof(first));
            if (null == second) throw new ArgumentNullException(nameof(second));
            if (null == resultSelector) throw new ArgumentNullException(nameof(resultSelector));
            return ZipAllImpl(first, second, resultSelector);
        }

        private static IEnumerable<TResult> ZipAllImpl<TFirst, TSecond, TResult>(IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<Maybe<TFirst>, Maybe<TSecond>, TResult> resultSelector)
        {
            bool any1, any2;
            using (var e1 = first.GetEnumerator())
            using (var e2 = second.GetEnumerator())
            {
                while ((any1 = e1.MoveNext()) | (any2 = e2.MoveNext()))
                    yield return resultSelector(
                            Maybe.If(any1, () => e1.Current),
                            Maybe.If(any2, () => e2.Current));
            }
        }

        public static IEnumerable<TResult> Zip<TSource, TResult>(this IEnumerable<IEnumerable<TSource>> sources, Func<IEnumerable<TSource>, TResult> resultSelector)
        {
            if (null == sources) throw new ArgumentNullException(nameof(sources));
            if (null == resultSelector) throw new ArgumentNullException(nameof(resultSelector));

            var sourceList = sources.ToList();
            if (sourceList.Contains(null)) throw new ArgumentNullException(nameof(sources));

            return ZipImpl(sourceList, resultSelector);
        }

        private static IEnumerable<TResult> ZipImpl<TSource, TResult>(List<IEnumerable<TSource>> sources, Func<IEnumerable<TSource>, TResult> resultSelector)
        {
            var any = new bool[sources.Count];
            var e = new IEnumerator<TSource>[sources.Count];
            try
            {
                for (int i = 0; i < sources.Count; i++)
                    e[i] = sources[i].GetEnumerator();
                while (e.Select((o, i) => new { o, i }).Aggregate(true, (acc, cur) => acc && cur.o.MoveNext()))
                    yield return resultSelector(e.Select((_e, i) => _e.Current));
            }
            finally
            {
                foreach (var _e in e) if (_e != null) _e.Dispose();
            }
        }

        public static IEnumerable<TResult> ZipAll<TSource, TResult>(this IEnumerable<IEnumerable<TSource>> sources, Func<IEnumerable<Maybe<TSource>>, TResult> resultSelector)
        {
            if (null == sources) throw new ArgumentNullException(nameof(sources));
            if (null == resultSelector) throw new ArgumentNullException(nameof(resultSelector));

            var sourceList = sources.ToList();
            if (sourceList.Contains(null)) throw new ArgumentNullException(nameof(sources));

            return ZipAllImpl(sourceList, resultSelector);
        }

        private static IEnumerable<TResult> ZipAllImpl<TSource, TResult>(List<IEnumerable<TSource>> sources, Func<IEnumerable<Maybe<TSource>>, TResult> resultSelector)
        {
            var any = new bool[sources.Count];
            var e = new IEnumerator<TSource>[sources.Count];
            try
            {
                for (int i = 0; i < sources.Count; i++)
                    e[i] = sources[i].GetEnumerator();
                while (e.Select((o, i) => new { o, i }).Aggregate(false, (acc, cur) => acc || (any[cur.i] = cur.o.MoveNext())))
                    yield return resultSelector(e.Select((_e, i) => Maybe.If(any[i], () => _e.Current)));
            }
            finally
            {
                foreach (var _e in e) if (_e != null) _e.Dispose();
            }
        }

        #endregion
        #region Fork

        private static Tuple<IEnumerator<TSource>, IEnumerator<TSource>> Fork<TSource>(this IEnumerable<TSource> source)
        {
            var forker = new Forker<TSource>(source, 2);
            return Tuple.Create(forker.Forks[0], forker.Forks[1]);
        }

        private static IReadOnlyList<IEnumerator<TSource>> Fork<TSource>(IEnumerable<TSource> source, int forkCount)
        {
            return new Forker<TSource>(source, forkCount).Forks;
        }

        private sealed class Forker<T>
        {
            private readonly IEnumerable<T> able;
            private readonly List<ForkedEnumerator> forks;
            private IEnumerator<T> ator;

            public Forker(IEnumerable<T> source, int forkCount)
            {
                able = source;
                forks = new List<ForkedEnumerator>(forkCount);
                for (int i = 0; i < forkCount; i++)
                    forks.Add(new ForkedEnumerator(this));
            }

            public IReadOnlyList<IEnumerator<T>> Forks { get { return forks; } }

            private sealed class ForkedEnumerator : IEnumerator<T>
            {
                private readonly Forker<T> owner;
                public readonly Queue<T> queue = new Queue<T>();
                bool isStarted = false;
                bool isDisposed = false;

                public ForkedEnumerator(Forker<T> owner) { this.owner = owner; }

                public bool MoveNext()
                {
                    if (isDisposed) throw new ObjectDisposedException("forked enumerator");

                    if (!isStarted) isStarted = true;
                    else if (queue.Count > 0) queue.Dequeue();

                    if (owner.ator == null) owner.ator = owner.able.GetEnumerator();
                    if (queue.Count == 0 && owner.ator.MoveNext())
                    {
                        foreach (var o in owner.forks) o.queue.Enqueue(owner.ator.Current);
                    }
                    return queue.Count > 0;
                }

                object System.Collections.IEnumerator.Current { get { return Current; } }

                public T Current
                {
                    get
                    {
                        if (isDisposed) throw new ObjectDisposedException("forked enumerator");
                        if (queue.Count == 0) throw new InvalidOperationException("The enumerator is not positioned on a element.");
                        return queue.Peek();
                    }
                }

                public void Reset()
                {
                    throw new NotSupportedException("The enumerator does not support Reset().");
                }

                public void Dispose()
                {
                    if (isDisposed) throw new ObjectDisposedException("forked enumerator");
                    isDisposed = true;
                    owner.forks.Remove(this);
                    if (owner.forks.Count == 0) owner.ator.Dispose();
                }
            }
        }

        #endregion
        #region Concat

        public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> source, params TSource[] items)
        {
            return source.Concat((IEnumerable<TSource>)items);
        }

        #endregion
        #region ToStrings

        public static IEnumerable<string> ToStrings<TSource>(this IEnumerable<TSource> source)
        {
            if (null == source) throw new ArgumentNullException(nameof(source));
            return ToStringsImpl(source);
        }

        private static IEnumerable<string> ToStringsImpl<TSource>(IEnumerable<TSource> source)
        {
            foreach (var o in source)
            {
                yield return o == null ? null : o.ToString();
            }
        }

        #endregion
        #region Interpose

        public static IEnumerable<TSource> Interpose<TSource>(this IEnumerable<TSource> source, TSource separator)
        {
            if (null == source) throw new ArgumentNullException(nameof(source));
            return InterposeImpl(source, separator);
        }

        private static IEnumerable<TSource> InterposeImpl<TSource>(IEnumerable<TSource> source, TSource separator)
        {
            bool isFirst = true;
            foreach (var o in source)
            {
                if (!isFirst) yield return separator;
                yield return o;
                isFirst = false;
            }
        }

        #endregion
        #region None

        public static bool None<TSource>(this IEnumerable<TSource> source)
        {
            return !source.Any();
        }

        public static bool None<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return !source.Any(predicate);
        }

        #endregion
        #region OrEmpty

        public static IEnumerable<TSource> OrEmpty<TSource>(this IEnumerable<TSource> source)
        {
            return source ?? Enumerable.Empty<TSource>();
        }

        #endregion
        #region IsDistinct

        public static bool IsDistinct<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> equalityComparer = null)
        {
            if (null == source) throw new ArgumentNullException(nameof(source));
            equalityComparer = equalityComparer ?? EqualityComparer<TSource>.Default;

            var visited = new HashSet<TSource>(equalityComparer);
            foreach (var o in source)
            {
                if (visited.Contains(o))
                    return false;

                visited.Add(o);
            }
            return true;
        }

        #endregion
        #region ToDictionary, ToSortedDictionary

        public static Dictionary<TKey, TElement> ToDictionary<TKey, TElement>(this IEnumerable<KeyValuePair<TKey, TElement>> source, IEqualityComparer<TKey> comparer = null)
            => source.ToDictionary(pair => pair.Key, pair => pair.Value, comparer);

        public static Dictionary<TKey, TElement> ToDictionary<TKey, TElement>(this IEnumerable<Tuple<TKey, TElement>> source, IEqualityComparer<TKey> comparer = null)
            => source.ToDictionary(pair => pair.Item1, pair => pair.Item2, comparer);

        public static SortedDictionary<TKey, TElement> ToSortedDictionary<TKey, TElement>(this IEnumerable<KeyValuePair<TKey, TElement>> source, IComparer<TKey> comparer = null)
            => source.ToSortedDictionary(pair => pair.Key, pair => pair.Value, comparer);

        public static SortedDictionary<TKey, TElement> ToSortedDictionary<TKey, TElement>(this IEnumerable<Tuple<TKey, TElement>> source, IComparer<TKey> comparer = null)
            => source.ToSortedDictionary(pair => pair.Item1, pair => pair.Item2, comparer);

        public static SortedDictionary<TKey, TSource> ToSortedDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer = null)
            => source.ToSortedDictionary(keySelector, F.Id, comparer);

        public static SortedDictionary<TKey, TElement> ToSortedDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IComparer<TKey> comparer = null)
        {
            if (null == source) throw new ArgumentNullException(nameof(source));
            if (null == keySelector) throw new ArgumentNullException(nameof(keySelector));
            if (null == elementSelector) throw new ArgumentNullException(nameof(elementSelector));

            var result = new SortedDictionary<TKey, TElement>(comparer);
            foreach (var o in source)
                result.Add(keySelector(o), elementSelector(o));
            return result;
        }

        #endregion
        #region Nth, NthOrDefault

        public static TSource Nth<TSource>(this IEnumerable<TSource> source, int zeroBasedIndex)
            => source.Skip(zeroBasedIndex).First();

        public static TSource Nth<TSource>(this IEnumerable<TSource> source, int zeroBasedIndex, Func<TSource, bool> predicate)
            => source.Skip(zeroBasedIndex).First(predicate);

        public static TSource NthOrDefault<TSource>(this IEnumerable<TSource> source, int zeroBasedIndex)
            => source.Skip(zeroBasedIndex).FirstOrDefault();

        public static TSource NthOrDefault<TSource>(this IEnumerable<TSource> source, int zeroBasedIndex, Func<TSource, bool> predicate)
            => source.Skip(zeroBasedIndex).FirstOrDefault(predicate);

        #endregion
        #region Batch, Buffer

        public static IEnumerable<TResult> Batch<TSource, TResult>(this IEnumerable<TSource> source, int size, Func<IEnumerable<TSource>, TResult> resultSelector)
            => source.Batch(size).Select(resultSelector);

        public static IEnumerable<IEnumerable<TSource>> Batch<TSource>(this IEnumerable<TSource> source, int size)
        {
            if (null == source) throw new ArgumentNullException(nameof(source));
            if (size < 1) throw new ArgumentOutOfRangeException(nameof(size));

            return source.BatchImpl(size);
        }

        private static IEnumerable<IEnumerable<TSource>> BatchImpl<TSource>(this IEnumerable<TSource> source, int size)
        {
            using (var e = source.GetEnumerator())
            {
                while (e.MoveNext())
                    yield return e.GetBatch(size);
            }
        }

        private static IEnumerable<TSource> GetBatch<TSource>(this IEnumerator<TSource> source, int size)
        {
            yield return source.Current;
            for (int i = 1; i < size && source.MoveNext(); i++)
                yield return source.Current;
        }

        public static IEnumerable<IReadOnlyList<TSource>> Buffer<TSource>(this IEnumerable<TSource> source, int size)
            => source.Batch(size, Enumerable.ToList);

        public static IEnumerable<TResult> Buffer<TSource, TResult>(this IEnumerable<TSource> source, int size, Func<IReadOnlyList<TSource>, TResult> resultSelector)
            => source.Buffer(size).Select(resultSelector);

        #endregion
        #region Index, Unindex

        public static IEnumerable<Indexed<TSource>> Index<TSource>(this IEnumerable<TSource> source)
        {
            return source.Select((o, i) => new Indexed<TSource>(i, o));
        }

        public static IEnumerable<TSource> Unindex<TSource>(this IEnumerable<Indexed<TSource>> source)
        {
            return source.Select(t => t.Value);
        }

        private static IEnumerable<TResult> ApplyWithIndex<TSource, TFuncResult, TResult>(
            Func<IEnumerable<Indexed<TSource>>, Func<Indexed<TSource>, TFuncResult>, IEnumerable<Indexed<TResult>>> operation,
            IEnumerable<TSource> source,
            Func<TSource, int, TFuncResult> function)
        {
            return Unindex(ApplyWithIndexFlat(operation, source, function));
        }

        private static TResult ApplyWithIndexFlat<TSource, TFuncResult, TResult>(
            Func<IEnumerable<Indexed<TSource>>, Func<Indexed<TSource>, TFuncResult>, TResult> operation,
            IEnumerable<TSource> source,
            Func<TSource, int, TFuncResult> function)
        {
            return operation(Index(source), t => function(t.Value, t.Index));
        }

        #endregion
        #region AggregateWhile

        public static TAccumulate AggregateWhile<TSource, TAccumulate>(
            this IEnumerable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func,
            Func<TAccumulate, bool> predicate)
        {
            if (null == source) throw new ArgumentNullException(nameof(source));
            if (null == func) throw new ArgumentNullException(nameof(func));
            if (null == predicate) throw new ArgumentNullException(nameof(predicate));

            using (var e = source.GetEnumerator())
            {
                var acc = seed;
                while (e.MoveNext())
                {
                    acc = func(acc, e.Current);
                    if (!predicate(acc)) break;
                }

                return acc;
            }
        }

        public static TResult AggregateWhile<TSource, TAccumulate, TResult>(
            this IEnumerable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func,
            Func<TAccumulate, bool> predicate,
            Func<TAccumulate, TResult> resultSelector)
        {
            if (null == resultSelector) throw new ArgumentNullException(nameof(resultSelector));
            return resultSelector(source.AggregateWhile(seed, func, predicate));
        }

        /* Alternate signature - cleaner, but less like built-in Linq operator.
         
        public static TSource AggregateWhile_<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, TSource, TSource> reduce,
            Func<TSource, bool> predicate)
            => source.AggregateWhile_(F.Id, reduce, predicate);

        public static TAccumulate AggregateWhile_<TSource, TAccumulate>(
            this IEnumerable<TSource> source,
            Func<TSource, TAccumulate> map,
            Func<TAccumulate, TAccumulate, TAccumulate> reduce,
            Func<TAccumulate, bool> predicate)
        {
            if (null == source) throw new ArgumentNullException(nameof(source));
            if (null == map) throw new ArgumentNullException(nameof(map));
            if (null == reduce) throw new ArgumentNullException(nameof(reduce));
            if (null == predicate) throw new ArgumentNullException(nameof(predicate));

            using (var e = source.GetEnumerator())
            {
                if (!e.MoveNext()) throw new InvalidOperationException("Source contains no elements.");
                var acc = map(e.Current);

                while (e.MoveNext() && predicate(acc))
                    acc = reduce(acc, map(e.Current));

                return acc;
            }
        }

        public static TResult AggregateWhile_<TSource, TAccumulate, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TAccumulate> map,
            Func<TAccumulate, TAccumulate, TAccumulate> reduce,
            Func<TAccumulate, bool> predicate,
            Func<TAccumulate, TResult> resultSelector)
        {
            if (null == resultSelector) throw new ArgumentNullException(nameof(resultSelector));
            return resultSelector(source.AggregateWhile_(map, reduce, predicate));
        }

        public static bool All_<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
            => source.AggregateWhile_(cur => predicate(cur), (acc, cur) => acc && cur, acc => acc);

        public static bool All<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
            => source.AggregateWhile(true, (acc, cur) => acc && predicate(cur), acc => acc);

        public static Maybe<TSource> AllEqual_<TSource>(this IEnumerable<TSource> source)
            => source.AggregateWhile_(
                Maybe.Some,
                (acc, cur) => Maybe.If(Equals(acc.Value, cur.Value), acc.Value),
                acc => acc.HasValue);
        */

        #endregion
        #region AllEqual

        public static Maybe<TSource> AllEqual<TSource>(this IEnumerable<TSource> source)
            => source.AggregateWhile(
                Maybe<TSource>.None,
                (acc, cur) => Maybe.If((!acc.HasValue) || Equals(acc.Value, cur), acc.ValueOrDefault(cur)),
                acc => acc.HasValue);

        #endregion
        #region All, Any

        public static bool All(this IEnumerable<bool> source)
            => source.All(F.Id);

        public static bool Any(this IEnumerable<bool> source)
            => source.Any(F.Id);

        #endregion
        #region EqualsByValues~, GetHashCodeByValues~

        public static bool EqualsByValuesOrdered<TSource>(
            this IEnumerable<TSource> sourceA,
            IEnumerable<TSource> sourceB,
            IEqualityComparer<TSource> equalityComparer = null)
        {
            if (null == sourceA) throw new ArgumentNullException(nameof(sourceA));
            if (null == sourceB) throw new ArgumentNullException(nameof(sourceB));

            var maybeEqualityComparer = Maybe.EquateBy(
                equalityComparer ?? EqualityComparer<TSource>.Default);

            return sourceA
                .ZipAll(sourceB, (a, b) => maybeEqualityComparer.Equals(a, b))
                .All(F.Id);
        }

        public static int GetHashCodeByValuesOrdered<TSource>(
            this IEnumerable<TSource> source,
            IEqualityComparer<TSource> equalityComparer = null)
        {   // TODO: Improve hash code algorithm.
            if (null == source) throw new ArgumentNullException(nameof(source));

            equalityComparer = equalityComparer ?? EqualityComparer<TSource>.Default;
            return source.Aggregate(0, (acc, cur) => acc ^ cur.GetHashCode());
        }

        public static bool EqualsByValuesUnordered<TSource>(
            this IEnumerable<TSource> sourceA,
            IEnumerable<TSource> sourceB,
            IEqualityComparer<TSource> equalityComparer = null)
        {
            if (null == sourceA) throw new ArgumentNullException(nameof(sourceA));
            if (null == sourceB) throw new ArgumentNullException(nameof(sourceB));

            return sourceA.ToHashSet(equalityComparer).SetEquals(sourceB);
        }

        public static int GetHashCodeByValuesUnordered<TSource>(
            this IEnumerable<TSource> source,
            IEqualityComparer<TSource> equalityComparer = null)
        {   // TODO: Improve hash code algorithm.
            if (null == source) throw new ArgumentNullException(nameof(source));

            equalityComparer = equalityComparer ?? EqualityComparer<TSource>.Default;
            return source.Aggregate(0, (acc, cur) => acc + cur.GetHashCode());
        }

        #endregion
        #region To~Set, To~SetDeep, ToListDeep

        public static ISet<TSource> ToSet<TSource>(this IEnumerable<TSource> source)
        {
            if (null == source) throw new ArgumentNullException(nameof(source));

            return source as ISet<TSource> 
                ?? new HashSet<TSource>(source);
        }

        public static ISet<ISet<TSource>> ToSetDeep<TSource>(this IEnumerable<IEnumerable<TSource>> source)
            => source.Select(o => o.ToSet()).ToSet();

        public static ISet<ISet<ISet<TSource>>> ToSetDeep<TSource>(this IEnumerable<IEnumerable<IEnumerable<TSource>>> source)
            => source.Select(o => o.ToSetDeep()).ToSet();

        public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source)
        {
            if (null == source) throw new ArgumentNullException(nameof(source));

            return source as HashSet<TSource> 
                ?? new HashSet<TSource>(source);
        }

        public static HashSet<HashSet<TSource>> ToHashSetDeep<TSource>(this IEnumerable<IEnumerable<TSource>> source)
            => source.Select(o => o.ToHashSet()).ToHashSet();

        public static HashSet<HashSet<HashSet<TSource>>> ToHashSetDeep<TSource>(this IEnumerable<IEnumerable<IEnumerable<TSource>>> source)
            => source.Select(o => o.ToHashSetDeep()).ToHashSet();

        public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> equalityComparer)
        {
            if (null == source) throw new ArgumentNullException(nameof(source));
            if (null == equalityComparer) throw new ArgumentNullException(nameof(equalityComparer));

            return new HashSet<TSource>(source, equalityComparer);
        }

        public static HashSet<HashSet<TSource>> ToHashSetDeep<TSource>(this IEnumerable<IEnumerable<TSource>> source, IEqualityComparer<TSource> equalityComparer)
            => source.Select(o => o.ToHashSet(equalityComparer)).ToHashSet();

        public static HashSet<HashSet<HashSet<TSource>>> ToHashSetDeep<TSource>(this IEnumerable<IEnumerable<IEnumerable<TSource>>> source, IEqualityComparer<TSource> equalityComparer)
            => source.Select(o => o.ToHashSetDeep(equalityComparer)).ToHashSet();

        public static SortedSet<TSource> ToSortedSet<TSource>(this IEnumerable<TSource> source)
        {
            if (null == source) throw new ArgumentNullException(nameof(source));

            return source as SortedSet<TSource> 
                ?? new SortedSet<TSource>(source);
        }

        public static SortedSet<SortedSet<TSource>> ToSortedSetDeep<TSource>(this IEnumerable<IEnumerable<TSource>> source)
            => source.Select(o => o.ToSortedSet()).ToSortedSet();

        public static SortedSet<SortedSet<SortedSet<TSource>>> ToSortedSetDeep<TSource>(this IEnumerable<IEnumerable<IEnumerable<TSource>>> source)
            => source.Select(o => o.ToSortedSetDeep()).ToSortedSet();

        public static SortedSet<TSource> ToSortedSet<TSource>(this IEnumerable<TSource> source, IComparer<TSource> comparer)
        {
            if (null == source) throw new ArgumentNullException(nameof(source));
            if (null == comparer) throw new ArgumentNullException(nameof(comparer));

            return new SortedSet<TSource>(source, comparer);
        }

        public static SortedSet<SortedSet<TSource>> ToSortedSetDeep<TSource>(this IEnumerable<IEnumerable<TSource>> source, IComparer<TSource> comparer)
            => source.Select(o => o.ToSortedSet(comparer)).ToSortedSet();

        public static SortedSet<SortedSet<SortedSet<TSource>>> ToSortedSetDeep<TSource>(this IEnumerable<IEnumerable<IEnumerable<TSource>>> source, IComparer<TSource> comparer)
            => source.Select(o => o.ToSortedSetDeep(comparer)).ToSortedSet();

        public static List<List<TSource>> ToListDeep<TSource>(this IEnumerable<IEnumerable<TSource>> source)
            => source.Select(Enumerable.ToList).ToList();

        public static List<List<List<TSource>>> ToListDeep<TSource>(this IEnumerable<IEnumerable<IEnumerable<TSource>>> source)
            => source.Select(ToListDeep).ToList();

        #endregion
    }
}