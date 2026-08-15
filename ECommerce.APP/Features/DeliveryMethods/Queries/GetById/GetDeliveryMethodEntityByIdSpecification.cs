using ECommerce.APP.Specifications;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.DeliveryMethods.Queries.GetById;

// Returns the DeliveryMethod entity itself (not a projected DTO) —
// needed here specifically because we call .Update() on the result,
// which only exists on the entity, not on DeliveryMethodResponse.
public sealed class GetDeliveryMethodEntityByIdSpecification : Specification<DeliveryMethod>
{
    public GetDeliveryMethodEntityByIdSpecification(Guid id)
    {
        Query.Where(dm => dm.Id == id);
    }
}
