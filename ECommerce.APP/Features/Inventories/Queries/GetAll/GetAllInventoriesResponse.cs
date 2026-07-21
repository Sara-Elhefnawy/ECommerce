namespace ECommerce.APP.Features.Inventories.Queries.GetAll;

public sealed record GetAllInventoriesResponse(
    Guid ProductId,
    string ProductName,
    int QuantityOnHand,
    bool InStock);
