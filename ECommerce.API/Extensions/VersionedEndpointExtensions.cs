namespace ECommerce.API.Extensions;

public static class ApiVersions
{
    public const string V1 = "1.0";
    public const string V2 = "2.0";

    // change this to update all endpoints
    public static string Current => V1;

    public static string GetVersionPath(string version)
    {
        // "1.0" -> "1", "2.0" -> "2"
        return version.Split('.')[0];
    }
}

public static class VersionedEndpointExtensions
{
    public static RouteGroupBuilder MapVersionedEndpoint(
        this WebApplication app,
        string path,
        string version)
    {
        var versionPath = ApiVersions.GetVersionPath(version);

        return app.MapGroup($"/api/v{versionPath}/{path}")
            .WithTags($"{path} V{version}");
    }
}
