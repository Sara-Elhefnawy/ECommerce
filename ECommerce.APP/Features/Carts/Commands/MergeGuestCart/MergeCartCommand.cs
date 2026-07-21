using ECommerce.APP.Features.Carts.Queries.GetCart;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Carts.Commands.MergeGuestCart;

public sealed record MergeCartCommand(Guid BuyerId, Guid AnonymousBuyerId)
    : IRequest<ResultOfT<GetCartResponse>>;
