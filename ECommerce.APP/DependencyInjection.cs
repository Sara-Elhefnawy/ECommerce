using ECommerce.APP.Abstractions.Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.APP;

public static class DependencyInjection
{
    // could return void but IServiceCollection return type makes it useful to chain
    public static IServiceCollection AddApp(this IServiceCollection services)
    {
        // AddMediatorPattern scans this assembly,
        //      finds every IRequestHandler<,> implementation and registers each
        //      against its specific handler interface — plus registers IMediator itself.
        services.AddMediatorPattern(typeof(DependencyInjection).Assembly);

        return services;
    }
}
