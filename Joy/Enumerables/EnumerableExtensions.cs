﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvP.Joy
{
    public static class EnumerableExtensions
    {
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

        public static IEnumerable<IReadOnlyList<TSource>> Slide<TSource>(this IEnumerable<TSource> source, int windowSize)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (windowSize < 1) throw new ArgumentOutOfRangeException("windowSize", "Parameter value must not be less than 1.");

            IDisposable disposer;
            var sequence = source.AsSequence(out disposer);
            return sequence.Slide(windowSize).AsEnumerable(disposer).Select(Sequence.ToList);

            // Fully functional; speed okay (31 secs / 1 million).
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

        public static IEnumerable<TResult> ZipAll<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<Maybe<TFirst>, Maybe<TSecond>, TResult> resultSelector)
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

        public static IEnumerable<TResult> ZipAll<TSource, TResult>(this IEnumerable<IEnumerable<TSource>> sources, Func<IEnumerable<Maybe<TSource>>, TResult> resultSelector)
        {
            var sourceList = sources.ToList();
            var any = new bool[sourceList.Count];
            var e = new IEnumerator<TSource>[sourceList.Count];
            try
            {
                for (int i = 0; i < sourceList.Count; i++) e[i] = sourceList[i].GetEnumerator();
                while (e.Select((o, i) => new { o, i }).Aggregate(false, (acc, cur) => acc | (any[cur.i] = cur.o.MoveNext())))
                    yield return resultSelector(e.Select((_e, i) => Maybe.If(any[i], () => _e.Current)));
            }
            finally
            {
                foreach (var _e in e) if (_e != null) _e.Dispose();
            }
        }

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

        public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> source, params TSource[] items)
        {
            return source.Concat((IEnumerable<TSource>)items);
        }

        public static IEnumerable<string> ToStrings<TSource>(this IEnumerable<TSource> source)
        {
            foreach (var o in source)
            {
                yield return o == null ? null : o.ToString();
            }
        }

        public static bool None<TSource>(this IEnumerable<TSource> source)
        {
            return !source.Any();
        }

        public static bool None<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return !source.Any(predicate);
        }

        public static IEnumerable<TSource> OrEmpty<TSource>(this IEnumerable<TSource> source)
        {
            return source ?? Enumerable.Empty<TSource>();
        }
    }
}