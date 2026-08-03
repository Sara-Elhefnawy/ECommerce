using ECommerce.APP.Identity;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Users.Queries.GetUser;

public sealed class GetCurrentUserHandler(
    IIdentityService identityService,
    ICurrentUserService userService)
    : IRequestHandler<GetCurrentUserQuery, ResultOfT<UserProfileResponse>>
{
    public async Task<ResultOfT<UserProfileResponse>> Handle(GetCurrentUserQuery request, CancellationToken ct = default)
    {
        if (userService.UserId is null)
            return ResultOfT<UserProfileResponse>.Unauthorized(IdentityErrors.InvalidCredentials);

        var userResult = await identityService.GetUserByIdAsync(userService.UserId.Value, ct);

        if (userResult.IsFailure)
            return ResultOfT<UserProfileResponse>.Failure(userResult.Error!);

        return ResultOfT<UserProfileResponse>.Ok(new UserProfileResponse
        (
            userResult.Value.UserId,
            userResult.Value.Email,
            userResult.Value.UserDisplayName
        ));
    }
}
