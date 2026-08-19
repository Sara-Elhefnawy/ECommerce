using ECommerce.APP.Identity;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Users.Commands.UpdateUser;

public sealed class UpdateUserHandler(
    ICurrentUserService currentUser,
    IIdentityService identityService)
    : IRequestHandler<UpdateUserCommand, ResultOfT<UserProfileResponse>>
{
    public async Task<ResultOfT<UserProfileResponse>> Handle(
        UpdateUserCommand request,
        CancellationToken ct = default)
    {
        if (currentUser.UserId is null)
            return IdentityErrors.InvalidCredentials;

        var updateResult = await identityService.UpdateProfileAsync(
            currentUser.UserId.Value,
            request.UserDisplayName,
            ct);

        if (updateResult.IsFailure)
            return updateResult.Error!;

        var user = updateResult.Value;
        return new UserProfileResponse(user.UserId, user.Email, user.UserDisplayName);
    }
}
