using ECommerce.APP.Cachings;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ECommerce.Infrastructure.Cachings;

public sealed class Cache<T>(
    HybridCache cache, 
    IOptionsMonitor<CacheEntryPolicy> options,
    ILogger<Cache<T>> logger) 
    : ICache<T> where T : class
{
    // T is the is Basket that is in appsettings
    private readonly CacheEntryPolicy _options = options.Get(typeof(T).Name);

    public async Task<ResultOfT<T?>> GetAsync(string cacheKey, CancellationToken ct = default)
    {
        try
        {
            // envelop contains the data in CacheEnvelope (all data in cart + CreatedAtUtc + LastAccessedUtc)
            var envelop = await cache.TryGetAsync<CacheEnvelope<T>>(cacheKey, ct);
            return ResultOfT<T?>.Ok(envelop?.Payload);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Cache read failed for key {CacheKey}", cacheKey);
            return CacheErrors.OperationFailed;
        }
    }

    // if found the key then return it and update LastAccessedUtc
    //      check SlidingRefreshThresholdMinutes before updating LastAccessedUtc
    //      if 30 minutes of SlidingRefreshThresholdMinutes passed then update LastAccessedUtc
    // if not then create and update CreatedAtUtc
    public async Task<ResultOfT<T>> GetOrCreateAsync(string cacheKey, Func<CancellationToken, Task<T>> factory, CancellationToken ct = default)
    {
        // Deliberately OUTSIDE the try/catch below: this can throw for a
        // business-rule reason ("user is only allowed 30 days but he opened
        // cart on day 31"), not because Redis is down. Letting it propagate
        // keeps that distinct from CacheErrors.OperationFailed — don't merge
        // these two catches later without re-checking this.
        CacheEnvelope<T> envelop;

        try
        {
            envelop = await cache.GetOrCreateAsync(
                cacheKey,
                async innerCt =>    // if key not found
                {
                    // call the factory function that takes CancellationToken
                    // that return the key value from DB
                    var value = await factory(innerCt);

                    var utcNow = DateTimeOffset.UtcNow;

                    return new CacheEnvelope<T> { Payload = value, CreatedAtUtc = utcNow, LastAccessedUtc = utcNow };
                },
                // when creating the value in cache must tell how much it lives is RAM
                CreateEntryOptionsForNewEnvelopInCache(),  // if key not found
                cancellationToken: ct
            );
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // NOTE: this also catches an exception thrown by `factory` itself
            // (e.g. a DB error looking up the value on a cache miss) — HybridCache
            // doesn't let us tell "Redis is down" and "the DB lookup failed"
            // apart here. If that distinction matters to you later, `factory`
            // needs to catch its own failures before they reach this catch.
            logger.LogError(ex, "Cache get-or-create failed for key {CacheKey}", cacheKey);
            return CacheErrors.OperationFailed;
        }

        var refreshResult = await RefreshExpirationIfNeededAsync(cacheKey, envelop, ct);
        if (refreshResult.IsFailure)
            return refreshResult.Error!;

        return envelop.Payload;
    }

    public async Task<Result> SetAsync(string cacheKey, T value, CancellationToken ct = default)
    {
        try
        {
            var exsisting = await cache.TryGetAsync<CacheEnvelope<T>>(cacheKey, ct);

            var envelop = new CacheEnvelope<T>
            {
                Payload = value,
                CreatedAtUtc = exsisting?.CreatedAtUtc ?? DateTimeOffset.UtcNow,
                LastAccessedUtc = DateTimeOffset.UtcNow
            };

            return await SetOrRemoveIfExpiredAsync(cacheKey, envelop, ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Cache write failed for key {CacheKey}", cacheKey);
            return CacheErrors.OperationFailed;
        }
    }

    public async Task<Result> RemoveAsync(string cacheKey, CancellationToken ct = default)
    {
        try
        {
            await cache.RemoveAsync(cacheKey, ct);
            return Result.Ok();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Cache remove failed for key {CacheKey}", cacheKey);
            return CacheErrors.OperationFailed;
        }
    }

    private async Task<Result> RefreshExpirationIfNeededAsync(string cacheKey, CacheEnvelope<T> envelop, CancellationToken ct)
    {
        var utcNow = DateTimeOffset.UtcNow;

        var age = utcNow - envelop.LastAccessedUtc;

        // Skip Redis write when the entry was accessed recently
        if (age < TimeSpan.FromMinutes(_options.SlidingRefreshThresholdMinutes))
            return Result.Ok();


        var refreshed = new CacheEnvelope<T>
        {
            Payload = envelop.Payload,
            CreatedAtUtc = envelop.CreatedAtUtc,
            LastAccessedUtc = utcNow
        };

        try
        {
            return await SetOrRemoveIfExpiredAsync(cacheKey, refreshed, ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Cache refresh failed for key {CacheKey}", cacheKey);
            return CacheErrors.OperationFailed;
        }
    }

    private async Task<Result> SetOrRemoveIfExpiredAsync(string cacheKey, CacheEnvelope<T> refreshed, CancellationToken ct)
    {
        var expiration = CalculateExpiration(refreshed.CreatedAtUtc, refreshed.LastAccessedUtc, DateTimeOffset.UtcNow);

        if (expiration is null)
        {
            await cache.RemoveAsync(cacheKey, ct);
            return Result.Ok();
        }

        //          to set i need the key and the value and how much will it live
        await cache.SetAsync(cacheKey, refreshed, CreateEntryOption(expiration.Value), cancellationToken: ct);

        return Result.Ok();
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
        DateTimeOffset utcNow)
    {
        // === MINUTE-BASED EXPIRATION (overrides day-based when set) ===
        // Used by ResetPasswordToken (real config) and can be used by any
        // type when AbsoluteExpirationMinutes is set in its Cache:<Name> section.
        if (_options.AbsoluteExpirationMinutes > 0)
        {
            var expiresAt = createdAtUtc.AddMinutes(_options.AbsoluteExpirationMinutes);
            var remaining = expiresAt.Subtract(utcNow);

            return remaining <= TimeSpan.Zero ? null : remaining;
        }

        // === DAY-BASED EXPIRATION (absolute + sliding, e.g. Cart) ===
        var absoluteRemaining = createdAtUtc
            .AddDays(_options.AbsoluteExpirationDays)
            .Subtract(utcNow);

        var slidingRemaining = lastAccessedAtUtc
            .AddDays(_options.SlidingExpirationDays)
            .Subtract(utcNow);

        if (absoluteRemaining <= TimeSpan.Zero || slidingRemaining <= TimeSpan.Zero)
            return null;

        return absoluteRemaining >= slidingRemaining
            ? absoluteRemaining
            : slidingRemaining;
    }
}
