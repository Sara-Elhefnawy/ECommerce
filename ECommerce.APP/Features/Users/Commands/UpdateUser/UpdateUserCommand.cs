using ECommerce.APP.Features.Users.Commands.UpdateUser.Common;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Users.Commands.UpdateUser;

public sealed record UpdateUserCommand(Optional<string?> UserDisplayName = default)
    : IRequest<ResultOfT<UserProfileResponse>>;
