using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Auth.Commands.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string? UserDisplayName) : IRequest<ResultOfT<EmailSentResponse>>;
