using ECommerce.API.Middlewares;

namespace ECommerce.API;

public static class DependencyInjection
{
    // could return void but IServiceCollection return type makes it useful to chain
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddProblemDetails();

        services.AddExceptionHandler<GlobalExceptionMiddleware>();

        // discovers Minimal API routes (MapGet, etc.) for OpenAPI
        services.AddEndpointsApiExplorer();

        // Registers the service that generate the Swagger / OpenAPI file
        //      that describes every API in the app
        services.AddSwaggerGen();

        return services;
    }
}
