using System;

namespace AvP.Joy.Adts
{
    internal sealed class Case1<T1, T2, T3> : Wrapper<T1>, Union<T1, T2, T3> 
    {
        public Case1(T1 value) : base(value) {}

        public TResult Match<TResult>(Func<T1, TResult> function1, Func<T2, TResult> function2, Func<T3, TResult> function3) =>
            function1(Value);
    }
}