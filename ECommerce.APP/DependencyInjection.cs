using ECommerce.APP.Mediator;
using ECommerce.APP.Mediator.Behaviors;
using ECommerce.APP.Settings;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.APP;

public static class DependencyInjection
{
    // could return void but IServiceCollection return type makes it useful to chain
    public static IServiceCollection AddApp(this IServiceCollection services, IConfiguration configuration)
    {
        // AddMediatorPattern scans this assembly,
        //      finds every IRequestHandler<,> implementation and registers each
        //      against its specific handler interface — plus registers IMediator itself.
        services.AddMediatorPattern(typeof(DependencyInjection).Assembly);

        // Register all validators in the APP assembly
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.Configure<ResetPasswordSettings>(
            configuration.GetSection(ResetPasswordSettings.SectionName));

        return services;
    }
}
