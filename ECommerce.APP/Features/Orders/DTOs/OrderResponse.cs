namespace ECommerce.APP.Features.Orders.DTOs;

public sealed record OrderResponse(
    Guid Id,
    Guid UserId,
    string Status,
    Guid DeliveryMethodId,
    string DeliveryMethodName,
    decimal DeliveryMethodPrice,
    string DeliveryMethodEstimatedTime,
    ShippingAddressResponse ShippingAddress,
    decimal ItemsTotal,
    decimal ShippingCost,
    decimal Total,
    DateTimeOffset CreatedAt,
    IReadOnlyList<OrderItemResponse> Items
    );