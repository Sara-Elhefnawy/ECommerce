using ECommerce.APP.Mediator;
using ECommerce.Domain.Common;

namespace ECommerce.APP.Features.Carts.Commands.RemoveItem;

public sealed record RemoveCartItemCommand(Guid BuyerId, Guid ProductId)
    : IRequest<Result>;
