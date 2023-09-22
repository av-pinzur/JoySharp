namespace AvP.Joy
{
    public sealed class Counted<T> : IEquatable<Counted<T>>
    {
        private readonly T value;
        private readonly int count;

        public Counted(T value, int count)
        {
            this.value = value;
            this.count = count;
        }

        public T Value { get { return value; } }
        public int Count { get { return count; } }

        public bool Equals(Counted<T>? other)
        {
            return ReferenceEquals(this, other)
                || (other != null
                    && Equals(value, other.value)
                    && Equals(count, other.count));
        }

        public override bool Equals(object? obj)
        {
            var objAs = obj as Counted<T>;
            return objAs != null & Equals(objAs);
        }

        public override int GetHashCode()
        {
            return unchecked((value == null ? 0 : value.GetHashCode())
                * 397 ^ count.GetHashCode());
        }

        public override string ToString()
        {
            return string.Format("([{0}]: {1})", value, count);
        }
    }
}