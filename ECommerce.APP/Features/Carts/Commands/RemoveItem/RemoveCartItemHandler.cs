using ECommerce.APP.Cachings.Carts;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Carts.Commands.RemoveItem;

public sealed class RemoveCartItemHandler
    (ICartRepository repo)
    : IRequestHandler<RemoveCartItemCommand, Result>
{
    public async Task<Result> Handle(
        RemoveCartItemCommand request,
        CancellationToken ct)
    {
        var cart = await repo.GetOrCreateAsync(request.BuyerId, ct);

        if (cart.IsFailure)
            return cart.Error!;

        var removeResult = cart.Value.RemoveItem(request.ProductId);

        if (removeResult.IsFailure)
            return removeResult.Error!;

        await repo.SaveAsync(cart.Value, ct);

        return Result.NoContent();
    }
}
