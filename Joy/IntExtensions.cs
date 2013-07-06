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
    }
}
