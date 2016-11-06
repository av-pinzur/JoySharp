using System;

namespace AvP.Joy.Sequences
{
    public static class SequenceObjectExtensions
    {
        public static ISequence<TValue> InSingletonSeq<TValue>(this TValue value)
        {
            return Sequence.Singleton<TValue>(value);
        }

        public static ISequence<TValue> InSingletonSeqIf<TValue>(this TValue value, bool predicate)
        {
            return predicate ? value.InSingletonSeq() : Sequence.Empty<TValue>();
        }

        public static ISequence<TValue> InSingletonSeqIf<TValue>(this TValue value, Func<TValue, bool> predicate)
        {
            return value.InSingletonSeqIf(predicate(value));
        }

        public static ISequence<TValue> InSingletonSeqOrEmpty<TValue>(this TValue value) where TValue : class
        {
            return value.InSingletonSeqIf(value != null);
        }

        public static ISequence<TValue> InSingletonSeqOrEmpty<TValue>(this TValue? nullableValue) where TValue : struct
        {
            return (nullableValue ?? default(TValue)).InSingletonSeqIf(nullableValue.HasValue);
        }

        public static ISequence<TValue> FollowedBySeq<TValue>(this TValue head, ISequence<TValue> tail)
        {
            return new LinkedSequence<TValue>(head, tail);
        }
    }
}