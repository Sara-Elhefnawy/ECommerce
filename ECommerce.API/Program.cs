using ECommerce.API;
using ECommerce.API.Extensions;
using ECommerce.API.Serilog;
using ECommerce.APP;
using ECommerce.Infrastructure;
using ECommerce.Infrastructure.HealthChecks;
using ECommerce.Infrastructure.Identity;
using ECommerce.Infrastructure.Persistent;
using ECommerce.Infrastructure.Persistent.Seedings;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogLogging();

builder.Services.AddPresentation(builder.Configuration)
                .AddInfrastructure(builder.Configuration)
                .AddApp(builder.Configuration);

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

// Redirects HTTP requests to HTTPS
app.UseHttpsRedirection();

// Use CORS policy for frontend application
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevCORS");
}
else
{
    app.UseCors("FrontendCORSPolicy");
}

// Runs the middleware that makes documentation available as an HTTP endpoint
app.UseSwagger();

app.UseSwaggerUI(c =>
{
    // Point Swagger UI to both API versions
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ECommerce API V1");
    c.SwaggerEndpoint("/swagger/v2/swagger.json", "ECommerce API V2");
});

await using (var scope = app.Services.CreateAsyncScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    var dbContext = scope.ServiceProvider.GetRequiredService<ECommerceDbContext>();
    var identityDbContext = scope.ServiceProvider.GetRequiredService<ECommerceIdentityDbContext>();

    await dbContext.Database.MigrateAsync();
    await identityDbContext.Database.MigrateAsync();

    await seeder.SeedAllAsync();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.MapApplicationHealthChecks();

app.MapEndpoints();

app.Run();
