namespace ECommerce.APP.Features.Auth;

public sealed record AuthResponse(
    Guid UserId,
    string Email,
    string? UserDisplayName,
    string AccessToken,
    DateTimeOffset ExpiresAtUtc);
