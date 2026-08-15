namespace ECommerce.APP.Features.DeliveryMethods;

public sealed record DeliveryMethodResponse(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    string EstimatedDeliveryTime,
    bool IsAvailable,
    int DisplayOrder);
