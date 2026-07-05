using ECommerce.API.Extensions;
using ECommerce.API.Result;
using ECommerce.APP.Brands.Queries;

namespace ECommerce.API.Endpoints;

public static class BrandEndpoints
{
    public static void MapBrandEndpoints(this WebApplication app)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();

        app.MapGet("/api/brands", async (GetAllBrandsQuery query, HttpContext httpContext, CancellationToken ct = default) =>
        {
            logger.LogInformation("Retrieving all brands from database");

            var result = await query.Execute(ct);

            logger.LogInformation("Query completed with result: {Result}", result);

            return result.ToApiResult(httpContext);
        });
    }
}
