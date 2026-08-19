using ECommerce.APP.Specifications;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Enums;

namespace ECommerce.APP.Features.Orders.Specifications;

public abstract class GetCurrentUserOrdersSpecification : Specification<Order>
{
    protected GetCurrentUserOrdersSpecification(
        Guid userId,
        string? searchTerm)
    {
        Query.Where(o => o.UserId == userId);

        ApplySearch(searchTerm);
    }

    private void ApplySearch(string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return;

        var normalizedSearchTerm = searchTerm.Trim();

        var hasOrderId =  Guid.TryParse(normalizedSearchTerm, out var orderId);

        var hasStatus = Enum.TryParse<OrderStatus>(normalizedSearchTerm, ignoreCase: true, out var status);

        Query.Where(o =>
            (hasOrderId && o.Id == orderId) ||
            (hasStatus && o.Status == status) ||
            o.DeliveryMethodName.Contains(normalizedSearchTerm) ||
            o.Items.Any(i =>
                i.ItemOrdered.ProductName.Contains(normalizedSearchTerm)));
    }
}
