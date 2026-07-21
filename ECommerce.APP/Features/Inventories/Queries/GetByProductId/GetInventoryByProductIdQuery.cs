using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Inventories.Queries.GetByProductId;

public sealed record GetInventoryByProductIdQuery(Guid ProductId) : IRequest<ResultOfT<GetInventoryByProductIdResponse>>;
