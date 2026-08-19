using ECommerce.APP.Features.Orders.DTOs;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Orders.Queries.GetById;

public sealed record GetOrderByIdQuery(Guid OrderId) : IRequest<ResultOfT<OrderResponse>>;
