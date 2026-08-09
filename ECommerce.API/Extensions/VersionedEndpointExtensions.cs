using ECommerce.API.Filters;

namespace ECommerce.API.Extensions;

public static class ApiVersions
{
    public const string V1 = "1.0";
    public const string V2 = "2.0";
}

public static class VersionedEndpointExtensions
{
    public static RouteGroupBuilder MapVersionedEndpoint(
        this IEndpointRouteBuilder app,
        string path,
        string version,
        bool includeAudit = true)
    {
        var versionPath = version.Split('.')[0];

        var group = app.MapGroup($"/api/v{versionPath}/{path}");

        // Health checks, and anything else not hit by real users, would otherwise
        // flood the audit log with noise
        return includeAudit
            ? group.AddEndpointFilter<AuditEndpointFilter>()
            : group;
    }
}
