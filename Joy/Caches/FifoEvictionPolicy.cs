using System.Diagnostics.CodeAnalysis;

namespace AvP.Joy.Caches;

public class FifoEvictionPolicy<TKey> : IEvictionPolicy<TKey>
{
    private readonly Queue<TKey> keysByInsertionOrder = new();
    private readonly int maxCount;

    public FifoEvictionPolicy(int maxCount = 1024)
    {
        if (maxCount < 0)
            throw new ArgumentOutOfRangeException(nameof(maxCount));

        this.maxCount = maxCount;
    }

    public bool ShouldEvictOnAdding(TKey keyBeingAdded, [MaybeNullWhen(false)] out TKey keyToEvict)
    {
        keysByInsertionOrder.Enqueue(keyBeingAdded);

        if (keysByInsertionOrder.Count > maxCount)
        {
            keyToEvict = keysByInsertionOrder.Dequeue();
            return true;
        }
        else
        {
            keyToEvict = default;
            return false;
        }
    }

    public void OnGetting(TKey key)
    {
        // No op.
    }
}
