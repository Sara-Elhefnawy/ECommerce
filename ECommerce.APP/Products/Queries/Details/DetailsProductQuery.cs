using ECommerce.APP.Abstractions.Mediator;
using ECommerce.Domain.Common;

namespace ECommerce.APP.Products.Queries.Details;

public sealed record DetailsProductQuery(Guid Id) : IRequest<ResultOfT<DetailsProductResponse>>;
