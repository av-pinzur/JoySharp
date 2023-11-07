using AvP.Joy.Enumerables;

namespace AvP.Joy;

public static class DecimalExtensions
{
    public static decimal IntegralPart(this decimal value)
        => Math.Truncate(value);

    public static decimal FractionalPart(this decimal value)
        => value % 1M;

    public static IntegralDivisionResult<decimal> IntegralDivide(this decimal x, decimal y)
    {
        var remainder = x % y;
        var quotient = (x - remainder) / y;
        return new IntegralDivisionResult<decimal>(quotient, remainder);
    }

    public static Tuple<IEnumerable<int>, IEnumerable<int>> Digits(this decimal value, int digitBase = 10)
    {
        if (digitBase < 2) throw new ArgumentOutOfRangeException("digitBase");
        return value < 0 ? Digits(-value) : Tuple.Create(
            IntegralPartDigits(IntegralPart(value), digitBase).DefaultIfEmpty(0),
            FractionalPartDigits(FractionalPart(value), digitBase));
    }

    internal static IEnumerable<int> IntegralPartDigits(decimal integralPart, int digitBase)
    {
        if (integralPart == 0) return Enumerable.Empty<int>();

        var divisionResult = IntegralDivide(integralPart, digitBase);
        return ((int)divisionResult.Remainder).FollowedBy(
            IntegralPartDigits(divisionResult.Quotient, digitBase));
    }

    internal static IEnumerable<int> FractionalPartDigits(decimal fractionalPart, int digitBase)
    {
        if (fractionalPart == 0) return Enumerable.Empty<int>();

        var shiftedLeft = fractionalPart * digitBase;
        return ((int)shiftedLeft.IntegralPart()).FollowedBy(
            FractionalPartDigits(shiftedLeft.FractionalPart(), digitBase));
    }
}
