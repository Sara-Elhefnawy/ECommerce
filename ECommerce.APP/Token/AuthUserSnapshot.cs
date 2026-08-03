namespace ECommerce.APP.Token;

public sealed record AuthUserSnapshot(
    Guid UserId, 
    string Email, 
    string? UserDisplayName);
