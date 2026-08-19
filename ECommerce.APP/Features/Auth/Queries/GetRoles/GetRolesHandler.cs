using ECommerce.APP.Identity;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Auth.Queries.GetRoles;

public sealed class GetRolesHandler(IIdentityService identityService)
    : IRequestHandler<GetRolesQuery, ResultOfT<GetRolesResponse>>
{
    public async Task<ResultOfT<GetRolesResponse>> Handle(GetRolesQuery request, CancellationToken ct = default)
    {
        var user = await identityService.GetUserByIdAsync(request.UserId, ct);

        if (user.IsFailure)
            return IdentityErrors.UserNotFound;

        var userRoles = await identityService.GetRolesAsync(request.UserId, ct);

        return new GetRolesResponse(
            user.Value.UserId,
            user.Value.Email,
            user.Value.UserDisplayName,
            userRoles
        );
    }
}
