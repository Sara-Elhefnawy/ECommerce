using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Result;
using ECommerce.APP.Types.Queries;
using ECommerce.APP.Types.Response;

namespace ECommerce.API.Endpoints.V1;

public static class TypeEndpoints
{
    public static void MapTypeEndpoints(this WebApplication app)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();

        var group = app.MapVersionedEndpoint("types", ApiVersions.V1);

        group.MapGet("/", async (GetAllTypesQuery query, HttpContext httpContext, CancellationToken ct = default) =>
        {
            logger.LogInformation("Retrieving all types from database");

            var result = await query.Execute(ct);

            logger.LogInformation("Query completed with result: {Result}", result);

            return result.ToApiResult(httpContext);
        })
            .WithName("GetTypes")
            .WithGroupName("v1")
            .Produces<ApiResponse<IReadOnlyList<GetAllTypesResponse>>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get types")
            .WithDescription("Returns all types in DB, or 404 if list is empty");

        group.MapGet("/{id:guid}", async (DetailsTypeQuery query, Guid id, HttpContext httpContext, CancellationToken ct = default) =>
        {
            logger.LogInformation("Retrieving type with ID {Id} from database", id);
            var result = await query.Execute(id, ct);
            logger.LogInformation("Query completed with result: {Result}", result);
            return result.ToApiResult(httpContext);
        })
            .WithName("GetTypeById")
            .WithGroupName("v1")
            .Produces<ApiResponse<DetailsTypeResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get type by ID")
            .WithDescription("Returns a type by its ID, or 404 if not found");
    }
}
