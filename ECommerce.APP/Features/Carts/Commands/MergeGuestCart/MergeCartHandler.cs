using ECommerce.APP.Cachings;
using ECommerce.APP.Features.Carts.Queries.GetCart;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Common;
using ECommerce.Domain.Common.Errors;
using Microsoft.Extensions.Logging;

namespace ECommerce.APP.Features.Carts.Commands.MergeGuestCart;

public sealed class MergeCartHandler(
    ICartRepository repo,
    ILogger<MergeCartHandler> logger)
    : IRequestHandler<MergeCartCommand, ResultOfT<GetCartResponse>>
{
    public async Task<ResultOfT<GetCartResponse>> Handle(

        MergeCartCommand request, CancellationToken ct = default)
    {
        if (request.AnonymousBuyerId == Guid.Empty)
        {
            logger.LogWarning(
                "Cart merge failed. Guest BuyerId was empty. BuyerId={BuyerId}",
                request.BuyerId);

            return ResultOfT<GetCartResponse>.Failure(
                CartErrors.AnonymousBuyerRequired);
        }

        var anonymousCart = await repo.GetAsync(request.AnonymousBuyerId, ct);

        if (anonymousCart is null)
        {
            logger.LogWarning(
                "Cart merge failed. Guest cart not found. BuyerId={BuyerId}, GuestBuyerId={GuestBuyerId}",
                request.BuyerId,
                request.AnonymousBuyerId);

            return ResultOfT<GetCartResponse>.Failure(
                CartErrors.AnonymousCartNotFound);
        }

        if (anonymousCart.Items.Count == 0)
        {
            logger.LogWarning(
                "Cart merge failed. Guest cart was empty. BuyerId={BuyerId}, GuestBuyerId={GuestBuyerId}",
                request.BuyerId,
                request.AnonymousBuyerId);

            return ResultOfT<GetCartResponse>.Failure(
                CartErrors.AnonymousCartNotFound);
        }

        var cart = await repo.GetOrCreateAsync(request.BuyerId, ct);

        var mergeResult = cart.MergeCartFromGuestCart(anonymousCart);

        if (mergeResult.IsFailure)
            return ResultOfT<GetCartResponse>.Failure(mergeResult.Error!);

        await repo.SaveAsync(cart, ct);
        await repo.DeleteAsync(request.AnonymousBuyerId, ct);

        logger.LogInformation(
            "Cart merged successfully. BuyerId={BuyerId}, GuestBuyerId={GuestBuyerId}, ItemsMerged={ItemsMerged}",
            request.BuyerId,
            request.AnonymousBuyerId,
            anonymousCart.Items.Count);

        return ResultOfT<GetCartResponse>.Ok(GetCartMapper.ToResponse(cart));
    }
}
