using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Reflection;
using AvP.Joy.Enumerables;

namespace AvP.Joy.Proxies
{
    public abstract class LocalProxy
    {
        private readonly IReadOnlyCollection<Type> supportedInterfaces;
        private readonly LocalRealProxy innerProxy;

        protected LocalProxy(Type firstSupportedInterface, params Type[] otherSupportedInterfaces)
            : this(firstSupportedInterface.FollowedBy(otherSupportedInterfaces)) {}

        protected LocalProxy(IEnumerable<Type> supportedInterfaces)
        {
            if (supportedInterfaces == null) throw new ArgumentNullException(nameof(supportedInterfaces));

            this.supportedInterfaces = supportedInterfaces.ToList();

            if (this.supportedInterfaces.None())
                throw new ArgumentException("Argument must not be empty.", nameof(supportedInterfaces));
            if (this.supportedInterfaces.Contains(null))
                throw new ArgumentException("Argument elements must not be null.", nameof(supportedInterfaces));
            if (this.supportedInterfaces.Any(t => !t.IsInterface))
                throw new ArgumentException("Argument elements must be interface types.", nameof(supportedInterfaces));

            this.innerProxy = new LocalRealProxy(this);
        }

        protected abstract object Invoke(MethodInfo method, object[] parameters);

        public object GetTransparentProxy()
        {
            return innerProxy.GetTransparentProxy();
        }

        private sealed class LocalRealProxy : RealProxy, IRemotingTypeInfo
        {
            private readonly LocalProxy container;

            public LocalRealProxy(LocalProxy container)
                :base(container.supportedInterfaces.First())
            {
                this.container = container;
            }

            string IRemotingTypeInfo.TypeName
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            bool IRemotingTypeInfo.CanCastTo(Type type, object o)
            {
                return container.supportedInterfaces.Any(type.IsAssignableFrom);
            }

            public override IMessage Invoke(IMessage msg)
            {
                var callMsg = (IMethodCallMessage)msg;
                var method = (MethodInfo)callMsg.MethodBase;
                var args = callMsg.Args;
                try
                {
                    var result = container.Invoke(method, args);
                    return new ReturnMessage(result, args, args.Length, null, callMsg);
                }
                catch (Exception e)
                {
                    return new ReturnMessage(e, callMsg);
                }
            }
        }
    }
}