using ECommerce.APP.Features.Inventories.Queries.GetByProductId;
using ECommerce.APP.Features.Products.Queries.GetById;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Inventories.Commands.CreateInventory;

public sealed class CreateInventoryHandler(
    IRepository<Inventory> repository,
    IReadRepository<Product> productRepository,
    IUnitOfWork uow)
    : IRequestHandler<CreateInventoryCommand, ResultOfT<CreateInventoryResponse>>
{
    public async Task<ResultOfT<CreateInventoryResponse>> Handle(
        CreateInventoryCommand request,
        CancellationToken ct = default)
    {
        var product = await productRepository.FirstOrDefaultAsync(
            new GetProductByIdSpecification(request.ProductId), ct);

        if (product is null)
            return ProductErrors.NotFound;

        var existing = await repository.AnyAsync(
            new GetInventoryByProductIdEntitySpecification(request.ProductId), ct);

        if (existing)
            return InventoryErrors.AlreadyExists;

        var createResult = Inventory.Create(request.ProductId, request.Quantity);

        if (createResult.IsFailure)
            return createResult.Error!;

        repository.Add(createResult.Value);
        await uow.SaveChangesAsync(ct);

        return ResultOfT<CreateInventoryResponse>.Created(new CreateInventoryResponse(createResult.Value.ProductId, createResult.Value.QuantityOnHand));
    }
}
