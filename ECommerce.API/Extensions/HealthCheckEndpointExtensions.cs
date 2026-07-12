using ECommerce.Infrastructure.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace ECommerce.API.Extensions;

public static class HealthCheckEndpointExtensions
{
    public static WebApplication MapApplicationHealthChecks(this WebApplication app)
    {
        // Check if the app is ALIVE and running
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("live")
        });

        // Check if the app is READY to receive traffic
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready"),
            ResponseWriter = HealthCheckResponseWriter.WriteResponse // Custom JSON
        });

        // Run ALL checks (liveness + readiness + anything else)
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true, // Run everything
            ResponseWriter = HealthCheckResponseWriter.WriteResponse
        });

        app.MapHealthChecks("/health/detailed", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = HealthCheckResponseWriter.WriteResponse
        });
        //.RequireAuthorization("AdminOnly"); // 🔒 Protected!

        return app;
    }
}
