using ECommerce.APP.Cachings;
using ECommerce.APP.Features.Carts.Queries.GetCart;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Common;

namespace ECommerce.APP.Features.Carts.Commands.UpdateQuantity;

public sealed class UpdateCartItemQuantityHandler
    (ICartRepository repo)
    : IRequestHandler<UpdateCartItemQuantityCommand, ResultOfT<GetCartResponse>>
{
    public async Task<ResultOfT<GetCartResponse>> Handle(
        UpdateCartItemQuantityCommand request,
        CancellationToken cancellationToken)
    {
        var cart = await repo.GetOrCreateAsync(request.BuyerId, cancellationToken);

        var updateResult = cart.UpdateItemQuantity(request.ProductId, request.Quantity);

        if (updateResult.IsFailure)
            return ResultOfT<GetCartResponse>.Failure(updateResult.Error!);

        await repo.SaveAsync(cart, cancellationToken);

        return ResultOfT<GetCartResponse>.Ok(GetCartMapper.ToResponse(cart));
    }
}
