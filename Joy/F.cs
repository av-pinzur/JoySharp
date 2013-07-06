using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvP.Joy
{
    public static class F
    {
        #region Let

        public static TResult Let<T, TResult>(T arg, Func<T, TResult> fn)
        {
            return fn(arg);
        }

        public static TResult Let<T1, T2, TResult>(T1 arg1, T2 arg2, Func<T1, T2, TResult> fn)
        {
            return fn(arg1, arg2);
        }

        public static TResult Let<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3, Func<T1, T2, T3, TResult> fn)
        {
            return fn(arg1, arg2, arg3);
        }

        public static TResult Let<T1, T2, T3, T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, Func<T1, T2, T3, T4, TResult> fn)
        {
            return fn(arg1, arg2, arg3, arg4);
        }

        #endregion
        #region Y

        public delegate Func<TResult> YBody<TResult>(Func<TResult> self);
        public delegate Func<T, TResult> YBody<T, TResult>(Func<T, TResult> self);
        public delegate Func<T1, T2, TResult> YBody<T1, T2, TResult>(Func<T1, T2, TResult> self);
        public delegate Func<T1, T2, T3, TResult> YBody<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> self);
        public delegate Func<T1, T2, T3, T4, TResult> YBody<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> self);

        private delegate Func<T, TResult> Recursive<T, TResult>(Recursive<T, TResult> rec);

        public static Func<T, TResult> Y<T, TResult>(YBody<T, TResult> body)
        {
            Recursive<T, TResult> rec = r => arg => body(r(r))(arg);
            return rec(rec);
        }

        //public static Func<TResult> Y<TResult>(YBody<TResult> body)
        //{
        //    var y = Y<object, TResult>(
        //            self => o => body(
        //                    () => self(null)
        //                )()
        //        );
        //    return () => y(null);
        //}

        public static Func<T1, T2, TResult> Y<T1, T2, TResult>(YBody<T1, T2, TResult> body)
        {
            var y = Y<Tuple<T1, T2>, TResult>(
                    self => arg => body(
                            (arg1, arg2) => self(Tuple.Create(arg1, arg2))
                        )(arg.Item1, arg.Item2)
                );
            return (arg1, arg2) => y(Tuple.Create(arg1, arg2));
        }

        public static Func<T1, T2, T3, TResult> Y<T1, T2, T3, TResult>(YBody<T1, T2, T3, TResult> body)
        {
            var y = Y<Tuple<T1, T2, T3>, TResult>(
                    self => arg => body(
                            (arg1, arg2, arg3) => self(Tuple.Create(arg1, arg2, arg3))
                        )(arg.Item1, arg.Item2, arg.Item3)
                );
            return (arg1, arg2, arg3) => y(Tuple.Create(arg1, arg2, arg3));
        }

        public static Func<T1, T2, T3, T4, TResult> Y<T1, T2, T3, T4, TResult>(YBody<T1, T2, T3, T4, TResult> body)
        {
            var y = Y<Tuple<T1, T2, T3, T4>, TResult>(
                    self => arg => body(
                            (arg1, arg2, arg3, arg4) => self(Tuple.Create(arg1, arg2, arg3, arg4))
                        )(arg.Item1, arg.Item2, arg.Item3, arg.Item4)
                );
            return (arg1, arg2, arg3, arg4) => y(Tuple.Create(arg1, arg2, arg3, arg4));
        }

        #endregion
        #region Loop

        public delegate Recurrence<T, TResult> LoopBody<T, TResult>(
            T arg,
            Func<TResult, Recurrence<T, TResult>> complete,
            Func<T, Recurrence<T, TResult>> recur);

        public delegate Recurrence<Tuple<T1, T2>, TResult> LoopBody<T1, T2, TResult>(
            T1 arg1, T2 arg2,
            Func<TResult, Recurrence<Tuple<T1, T2>, TResult>> complete,
            Func<T1, T2, Recurrence<Tuple<T1, T2>, TResult>> recur);

        public delegate Recurrence<Tuple<T1, T2, T3>, TResult> LoopBody<T1, T2, T3, TResult>(
            T1 arg1, T2 arg2, T3 arg3,
            Func<TResult, Recurrence<Tuple<T1, T2, T3>, TResult>> complete,
            Func<T1, T2, T3, Recurrence<Tuple<T1, T2, T3>, TResult>> recur);

        public delegate Recurrence<Tuple<T1, T2, T3, T4>, TResult> LoopBody<T1, T2, T3, T4, TResult>(
            T1 arg1, T2 arg2, T3 arg3, T4 arg4,
            Func<TResult, Recurrence<Tuple<T1, T2, T3, T4>, TResult>> complete,
            Func<T1, T2, T3, T4, Recurrence<Tuple<T1, T2, T3, T4>, TResult>> recur);

        public static TResult Loop<T, TResult>(T initialArg, LoopBody<T, TResult> body)
        {
            return Recurrence<T, TResult>.Loop(
                initialArg, 
                (arg, complete, recur) => body(arg, complete, recur) );
        }

        public static TResult Loop<T1, T2, TResult>(T1 initialArg1, T2 initialArg2, LoopBody<T1, T2, TResult> body)
        {
            return Recurrence<Tuple<T1, T2>, TResult>.Loop(
                Tuple.Create(initialArg1, initialArg2),
                (arg, complete, recur) => body(
                    arg.Item1, arg.Item2, 
                    complete, 
                    (nextArg1, nextArg2) => recur(Tuple.Create(nextArg1, nextArg2))) );
        }

        public static TResult Loop<T1, T2, T3, TResult>(T1 initialArg1, T2 initialArg2, T3 initialArg3, LoopBody<T1, T2, T3, TResult> body)
        {
            return Recurrence<Tuple<T1, T2, T3>, TResult>.Loop(
                Tuple.Create(initialArg1, initialArg2, initialArg3),
                (arg, complete, recur) => body(
                    arg.Item1, arg.Item2, arg.Item3,
                    complete,
                    (nextArg1, nextArg2, nextArg3) => recur(Tuple.Create(nextArg1, nextArg2, nextArg3))));
        }

        public static TResult Loop<T1, T2, T3, T4, TResult>(T1 initialArg1, T2 initialArg2, T3 initialArg3, T4 initialArg4, LoopBody<T1, T2, T3, T4, TResult> body)
        {
            return Recurrence<Tuple<T1, T2, T3, T4>, TResult>.Loop(
                Tuple.Create(initialArg1, initialArg2, initialArg3, initialArg4),
                (arg, complete, recur) => body(
                    arg.Item1, arg.Item2, arg.Item3, arg.Item4,
                    complete,
                    (nextArg1, nextArg2, nextArg3, nextArg4) => recur(Tuple.Create(nextArg1, nextArg2, nextArg3, nextArg4))));
        }

        public sealed class Recurrence<TArg, TResult>
        {
            private readonly bool isCompleting;
            private readonly TArg nextArg;
            private readonly TResult result;

            internal static Recurrence<TArg, TResult> Complete(TResult result) { return new Recurrence<TArg, TResult>(true, default(TArg), result); }
            internal static Recurrence<TArg, TResult> Recur(TArg nextArg) { return new Recurrence<TArg, TResult>(false, nextArg, default(TResult)); }

            private Recurrence(bool isCompleting, TArg nextArg, TResult result)
            {
                this.isCompleting = isCompleting;
                this.nextArg = nextArg;
                this.result = result;
            }

            internal static TResult Loop(
                TArg initialArg,
                Func<
                    TArg,
                    Func<TResult, Recurrence<TArg, TResult>>,
                    Func<TArg, Recurrence<TArg, TResult>>,
                    Recurrence<TArg, TResult>> body)
            {
                var recurrence = Recurrence<TArg, TResult>.Recur(initialArg);
                while (!recurrence.isCompleting)
                {
                    recurrence = body(
                        recurrence.nextArg,
                        Recurrence<TArg, TResult>.Complete,
                        Recurrence<TArg, TResult>.Recur);
                }
                return recurrence.result;
            }
        }

        #endregion
    }
}