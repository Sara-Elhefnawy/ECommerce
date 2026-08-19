using ECommerce.APP.Specifications;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.Inventories.Queries.GetByProductId;

public sealed class GetInventoryByProductIdEntitySpecification : Specification<Inventory>
{
    public GetInventoryByProductIdEntitySpecification(Guid productId, bool tracking = false)
    {
        var query = Query.Where(i => i.ProductId == productId);

        if (tracking)
            query.AsTracking();
        else
            query.AsNoTracking();
    }
}
