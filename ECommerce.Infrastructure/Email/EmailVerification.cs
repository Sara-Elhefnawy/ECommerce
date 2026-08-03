using ECommerce.APP.Email;
using ECommerce.Infrastructure.Caching;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace ECommerce.Infrastructure.Email;

// EmailVerification uses raw HybridCache with a flat Expiration = ExpirationMinutes
// Forcing it into the ICache<T> shape would be overengineering in the bad direction:
//      bending a security-sensitive short-lived token to fit an abstraction built for
//      session-continuity data, just for the sake of 1 caching pattern for everything
// Two independent cache entries per email:
//  1- "code" entry: the hashed OTP. Dies at ExpirationMinutes,
//      replaced whenever a new code is requested (SaveAsync).
//  2- "lockout" entry: a failed-attempt counter with LockoutMinutes, tracked
//      separately from the code.
//  Regenerating the code must NOT reset how many times
//      someone has already guessed wrong.
public sealed class EmailVerification(
    HybridCache cache,
    IOptions<EmailVerificationSettings> settings) : IEmailVerification
{
    private const string CodeKeyPrefix = "email-verification:code:";
    private const string LockoutKeyPrefix = "email-verification:lockout:";

    public async Task SaveAsync(
        string email,
        string code,
        CancellationToken ct = default)
    {
        // if already locked out, don't hand out a new code
        if (await IsLockedOutAsync(email, ct))
            throw new InvalidOperationException("Too many failed attempts. Try again later.");

        var options = settings.Value;
        var codeEntry = new CodeEntry(HashCode(code));

        await cache.SetAsync(
            BuildCodeKey(email),
            codeEntry,
            new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(options.ExpirationMinutes),
                LocalCacheExpiration = TimeSpan.FromMinutes(Math.Min(5, options.ExpirationMinutes))
            },
            cancellationToken: ct);
    }

    public async Task<bool> ValidateAndConsumeAsync(
        string email,
        string code,
        CancellationToken ct = default)
    {
        if (await IsLockedOutAsync(email, ct))
            return false;

        var codeEntry = await cache.TryGetAsync<CodeEntry>(BuildCodeKey(email), ct);

        if (codeEntry is null)
            return false;

        var expected = Encoding.UTF8.GetBytes(codeEntry.CodeHash);
        var actual = Encoding.UTF8.GetBytes(HashCode(code));

        var isMatch = expected.Length == actual.Length
            && CryptographicOperations.FixedTimeEquals(expected, actual);

        if (!isMatch)
        {
            await RegisterFailedAttemptAsync(email, ct);
            return false;
        }

        // success clears both the code AND any accumulated failed-attempt
        // count — a legitimate verification should wipe the slate
        await cache.RemoveAsync(BuildCodeKey(email), ct);
        await cache.RemoveAsync(BuildLockoutKey(email), ct);
        return true;
    }

    private async Task<bool> IsLockedOutAsync(string email, CancellationToken ct)
    {
        var lockoutEntry = await cache.TryGetAsync<LockoutEntry>(BuildLockoutKey(email), ct);
        return lockoutEntry is not null
            && lockoutEntry.FailedAttempts >= settings.Value.MaxFailedAttempts;
    }

    private async Task RegisterFailedAttemptAsync(string email, CancellationToken ct)
    {
        var options = settings.Value;
        var key = BuildLockoutKey(email);
        var existing = await cache.TryGetAsync<LockoutEntry>(key, ct);
        var nextCount = (existing?.FailedAttempts ?? 0) + 1;

        await cache.SetAsync(
            key,
            new LockoutEntry(nextCount),
            new HybridCacheEntryOptions
            {
                // LockoutMinutes, not ExpirationMinutes — this clock is
                // independent of the code's own lifetime.
                Expiration = TimeSpan.FromMinutes(options.LockoutMinutesAfterMaxFailedAttempts),
                LocalCacheExpiration = TimeSpan.FromMinutes(Math.Min(5, options.LockoutMinutesAfterMaxFailedAttempts))
            },
            cancellationToken: ct);
    }

    private static string BuildCodeKey(string email) =>
        CodeKeyPrefix + Normalize(email);

    private static string BuildLockoutKey(string email) =>
        LockoutKeyPrefix + Normalize(email);

    private static string Normalize(string email) => email.Trim().ToLowerInvariant();

    private static string HashCode(string code)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(code.Trim()));
        return Convert.ToHexString(bytes);
    }

    private sealed record CodeEntry(string CodeHash);
    private sealed record LockoutEntry(int FailedAttempts);
}
