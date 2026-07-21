using ECommerce.APP.Features.Inventories.Queries.GetByProductId;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Inventories.Commands.CreateInventory;

public sealed class CreateInventoryHandler(
    IRepository<Inventory> repository,
    IUnitOfWork uow)
    : IRequestHandler<CreateInventoryCommand, ResultOfT<CreateInventoryResponse>>
{
    public async Task<ResultOfT<CreateInventoryResponse>> Handle(
        CreateInventoryCommand request,
        CancellationToken ct = default)
    {
        // Guard against creating a second Inventory row for a product that already has one
        // Inventory.Create doesn't know about existing rows,
        //      only the handler can check that via the repository.
        var existing = await repository.FirstOrDefaultAsync(
            new GetInventoryByProductIdSpecification(request.ProductId), ct);

        if (existing is not null)
            return ResultOfT<CreateInventoryResponse>.Failure(InventoryErrors.AlreadyExists);

        var createResult = Inventory.Create(request.ProductId, request.Quantity);

        if (createResult.IsFailure)
            return ResultOfT<CreateInventoryResponse>.Failure(createResult.Error!);

        repository.Add(createResult.Value);
        await uow.SaveChangesAsync(ct);

        return ResultOfT<CreateInventoryResponse>.Created(
            new CreateInventoryResponse(createResult.Value.ProductId, createResult.Value.QuantityOnHand));
    }
}
