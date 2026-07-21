using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ECommerce.API.Endpoints.V1.Health;

public sealed class HealthCheckEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("health", ApiVersions.V1)
            .MapGet("/", Handle)
            .WithTags("Health")
            .WithName("HealthCheck")
            .WithGroupName("v1")
            .Produces<HealthCheckResponse>(StatusCodes.Status200OK)
            .Produces<HealthCheckResponse>(StatusCodes.Status503ServiceUnavailable)

            // infra/uptime monitors and admins hit this without a buyer/user token
            // If you want it locked to admins only instead, swap .AllowAnonymous() for .RequireAuthorization("AdminOnly")
            .AllowAnonymous()
            
            .WithSummary("Health Check")
            .WithDescription("Reports application health including database and Redis connectivity");

    public static async Task<IResult> Handle(
        HealthCheckService healthCheckService,
        CancellationToken ct = default)
    {
        // This runs every check you registered (self, db, redis) and
        // aggregates their statuses — Unhealthy from any single check
        // that has failureStatus: HealthStatus.Unhealthy bubbles up to
        // the overall report.Status.
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

        // Deliberately NOT always 200 — Unhealthy returns 503 so that
        // anything watching this endpoint via HTTP status (Azure's own
        // health check config, an uptime pinger, a load balancer probe)
        // actually reacts to Redis/DB being down, not just a human
        // reading the JSON body.
        return report.Status == HealthStatus.Unhealthy
            ? Results.Json(response, statusCode: StatusCodes.Status503ServiceUnavailable)
            : Results.Ok(response);
    }
}
