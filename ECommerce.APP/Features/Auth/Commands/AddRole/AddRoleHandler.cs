using ECommerce.APP.Identity;
using ECommerce.APP.Mediator;
using ECommerce.APP.Token;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Auth.Commands.AddRole;

public sealed class AddRoleHandler(IIdentityService identityService) 
    : IRequestHandler<AddRoleCommand, ResultOfT<AuthUserSnapshot>>
{
    public async Task<ResultOfT<AuthUserSnapshot>> Handle(AddRoleCommand request, CancellationToken ct = default)
    {
        // AddRoleAsync already checks user existence (UserNotFound), role existence,
        // duplicate-role, and builds the AuthUserSnapshot on success — nothing left
        // for the handler to add, so just pass the result straight through.
        var result = await identityService.AddRoleAsync(request.UserId, request.Role, ct);

        return result;
    }
}
