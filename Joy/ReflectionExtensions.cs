using System;
using System.Linq;
using System.Reflection;

namespace AvP.Joy
{
    public static class ReflectionExtensions
    {
        public static TAttribute GetCustomAttribute<TAttribute>(this MemberInfo source, bool inherit) where TAttribute : Attribute
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return source.GetCustomAttributes<TAttribute>().SingleOrDefault();
        }

        public static TAttribute[] GetCustomAttributes<TAttribute>(this MemberInfo source, bool inherit) where TAttribute : Attribute
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return (TAttribute[]) source.GetCustomAttributes(typeof(TAttribute), inherit);
        }

        public static bool SignatureEquals(this MethodInfo first, MethodInfo second)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            var firstParameters = first.GetParameters();
            var secondParameters = second.GetParameters();
            return first.ReturnType == second.ReturnType
                && firstParameters.Length == secondParameters.Length
                && Enumerable.Range(0, firstParameters.Length).All(i =>
                    firstParameters[i].ParameterType == secondParameters[i].ParameterType
                        && firstParameters[i].IsIn == secondParameters[i].IsIn
                        && firstParameters[i].IsOut == secondParameters[i].IsOut );
        }
    }
}