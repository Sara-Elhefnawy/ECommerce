namespace ECommerce.API.Endpoints.V1.Inventories.Create;

public sealed record CreateInventoryRequest(Guid ProductId, int Quantity);
