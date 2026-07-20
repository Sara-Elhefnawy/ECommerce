using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistent.Seedings;

public class ProductTypeSeeder(ECommerceDbContext dbContext) : IDataSeeder
{
    public int Order => 2;

    public async Task SeedAsync(CancellationToken ct = default)
    {
        if (await dbContext.Types.AnyAsync(ct))
            return;

        var types = new List<ProductType>();

        var typeNames = new[] { "Tops", "Bottoms", "Outerwear & Dresses", "Accessories & Footwear" };

        foreach (var name in typeNames)
        {
            var result = ProductType.Create(name);

            // In development, we want to know if seed data is invalid
            if (result.IsFailure)
                throw new InvalidOperationException($"Failed to create type '{name}': {result.Error?.Message}");

            types.Add(result.Value);
        }

        if (types.Count != 0)
            await dbContext.Types.AddRangeAsync(types, ct);
    }
}
