using ECommerce.APP.Features.Orders.DTOs;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Orders.Commands.Create;

public sealed record CreateOrderCommand(Guid ShippingAddressId, Guid DeliveryMethodId) : IRequest<ResultOfT<OrderResponse>>;
