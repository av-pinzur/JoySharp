using AvP.Joy.Enumerables;
using System.Collections;

namespace AvP.Joy
{
    public class ValueSet<T> : IReadOnlyCollection<T>, IEquatable<IEnumerable<T>>
    {
        private readonly ISet<T> innerSet;

        public ValueSet(IReadOnlyCollection<T> innerSet)
        {
            this.innerSet = innerSet.ToSet();
        }

        public int Count { get { return innerSet.Count; } }
        public bool Contains(T item) => innerSet.Contains(item);
        public IEnumerator<T> GetEnumerator() => innerSet.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)innerSet).GetEnumerator();

        public override int GetHashCode()
            => innerSet.GetHashCodeByElementsUnordered();

        public bool Equals(IEnumerable<T>? other)
            => other != null && innerSet.SetEquals(other);

        public override bool Equals(object? obj)
        {
            var objAs = obj as IEnumerable<T>;
            return objAs != null && Equals(objAs);
        }

        public override string ToString()
            => "{ " + this.ToStrings().Join(", ") + " }";
    }

    public static class CollectionExtensions
    {
        public static ValueSet<T> AsValueSet<T>(this IReadOnlyCollection<T> source)
        {
            if (null == source) throw new ArgumentNullException(nameof(source));

            return source as ValueSet<T>
                ?? new ValueSet<T>(source);
        }

        public static ValueSet<ValueSet<T>> AsValueSetDeep<T>(this IReadOnlyCollection<IReadOnlyCollection<T>> source)
            => source.Select(AsValueSet).ToHashSet().AsValueSet();

        public static ValueSet<ValueSet<ValueSet<T>>> AsValueSetDeep<T>(this IReadOnlyCollection<IReadOnlyCollection<IReadOnlyCollection<T>>> source)
            => source.Select(AsValueSetDeep).ToHashSet().AsValueSet();
    }
}