using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistent.Seedings;

public sealed class DeliveryMethodSeeder(ECommerceDbContext dbContext) : IDataSeeder
{
    public int Order => 3;

    public async Task SeedAsync(CancellationToken ct = default)
    {
        if (await dbContext.DeliveryMethods.AnyAsync(ct))
            return;

        var methods = new[]
        {
            DeliveryMethod.Create(
                "Standard Delivery",
                5.00m,
                "3-5 business days",
                description: "Affordable ground shipping",
                displayOrder: 1).Value,

            DeliveryMethod.Create(
                "Express Delivery",
                15.00m,
                "1-2 business days",
                description: "Fast priority shipping",
                displayOrder: 2).Value,

            DeliveryMethod.Create(
                "Same Day Delivery",
                30.00m,
                "Same day",
                description: "Order before noon for same-day delivery",
                displayOrder: 3).Value
        };

        await dbContext.DeliveryMethods.AddRangeAsync(methods, ct);
    }
}
