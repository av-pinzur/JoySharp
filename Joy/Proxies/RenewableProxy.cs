using System;

namespace AvP.Joy.Proxies
{
    public class RenewableProxy<TInterface> : HotSwapProxyBase<TInterface>
    {
        private Func<TInterface> targetGetter;

        public RenewableProxy(Func<TInterface> targetGetter) 
            : base(EnsuringNotNull(targetGetter, "targetGetter")())
        {
            this.targetGetter = targetGetter;
        }

        private static T EnsuringNotNull<T>(T parameterValue, string parameterName) where T : class
        {
            if (parameterValue == null) throw new ArgumentNullException(parameterName);
            return parameterValue;
        }

        public void Renew() { SetTarget(targetGetter()); }
    }
}