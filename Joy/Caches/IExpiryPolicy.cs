
namespace AvP.Joy.Caches;

public interface IExpiryPolicy<in TValue>
{
    bool IsExpired(TValue value, DateTimeOffset fetchedAt, DateTimeOffset now);
}
