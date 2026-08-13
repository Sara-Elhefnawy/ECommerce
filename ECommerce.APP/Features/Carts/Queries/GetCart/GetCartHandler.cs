using ECommerce.APP.Cachings.Carts;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Carts.Queries.GetCart;

public sealed class GetCartHandler(ICartRepository repo)
    : IRequestHandler<GetCartQuery, ResultOfT<GetCartResponse>>
{
    public async Task<ResultOfT<GetCartResponse>> Handle(
        GetCartQuery request,
        CancellationToken cancellationToken)
    {
        var cart = await repo.GetOrCreateAsync(request.BuyerId, cancellationToken);

        if (cart.IsFailure)
            return ResultOfT<GetCartResponse>.Failure(cart.Error!);

        return ResultOfT<GetCartResponse>.Ok(GetCartMapper.ToResponse(cart.Value));
    }
}
