using System.Collections.Generic;
using System.Linq;
using AvP.Joy.Enumerables;

namespace AvP.Joy
{
    public static class IntExtensions
    {
        public static bool IsOdd(this int value) { return value % 2 == 1; }
        public static bool IsEven(this int value) { return !value.IsOdd(); }
        public static bool IsOdd(this uint value) { return value % 2 == 1; }
        public static bool IsEven(this uint value) { return !value.IsOdd(); }
        public static bool IsOdd(this short value) { return value % 2 == 1; }
        public static bool IsEven(this short value) { return !value.IsOdd(); }
        public static bool IsOdd(this ushort value) { return value % 2 == 1; }
        public static bool IsEven(this ushort value) { return !value.IsOdd(); }
        public static bool IsOdd(this long value) { return value % 2 == 1; }
        public static bool IsEven(this long value) { return !value.IsOdd(); }
        public static bool IsOdd(this ulong value) { return value % 2 == 1; }
        public static bool IsEven(this ulong value) { return !value.IsOdd(); }
        public static bool IsOdd(this sbyte value) { return value % 2 == 1; }
        public static bool IsEven(this sbyte value) { return !value.IsOdd(); }
        public static bool IsOdd(this byte value) { return value % 2 == 1; }
        public static bool IsEven(this byte value) { return !value.IsOdd(); }

        public static bool DividesBy(this int value, int divisor) { return value % divisor == 0; }
        public static bool DividesBy(this uint value, uint divisor) { return value % divisor == 0; }
        public static bool DividesBy(this short value, short divisor) { return value % divisor == 0; }
        public static bool DividesBy(this ushort value, ushort divisor) { return value % divisor == 0; }
        public static bool DividesBy(this long value, long divisor) { return value % divisor == 0; }
        public static bool DividesBy(this ulong value, ulong divisor) { return value % divisor == 0; }
        public static bool DividesBy(this sbyte value, sbyte divisor) { return value % divisor == 0; }
        public static bool DividesBy(this byte value, byte divisor) { return value % divisor == 0; }

        public static int DigitSum(this int value, int digitBase = 10) { return value.Digits(digitBase).Sum(); }

        public static IEnumerable<int> Digits(this int value, int digitBase = 10)
        {
            return value < 0 ? Digits(-value) 
                : value < digitBase ? value.InSingleton() 
                : (value % digitBase).FollowedBy(Digits(value / digitBase));
        }
    }
}