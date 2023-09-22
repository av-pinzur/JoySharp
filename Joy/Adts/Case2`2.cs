using System;

namespace AvP.Joy.Adts
{
    internal sealed class Case2<T1, T2> : Wrapper<T2>, Union<T1, T2> 
    {
        public Case2(T2 value) : base(value) {}

        public TResult Match<TResult>(Func<T1, TResult> function1, Func<T2, TResult> function2) =>
            function2(Value);
    }
}