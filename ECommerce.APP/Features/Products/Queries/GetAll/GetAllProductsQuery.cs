using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Products.Queries.GetAll;

public sealed record GetAllProductsQuery(int? Count) : IRequest<ResultOfT<IReadOnlyList<GetAllProductsResponse>>>;
