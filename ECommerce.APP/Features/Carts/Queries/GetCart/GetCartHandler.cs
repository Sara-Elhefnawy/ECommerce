using ECommerce.APP.Cachings;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Common;

namespace ECommerce.APP.Features.Carts.Queries.GetCart;

public sealed class GetCartHandler(ICartRepository repo)
    : IRequestHandler<GetCartQuery, ResultOfT<GetCartResponse>>
{
    public async Task<ResultOfT<GetCartResponse>> Handle(
        GetCartQuery request,
        CancellationToken cancellationToken)
    {
        var cart = await repo.GetOrCreateAsync(request.BuyerId, cancellationToken);
        return ResultOfT<GetCartResponse>.Ok(GetCartMapper.ToResponse(cart));
    }
}
