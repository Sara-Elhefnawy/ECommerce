using ECommerce.APP.Specifications;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.Inventories.Queries.GetByProductId;

public sealed class GetInventoryByProductIdSpecification : Specification<Inventory, GetInventoryByProductIdResponse>
{
    public GetInventoryByProductIdSpecification(Guid productId)
    {
        Query.Where(i => i.ProductId == productId)
             .Select(i => new GetInventoryByProductIdResponse(
                 i.ProductId,
                 i.Product.Name,
                 i.QuantityOnHand,
                 i.QuantityOnHand > 0));
    }
}
