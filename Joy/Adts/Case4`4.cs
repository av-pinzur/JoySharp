using System;

namespace AvP.Joy.Adts
{
    internal sealed class Case4<T1, T2, T3, T4> : Wrapper<T4>, Union<T1, T2, T3, T4> 
    {
        public Case4(T4 value) : base(value) {}

        public TResult Match<TResult>(Func<T1, TResult> function1, Func<T2, TResult> function2, Func<T3, TResult> function3, Func<T4, TResult> function4) =>
            function4(Value);
    }
}