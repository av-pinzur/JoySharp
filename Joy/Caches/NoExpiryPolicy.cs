
namespace AvP.Joy.Caches;

internal sealed class NoExpiryPolicy<TValue> : IExpiryPolicy<TValue>
{
    public NoExpiryPolicy() { }
    public bool IsExpired(TValue value, DateTimeOffset fetchedAt, DateTimeOffset now) => false;
}
