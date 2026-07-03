using ECommerce.API.Extensions;
using ECommerce.APP.Products.Commands;
using ECommerce.APP.Products.Queries;
using ECommerce.APP.Products.Responses;
using Serilog.Context;

namespace ECommerce.API.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();

        app.MapGet("/api/products", async (GetAllProductsQuery query, CancellationToken ct) =>
        {
            logger.LogInformation("Retrieving all products from database");

            var result = await query.Execute(ct);

            logger.LogInformation("Query completed with result: {Result}", result);

            return result.ToApiResult();
        })
        .WithName("GetProducts");

        app.MapGet("/api/products/{id:guid}", async (Guid id, DetailsProductQuery query, CancellationToken ct) =>
        {
            // It's a Serilog feature, not part of your Result pattern at all.
            // Inside the using block, every log line written automatically
            // gets a ProductId field attached in the structured log output
            using (LogContext.PushProperty("ProductId", id))
            {
                var result = await query.Execute(id, ct);
                return result.ToApiResult();
            }
        })
        .WithName("GetProduct");

        app.MapPost("/api/products", async (CreateProductRequest request, CreateProductCommand command, CancellationToken ct) =>
        {
            var result = await command.Execute(request, ct);

            return result.ToApiResult();
        });
    }
}
