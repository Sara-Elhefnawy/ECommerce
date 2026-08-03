using ECommerce.APP.Identity;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Users.Queries.GetUserAddresses;

public sealed class GetUserAddressesHandler(
    ICurrentUserService currentUser,
    IReadRepository<UserAddress> addressRepository)
    : IRequestHandler<GetUserAddressesQuery, ResultOfT<IReadOnlyList<UserAddressResponse>>>
{
    public async Task<ResultOfT<IReadOnlyList<UserAddressResponse>>> Handle(
        GetUserAddressesQuery request,
        CancellationToken ct = default)
    {
        if (currentUser.UserId is null)
            return ResultOfT<IReadOnlyList<UserAddressResponse>>.Failure(IdentityErrors.InvalidCredentials);

        var addresses = await addressRepository.ListAsync(
            new GetUserAddressesSpecification(currentUser.UserId.Value),
            ct);

        return ResultOfT<IReadOnlyList<UserAddressResponse>>.Ok(addresses);
    }
}
