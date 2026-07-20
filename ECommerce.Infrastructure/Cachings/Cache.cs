using ECommerce.APP.Cachings;
using ECommerce.Infrastructure.Caching;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;

namespace ECommerce.Infrastructure.Cachings;

public sealed class Cache<T>(HybridCache cache, IOptionsMonitor<CacheEntryPolicy> options) : ICache<T> where T : class
{
    // T is the is Basket that is in appsettings
    private readonly CacheEntryPolicy _options = options.Get(typeof(T).Name);

    public async Task<T?> GetAsync(string cacheKey, CancellationToken ct = default)
    {
        // envelop contains the data in CacheEnvelope (all data in cart + CreatedAtUtc + LastAccessedUtc)
        var envelop = await cache.TryGetAsync<CacheEnvelope<T>>(cacheKey, ct);

        return envelop?.Payload;
    }

    // if found the key then return it and update LastAccessedUtc
    //      check SlidingRefreshThresholdMinutes before updating LastAccessedUtc
    //      if 30 minutes of SlidingRefreshThresholdMinutes passed then update LastAccessedUtc
    // if not then create and update CreatedAtUtc
    public async Task<T> GetOrCreateAsync(string cacheKey, Func<CancellationToken, Task<T>> factory, CancellationToken ct = default)
    {
        var envelop = await cache.GetOrCreateAsync(
                cacheKey,
                async ct =>    // if key not found
                {
                    // call the factory function that takes CancellationToken
                    // that return the key value from DB
                    var value = await factory(ct);

                    var utcNow = DateTimeOffset.UtcNow;

                    return new CacheEnvelope<T> { Payload = value, CreatedAtUtc = utcNow, LastAccessedUtc = utcNow };
                },
                // when creating the value in cache must tell how much it lives is RAM
                CreateEntryOptionsForNewEnvelopInCache(),  // if key not found
                cancellationToken: ct
            );

        // before updating LastAccessedUtc i must ensure SlidingRefreshThresholdMinutes has passed
        await RefreshExpirationIfNeededAsync(cacheKey, envelop, ct);

        return envelop.Payload;
    }

    public async Task SetAsync(string cacheKey, T value, CancellationToken ct = default)
    {
        var exsisting = await cache.TryGetAsync<CacheEnvelope<T>>(cacheKey, ct);

        var envelop = new CacheEnvelope<T>
        {
            Payload = value,
            CreatedAtUtc = exsisting?.CreatedAtUtc ?? DateTimeOffset.UtcNow,
            LastAccessedUtc = DateTimeOffset.UtcNow
        };

        await SetOrRemoveIfExpiredAsync(cacheKey, envelop, ct);
    }

    public async Task RemoveAsync(string cacheKey, CancellationToken ct = default)
        => await cache.RemoveAsync(cacheKey, ct);

    private async Task RefreshExpirationIfNeededAsync(string cacheKey, CacheEnvelope<T> envelop, CancellationToken ct)
    {
        var utcNow = DateTimeOffset.UtcNow;

        var age = utcNow - envelop.LastAccessedUtc;

        // Skip Redis write when the entry was accessed recently
        if (age < TimeSpan.FromMinutes(_options.SlidingRefreshThresholdMinutes))
            return;

        var refreshed = new CacheEnvelope<T>
        {
            Payload = envelop.Payload,
            CreatedAtUtc = envelop.CreatedAtUtc,
            LastAccessedUtc = utcNow
        };

        await SetOrRemoveIfExpiredAsync(cacheKey, refreshed, ct);
    }

    private async Task SetOrRemoveIfExpiredAsync(string cacheKey, CacheEnvelope<T> refreshed, CancellationToken ct)
    {
        var expiration = CalculateExpiration(refreshed.CreatedAtUtc, refreshed.LastAccessedUtc, DateTimeOffset.UtcNow);

        if (expiration is null)
        {
            await cache.RemoveAsync(cacheKey, ct);
            return;
        }

        //          to set i need the key and the value and how much will it live
        await cache.SetAsync(cacheKey, refreshed, CreateEntryOption(expiration.Value), cancellationToken: ct);
    }

    private HybridCacheEntryOptions CreateEntryOptionsForNewEnvelopInCache()
    {
        var utcNow = DateTimeOffset.UtcNow;

        var expiration = CalculateExpiration(utcNow, utcNow, utcNow) ?? throw new Exception("user is only allowed 30 days but he opened cart on day 31");

        // to turn the expiration of type TimeSpan into type HybridCacheEntryOptions
        // as GetOrCreateAsync only accept that type not TimeSpan
        return CreateEntryOption(expiration);
    }

    private HybridCacheEntryOptions CreateEntryOption(TimeSpan expiration)
    {
        // expiration is in redis while localExpiration is in RAM for caching
        var localExpiration = TimeSpan.FromMinutes(_options.LocalCacheExpirationMinutes);

        // localExpiration must never be longer in time than expiration in redis
        //      as redis's is the source of truth
        if (localExpiration > expiration)
            localExpiration = expiration;

        return new() 
        { 
            Expiration = expiration,
            LocalCacheExpiration = localExpiration
        };
    }

    private TimeSpan? CalculateExpiration(
        DateTimeOffset createdAtUtc,
        DateTimeOffset lastAccessedAtUtc,
        DateTimeOffset utcNow
        )
    {
        var absoluteRemaining = createdAtUtc                 // account was created 10 days ago
            .AddDays(_options.AbsoluteExpirationDays)        // AbsoluteExpirationDays is 30
            .Subtract(utcNow);                               // remaining = createdAtUtc + AbsoluteExpirationDays - Today

        var slidingRemaining = lastAccessedAtUtc
            .AddDays(_options.SlidingExpirationDays)
            .Subtract(utcNow);

        // check if they are negative
        //      this will happen if user allowed 30 days but he opened cart on day 31
        if (absoluteRemaining <= TimeSpan.Zero || slidingRemaining <= TimeSpan.Zero)
            return null;

        //            30         >=        7
        return absoluteRemaining >= slidingRemaining
            ? absoluteRemaining         // return absolute to remove from cache
            : slidingRemaining;         // return sliding 

        // when sliding is bigger than absolute? 
        // if user have 30 days but he accessed 2 days before absolute so sliding is added 7 days
        //      then sliding is 30 - 2 + 7 
    }
}
