using ECommerce.APP.Abstractions.Mediator;
using ECommerce.Domain.Common;

namespace ECommerce.APP.Features.Types.Queries.GetAll;

public sealed record GetAllTypesQuery : IRequest<ResultOfT<IReadOnlyList<GetAllTypesResponse>>>;
