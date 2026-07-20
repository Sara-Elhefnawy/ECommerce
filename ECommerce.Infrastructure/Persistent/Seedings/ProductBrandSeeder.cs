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

        var brands = new List<ProductBrand>();

        var brandNames = new[] { "H&M", "ZARA", "Nike", "Activ", "Mango", "Levi's" };

        foreach (var name in brandNames)
        {
            var result = ProductBrand.Create(name);

            // In development, we want to know if seed data is invalid
            if (result.IsFailure)
                throw new InvalidOperationException($"Failed to create brand '{name}': {result.Error?.Message}");

            brands.Add(result.Value);
        }

        if (brands.Count != 0)
            await dbContext.Brands.AddRangeAsync(brands, ct);
    }
}
