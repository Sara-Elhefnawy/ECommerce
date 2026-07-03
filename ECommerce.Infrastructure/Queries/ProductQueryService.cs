using ECommerce.APP.Products;
using ECommerce.APP.Products.Responses;
using ECommerce.APP.Products.Responses.Extensions;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Persistent;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Queries;

public class ProductQueryService(ECommerceDbContext dbContext) : IProductQueryService
{
    public async Task<IReadOnlyList<GetAllProductsResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var data = await dbContext.Products
        .AsNoTracking()
        .Select(p => new
        {
            p.Id,
            p.Name,
            p.Description,
            p.PictureUrl,
            p.Price,
            TypeName = p.Type.Name,
            BrandName = p.Brand.Name
        })
        .ToListAsync(ct);

        return data
            .Select(x => x.Id.ToGetAllResponse(x.Name, x.Description, x.PictureUrl, x.Price, x.TypeName, x.BrandName))
            .ToList();
    }

    public async Task<DetailsProductResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var item = await dbContext.Products
        .AsNoTracking()
        .Where(p => p.Id == id)
        .Select(p => new
        {
            p.Id,
            p.Name,
            p.Description,
            p.PictureUrl,
            p.Price,
            TypeName = p.Type.Name,
            BrandName = p.Brand.Name
        })
        .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (item is null) return null;

        return item.Id.ToDetailsResponse(item.Name, item.Description, item.PictureUrl, item.Price, item.TypeName, item.BrandName);
    }

    public async Task<IReadOnlyList<GetAllProductsResponse>?> GetByBrandIdAsync(Guid brandId, CancellationToken ct = default)
    {
        var data = await dbContext.Products
        .AsNoTracking()
        .Where(p => p.BrandId == brandId)
        .Select(p => new
        {
            p.Id,
            p.Name,
            p.Description,
            p.PictureUrl,
            p.Price,
            TypeName = p.Type.Name,
            BrandName = p.Brand.Name
        })
        .ToListAsync(ct);
        return data
            .Select(x => x.Id.ToGetAllResponse(x.Name, x.Description, x.PictureUrl, x.Price, x.TypeName, x.BrandName))
            .ToList();
    }

    public async Task<IReadOnlyList<GetAllProductsResponse>?> GetByTypeIdAsync(Guid typeId, CancellationToken ct = default)
    {
        var data = await dbContext.Products
        .AsNoTracking()
        .Where(p => p.TypeId == typeId)
        .Select(p => new
        {
            p.Id,
            p.Name,
            p.Description,
            p.PictureUrl,
            p.Price,
            TypeName = p.Type.Name,
            BrandName = p.Brand.Name
        })
        .ToListAsync(ct);
        return data
            .Select(x => x.Id.ToGetAllResponse(x.Name, x.Description, x.PictureUrl, x.Price, x.TypeName, x.BrandName))
            .ToList();
    }

    public async Task CreateAsync(CreateProductRequest request, CancellationToken ct = default)
    {
        var product = Product.Create(
            request.Name,
            request.Description,
            request.PictureUrl,
            request.Price,
            request.TypeId,
            request.BrandId
        );

        dbContext.Products.Add(product.Value);
        await dbContext.SaveChangesAsync(ct);
    }
}
