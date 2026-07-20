using ECommerce.APP.Mediator;
using ECommerce.Domain.Common;

namespace ECommerce.APP.Features.Types.Queries.GetAll;

public sealed record GetAllTypesQuery(int? Count) : IRequest<ResultOfT<IReadOnlyList<GetAllTypesResponse>>>;
