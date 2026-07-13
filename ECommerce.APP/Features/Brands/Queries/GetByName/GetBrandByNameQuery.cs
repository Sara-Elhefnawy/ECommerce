using ECommerce.APP.Abstractions.Mediator;
using ECommerce.Domain.Common;

namespace ECommerce.APP.Features.Brands.Queries.GetByName;

public sealed record GetBrandByNameQuery(string Name) : IRequest<ResultOfT<GetBrandByNameResponse>>;
