using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistent.Seedings;

public class ProductBrandSeeder(ECommerceDbContext dbContext) : IDataSeeder
{
    public int Order => 1;

    public async Task SeedAsync(CancellationToken ct = default)
    {
        if (await dbContext.Brands.AnyAsync(ct))
            return;

        var brands = new List<ProductBrand>
        {
            ProductBrand.Create("H&M"),
            ProductBrand.Create("ZARA"),
            ProductBrand.Create("Nike"),
            ProductBrand.Create("Activ"),
            ProductBrand.Create("Mango"),
            ProductBrand.Create("Levi's")
        };

        await dbContext.Brands.AddRangeAsync(brands, ct);
    }
}
