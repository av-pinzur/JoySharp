using System;

namespace AvP.Joy
{
    public struct IntegralDivisionResult<T> : IEquatable<IntegralDivisionResult<T>>
    {
        private readonly T quotient;
        private readonly T remainder;

        public IntegralDivisionResult(T quotient, T remainder)
        {
            this.quotient = quotient;
            this.remainder = remainder;
        }

        public T Quotient => quotient;
        public T Remainder => remainder;

        public bool Equals(IntegralDivisionResult<T> other)
            => Equals(quotient, other.quotient) && Equals(remainder, other.remainder);

        public override bool Equals(object? obj)
            => obj is IntegralDivisionResult<T> && Equals((IntegralDivisionResult<T>) obj);

        public override int GetHashCode()
            => (quotient == null ? 0 : quotient.GetHashCode())
                ^ (remainder == null ? 0 : remainder.GetHashCode());

        public static bool operator ==(IntegralDivisionResult<T> x, IntegralDivisionResult<T> y)
            => Equals(x, y);

        public static bool operator !=(IntegralDivisionResult<T> x, IntegralDivisionResult<T> y)
            => !(x == y);
    }
}
