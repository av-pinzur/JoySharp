using System.Reflection;

namespace AvP.Joy.Proxies.Internal
{
    public class DelegatingDispatchProxy : DispatchProxy
    {
        private static readonly object initLock = new object();
        private static Func<MethodInfo, object[], object?>? initDelegate = null;

        private readonly Func<MethodInfo, object[], object?> @delegate;

        public DelegatingDispatchProxy()
            : base()
        {
            @delegate = initDelegate!;
        }

        public static TInterface Create<TInterface>(Func<MethodInfo, object[], object?> @delegate)
        {
            lock (initLock)
            {
                try
                {
                    initDelegate = @delegate;
                    return Create<TInterface, DelegatingDispatchProxy>();
                }
                finally
                {
                    initDelegate = null;
                }
            }
        }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            return @delegate(targetMethod!, args!);
        }
    }
}
