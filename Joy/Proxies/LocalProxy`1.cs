﻿using System;
using System.Linq;
using System.Reflection;
using AvP.Joy.Enumerables;

namespace AvP.Joy.Proxies
{
    public abstract class LocalProxy<TInterface> : LocalProxy
    {
        static LocalProxy()
        {
            if (!typeof(TInterface).IsInterface) throw new ArgumentException("Type argument must be an interface type.", "TInterface");
        }

        public LocalProxy() : base(typeof(TInterface)) {}

        public static LocalProxy<TInterface> DelegatingTo(Func<MethodInfo, object[], object> invocationHandler)
        {
            if (invocationHandler == null) throw new ArgumentNullException("invocationHandler");
            return new DelegatingLocalProxy(invocationHandler);
        }

        public static LocalProxy<TInterface> DelegatingSingleMethodTo(Delegate target)
        {
            if (target == null) throw new ArgumentNullException("target");
            
            var type = typeof(TInterface);
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
            var methods = type.GetMethods(bindingFlags)
                .Where(m => m.DeclaringType != typeof(object))
                .ToList();

            if (methods.Count < 1) 
                throw new ArgumentException("Type argument must declare a method.", "TInterface");
            if (methods.Count > 1)
                throw new ArgumentException(string.Format(
                    "Type argument must not declare more than method. Declared methods: {0}.", 
                    methods.Select(m => m.DeclaringType.Name + '.' + m.Name).Join(", ")), "TInterface");
            if (!methods[0].SignatureEquals(target.Method))
                throw new ArgumentException("Argument must have same signature as TInterface's method.", "target");
            
            return DelegatingTo((m, args) => target.DynamicInvoke(args));
        }

        public new TInterface GetTransparentProxy()
        {
            return (TInterface)base.GetTransparentProxy();
        }

        private sealed class DelegatingLocalProxy : LocalProxy<TInterface>
        {
            private readonly Func<MethodInfo, object[], object> invocationHandler;

            public DelegatingLocalProxy(Func<MethodInfo, object[], object> invocationHandler)
            {
                this.invocationHandler = invocationHandler;
            }

            protected override object Invoke(MethodInfo method, object[] parameters)
            {
                return invocationHandler(method, parameters);
            }
        }
    }
}