using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Brands.Queries.GetAll;

public sealed record GetAllBrandsQuery(int? Count) : IRequest<ResultOfT<IReadOnlyList<GetAllBrandsResponse>>>;
