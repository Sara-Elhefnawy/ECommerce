using ECommerce.APP.Cachings;
using ECommerce.APP.Features.Carts.Queries.GetCart;
using ECommerce.APP.Features.Inventories.Queries;
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
            return ResultOfT<GetCartResponse>.Failure(CartErrors.InvalidQuantity);

        var cart = await repo.GetOrCreateAsync(request.BuyerId, ct);

        if (request.Quantity > 0)
        {
            var inventory = await inventoryRepo.FirstOrDefaultAsync(
                new InventoryByProductIdSpecification(request.ProductId), ct);

            if (inventory is null)
                return ResultOfT<GetCartResponse>.Failure(InventoryErrors.NotFound);

            if (!inventory.HasEnough(request.Quantity))
                return ResultOfT<GetCartResponse>.Failure(InventoryErrors.NotEnoughStock);
        }

        var updateResult = cart.UpdateItemQuantity(request.ProductId, request.Quantity);

        if (updateResult.IsFailure)
            return ResultOfT<GetCartResponse>.Failure(updateResult.Error!);

        await repo.SaveAsync(cart, ct);

        return ResultOfT<GetCartResponse>.Ok(GetCartMapper.ToResponse(cart));
    }
}
