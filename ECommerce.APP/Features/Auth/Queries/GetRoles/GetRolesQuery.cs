using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Auth.Queries.GetRoles;

public sealed record GetRolesQuery(Guid UserId) : IRequest<ResultOfT<GetRolesResponse>>;
