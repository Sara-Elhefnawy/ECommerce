using ECommerce.APP.Mediator;
using ECommerce.APP.Token;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Auth.Commands.AddRole;

public sealed record AddRoleCommand(Guid UserId, string Role) : IRequest<ResultOfT<AuthUserSnapshot>>;
