using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.DeliveryMethods.Commands.Create;

public sealed record CreateDeliveryMethodCommand(
    string Name,
    string? Description,
    decimal Price,
    string EstimatedDeliveryTime,
    bool IsAvailable) : IRequest<ResultOfT<DeliveryMethodResponse>>;
