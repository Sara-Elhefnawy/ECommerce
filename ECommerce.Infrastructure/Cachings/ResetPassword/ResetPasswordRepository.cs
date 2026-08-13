using ECommerce.APP.Cachings;
using ECommerce.APP.Cachings.ResetPassword;
using ECommerce.Domain.Results;

namespace ECommerce.Infrastructure.Cachings.ResetPassword;

public class ResetPasswordRepository(
    ICache<ResetPasswordToken> cache)
    : IResetPasswordRepository
{
    public Task<Result> SaveAsync(string hashedToken, Guid userId, CancellationToken ct = default)
    {
        // Build the cache key using the hashed token
        string cacheKey = $"password-reset:{hashedToken}";

        // Create the token entry wrapping the userId
        var tokenEntry = new ResetPasswordToken { UserId = userId };

        // Save to cache. Expiration is managed by HybridCache
        // based on CacheEntryPolicy configured in appsettings
        // ("Caching:ResetPasswordToken" → AbsoluteExpirationMinutes: 15)
        return cache.SetAsync(cacheKey, tokenEntry, ct);
    }

    public async Task<ResultOfT<Guid?>> GetUserIdAsync(string hashedToken, CancellationToken ct = default)
    {
        string cacheKey = $"password-reset:{hashedToken}";
        var result = await cache.GetAsync(cacheKey, ct);

        if (result.IsFailure)
            return result.Error!;

        return ResultOfT<Guid?>.Ok(result.Value?.UserId);
    }

    public Task<Result> DeleteAsync(string hashedToken, CancellationToken ct = default)
        => cache.RemoveAsync($"password-reset:{hashedToken}", ct);
}
