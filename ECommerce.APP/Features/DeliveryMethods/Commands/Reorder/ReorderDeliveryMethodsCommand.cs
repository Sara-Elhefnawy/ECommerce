using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.DeliveryMethods.Commands.Reorder;

public sealed record ReorderDeliveryMethodsCommand(IReadOnlyList<Guid> DeliveryMethodIds) : IRequest<Result>;
