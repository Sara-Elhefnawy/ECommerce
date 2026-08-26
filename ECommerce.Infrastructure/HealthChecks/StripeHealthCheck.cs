using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.Http.Headers;

namespace ECommerce.Infrastructure.HealthChecks;

public sealed class StripeHealthCheck(
    IConfiguration configuration,
    IHttpClientFactory httpClientFactory) : IHealthCheck
{
    private const string StripeBalanceEndpoint =
        "https://api.stripe.com/v1/balance";

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken ct = default)
    {
        var secretKey = configuration["Stripe:SecretKey"];

        if (string.IsNullOrWhiteSpace(secretKey))
        {
            return HealthCheckResult.Degraded(
                "Stripe secret key is not configured.");
        }

        try
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                StripeBalanceEndpoint);

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", secretKey);

            var httpClient = httpClientFactory.CreateClient();

            using var response = await httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                ct);

            if (response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy(
                    "Stripe API is reachable and authentication succeeded.");
            }

            return HealthCheckResult.Degraded(
                $"Stripe API returned HTTP {(int)response.StatusCode}.");
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            return HealthCheckResult.Degraded(
                "Stripe health check timed out.");
        }
        catch (HttpRequestException ex)
        {
            return HealthCheckResult.Degraded(
                "Stripe API is unreachable.",
                ex);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Degraded(
                "Stripe health check failed.",
                ex);
        }
    }
}
