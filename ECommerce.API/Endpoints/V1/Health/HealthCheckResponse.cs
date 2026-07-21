namespace ECommerce.API.Endpoints.V1.Health;

// Plain DTOs, not going through your Result<T>/ApiResponse<T> envelope —
// this isn't a domain operation, so wrapping it the same way would be
// misleading (there's no "failure reason" in the domain sense here).
public sealed record HealthCheckResponse(
    string Status,
    TimeSpan TotalDuration,
    IEnumerable<HealthCheckEntryResponse> Checks);
