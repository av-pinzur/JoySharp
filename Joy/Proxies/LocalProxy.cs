using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Reflection;

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
            if (supportedInterfaces == null) throw new ArgumentNullException("supportedInterfaces");

            this.supportedInterfaces = supportedInterfaces.ToList();

            if (this.supportedInterfaces.None())
                throw new ArgumentException("Argument must not be empty.", "supportedInterfaces");
            if (this.supportedInterfaces.Contains(null))
                throw new ArgumentException("Argument elements must not be null.", "supportedInterfaces");
            if (!this.supportedInterfaces.All(CanProxy))
                throw new ArgumentException("Argument elements must be interface types.", "supportedInterfaces");

            this.innerProxy = new LocalRealProxy(this);
        }

        public static bool CanProxy(Type type)
        {
            return type.IsInterface;  // || typeof(MarshalByRefObject).IsAssignableFrom(type);
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
                return container.supportedInterfaces.Any(t => type.IsAssignableFrom(t));
            }

            public sealed override IMessage Invoke(IMessage msg)
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