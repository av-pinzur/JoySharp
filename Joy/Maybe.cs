using System;

namespace AvP.Joy
{
    public static class Maybe
    {
        public static Maybe<T> Some<T>(T value) { return Maybe<T>.Some(value); }
        public static Maybe<T> If<T>(bool condition, Func<T> valueGetter) { return Maybe<T>.If(condition, valueGetter); }
    }
}