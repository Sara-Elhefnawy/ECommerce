using ECommerce.APP.Cachings;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Carts.Commands.ClearCart;

public sealed class ClearCartHandler(
    ICartRepository repo)
    : IRequestHandler<ClearCartCommand, Result>
{
    public async Task<Result> Handle(
        ClearCartCommand request,
        CancellationToken cancellationToken)
    {
        var cart = await repo.GetOrCreateAsync(request.BuyerId, cancellationToken);

        cart.Clear();

        await repo.SaveAsync(cart, cancellationToken);

        return Result.Ok();
    }
}
