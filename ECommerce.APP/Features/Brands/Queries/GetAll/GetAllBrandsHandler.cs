using ECommerce.APP.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.Brands.Queries.GetAll;

public sealed class GetAllBrandsHandler(IReadRepository<ProductBrand> repository) : 
    IRequestHandler<GetAllBrandsQuery, ResultOfT<IReadOnlyList<GetAllBrandsResponse>>>
{
    public async Task<ResultOfT<IReadOnlyList<GetAllBrandsResponse>>> Handle(
        GetAllBrandsQuery request, CancellationToken ct)
    {
        var brands = await repository.ListAsync(new GetAllBrandsSpecification(request.Count), ct);

        return ResultOfT<IReadOnlyList<GetAllBrandsResponse>>.Ok(brands);
    }
}
