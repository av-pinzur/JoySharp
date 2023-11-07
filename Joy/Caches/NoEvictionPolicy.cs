using System.Diagnostics.CodeAnalysis;

namespace AvP.Joy.Caches;

internal sealed class NoEvictionPolicy<TKey> : IEvictionPolicy<TKey>
{
    public bool ShouldEvictOnAdding(TKey keyBeingAdded, [MaybeNullWhen(false)] out TKey keyToEvict)
    {
        keyToEvict = default;
        return false;
    }

    public void OnGetting(TKey key) { }
}