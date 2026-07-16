using ECommerce.API.Middlewares;
using ECommerce.Domain.Abstractions.ImageCloudinary;
using ECommerce.Infrastructure.ImageCloudinary;
using FluentValidation;

namespace ECommerce.API;

public static class DependencyInjection
{
    // could return void but IServiceCollection return type makes it useful to chain
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
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

        // discovers Fast Endpoints routes (MapGet, etc.) for OpenAPI
        services.AddEndpointsApiExplorer();

        // Add Cloudinary settings
        services.Configure<CloudinarySettings>(
            configuration.GetSection("CloudinarySettings"));

        // Register Cloudinary service
        services.AddScoped<ICloudinaryService, CloudinaryService>();

        // Register all validators in the API assembly
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
