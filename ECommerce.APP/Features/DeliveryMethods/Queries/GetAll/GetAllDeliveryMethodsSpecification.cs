using ECommerce.APP.Specifications;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.DeliveryMethods.Queries.GetAll;

public sealed class GetAllDeliveryMethodsSpecification : Specification<DeliveryMethod, DeliveryMethodResponse>
{
    public GetAllDeliveryMethodsSpecification(bool availableOnly = false, string? search = null)
    {
        if (availableOnly) 
            Query.Where(dm => dm.IsAvailable);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var trim = search.Trim();

            Query.Where(dm => dm.Name.Contains(trim) ||
                            (!string.IsNullOrWhiteSpace(dm.Description)) && dm.Description.Contains(trim) ||
                            dm.EstimatedDeliveryTime.Contains(trim));
        }

        Query
            .OrderBy(m => m.DisplayOrder)
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
