using ECommerce.APP.Cachings;
using ECommerce.APP.Features.Carts.Queries;
using ECommerce.APP.Features.Carts.Queries.GetCart;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Common;
using ECommerce.Domain.Common.Errors;

namespace ECommerce.APP.Features.Carts.Commands.MergeGuestCart;

public sealed class MergeCartHandler(
    ICartRepository repo)
    : IRequestHandler<MergeCartCommand, ResultOfT<GetCartResponse>>
{
    public async Task<ResultOfT<GetCartResponse>> Handle(
        
        MergeCartCommand request, CancellationToken ct = default)
    {
        if (request.AnonymousBuyerId == Guid.Empty)
            return ResultOfT<GetCartResponse>.Failure(CartErrors.AnonymousBuyerRequired);

        var anonymousCart = await repo.GetAsync(request.AnonymousBuyerId, ct);

        if (anonymousCart is null || anonymousCart.Items.Count == 0)
            return ResultOfT<GetCartResponse>.Failure(CartErrors.AnonymousCartNotFound);

        var cart = await repo.GetOrCreateAsync(request.BuyerId, ct);

        var mergeResult = cart.MergeCartFromGuestCart(anonymousCart);

        if (mergeResult.IsFailure)
            return ResultOfT<GetCartResponse>.Failure(mergeResult.Error!);

        await repo.SaveAsync(cart, ct);
        await repo.DeleteAsync(request.AnonymousBuyerId, ct);

        return ResultOfT<GetCartResponse>.Ok(GetCartMapper.ToResponse(cart));
    }
}
