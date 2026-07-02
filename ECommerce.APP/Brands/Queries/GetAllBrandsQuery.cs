using ECommerce.APP.Brands.Response;
using ECommerce.Domain.Common;

namespace ECommerce.APP.Brands.Queries;

public sealed class GetAllBrandsQuery(IBrandQueryService queryService)
{
    public async Task<ResultOfT<IReadOnlyList<GetAllBrandsResponse>>> Execute(CancellationToken ct = default)
    {
        var brands = await queryService.GetAllAsync(ct);

        if (brands is null || !brands.Any())
            return ProductErrors.NotFound;

        return ResultOfT<IReadOnlyList<GetAllBrandsResponse>>.Ok(brands);
    }
}
