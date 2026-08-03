using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Users.Queries.GetUserAddresses;

public sealed record GetUserAddressesQuery
    : IRequest<ResultOfT<IReadOnlyList<UserAddressResponse>>>;
