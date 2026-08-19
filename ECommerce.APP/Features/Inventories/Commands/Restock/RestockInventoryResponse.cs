namespace ECommerce.APP.Features.Inventories.Commands.Restock;

public sealed record RestockInventoryResponse(Guid ProductId, int QuantityOnHand);
