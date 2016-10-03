using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AvP.Joy.Enumerables;

namespace AvP.Joy
{
    public class ValueList<T> : IReadOnlyList<T>, IEquatable<IEnumerable<T>>
    {
        private readonly IReadOnlyList<T> innerList;

        public ValueList(IReadOnlyList<T> innerList)
        {
            this.innerList = innerList;
        }

        public int Count { get { return innerList.Count; } }
        public T this[int index] { get { return innerList[index]; } }
        public IEnumerator<T> GetEnumerator() => innerList.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) innerList).GetEnumerator();

        public override int GetHashCode()
            => innerList.GetHashCodeByElementsOrdered();

        public bool Equals(IEnumerable<T> other)
            => innerList.EqualsByElementsOrdered(other);

        public override bool Equals(object obj)
        {
            var objAs = obj as IEnumerable<T>;
            return objAs != null && Equals(objAs);
        }

        public override string ToString()
            => "[ " + this.ToStrings().Join(", ") + " ]";
    }

    public static class ListExtensions
    {
        public static ValueList<T> AsValueList<T>(this IReadOnlyList<T> source)
        {
            if (null == source) throw new ArgumentNullException(nameof(source));

            return source as ValueList<T> 
                ?? new ValueList<T>(source);
        }

        public static ValueList<ValueList<T>> AsValueListDeep<T>(this IReadOnlyList<IReadOnlyList<T>> source)
            => source.Select(AsValueList).ToList().AsValueList();

        public static ValueList<ValueList<ValueList<T>>> AsValueListDeep<T>(this IReadOnlyList<IReadOnlyList<IReadOnlyList<T>>> source)
            => source.Select(AsValueListDeep).ToList().AsValueList();
    }
}