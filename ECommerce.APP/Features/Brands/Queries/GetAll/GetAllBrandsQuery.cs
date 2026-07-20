using ECommerce.APP.Mediator;
using ECommerce.Domain.Common;

namespace ECommerce.APP.Features.Brands.Queries.GetAll;

public sealed record GetAllBrandsQuery(int? Count) : IRequest<ResultOfT<IReadOnlyList<GetAllBrandsResponse>>>;
