using ECommerce.APP.Abstractions.Mediator;
using ECommerce.Domain.Common;

namespace ECommerce.APP.Brands.Queries.Details;

public sealed record DetailsBrandQuery(Guid Id) : IRequest<ResultOfT<DetailsBrandResponse>>;
