using ECommerce.Infrastructure.Persistent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ECommerce.Infrastructure.HealthChecks;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddApplicationHealthChecks(
        this IServiceCollection services,
        string connectionString,
        int timeoutSeconds)
    {
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
                tags: ["ready", "db"]);

            //// Checks if products exist in the database
            //// Why: We want to know if our app has data to serve
            //// Note: Custom check - we wrote this ourselves
            //.AddCheck<ProductsAvailabilityHealthCheck>(
            //    name: "products-availability",
            //    timeout: TimeSpan.FromSeconds(timeoutSeconds),
            //    failureStatus: HealthStatus.Degraded, // No products = degraded, not dead
            //    tags: ["ready", "business"]);

        return services;
    }
}
