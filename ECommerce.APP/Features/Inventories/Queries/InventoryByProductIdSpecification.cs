using ECommerce.APP.Specifications;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.Inventories.Queries;

public sealed class InventoryByProductIdSpecification
    : Specification<Inventory>
{
    public InventoryByProductIdSpecification(Guid productId)
    {
        Query.Where(i => i.ProductId == productId);
    }
}
