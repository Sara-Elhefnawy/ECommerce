using ECommerce.APP.Brands;
using ECommerce.APP.Brands.Response;
using ECommerce.Infrastructure.Persistent;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Queries;

public sealed class BrandQueryService(ECommerceDbContext dbContext) : IBrandQueryService
{
    public async Task<IReadOnlyList<GetAllBrandsResponse>> GetAllAsync(CancellationToken ct = default)
        => await dbContext.Brands
            .AsNoTracking()
            .Select(b => new GetAllBrandsResponse(b.Id, b.Name))
            .ToListAsync(ct);
}
