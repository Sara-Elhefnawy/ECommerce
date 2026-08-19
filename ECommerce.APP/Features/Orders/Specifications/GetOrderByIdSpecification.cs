using ECommerce.APP.Specifications;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.Orders.Specifications;

public sealed class GetOrderByIdSpecification : Specification<Order>
{
    public GetOrderByIdSpecification(Guid orderId, Guid userId, bool tracking = false)
    {
        var query = Query
            .Where(o => o.Id == orderId && o.UserId == userId)
            .Include(o => o.Items);

        if (tracking)
            query.AsTracking();
        else
            query.AsNoTracking();
    }
}
