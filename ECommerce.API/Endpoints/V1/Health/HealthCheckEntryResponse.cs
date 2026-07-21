namespace ECommerce.API.Endpoints.V1.Health;

public sealed record HealthCheckEntryResponse(
    string Name,
    string Status,
    string? Description,
    TimeSpan Duration,
    string[] Tags);
