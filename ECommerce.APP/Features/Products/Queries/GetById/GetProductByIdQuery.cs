using ECommerce.APP.Mediator;
using ECommerce.Domain.Common;

namespace ECommerce.APP.Features.Products.Queries.GetById;

public sealed record GetProductByIdQuery(Guid Id) : IRequest<ResultOfT<GetProductByIdResponse>>;
