using ECommerce.APP.Features.Carts.Queries.GetCart;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Common;

namespace ECommerce.APP.Features.Carts.Commands.AddItemToCart;

public sealed record AddCartItemCommand(Guid BuyerId, Guid ProductId, int Quantity)
    : IRequest<ResultOfT<GetCartResponse>>;
