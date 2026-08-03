using ECommerce.APP.Specifications;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.Users.Queries.GetUserAddresses;

public sealed class GetUserAddressesSpecification : Specification<UserAddress, UserAddressResponse>
{
    public GetUserAddressesSpecification(Guid userId)
    {
        Query
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.IsDefault)
            .ThenBy(a => a.UserId)
            .Select(a => new UserAddressResponse(
                a.Id,
                a.RecipientFirstName,
                a.RecipientLastName,
                a.PhoneNumber,
                a.Country,
                a.City,
                a.Street,
                a.PostalCode,
                a.IsDefault));
    }
}
