using ECommerce.Infrastructure.Identity;
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

        services.AddHttpClient<BrevoHealthCheck>();

        services.AddHealthChecks()
                    // Checks if the application is running
                    // Used by: Kubernetes liveness probe
                    // Note: NO database or external dependencies!
                    .AddCheck("self", () => HealthCheckResult.Healthy("Application is running"),
                tags: ["live"])

            .AddNpgSql(
                connectionString: configuration.GetConnectionString("LogsDb")!,
                name: "postgres-logging",
                tags: ["ready", "db"])

            // Checks if we can connect to the database
            // Used by: Kubernetes readiness probe
            // Note: Uses DbContext (EF Core) - we already have it registered
            // REQUIRES: dotnet add package Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore
            .AddDbContextCheck<ECommerceDbContext>(
                name: "ecommerce-db",
                failureStatus: HealthStatus.Unhealthy, // DB is critical!
                tags: ["ready", "db"])

            .AddDbContextCheck<ECommerceIdentityDbContext>(
                name: "ecommerce-identity-db",
                failureStatus: HealthStatus.Unhealthy, // DB is critical!
                tags: ["ready", "identity-db"])

            .AddRedis(
                redisConnectionString: configuration.GetConnectionString("Redis")!,
                name: "redis",
                // Unhealthy (not just Degraded) if Redis is unreachable — for a cache
                // this isn't fatal to the app, but you want the signal to be loud
                // while you're debugging, not buried.
                failureStatus: HealthStatus.Unhealthy,
                timeout: TimeSpan.FromSeconds(3),
                tags: ["ready", "cache"])

            .AddCheck<BrevoHealthCheck>(
                name: "brevo-email",
                // Degraded, not Unhealthy — deliberately doesn't fail /health/ready.
                // Email is a secondary capability; you don't want Kubernetes pulling
                // a pod out of rotation because a transactional email provider hiccupped.
                failureStatus: HealthStatus.Degraded,
                tags: ["ready", "email"])

            // Cloudinary and Brevo are both external HTTP APIs with no official
            // AspNetCore.Diagnostics.HealthChecks package, so a lightweight custom
            // IHealthCheck hitting their status/ping endpoint is the standard approach.
            .AddCheck<CloudinaryHealthCheck>(
                name: "cloudinary",
                failureStatus: HealthStatus.Degraded,
                tags: ["ready", "external"]);

        return services;
    }
}
