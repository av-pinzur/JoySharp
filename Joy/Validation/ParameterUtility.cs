using System;
using System.Collections.Generic;

namespace AvP.Joy.Validation
{
    public static class ParameterUtility
    {
        public static IParam<T> DisallowsNull<T>(this T paramValue, string paramName)
        {
            return new Param<T>(paramName, paramValue).AndDisallowsNull();
        }

        public static TParam AndDisallowsNull<TParam>(this TParam param) where TParam : IParam
        {
            if (param.Value == null) throw new ArgumentNullException(param.Name);
            return param;
        }

        public static IParam<T> MustNotBeLessThan<T, TLimit>(this T paramValue, string paramName, TLimit limit) where TLimit : IComparable<T>
        {
            return new Param<T>(paramName, paramValue).AndMustNotBeLessThan(limit);
        }

        public static IParam<T> AndMustNotBeLessThan<T, TLimit>(this IParam<T> param, TLimit limit) where TLimit : IComparable<T>
        {
            if (0 < limit.CompareTo(param.Value)) throw new ArgumentOutOfRangeException(param.Name, param.Value, string.Format("Value must not be less than {0}.", limit));
            return param;
        }

        public static IParam<T> MustNotBeGreaterThan<T, TLimit>(this T paramValue, string paramName, TLimit limit) where TLimit : IComparable<T>
        {
            return new Param<T>(paramName, paramValue).AndMustNotBeGreaterThan(limit);
        }

        public static IParam<T> AndMustNotBeGreaterThan<T, TLimit>(this IParam<T> param, TLimit limit) where TLimit : IComparable<T>
        {
            if (0 > limit.CompareTo(param.Value)) throw new ArgumentOutOfRangeException(param.Name, param.Value, string.Format("Value must not be greater than {0}.", limit));
            return param;
        }

        public static IParam<T> MustBeLessThan<T, TLimit>(this T paramValue, string paramName, TLimit limit) where TLimit : IComparable<T>
        {
            return new Param<T>(paramName, paramValue).AndMustBeLessThan(limit);
        }

        public static IParam<T> AndMustBeLessThan<T, TLimit>(this IParam<T> param, TLimit limit) where TLimit : IComparable<T>
        {
            if (0 >= limit.CompareTo(param.Value)) throw new ArgumentOutOfRangeException(param.Name, param.Value, string.Format("Value must be less than {0}.", limit));
            return param;
        }

        public static IParam<T> MustBeGreaterThan<T, TLimit>(this T paramValue, string paramName, TLimit limit) where TLimit : IComparable<T>
        {
            return new Param<T>(paramName, paramValue).AndMustBeGreaterThan(limit);
        }

        public static IParam<T> AndMustBeGreaterThan<T, TLimit>(this IParam<T> param, TLimit limit) where TLimit : IComparable<T>
        {
            if (0 <= limit.CompareTo(param.Value)) throw new ArgumentOutOfRangeException(param.Name, param.Value, string.Format("Value must be greater than {0}.", limit));
            return param;
        }

        public static IEnumerableParam<TElement> DisallowingNull<TElement>(this IEnumerable<TElement> paramValue, string paramName)
        {
            return new EnumerableParam<TElement>(paramName, paramValue).AndDisallowsNull();
        }

        public static IEnumerableParam<TElement> AndDisallowingNull<TElement>(this IEnumerableParam<TElement> param)
        {
            return param.AndDisallowsNull();
        }

        public static IEnumerableParam<TElement> CheckingElements<TElement>(this IEnumerable<TElement> paramValue, string paramName, Action<IParam<TElement>> elementChecker)
        {
            return new EnumerableParam<TElement>(paramName, paramValue).AndCheckingElements(elementChecker);
        }

        public static IEnumerableParam<TElement> AndCheckingElements<TElement>(this IEnumerableParam<TElement> param, Action<IParam<TElement>> elementChecker)
        {
            return new EnumerableParam<TElement>(param.Name, Intercept(param.Value, element => elementChecker(new ParamElement<TElement>(param.Name, element))));
        }

        private static IEnumerable<TElement> Intercept<TElement>(IEnumerable<TElement> enumerable, Action<TElement> action)
        {
            foreach (var element in enumerable)
            {
                action(element);
                yield return element;
            }
        }

        public interface IParam
        {
            string Name { get; }
            object Value { get; }
        }

        public interface IParam<out T> : IParam
        {
            new T Value { get; }
        }

        public interface IEnumerableParam<out TElement> : IParam<IEnumerable<TElement>>, IEnumerable<TElement> {}

        private class Param<T> : IParam<T>
        {
            private readonly string name;
            private readonly T value;

            public Param(string name, T value)
            {
                this.name = name;
                this.value = value;
            }

            public string Name { get { return name; } }
            object IParam.Value { get { return Value; } }
            public T Value { get { return value; } }
        }

        private class EnumerableParam<TElement> : Param<IEnumerable<TElement>>, IEnumerableParam<TElement>
        {
            public EnumerableParam(string name, IEnumerable<TElement> value) : base(name, value) { }
            public IEnumerator<TElement> GetEnumerator() { return Value.GetEnumerator(); }
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
        }

        private class ParamElement<TElement> : Param<TElement>
        {
            public ParamElement(string name, TElement value) : base(name, value) { }
        }
    }
}