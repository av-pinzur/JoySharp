using System;

namespace AvP.Joy.Sequences
{
    internal sealed class ConcatSequence<T> : ISequence<T>
    {
        private readonly ISequence<T> first;
        private readonly ISequence<T>[] others;
        private readonly int offset;

        private ConcatSequence(ISequence<T> first, ISequence<T>[] others, int offset)
        {
            this.first = first;
            this.others = others;
            this.offset = offset;
        }

        public static ISequence<T> Create(ISequence<T> first, ISequence<T> second)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));

            if (second.None()) return first;
            if (first.None()) return second;

            var firstAs = first as ConcatSequence<T>;
            var secondAs = second as ConcatSequence<T>;
            if (firstAs == null && secondAs == null) return new ConcatSequence<T>(first, new[] { second }, 0);

            var firstFirst = firstAs == null ? first : firstAs.first;
            var firstOthers = firstAs == null ? new ISequence<T>[0] : firstAs.others;
            var firstOffset = firstAs == null ? 0 : firstAs.offset;

            var secondFirst = secondAs == null ? second : secondAs.first;
            var secondOthers = secondAs == null ? new ISequence<T>[0] : secondAs.others;
            var secondOffset = secondAs == null ? 0 : secondAs.offset;

            var others = (new[] { 
                Tuple.Create(firstOthers, firstOffset),
                Tuple.Create(new[] { secondFirst }, 0),
                Tuple.Create(secondOthers, secondOffset) }).Concat();
            return new ConcatSequence<T>(firstFirst, others, 0);
        }

        public bool Any { get { return true; } }
        public T Head { get { return first.Head; } }

        public ISequence<T> GetTail()
        {
            var firstTail = first.GetTail();
            return firstTail.Any ? new ConcatSequence<T>(firstTail, others, offset)
                : offset < others.Length - 1 ? new ConcatSequence<T>(others[offset], others, offset + 1)
                : others[offset];
        }
    }
}