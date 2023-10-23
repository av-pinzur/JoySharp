using AvP.Joy.Proxies.Internal;
using System.Reflection;

namespace AvP.Joy.Proxies
{
    public abstract class LocalProxy<TInterface>
    {
        private readonly TInterface transparentProxy;

        static LocalProxy()
        {
            if (!typeof(TInterface).IsInterface) throw new ArgumentException("Type argument must be an interface type.", nameof(TInterface));
        }

        protected LocalProxy()
        {
            this.transparentProxy = DelegatingDispatchProxy.Create<TInterface>(this.Invoke);
        }

        #region Static Factory Methods

        public static LocalProxy<TInterface> DelegatingTo(Func<MethodInfo, object?[], object?> invocationHandler)
        {
            if (invocationHandler == null) throw new ArgumentNullException(nameof(invocationHandler));
            return new DelegatingLocalProxy(invocationHandler);
        }

        public static LocalProxy<TInterface> DelegatingSingleMethodTo(Delegate target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            var type = typeof(TInterface);
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
            var methods = type.GetMethods(bindingFlags)
                .Where(m => m.DeclaringType != typeof(object))
                .ToList();

            if (methods.Count < 1)
                throw new ArgumentException("Type argument must declare a method.", nameof(TInterface));
            if (methods.Count > 1)
                throw new ArgumentException(string.Format(
                    "Type argument must not declare more than one method. Declared methods: {0}.",
                    methods.Select(m => m.DeclaringType!.Name + '.' + m.Name).Join(", ")), nameof(TInterface));
            if (!methods[0].SignatureEquals(target.Method))
                throw new ArgumentException("Argument must have same signature as TInterface's method.", nameof(target));

            return DelegatingTo((m, args) => target.DynamicInvoke(args));
        }

        #endregion

        protected abstract object? Invoke(MethodInfo method, object?[] args);

        public TInterface GetTransparentProxy()
        {
            return this.transparentProxy;
        }

        private sealed class DelegatingLocalProxy : LocalProxy<TInterface>
        {
            private readonly Func<MethodInfo, object?[], object?> invocationHandler;

            public DelegatingLocalProxy(Func<MethodInfo, object?[], object?> invocationHandler)
            {
                this.invocationHandler = invocationHandler;
            }

            protected override object? Invoke(MethodInfo method, object?[] args)
            {
                return invocationHandler(method, args);
            }
        }
    }
}