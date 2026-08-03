using ECommerce.APP.Specifications;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.Users.Commands.AddUserAddersses;

// Finds all of a user's addresses currently marked as default.
// In practice this should return 0 or 1 rows if the invariant is being
// maintained correctly — but the query itself doesn't assume that, since
// enforcing "only one default" is exactly the bug this query exists to fix.
public sealed class AddUserAddressesSpecification : Specification<UserAddress>
{
    public AddUserAddressesSpecification(Guid userId)
    {
        Query.Where(a => a.UserId == userId && a.IsDefault);
    }
}
