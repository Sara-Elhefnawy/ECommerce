using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Auth.Commands.RefreshTokens;

public sealed record RefreshTokenCommand(
    string RefreshToken) : IRequest<ResultOfT<AuthResponse>>;
