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
        string version)
    {
        var versionPath = version.Split('.')[0]; ;

        return app.MapGroup($"/api/v{versionPath}/{path}");
    }
}
