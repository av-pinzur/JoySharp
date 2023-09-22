using System.Diagnostics.CodeAnalysis;

namespace AvP.Joy
{
    // TODO: Make sure this is handling null correctly.
    public struct Maybe<T> : IEquatable<Maybe<T>>
    {
        private readonly bool hasValue;
        private readonly T? value;

        public static Maybe<T> Some(T? value)
            => new Maybe<T>(true, value);

        public static Maybe<T> None
        {
            get
            {
                return new Maybe<T>(false, default);
            }
        }


        public static Maybe<T> If(bool condition, Func<T> valueGetter)
            => condition ? Some(valueGetter()) : None;

        public static Maybe<T> IfNonNull(T value)
            => value != null ? Some(value) : None;

        /// <remarks>Remember - implicit conversion operator isn't available when the type of <param name="value"/> is an interface. Use <see cref="Maybe`0.Some"/> instead.</remarks>
        public static implicit operator Maybe<T>(T value) { return Some(value); }

        // Note: this struct has an implicit parameterless constructor as follows:
        //public Maybe()
        //{
        //    this.hasValue = false;
        //    this.value = default(T);
        //}

        private Maybe(bool hasValue, T? value)
        {
            this.hasValue = hasValue;
            this.value = value;
        }

        public readonly bool HasValue => hasValue;
        public readonly T Value => hasValue ? value! : throw new InvalidOperationException("The current Maybe has no value.");
        [return: NotNullIfNotNull(nameof(defaultValue))] public readonly T? ValueOrDefault(T? defaultValue) => hasValue ? value! : defaultValue;

        public bool Equals(Maybe<T> other)
        {
            return hasValue.Equals(other.hasValue)
                && Equals(value, other.value);
        }

        public override bool Equals(object? obj)
        {
            return obj is Maybe<T> && Equals((Maybe<T>)obj);
        }

        public override int GetHashCode()
        {
            return unchecked(hasValue.GetHashCode()
                * 397 ^ value.GetHashCodeNullable());
        }

        public override string ToString()
        {
            return hasValue ? value?.ToString() ?? string.Empty : "{none}";
        }
    }
}