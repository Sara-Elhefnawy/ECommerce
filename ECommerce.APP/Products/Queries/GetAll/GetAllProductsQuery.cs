using ECommerce.APP.Abstractions.Mediator;
using ECommerce.Domain.Common;

namespace ECommerce.APP.Products.Queries.GetAll;

public sealed record GetAllProductsQuery : IRequest<ResultOfT<IReadOnlyList<GetAllProductsResponse>>>;
