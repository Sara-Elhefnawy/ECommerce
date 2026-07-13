using ECommerce.APP.Abstractions.Mediator;
using ECommerce.Domain.Common;

namespace ECommerce.APP.Features.Brands.Queries.GetById;

public sealed record GetBrandByIdQuery(Guid Id) : IRequest<ResultOfT<GetBrandByIdResponse>>;
