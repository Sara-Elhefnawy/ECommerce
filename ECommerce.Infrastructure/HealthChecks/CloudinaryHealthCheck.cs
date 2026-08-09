using ECommerce.Domain.Abstractions.ImageCloudinary;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ECommerce.Infrastructure.HealthChecks;

public sealed class CloudinaryHealthCheck(ICloudinaryService cloudinaryService) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken ct = default)
    {
        try
        {
            // Reuses your existing service instead of talking to the
            // Cloudinary SDK directly a second way — keeps one code path
            // responsible for how the app talks to Cloudinary.
            var isHealthy = await cloudinaryService.PingAsync(ct); // whatever lightweight method it exposes

            return isHealthy
                ? HealthCheckResult.Healthy("Cloudinary reachable")
                : HealthCheckResult.Degraded("Cloudinary check failed");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Degraded("Cloudinary unreachable", ex);
        }
    }
}
