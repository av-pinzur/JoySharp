﻿using System;
using System.Threading;

namespace AvP.Joy
{
    public sealed class WrappedSequence<T> : ISequence<T>
    {
        private readonly ISequence<T> source;
        private readonly Func<ISequence<T>, ISequence<T>> tailWrapper;

        public WrappedSequence(ISequence<T> source, Func<ISequence<T>, ISequence<T>> tailWrapper)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (tailWrapper == null) throw new ArgumentNullException("tailWrapper");

            var sourceAs = source as WrappedSequence<T>;
            if (sourceAs != null)
            {
                this.source = sourceAs.source;
                this.tailWrapper = tail => tailWrapper(sourceAs.tailWrapper(tail));
            }
            else
            {
                this.source = source;
                this.tailWrapper = tailWrapper;
            }
        }

        public bool Any { get { return source.Any; } }
        public T Head { get { return source.Head; } }
        public ISequence<T> GetTail() { return tailWrapper(source.GetTail()); }
    }
}