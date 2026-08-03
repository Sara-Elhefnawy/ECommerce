namespace ECommerce.APP.Features.Users;

public sealed record UserProfileResponse(
    Guid UserId,
    string Email,
    string? UserDisplayName);
