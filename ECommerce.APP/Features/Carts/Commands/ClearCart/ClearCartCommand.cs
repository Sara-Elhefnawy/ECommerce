using ECommerce.APP.Mediator;
using ECommerce.Domain.Common;

namespace ECommerce.APP.Features.Carts.Commands.ClearCart;

public sealed record ClearCartCommand(Guid BuyerId) : IRequest<Result>;
