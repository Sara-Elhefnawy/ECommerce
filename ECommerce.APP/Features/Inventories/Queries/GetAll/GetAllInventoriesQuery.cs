using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Inventories.Queries.GetAll;

public sealed record GetAllInventoriesQuery(int? Count) : IRequest<ResultOfT<IReadOnlyList<GetAllInventoriesResponse>>>;
