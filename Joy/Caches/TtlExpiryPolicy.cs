namespace AvP.Joy.Caches;

public class TtlExpiryPolicy<TValue> : IExpiryPolicy<TValue>
{
    private readonly TimeSpan maxAge;

    public TtlExpiryPolicy(TimeSpan maxAge)
    {
        if (maxAge < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(maxAge));

        this.maxAge = maxAge;
    }

    public bool IsExpired(TValue value, DateTimeOffset fetchedAt, DateTimeOffset now) =>
        F.Let(
            now - fetchedAt,
            age => age > maxAge
        );
}
