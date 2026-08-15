using ECommerce.APP.Specifications;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.DeliveryMethods.Queries.GetByName;

public sealed class GetDeliveryMethodByNameSpecification : Specification<DeliveryMethod, DeliveryMethodResponse>
{
    public GetDeliveryMethodByNameSpecification(string name, Guid? excludeId = null)
    {
        Query
            .Where(x => x.Name.Equals(name.ToUpperInvariant().Trim()))
            .Select(x => new DeliveryMethodResponse(
                x.Id,
                x.Name,
                x.Description,
                x.Price,
                x.EstimatedDeliveryTime,
                x.IsAvailable,
                x.DisplayOrder
                ));

        if (excludeId.HasValue)
        {
            Query.Where(dm => dm.Id != excludeId.Value);
        }
    }
}
