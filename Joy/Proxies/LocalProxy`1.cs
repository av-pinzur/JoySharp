using System;
using System.Linq;
using System.Reflection;

namespace AvP.Joy.Proxies
{
    public abstract class LocalProxy<TInterface> : LocalProxy
    {
        static LocalProxy()
        {
            if (!typeof(TInterface).IsInterface) throw new ArgumentException("Type argument must be an interface type.", "TInterface");
        }

        public LocalProxy() : base(typeof(TInterface)) {}

        public static LocalProxy<TInterface> DelegatingTo(Func<MethodInfo, object[], object> invoker)
        {
            return new DelegatingLocalProxy(invoker);
        }

        public static LocalProxy<TInterface> DelegatingSingleMethodTo(Delegate implementation)
        {
            if (implementation == null) throw new ArgumentNullException("implementation");
            
            var type = typeof(TInterface);
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
            var methods = type.GetMethods(bindingFlags)
                .Concat(type.GetProperties(bindingFlags).SelectMany(p => p.GetAccessors()))
                .Where(m => m.DeclaringType != typeof(object))
                .ToList();

            if (methods.Count < 1) 
                throw new ArgumentException("Type argument must declare a method.", "TInterface");
            if (methods.Count > 1)
                throw new ArgumentException("Type argument must not declare more than method.", "TInterface");
            if (!methods[0].SignatureEquals(implementation.Method))
                throw new ArgumentException("Argument must have same signature as TInterface's method.", "implementation");
            
            return DelegatingTo((m, args) => implementation.DynamicInvoke(args));
        }

        public new TInterface GetTransparentProxy()
        {
            return (TInterface)base.GetTransparentProxy();
        }

        private sealed class DelegatingLocalProxy : LocalProxy<TInterface>
        {
            private readonly Func<MethodInfo, object[], object> invoker;

            public DelegatingLocalProxy(Func<MethodInfo, object[], object> invoker)
            {
                if (invoker == null) throw new ArgumentNullException("invoker");
                this.invoker = invoker;
            }

            protected override object Invoke(MethodInfo method, object[] parameters)
            {
                return invoker(method, parameters);
            }
        }
    }
}