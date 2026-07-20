namespace ECommerce.API.Endpoints.V1.Carts.UpdateQuantity;

public sealed record UpdateItemQuantityRequest(
    Guid ProductId,
    int Quantity);
