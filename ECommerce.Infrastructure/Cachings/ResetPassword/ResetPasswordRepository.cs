using ECommerce.APP.Cachings;
using ECommerce.APP.Cachings.ResetPassword;

namespace ECommerce.Infrastructure.Cachings.ResetPassword;

public class ResetPasswordRepository(
    ICache<ResetPasswordToken> cache)
    : IResetPasswordRepository
{
    public Task SaveAsync(string hashedToken, Guid userId, CancellationToken ct = default)
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

    public async Task<Guid?> GetUserIdAsync(string hashedToken, CancellationToken ct = default)
    {
        string cacheKey = $"password-reset:{hashedToken}";
        var tokenEntry = await cache.GetAsync(cacheKey, ct);
        return tokenEntry?.UserId;
    }

    public Task DeleteAsync(
        string hashedToken,
        CancellationToken ct = default)
    {
        string cacheKey = $"password-reset:{hashedToken}";
        return cache.RemoveAsync(cacheKey, ct);
    }
}
