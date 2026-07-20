namespace ECommerce.Infrastructure.Cachings;

public sealed class CacheEnvelope<T>
{
    // is the actual data we want to cache, and we don't want to cache null values
    public required T Payload { get; init; }

    public DateTimeOffset CreatedAtUtc { get; init; }

    public DateTimeOffset LastAccessedUtc { get; init; }
}
