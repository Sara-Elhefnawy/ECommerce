using ECommerce.APP.Brands.Response;
using ECommerce.APP.Brands.Specifications;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Brands.Queries;

public sealed class GetAllBrandsQuery(IReadRepository<ProductBrand> repository)
{
    public async Task<ResultOfT<IReadOnlyList<GetAllBrandsResponse>>> Execute(CancellationToken ct = default)
    {
        var brands = await repository.ListAsync(new GetAllBrandsSpecification(), ct);

        if (brands is null || !brands.Any())
            return ProductErrors.NotFound;

        return ResultOfT<IReadOnlyList<GetAllBrandsResponse>>.Ok(brands);
    }
}
