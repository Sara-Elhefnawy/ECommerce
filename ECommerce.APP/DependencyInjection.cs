using ECommerce.APP.Brands.Queries;
using ECommerce.APP.Products.Commands;
using ECommerce.APP.Products.Queries;
using ECommerce.APP.Types.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.APP;

public static class DependencyInjection
{
    // could return void but IServiceCollection return type makes it useful to chain
    public static IServiceCollection AddApp(this IServiceCollection services)
    {
        services.AddScoped<GetAllProductsQuery>();
        services.AddScoped<DetailsProductQuery>();
        services.AddScoped<GetAllBrandsQuery>();
        services.AddScoped<GetAllTypesQuery>();
        services.AddScoped<CreateProductCommand>();

        return services;
    }
}
