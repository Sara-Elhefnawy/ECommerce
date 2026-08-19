using ECommerce.APP.Cachings.Carts;
using ECommerce.APP.Features.Carts.Commands.AddItemToCart.ProductLookup;
using ECommerce.APP.Features.Carts.Queries.GetCart;
using ECommerce.APP.Features.Inventories.Queries;
using ECommerce.APP.Features.Inventories.Queries.GetByProductId;
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
            return CartErrors.InvalidQuantity;

        var product = await productRepository.FirstOrDefaultAsync(
            new ProductForCartSpecification(request.ProductId),
            ct);

        if (product is null)
            return ProductErrors.NotFound;

        var inventory = await inventoryRepository.FirstOrDefaultAsync(
            new GetInventoryByProductIdEntitySpecification(request.ProductId),
            ct);

        if (inventory is null)
            return InventoryErrors.NotFound;

        var cart = await repo.GetOrCreateAsync(request.BuyerId, ct);

        if (cart.IsFailure)
            return cart.Error!;

        var existingQuantity = cart.Value
            .Items
            .FirstOrDefault(i => i.ProductId == request.ProductId)
                ?.Quantity ?? 0;

        if (!inventory.HasEnough(existingQuantity + request.Quantity))
            return InventoryErrors.NotEnoughStock;

        var addResult = cart.Value.AddItem(
            product.Id, product.Name, product.PictureUrl, product.Price, request.Quantity);

        if (addResult.IsFailure)
            return addResult.Error!;

        await repo.SaveAsync(cart.Value, ct);

        return GetCartMapper.ToResponse(cart.Value);
    }
}
