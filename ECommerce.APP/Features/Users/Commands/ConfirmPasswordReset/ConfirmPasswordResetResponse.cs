namespace ECommerce.APP.Features.Users.Commands.ConfirmPasswordReset;

public sealed record ConfirmPasswordResetResponse(
    Guid UserId,
    string Message);
