using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.DeliveryMethods.Queries.GetById;

public sealed record GetDeliveryMethodByIdQuery(Guid Id) : IRequest<ResultOfT<DeliveryMethodResponse>>;
