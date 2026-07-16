using ECommerce.API;
using ECommerce.API.Extensions;
using ECommerce.API.Serilog;
using ECommerce.APP;
using ECommerce.APP.Features.Products.Queries.GetPagination.Enums;
using ECommerce.Infrastructure;
using ECommerce.Infrastructure.HealthChecks;
using ECommerce.Infrastructure.Persistent;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogLogging();

builder.Services.AddPresentation(builder.Configuration)
                .AddInfrastructure(builder.Configuration)
                .AddApp();

// Configures Swagger documentation generation
builder.Services.AddSwaggerGen(c =>
{
    // Define a Swagger document for API version 1, 2
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ECommerce API V1", Version = "v1" });
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "ECommerce API V2", Version = "v2" });

    // Tells Swagger which endpoints belong to which version
    // Show endpoints based on GroupName
    c.DocInclusionPredicate((docName, apiDesc) => apiDesc.GroupName == docName);
});

// Make System.Text.Json serialize/deserialize enums as strings
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Register ALL Health Checks
var timeout = builder.Configuration
    .GetValue<int>("HealthChecksSettings:TimeoutSeconds");

builder.Services.AddApplicationHealthChecks(
    builder.Configuration.GetConnectionString("DefaultConnection")!,
    timeout);

// Setup Authorization for protected endpoints
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
//});

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
