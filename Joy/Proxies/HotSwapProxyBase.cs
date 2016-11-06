using System;
using System.Threading;

namespace AvP.Joy.Proxies
{
    public abstract class HotSwapProxyBase<TInterface> : TargetedProxyBase<TInterface>
    {
        private TInterface target;
        private readonly ReaderWriterLockSlim targetLock = new ReaderWriterLockSlim();

        protected HotSwapProxyBase(TInterface target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            this.target = target;
        }

        protected override TInterface Target
        {
            get
            {
                using (targetLock.EnterReadLockDisposable())
                    return target;
            }
        }

        protected void SetTarget(TInterface target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            using (targetLock.EnterWriteLockDisposable())
                this.target = target;
        }
    }
}