using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.DeliveryMethods.Commands.Delete;

public sealed record DeleteDeliveryMethodCommand(Guid Id) : IRequest<Result>;
