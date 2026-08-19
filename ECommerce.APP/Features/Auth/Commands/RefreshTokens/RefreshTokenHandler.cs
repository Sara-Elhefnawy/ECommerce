using ECommerce.APP.Identity;
using ECommerce.APP.Mediator;
using ECommerce.APP.Token;
using ECommerce.APP.Token.RefreshTokens;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Auth.Commands.RefreshTokens;

public sealed class RefreshTokenHandler(
    IRefreshTokenService refreshTokenService,   // does the actual token rotation/DB work (see RefreshTokenService.cs)
    IIdentityService identityService,            // looks up user info (email, display name, roles) by their Guid id
    IJwtTokenGenerator jwtTokenGenerator)        // builds a brand-new signed JWT access token
    : IRequestHandler<RefreshTokenCommand, ResultOfT<AuthResponse>>
{
    public async Task<ResultOfT<AuthResponse>> Handle(
        RefreshTokenCommand request,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return IdentityErrors.InvalidRefreshToken;

        // STEP 1: Rotate the refresh token.
        //  RotateAsync internally: validates the token, checks for reuse/expiry,
        //  and if everything's fine, revokes the old token and issues a new one
        var rotateResult = await refreshTokenService.RotateAsync(
            request.RefreshToken.Trim(),
            ct);

        if (rotateResult.IsFailure)
            return rotateResult.Error!;

        var refresh = rotateResult.Value;

        // STEP 2: Look up the actual user this refresh token belongs to.
        //  RotateAsync only dealt with tokens - it doesn't know or care about
        //      email, display name, roles, etc.
        var userResult = await identityService.GetUserByIdAsync(
            refresh.UserId,
            ct);

        if (userResult.IsFailure)
            return IdentityErrors.InvalidRefreshToken;

        var user = userResult.Value;

        var roles = await identityService.GetRolesAsync(user.UserId, ct);

        // STEP 3: Mint a new short-lived access token with fresh claims
        //         (id, email, display name, current roles).
        var accessToken = jwtTokenGenerator.GenerateToken(
            user.UserId,
            user.Email,
            user.UserDisplayName,
            roles);

        return new AuthResponse(
            user.UserId,
            user.Email,
            user.UserDisplayName,
            accessToken.AccessToken,
            accessToken.ExpirationDate,
            refresh.Token,
            refresh.ExpiresAtUtc);
    }
}
