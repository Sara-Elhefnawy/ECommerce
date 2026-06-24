namespace ECommerce.Infrastructure;

public static class DependencyInjection
{
    // could return void but IServiceCollection return type makes it useful to chain
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        return services;
    }
}
