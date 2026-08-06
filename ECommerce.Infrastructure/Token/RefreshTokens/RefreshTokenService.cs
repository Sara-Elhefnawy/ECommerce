using ECommerce.APP.Settings;
using ECommerce.APP.Token.RefreshTokens;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;
using ECommerce.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace ECommerce.Infrastructure.Token.RefreshTokens;

public sealed class RefreshTokenService(
    ECommerceIdentityDbContext dbContext,
    IOptions<JwtSettings> settings)
    : IRefreshTokenService
{
    private readonly JwtSettings _settings = settings.Value;

    // Called when a user logs in successfully and when confirming email
    public async Task<RefreshTokenIssueResult> IssueAsync(
        Guid userId,
        CancellationToken ct = default)
    {
        // Generate the actual "plainToken" value
        // AND the DB "entity" that represents it
        var (plainToken, entity) = CreateTokenEntity(userId);

        dbContext.RefreshTokens.Add(entity);

        await dbContext.SaveChangesAsync(ct);

        // Return the PLAIN (unhashed) token to the caller
        //  This is the only time the plain value ever exists outside this method.
        //  The caller will send it to the client (e.g. as a cookie or JSON field).
        return new RefreshTokenIssueResult(userId, plainToken, entity.ExpiresAtUtc);
    }

    // Called when a client sends back a refresh token asking for a new access token.
    // "Rotate" = validate the old one, then replace it with a new one (old one becomes unusable).
    // This single-use pattern is what lets us detect token theft
    public async Task<ResultOfT<RefreshTokenIssueResult>> RotateAsync(
        string refreshToken,
        CancellationToken ct = default)
    {
        // We never look up by the plain token
        //      we hash whatever came in and compare hashes,
        //          because that's what's stored in the DB.
        // This is like how password checking never stores/searches plain passwords.
        var tokenHash = HashToken(refreshToken);

        // TokenHash is configured as a unique index in the DB
        var existing = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, ct);

        if (existing is null)
            return ResultOfT<RefreshTokenIssueResult>.Failure(IdentityErrors.InvalidRefreshToken);

        // IMPORTANT SECURITY CHECK:
        // Remember, tokens are single-use -
        //      once rotated, the old one gets marked revoked
        // So if someone presents a token that's ALREADY revoked, that means either:
        //   (a) it was already used once legitimately, and now someone else
        //       (an attacker who stole a copy) is trying to reuse it, or
        //   (b) the real user's copy was stolen and used by an attacker, and
        //       now the real user is trying to use their now-stale copy.
        // So the safe move is to kill EVERY active refresh token for this user
        //      forcing them to log in again everywhere
        if (existing.IsRevoked)
        {
            await RevokeAllActiveForUserAsync(existing.UserId, ct);
            return ResultOfT<RefreshTokenIssueResult>.Failure(IdentityErrors.InvalidRefreshToken);
        }

        if (existing.IsExpired)
            return ResultOfT<RefreshTokenIssueResult>.Failure(IdentityErrors.RefreshTokenExpired);

        // Token is valid - generate its replacement (new plain token + new DB row)
        var (plainToken, replacement) = CreateTokenEntity(existing.UserId);

        // Mark the OLD row as revoked, and record which new token replaced it
        // Storing this link lets you trace a token's rotation history if you ever need to audit/debug.
        existing.Revoke(replacement.TokenHash);

        // Insert the new row alongside marking the old one revoked.
        dbContext.RefreshTokens.Add(replacement);

        await dbContext.SaveChangesAsync(ct);

        return ResultOfT<RefreshTokenIssueResult>.Ok(
            new RefreshTokenIssueResult(existing.UserId, plainToken, replacement.ExpiresAtUtc));
    }

    // Called on explicit logout - rather than waiting for it to expire or be rotated.
    public async Task<Result> RevokeAsync(
        string refreshToken,
        CancellationToken ct = default)
    {
        var tokenHash = HashToken(refreshToken);
        var existing = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, ct);

        if (existing is null)
            return Result.Failure(IdentityErrors.InvalidRefreshToken);

        if (!existing.IsRevoked)
        {
            existing.Revoke();
            await dbContext.SaveChangesAsync(ct);
        }

        return Result.Ok();
    }

    // Shared helper: builds BOTH the plain token (to give to the client) and
    // the DB entity (to store the hash of it).
    private (string PlainToken, RefreshToken Entity) CreateTokenEntity(Guid userId)
    {
        var plainToken = GenerateSecureToken();

        var expiresAt = DateTimeOffset.UtcNow.AddDays(_settings.RefreshTokenExpirationDays);

        var entity = RefreshToken.Create(
            userId,
            HashToken(plainToken),   // we only ever persist the HASH, never the plain value
            expiresAt);

        return (plainToken, entity);
    }

    // The "theft response" helper called from RotateAsync when reuse of a revoked token is detected.
    // Revokes every token this user currently has that hasn't already been revoked
    //      (RevokedAtUtc == null means "still active").
    private async Task RevokeAllActiveForUserAsync(Guid userId, CancellationToken ct = default)
    {
        var activeTokens = dbContext.RefreshTokens
            .Where(x => x.UserId == userId && x.RevokedAtUtc == null)
            .ToList();

        foreach (var token in activeTokens)
            token.Revoke();

        if (activeTokens.Count > 0)
            await dbContext.SaveChangesAsync(ct);
    }

    // Generates the actual random token value that gets sent to the client.
    private static string GenerateSecureToken()
    {
        // stackalloc allocates this 64-byte buffer on the stack instead of heap
        //      it's freed automatically when the method returns, no GC needed
        // Slight performance optimization for a short-lived buffer like this.
        Span<byte> bytes = stackalloc byte[64];

        // RandomNumberGenerator is CRYPTOGRAPHICALLY secure
        //      meaning its output can't be predicted even if you know previous outputs
        // Regular Random must NEVER be used for anything security-related
        //      (tokens, passwords, keys) because its output is predictable/guessable.
        RandomNumberGenerator.Fill(bytes);

        // Base64-encode the random bytes into a plain string so it's safe to put in
        //      JSON, cookies, headers, etc.
        return Convert.ToBase64String(bytes);
    }

    // Turns a plain token string into its SHA-256 hash (as a hex string),
    // This is a ONE-WAY you can hash a token to check it,
    //      but you can never reverse a hash back into the original token.
    // Even if someone reads your DB, they can't reconstruct usable tokens from it.
    private static string HashToken(string token)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(hash);
    }
}
