using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.DeliveryMethods.Commands.Update;

public sealed record UpdateDeliveryMethodCommand(
    Guid Id,
    string Name,
    decimal Price,
    string EstimatedDeliveryTime,
    string? Description = null,
    bool IsAvailable = true
    ) : IRequest<ResultOfT<DeliveryMethodResponse>>;
