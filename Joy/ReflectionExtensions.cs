using System;
using System.Linq;
using System.Reflection;

namespace AvP.Joy
{
    public static class ReflectionExtensions
    {
        public static TAttribute GetCustomAttribute<TAttribute>(this MemberInfo source, bool inherit) where TAttribute : Attribute
        {
            if (source == null) throw new ArgumentNullException("source");
            return source.GetCustomAttributes<TAttribute>().SingleOrDefault();
        }

        public static TAttribute[] GetCustomAttributes<TAttribute>(this MemberInfo source, bool inherit) where TAttribute : Attribute
        {
            if (source == null) throw new ArgumentNullException("source");
            return (TAttribute[]) source.GetCustomAttributes(typeof(TAttribute), inherit);
        }
    }
}