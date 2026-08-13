using ECommerce.APP.Cachings.Carts;
using ECommerce.APP.Features.Carts.Commands.AddItemToCart.ProductLookup;
using ECommerce.APP.Features.Carts.Queries.GetCart;
using ECommerce.APP.Features.Inventories.Queries;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Carts.Commands.AddItemToCart;

public sealed class AddCartItemHandler(
    ICartRepository repo,
    IReadRepository<Product> productRepository,
    IReadRepository<Inventory> inventoryRepository)
    : IRequestHandler<AddCartItemCommand, ResultOfT<GetCartResponse>>
{
    public async Task<ResultOfT<GetCartResponse>> Handle(
        AddCartItemCommand request,
        CancellationToken ct = default)
    {
        if (request.Quantity <= 0)
            return ResultOfT<GetCartResponse>.Failure(CartErrors.InvalidQuantity);

        var product = await productRepository.FirstOrDefaultAsync(
            new ProductForCartSpecification(request.ProductId),
            ct);

        if (product is null)
            return ResultOfT<GetCartResponse>.Failure(ProductErrors.NotFound);

        var inventory = await inventoryRepository.FirstOrDefaultAsync(
            new InventoryByProductIdSpecification(request.ProductId),
            ct);

        if (inventory is null)
            return ResultOfT<GetCartResponse>.Failure(InventoryErrors.NotFound);

        var cart = await repo.GetOrCreateAsync(request.BuyerId, ct);

        if (cart.IsFailure)
            return ResultOfT<GetCartResponse>.Failure(cart.Error!);

        var existingQuantity = cart.Value
            .Items
            .FirstOrDefault(i => i.ProductId == request.ProductId)
                ?.Quantity ?? 0;

        if (!inventory.HasEnough(existingQuantity + request.Quantity))
            return ResultOfT<GetCartResponse>.Failure(
                InventoryErrors.NotEnoughStock);

        var addResult = cart.Value.AddItem(
            product.Id, product.Name, product.PictureUrl, product.Price, request.Quantity);

        if (addResult.IsFailure)
            return ResultOfT<GetCartResponse>.Failure(addResult.Error!);

        await repo.SaveAsync(cart.Value, ct);

        return ResultOfT<GetCartResponse>.Ok(GetCartMapper.ToResponse(cart.Value));
    }
}
