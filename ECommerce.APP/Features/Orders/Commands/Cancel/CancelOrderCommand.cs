using ECommerce.APP.Features.Orders.DTOs;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Orders.Commands.Cancel;

public sealed record CancelOrderCommand(Guid OrderId) : IRequest<ResultOfT<OrderResponse>>;
