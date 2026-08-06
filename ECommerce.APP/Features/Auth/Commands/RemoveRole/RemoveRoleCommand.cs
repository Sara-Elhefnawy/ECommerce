using ECommerce.APP.Mediator;
using ECommerce.APP.Token;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Auth.Commands.RemoveRole;

public sealed record RemoveRoleCommand(Guid UserId, string Role) : IRequest<ResultOfT<AuthUserSnapshot>>;
