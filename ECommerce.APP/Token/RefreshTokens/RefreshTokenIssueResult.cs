namespace ECommerce.APP.Token.RefreshTokens;

public sealed record RefreshTokenIssueResult(
    Guid UserId,
    string Token,
    DateTimeOffset ExpiresAtUtc
    );
