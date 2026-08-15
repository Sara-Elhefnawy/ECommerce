using ECommerce.APP.Specifications;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.DeliveryMethods.Queries.GetById;

public sealed class GetDeliveryMethodByIdSpecification : Specification<DeliveryMethod, DeliveryMethodResponse>
{
    public GetDeliveryMethodByIdSpecification(Guid id)
    {
        Query
            .Where(dm => dm.Id == id)
            .Select(dm => new DeliveryMethodResponse(
                dm.Id,
                dm.Name,
                dm.Description,
                dm.Price,
                dm.EstimatedDeliveryTime,
                dm.IsAvailable,
                dm.DisplayOrder
                ));
    }
}
