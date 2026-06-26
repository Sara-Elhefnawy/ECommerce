using ECommerce.Infrastructure.Persistent;
using ECommerce.Infrastructure.Persistent.Seedings;

public sealed class DatabaseSeeder(
    ECommerceDbContext dbContext,
    IEnumerable<IDataSeeder> seeders)
{
    public async Task SeedAllAsync(CancellationToken ct = default)
    {
        foreach (var seeder in seeders.OrderBy(s => s.Order))
        {
            await seeder.SeedAsync(ct);
            await dbContext.SaveChangesAsync(ct);
        }
    }
}
