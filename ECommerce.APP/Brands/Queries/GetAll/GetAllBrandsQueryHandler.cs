using ECommerce.APP.Abstractions.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Brands.Queries.GetAll;

public sealed class GetAllBrandsQueryHandler(IReadRepository<ProductBrand> repository) : 
    IRequestHandler<GetAllBrandsQuery, ResultOfT<IReadOnlyList<GetAllBrandsResponse>>>
{
    public async Task<ResultOfT<IReadOnlyList<GetAllBrandsResponse>>> Handle(
        GetAllBrandsQuery request, CancellationToken ct)
    {
        var brands = await repository.ListAsync(new GetAllBrandsSpecification(), ct);

        if (brands is null || !brands.Any())
            return ProductErrors.NotFound;

        return ResultOfT<IReadOnlyList<GetAllBrandsResponse>>.Ok(brands);
    }
}
