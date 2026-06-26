namespace ECommerce.Infrastructure.Persistent.Seedings;

public interface IDataSeeder
{
    int Order { get; }

    // Why CancellationToken?
    //      Cuz DB seeding can take time
    //      if app crashed for any reason while the database is being seeded,
    //          the token triggers an immediate cancellation, instead of freezing forever 
    Task SeedAsync(CancellationToken ct = default);
}
