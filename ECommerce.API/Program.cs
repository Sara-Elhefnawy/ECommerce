using ECommerce.API;
using ECommerce.API.Extensions;
using ECommerce.API.Serilog;
using ECommerce.APP;
using ECommerce.Infrastructure;
using ECommerce.Infrastructure.HealthChecks;
using ECommerce.Infrastructure.Persistent;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogLogging();

builder.Services.AddPresentation(builder.Configuration)
                .AddInfrastructure(builder.Configuration)
                .AddApp();

builder.Services.AddApplicationHealthChecks(
    builder.Configuration);

var app = builder.Build();

// outer layer — pushes TraceId first
app.UseTraceIdEnrichment();
// inner layer — now covered by the enrichment
app.UseSerilogRequestLoggingConfigured();

// Activates the GlobalExceptionMiddleware registered in AddPresentation().
// Catches any unhandled exception and returns a structured ProblemDetails 500 response.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    // Runs the middleware that makes documentation available as an HTTP endpoint
    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        // Point Swagger UI to both API versions
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ECommerce API V1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "ECommerce API V2");
    });
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

// Map ALL Health Check Endpoints
app.MapApplicationHealthChecks();

app.MapEndpoints();

app.Run();
