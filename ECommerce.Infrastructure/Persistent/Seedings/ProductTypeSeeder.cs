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

        var types = new List<ProductType>
        {
            ProductType.Create("Tops"),
            ProductType.Create("Bottoms"),
            ProductType.Create("Outerwear & Dresses"),
            ProductType.Create("Accessories & Footwear")
        };

        await dbContext.Types.AddRangeAsync(types, ct);
    }
}
