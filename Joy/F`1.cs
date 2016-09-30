namespace AvP.Joy
{
    public static class F<TResult>
    {
        #region YEval

        public static TResult YEval<T>(T initialArg, F.YBody<T, TResult> body)
        {
            return F.Y(body)(initialArg);
        }

        public static TResult YEval<T1, T2>(T1 initialArg1, T2 initialArg2, F.YBody<T1, T2, TResult> body)
        {
            return F.Y(body)(initialArg1, initialArg2);
        }

        public static TResult YEval<T1, T2, T3>(T1 initialArg1, T2 initialArg2, T3 initialArg3, F.YBody<T1, T2, T3, TResult> body)
        {
            return F.Y(body)(initialArg1, initialArg2, initialArg3);
        }

        public static TResult YEval<T1, T2, T3, T4>(T1 initialArg1, T2 initialArg2, T3 initialArg3, T4 initialArg4, F.YBody<T1, T2, T3, T4, TResult> body)
        {
            return F.Y(body)(initialArg1, initialArg2, initialArg3, initialArg4);
        }

        #endregion
        #region Loop

        public static TResult Loop<T>(T initialArg, F.LoopBody<T, TResult> body)
        {
            return F.Loop(initialArg, body);
        }

        public static TResult Loop<T1, T2>(T1 initialArg1, T2 initialArg2, F.LoopBody<T1, T2, TResult> body)
        {
            return F.Loop(initialArg1, initialArg2, body);
        }

        public static TResult Loop<T1, T2, T3>(T1 initialArg1, T2 initialArg2, T3 initialArg3, F.LoopBody<T1, T2, T3, TResult> body)
        {
            return F.Loop(initialArg1, initialArg2, initialArg3, body);
        }

        public static TResult Loop<T1, T2, T3, T4>(T1 initialArg1, T2 initialArg2, T3 initialArg3, T4 initialArg4, F.LoopBody<T1, T2, T3, T4, TResult> body)
        {
            return F.Loop(initialArg1, initialArg2, initialArg3, initialArg4, body);
        }

        #endregion
        #region Id

        public static T Id<T>(T value)
            => value;

        #endregion
    }
}