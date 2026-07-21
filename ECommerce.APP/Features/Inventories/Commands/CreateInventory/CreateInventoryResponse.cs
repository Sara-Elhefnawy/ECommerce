namespace ECommerce.APP.Features.Inventories.Commands.CreateInventory;

public sealed record CreateInventoryResponse(Guid ProductId, int QuantityOnHand);
