using ECommerce.APP.Mediator;
using ECommerce.APP.Token.RefreshTokens;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Auth.Commands.Logout;

public sealed class LogoutHandler(IRefreshTokenService refreshTokenService) : IRequestHandler<LogoutCommand, Result>
{
    public async Task<Result> Handle(
        LogoutCommand request,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return Result.Failure(IdentityErrors.InvalidRefreshToken);

        return await refreshTokenService.RevokeAsync(
            request.RefreshToken.Trim(),
            ct);
    }
}
