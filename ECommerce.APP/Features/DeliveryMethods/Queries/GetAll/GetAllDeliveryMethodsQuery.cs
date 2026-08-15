using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.DeliveryMethods.Queries.GetAll;

public sealed record GetAllDeliveryMethodsQuery(bool AvailableOnly = true, string? SearchTerm = null) : IRequest<ResultOfT<IReadOnlyList<DeliveryMethodResponse>>>;
