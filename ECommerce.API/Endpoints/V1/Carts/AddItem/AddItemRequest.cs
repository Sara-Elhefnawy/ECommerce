namespace ECommerce.API.Endpoints.V1.Carts.AddItem;

public sealed record AddItemRequest(
    Guid ProductId,
    int Quantity);
