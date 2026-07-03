using ECommerce.API.Extensions;
using ECommerce.APP.Types.Queries;

namespace ECommerce.API.Endpoints;

public static class TypeEndpoints
{
    public static void MapTypeEndpoints(this WebApplication app)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();

        app.MapGet("/api/types", async (GetAllTypesQuery query, CancellationToken ct = default) =>
        {
            logger.LogInformation("Retrieving all types from database");

            var result = await query.Execute(ct);

            logger.LogInformation("Query completed with result: {Result}", result);

            return result.ToApiResult();
        });
    }
}
