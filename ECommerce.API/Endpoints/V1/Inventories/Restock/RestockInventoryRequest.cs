namespace ECommerce.API.Endpoints.V1.Inventories.Restock;

public sealed class RestockInventoryRequest
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
}
