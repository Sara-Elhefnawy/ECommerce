namespace ECommerce.API.Endpoints.V1.DeliveryMethods.Create;

public sealed record CreateDeliveryMethodRequest(
    string Name,
    string? Description,
    decimal Price,
    string EstimatedDeliveryTime,
    bool IsAvailable
    );