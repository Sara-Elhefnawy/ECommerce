using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Users.Commands.ResetPassword;

public sealed record ResetPasswordCommand(string Email) : IRequest<ResultOfT<ResetPasswordResponse>>;
