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
            if (fn == null) throw new ArgumentNullException("fn");
            return fn(arg);
        }

        public static TResult Let<T1, T2, TResult>(T1 arg1, T2 arg2, Func<T1, T2, TResult> fn)
        {
            if (fn == null) throw new ArgumentNullException("fn");
            return fn(arg1, arg2);
        }

        public static TResult Let<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3, Func<T1, T2, T3, TResult> fn)
        {
            if (fn == null) throw new ArgumentNullException("fn");
            return fn(arg1, arg2, arg3);
        }

        public static TResult Let<T1, T2, T3, T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, Func<T1, T2, T3, T4, TResult> fn)
        {
            if (fn == null) throw new ArgumentNullException("fn");
            return fn(arg1, arg2, arg3, arg4);
        }

        #endregion
        #region Y

        public delegate Func<T, TResult> YBody<T, TResult>(Func<T, TResult> self);
        public delegate Func<T1, T2, TResult> YBody<T1, T2, TResult>(Func<T1, T2, TResult> self);
        public delegate Func<T1, T2, T3, TResult> YBody<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> self);
        public delegate Func<T1, T2, T3, T4, TResult> YBody<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> self);

        private delegate Func<T, TResult> Recursive<T, TResult>(Recursive<T, TResult> rec);

        public static Func<T, TResult> Y<T, TResult>(YBody<T, TResult> body)
        {
            if (body == null) throw new ArgumentNullException("body");
            Recursive<T, TResult> rec = r => arg => body(r(r))(arg);
            return rec(rec);
        }

        public static Func<T1, T2, TResult> Y<T1, T2, TResult>(YBody<T1, T2, TResult> body)
        {
            if (body == null) throw new ArgumentNullException("body");
            var y = Y<Tuple<T1, T2>, TResult>(
                    self => arg => body(
                            (arg1, arg2) => self(Tuple.Create(arg1, arg2))
                        )(arg.Item1, arg.Item2)
                );
            return (arg1, arg2) => y(Tuple.Create(arg1, arg2));
        }

        public static Func<T1, T2, T3, TResult> Y<T1, T2, T3, TResult>(YBody<T1, T2, T3, TResult> body)
        {
            if (body == null) throw new ArgumentNullException("body");
            var y = Y<Tuple<T1, T2, T3>, TResult>(
                    self => arg => body(
                            (arg1, arg2, arg3) => self(Tuple.Create(arg1, arg2, arg3))
                        )(arg.Item1, arg.Item2, arg.Item3)
                );
            return (arg1, arg2, arg3) => y(Tuple.Create(arg1, arg2, arg3));
        }

        public static Func<T1, T2, T3, T4, TResult> Y<T1, T2, T3, T4, TResult>(YBody<T1, T2, T3, T4, TResult> body)
        {
            if (body == null) throw new ArgumentNullException("body");
            var y = Y<Tuple<T1, T2, T3, T4>, TResult>(
                    self => arg => body(
                            (arg1, arg2, arg3, arg4) => self(Tuple.Create(arg1, arg2, arg3, arg4))
                        )(arg.Item1, arg.Item2, arg.Item3, arg.Item4)
                );
            return (arg1, arg2, arg3, arg4) => y(Tuple.Create(arg1, arg2, arg3, arg4));
        }

        #endregion
        #region Loop

        public interface ILoopControl<out T, out TResult>
        {
            bool IsCompleting { get; }
            TResult Result { get; }
            T Arg { get; }
        }

        public interface ILoopController<T, TResult>
        {
            ILoopControl<T, TResult> Complete(TResult result);
            ILoopControl<T, TResult> Recur(T arg);
        }

        public interface ILoopController<T1, T2, TResult>
        {
            ILoopControl<Tuple<T1, T2>, TResult> Complete(TResult result);
            ILoopControl<Tuple<T1, T2>, TResult> Recur(T1 arg1, T2 arg2);
        }

        public interface ILoopController<T1, T2, T3, TResult>
        {
            ILoopControl<Tuple<T1, T2, T3>, TResult> Complete(TResult result);
            ILoopControl<Tuple<T1, T2, T3>, TResult> Recur(T1 arg1, T2 arg2, T3 arg3);
        }

        public interface ILoopController<T1, T2, T3, T4, TResult>
        {
            ILoopControl<Tuple<T1, T2, T3, T4>, TResult> Complete(TResult result);
            ILoopControl<Tuple<T1, T2, T3, T4>, TResult> Recur(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
        }

        public delegate Func<T, ILoopControl<T, TResult>> LoopBody<T, TResult>(ILoopController<T, TResult> controller);
        public delegate Func<T1, T2, ILoopControl<Tuple<T1, T2>, TResult>> LoopBody<T1, T2, TResult>(ILoopController<T1, T2, TResult> controller);
        public delegate Func<T1, T2, T3, ILoopControl<Tuple<T1, T2, T3>, TResult>> LoopBody<T1, T2, T3, TResult>(ILoopController<T1, T2, T3, TResult> controller);
        public delegate Func<T1, T2, T3, T4, ILoopControl<Tuple<T1, T2, T3, T4>, TResult>> LoopBody<T1, T2, T3, T4, TResult>(ILoopController<T1, T2, T3, T4, TResult> controller);

        public static TResult Loop<T, TResult>(T initialArg, LoopBody<T, TResult> body)
        {
            if (body == null) throw new ArgumentNullException("body");
            return LoopControl<T, TResult>.Loop(
                initialArg, 
                arg => body(new LoopController<T, TResult>())(arg) );
        }

        public static TResult Loop<T1, T2, TResult>(T1 initialArg1, T2 initialArg2, LoopBody<T1, T2, TResult> body)
        {
            if (body == null) throw new ArgumentNullException("body");
            return LoopControl<Tuple<T1, T2>, TResult>.Loop(
                Tuple.Create(initialArg1, initialArg2),
                args => body(new LoopController<T1, T2, TResult>())(args.Item1, args.Item2) );
        }

        public static TResult Loop<T1, T2, T3, TResult>(T1 initialArg1, T2 initialArg2, T3 initialArg3, LoopBody<T1, T2, T3, TResult> body)
        {
            if (body == null) throw new ArgumentNullException("body");
            return LoopControl<Tuple<T1, T2, T3>, TResult>.Loop(
                Tuple.Create(initialArg1, initialArg2, initialArg3),
                args => body(new LoopController<T1, T2, T3, TResult>())(args.Item1, args.Item2, args.Item3) );
        }

        public static TResult Loop<T1, T2, T3, T4, TResult>(T1 initialArg1, T2 initialArg2, T3 initialArg3, T4 initialArg4, LoopBody<T1, T2, T3, T4, TResult> body)
        {
            if (body == null) throw new ArgumentNullException("body");
            return LoopControl<Tuple<T1, T2, T3, T4>, TResult>.Loop(
                Tuple.Create(initialArg1, initialArg2, initialArg3, initialArg4),
                args => body(new LoopController<T1, T2, T3, T4, TResult>())(args.Item1, args.Item2, args.Item3, args.Item4) );
        }

        private sealed class LoopControl<T, TResult> : ILoopControl<T, TResult>
        {
            private readonly bool isCompleting;
            private readonly TResult result;
            private readonly T arg;

            internal static ILoopControl<T, TResult> Complete(TResult result) { return new LoopControl<T, TResult>(true, result, default(T)); }
            internal static ILoopControl<T, TResult> Recur(T arg) { return new LoopControl<T, TResult>(false, default(TResult), arg); }

            private LoopControl(bool isCompleting, TResult result, T arg)
            {
                this.isCompleting = isCompleting;
                this.result = result;
                this.arg = arg;
            }

            public bool IsCompleting { get { return isCompleting; } }
            public TResult Result { get { return result; } }
            public T Arg { get { return arg; } }

            internal static TResult Loop(T initialArg, Func<T, ILoopControl<T, TResult>> body)
            {
                var recurrence = LoopControl<T, TResult>.Recur(initialArg);
                while (!recurrence.IsCompleting)
                    recurrence = body(recurrence.Arg);
                return recurrence.Result;
            }
        }

        private struct LoopController<T, TResult> : ILoopController<T, TResult>
        {
            public ILoopControl<T, TResult> Complete(TResult result) { return LoopControl<T, TResult>.Complete(result); }
            public ILoopControl<T, TResult> Recur(T arg) { return LoopControl<T, TResult>.Recur(arg); }
        }

        private struct LoopController<T1, T2, TResult> : ILoopController<T1, T2, TResult>
        {
            public ILoopControl<Tuple<T1, T2>, TResult> Complete(TResult result) { return LoopControl<Tuple<T1, T2>, TResult>.Complete(result); }
            public ILoopControl<Tuple<T1, T2>, TResult> Recur(T1 arg1, T2 arg2) { return LoopControl<Tuple<T1, T2>, TResult>.Recur(Tuple.Create(arg1, arg2)); }
        }

        private struct LoopController<T1, T2, T3, TResult> : ILoopController<T1, T2, T3, TResult>
        {
            public ILoopControl<Tuple<T1, T2, T3>, TResult> Complete(TResult result) { return LoopControl<Tuple<T1, T2, T3>, TResult>.Complete(result); }
            public ILoopControl<Tuple<T1, T2, T3>, TResult> Recur(T1 arg1, T2 arg2, T3 arg3) { return LoopControl<Tuple<T1, T2, T3>, TResult>.Recur(Tuple.Create(arg1, arg2, arg3)); }
        }

        private struct LoopController<T1, T2, T3, T4, TResult> : ILoopController<T1, T2, T3, T4, TResult>
        {
            public ILoopControl<Tuple<T1, T2, T3, T4>, TResult> Complete(TResult result) { return LoopControl<Tuple<T1, T2, T3, T4>, TResult>.Complete(result); }
            public ILoopControl<Tuple<T1, T2, T3, T4>, TResult> Recur(T1 arg1, T2 arg2, T3 arg3, T4 arg4) { return LoopControl<Tuple<T1, T2, T3, T4>, TResult>.Recur(Tuple.Create(arg1, arg2, arg3, arg4)); }
        }

        #endregion
    }
}