namespace AvP.Joy.Caches
{
    public class FifoReadCache<TKey, TValue> : IReadCache<TKey, TValue> where TKey : notnull
    {
        private readonly ReaderWriterLockSlim cacheLock = new(LockRecursionPolicy.SupportsRecursion);
        private readonly Dictionary<TKey, TValue> cache = new();
        private readonly int maxCount;
        private readonly Queue<TKey> keysByInsertionOrder = new();

        public FifoReadCache(int maxCount = 1024)
        {
            this.maxCount = maxCount;
        }

        public TValue GetOrAdd(TKey key, Func<TValue> valueFn)
        {
            // Try with a read-only lock to maximize throughput.
            using (cacheLock.EnterReadLockDisposable())
                if (cache.TryGetValue(key, out TValue? value))
                    return value;

            // Release read lock before obtaining write lock
            // to avoid deadlock potential.
            using (cacheLock.EnterWriteLockDisposable())
                // Check again for a current value,
                // since another thread could have fetched it
                // once we released our original read lock.
                return cache.GetOrAdd(key, () =>
                {
                    TValue value = valueFn();
                    keysByInsertionOrder.Enqueue(key);

                    while (keysByInsertionOrder.Count > maxCount)
                        cache.Remove(keysByInsertionOrder.Dequeue());

                    return value;
                });
        }
    }
}
