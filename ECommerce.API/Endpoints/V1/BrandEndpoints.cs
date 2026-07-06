using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Result;
using ECommerce.APP.Brands.Queries;
using ECommerce.APP.Brands.Response;

namespace ECommerce.API.Endpoints.V1;

public static class BrandEndpointsV2
{
    public static void MapBrandEndpoints(this WebApplication app)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();

        var group = app.MapVersionedEndpoint("brands", ApiVersions.V1);

        group.MapGet("/", async (GetAllBrandsQuery query, HttpContext httpContext, CancellationToken ct = default) =>
        {
            logger.LogInformation("Retrieving all brands from database");

            var result = await query.Execute(ct);

            logger.LogInformation("Query completed with result: {Result}", result);

            return result.ToApiResult(httpContext);
        })
        .WithName("GetBrands")
        .WithGroupName("v1")
        .Produces<ApiResponse<IReadOnlyList<GetAllBrandsResponse>>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Get brands")
        .WithDescription("Returns all brands in DB, or 404 if list is empty");
    }
}
