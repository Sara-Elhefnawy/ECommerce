using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ECommerce.API.Endpoints.V1.Health.Ready;

public sealed class HealthCheckReadyEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("health", ApiVersions.V1, includeAudit: false)
            .MapGet("/ready", Handle)
            .WithTags("Health")
            .WithName("HealthCheckReady")
            .WithGroupName("v1")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status503ServiceUnavailable)
            .WithSummary("Readiness Health Check")
            .WithDescription("Checks whether the application is ready to receive traffic and its dependencies are available.");

    public static async Task<IResult> Handle(
        HealthCheckService healthCheckService,
        CancellationToken ct = default)
    {
        var report = await healthCheckService.CheckHealthAsync(
            check => check.Tags.Contains("ready"),
            ct);

        return report.Status == HealthStatus.Unhealthy
            ? Results.StatusCode(StatusCodes.Status503ServiceUnavailable)
            : Results.Ok();
    }
}
