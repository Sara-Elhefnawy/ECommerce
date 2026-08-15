using ECommerce.APP.Specifications;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.DeliveryMethods.Commands.Reorder;

public sealed class ReorderDeliveryMethodsSpecification : Specification<DeliveryMethod>
{
    public ReorderDeliveryMethodsSpecification(IReadOnlyList<Guid> deliveryMethodIds)
    {
        Query
            .Where(x => deliveryMethodIds.Contains(x.Id));
    }
}
