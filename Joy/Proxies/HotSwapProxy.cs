namespace AvP.Joy.Proxies
{
    public class HotSwapProxy<TInterface> : HotSwapProxyBase<TInterface>
    {
        public HotSwapProxy(TInterface target) : base(target) { }
        public new TInterface Target { get { return base.Target; } }
        public new void SetTarget(TInterface target) { base.SetTarget(target); }
    }
}