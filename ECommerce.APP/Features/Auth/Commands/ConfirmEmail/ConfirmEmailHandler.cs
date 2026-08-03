using ECommerce.APP.Email;
using ECommerce.APP.Identity;
using ECommerce.APP.Mediator;
using ECommerce.APP.Token;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Auth.Commands.ConfirmEmail;

public sealed class ConfirmEmailHandler(
    IIdentityService identityService,
    IEmailVerification emailVerification,
    IJwtTokenGenerator jwtTokenGenerator
        ) : IRequestHandler<ConfirmEmailCommand, ResultOfT<AuthResponse>>
{
    public async Task<ResultOfT<AuthResponse>> Handle(
        ConfirmEmailCommand request,
        CancellationToken ct = default)
    {
        var email = request.Email.Trim();
        var code = request.Code.Trim();

        var userResult = await identityService.GetUserByEmailAsync(email, ct);

        // Both "no such user" AND "user exists but already confirmed" now return
        //      the same generic InvalidVerificationCode
        if (userResult.IsFailure)
            return ResultOfT<AuthResponse>.Failure(IdentityErrors.InvalidVerificationCode);

        if (await identityService.IsEmailConfirmedAsync(email, ct))
            return ResultOfT<AuthResponse>.Failure(IdentityErrors.EmailAlreadyConfirmed);

        var isValid = await emailVerification.ValidateAndConsumeAsync(
            email,
            code,
            ct);

        if (!isValid)
            return ResultOfT<AuthResponse>.Failure(IdentityErrors.InvalidVerificationCode);

        var confirmResult = await identityService.ConfirmEmailAsync(email, ct);
        if (confirmResult.IsFailure)
            return ResultOfT<AuthResponse>.Failure(confirmResult.Error!);

        var user = userResult.Value;
        var roles = await identityService.GetRolesAsync(user.UserId, ct);
        var accessToken = jwtTokenGenerator.GenerateToken(
            user.UserId,
            user.Email,
            user.UserDisplayName,
            roles);

        return ResultOfT<AuthResponse>.Ok(new AuthResponse(
            user.UserId,
            user.Email,
            user.UserDisplayName,
            accessToken.AccessToken,
            accessToken.ExpirationDate));
    }
}
