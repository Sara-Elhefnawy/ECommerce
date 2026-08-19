using ECommerce.APP.Features.Inventories.Queries.GetByProductId;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Inventories.Commands.Restock;

public sealed class RestockInventoryHandler(
    IRepository<Inventory> repository,
    IUnitOfWork uow)
    : IRequestHandler<RestockInventoryCommand, ResultOfT<RestockInventoryResponse>>
{
    public async Task<ResultOfT<RestockInventoryResponse>> Handle(
        RestockInventoryCommand request,
        CancellationToken ct = default)
    {
        var inventory = await repository.FirstOrDefaultAsync(
            new GetInventoryByProductIdEntitySpecification(request.ProductId, tracking: true),
            ct);

        if (inventory is null)
            return InventoryErrors.NotFound;

        var addResult = inventory.AddStock(request.Quantity);

        if (addResult.IsFailure)
            return addResult.Error!;

        repository.Update(inventory);
        await uow.SaveChangesAsync(ct);

        return new RestockInventoryResponse(inventory.ProductId, inventory.QuantityOnHand);
    }
}
