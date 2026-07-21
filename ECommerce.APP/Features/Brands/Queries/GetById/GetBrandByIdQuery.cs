using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Brands.Queries.GetById;

public sealed record GetBrandByIdQuery(Guid Id) : IRequest<ResultOfT<GetBrandByIdResponse>>;
