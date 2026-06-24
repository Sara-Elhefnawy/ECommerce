using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.APP;

public static class DependencyInjection
{
    // could return void but IServiceCollection return type makes it useful to chain
    public static IServiceCollection AddApp(this IServiceCollection services)
    {
        return services;
    }
}
