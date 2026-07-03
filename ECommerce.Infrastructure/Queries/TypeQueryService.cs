using ECommerce.APP.Types;
using ECommerce.APP.Types.Response;
using ECommerce.Infrastructure.Persistent;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Queries;

public sealed class TypeQueryService(ECommerceDbContext dbContext) : ITypeQueryService
{
    public async Task<IReadOnlyList<GetAllTypesResponse>> GetAllAsync(CancellationToken ct = default)
       => await dbContext.Types
            .AsNoTracking()
            .Select(b => new GetAllTypesResponse(b.Id, b.Name))
            .ToListAsync(ct);
}
