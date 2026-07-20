using ECommerce.APP.Mediator;
using ECommerce.Domain.Common;

namespace ECommerce.APP.Features.Products.Queries.GetAll;

public sealed record GetAllProductsQuery(int? Count) : IRequest<ResultOfT<IReadOnlyList<GetAllProductsResponse>>>;
