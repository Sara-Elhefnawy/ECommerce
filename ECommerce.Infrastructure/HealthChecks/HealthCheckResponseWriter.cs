using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace ECommerce.Infrastructure.HealthChecks;

public static class HealthCheckResponseWriter
{
    public static Task WriteResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        // Build a clean JSON response
        var result = JsonSerializer.Serialize(new
        {
            // Overall status (Healthy/Degraded/Unhealthy)
            status = report.Status.ToString(),

            // How long the entire health check took
            totalDurationMs = report.TotalDuration.TotalMilliseconds,

            // Details for each individual check
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                durationMs = e.Value.Duration.TotalMilliseconds,
                data = e.Value.Data
                // ⚠️ IMPORTANT: We deliberately do NOT include Exception details
                // This prevents leaking sensitive information
            })
        }, new JsonSerializerOptions { WriteIndented = true });

        return context.Response.WriteAsync(result);
    }
}
