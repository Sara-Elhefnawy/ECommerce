using ECommerce.APP.Cachings;
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

        var removeResult = cart.RemoveItem(request.ProductId);

        if (removeResult.IsFailure)
            return Result.Failure(removeResult.Error!);

        await repo.SaveAsync(cart, ct);

        return Result.Ok();
    }
}
