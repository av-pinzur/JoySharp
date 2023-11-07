using System.Reflection;

namespace AvP.Joy;

public static class ReflectionExtensions
{
    public static TAttribute? GetCustomAttribute<TAttribute>(this MemberInfo source, bool inherit) where TAttribute : Attribute =>
        source.GetCustomAttributes<TAttribute>().SingleOrDefault();

    public static TAttribute[] GetCustomAttributes<TAttribute>(this MemberInfo source, bool inherit) where TAttribute : Attribute =>
        (TAttribute[])source.GetCustomAttributes(typeof(TAttribute), inherit);

    public static bool SignatureEquals(this MethodInfo first, MethodInfo second)
    {
        var firstParameters = first.GetParameters();
        var secondParameters = second.GetParameters();
        return first.ReturnType == second.ReturnType
            && firstParameters.Length == secondParameters.Length
            && Enumerable.Range(0, firstParameters.Length).All(i =>
                firstParameters[i].ParameterType == secondParameters[i].ParameterType
                    && firstParameters[i].IsIn == secondParameters[i].IsIn
                    && firstParameters[i].IsOut == secondParameters[i].IsOut);
    }

    public static string ToDescriptiveString(this MethodInfo methodInfo) =>
        $"{methodInfo.DeclaringType!.Name}.{methodInfo.Name}({methodInfo.GetParameters().Select(p => p.ParameterType.Name).Join(", ")})";
}
