using ECommerce.API.Middlewares;

namespace ECommerce.API;

public static class DependencyInjection
{
    // could return void but IServiceCollection return type makes it useful to chain
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            // Runs for EVERY ProblemDetails response in the entire app —
            //      this includes MapError's 4xx responses (validation/not-found/conflict/etc.)
            //      AND GlobalExceptionMiddleware's 500s,
            //          since that middleware also writes via IProblemDetailsService under the hood.
            // This is the ONE place traceId gets attached to error responses.
            // Individual error-building code (MapError, GlobalExceptionMiddleware)
            //      no longer needs to know about traceId at all —
            // It just happens automatically for every current AND future ProblemDetails response,
            //      without anyone having to remember to add it manually each time.
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
            };
        });

        services.AddExceptionHandler<GlobalExceptionMiddleware>();

        // discovers Minimal API routes (MapGet, etc.) for OpenAPI
        services.AddEndpointsApiExplorer();

        // Registers the service that generate the Swagger / OpenAPI file
        //      that describes every API in the app
        services.AddSwaggerGen();

        return services;
    }
}
