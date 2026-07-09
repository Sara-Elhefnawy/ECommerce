using ECommerce.APP.Specifications;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Types.Queries.Details;

public sealed class DetailsTypeSpecification : Specification<ProductType, DetailsTypeResponse>
{
    public DetailsTypeSpecification(Guid id)
    {
        Query
            .Where(b => b.Id == id)
            .Select(b => new DetailsTypeResponse(
                b.Id,
                b.Name));
    }
}
