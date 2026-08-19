using ECommerce.APP.Cachings.Carts;
using ECommerce.APP.Features.Carts.Queries.GetCart;
using ECommerce.APP.Features.Inventories.Queries;
using ECommerce.APP.Features.Inventories.Queries.GetByProductId;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Carts.Commands.UpdateQuantity;

public sealed class UpdateCartItemQuantityHandler(
    ICartRepository repo, 
    IReadRepository<Inventory> inventoryRepo)
    : IRequestHandler<UpdateCartItemQuantityCommand, ResultOfT<GetCartResponse>>
{
    public async Task<ResultOfT<GetCartResponse>> Handle(
        UpdateCartItemQuantityCommand request,
        CancellationToken ct = default)
    {
        if (request.Quantity < 0)
            return CartErrors.InvalidQuantity;

        var cart = await repo.GetOrCreateAsync(request.BuyerId, ct);

        if (cart.IsFailure)
            return cart.Error!;

        if (request.Quantity > 0)
        {
            var inventory = await inventoryRepo.FirstOrDefaultAsync(
                new GetInventoryByProductIdEntitySpecification(request.ProductId), ct);

            if (inventory is null)
                return InventoryErrors.NotFound;

            if (!inventory.HasEnough(request.Quantity))
                return InventoryErrors.NotEnoughStock;
        }

        var updateResult = cart.Value.UpdateItemQuantity(request.ProductId, request.Quantity);

        if (updateResult.IsFailure)
            return updateResult.Error!;

        await repo.SaveAsync(cart.Value, ct);

        return GetCartMapper.ToResponse(cart.Value);
    }
}
