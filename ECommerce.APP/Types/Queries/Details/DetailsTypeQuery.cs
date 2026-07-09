using ECommerce.APP.Abstractions.Mediator;
using ECommerce.Domain.Common;

namespace ECommerce.APP.Types.Queries.Details;

public sealed record DetailsTypeQuery(Guid Id) : IRequest<ResultOfT<DetailsTypeResponse>>;
