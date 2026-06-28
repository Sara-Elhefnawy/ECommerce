using ECommerce.Infrastructure;
using ECommerce.APP;
using ECommerce.Infrastructure.Persistent;
using Microsoft.EntityFrameworkCore;
using ECommerce.API;

var builder = WebApplication.CreateBuilder(args);

// can chain cuz method return IServiceCollection
builder.Services.AddPresentation()
                .AddInfrastructure(builder.Configuration)
                .AddApp();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    await using var scope = app.Services.CreateAsyncScope();

    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    var dbContext = scope.ServiceProvider.GetRequiredService<ECommerceDbContext>();

    await dbContext.Database.MigrateAsync();
    await seeder.SeedAllAsync();
}

app.Run();
