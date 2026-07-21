using ECommerce.APP.Specifications;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.Inventories.Queries.GetAll;

public sealed class GetAllInventoriesSpecification : Specification<Inventory, GetAllInventoriesResponse>
{
    public GetAllInventoriesSpecification(int? count)
    {
        Query.OrderBy(i => i.ProductId);

        if (count is int value)
            Query.Take(count.Value);

        Query
            .Select(i => new GetAllInventoriesResponse
            (
                i.ProductId,
                i.Product.Name,
                i.QuantityOnHand,
                i.QuantityOnHand > 0));
    }
}
