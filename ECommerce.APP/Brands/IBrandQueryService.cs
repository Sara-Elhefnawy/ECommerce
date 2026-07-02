using ECommerce.APP.Brands.Response;

namespace ECommerce.APP.Brands;

public interface IBrandQueryService
{
    Task<IReadOnlyList<GetAllBrandsResponse>> GetAllAsync(CancellationToken ct = default);
}
