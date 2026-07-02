using ECommerce.APP.Products;
using ECommerce.APP.Products.Responses;
using ECommerce.Infrastructure.Persistent;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Queries;

public class ProductQueryService(ECommerceDbContext dbContext) : IProductQueryService
{
    public async Task<IReadOnlyList<GetAllProductsResponse>> GetAllAsync(CancellationToken ct = default)
        => await dbContext.Products
        .AsNoTracking()
        .Select(p => new GetAllProductsResponse
        (
            p.Id, 
            p.Name, 
            p.Description, 
            p.PictureUrl, 
            p.Price, 
            p.Type.Name, 
            p.Brand.Name
        ))
        .ToListAsync(ct);

    public async Task<DetailsProductResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await dbContext.Products
        .AsNoTracking()
        .Select(p => new DetailsProductResponse
        (
            p.Id,
            p.Name,
            p.Description,
            p.PictureUrl,
            p.Price,
            p.Type.Name,
            p.Brand.Name
        ))
        .FirstOrDefaultAsync(p => p.Id == id, ct);
}
