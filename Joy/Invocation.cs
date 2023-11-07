using AvP.Joy.Enumerables;
using System.Reflection;

namespace AvP.Joy;

public record Invocation(MethodInfo Method, object?[] Arguments)
{
    public object? InvokeOn(object target) =>
        Method.Invoke(target, Arguments);

    public override string ToString() =>
        $"Invocation {{ Method = {Method.ToDescriptiveString()}, Arguments = {Arguments.ToDescriptiveString()} }}";
}
