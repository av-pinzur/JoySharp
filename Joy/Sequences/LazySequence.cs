using System;
using System.Threading;

namespace AvP.Joy.Sequences
{
    public sealed class LazySequence<T> : ISequence<T>
    {
        private readonly T head;
        private readonly Lazy<ISequence<T>> tail;

        public LazySequence(T head, Func<ISequence<T>> tailGetter)
        {
            if (tailGetter == null) throw new ArgumentNullException("tailGetter");

            this.head = head;
            this.tail = new Lazy<ISequence<T>>(tailGetter);
        }

        public bool Any { get { return true; } }
        public T Head { get { return head; } }
        public ISequence<T> GetTail() { return tail.Value ?? Sequence.Empty<T>(); }
    }
}