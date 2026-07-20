using ECommerce.APP.Features.Carts.Queries.GetCart;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Common;

namespace ECommerce.APP.Features.Carts.Commands.UpdateQuantity;

public sealed record UpdateCartItemQuantityCommand(Guid BuyerId, Guid ProductId, int Quantity)
    : IRequest<ResultOfT<GetCartResponse>>;
