using ECommerce.APP.Abstractions.Mediator;
using ECommerce.Domain.Common;

namespace ECommerce.APP.Brands.Queries.GetAll;

public sealed record GetAllBrandsQuery : IRequest<ResultOfT<IReadOnlyList<GetAllBrandsResponse>>>;
