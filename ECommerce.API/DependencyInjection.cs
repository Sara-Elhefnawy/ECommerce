using ECommerce.API.Middlewares;
using ECommerce.Domain.Abstractions.ImageCloudinary;
using ECommerce.Infrastructure.ImageCloudinary;
using FluentValidation;
using Microsoft.OpenApi;
using System.Text.Json.Serialization;

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
        // services.Configure<CloudinarySettings>(...) was already correctly binding your user-secrets values to the class
        // this binding is only a safety net for the failure case
        services
            .AddOptions<CloudinarySettings>()
            .Bind(configuration.GetSection("CloudinarySettings"))
            .ValidateDataAnnotations()   // needs the [Required] attributes in CloudinarySettings
            .ValidateOnStart();

        // Register Cloudinary service
        services.AddScoped<ICloudinaryService, CloudinaryService>();

        // Register all validators in the API assembly
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // Configures Swagger documentation generation
        services.AddSwaggerGen(c =>
        {
            // Define a Swagger document for API version 1, 2
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "ECommerce API V1", Version = "v1" });
            c.SwaggerDoc("v2", new OpenApiInfo { Title = "ECommerce API V2", Version = "v2" });

            // Tells Swagger which endpoints belong to which version
            // Show endpoints based on GroupName
            c.DocInclusionPredicate((docName, apiDesc) => apiDesc.GroupName == docName);
        });

        // Make System.Text.Json serialize/deserialize enums as strings
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        return services;
    }
}
