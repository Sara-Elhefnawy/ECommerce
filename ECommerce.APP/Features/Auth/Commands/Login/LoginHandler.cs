using ECommerce.APP.Identity;
using ECommerce.APP.Mediator;
using ECommerce.APP.Token;
using ECommerce.APP.Token.RefreshTokens;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Auth.Commands.Login;

public sealed class LoginHandler(
    IIdentityService identityService,
    IJwtTokenGenerator jwtTokenGenerator,
    IRefreshTokenService refreshTokenService
    ) : IRequestHandler<LoginCommand, ResultOfT<AuthResponse>>
{
    public async Task<ResultOfT<AuthResponse>> Handle(
        LoginCommand request,
        CancellationToken ct = default)
    {
        var validateResult = await identityService.ValidateCredentialsAsync(
            request.Email,
            request.Password,
            ct);

        // Generic failure for wrong password / missing user — never reveal which.
        // EmailNotConfirmed is intentional so the client can prompt for verification.
        if (validateResult.IsFailure)
        {
            return ResultOfT<AuthResponse>.Failure(
                validateResult.Error!.Code == IdentityErrors.EmailNotConfirmed.Code
                    ? IdentityErrors.EmailNotConfirmed
                    : IdentityErrors.InvalidCredentials);
        }

        var user = validateResult.Value;
        var roles = await identityService.GetRolesAsync(user.UserId, ct);
        var accessToken = jwtTokenGenerator.GenerateToken(
            user.UserId,
            user.Email,
            user.UserDisplayName,
            roles);
        var refreshToken = await refreshTokenService.IssueAsync(user.UserId, ct);

        return ResultOfT<AuthResponse>.Ok(new AuthResponse(
            user.UserId,
            user.Email,
            user.UserDisplayName,
            accessToken.AccessToken,
            accessToken.ExpirationDate,
            refreshToken.Token,
            refreshToken.ExpiresAtUtc));
    }
}
