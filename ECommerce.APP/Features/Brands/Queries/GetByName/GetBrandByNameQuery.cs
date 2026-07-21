using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Brands.Queries.GetByName;

public sealed record GetBrandByNameQuery(string Name) : IRequest<ResultOfT<GetBrandByNameResponse>>;
