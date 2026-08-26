using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ECommerce.API.Endpoints.V1.Health.Live;

public sealed class HealthCheckLiveEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("health", ApiVersions.V1, includeAudit: false)
            .MapGet("/live", Handle)
            .WithTags("Health")
            .WithName("HealthCheckLive")
            .WithGroupName("v1")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status503ServiceUnavailable)
            .WithSummary("Liveness Health Check")
            .WithDescription("Checks whether the application process is alive.");

    public static async Task<IResult> Handle(
        HealthCheckService healthCheckService,
        CancellationToken ct = default)
    {
        var report = await healthCheckService.CheckHealthAsync(
            check => check.Tags.Contains("live"),
            ct);

        return report.Status == HealthStatus.Unhealthy
            ? Results.StatusCode(StatusCodes.Status503ServiceUnavailable)
            : Results.Ok();
    }
}
