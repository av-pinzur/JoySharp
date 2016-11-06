using System.Reflection;

namespace AvP.Joy.Proxies
{
    public abstract class TargetedProxyBase<TInterface> : LocalProxy<TInterface>
    {
        protected abstract TInterface Target { get; }

        protected sealed override object Invoke(MethodInfo method, object[] parameters)
        {
            return method.Invoke(Target, parameters);
        }
    }
}