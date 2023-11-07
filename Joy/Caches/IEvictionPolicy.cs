using System.Diagnostics.CodeAnalysis;

namespace AvP.Joy.Caches;

public interface IEvictionPolicy<TKey>
{
    void OnGetting(TKey key);
    bool ShouldEvictOnAdding(TKey keyBeingAdded, [MaybeNullWhen(false)] out TKey keyToEvict);
}
