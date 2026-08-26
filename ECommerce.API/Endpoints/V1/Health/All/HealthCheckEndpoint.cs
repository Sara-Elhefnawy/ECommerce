using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.Domain.Constants;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ECommerce.API.Endpoints.V1.Health.All;

public sealed class HealthCheckEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("health", ApiVersions.V1, includeAudit: false)
            .MapGet("/", Handle)
            .WithTags("Health")
            .WithName("HealthCheck")
            .WithGroupName("v1")
            .Produces<HealthCheckResponse>(StatusCodes.Status200OK)
            .Produces<HealthCheckResponse>(StatusCodes.Status503ServiceUnavailable)
            .WithSummary("Health Check")
            .WithDescription("Reports the health of the application and its configured dependencies.")
            .RequireAuthorization(policy => policy.RequireRole(Roles.SuperAdmin));

    public static async Task<IResult> Handle(
        HealthCheckService healthCheckService,
        CancellationToken ct = default)
    {
        var report = await healthCheckService.CheckHealthAsync(ct);

        var response = new HealthCheckResponse(
            report.Status.ToString(),
            report.TotalDuration,
            report.Entries.Select(entry => new HealthCheckEntryResponse(
                entry.Key,
                entry.Value.Status.ToString(),
                entry.Value.Description,
                entry.Value.Duration,
                entry.Value.Tags.ToArray())));

        return report.Status == HealthStatus.Unhealthy
            ? Results.Json(response, statusCode: StatusCodes.Status503ServiceUnavailable)
            : Results.Ok(response);
    }
}
