namespace ECommerce.APP.Features.Inventories.Queries.GetByProductId;

public sealed record GetInventoryByProductIdResponse(
    Guid ProductId,
    string ProductName,
    int QuantityOnHand,
    bool InStock);
