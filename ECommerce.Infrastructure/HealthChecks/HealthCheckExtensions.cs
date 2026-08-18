using ECommerce.Infrastructure.HealthChecks;
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
        var logsDbConnectionString = configuration.GetConnectionString("LogsDb");
        var redisConnectionString = configuration.GetConnectionString("Redis");

        var healthChecksBuilder = services.AddHealthChecks()
            // Checks if the application is running
            // Used by: Kubernetes liveness probe
            // Note: NO database or external dependencies!
            .AddCheck("self", () => HealthCheckResult.Healthy("Application is running"),
                tags: ["live"])

            // Checks if we can connect to the database
            // Used by: Kubernetes readiness probe
            .AddDbContextCheck<ECommerceDbContext>(
                name: "ecommerce-db",
                failureStatus: HealthStatus.Unhealthy, // DB is critical!
                tags: ["ready", "db"])

            .AddDbContextCheck<ECommerceIdentityDbContext>(
                name: "ecommerce-identity-db",
                failureStatus: HealthStatus.Unhealthy, // DB is critical!
                tags: ["ready", "identity-db"])

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

        // Only registered when a real connection string exists — AddNpgSql
        // validates its argument immediately at startup (unlike the custom
        // IHealthCheck classes above, which only run when /health/ready is
        // actually hit), so an empty string here throws before the app boots.
        if (!string.IsNullOrWhiteSpace(logsDbConnectionString))
        {
            healthChecksBuilder.AddNpgSql(
                connectionString: logsDbConnectionString,
                name: "postgres-logging",
                tags: ["ready", "db"]);
        }

        if (!string.IsNullOrWhiteSpace(redisConnectionString))
        {
            healthChecksBuilder.AddRedis(
                redisConnectionString: redisConnectionString,
                name: "redis",
                failureStatus: HealthStatus.Unhealthy,
                timeout: TimeSpan.FromSeconds(3),
                tags: ["ready", "cache"]);
        }

        return services;
    }
}
