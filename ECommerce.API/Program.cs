using ECommerce.API;
using ECommerce.API.Endpoints;
using ECommerce.API.Serilog;
using ECommerce.APP;
using ECommerce.Infrastructure;
using ECommerce.Infrastructure.Persistent;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogLogging();

builder.Services.AddPresentation()
                .AddInfrastructure(builder.Configuration)
                .AddApp();

var app = builder.Build();

app.UseSerilogRequestLoggingConfigured();

// Activates the GlobalExceptionMiddleware registered in AddPresentation().
// Catches any unhandled exception and returns a structured ProblemDetails 500 response.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    // Runs the middleware that makes documentation available as an HTTP endpoint
    app.UseSwagger();

    app.UseSwaggerUI();
}

if (app.Environment.IsDevelopment())
{
    await using var scope = app.Services.CreateAsyncScope();
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    var dbContext = scope.ServiceProvider.GetRequiredService<ECommerceDbContext>();

    // Applies any pending EF Core migrations on startup in Development.
    await dbContext.Database.MigrateAsync();
    // Seeds initial data (brands, types, products) if the tables are empty.
    await seeder.SeedAllAsync();
}

app.MapProductEndpoints();
app.MapTypeEndpoints();
app.MapBrandEndpoints();

app.Run();
