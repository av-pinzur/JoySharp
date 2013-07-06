using System;

namespace AvP.Joy
{
    public struct LinkedSequence<T> : ISequence<T>
    {
        private readonly bool any;
        private readonly T head;
        private readonly ISequence<T> tail;

        // Note: this struct has an implicit parameterless constructor as follows:
        //public LinkedSequence()
        //{
        //    this.any = false;
        //    this.head = default(T);
        //    this.tail = null;
        //}

        public LinkedSequence(T head, ISequence<T> tail)
        {
            this.any = true;
            this.head = head;
            this.tail = tail;
        }

        public bool Any { get { return any; } }
        public T Head { get { if (!any) throw new InvalidOperationException("Enumeration has completed."); return head; } }
        public ISequence<T> GetTail() { return tail ?? Sequence.Empty<T>(); }
    }
}