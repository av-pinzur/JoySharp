namespace AvP.Joy.Caches;

public class FifoReadCache<TKey, TValue> : MemoryReadCache<TKey, TValue> where TKey : notnull
{
    public FifoReadCache(int maxCount = 1024)
        : base(new FifoEvictionPolicy<TKey>(maxCount)) { }
}
