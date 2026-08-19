using ECommerce.APP.Identity;
using ECommerce.APP.Mediator;
using ECommerce.APP.Token;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Auth.Commands.RemoveRole;

public sealed class RemoveRoleCommandHandler(IIdentityService identityService)
    : IRequestHandler<RemoveRoleCommand, ResultOfT<AuthUserSnapshot>>
{
    public async Task<ResultOfT<AuthUserSnapshot>> Handle(RemoveRoleCommand request, CancellationToken ct)
    {
        var result = await identityService.RemoveRoleAsync(request.UserId, request.Role, ct);

        return result;
    }
}
