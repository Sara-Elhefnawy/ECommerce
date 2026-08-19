using ECommerce.APP.Specifications;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.Users.Queries.GetUserAddresses;

public sealed class GetUserAddressesEntitySpecification : Specification<UserAddress>
{
    public GetUserAddressesEntitySpecification(Guid addressId, Guid userId)
    {
        Query
            .Where(a => a.Id == addressId && a.UserId == userId);
    }
}
