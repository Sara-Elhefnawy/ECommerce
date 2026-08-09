using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.Sockets;

namespace ECommerce.Infrastructure.HealthChecks;

public sealed class BrevoHealthCheck(
    HttpClient httpClient, 
    IConfiguration configuration) 
    : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken ct = default)
    {
        var host = configuration["Email:Host"] ?? "smtp-relay.brevo.com";
        var port = configuration.GetValue("Email:Port", 587);

        try
        {
            using var client = new TcpClient();
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(3));

            // A successful TCP connect + a valid SMTP greeting (220 response)
            // confirms Brevo's SMTP relay is reachable and accepting connections —
            // that's the actual thing your app depends on, since you send email
            // via SMTP, not the REST API. No API key needed, nothing new to store.
            await client.ConnectAsync(host, port, cts.Token);

            using var stream = client.GetStream();
            using var reader = new StreamReader(stream);

            var greeting = await reader.ReadLineAsync(cts.Token);

            return greeting?.StartsWith("220") == true
                ? HealthCheckResult.Healthy($"Brevo SMTP reachable ({host}:{port})")
                : HealthCheckResult.Degraded($"Unexpected SMTP greeting: {greeting}");
        }
        catch (OperationCanceledException)
        {
            return HealthCheckResult.Degraded("Brevo SMTP health check timed out");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Degraded("Brevo SMTP unreachable", ex);
        }
    }
}
