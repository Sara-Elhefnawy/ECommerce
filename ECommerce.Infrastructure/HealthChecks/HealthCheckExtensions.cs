using ECommerce.Infrastructure.Persistent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ECommerce.Infrastructure.HealthChecks;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddApplicationHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")!;
        var timeoutSeconds = configuration.GetValue<int>("HealthChecksSettings:TimeoutSeconds");

        services.AddHealthChecks()
            // Checks if the application is running
            // Used by: Kubernetes liveness probe
            // Note: NO database or external dependencies!
            .AddCheck("self", () => HealthCheckResult.Healthy("Application is running"),
                tags: ["live"])

            // Checks if we can connect to the database
            // Used by: Kubernetes readiness probe
            // Note: Uses DbContext (EF Core) - we already have it registered
            // REQUIRES: dotnet add package Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore
            .AddDbContextCheck<ECommerceDbContext>(
                name: "ecommerce-db",
                failureStatus: HealthStatus.Unhealthy, // DB is critical!
                tags: ["ready", "db"])

            .AddRedis(
                redisConnectionString: configuration.GetConnectionString("Redis")!,
                name: "redis",
                // Unhealthy (not just Degraded) if Redis is unreachable — for a cache
                // this isn't fatal to the app, but you want the signal to be loud
                // while you're debugging, not buried.
                failureStatus: HealthStatus.Unhealthy,
                timeout: TimeSpan.FromSeconds(3));

        return services;
    }
}
