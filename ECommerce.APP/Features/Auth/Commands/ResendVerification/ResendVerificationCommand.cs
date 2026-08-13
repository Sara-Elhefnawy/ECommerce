using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Auth.Commands.ResendVerification;

public sealed record ResendVerificationCommand(
    string Email)
    : IRequest<ResultOfT<EmailSentResponse>>;
