namespace ECommerce.Domain.Entities;

public sealed class RefreshToken : BaseEntity
{
    public const int MaxTokenHashLength = 64;

    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = default!;
    public DateTimeOffset ExpiresAtUtc { get; private set; }
    public DateTimeOffset? RevokedAtUtc { get; private set; }
    public string? ReplacedByTokenHash { get; private set; }

    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAtUtc;
    public bool IsRevoked => RevokedAtUtc is not null;
    public bool IsActive => !IsRevoked && !IsExpired;

    private RefreshToken()
    {
    }

    private RefreshToken(
        Guid userId,
        string tokenHash,
        DateTimeOffset expiresAtUtc
        )
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAtUtc = expiresAtUtc;

        Id = Guid.NewGuid();
    }

    public static RefreshToken Create(
        Guid userId,
        string tokenHash,
        DateTimeOffset expiresAtUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tokenHash);

        if (userId == Guid.Empty)
            throw new ArgumentException("User id is required.", nameof(userId));

        return new RefreshToken
        (
            userId,
            tokenHash,
            expiresAtUtc
        );
    }

    public void Revoke(string? replacedByTokenHash = null)
    {
        if (IsRevoked)
            return;

        RevokedAtUtc = DateTimeOffset.UtcNow;       // (RevokedAtUtc == null means "still active").
        ReplacedByTokenHash = replacedByTokenHash;
    }
}
