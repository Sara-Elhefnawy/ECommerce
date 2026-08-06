using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Users.Commands.ConfirmPasswordReset;

public sealed record ConfirmPasswordResetCommand(
    string Email,
    string PasswordResetToken,
    string NewPassword)
    : IRequest<ResultOfT<ConfirmPasswordResetResponse>>;
