using System;

namespace AvP.Joy
{
    public sealed class Indexed<T> : IEquatable<Indexed<T>>
    {
        private readonly int index;
        private readonly T value;

        public Indexed(int index, T value)
        {
            this.index = index;
            this.value = value;
        }

        public int Index { get { return index; } }
        public T Value { get { return value; } }

        public bool Equals(Indexed<T> other)
        {
            return ReferenceEquals(this, other)
                || (other != null
                    && Equals(index, other.index)
                    && Equals(value, other.value));
        }

        public override bool Equals(object obj)
        {
            var objAs = obj as Indexed<T>;
            return objAs != null & Equals(objAs);
        }

        public override int GetHashCode()
        {
            return index.GetHashCode()
                ^ (value == null ? 0 : value.GetHashCode());
        }

        public override string ToString()
        {
            return string.Format("({0}: [{1}])", index, value);
        }
    }
}