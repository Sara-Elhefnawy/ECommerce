using ECommerce.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace ECommerce.Tests.Unit.Domain;

public class RefreshTokenTests
{
    private static RefreshToken CreateValid(
        Guid? userId = null,
        string tokenHash = "hash123",
        DateTimeOffset? expiresAtUtc = null) =>
        RefreshToken.Create(
            userId ?? Guid.NewGuid(),
            tokenHash,
            expiresAtUtc ?? DateTimeOffset.UtcNow.AddDays(7));

    // ---------- Create ----------
    [Fact]
    public void Create_WithValidData_Succeeds()
    {
        var userId = Guid.NewGuid();
        var expires = DateTimeOffset.UtcNow.AddDays(7);

        var token = RefreshToken.Create(userId, "hash123", expires);

        token.UserId.Should().Be(userId);
        token.TokenHash.Should().Be("hash123");
        token.ExpiresAtUtc.Should().Be(expires);
    }

    [Fact]
    public void Create_WithEmptyUserId_ThrowsArgumentException()
    {
        Action act = () => RefreshToken.Create(Guid.Empty, "hash123", DateTimeOffset.UtcNow.AddDays(1));

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidTokenHash_ThrowsArgumentException(string? tokenHash)
    {
        Action act = () => RefreshToken.Create(Guid.NewGuid(), tokenHash!, DateTimeOffset.UtcNow.AddDays(1));

        act.Should().Throw<ArgumentException>();
    }

    // ---------- IsExpired / IsRevoked / IsActive ----------
    [Fact]
    public void IsExpired_WhenExpiryIsInThePast_ReturnsTrue()
    {
        var token = CreateValid(expiresAtUtc: DateTimeOffset.UtcNow.AddMinutes(-1));

        token.IsExpired.Should().BeTrue();
        token.IsActive.Should().BeFalse();
    }

    [Fact]
    public void IsExpired_WhenExpiryIsInTheFuture_ReturnsFalse()
    {
        var token = CreateValid(expiresAtUtc: DateTimeOffset.UtcNow.AddMinutes(5));

        token.IsExpired.Should().BeFalse();
    }

    [Fact]
    public void NewToken_IsNotRevokedAndIsActive()
    {
        var token = CreateValid();

        token.IsRevoked.Should().BeFalse();
        token.IsActive.Should().BeTrue();
    }

    // ---------- Revoke ----------
    [Fact]
    public void Revoke_SetsRevokedAtUtcAndReplacedByTokenHash()
    {
        var token = CreateValid();

        token.Revoke("newHash456");

        token.IsRevoked.Should().BeTrue();
        token.RevokedAtUtc.Should().NotBeNull();
        token.ReplacedByTokenHash.Should().Be("newHash456");
        token.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Revoke_WithoutReplacement_LeavesReplacedByTokenHashNull()
    {
        var token = CreateValid();

        token.Revoke();

        token.IsRevoked.Should().BeTrue();
        token.ReplacedByTokenHash.Should().BeNull();
    }

    [Fact]
    public void Revoke_CalledTwice_IsIdempotentAndKeepsFirstRevocation()
    {
        // Revoke() short-circuits if already revoked, so a second call
        // (e.g. a retried request) must not overwrite the original
        // RevokedAtUtc/ReplacedByTokenHash with different values.
        var token = CreateValid();
        token.Revoke("firstReplacement");
        var firstRevokedAt = token.RevokedAtUtc;

        token.Revoke("secondReplacement");

        token.RevokedAtUtc.Should().Be(firstRevokedAt);
        token.ReplacedByTokenHash.Should().Be("firstReplacement");
    }
}
