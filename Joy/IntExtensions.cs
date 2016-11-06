using System.Collections.Generic;
using System.Linq;
using AvP.Joy.Enumerables;

namespace AvP.Joy
{
    public static class IntExtensions
    {
        public static bool IsOdd(this int value) => value % 2 == 1;
        public static bool IsEven(this int value) => !value.IsOdd();
        public static bool IsOdd(this uint value) => value % 2 == 1;
        public static bool IsEven(this uint value) => !value.IsOdd();
        public static bool IsOdd(this short value) => value % 2 == 1;
        public static bool IsEven(this short value) => !value.IsOdd();
        public static bool IsOdd(this ushort value) => value % 2 == 1;
        public static bool IsEven(this ushort value) => !value.IsOdd();
        public static bool IsOdd(this long value) => value % 2 == 1;
        public static bool IsEven(this long value) => !value.IsOdd();
        public static bool IsOdd(this ulong value) => value % 2 == 1;
        public static bool IsEven(this ulong value) => !value.IsOdd();
        public static bool IsOdd(this sbyte value) => value % 2 == 1;
        public static bool IsEven(this sbyte value) => !value.IsOdd();
        public static bool IsOdd(this byte value) => value % 2 == 1;
        public static bool IsEven(this byte value) => !value.IsOdd();

        public static bool DividesBy(this int value, int divisor) => value % divisor == 0;
        public static bool DividesBy(this uint value, uint divisor) => value % divisor == 0;
        public static bool DividesBy(this short value, short divisor) => value % divisor == 0;
        public static bool DividesBy(this ushort value, ushort divisor) => value % divisor == 0;
        public static bool DividesBy(this long value, long divisor) => value % divisor == 0;
        public static bool DividesBy(this ulong value, ulong divisor) => value % divisor == 0;
        public static bool DividesBy(this sbyte value, sbyte divisor) => value % divisor == 0;
        public static bool DividesBy(this byte value, byte divisor) => value % divisor == 0;

        public static int DigitSum(this int value, int digitBase = 10) => value.Digits(digitBase).Sum();
        public static uint DigitSum(this uint value, uint digitBase = 10) => value.Digits(digitBase).Sum();
        public static long DigitSum(this long value, long digitBase = 10) => value.Digits(digitBase).Sum();
        public static ulong DigitSum(this ulong value, ulong digitBase = 10) => value.Digits(digitBase).Sum();

        public static IEnumerable<int> Digits(this int value, int digitBase = 10)
            => value < 0 ? Digits(-value)
                : value < digitBase ? value.InSingleton()
                : (value % digitBase).FollowedBy(Digits(value / digitBase));

        public static IEnumerable<uint> Digits(this uint value, uint digitBase = 10)
            => value < digitBase ? value.InSingleton()
                : (value % digitBase).FollowedBy(Digits(value / digitBase));

        public static IEnumerable<long> Digits(this long value, long digitBase = 10)
            => value < 0 ? Digits(-value)
                : value < digitBase ? value.InSingleton()
                : (value % digitBase).FollowedBy(Digits(value / digitBase));

        public static IEnumerable<ulong> Digits(this ulong value, ulong digitBase = 10)
            => value < digitBase ? value.InSingleton()
                : (value % digitBase).FollowedBy(Digits(value / digitBase));
    }
}