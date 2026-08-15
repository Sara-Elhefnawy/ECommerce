namespace ECommerce.API.Endpoints.V1.DeliveryMethods.Update;

public sealed record UpdateDeliveryMethodRequest(
    string Name,
    decimal Price,
    string EstimatedDeliveryTime,
    string? Description,
    bool IsAvailable);
